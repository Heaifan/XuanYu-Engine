# MAP-DATA-A-R2-F3-D · Road Vertex Editing

状态：`OPEN`；D1 真机验收 FAIL，F1 修复已通过自动门禁，当前等待 F1 定向真机复验。D2 为 `BLOCKED`，D3 尚未启动。

## D1 · Road Vertex Selection

状态：`OPEN`；原 D1 真机验收 FAIL，当前进入 `D1-F1 · Road Draw → Select State Fix`，等待定向复验。

冻结目标：复用当前 Dataset-backed Road/Polyline 数据源，完成 Road feature 选择与顶点选择状态投影，不新增 Road 数据模型、序列化字段或几何算法。

必须证明：

- Road 可以被选中，选中后显示该 Polyline 的全部顶点。
- 显示顶点数量与 Dataset 节点数量一致。
- 点击不同顶点得到正确的顶点索引。
- 切换 Road 时清理旧 Road 的顶点选择状态。
- 切换 Region 模式时清理 Road 顶点选择状态。
- 隐藏 Road 与锁定 Road 不可编辑、不可进入顶点编辑状态。

自动验证：原 D1 专项 8/8；F1 输入状态专项 6/6；Core 339/339；World 1356/1356；WarCore 22/22；Solution Build 0 Warning/0 Error；ARCH-A PASS。原验收记录见 `MAP-DATA-A-R2-F3-D1-acceptance.md`，F1 定向复验见 `MAP-DATA-A-R2-F3-D1-F1-acceptance.md`。

## D1-F1 · Road Draw → Select State Fix

状态：`READY FOR USER RE-ACCEPTANCE`。

- 完成 Road 后清理 Draft，明确切回 `选择` 工具。
- 新建 Road 使用明确 ID，自动成为当前选择并显示全部顶点。
- `选择 + 道路` 的 PointerDown 只执行 Road Picking/清选，不创建 Draft。
- 第二条 Road 必须再次点击“绘制道路”；本轮不实现连续绘制。

自动验证：F1 输入状态专项 6/6；覆盖完成退出、自动选择、空地不创建、已有 Road Picking、连续 PointerDown 不增量、再次显式绘制第二条 Road。

本轮不做：Road 顶点拖动、Road Vertex Snap、Road Edge Snap、Road 与 Region 联动、Topology Weld、新 Road 数据源，以及任何新的保存合同。

## D2 · Road Vertex Drag

状态：`BLOCKED`。仅在 D1-F1 定向真机复验通过后启动；负责 Preview → Release → Dataset Commit、Esc 取消、Undo/Redo 与现有 Road 几何校验。

## D3 · Road Snap

状态：`NOT STARTED`。仅在 D2 完成并通过验收后规划；不在 F3-D 偷渡 Region Snap 之外的新拓扑能力。
