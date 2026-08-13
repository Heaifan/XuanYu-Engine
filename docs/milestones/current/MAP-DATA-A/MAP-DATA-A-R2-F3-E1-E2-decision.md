# MAP-DATA-A-R2-F3-E1 · E2 Decision

## E2 可以抽什么

1. 保留 `MapGeometryDrag` 的 Source Feature、VertexIndex、OriginalGeometry 语义，抽出最小 Begin/Preview/Commit/Cancel 生命周期协调器。
2. 继续复用 `MapEditSession.CommitMapChange` 与现有 Map History；优先以 Adapter 提供 Geometry Read/Replace/Validate，不创建第二份地图状态。
3. 统一 Pointer Capture、Preview-only、Release Commit、Esc Cancel 的时序合同；Region 与 Road 作为两个已验证 Consumer 逐一接入。
4. 让 Point/Polyline/Polygon 由 Adapter 描述顶点数量、开放/闭合形状和合法性，而不是由共享生命周期硬编码。

## E2 不能碰什么

- 不改变 Pointer 路由优先级、现有 Region/Road 真机行为或 Save/Reload 合同。
- 不把 `MapGeometryHitTester` 的 Region inside-polygon、Road segment-distance 或领域校验粗暴抹平成一个万能算法。
- 不实现 Road Snap，不引入新的 Snap Candidate、局部索引或全量扫描；这些属于 E3/E5。
- 不把 Snappable 与 SnapTarget 合并，不实现 SegmentEditable/InsertVertex/DeleteVertex。
- 不替换 `MapHistoryEntry` 为尚未验证的 GeometryEdit 历史模型；如需诊断字段，先做兼容性调查。
- 不触碰 Topology Weld、共享节点、共享边或任何拓扑数据模型。

E2 的进入条件是本 E1 契约、映射、Gap Report 和本决策通过评审；实现仍须小步、先 Region 后 Road，并重新通过自动门禁。
