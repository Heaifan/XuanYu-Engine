# MAP-DATA-A-R2-F3-E · Generic Geometry Editing & Snap

状态：`OPEN`；E1 已 `CLOSED`，E2 为 `NEXT`，E3/E4/E5 `BLOCKED`，等待后续小步启动。全阶段不新增孤立 Road Snap，不引入 Topology Weld。

## 默认能力契约

凡地图 Feature 声明 `GeometryKind = Point | Polyline | Polygon` 且 `Editable = true`，默认应具备：Feature/Vertex 选择、控制点显示、Vertex Drag、Preview、Release Commit、Esc Cancel、Undo/Redo、适用的 Vertex/Segment Snap、Dataset-backed Save/Reload、Identity Preserve，以及 Hidden/Locked 不可编辑约束。

能力维度固定为：`Selectable`、`VertexEditable`、`Snappable`、`SnapTarget`、`GeometryKind`。本契约不要求建立巨大继承体系；具体实现必须服从现有分层和单一职责边界。

## 子阶段

### E1 · Geometry Capability Contract

状态：`CLOSED`。产物：Capability Contract、Region/Road Mapping + Gap Report、E2 Decision。无生产代码变化。

### E2 · Generic Vertex Edit Lifecycle

状态：`NEXT`。收敛 `Begin → Preview → Commit → Cancel → History` 共享机制；不同几何只提供顶点读取、顶点替换和合法性校验。

### E3 · Generic Snap Candidate

状态：`BLOCKED`。依赖 E2 与 Road 局部候选源调查。

### E4 · Generic Snap Arbitration

状态：`BLOCKED`。统一 Vertex > Segment > Free、Enter 8px、Release 12px、Target Lock、稳定 Tie Break 和 Source Feature Exclusion。候选必须来自局部空间查询，禁止 PointerMove 正式路径扫描全部 Feature。

### E5 · Road Consumer

状态：`BLOCKED`。让 Road 作为首个通用框架消费者，验证 Road Vertex → Region/Road Vertex/Segment 的复用结果；不开发 Road 专属 Solver。

## F3-F 边界

F3-F 只做 Region + Road 的跨几何复用验收。Snap 只改变独立几何坐标，不建立共享拓扑关系；Topology Weld 属于独立数据模型能力，继续不启动。
