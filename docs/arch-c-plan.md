# ARCH-C-Plan：真实场景编辑交互闭环规划

版本：v0.2.17.19-rz
日期：2026-07-19
类型：规划文档
范围：纯规划与架构冻结，不实现 Picking、Gizmo、Transform、Undo 或场景运行时代码。

## 1. 文档目的

本文件冻结 ARCH-C 的所有权、数据流、实现顺序和验收标准，约束后续 `ARCH-C-R1` 到 `ARCH-C-R8` 的开发。ARCH-C 的目标不是一次性建设完整编辑器，而是先完成真实场景对象从点击、选择、Gizmo 命中、拖动预览、提交、取消到 Undo 的最小可信闭环。

## 2. 当前前置条件

- 当前分支：`fix/RZ-VK3-A-surface-contract`
- 当前 HEAD：`999b8d4e4d12f43c7b4002a691f26fe6d3251e32`
- 上游分支：`origin/fix/RZ-VK3-A-surface-contract`
- ahead / behind：`0 / 0`
- 执行前工作区：干净
- ARCH-A：已建立 UI 不直接依赖 Vulkan/Silk、唯一 App 启动入口、版本一致性与结构守卫。
- ARCH-B：已建立 Selection、Tool、Interaction 的最小 Owner 与 Begin / Preview / Commit / Cancel 输入事务闭环。

当前 Selection / Input 实际状态：

- `EditorStateOwner` 是当前选择状态、工具状态和交互事务状态的唯一写入入口。
- `EditorSelectionSnapshot` 已包含稳定 `SelectionKey`、标题、类型与路径，但仍是编辑器 UI 项选择，不是正式场景实体身份。
- `EditorInteractionSnapshot` 已包含 `SessionId`、OwnerTool、StartSnapshot、Preview 与 Pointer 快照；它验证输入事务，但不保存真实 Transform。
- `VulkanNativeHost.Pointer` 已能把真实 Win32 子窗口 Pointer 事件接入既有交互事务，不承担 Picking 或 Gizmo 判断。

## 3. ARCH-C 定义

ARCH-C 是“真实场景编辑交互闭环”阶段。它负责把 ARCH-B 的输入事务与状态 Owner 扩展到真实场景对象，但仍保持 Vulkan 生命周期、窗口宿主、Swapchain、Present、Resize 和 UI 布局稳定。

最终闭环：

```text
点击真实对象
-> 统一 Selection
-> Hierarchy / Inspector / Viewport Highlight 同步
-> Move Gizmo 显示
-> 命中 X/Y/Z 轴
-> Transform Preview
-> MouseUp Commit 或 Escape Cancel
-> 最小 Undo 恢复
```

## 4. 本阶段目标

- 建立单场景、单测试对象、单选、世界坐标 Position Transform 的最小模型。
- 明确 Transform 的 `CommittedTransform`、`TransformStartSnapshot`、`PreviewTransform` 三层状态。
- 明确 Picking 首版使用 CPU Ray-AABB，不使用 GPU Picking。
- 明确 Picking 请求与结果的 Session、RequestSequence、ViewportRevision、SceneSpatialRevision 过期保护。
- 明确 Selection 仍由统一命令链进入 `EditorStateOwner`，所有视图只消费快照。
- 明确 Move Gizmo 首版只支持世界坐标 X/Y/Z 轴位置移动。
- 明确一次拖动只产生一次正式 Transform 修改、一次 Undo 记录和一次 Commit 日志。
- 明确 R1 到 R8 每轮都可以独立真机验收。

## 5. 明确不做

ARCH-C 首轮不做多选、父子 Transform、旋转、缩放、局部坐标轴、Prefab、ECS、通用组件系统、场景持久化、GPU Picking、精确网格 Picking、完整世界分区、通用命令总线、UI 布局重做或 Vulkan 生命周期修改。发现相关问题时，只记录到风险清单或后续阶段接口，不借规划轮修复运行时代码。

## 5.1 长期空间查询原则

ARCH-C-R2 之后，Picking 不得再以“当前实体少”为理由把全实体线性扫描写成正式主路径。正式主链必须通过渲染后端无关的空间查询服务访问增量维护的空间索引，再对候选执行 Ray-AABB 精确检测。

`ARCH-C-R2-C` 已封版后，Vulkan 渲染与未来 Picking 已确认共用同一 `CameraState / ViewportState / ViewProjectionState` 空间事实。R2 后续顺序调整为：先进入动态空间索引，再进入 Ray-AABB / 最近命中与真实 Picking。禁止绕过空间索引直接把 `Pointer -> WorldRay -> 全实体扫描` 串成正式主链。

