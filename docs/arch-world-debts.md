# ARCH-WORLD 受控债务登记

> 本文件登记 ARCH-WORLD 分层治理过程中已知的、经裁定的受控债务（Controlled Debt）。
> 债务不代表 R1 失败，而是"第一刀切开后暴露出的耦合"，须在指定轮次收口。
> 在收口前，相关方向禁止新增依赖。治理序列见 `docs/arch-world-layer-attribution.md`。

## D1 — TransformSession 暂居 World，含 Editor/Gizmo 语义（收口轮次：R4）

- **现状**：`XuanYu.World.Transform.TransformSession` 直接 `using XuanYu.Core.Gizmo;`，
  持有 `MoveGizmoAxis` / `PreviewTransform` / `TransformStartSnapshot`；`Begin()` 接收 `MoveGizmoAxis`，
  `TryCommit()` 直接操作 `SceneStateOwner`。
- **性质**：表达"用户拖 Gizmo → 建立编辑 Session → Preview → Commit/Cancel"，属 Editor Transaction /
  Interaction，不是世界事实。
- **物理层合法的根因**：`Gizmo` 本身错误藏在 `Core` 中，Editor 概念伪装成了 Core 类型；因此
  `World → Core` 看起来合规，语义上却是 `World → Editor 概念`。
- **裁定**：R1 为解决 `SceneStateOwner → GlobalWorld` 编译闭包不得不扩大迁移范围，且 R1 明确不同时做
  Editor 剥离；`TransformSession` 现位于 World 为**过渡位置，非最终正确归属**。
- **红线路令**：**禁止再新增 World 对 `Core.Gizmo` 的依赖**。R4 将其迁至 Editor 层。

## D2 — SceneRenderSnapshot 污染 Core（收口轮次：R5）

- **现状**：`XuanYu.Core.Scene.SceneRenderSnapshot` 含 `IsSelected` / `PreviewTransform` / `ShowMoveGizmo` /
  `Camera`，乃至 `Camera ?? DefaultEditorCamera.Create(0)`；已迁 World 的 `SceneStateOwner` 仍实现
  `ISceneRenderSnapshotSource` 并持有该快照。
- **性质**：一个名义上的 `Core.Scene` 类型实际上知道选中状态、Gizmo 显示、编辑预览、编辑器默认相机——
  明显的 Editor / Presentation 语义。
- **裁定**：属 R5 Snapshot / Camera / Presentation 边界整理，**不得混入 R2**。维持"程序集依赖正确 ≠
  语义依赖完全正确"的认知。

## D3 — 测试程序集未严格映射生产层（收口轮次：R4/R5）

- **现状**：`XuanYu.World.Tests` 引用 World + Core + Editor.UI（含 `WorldCameraFramingTests` /
  `WorldPartitionUiTests` / `WorldUi*` / `TransformSessionTests` 等 Editor/UI 性质测试）；
  `XuanYu.Core.Tests` 引用 Core + World + Editor.UI（Picking / History 遗留）。
- **性质**：仅为 Test Project，不破坏运行时架构，但不映射生产层边界。
- **目标形态**：`Core.Tests → Core`；`World.Tests → World + Core`；`Editor.Tests → Editor + World + Core`。
  不阻挡 R2。

## 红线程式

- R1 主线（先建 World 边界、双轨 SpatialIndex 留 R2）未偏。
- 双轨 SpatialIndex 确证存在（GlobalWorld → WorldQuery → SpatialIndexOwner A；
  SceneStateOwner → SpatialIndexOwner B），收口轮次：**R2 单一空间权威**。
- 守卫现状：自 R1-R1 起，`scripts/arch-a-guard.ps1` 已自动校验 Core ✕→ World、World only → Core、
  World ✕→ Editor/Vulkan/Avalonia/Silk，以及 Solution 必须含 World / World.Tests。
