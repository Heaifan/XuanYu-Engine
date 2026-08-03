# WORLD-A-R2-R2 Partition Scale + Consistency Gate

版本：v0.2.18.15-rz

## 目标

本轮不新增世界能力，只对 `WORLD-A-R2` 分区制度做毕业前一致性压力 Gate：Partition Invariant、1000 Entity 多 Region 迁移、Hierarchy 稳定节点生命周期、Activity 查询不变和 RegionKey 几何依赖红线。

## 已冻结 Gate

- 所有 Alive Entity 必须恰好有一份 Partition Membership。
- `Membership.RegionKey` 必须等于 `PartitionStrategy.Resolve(GlobalPosition)`。
- `GlobalWorld.Create` 不再允许调用方手写 Region 绕过策略。
- `MoveToRegion` 只接受与当前 Position 策略推导一致的 Region；真实迁移仍由 Transform / Position Commit 驱动。
- 1000 Entity / 10000 次随机迁移后无重复 Membership、无丢失、无 EntityId 改变。
- Dormant Entity 仍可 `Exists` / `TryGet`，Region 与 GlobalPosition 不变。
- Hierarchy 迁移复用 Entity node，Destroy 后删除 Entity node 并清理 node cache。
- 除 `GridWorldPartitionStrategy` 和 `RegionKey` 自身外，生产代码不得调用 `RegionKey.FromGrid` 解释网格几何。

## 真机准备

编辑器 UI 使用近距离 `GridWorldPartitionStrategy(regionSize: 5)` 作为调试场景，因此可用当前 Move Gizmo 在小范围内拖动实体跨 Region，并通过 Inspector 观察 `EntityId`、`GlobalPosition`、`RegionKey` 和 `Activity`。

## 禁止项确认

未实现 Spatial Index 新阶段、Organization Graph、Terrain、Earth Mesh、GIS、完整 Streaming、Persistence、ECS 或 GPU-driven Renderer。
