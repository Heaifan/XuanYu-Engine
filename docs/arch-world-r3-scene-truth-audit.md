# ARCH-WORLD-R3-R0A Scene Truth 现状审计

版本：`v0.2.19.3-rz`｜分支：`refactor/ARCH-WORLD-layer-boundary`｜前置：`ARCH-WORLD-R2 CLOSED`（`3093eba`）

> **状态：ARCH-WORLD R3 = CLOSED（2026-07-25，R3-R0B 决策 B）**。原 R3-M1 建议经调用链核查推翻，正式撤销、不实施；收口裁定见第三节。
> 本轮性质：**只读审计**，不修改任何生产代码、不移动目录、不新增 UI。
> 目标：回答"Scene 层目前还保存了哪些本应来自 World 的权威状态？哪些只是编辑器投影，哪些仍构成第二套真相？"，并产出 R3 最小迁移计划，供后续子轮实装。

## 一、审计结论（TL;DR）

- **R1（建立 World）+ R2（单一空间权威）已消除硬件意义的"第二套真相"**：`SceneStateOwner` 不再持有独立 Transform 副本，也不持有空间索引；全部实体真相读写都经 `GlobalWorld`。
- **Scene 层当前保留的"状态"只有三类，均非第二真相**：
  1. `_world`（World 真相引用，委托源）；
  2. `_snapshot`（World 投影的渲染快照缓存）；
  3. `_activeEntityKey`（编辑器"当前激活实体"游标，属 Editor 选择态，非世界事实）。
- **真正的跨层气味集中在"表现 DTO 与相机后门"**，已由受控债务登记，归属 R4/R5，不在 R3 大规模移动：
  - D2：`SceneRenderSnapshot`（含 `IsSelected`/`PreviewTransform`/`ShowMoveGizmo`/`Camera` + `Camera ?? DefaultEditorCamera.Create(0)` 隐藏后门）仍在 `Core.Scene`；
  - D4：`DefaultEditorCamera`/`EditorCameraFraming` 仍在 `Core.Space`；
  - D1：`TransformSession`（含 `Core.Gizmo` 语义）仍暂居 `World.Transform`。
- **`SceneStateOwner` 与 `UiVm` 双实现 `ISceneRenderSnapshotSource`，但语义不同、非重复权威**：`SceneStateOwner` 返回基础 World/Scene 投影（无 Selection/Preview/Gizmo/Camera），`UiVm` 返回叠加编辑器语义的组合投影；生产渲染端只把 `UiVm` 作为 `ISceneRenderSnapshotSource` 注入（`SurfaceBridgeFactory.Create(vm.SceneSnapshotSource)` → `VulkanNativeHostSurfaceBridge._sceneSource`），`SceneStateOwner` 仅被 `UiVm` 当具体 World 门面读取，从未作为接口注入生产消费者。两者是"基础快照"与"编辑器组合快照"两个不同语义层，不构成第二套真相；接口名 `ISceneRenderSnapshotSource` 过宽属语义问题，转交 R5（见第三节 R3-R0B 决策 B）。

## 二、八项核查（对应 R3 焦点）

### 3.1 `SceneStateOwner` 是否仍形成第二套实体/空间状态？
**否。**（`XuanYu.World/Scene/SceneStateOwner.cs`）
- 字段仅 `_world`(:11)、`_snapshot`(:12)、`_activeEntityKey`(:13)；无第二 Transform/位置副本。
- 空间查询已全部委托：`QuerySpatial`/`RaycastSpatial`(:39-41) → `_world`；R2 已删 `_spatialIndex`。
- `RenderSnapshot`(:34) 返回 `_snapshot`，由 `RefreshSnapshot()`(:72-77) 经 `SceneWorldProjection.ToRenderSnapshot(_world 活跃实体, _world.Entities)` 重建——纯投影。
- 结论：Scene 不保有第二套实体/空间真相，是 World 的**门面（Facade）+ 投影层**。

### 3.2 `SceneRenderSnapshot` 数据来源是否全部来自 World 权威快照？
**否，按设计混合。**（`XuanYu.Core/Scene/SceneRenderSnapshot.cs:8-14`）
- 来自 World：`Entity`(SceneEntitySnapshot) + `RenderEntities`（均由 `SceneWorldProjection` 由 `WorldEntitySnapshot` 投影）；
- 来自 Editor：`IsSelected`、`PreviewTransform`、`ShowMoveGizmo`、`Camera`。
- 该 DTO 是"渲染画面"综合结构，混合属预期；问题在它**位于 `Core.Scene` 且自带编辑器语义与相机后门**（D2）。R3 不移动它，归 R5。

