# ARCH-WORLD 受控债务登记

> 本文件登记 ARCH-WORLD 分层治理过程中已知的、经裁定的受控债务（Controlled Debt）。
> 债务不代表 R1 失败，而是"第一刀切开后暴露出的耦合"，须在指定轮次收口。
> 在收口前，相关方向禁止新增依赖。治理序列见 `docs/milestones/closed/ARCH-WORLD/arch-world-layer-attribution.md`。

## D1 — TransformSession 暂居 World，含 Editor/Gizmo 语义（原收口轮次：R4）

- **状态**：**已收口**。当前实现位于 `XuanYu.Editor/Transform/TransformSession*.cs`，不再位于 World。
- **原问题**：Transform 编辑事务属于 Editor Interaction，却曾暂存在 World，并依赖 `Core.Gizmo`。
- **收口结果**：TransformSession 的物理归属已经回到 Editor；World 不再承担该编辑事务。
- **剩余边界**：`Core.Gizmo` 是否继续保留在 Core 属于独立结构问题，不再冒充 D1 未完成。
## D2 — SceneRenderSnapshot 污染 Core（收口轮次：R5）

- **状态**：**已于 ARCH-WORLD R5 CLOSED 收口**。Render 生产路径已改为消费 `RenderProjection`，
  `Render.Vulkan` 不再引用 `SceneRenderSnapshot` / `ISceneRenderSnapshotSource` / `DefaultEditorCamera`；
  `SceneRenderSnapshot` 仅保留为 World/Editor 上层组合快照，不再作为 Render 合同。
- **原现状**：`XuanYu.Core.Scene.SceneRenderSnapshot` 含 `IsSelected` / `PreviewTransform` / `ShowMoveGizmo` /
  `Camera`，乃至 `Camera ?? DefaultEditorCamera.Create(0)`；已迁 World 的 `SceneStateOwner` 仍实现
  `ISceneRenderSnapshotSource` 并持有该快照。
- **性质**：一个名义上的 `Core.Scene` 类型实际上知道选中状态、Gizmo 显示、编辑预览、编辑器默认相机——
  明显的 Editor / Presentation 语义。
- **收口裁定**：R5 采用最小 Render Projection，而非整体搬迁 Snapshot；Preview / Gizmo / Camera 在
  Editor/UI 组合边界解析，Render 只见不可变帧级投影。该债务对 Render 生产路径已关闭。

## D3 — 测试程序集未严格映射生产层（收口轮次：R4/R5）

- **状态**：ARCH-WORLD R6 判定为**非阻断退出后债务**。当前测试混层范围较广，
  单独移动少量文件会制造假干净；后续按触碰范围逐步建立
  `XuanYu.Editor.Tests` / 必要测试项目。当前 `XuanYu.World.Tests` 已发展为 554 个已跟踪文件，必须停止继续混入新的 Editor/UI 测试。
- **现状**：`XuanYu.World.Tests` 当前引用 World + Core + Editor + Editor.UI + Render.Vulkan，内部同时存在 UI、UiRuntime、Viewport、MapEditing、Transform 等跨层测试；
  `XuanYu.Core.Tests` 也仍引用 Core 之外的上层项目。
- **性质**：仅为 Test Project，不破坏运行时架构，但不映射生产层边界。
- **目标形态**：`Core.Tests → Core`；`World.Tests → World + Core`；`Editor.Tests → Editor + World + Core`。
  不阻挡 R2。

## 红线程式

- R1 主线（先建 World 边界、双轨 SpatialIndex 留 R2）未偏。
- 双轨 SpatialIndex 确证存在（GlobalWorld → WorldQuery → SpatialIndexOwner A；
  SceneStateOwner → SpatialIndexOwner B），收口轮次：**R2 单一空间权威**。
- 守卫现状：自 R1-R1 起，`scripts/arch-a-guard.ps1` 已自动校验 Core ✕→ World、World only → Core、
  World ✕→ Editor/Vulkan/Avalonia/Silk，以及 Solution 必须含 World / World.Tests。
