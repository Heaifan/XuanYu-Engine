# WORLD-A-R1 FINAL 最终收口报告

版本：v0.2.18.12-rz

## 最终裁定

`WORLD-A-R1` 正式 CLOSED。

本轮不进入 WORLD-A-R2，不新增 Partition、Organization、Terrain、Streaming、ECS、Instancing、Rotation、Scale 或 Local Gizmo。

## 毕业 Gate

| 项目 | 自动证据 | 真机证据 | 结论 |
| --- | --- | --- | --- |
| GlobalWorld 唯一事实源 | `WorldSceneConsumptionTests` | R1-R1 已通过 | PASS |
| EntityRegistry 稳定身份 | `EntityRegistryTests` / `GlobalWorldTests` | 不适用 | PASS |
| 10 实体同时存在 | `WorldSceneMultiEntityGateTests` | 用户截图确认同时可见 | PASS |
| 1→10 连续选择 | `WorldR1FinalSelectionTests` | 用户确认狂点不闪退 | PASS |
| Move 隔离 | `WorldR1FinalSceneTests` | R1-R2-R1 后可操作 | PASS |
| Undo / Redo 身份不串线 | `WorldR1FinalSceneTests` | R1-R2-R1 后可操作 | PASS |
| Destroy 无幽灵实体 | `WorldR1FinalSceneTests` | 自动覆盖 World / Snapshot / Spatial / Render | PASS |
| 1K Registry Gate | `GlobalWorldTests` | 不绘制 1K | PASS |
| Resize / Vulkan 回归 | 自动守卫 + 用户日志 | 日志栏展开后 Swapchain 自愈并恢复 Present | PASS |

## 架构结论

R1 的目标是建立中央总账，并让 Scene / Editor / Render / Selection / Picking 消费同一个实体事实。

当前已经证明：

- 多实体身份稳定；
- 选择链单向提交；
- 移动、撤销、重做只作用同一 EntityId；
- Destroy 后无幽灵实体；
- 1K Registry 结构冒烟通过。

## R2 前置债务

进入 `WORLD-A-R2` 前必须正式处理或设计：

- Stable HierarchyNode Identity；
- Key-based Hierarchy Selection；
- 1K / 10K 可见实体不能沿用临时多 draw 作为最终渲染架构。

## 下一阶段

下一阶段可进入：

```text
WORLD-A-R2
Global Coordinate + World Partition
```
