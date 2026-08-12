# MAP-DATA-A-R2-F3-B · Region Vertex-to-Vertex Snap

状态：`IMPLEMENTED · PRE-GATE PASS`；F3-A 已 CLOSED。本轮只把 F2 Region 顶点拖拽接入 F3-A 局部查询，不重写拖拽、不实现 Edge Snap 或 Road Snap。

## 冻结范围

- 屏幕空间进入半径 8 px，释放半径 12 px；吸附为坐标对齐，两个 Region 仍是独立数据。
- Resolver 只返回预览点和运行时目标，不提交地图、不写历史、不写持久化、不写空间索引。
- 候选唯一来自 `MapSession.QueryLocalRegions`；排除当前 Region 后，按屏幕距离、稳定 RegionId、顶点序号决胜。
- 投影或局部查询不可用时本帧返回 RawPoint，禁止 `map.Regions` 全量 fallback。

## 数据流

`RawPoint → RegionVertexSnapResolver → F2 Preview → Existing Commit/Undo/Redo/Save`。

Begin、End、Cancel 清理运行时 Snap State；F3-A 的生命周期和 AABB 树语义不变。

## 自动验证

专项覆盖 16 项核心行为及 10,000 候选结构烟测：滞回、最近点、稳定决胜、当前区域排除、局部 bounds、查询失败、F2 接线、状态清理和候选工作集边界。F3-B 需要 fresh 完整门禁后进入 `READY FOR USER ACCEPTANCE`，不得提前 CLOSED。

## 明确不做

Edge/Segment Snap、自动共边、拓扑焊接、Road Snap、Schema 修改、第二套空间索引、GlobalWorld/EntityId 改造和高频 PointerMove 日志。

## 真机验收范围

只验新增吸附手感：邻近吸附、超距自由拖动、最近目标、稳定保持/释放、自身排除、保留空白、不同缩放、Undo/Redo、保存重开与大量 Region 拖动表现；不重复验收 F2 原有顶点拖动闭环。
