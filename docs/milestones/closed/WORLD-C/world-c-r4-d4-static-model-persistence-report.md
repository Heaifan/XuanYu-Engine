# WORLD-C-R4-D4：静态模型持久化完整闭环报告

版本：`v0.2.21.25-rz`
状态：自动验证 PASS；真机验收待用户执行（本报告不含真机结论）

## 一、范围与目标

一次完成 D4 全部剩余工作（不再拆 I2/I3/I4）：

```text
导入 GLB → StaticModel 实体 → 保存/另存为 → GLB 复制进入 .xyassets
→ .xyscene 写入 Schema v3 → 新建/关闭场景 → 重新打开
→ 恢复资产、实体与 Transform → 缺失/损坏资源保留实体 + 固定 Bounds 占位
→ 一次明确弹窗反馈
```

**命名规范已冻结**:R/I/F 定义写入宪法,R 每阶段最多一次、I 为可选实施切片、F 仅用于验收失败修复。`D4-I1` 托管事务内核为已批准切片并作为既有基础复用;D4 剩余功能本轮一次完成。

## 二、宪法修订

`docs/玄域引擎_AI开发宪法.md` 二十九章新增:

- **3.1 I:Implementation,可选实施切片**——不是强制阶段;只有同时满足真实独立技术边界、单轮显著增加风险、可独立验证、计划/用户批准才允许使用;禁止默认拆 I1/I2/I3。
- **3.2 防止过度拆分**——不得把管理编号当成果;自动测试/架构边界/回滚能力足以控险时应同一开发轮完成同一 D。

`AI_DEVELOPMENT_RULES.md` / `dev-rules.md` 无冲突规则,未改。

## 三、命名统一(D4-R1 → D4-I1)

- 报告文件:`docs/world-c-r4-d4-i1-hosted-assets-report.md`(git mv)。
- changelog、file-tree、测试注释同步替换;`commit e089325` 与历史提交不重写。

## 四、Schema v3(D0 合同为准)

```json
assets: [ { "assetId", "kind": "ModelGltf", "relativePath": "models/<id>/source.glb", "displayName", "importerVersion" } ]
entities: [ { ..., "modelAssetId" } ]
```

- 资源与实体引用分离;同一 AssetId 多个实体共享。
- 不得写入:外部 SourcePath、RenderKey、顶点/索引、GPU 数据、BaseVertex、异常堆栈。
- 保存固定写 v3;加载接受 v1/v2/v3;v1/v2 无 Assets 合法、无 ModelAssetId 合法。
- 校验:AssetId 合法且不重复、Kind 仅 ModelGltf、RelativePath 经 `SceneAssetPathPolicy`(禁 `..`/绝对路径/逃逸);StaticModel 必须有 ModelAssetId 且引用存在 Asset;新增错误码 `DuplicateAssetId / UnknownAssetKind / InvalidAssetPath / MissingEntityAssetId / UnknownEntityAssetId`。

## 五、保存事务

`SceneDocumentSaveTransaction`:候选构建 → `SceneAssetHostingPlanner` → `Hosting Prepare/Activate` → 原子写 `.xyscene`(v3+Assets)→ `Hosting Complete`;写入失败 `Rollback` 恢复旧目录。StaticModel 无 Catalog 绑定 → `MissingStaticModelBinding` 保存失败。保存成功 `Catalog.RebindSourcePaths` 改绑到 `.xyassets` 内绝对路径(AssetId/Key/GPU 不变、Selection/History 不变、Dirty=false)。

- 重复保存:SourcePath 已托管,经 staging 替换正式 assetRoot;禁止从已移动目录读源;无 staging/backup 残留。
- 另存为:Battle01 完全不变、Battle02 独立资产副本、Session/Catalog 切换、Dirty=false;取消不创建 staging/不改状态。

## 六、加载事务

`SceneDocumentLoadTransaction` 候选/提交两阶段:

- 候选(只读):读 JSON → 校验 → 解析 `.xyassets` 相对路径 → 逐 Asset 导入 → 候选 World/Catalog/资源;不触碰当前 World/Catalog/Selection/History/Dirty/Session。
- 结构错误(BrokenJson/UnsupportedSchema/重复 AssetId/未知 Kind/不安全路径/StaticModel 缺 ModelAssetId/未知引用/层级错误):整场打开失败,当前场景完全不变。
- 单资源 Missing/Failed:D0 冻结语义——不整场失败、不删实体、不丢 ModelAssetId;实体/层级/检查器保留、固定占位 Bounds、其余 Ready 资产正常;提交阶段一次性替换 World/Catalog/资源/清空 Selection/History/Session、Dirty=false。
- 缺失资源再保存:保留资产记录与 RelativePath,不生成缺失文件,不因单资产缺失删除整场。

## 七、错误弹窗

- `IEditorDialogService`(ShowErrorAsync/ShowWarningAsync)+ UiWin 实现(`UiWin.Dialogs.cs`,复用 UnsavedDialog 窗口风格,无第三方包);UiVm 默认 `NullEditorDialogService`,App 组合根传入 UiWin。
- 导入 GLB 失败 →「导入 GLB 失败」一次;场景结构打开失败 →「打开场景失败」一次;部分资源缺失 →「场景已打开，但部分资源不可用」一次汇总。
- 用户取消选择、相机、投影更新、GPU 重试、Swapchain 重建不弹窗;Core/World/Render.Vulkan 不直接弹窗。

## 八、验证(全部真实执行)

| 门禁 | 结果 |
|---|---|
| 串行 build 10 项目 | 0 warning / 0 error |
| Core Tests | 145/145 PASS |
| World Tests | 303/303 PASS(D4 新增 54) |
| D4 聚焦 ×3 | 54/54 × 3 全过 |
| `scripts/arch-a-guard.ps1` | PASS |
| glslc scene.vert / scene.frag | PASS |
| `git diff --check` | PASS |
| 守卫口径 5+100 | 0 超线 |
| D4-R1 标签残留 | 零残留 |
| staging/backup 残留 | 无 |

## 九、测试清单(`XuanYu.World.Tests/Assets/`)

- SaveTransactionTests:单模型 v3 资产与托管文件、双模型双资产、改绑、无绑定保存失败。
- SaveAsTests:重复保存无残留、另存为独立根、源场景文件不变。
- LoadTransactionTests:单/双模型往返、缺失保留实体、损坏保留实体。
- LoadStructureErrorTests:非法 JSON、UnsupportedSchema、缺 ModelAssetId、重复 AssetId。
- SchemaCompatibilityTests:v1/v2/v3 加载、空资产 v3、普通实体无 ModelAssetId 合法、未知 Kind/不安全路径/未知引用拒绝。
- DialogTests(Fake Dialog):损坏导入 1 次、成功 0 次、非法打开 1 次、缺失汇总 1 次。

既有测试更新:`WorldCR2DocumentTests` 保存断言 v2→v3、schema 3→4 拒绝(行为变化,非缺陷)。

## 十、当前状态

```text
开发宪法 R/I/F 规范：已修订并落库
WORLD-C-R4-D4 自动验证：PASS
WORLD-C-R4-D4-A1：等待真机验收
WORLD-C-R4：IN PROGRESS
```

不声明 D4 COMPLETE / R4 CLOSED / WORLD-C CLOSED;不再创建 D4-I2/I3/I4。