### 3.3 `DefaultEditorCamera` fallback 是否掩盖缺失状态？
**是，存在隐藏后门。**（`XuanYu.Core/Scene/SceneRenderSnapshot.cs:34`）
- `CameraState => Camera ?? DefaultEditorCamera.Create(0)`：消费方未设相机时静默生成默认编辑器相机，可掩盖"相机从未被提供"的缺陷。
- 生产路径被 `UiVm.Scene.RenderSnapshot`(:28 `Camera = _camera`) 覆盖，故线上被掩盖；但 DTO 自身仍携带该 fallback。
- `DefaultEditorCamera` 当前在 `Core.Space`（`XuanYu.Core/Space/DefaultEditorCamera.cs:5-19`），`EditorCameraFraming`(:9,:14) 与 `UiVm.Camera`(`_camera = DefaultEditorCamera.Create(1)`) 均依赖之。归属裁定：相机归 Editor（R4，D4）。

### 3.4 Scene / World / Editor / Render 各自有哪些 Writer？
**唯一真相写者链：Editor → SceneStateOwner → GlobalWorld。**
- World 真相写者（权威）：`GlobalWorld`/`EntityRegistry`（`Create`/`Destroy`/`UpdateTransform`/`MoveToRegion`/`SetActivity`/`Rebuild`）。
- Scene 门面写者（委托 World）：`SceneStateOwner.CommitPosition`/`RestoreTransform`/`CreateEntity`/`DestroyEntity`/`MoveEntityToRegion`/`SetEntityActivity`（`SceneStateOwner.cs:46-70`、`SceneStateOwner.Lifecycle.cs:10-53`），全部 `_world.*` 调用。
- Editor 写者：`UiVm`（选择/工具/相机/Gizmo 会话态）+ `TransformSession`（Preview 瞬态）。Editor **不直写实体真相**，仅经 Commit 路径 → `SceneStateOwner` → World。
- Render：纯消费 `SceneRenderSnapshot`，无写者。
- 证据：Editor.UI 全仓 grep `GlobalWorld`/`_world.` **零命中**，确认无 Editor 直连 World 写者。

### 3.5 Selection / Hierarchy / Inspector 是否只是投影？
**是，全为只读投影，无写回 World。**（`XuanYu.Editor.UI/Vm/`）
- Selection（`UiVm.Selection.cs`）：状态在 `_editorState`；`ApplySelection`(:8-32) 调 `_editorState.Select` + `_sceneState.SetActiveEntity(key, publish:false)`——后者仅设 Scene 激活游标（`:55-62`），不改实体真相；末 `PublishSceneRenderSnapshot`(:84)。
- Hierarchy（`UiVm.WorldProjection.cs:7-26` `BuildHierarchyItems`）：读 `_sceneState.Entities`（World 真相）建树节点，纯投影。
- Inspector（`UiVm.WorldProjection.cs:46-66` `BuildInspectorFields`）：读 `entity.Transform/Region/Activity` 返回展示字符串，纯投影；当前编辑器无字段编辑入口，无写回。

### 3.6 Transform Preview / Commit 的最终写入权在哪里？
- **Preview**：`TransformSession.Preview`（瞬态会话态，`XuanYu.World/Transform/TransformSession.cs:15,30-35`），仅流入 `SceneRenderSnapshot.RenderPosition` 供显示，**从不写入 World/空间索引**。
- **Commit 最终权威 = World**：`TransformSession.TryCommit`(:42-54) → `scene.CommitPositionWithResult(position)`(:52) → `_world.UpdateTransform`（`SceneStateOwner.cs:66`）。最终写入权唯一在 `GlobalWorld`。

### 3.7 是否仍存在从 Scene 反向回写 World 的旁路？
**否（无未登记旁路）。**
- 合法的 Scene→World 写即 `SceneStateOwner` 经 `_world.*`（3.4）；`SceneWorldProjection` 单向（World→Scene，`XuanYu.World/Scene/SceneWorldProjection.cs:8-23`）。
- 唯一"双"是**读/投影**重复（`SceneStateOwner` 与 `UiVm` 双实现 `ISceneRenderSnapshotSource`，见 3.8），非写回旁路。

