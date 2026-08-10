# MAP-A-R3 Backlog

R2 已关闭。本文件登记 R3 当前裁定与候选方向；每轮先冻结目标和范围。

## 当前裁定

- D1：CLOSED。
- D2：OPEN。S01 已由既有真机日志证明 PASS；S02 已 FAIL 且根因已确认；A03～A06 BLOCKED。
- D2-F1：继续收口三项代码根因；不重复要求用户证明既有 S01/S02 失败事实。修复完成后仅需验证修复结果。
- D3：禁止启动。
- F2：不创建。

## MAP-A-R3-D2-F1：根因收口范围与结果

只修三项已由真机日志与代码对照确认的根因：

1. Region Drawing 作为真实可操作 Tool，放入与选择/框选/移动同一行的地图工具栏，不再以旁边状态文字冒充入口。
2. Normal、Hover、Selected、Selected+Hover 全部使用正式深色正文 token。

3. 区域资源必须过滤零长度 primitive，区域 world-space 顶点使用单位变换，拾取必须遵守地图中心原点坐标合同。

真实链路已由 Runtime 测试覆盖：Tool 控件 → Tool Active → Viewport PointerPressed → Map Surface Picking → MapPoint → MapRegionDraft 首点 → Draft RenderProjection。既有日志已证明输入、路由、拾取与 Draft 状态到达；本轮补充 Vulkan 资源合法性、单位变换和负世界坐标回归。

禁止修改 Region Domain、Validator、History、LayerPanel、Inspector、地形、持久化或启动 D3。

## 下一步验收

- S01：既有真机日志已证明工具可见、可选中、文字可读，不重复执行旧事实确认。
- S02：既有真机日志裁定为 FAIL；输入/路由/Draft 状态 PASS，Primitive 合同、Region 变换和可见渲染 FAIL。
- 修复后需对修复结果做一次真机确认；通过前继续保持 OPEN，不恢复 A03～A06。
- 本轮没有 F2。

## 候选主题

- Inspector 完整编辑闭环。
- 地图数据落盘与 `.xymap` 持久化。
- 区域绘制与地形表达的后续真实闭环。
- DGD 衔接最小真实入口。
