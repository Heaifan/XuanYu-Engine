# MAP-DATA-A-R2-F3-A · Region Local Spatial Query

状态：`READY FOR USER ACCEPTANCE`；F2 已 CLOSED。本轮只建立 Region 局部候选查询基础设施，不实现 Snap 或 UI。

## 冻结目标

- T1：建立 `MapRegionId + Bounds` 派生 `RegionSpatialIndex`，输入局部 Bounds 并稳定返回附近原生 Region ID。
- T2：接通会话初始化、新建/替换、Runtime Projection、Region Create/Delete、几何提交及 Undo/Redo 的 Rebuild/Upsert/Remove 生命周期。
- T3：用自动测试锁定远处排除、增删改同步、重建、规模边界和无 `EntityId`/全地图扫描 fallback。

## 架构边界

Map/Dataset/MapRegion 是几何、属性与持久化唯一真源；`RegionSpatialIndex` 仅持有可重建的 `MapRegionId + AABB` 派生查询数据。索引使用平衡动态 AABB 树，每个 Region 只占一个叶节点，内部节点只保存联合 Bounds 与高度；允许复用空间索引思想，不复用 `GlobalWorld` 的 `EntityId` 容器语义，禁止 Region→EntityId 映射。

## 正式查询链

`MapPoint → local bounds → RegionSpatialIndex.Query → nearby RegionId → future exact geometry`。索引未准备时必须 Rebuild 或明确不可查询，禁止以 `map.Regions` 全量遍历回退。

## 明确不做

不做 Vertex/Edge Snap、SnapResult、PointerMove UI 反馈、Road、通用 Map Feature 框架、Schema/JSON 变化、GlobalWorld 改造、Shared Boundary 或 Topology。

## 自动验证

局部查询只返回相交 Region ID；初始化、New/Replace/Runtime Projection、Create/Delete/Edit/Undo/Redo 后查询与事件发布同步；MapDefinition/MapRegion 不新增 Spatial 持久化字段；源代码守卫禁止 `EntityId` 与 `map.Regions` fallback。10,000 个跨中心轴区域锁定 `2N-1` 节点、树高、访问节点数与叶候选数上界，重复 Upsert/全量 Remove 锁定无孤儿节点。

## 收口边界

本轮没有 UI 入口，依据服务层能力验收规则不设置真机 IPO；正式自动门禁通过后 F3-A 可独立 `CLOSED`。该结论不代表 Vertex/Edge Snap、精确几何、PointerMove 反馈或 UI 已实现，R2 继续保持 OPEN。
