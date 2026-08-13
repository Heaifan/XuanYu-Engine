# MAP-DATA-A-R3 Point Consumer 决策

## 决策

Map Marker 是 Geometry Capability Contract 的首个 Point Consumer。Point 复用既有 Generic Edit Lifecycle、Local Spatial Query、Snap Arbitration、Map-level History 与 Dataset Save/Reload，不建立 Point 专属拖动、吸附或历史系统。

## 合同

Point 的 `VertexCount = 1`、`SegmentCount = 0`；Marker 同时声明 `Snappable` 与 `SnapTarget`。候选仍必须通过局部空间查询，PointerMove 禁止全量扫描 Feature。Snap Policy 显式允许 Marker 与 Region/Road/Marker 的合法组合，并排除自身。

## 边界

本轮只实现最小 `marker` Dataset 与 Map Marker Consumer，不引入城镇、资源、港口、Gameplay、Topology Weld、Shared Node、自动路口、交点、自动切分、节点增删或业务属性系统。
