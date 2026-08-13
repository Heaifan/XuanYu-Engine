# MAP-DATA-A-R2-F3-E1 · Geometry Capability Contract

状态：`CLOSED`（契约/调查轮，无生产代码变化）。基线：`5822bb3`。

## 描述性能力

`GeometryKind` 只描述几何形状：`Point`、`Polyline`、`Polygon`。Point 的顶点数为 1、没有 Segment；Polyline 的顶点按顺序组成开放线；Polygon 的顶点按顺序组成闭合边界，首尾不重复存储。

`GeometryCapabilities` 只描述编辑器允许的能力，不是万能行为接口：

- `Selectable`：Feature 可被 Picking 选中。
- `VertexEditable`：已有 Vertex 可进入拖动编辑；不包含插入或删除。
- `Snappable`：该 Feature 的活动 Vertex 可以作为 Snap Source。
- `SnapTarget`：该 Feature 的 Vertex/Segment 可以为其他 Source 提供候选。

`Snappable` 与 `SnapTarget` 必须独立；能力契约不包含 Save、Undo、Render、Validate 或 UI 方法。`SegmentEditable`、`InsertVertex`、`DeleteVertex` 记录为未来能力，不在 E1 实现。

## 默认约束

Editable Feature 默认沿用已验证生命周期：Select → Vertex Picking → Begin → Preview → Release Commit → Esc Cancel → Undo/Redo → Save/Reload。Hidden 或 Locked Feature 不得进入编辑。Snap 只改变独立几何坐标，不建立共享拓扑；候选必须来自局部空间查询。

契约必须允许 Point/Polyline/Polygon 共存，但不得抹平 Polygon 闭合与 Polyline 开放的领域校验差异。
