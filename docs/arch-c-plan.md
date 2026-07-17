# ARCH-C-Plan：真实场景编辑交互闭环规划

版本：v0.2.17.1-rz  
日期：2026-07-17 22:49:25  
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
- 明确 Picking 请求与结果的 Session、RequestSequence、ViewportGeneration 过期保护。
- 明确 Selection 仍由统一命令链进入 `EditorStateOwner`，所有视图只消费快照。
- 明确 Move Gizmo 首版只支持世界坐标 X/Y/Z 轴位置移动。
- 明确一次拖动只产生一次正式 Transform 修改、一次 Undo 记录和一次 Commit 日志。
- 明确 R1 到 R8 每轮都可以独立真机验收。

## 5. 明确不做

ARCH-C 首轮不做多选、父子 Transform、旋转、缩放、局部坐标轴、Prefab、ECS、通用组件系统、场景持久化、GPU Picking、精确网格 Picking、BVH、通用命令总线、UI 布局重做或 Vulkan 生命周期修改。发现相关问题时，只记录到风险清单或后续阶段接口，不借规划轮修复运行时代码。

## 6. 模块边界

| 模块 | ARCH-C 职责 | 禁止事项 |
| --- | --- | --- |
| Scene Model | 持有实体身份、AABB、正式 Transform | 不承担 Vulkan 资源和 UI 控件职责 |
| EditorStateOwner | 统一接收 Selection / Tool / Interaction / Transform 命令 | 不直接依赖 Avalonia 控件、Win32 HWND 或 Vulkan 对象 |
| Viewport Input | 提供 Pointer 坐标、Session 和 ViewportGeneration | 不直接修改场景 Transform |
| Picking | 把屏幕坐标转换为命中实体候选 | 不写 Selection，不写 Transform |
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

```text
Pointer 屏幕坐标
-> 视口局部坐标
-> 标准化设备坐标
-> 相机矩阵生成世界射线
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

首版 Picking 采用 `CPU Ray Picking + Ray-AABB`。

请求至少携带：

```text
InputSessionId
RequestSequence
ViewportGeneration
PointerPosition
```

结果至少携带：

```text
InputSessionId
RequestSequence
ViewportGeneration
EntityKey
HitDistance
HitPosition
```

应用结果前必须验证 Session 仍有效、RequestSequence 未过期、ViewportGeneration 未变化且实体仍存在。过期 Picking 结果不得覆盖更新的 Selection。

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

Transform Session 必须携带 SessionId、EntityKey、Axis、StartPointer、CurrentPointer、TransformStartSnapshot、LastValidPreview、ViewportGeneration 和 CancelReason。PointerMove 只能更新 `PreviewTransform` 和必要的渲染请求，不得生成 Undo，不得写普通日志，不得阻塞 UI 线程等待 Vulkan。

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
- ViewportGeneration 改变后，旧 Picking 和旧 Gizmo 拖动结果都必须失效。

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

Picking 初期可以线性遍历少量对象。只有当可 Picking 实体数量或真机测量时间超过既定预算时，才进入空间索引或 BVH；没有性能证据前不得提前开发 BVH。

## 19. R1 到 R8 里程碑

| 阶段 | 目标 | 独立验收 |
| --- | --- | --- |
| ARCH-C-R1 | 场景实体与 Transform 所有权 | 出现一个真实测试对象；EntityKey 运行期稳定；修改 Transform 后渲染同步；Resize 后身份不变 |
| ARCH-C-R2 | 坐标转换与 CPU Picking | 点击对象命中；点击空白返回未命中；Resize 和日志栏变化后坐标仍正确；过期 ViewportGeneration 被拒绝 |
| ARCH-C-R3 | 真实 Selection 同步 | 视口点击后层级树高亮；Inspector 显示真实对象；重复选择 NoChange；点击空白清除选择 |
| ARCH-C-R4 | Move Gizmo 显示与轴命中 | 未选中无 Gizmo；选中后 Gizmo 跟随对象；X/Y/Z 可独立命中；点击 Gizmo 不误选背后对象 |
| ARCH-C-R5 | Transform Preview | 拖动 X/Y/Z 只改变对应轴；PointerMove 不生成 Undo；无效数学结果不入状态 |
| ARCH-C-R6 | Commit 与 Cancel | 多次 Preview 后只 Commit 一次；Escape 和 `WM_CANCELMODE` 恢复原位；Cancel 后延迟 MouseUp 不 Commit |
| ARCH-C-R7 | 最小 Undo | Commit 后 Undo 返回拖动前位置；一次拖动不是数百条历史；对象删除后 Undo 明确失败或失效 |
| ARCH-C-R8 | 真机综合验收与封版 | 点击、同步、Gizmo、Preview、Commit、Cancel、Undo、Resize、日志栏、再次拖动、正常关闭全链路通过 |

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

- 相机矩阵契约尚未正式冻结。
- 固定三角形仍属于渲染数据，不是场景实体。
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
