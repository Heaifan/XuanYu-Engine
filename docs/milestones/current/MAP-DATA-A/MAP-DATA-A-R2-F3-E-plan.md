# MAP-DATA-A-R2-F3-E · Generic Geometry Editing & Snap

状态：`CLOSED`。E1 契约已纳入本任务；本轮一次性完成通用编辑、局部候选、吸附仲裁与 Region/Road 集成，并通过 M01～M10 真机验收。全阶段不引入 Topology Weld。

## 默认能力契约

凡地图 Feature 声明 `GeometryKind = Point | Polyline | Polygon` 且 `Editable = true`，默认应具备：Feature/Vertex 选择、控制点显示、Vertex Drag、Preview、Release Commit、Esc Cancel、Undo/Redo、适用的 Vertex/Segment Snap、Dataset-backed Save/Reload、Identity Preserve，以及 Hidden/Locked 不可编辑约束。

能力维度固定为：`Selectable`、`VertexEditable`、`Snappable`、`SnapTarget`、`GeometryKind`。本契约不要求建立巨大继承体系；具体实现必须服从现有分层和单一职责边界。

## 子阶段

### A · Generic Edit Lifecycle

状态：`IMPLEMENTED`。复用既有 `MapGeometryDrag`、Preview/Release/Cancel 和 Map-level History；Region/Road 通过 Feature Adapter 保留 Polygon/Polyline 差异。

### B · Generic Local Snap Engine

状态：`IMPLEMENTED`。新增 Vertex/Segment Candidate、8/12px 仲裁、Target Lock、稳定决胜、自身排除和 GeometrySpatialIndex；PointerMove 不扫描全地图。

### C · Region + Road Migration & Integration

状态：`IMPLEMENTED`。Region 保持既有 Vertex/Edge Snap 行为；Road 已接入通用 Candidate/Arbitration，可吸附 Region Vertex/Segment 与其他 Road Vertex/Segment。

## 综合验收

不再单独创建 F3-F；本文件的 F3-E 综合验收一次覆盖 Region/Road。正式产物：`MAP-DATA-A-R2-F3-E-acceptance.md`。F3-E 已 CLOSED；R2 以 `MAP-DATA-A-R2-closeout.md` 收口，下一开发任务为 R3 Point Feature Foundation。