禁止：

```text
GetAllEntities -> foreach 全场景 -> Ray-AABB
每次点击 -> 临时重建空间索引 -> 查询 -> 丢弃
Picking -> 读取 Render.Vulkan / Vk* / SwapchainGeneration
```

正确方向：

```text
SceneStateOwner 场景事实
-> ViewportState / CameraState / ViewProjectionState 统一空间事实
-> Renderer 正式消费同一 ViewProjection
-> Spatial Query Index 增量维护
-> Ray Query
-> 索引裁剪候选
-> Ray-AABB
-> 最近 EntityKey
```

第一版空间索引采用动态 AABB 树 / Dynamic BVH 类结构作为最小正确实现。它不承诺所有查询严格 `O(log N)`，但必须在架构上裁剪候选，而不是把全场景扫描设计为默认查询模型。

## 6. 模块边界

| 模块 | ARCH-C 职责 | 禁止事项 |
| --- | --- | --- |
| Scene Model | 持有实体身份、AABB、正式 Transform | 不承担 Vulkan 资源和 UI 控件职责 |
| EditorStateOwner | 统一接收 Selection / Tool / Interaction / Transform 命令 | 不直接依赖 Avalonia 控件、Win32 HWND 或 Vulkan 对象 |
| Viewport Input | 提供 Pointer 坐标、Session 和 ViewportRevision | 不直接修改场景 Transform |
| Picking | 把屏幕坐标转换为命中实体候选 | 不写 Selection，不写 Transform，不读取 Vulkan 内部数据 |
| Spatial Query | 增量维护空间索引、裁剪候选、执行空间查询 | 不持有 UI / Avalonia / Win32 / Vulkan 对象 |
| Selection | 发布唯一选择快照 | 不复制多个“当前选中对象” |
| Gizmo | 消费选择和相机矩阵生成轴命中与拖动意图 | 不拥有正式 Transform |
| Renderer | 消费场景/预览快照绘制 | 不决定选择、提交或 Undo |
| Undo | 记录正式提交的 Before / After | 不记录高频 Preview |

## 7. 状态所有权

ARCH-C 必须区分三类 Transform 状态：

```text
CommittedTransform
TransformStartSnapshot
PreviewTransform
```

`CommittedTransform` 是正式场景事实，只允许 Commit、Undo、Redo 等正式命令修改。`TransformStartSnapshot` 在拖动 Begin 时保存，用于 Escape、异常取消和 Undo Before。`PreviewTransform` 是拖动过程中的临时显示结果，可以高频覆盖，不生成 Undo，不直接污染正式场景状态。

禁止链路：

```text
PointerMove -> 直接修改正式 Transform -> Escape 再反推原位置
```

正确链路：

```text
Begin -> 保存 StartSnapshot -> Preview 更新临时状态
Commit -> 写入 CommittedTransform
Cancel -> 丢弃 Preview 并恢复 StartSnapshot
```

## 8. 数据流

R2-C 已封版的渲染接入统一空间事实数据流：

```text
SceneStateOwner 场景事实
-> CameraState / ViewportState
-> ViewProjectionState
-> Renderer 消费同一观察事实
-> 屏幕上的真实位置
```

R2-C 真机验收确认黄色三角形已经通过这条链渲染，场景 `Position`、默认相机、ViewProjection、Vulkan Shader 与未来 WorldRay 使用同一套方向契约。当前默认相机下世界 `+X` 映射到屏幕左侧，属于已冻结约定，不是渲染方向 Bug。

R2-D 已补全的长期空间索引数据流：

```text
SceneStateOwner 场景事实
-> SpatialBounds
-> SpatialIndexOwner 增量维护
-> DynamicAabbTree 内部裁剪
-> 候选对象查询
```

R2-D-R1 已确认 `SceneStateOwner` 初始化时登记 EntityId(1)，CommittedTransform 改变时同步更新同一个 EntityKey 的空间记录，重复 Position 不产生无意义 SpatialRevision。R2-D 只负责区域 / WorldRay 候选范围裁剪和结构性统计，不负责实体级最终 Ray-AABB、最近命中、Selection、Gizmo 或 Undo。

R2-E 建立实体级 AABB 精确命中数据流：

