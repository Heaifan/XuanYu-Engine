# WORLD-A-R1-R2 多实体真实闭环与 1K Registry Gate

版本：v0.2.18.11-rz

## 验收结论

`WORLD-A-R1-R2` 自动验收通过，但真机验收在 `v0.2.18.8-fix` 仍退回。

当前裁定：`WORLD-A-R1-R2-R1 / v0.2.18.10-fix` 真机复验通过，`WORLD-A-R1-R2` 阻断解除。

## 真机退回修正

| 项目 | 结论 |
| --- | --- |
| 启动后退出码 `-1073741571` | 阻断，按栈溢出/录制链风险处理 |
| 多实体不点击不显示 | 已定位为 UI RenderSnapshot 丢失全量 Entities |
| 本轮修正 | UI 保留全量实体投影，Vulkan 改稳定索引循环绘制 |
| 封闭条件 | 必须重新真机确认多实体可见、Picking、Inspector、Resize |

## R1 同步重入修正

| 项目 | 结论 |
| --- | --- |
| 点击第二实体转圈闪退 | 阻断，按 Selection / Hierarchy 同步重入最高嫌疑处理 |
| 本轮诊断 | 低频记录 Selection、Projection、Publish、RecordCommandBuffers 深度 |
| 本轮修正 | ActiveEntity 幂等 no-op；用户选择单入口提交；内部投影同步禁止回流 |
| 禁止项 | 未进入 WORLD-A-R2、Partition、Instancing、ECS 或 Vulkan 生命周期重构 |

## R1-R2-R1 真机验收

| 项目 | 结论 |
| --- | --- |
| 10 实体同时可见 | PASS |
| 连续点击 EntityId(2) 到 EntityId(10) | PASS |
| Selection Revision 线性递增 | PASS |
| 点击后无转圈、无闪退、无 `0xC00000FD` | PASS |
| 日志栏展开触发 Resize / Swapchain 自愈 | PASS |
| 根因裁定 | Selection 业务提交与 UI 投影同步曾存在回流风险，属局部编辑器交互架构债 |

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
