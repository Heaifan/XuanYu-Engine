# MAP-DATA-A-R2-F1 · Regional Authoring Hierarchy

状态：CLOSED（用户真机验收通过，2026-08-12）；R3 禁止启动。

## 冻结目标

- F1-T1：删除 RoadEditor 顶层 Workspace，引入 RegionAuthoringMode。
- F1-T2：新增 RegionalAuthoringPanel，统一 Region/Road Layer Stack 与 Selection Sync。
- F1-T3：完成 Region/Road 回归、正式门禁、Commit/Push；用户已确认 F1-M01～F1-M08 全部通过。

## 长期结构

`EditorWorkspaceId` 只有 `MapEditor`、`RegionEditor`；Region 内使用 `RegionAuthoringMode.RegionSurface/Road`。
Road、Region Dataset、Manifest、Feature JSON、Renderer、Save/Reload 与 Map History 合同保持不变。

## 禁止项

不修改 Dataset/Manifest Schema，不重写 Polygon/Polyline、Picking、Vulkan、Feature 编辑、Road Graph 或 R3 内容类型；不创建 SettlementEditor、ResourceEditor、RiverEditor 或 TerrainEditor。

## 持久化审计

旧 `RoadEditor` 仅存在于运行时枚举、Workspace 菜单和 VM 分支，未发现 Preference/Session 持久化；无需数据迁移。