```text
WorldRay
-> SpatialIndexOwner Ray Candidate Query
-> 候选 SpatialBounds
-> RayAabbIntersection Narrow Phase
-> 最近 SpatialRaycastHit
```

R2-E 允许对 Broad Phase 候选集合执行 O(k) 精确检测，禁止绕过空间索引对全场景 O(N) 扫描。命中结果携带 EntityKey、HitDistance、HitPoint 和 SpatialRevision；等距时按稳定 EntityKey 顺序裁决。结果发布前必须完成 `Start Revision -> Broad Phase -> Verify -> Narrow Phase -> Final Verify -> Publish`，任何 Narrow Phase 期间发生的 SpatialRevision 变化都不得发布旧 HitResult。

当前 `DynamicAabbTree` 叶节点 Broad Bounds 与真实 `SpatialBounds.WorldBounds` 相同，尚未引入 Fat AABB 或更粗代理盒；因此生产链路下 Broad 叶候选与 Narrow AABB 使用同一盒体。未来一旦引入 Fat AABB、宽松代理 Bounds、复杂 Shape 或 Mesh Picking，必须先补真实 false-positive 集成测试，证明 `CandidateCount >= 1` 且 `ActualHit = 0` 时最终返回 NoHit，之后才允许封版。
R2-F 建立真实 Pointer Picking 最小闭环：`Avalonia / Native PointerPressed -> ViewportState -> WorldRayFactory -> SceneStateOwner.RaycastSpatial -> EntityKey / NoHit`。

R3 将 Picking 结果接入既有 ARCH-B Selection 命令链。`EditorStateOwner` 仍是唯一 Selection 事实所有者；Tree 的 `SelectedItem` 只是 `EditorSelectionSnapshot.SelectionKey` 的 UI 投影，Inspector 直接绑定同一 Snapshot。命中实体提交 `SelectEditorItemCommand`，NoHit 提交 `ClearEditorSelectionCommand`，重复选择由 Owner 幂等拒绝，不增加 Selection Revision。R3 不实现 Gizmo、Transform、Undo 或 Vulkan 高亮扩展。

```text
Pointer 屏幕坐标
-> 视口局部坐标
-> ViewportState / CameraState / ViewProjectionState
-> WorldRayFactory 生成世界射线
-> Spatial Query Index 裁剪候选
-> CPU Ray-AABB
-> PickingResult
-> SelectionCommand
-> EditorStateOwner
-> SelectionSnapshot
-> Hierarchy / Inspector / Viewport / Gizmo
```

Transform 拖动数据流：

```text
GizmoAxisHit
-> BeginTransformSession
-> TransformStartSnapshot
-> PointerMove 世界射线
-> 拖动平面求交
-> 投影到选中世界轴
-> PreviewTransform
-> MouseUp Commit 或 Escape Cancel
```

## 9. Picking 契约

首版 Picking 采用 `Spatial Query Index + CPU Ray Picking + Ray-AABB`。调用方依赖空间查询服务，不绑定具体树实现。

请求至少携带：

```text
InputSessionId
RequestSequence
ViewportRevision
SceneSpatialRevision
PointerPosition
QueryMask
```

结果至少携带：

```text
InputSessionId
RequestSequence
ViewportRevision
SceneSpatialRevision
EntityKey
HitDistance
HitPosition
VisitedNodeCount
CandidateCount
```

应用结果前必须验证 Session 仍有效、RequestSequence 未过期、ViewportRevision 与 SceneSpatialRevision 未变化且实体仍存在。过期 Picking 结果不得覆盖更新的 Selection。

## 10. Selection 契约

三种入口必须汇入同一条命令链：

```text
Viewport Picking
Hierarchy 点击
程序命令选择
-> SelectionCommand
-> EditorStateOwner
-> SelectionSnapshot
```

Hierarchy、Inspector、Viewport Selection Highlight、Move Gizmo 和状态栏只能消费 `SelectionSnapshot`，禁止各自持有独立的“当前选中对象”。

## 11. Transform 编辑事务

Transform Session 必须携带 SessionId、EntityKey、Axis、StartPointer、CurrentPointer、TransformStartSnapshot、LastValidPreview、ViewportRevision 和 CancelReason。PointerMove 只能更新 `PreviewTransform` 和必要的渲染请求，不得生成 Undo，不得写普通日志，不得阻塞 UI 线程等待 Vulkan。

## 12. Gizmo 技术路线

首版 Move Gizmo 只支持 X 轴、Y 轴、Z 轴、世界坐标、单对象和 Position 移动。

