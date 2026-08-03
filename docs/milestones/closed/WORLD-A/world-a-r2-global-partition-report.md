# WORLD-A-R2 Global Coordinate + World Partition 基础轮

版本：v0.2.18.13-rz

## 目标

本轮建立 WORLD-A-R2 最小正式骨架：`RegionKey`、Partition Membership、Global Position Contract，并先收敛 Hierarchy / Selection 的最小稳定 key 前置债务。

## 已冻结合同

- `GlobalWorld` 仍是唯一 Entity 生命周期 Owner。
- `RegionKey` 只表示管理归属，不拥有第二套 Entity。
- `WorldPartitionMembership` 只维护 `EntityId -> RegionKey` 与活跃等级。
- `WorldEntitySnapshot.GlobalPosition` 使用双精度 `Vector3d`，当前由正式 Transform Position 同步。
- 正式 Transform 更新会按全局位置推导 Region，跨边界后 `EntityId` 不变。
- `Active / Dormant / Externalized` 只表达运行成本状态，不等同 Destroy。
- UI Selection 增加稳定 `SelectedNodeKey`，Hierarchy 刷新后按 key 重投影节点对象与 Inspector。

## 禁止项确认

未实现 Spatial Index 新阶段、Organization、Terrain、Earth Mesh、GIS、完整 Streaming Persistence、Gameplay、ECS 重构或 Renderer 大重构。

## 下一步

`WORLD-A-R2-R1` 可以继续做更完整的跨 Region 迁移真机场景：连续拖动跨边界、Region 激活/休眠刷新、Hierarchy 分组视图和更多 UI 交互验证。
