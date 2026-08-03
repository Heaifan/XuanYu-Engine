# WORLD-A-R3-R1 Spatial Consistency

版本：v0.2.18.19-rz

## 目标

本轮不新增 World Query API，只封死 SpatialIndex 生命周期一致性：
Create、Move、Cross Region、Preview Cancel、Undo、Redo、Destroy 与 Rebuild 后，索引必须始终跟随 `GlobalWorld` 正式 Position。

## Spatial Owner Matrix

| 模块 | Position 事实 | Index 维护 | 当前查询者 | 裁定 |
| --- | --- | --- | --- | --- |
| `GlobalWorld` | 唯一正式事实 | 通过 `WorldQuery` 派生维护 | World Query API | PASS |
| `WorldPartition` | 不持有 Position | 不维护 SpatialIndex | Region 查询 | PASS |
| `WorldQuery` | 不拥有 Position | 维护正式 World 派生索引 | QueryRadius / QueryBounds | PASS |
| `SceneStateOwner` | 投影 / 编辑会话 | 仍维护旧 Scene SpatialIndex | Picking 当前路径 | R3-R2 最小收敛 |
| `Picking` | 不拥有 Position | 不维护 Index | 当前消费 SceneIndex Raycast | R3-R2 优先接入 WorldQuery |

结论：当前确有 `SceneStateOwner SpatialIndex` 与 `GlobalWorld WorldQuery` 两套索引同时存在。R3-R1 不做大重构；本轮先确认旧索引仍服务既有 Picking，不作为新的正式 World Query 真相。R3-R2 必须优先把 Picking 候选查询接向 WorldQuery，避免长期双轨。

## 实现

- `GlobalWorld` 暴露 `SpatialEntityCount` 和 `RebuildSpatialIndexFromWorld()`。
- `WorldQuery` 新增 `Rebuild(IEnumerable<WorldEntitySnapshot>)`，从当前 World 正式状态重建空索引。
- `GlobalWorld.Create`、`UpdateTransform`、`Destroy` 继续同步派生索引。
- Preview Cancel 测试只验证正式 World Position 与 WorldQuery 未被预览污染。

## 自动 Gate

- Create 后 Query 能找到 Entity。
- Move 后旧位置查不到、新位置查得到。
- Cross Region 后 Region 与 Spatial Query 同步。
- Preview Cancel 不污染正式 WorldQuery。
- Undo / Redo 后 Query 跟随 Before / After。
- Destroy 后 QueryRadius / QueryBounds 均无空间幽灵。
- 1000 Entity Rebuild 前后 QueryRadius / QueryBounds 结果一致。
- 确定性随机 Move / Radius / Bounds 均与 O(N) Oracle 一致。
- 解决方案 build：7 项目 `0 warning / 0 error`。
- 自动测试：149 passed / 0 failed / 0 skipped。
- `scripts/arch-a-guard.ps1`、`git diff --check`、SVG XML 与 `file-tree.md` 411 / 411 通过。

## 下一轮准备

`WORLD-A-R3-R2` 优先接入 Picking：

- WorldQuery 负责 Candidate。
- Picking 负责 Exact Ray Hit。
- 不扩张 AI / Gameplay。
- 退休或降级旧 Scene SpatialIndex 的正式查询地位。