轴命中建议：

```text
世界轴端点投影到屏幕
-> 形成屏幕线段
-> 计算鼠标到线段距离
-> 小于约 6 到 10 像素则命中
```

阈值是可调交互参数，不写死为架构常量，必须在真机验收中微调。

## 13. Commit / Cancel 契约

Commit：

```text
Before = TransformStartSnapshot
After = 最后一个有效 PreviewTransform
```

一次完整拖动只允许产生一次正式 Transform 修改、一条 Undo 记录和一次 Commit 日志。

Cancel 必须丢弃 Preview、恢复 StartSnapshot、释放鼠标捕获、关闭 Transform Session、不生成 Undo，并拒绝延迟 MouseUp。

## 14. Undo 最小模型

最小 Undo 记录：

```text
EntityKey
BeforeTransform
AfterTransform
```

Undo 后恢复 `BeforeTransform`。Redo 可作为 ARCH-C 后半段候选，但不得成为 R1 到 R6 的硬阻断项；是否纳入 R7 由实现前审计决定。

## 15. 线程与生命周期边界

- UI 线程提交编辑命令。
- Picking 可以同步执行，也可以后续异步化，但结果必须带 Session 和 Generation。
- Renderer 只读取快照或渲染请求，不拥有编辑事实。
- Resize、Detach、窗口失焦、`WM_CANCELMODE` 必须取消当前 Transform Session。
- 延迟 MouseUp 不得复活旧 Session。
- ViewportRevision 改变后，旧 Picking 和旧 Gizmo 拖动结果都必须失效。

## 16. 错误与退化处理

必须处理相机视线与轴接近平行、拖动平面退化、鼠标移出视口、窗口失焦、`WM_CANCELMODE`、Escape、Detach、Resize 期间输入失效和无 Session 延迟 MouseUp。数学退化时必须明确取消或保持上一有效 Preview，禁止 NaN 或无限坐标进入状态。

## 17. 日志与探针格式

正式日志事件建议：开始选择命中、提交选择、清除选择、开始变换捕获、提交变换、取消变换、拒绝过期命中、拒绝失效会话、变换计算退化。

高频 Preview 默认不逐帧输出普通日志。必要探针格式：

```text
[ARCH-C探针] 事件=开始变换捕获 Session=4 Entity=TestCube Axis=X
[ARCH-C探针] 事件=提交变换 Session=4 Delta=(1.25,0,0)
[ARCH-C探针] 事件=取消变换 Session=5 原因=Escape
[ARCH-C探针] 事件=拒绝过期命中 Session=3 CurrentSession=5
```

探针必须能关联 Session，并区分 Picking、Selection、Transform；封版后删除或降级。

## 18. 性能预算

PointerMove 禁止每次移动分配大量对象、重建完整场景快照、写高频普通日志、生成 Undo 或阻塞 UI 线程等待 Vulkan。

建议使用 PointerMove 合并、Preview 覆盖、渲染请求合并、高频探针限流，并让正式日志只记录 Begin / Commit / Cancel / Error。

Picking 正式主路径禁止全实体线性遍历，禁止每次点击重建空间索引，禁止默认在 PointerMoved 高频路径持续 Picking。空间查询索引必须从实体创建、Transform Commit、Bounds 改变和实体删除处增量维护。

## 19. R1 到 R8 里程碑

| 阶段 | 目标 | 独立验收 |
| --- | --- | --- |
| ARCH-C-R1 | 场景实体与 Transform 所有权 | 出现一个真实测试对象；EntityKey 运行期稳定；修改 Transform 后渲染同步；Resize 后身份不变 |
| ARCH-C-R2 | 统一空间事实、渲染接入、空间查询地基与 CPU Picking | R2-A 冻结长期空间架构；R2-B 封版统一 Camera / Viewport / WorldRay；R2-C 封版 Render 正式消费统一 ViewProjection；随后进入空间索引、Ray-AABB 与真实 Picking |
| ARCH-C-R3 | 真实 Selection 同步 | 视口点击后层级树高亮；Inspector 显示真实对象；重复选择 NoChange；点击空白清除选择 |
| ARCH-C-R4 | Move Gizmo 显示与轴命中 | 未选中无 Gizmo；选中后 Gizmo 跟随对象；X/Y/Z 可独立命中；点击 Gizmo 不误选背后对象 |
| ARCH-C-R5 | Transform Preview | 拖动 X/Y/Z 只改变对应轴；PointerMove 不生成 Undo；无效数学结果不入状态 |
| ARCH-C-R6 | Commit 与 Cancel | 多次 Preview 后只 Commit 一次；Escape 和 `WM_CANCELMODE` 恢复原位；Cancel 后延迟 MouseUp 不 Commit |
| ARCH-C-R7 | 最小 Undo | Commit 后 Undo 返回拖动前位置；一次拖动不是数百条历史；对象删除后 Undo 明确失败或失效 |
| ARCH-C-R8 | 真机综合验收与封版 | 点击、同步、Gizmo、Preview、Commit、Cancel、Undo、Resize、日志栏、再次拖动、正常关闭全链路通过 |