### 3.8 哪些类型应保留，哪些职责应迁移（本轮不动代码）
| 类型 / 职责 | 当前位置 | 裁定 | 收口轮 |
| --- | --- | --- | --- |
| `SceneStateOwner`（World 门面 + 投影） | `World.Scene` | **保留** | — |
| `SceneWorldProjection`（单向投影） | `World.Scene` | **保留** | — |
| `SceneStateOwner` 的 `ISceneRenderSnapshotSource` 实现 | `World.Scene` | **保留**：基础 World/Scene 投影，被 `UiVm` 当具体门面读取；非测试冗余、非重复权威；接口语义过宽转 R5 | （撤销 R3-M1，归 R5） |
| `SceneRenderSnapshot` + `ISceneRenderSnapshotSource` | `Core.Scene` | 迁 `Render.Abstractions`（边界 DTO） | R5（D2） |
| `SceneRenderSnapshot.Camera` 后门 `?? DefaultEditorCamera.Create(0)` | `Core.Scene` | 删除，要求 Editor 必传 `Camera` | R5（D2） |
| `DefaultEditorCamera` / `EditorCameraFraming` | `Core.Space` | 迁 Editor | R4（D4） |
| `TransformSession`（含 `Core.Gizmo`） | `World.Transform` | 迁 Editor，解除 `World→Core.Gizmo` 依赖 | R4（D1） |

## 三、R3 收口裁定（R3-R0B 决策 B，2026-07-25）

经 R3-R0B 只读调用链核查，`SceneStateOwner` 与 `UiVm` 双实现 `ISceneRenderSnapshotSource` 实为两层投影（基础 World/Scene 快照 vs 编辑器组合快照），非重复权威。原 R3-M1（确立 UiVm 唯一活动源 + 降级 SceneStateOwner 为测试助手）经证据推翻，**正式撤销、不实施**。

1. **R3-M1 正式撤销，不实施。** 不得删除 `SceneStateOwner` 的 `ISceneRenderSnapshotSource` 实现，不得把 `UiVm` 提升为基础场景权威（UI 层不应自动成为引擎快照权威）。
2. **`SceneStateOwner` = 基础 World/Scene 投影**：返回 `SceneWorldProjection.ToRenderSnapshot` 基础快照（无 Selection/Preview/Gizmo/Camera），被 `UiVm` 当具体 World 门面读取，从未作为接口注入生产渲染链。
3. **`UiVm` = 编辑器组合投影**：在基础快照上叠加 `selected` + `_transformSession.Preview` + `showMove` + `_camera`，是编辑器最终组合源。
4. **生产渲染端唯一活动组合源为 `UiVm`**：经 `SurfaceBridgeFactory.Create(ReportVulkanMessage, vm.SceneSnapshotSource)` → `VulkanNativeHostSurfaceBridge._sceneSource`，Render 只消费 `UiVm`。
5. **两者不是重复权威，不构成第二套真相**：`GlobalWorld` 仍是唯一实体/Transform/Region/空间查询权威；Scene 仅是 World 的投影与协调层。
6. **`ISceneRenderSnapshotSource` 语义过宽问题转交 R5**：在 R5 拆分为"基础 Scene Snapshot Source"与"Editor Render Snapshot Source"两个语义明确的接口（不提前拉入 R3）。
7. **`DefaultEditorCamera` 与 Editor 职责归位转交 R4**：`DefaultEditorCamera.Create(0)` 隐藏兜底、`EditorCameraFraming`、`TransformSession` 含 Editor 语义等归属 R4 处理。
8. **ARCH-WORLD R3 = CLOSED**：Scene 无第二套世界真相已由 R1/R2 实质完成；R3 只读审计（R0A 八项 + R0B 调用链）确认无额外生产代码迁移需求，成果为确认依赖方向正确并阻止一次可能错误的接口删除。

- 关联：R3-M2/M3 原提案并入 R4/R5，不再作为 R3 子项；实体创建/删除编辑器 UI、P1 零位移 Undo、VK-LIFE-1、债A 均不在 R3 范围，保持转交/独立立项。

## 四、守禁区（本轮未违反）

- 未改 WorldQuery / 空间索引 / Region / EntityRegistry / Picking；
- 未移动任何目录或类型归属；
- 未新增/修改编辑器 UI；
- 未改 `SceneRenderSnapshot`、相机 fallback、TransformSession；
- 未触碰 Git 历史、未 amend、未创建 Tag/Release。
