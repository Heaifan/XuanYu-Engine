# MAP-DATA-A-R2-F3-D1 · Road Vertex Selection 真机验收

状态：`FAILED USER ACCEPTANCE`；失败日期：2026-08-13；自动门禁基线：`3078f07`；验收方式：用户真机验收。

| 序号 | 路径 | 输入 | 过程 | 输出 |
|---|---|---|---|---|
| D1-M01 | 地图编辑 → 区域编辑 → 道路 | 已有 Road Dataset | 进入道路 authoring 并点击一条可见、未锁定 Road | Road 被选中，显示全部 Polyline 顶点 |
| D1-M02 | 地图编辑 → 区域编辑 → 道路 | Dataset 节点数量为 N | 对照数据与画面顶点 | 画面顶点数量等于 Dataset 节点数量 |
| D1-M03 | 地图编辑 → 区域编辑 → 道路 | Road 的多个顶点 | 依次点击不同顶点 | 当前顶点索引与 Dataset 顺序一致 |
| D1-M04 | 地图编辑 → 区域编辑 → 道路 | 两条可见、未锁定 Road | 先选 Road A，再选 Road B | B 替换 A，旧顶点状态清理 |
| D1-M05 | 地图编辑 → 区域编辑 → 区域面 | 已选 Road 顶点 | 切换到区域面 authoring | Road 选择与顶点索引清理 |
| D1-M06 | 地图编辑 → 区域编辑 → 道路 | 隐藏 Road | 尝试点击 Road 或其顶点 | 不可选中、不可进入顶点编辑状态 |
| D1-M07 | 地图编辑 → 区域编辑 → 道路 | 锁定 Road | 尝试点击 Road 或其顶点 | 不可选中、不可进入顶点编辑状态 |
| D1-M08 | 地图编辑 → 区域编辑 → 道路 | 已选 Road | 观察 PointerDown/Move/Up | 本轮只选择，不执行 Road 拖动或数据写回 |

自动证据：D1 专项 8/8；Core 339/339；World 1350/1350；WarCore 22/22；Solution Build 0 Warning/0 Error；ARCH-A PASS。用户真机已判定 FAIL；D1-F1 定向复验通过前不得启动 D2。
