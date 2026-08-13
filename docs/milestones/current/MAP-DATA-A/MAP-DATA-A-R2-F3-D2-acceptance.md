# MAP-DATA-A-R2-F3-D2 · Road Vertex Drag 真机验收

状态：`IMPLEMENTED · AUTOMATED GATES PASS · READY FOR USER ACCEPTANCE`。实现基线：本轮 D2 提交；前置收口基线：`f2c8ed3`。

| 序号 | 路径 | 输入 | 过程 | 输出 |
|---|---|---|---|---|
| D2-M01 | 区域编辑 → 道路 → 选择 | 已完成含 3 个以上节点的 Road | 按住起点并拖到新位置，释放 | 起点移动；Preview 期间 Dataset 不变，释放后写回 |
| D2-M02 | 区域编辑 → 道路 → 选择 | 已完成含 3 个以上节点的 Road | 按住中间顶点并拖到新位置，释放 | 中间顶点移动；顶点索引与顺序保持 |
| D2-M03 | 区域编辑 → 道路 → 选择 | 已完成含 3 个以上节点的 Road | 按住终点并拖到新位置，释放 | 终点移动；Polyline 仍保持开放，不闭合 |
| D2-M04 | 区域编辑 → 道路 → 选择 | 任一已选 Road 顶点 | PointerMove 多次后按 Esc | 恢复拖动前几何；不新增 History |
| D2-M05 | 区域编辑 → 道路 → 选择 | 已完成一次顶点拖动 | Ctrl+Z，再 Ctrl+Y | Undo 恢复 Before；Redo 恢复 After；一次拖动只一条 History |
| D2-M06 | 区域编辑 → 道路 → 选择 | 两条可编辑 Road | 连续编辑 Road A，再编辑 Road B | 无 stale drag 状态；A/B 的 Road ID、Dataset ID、Layer 不串写 |
| D2-M07 | 区域编辑 → 道路 | 隐藏或锁定 Road | 尝试点击并拖动顶点 | 不进入拖动、不写 Dataset、不产生 History |
| D2-M08 | 区域编辑 → 道路 → 保存/重载 | 已提交的 Road 顶点编辑 | 保存后重载地图 | 几何、顶点数量/顺序、Road ID、Dataset/Layer 归属保持 |

自动证据：D2 专项 5/5；完整正式门禁通过后补录。自动测试不替代本清单的真机结论；通过后才可将 F3-D2 标记 `CLOSED`。
