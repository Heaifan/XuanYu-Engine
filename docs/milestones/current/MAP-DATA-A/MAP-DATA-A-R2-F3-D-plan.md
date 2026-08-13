# MAP-DATA-A-R2-F3-D · Road Vertex Editing

状态：`OPEN`；D1 已完成自动门禁，当前为 `READY FOR USER ACCEPTANCE`。D2、D3 尚未启动。

## D1 · Road Vertex Selection

状态：`READY FOR USER ACCEPTANCE`。

冻结目标：复用当前 Dataset-backed Road/Polyline 数据源，完成 Road feature 选择与顶点选择状态投影，不新增 Road 数据模型、序列化字段或几何算法。

必须证明：

- Road 可以被选中，选中后显示该 Polyline 的全部顶点。
- 显示顶点数量与 Dataset 节点数量一致。
- 点击不同顶点得到正确的顶点索引。
- 切换 Road 时清理旧 Road 的顶点选择状态。
- 切换 Region 模式时清理 Road 顶点选择状态。
- 隐藏 Road 与锁定 Road 不可编辑、不可进入顶点编辑状态。

自动验证：D1 专项 8/8；Core 339/339；World 1350/1350；WarCore 22/22；Solution Build 0 Warning/0 Error；ARCH-A PASS。真机 IPO 见 `MAP-DATA-A-R2-F3-D1-acceptance.md`。

本轮不做：Road 顶点拖动、Road Vertex Snap、Road Edge Snap、Road 与 Region 联动、Topology Weld、新 Road 数据源，以及任何新的保存合同。

## D2 · Road Vertex Drag

状态：`NOT STARTED`。仅在 D1 完成并通过验收后规划；负责 Preview → Release → Dataset Commit、Esc 取消、Undo/Redo 与现有 Road 几何校验。

## D3 · Road Snap

状态：`NOT STARTED`。仅在 D2 完成并通过验收后规划；不在 F3-D 偷渡 Region Snap 之外的新拓扑能力。
