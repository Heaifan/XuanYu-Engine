# MAP-DATA-A-R2-F3-C3 · Region Snap 真机验收

状态：`CLOSED`；验收日期：2026-08-13；验收方式：用户真机验收；验证基线：`36ad9e5`。

| 序号 | 路径 | 输入 | 过程 | 输出 |
|---|---|---|---|---|
| C3-M01 | 地图编辑 → 区域编辑 → 顶点拖动 | Region 顶点靠近另一 Region 顶点 | 进入约 8 px 的吸附范围并释放 | Vertex → Vertex Snap：PASS |
| C3-M02 | 地图编辑 → 区域编辑 → 顶点拖动 | Region 顶点靠近另一 Region 边 | 进入约 8 px 的边吸附范围并释放 | Vertex → Edge Snap：PASS |
| C3-M03 | 地图编辑 → 区域编辑 → 顶点拖动 | 已吸附顶点继续沿边移动 | 在释放阈值内移动并再次靠近端点 | Edge → Vertex 升级：PASS |
| C3-M04 | 地图编辑 → 区域编辑 → 相邻区域编辑 | 两个 Region 的边界顶点接近 | 完成吸附后检查两侧几何 | Region stitching：PASS |
| C3-M05 | 地图编辑 → 区域编辑 → 顶点拖动 | 鼠标在吸附点附近小范围抖动 | 观察吸附锁定与解除 | 吸附手感稳定：PASS |
| C3-M06 | 地图编辑 → 区域编辑 → 顶点拖动 | 不同缩放级别 | 重复顶点与边吸附 | 屏幕空间手感一致：PASS |
| C3-M07 | 地图编辑 → 区域编辑 → 保存 | 已完成 Vertex/Edge Snap 的 Region | 保存后重新加载 Dataset | 几何位置保持：PASS |
| C3-M08 | 地图编辑 → 区域编辑 → 撤销/取消 | 已完成吸附的编辑 | 执行 Undo 与取消编辑 | 既有编辑行为未破坏：PASS |

结论：F3-B Vertex Snap、F3-C1 Edge Snap Geometry、F3-C2 Drag Pipeline、F3-C3 User Acceptance 以及整条 Region Snap 线均正式 `CLOSED`。本轮后续只允许启动 `MAP-DATA-A-R2-F3-D1 · Road Vertex Selection`；Road 拖动、Road Snap 与 Topology Weld 不属于本验收范围。
