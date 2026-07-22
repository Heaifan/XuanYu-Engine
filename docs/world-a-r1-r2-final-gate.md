# WORLD-A-R1-R2 多实体真实闭环与 1K Registry Gate

版本：v0.2.18.9-fix

## 验收结论

`WORLD-A-R1-R2` 自动验收通过，但真机验收在 `v0.2.18.8-fix` 仍退回。

当前裁定：`WORLD-A-R1` 暂不 CLOSED，等待 `v0.2.18.9-fix` 真机复验。

## 真机退回修正

| 项目 | 结论 |
| --- | --- |
| 启动后退出码 `-1073741571` | 阻断，按栈溢出/录制链风险处理 |
| 多实体不点击不显示 | 已定位为 UI RenderSnapshot 丢失全量 Entities |
| 本轮修正 | UI 保留全量实体投影，Vulkan 改稳定索引循环绘制 |
| 封闭条件 | 必须重新真机确认多实体可见、Picking、Inspector、Resize |

## 多实体 Gate

| 项目 | 结果 |
| --- | --- |
| 10 个实体进入 GlobalWorld | PASS |
| 10 个实体投影到 SceneRenderSnapshot | PASS |
| EntityId 唯一且稳定 | PASS |
| Picking 可返回不同 EntityId | PASS |
| 移动 B 不污染 A / C | PASS |
| Undo 只恢复同一 EntityId | PASS |
| Destroy 后 Snapshot 无幽灵实体 | PASS |
| Destroy 后 Spatial / Picking 不再命中 | PASS |

## 1K Registry Gate

覆盖：

- Create 1000
- Get / TryGet / Exists
- Snapshot
- Destroy
- Destroy 后 Exists=false
- EntityId 运行期不立即复用

记录项：

- 创建耗时：测试输出 `WORLD-A-R1 1000实体冒烟：创建Ticks=...`
- 查询耗时：测试输出 `查询Ticks=...`
- 内存变化：测试输出 `内存变化Bytes=...`

该 Gate 是结构冒烟，不是 WORLD-A-R6 可见实体性能毕业测试。

## 禁止项确认

本轮未进入 Partition、Spatial Index 正式扩展、Organization、ECS 大重构、Terrain、Streaming、Gameplay、Rotation、Scale 或 Local Gizmo。
