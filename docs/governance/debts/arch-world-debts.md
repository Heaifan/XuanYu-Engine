# ARCH-WORLD 受控债务登记

> 本文件登记 ARCH-WORLD 分层治理过程中已知的、经裁定的受控债务（Controlled Debt）。
> 债务不代表 R1 失败，而是"第一刀切开后暴露出的耦合"，须在指定轮次收口。
> 在收口前，相关方向禁止新增依赖。治理序列见 `docs/milestones/closed/ARCH-WORLD/arch-world-layer-attribution.md`。

## D1 — TransformSession 暂居 World，含 Editor/Gizmo 语义（原收口轮次：R4）

- **状态**：**已收口**。当前实现位于 `XuanYu.Editor/Transform/TransformSession*.cs`，不再位于 World。
- **原问题**：Transform 编辑事务属于 Editor Interaction，却曾暂存在 World，并依赖 `Core.Gizmo`。
- **收口结果**：TransformSession 的物理归属已经回到 Editor；World 不再承担该编辑事务。
- **剩余边界**：`Core.Gizmo` 是否继续保留在 Core 属于独立结构问题，不再冒充 D1 未完成。
## D2 — SceneRenderSnapshot 污染 Core（Render 路径已收口，Core 语义债待拆）

- **状态**：**PARTIAL / 受控债务**。
- **已收口部分**：Render 生产路径已经只消费 `RenderProjection`；`Render.Vulkan` 不再直接依赖
  `SceneRenderSnapshot` / `ISceneRenderSnapshotSource` / `DefaultEditorCamera`。这一部分保持 CLOSED。
- **仍存在的问题**：`XuanYu.Core.Scene.SceneRenderSnapshot` 仍包含 `IsSelected`、`PreviewTransform`、
  `ShowMoveGizmo` / `ShowRotateGizmo` / `ShowScaleGizmo`、`Camera` 等 Editor / Presentation 状态。
  `SceneStateOwner` 产出世界快照，而 `UiVm` 又基于同一 DTO 叠加选择、预览、相机和 Gizmo 状态，
  因此 Core 仍承担了上层组合语义。
- **本轮裁定**：不为月度治理强行拆断 World → Editor → Render 现有链路。后续应把“世界场景事实快照”
  与“编辑器帧级组合状态”拆成两个类型，再将 Gizmo/Camera/Preview 从 Core DTO 移出。
- **禁止事项**：在债务关闭前，不得继续向 `SceneRenderSnapshot` 增加新的 Editor/UI 专属字段。

## D3 — 测试程序集未严格映射生产层（进行中）

- **状态**：**PARTIAL / 非阻断退出后债务**。
- **本轮已完成**：
  - 已建立 `XuanYu.Editor.Tests`，新的 Editor 领域测试有独立归属；
  - 已建立 `XuanYu.Editor.UI.Tests`，新的 Avalonia/Editor UI 合同测试有独立归属；
  - 4 个 Editor History 测试已从 `Core.Tests` 迁入 `Editor.Tests`；
  - UI 版本合同测试已迁入 `Editor.UI.Tests`；
  - 两个新测试工程均已接入 GitHub CI。
- **剩余现状**：`XuanYu.World.Tests` 仍引用 World + Core + Editor + Editor.UI + Render.Vulkan；
  `XuanYu.Core.Tests` 仍引用 World + Editor + Editor.UI + Render.Vulkan。历史测试桶仍需按触碰范围逐步迁移，
  禁止继续把新的 Editor/UI 测试塞回旧桶。
- **目标形态**：`Core.Tests → Core`；`World.Tests → World + Core`；
  `Editor.Tests → Editor`（必要时只向下引用）；`Editor.UI.Tests → Editor.UI`。
- **迁移原则**：按真实职责和当前修改范围迁移，不做一次性数百文件“大扫除”制造假干净。

## 红线程式

- R1 主线（先建 World 边界、双轨 SpatialIndex 留 R2）未偏。
- 双轨 SpatialIndex 确证存在（GlobalWorld → WorldQuery → SpatialIndexOwner A；
  SceneStateOwner → SpatialIndexOwner B），收口轮次：**R2 单一空间权威**。
- 守卫现状：自 R1-R1 起，`scripts/arch-a-guard.ps1` 已自动校验 Core ✕→ World、World only → Core、
  World ✕→ Editor/Vulkan/Avalonia/Silk，以及 Solution 必须含 World / World.Tests。
