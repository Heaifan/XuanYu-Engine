# MAP-A-R3 Backlog

R2 已关闭。本文件登记 R3 当前裁定与候选方向；每轮先冻结目标和范围。

## 当前裁定

- D1：CLOSED。
- D2：OPEN。A01a/A01b 与 A02 已 FAIL；A03～A06 BLOCKED。
- D2-F1：REOPEN 后已完成返工，等待用户重新执行 S01/S02。
- D3：禁止启动。
- F2：不创建。

## MAP-A-R3-D2-F1：返工范围与结果

只修两个根因：

1. Region Drawing 作为真实可操作 Tool，放入与选择/框选/移动同一行的地图工具栏，不再以旁边状态文字冒充入口。
2. Normal、Hover、Selected、Selected+Hover 全部使用正式深色正文 token。

真实链路已由 Runtime 测试覆盖：Tool 控件 → Tool Active → Viewport PointerPressed → Map Surface Picking → MapPoint → MapRegionDraft 首点 → Draft RenderProjection。F1 聚焦 Runtime/静态测试 5/5，完整门禁通过。

禁止修改 Region Domain、Validator、History、LayerPanel、Inspector、地形、持久化或启动 D3。

## 下一步验收

- S01：真机工具栏真实看到“区域绘制”，且可选中、文字可读。
- S02：选中后点击有效地图面，立即出现第一个 Draft 顶点。
- S01/S02 任一失败则继续保持 OPEN；两项通过后才恢复 A03～A06。
- 本轮没有 F2。

## 候选主题

- Inspector 完整编辑闭环。
- 地图数据落盘与 `.xymap` 持久化。
- 区域绘制与地形表达的后续真实闭环。
- DGD 衔接最小真实入口。
