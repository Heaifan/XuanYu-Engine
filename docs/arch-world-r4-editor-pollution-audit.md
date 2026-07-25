# ARCH-WORLD-R4-R0A Editor 污染归属只读审计

版本：`v0.2.19.3-rz`｜分支：`refactor/ARCH-WORLD-layer-boundary`｜前置：`ARCH-WORLD-R3 CLOSED`（`e50d890`）+ changelog 哈希纠偏（`b82a240`）

> 本轮性质：**只读审计**，不修改任何生产代码、不移动类型、不新建项目/模块。仅输出归属结论与最小迁移计划草案，待用户裁定后实装。

## 一、审计范围与四组焦点

R4 目标：识别并定位"编辑器职责"对基础 `Scene`/`World`/`Core` 层的污染，确认依赖方向，产出最小迁移计划。四组焦点：

1. `DefaultEditorCamera.Create(0)` 隐藏兜底；
2. `TransformSession` 归属；
3. `Framing` / `Selection` / `Preview` 编辑器职责污染；
4. 依赖方向精确表。

## 二、焦点 1：`DefaultEditorCamera.Create(0)` 隐藏后门

- **位置**：`XuanYu.Core/Scene/SceneRenderSnapshot.cs:34` `CameraState => Camera ?? DefaultEditorCamera.Create(0)`。
- **`DefaultEditorCamera`**：静态类，`XuanYu.Core/Space/DefaultEditorCamera.cs:5-19`，纯只读相机合同（Position/Target/Up + `Create(revision)` 派生 `CameraState`）。
- **谁提供 `Camera`**：`SceneRenderSnapshot.Camera` 为可选 `CameraState?`。生产唯一生产者是 `UiVm.RenderSnapshot`（`UiVm.Scene.cs:22-28`），其构造快照时**始终传入 `_camera`**（`UiVm.Camera.cs:9` 初始化为 `DefaultEditorCamera.Create(1)`）。
- **是否触发**：生产路径中无任何代码直接读取 `SceneStateOwner.RenderSnapshot`（基础 World 投影，`SceneWorldProjection.cs:19-22` 不带 Camera）的 `.CameraState`；`UiVm` 仅从中取 `.Entity`/`.Entities` 并以自身 `_camera` 重建。故后门在真实编辑器中**永不触发，是死代码**。
- **掩盖缺陷分析**：后门仅在"构造 `SceneRenderSnapshot` 而不传 Camera"时静默生成默认相机——生产不触达，但会掩盖"`Camera` 从未被提供"的潜在缺陷（若未来新增生产路径漏传 Camera，将静默得到默认而非快速失败）。
- **处置选项**：① 显式可选（返回 `Camera`，由消费者处理 null）；② 快速失败（为 null 时抛异常）；③ Editor 侧必传（移除 `??`，要求 `UiVm` 始终提供——其已实现）。
- **移除影响（暂不删除）**：无生产路径受影响；仅破坏未传 Camera 的测试/静态夹具（`SceneRenderSnapshot.Empty`、`TestEntityAtOrigin`）或读取其 `.CameraState` 的测试。移除需同步调整这些测试。
- **归属裁定**：相机合同本身归 Editor（D4，R4）；DTO 内静默后门属 D2（R5）边界整理细节。R4 建议：显式要求 Editor 必传 `Camera`，移除 `??` 兜底，但拆除动作与 `SceneRenderSnapshot` 迁 `Render.Abstractions` 协同进行（R5），不在 R4 单独立项强删。

## 三、焦点 2：`TransformSession` 归属

