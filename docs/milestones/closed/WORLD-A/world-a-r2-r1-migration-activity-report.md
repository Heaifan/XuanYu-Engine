# WORLD-A-R2-R1 Migration + Activity 第一阶段

版本：v0.2.18.14-rz

## 目标

本轮在 `v0.2.18.13-rz` 基础骨架上冻结真实跨 Region 迁移语义，并完成 Active / Dormant 生命周期第一阶段。

## 已冻结语义

- `GlobalPosition -> Partition Strategy -> RegionKey` 是唯一方向。
- `IWorldPartitionStrategy` 允许后续替换地理或球面分区策略，World 核心不绑定最终平面世界假设。
- Transform Preview 不修改正式 Region Membership。
- Transform Commit 写入正式 Position 后才重新计算 Region 并更新 Membership。
- Undo / Redo 只恢复 Position，Region 由 Position 重新推导，不在 History 中保存另一套 Region 真相。
- `Active -> Dormant -> Active` 不改变 EntityId、Region 或 GlobalPosition。
- `Externalized` 只保留接口边界，本轮不伪造完整 Streaming / Persistence。
- Hierarchy Region 分组只是调试投影；Region 节点不拥有 Entity，Entity 节点继续以 EntityId 作为稳定 key。

## 自动 Gate

- Preview into B -> Cancel 仍在 A。
- Preview into B -> Commit 正式进入 B。
- A -> B 后 Undo -> A，Redo -> B。
- 多实体迁移只影响目标实体。
- 1000 Entity 多 Region 迁移后，每个 Entity 恰好一个 Region，无重复、无丢失。

## 禁止项确认

未实现完整 Streaming、Persistence、Earth Mesh、GIS、Terrain、Organization、Spatial Index 新阶段、ECS 重构或 GPU Renderer 大改。