## 19.1 ARCH-C-R2 Entry Gate / Exit Gate

R2 开工入口条件：

```text
R1 已封版
EntityKey 稳定
CommittedTransform 权威状态明确
Scene 生命周期独立于 Vulkan
长期空间查询架构已冻结
渲染后端无关 ViewportState / CameraState 契约存在
```

R2 封版出口条件：

```text
没有全实体扫描正式主路径
空间索引增量维护
Picking 不依赖 Vulkan
Render / Picking 共用统一空间事实
实体移动后索引同步
Resize 后坐标正确
最近命中正确
不修改 Selection
0 warning / 0 error
真机验收通过
```

R2 分解顺序：

```text
R2-A 已完成：长期空间查询架构与入口审计
R2-B 已封版：统一 Camera / Viewport / ViewProjection / WorldRay 数学契约
R2-C 已封版：Vulkan 渲染正式消费统一空间事实
R2-D 已补全：真实 Scene 接线、动态空间索引、SpatialRevision、AABB / WorldRay 候选查询和 1k / 10k 规模回归
R2-E 已实装：实体级 Ray-AABB / 最近命中
R2-F：真实鼠标 Picking
```

## 20. 自动验证矩阵

每轮至少执行：

```text
git status
git diff --check
版本号一致性检查
scripts/arch-a-guard.ps1
dotnet restore（仅在需要时）
dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false
现有测试
手写代码 5+100 检查
文档链接检查
SVG XML 有效性检查
```

验收底线：0 error、0 warning、不新增依赖、不新增未规划运行时代码、不改变 Vulkan 行为、不改变输入生命周期、不改变 UI 布局。

## 21. 真机验收矩阵

R8 必须完成以下手工链路：

```text
启动编辑器
-> 点击真实对象
-> 层级树同步
-> Inspector 同步
-> Gizmo 显示
-> 拖动 Preview
-> MouseUp Commit
-> 再次拖动
-> Escape Cancel
-> Undo
-> Resize
-> 展开/收起日志栏
-> 再次选择和拖动
-> 正常关闭并释放 Vulkan 资源
```

## 22. 风险与阻断项

当前风险：

- 相机 / 视口 / 世界射线数学契约已在 `ARCH-C-R2-B` 封版；Render 正式消费同一套 ViewProjection 已在 `ARCH-C-R2-C` 真机验收封版。
- 当前默认相机下世界 `+X` 映射到屏幕左侧，已确认为 Render / WorldRay 共享的冻结坐标约定；若未来产品层要求 `+X` 屏幕向右，必须整体调整相机、ViewProjection、WorldRay、测试与 Gizmo 约定，禁止局部取负号。
- Transform 正式所有权尚未落到具体类型。
- CPU Picking 需要明确视口逻辑像素、物理像素和 DPI 的换算边界。
- Selection 现有 Key 来自 UI 树节点，不能直接当作长期场景 EntityKey。

阻断条件：

- 必须修改 Vulkan 生命周期才能规划或实现 ARCH-C。
- 现有 Selection 所有权与报告不一致。
- 需要新增第三方数学、碰撞或 ECS 依赖。
- 需要新增项目或公共程序集。
- 需要移动、删除或重命名既有模块。
- ARCH-A Guard 失败。
- Build 出现新 warning 或 error。
- 规划需要直接决定场景存档格式。
- 修改范围超过规划文档、版本同步和治理文档。

## 23. 后续阶段接口

ARCH-C 之后可扩展多选、局部坐标、旋转 / 缩放 Gizmo、精确网格 Picking、BVH / 空间索引、Redo、场景存档格式、Prefab / 组件系统和更完整的相机与视口工具。这些能力必须在 ARCH-C 最小闭环封版后另开规划，不在 R1 到 R8 中顺手实现。
