# MAP-DATA-A-R2-F3-D · Road Vertex Editing

状态：`CLOSED`；D1、D1-F1、D2 均已完成真机验收；后续转入 F3-E 通用几何编辑与吸附框架。

## D1 · Road Vertex Selection

状态：`CLOSED`；原 D1 真机验收失败已由 D1-F1 修复并通过用户真机复验。

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

状态：`CLOSED`；用户真机定向复验：`PASS`。实现基线：`4329376`。

- 完成 Road 后清理 Draft，明确切回 `选择` 工具。
- 新建 Road 使用明确 ID，自动成为当前选择并显示全部顶点。
- `选择 + 道路` 的 PointerDown 只执行 Road Picking/清选，不创建 Draft。
- 第二条 Road 必须再次点击“绘制道路”；本轮不实现连续绘制。

自动验证：F1 输入状态专项 6/6；覆盖完成退出、自动选择、空地不创建、已有 Road Picking、连续 PointerDown 不增量、再次显式绘制第二条 Road。

本轮不做：Road 顶点拖动、Road Vertex Snap、Road Edge Snap、Road 与 Region 联动、Topology Weld、新 Road 数据源，以及任何新的保存合同。

## D2 · Road Vertex Drag

状态：`CLOSED`；用户真机验收：`PASS`。实现基线：`5a07aba`；前置 D1/D1-F1 收口基线：`f2c8ed3`。

已实现：Road 起点/中间点/终点自由拖动；PointerDown → Preview → PointerReleased → Dataset Commit；Esc 取消；一次拖动一条 History；Undo/Redo；保持 Polyline 开放性、顶点数量/顺序及 Road/Dataset/Layer 身份。

自动验证：D2 专项 5/5；Core 339/339；World 1361/1361；WarCore 22/22；Solution Build 0W/0E；ARCH-A PASS。用户真机验收记录见 `MAP-DATA-A-R2-F3-D2-acceptance.md`。

## F3-E · Generic Geometry Editing & Snap

状态：`NEXT`。不新增孤立的 Road Snap；先建立通用几何能力契约，再逐步收敛编辑生命周期、候选模型、吸附仲裁，最后由 Road 作为消费者验证。

拆分：

- E1：Geometry Capability Contract（Selectable / VertexEditable / Snappable / SnapTarget / GeometryKind），只做契约与 Region/Road 映射调查，不改交互。
- E2：Generic Vertex Edit Lifecycle，统一 Begin/Preview/Commit/Cancel/History，保持 Region/Road 真机行为不变。
- E3：Generic Snap Candidate，统一 Vertex/Segment 候选类型。
- E4：Generic Snap Arbitration，统一 Vertex > Segment > Free、8px/12px、Target Lock、稳定决胜与 Source Feature 排除。
- E5：Road Consumer，验证 Road 复用通用框架，不再开发 Road 专属 Solver。

明确边界：Snap 只改变独立几何坐标，不建立共享拓扑；Topology Weld 继续独立且未启动。

## F3-F · Cross-Geometry Acceptance

状态：`NOT STARTED`。只验证 Region + Road 对通用编辑/吸附框架的复用结果，不新增第二套功能实现。
