# Data 数据与资产知识

## K-DATA-001 覆盖保存必须采用可回滚 Staging 事务

**状态**：Active
**优先级**：P0
**证据等级**：E3
**标签**：Persistence、Transaction、Staging、Backup、Rollback、xyassets
**适用范围**：场景保存、地图保存、托管资产覆盖、Save As、任何需要替换既有磁盘数据的操作。

**确认时间**：2026-08-02 14:10:00（UTC+08:00）
**版本**：`v0.2.21.24-rz`
**Commit**：`e0893253a4d7bf27dbcdb5a8f3d308aef9be583d`
**来源**：`docs/archive/changelog/changelog-2026-07.md`、Git Commit `feat(asset): add transactional scene asset hosting`

### 问题

直接 `Delete old → Write new` 会让第二步任何 IO/权限/校验失败变成用户数据丢失。资产根目录包含多文件时，逐文件覆盖还会产生“半新半旧”的不一致状态。

### 工程规则

替换型持久化应采用显式状态机：

```text
Plan
→ Prepare（只写 staging，不碰正式根）
→ Validate
→ Activate（旧根转 backup，新 staging 原子/准原子移入）
→ 外层正式文件保存
→ Complete（确认成功后删除 backup）
```

任何关键步骤失败都要有明确 Rollback，且“旧数据安全”优先于“临时目录绝对干净”。

### 真实历史示例

`.xyassets` 托管事务在 `v0.2.21.24-rz` 中冻结：Prepare 只写 `.Battle01.xyassets.staging-<Guid>`；Activate 若正式根已存在，先移为 backup，再激活 staging；激活第三步失败自动恢复旧目录；Complete 只在 Activated 后删除 backup；Rollback 根据 Prepared/Activated 状态恢复。

### 未来应用示例

未来实现 Map 保存：不要先删除 `BattleMap.xyassets` 再复制新资源。应该先写 `.BattleMap.xyassets.staging-*`，验证全部资源，然后备份旧根、切换新根；`.xymap` 主文件写失败时恢复旧资源根。

### 禁止做法

- 写新文件前先不可逆删除旧文件。
- 保存失败后只弹错误，不恢复已替换资源。
- `Complete` 在主文档尚未成功写盘前就删 backup。
- RelativePath 允许 `..`、盘符、UNC 或逃逸资产根。

### 验证方法

事务测试必须覆盖：新根、旧根、激活中断、重复 Activate、Complete 状态守卫、Prepared Rollback、Activated Rollback、失败后旧数据内容一致、无路径逃逸。

**关联 Incident**：INC-2026-08-02-002
**关联 Knowledge**：K-DATA-002

---

## K-DATA-002 Load 必须 Candidate→Commit，结构失败与资源失败分级

**状态**：Active
**优先级**：P0
**证据等级**：E3
**标签**：Load Transaction、Candidate、Commit、Recovery、Missing Asset
**适用范围**：场景/地图/项目文档加载、资源恢复、Schema 兼容。

**确认时间**：2026-08-02 15:30:00（UTC+08:00）
**版本**：`v0.2.21.25-rz`
**Commit**：`cafe400fff6a1dde179d011ec14ddc9dfb3a5724`
**来源**：`docs/archive/changelog/changelog-2026-07.md`

### 问题

如果加载流程边解析边修改当前 World/Catalog/Selection，当文件在后半段才发现 Schema 错误或引用非法，用户当前场景已经被部分破坏。另一方面，单个资源丢失并不等同于整个文档结构非法；若一律整场失败，会损失可恢复信息。

### 工程规则

加载必须分成 Candidate 和 Commit：

```text
Read
→ Parse / Validate
→ Resolve / Import into Candidate
→ Candidate World/Catalog/Resources
→ Commit once
```

同时区分：

- **结构失败**：Broken JSON、Unsupported Schema、重复 ID、非法引用、路径逃逸等 → 整体拒绝，当前场景完全不变。
- **资源失败**：单个 GLB Missing/Failed → 文档结构仍可成立，保留实体/AssetId/Transform/层级，使用 Placeholder，其他资源继续加载。

### 真实历史示例

`v0.2.21.25-rz` 的 `SceneDocumentLoadTransaction` 在候选阶段完成 JSON 校验、`.xyassets` 路径解析、逐资产导入和候选 World/Catalog 构建；结构错误整场打开失败且原场景完全不变。单资产 Missing/Failed 时仍保留实体、ModelAssetId、Hierarchy、Transform，并用固定 Bounds 占位；提交阶段一次性替换 World/Catalog/资源/Selection/History/Session。

### 未来应用示例

打开一个包含 100 个区域和 20 个纹理的地图：若第 19 个纹理文件丢失，地图结构应仍可打开并显示缺失占位；但若区域 ID 重复导致引用语义不确定，则整个 Candidate 应拒绝，用户正在编辑的旧地图不能被清空。

### 禁止做法

- Load 开头先 `CurrentWorld.Clear()`。
- 单个资源 IOException 直接判整份文档无效。
- 结构错误后尝试“尽量继续”并提交半合法状态。
- 加载失败时顺手清 Selection/History。

