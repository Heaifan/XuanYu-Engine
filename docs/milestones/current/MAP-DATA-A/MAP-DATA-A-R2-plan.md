# MAP-DATA-A-R2 · Regional Content Authoring

状态：R2-F2、R2-F2-F2、R2-F2-F2-F1 与 R2-F3-A Region Local Spatial Query 均已 `CLOSED`；R2-F3-B Region Vertex-to-Vertex Snap 正在实现，完整 Geometry Snapping 尚未实现，R2 保持 OPEN。

## 冻结目标

- T1：保留 R1 Closeout 与旧 Region Dataset 兼容；旧 `0.2.0` Region 文件不因读取而被强制改写。
- T2：Dataset `0.3.0` 增加 Road/Polyline 合同：稳定 32 hex ID、`geometry.type=polyline`、2～1024 个有限且不相邻重复的节点、`name/kind` 属性。
- T3：区域编辑内的 Region/Road Authoring 完成 Authoring → Render → Save/Reload；正式内容提交只产生一条 Map History 记录。
- T4：已完成 Region/Road 几何支持顶点选择、拖动预览、释放提交、Esc 取消、Undo/Redo 与领域校验。

## R2-F1 层级纠偏

Road Dataset 与 Polyline 数据闭环保留；顶层 Workspace 仅有 MapEditor/RegionEditor，Road 作为 RegionEditor 内的 `RegionAuthoringMode.Road`。
Region 与 Road Dataset、Manifest、Feature JSON、Render 和 Save/Reload 合同不变。

## R2-F2 当前修复轮：Region Pointer Safety

本轮只处理 Region Tool 空 Draft PointerMove 闪退，以及已有顶点交互被 Region Preview 抢占；修复完成并通过真机验收后，回到 R2-F1 剩余验收链。既有几何顶点编辑代码保持，不扩展道路、Vulkan、Schema、持久化或 Layer 范围。

## R2-F2 几何顶点编辑基线

点击已完成区域面或道路选中 feature 后显示顶点控制柄；拖动单个顶点先更新预览，释放鼠标才通过 `MapEditSession` 提交一条历史。区域继续执行多边形合法性校验，道路拒绝相邻重复节点；不引入吸附、共享边界或拓扑联动。

## R2-F3-A Region 局部查询基础设施

Editor 域以 MapRegion 为唯一真源，维护只含原生 `MapRegionId + Bounds` 的可重建平衡动态 AABB 树；索引已接入初始化、新建/替换、Runtime Projection、Create/Delete/Edit 与 Undo/Redo。该基础设施不引入 `EntityId` 或持久化字段，也不代表 Snap、精确几何或 UI 已完成。

## 明确不做

Road Graph、寻路、宽度/坡度、吸附、共享边界/拓扑联动、XYUI 全面改造及其他非道路功能不属于 R2-F2。

## 兼容与边界

Region/Polygon 保持原有模型和旧版本读取；新建 Dataset 使用 `0.3.0`。Road 复用现有用户数据图层投影，不新增 Vulkan 依赖或跨层 UI 依赖。
