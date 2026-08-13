# MAP-DATA-A-R3 · Point Feature Foundation

状态：`IMPLEMENTED · AUTOMATED FOCUSED PASS · READY FOR USER ACCEPTANCE`

## 目标

以 `Map Marker` 作为最小 Point Consumer，验证 Geometry Contract 的第三种基础形态：`Point`。Point 只有一个 Position，`VertexCount = 1`、`SegmentCount = 0`。

## 一轮范围

- Generic Point Dataset/Feature 与最小 Map Marker Consumer。
- Placement、Selection、Vertex Drag、Preview、Commit、Esc Cancel。
- Generic Vertex Snap 与局部空间查询。
- Map-level Undo/Redo 与 Dataset Save/Reload。
- 保留 Feature/Dataset/Layer 身份和既有 Region/Road 行为。

## 明确不做

城镇、资源、港口、势力归属、AI、Gameplay、Topology Weld、Shared Node、自动路口、交点、自动切分、节点增删，以及 Point 之外的业务属性系统均不属于本轮。

## 设计约束

复用现有 Geometry Capability、Edit Lifecycle、Local Query、Snap 与 Map History；不另建 Point 专用拖动/历史体系，不改变现有 Schema 边界，先完成最小 Point 数据合同与一个真实 Consumer。

正式实现基线：本轮提交；正式门禁结果与真机验收状态见 `MAP-DATA-A-R3-point-feature-foundation-acceptance.md`。