### 验证方法

自动测试至少断言：各种结构错误后旧 World/Catalog/Selection/History 完全相同；Missing/Corrupt 单资源保留实体；Ready 资源正常；最终 Commit 后 Dirty=false。

**关联 Incident**：INC-2026-08-02-003
**关联 Knowledge**：K-DATA-001

---

## K-ASSET-001 数据归一化或 Bake 后必须同步归一化相关元数据

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：GLB、BaseVertex、Normalization、Bake、Metadata
**适用范围**：模型导入、索引转换、Transform Bake、坐标系转换、离线资源处理。

**确认时间**：2026-08-02 12:45:00（UTC+08:00）
**版本**：`v0.2.21.23-fix`
**Commit**：`a9c1ec6c302dce5efec2215931eafb58eb9b4f75`
**来源**：`docs/archive/changelog/changelog-2026-07.md`

### 问题

当数据已经把某个偏移/变换“烘焙”进最终值，但相关元数据仍保留旧偏移，消费方就会再次应用同一变换，产生 double-apply。

### 真实历史示例

`StaticModelBuilder.AddPrimitive` 已把局部索引归一化为全局索引：

```text
globalIndex = localIndex + baseVertex
```

但 Primitive 仍保存非零 `BaseVertex`。真实多 Primitive GLB `german_ss_soldier_mp40.glb` 导入得到 211,517 vertices / 926,148 indices 后，Vulkan 资源创建失败：`non-zero BaseVertex not supported`。测试样本此前恰好只有单 Primitive 或 BaseVertex=0，没有覆盖组合。修复是在索引归一化后把记录的 `BaseVertex = 0`。

### 工程规则

任何 Bake / Flatten / Normalize 都要列出“数据”和“描述数据的元数据”两张表。只要原语义已经吸收到最终数据中，描述旧变换的元数据必须同步归零、移除或改语义。

### 未来应用示例

若导入器执行：

```text
worldVertex = NodeTransform * localVertex
```

把 Node Transform 烘焙进顶点，那么下游 Render Primitive 就不能再携带同一个 NodeTransform 并再次乘一次。坐标系转换、材质合并、骨骼预变换同理。

### 禁止做法

- 只看输出顶点是否“肉眼合理”，不审元数据。
- 测试只用单 Primitive / 零 offset 的最简单资产。
- 为通过 Vulkan Validator 直接放宽 Validator 接受非零 BaseVertex，而不判断数据是否已经全局化。

### 验证方法

测试必须覆盖：多 Primitive、三 Primitive、非零 BaseVertex、无索引、越界/溢出；Normalize 后资源应能通过 Vulkan Validator，并断言相关元数据已规范化。

**关联 Incident**：INC-2026-08-02-001
**关联 Knowledge**：K-ASSET-002

---

## K-ASSET-002 确定性资源创建失败必须按 Key+Revision 负缓存

**状态**：Active
**优先级**：P1
**证据等级**：E2
**标签**：GPU Resource、Failure Cache、Revision、Log Storm、Retry
**适用范围**：Vulkan 资源创建、Shader 编译、资产解析、任何帧循环中可重复触发的确定性失败。

**确认时间**：2026-08-02 12:45:00（UTC+08:00）
**版本**：`v0.2.21.23-fix`
**Commit**：`a9c1ec6c302dce5efec2215931eafb58eb9b4f75`
**来源**：`docs/archive/changelog/changelog-2026-07.md`

### 问题

确定性输入没有变化时，某个资源创建失败几乎必然会再次失败。如果 RenderProjection 每帧都重新尝试，就会造成重复 CPU/GPU 工作与日志风暴，掩盖真正错误。

### 真实历史示例

BaseVertex 问题下，`VulkanStaticModelCache.Get` 创建失败后没有失败记录，后续每次 RenderProjection 更新都会再次创建并重复写错误。修复新增 `VulkanStaticModelFailureTracker`，按 `RenderStaticModelKey + Revision` 记录失败；相同 Key+Revision 不再创建、不再刷日志，Revision 改变或重新导入后才允许重试。

### 工程规则

对“输入不变则结果确定”的创建任务使用负缓存：

```text
FailureKey = ResourceIdentity + Revision
```

相同 FailureKey 再次请求直接返回已知失败；只有输入 Revision、设备环境或其它决定性条件改变时才解除负缓存。

### 未来应用示例

Shader Source Revision=42 编译失败后，不应每帧重新调用 glslc 并刷 500 条错误。记录 `(ShaderKey, Revision=42)` 失败；用户修改源码进入 Revision=43 后才重新编译。

### 禁止做法

- 单纯“日志去重”，但仍每帧做昂贵创建。
- 失败永不重试，不带 Revision。
- Revision 改变后仍错误复用旧失败状态。

### 验证方法

测试至少覆盖：同 Key+Revision 只尝试一次；Revision 改变可重试；Retain/Clear 清理无引用失败项；成功后不被旧失败记录阻断。

**关联 Incident**：INC-2026-08-02-001
**关联 Knowledge**：K-ASSET-001、K-PERF-001