- **位置**：`XuanYu.World/Transform/TransformSession.cs:9`，`using XuanYu.Core.Gizmo;`（D1 反向依赖 World→Core.Gizmo）+ Core.Scene / Core.Transform / World.Scene。
- **全部生产消费者**：仅 `XuanYu.Editor.UI/Vm/UiVm.MoveGizmo.cs:10`（`readonly TransformSession _transformSession = new();`）。确认**生产只有 Editor.UI 使用**。
- **测试消费者**：`WorldPartitionR1Tests`、`TransformSessionTests`、`WorldSceneSelectionReentryTests`、`WorldR1FinalSceneTests`、`TransformHistoryRedoIntegrationTests`、`TransformHistoryIntegrationTests`（均位于 `World.Tests`/`Core.Tests`）。
- **是否令 UI 成为核心权威**：**否**。`TransformSession` 是瞬态 UI 交互会话（Begin/Preview/Commit/Cancel）；`TryCommit`（:42-54）最终调用 `scene.CommitPositionWithResult` → `SceneStateOwner` → `GlobalWorld.UpdateTransform`，写入权始终在 World。它只是"UI 侧会话态"，迁 Editor 不会反转权威方向。
- **合适迁移目标**：当前无独立 `XuanYu.Editor` 非 UI 项目；唯一自然落点是其唯一消费者所在项目 `XuanYu.Editor.UI`。**不得为迁移擅自新建项目/模块**（需用户批准）。
- **归属裁定**：迁 Editor 层（D1，R4），解除 `World→Core.Gizmo` 反向依赖；落点 = `XuanYu.Editor.UI`（或待批准的新 `XuanYu.Editor`），并随迁其测试。

## 四、焦点 3：`Framing` / `Selection` / `Preview`

- **`EditorCameraFraming`**（`Core/Space/EditorCameraFraming.cs:5`）：静态纯函数，根据实体位置/视口比例/FOV 计算 `CameraState`，无状态、无 World 回渗。唯一生产消费者 `UiVm.Camera`（`FrameAll`/`FrameSelected`）。属编辑器构图职责，应迁 Editor（D4，R4）；迁移风险低（纯函数）。
- **`Selection`**（`UiVm.Selection`）：Editor 只读投影，反映 `World` 激活实体游标，**无写回 World**。非污染，维持现状。
- **`Preview`**（`TransformSession.Preview`）：拖拽瞬态会话态，仅流入 `SceneRenderSnapshot.RenderPosition` 供显示，**从不写入 World/空间索引**。属设计内的只读投影，随 `TransformSession` 一并迁 Editor，非独立污染。

## 五、焦点 4：依赖方向精确表

| 类型 | 当前项目 | 真实生产消费者 | 写入权 | 应保留/迁移位置 | 迁移风险 |
|---|---|---|---|---|---|
| `DefaultEditorCamera` | `Core.Space` | Core(`SceneRenderSnapshot`,`EditorCameraFraming`)、Editor.UI(`UiVm.Camera`) | 无（只读合同） | 迁 Editor（D4） | 低；纯静态 |
| `EditorCameraFraming` | `Core.Space` | 仅 Editor.UI(`UiVm.Camera`) | 无（纯计算） | 迁 Editor（D4） | 低；纯函数 |
| `SceneRenderSnapshot.Camera` 后门 | `Core.Scene` | 生产者=Editor.UI(`UiVm`) | 无（投影 DTO） | 删 `??`，Editor 必传（D2→R5 协同） | 中；需修测试 |
| `TransformSession` | `World.Transform` | 仅 Editor.UI(`UiVm.MoveGizmo`) | 无（Commit 经 SceneStateOwner→World） | 迁 Editor.UI（D1） | 中；解 World→Core.Gizmo，需挪测试 |
| `Selection` | `Editor.UI` | 仅 Editor.UI | 无（只读投影） | 维持现状 | 无 |
| `Preview` | `World.Transform`(会话态) | 仅 Editor.UI(经 TransformSession) | 无（仅进 RenderSnapshot 显示） | 随 TransformSession 迁 Editor | 无 |

## 六、结论与最小迁移计划草案（R4 待实施，本轮不写代码）

- **R4-M1（D4）**：`DefaultEditorCamera` / `EditorCameraFraming` 从 `Core.Space` 迁 `Editor` 层；相机合同不再驻 Core。
- **R4-M2（D1）**：`TransformSession` 从 `World.Transform` 迁 `Editor.UI`（或待批准新 `XuanYu.Editor`），解除 `World→Core.Gizmo` 反向依赖；随迁其测试。
- **R4-M3（D2→R5 协同）**：`SceneRenderSnapshot` 迁 `Render.Abstractions` 时移除 `Camera ?? DefaultEditorCamera.Create(0)`，显式要求 Editor 必传 `Camera`（本审计建议 R4 起草契约、R5 实施）。
- **不纳入 R4**：接口语义拆分（R5）、G1（已 CLOSED）、空间索引（R2 CLOSED）、创建/删除实体 UI、VK-LIFE-1、P1 零位移 Undo。
- **本轮未改任何生产代码、未移动类型、未新建项目；仅产出归属结论与计划草案。**
