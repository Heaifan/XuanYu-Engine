# WORLD-A-R3 Spatial Index + World Query

版本：v0.2.18.18-rz

## 目标

本轮进入 `WORLD-A-R3`，建立 Spatial Index + World Query 最小正式骨架。

R3 解决的问题是：世界实体规模增长后，调用者不能通过全局扫描 Registry 查询附近实体。

## 架构边界

- `GlobalWorld` 仍是 Entity、Position、Activity 与生命周期唯一事实源。
- `WorldPartition` 继续维护 Region Membership 管理事实。
- `WorldQuery` 只维护从 `GlobalWorld` 当前实体位置派生出的空间索引。
- `SpatialIndex` 可从当前 World 状态重建，不反向决定 Entity 在哪里。
- `Region` 不等于 `Spatial Cell / Node`。

## 本轮实现

- `GlobalWorld` 在 Create / UpdateTransform / Destroy 时同步维护 `WorldQuery`。
- 新增 `GlobalWorld.QueryRadius` 和 `GlobalWorld.QueryBounds`。
- Query API 只返回 `EntityId` 集合，调用者再回到 `GlobalWorld` 查询正式实体状态。
- Radius Query 先走 Spatial AABB 候选，再对候选做半径精确过滤。
- 生产 `WorldQuery` 文件由治理测试锁定不得扫描 `GlobalWorld.Entities`。

## 自动 Gate

- 1000 Entity：`QueryRadius` / `QueryBounds` 与测试 Oracle 暴力扫描结果一致，记录 `Visited=813 / Candidates=76`。
- 10000 Entity：`QueryRadius` 与测试 Oracle 暴力扫描结果一致，记录 `Visited=3569 / Candidates=289`。
- Move / Cross Region / Destroy 后 Query 结果同步更新。
- O(N) 暴力扫描只存在于测试 Oracle，不进入生产 World Query。
- 解决方案 build：7 项目 `0 warning / 0 error`。
- 自动测试：144 passed / 0 failed / 0 skipped。
- `scripts/arch-a-guard.ps1`、`git diff --check`、SVG XML 与 `file-tree.md` 406 / 406 通过。

## 禁止项确认

- 未引入 Organization Graph。
- 未引入 Gameplay / Combat。
- 未引入 Terrain / Earth Mesh / GIS。
- 未引入完整 Streaming / Persistence。
- 未引入 Octree 大工程、GPU Driven Renderer 或 ECS 重构。

## 当前裁定

`WORLD-A-R3` 基础骨架通过后，后续可进入 `WORLD-A-R3-R1` 的 Nearby / Bounds Query 消费者接入与更完整 World Query 门面。
