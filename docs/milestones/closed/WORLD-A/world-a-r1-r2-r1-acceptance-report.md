# WORLD-A-R1-R2-R1 真机验收报告

版本：v0.2.18.11-rz

## 验收结论

`WORLD-A-R1-R2-R1 / v0.2.18.10-fix` 真机验收通过。

用户确认：连续狂点多实体不再闪退，程序可继续操作。

## 证据摘要

| 项目 | 证据 | 结论 |
| --- | --- | --- |
| 多实体同时可见 | 用户截图与复验确认 | PASS |
| 连续点击 EntityId(2)~EntityId(10) | 日志 `Revision=1->2` 到 `9->10` 线性递增 | PASS |
| 点击第二实体闪退 | 本轮未复现，用户确认可狂点操作 | PASS |
| 日志栏展开 / Resize | Swapchain 自愈到 `1248x478` 并恢复 Present | PASS |
| Vulkan 地基 | Instance / Surface / Device / Swapchain / Present 均稳定 | PASS |

## 原因裁定

本问题不是 Vulkan 架构崩坏，也不是 GlobalWorld / EntityRegistry 方向错误。

真正风险是编辑器交互层的局部架构债：

```text
业务选择提交
+ ActiveEntity 切换
+ Hierarchy SelectedItem TwoWay 投影同步
+ RenderSnapshot 发布
```

这些动作在 R1-R2 前曾可能被拆成多次同步发布，并允许程序同步 UI 时回流成一次新的业务选择。

`v0.2.18.10-fix` 通过三道保险压住该风险：

- `SetActiveEntity` 同 EntityId no-op；
- 树与视口选择统一走 `ApplySelection` 单入口；
- Selection Projection 内部同步期间禁止回流成业务提交。

## 影响面判断

短期影响面已覆盖：

- Hierarchy 连续选择；
- Inspector 跟随选中实体；
- RenderSnapshot 全量实体投影；
- Picking 选择入口；
- Select B 后 Move / Undo / Redo。

长期仍需关注：

- `HierarchyItems` getter 每次重建节点对象；
- 后续 Partition / Organization 接入后，层级节点 identity 应稳定；
- 1K / 10K 级实体可见不应沿用临时多 draw 作为最终渲染架构。

## 当前裁定

`WORLD-A-R1-R2` 阻断解除。

`WORLD-A-R1` 可进入最终收口复验，不进入 WORLD-A-R2。
