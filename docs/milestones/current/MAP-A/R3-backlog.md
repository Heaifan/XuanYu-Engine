# MAP-A-R3 Backlog

R2 已关闭。本文件登记 R3 当前裁定与候选方向；每轮先冻结目标和范围。

## 当前裁定

- D1：CLOSED。
- D2：OPEN。S01 已由既有真机日志证明 PASS；S02 修复确认已 PASS；A03～A06 BLOCKED。
- D2-F1：F1-C 稳定性与日志收口、F1-C2 地图相机语义自动回归已完成，F1-V 矢量叠加尚未开始；不重复要求用户证明既有 S01/S02 失败事实。
- 当前重点：等待用户真机确认 C2 地图查看全部、Draft 聚焦与相机往返；F1TRACE 取证噪声已移除。
- D3：禁止启动。
- F2：不创建。

## MAP-A-R3-D2-F1：根因收口范围与结果

已完成 F1 根因链的首轮修复；后续仅保留 F1-V 向量叠加，不回到“首点为什么不显示”的旧问题：

1. Region Drawing 作为真实可操作 Tool，放入与选择/框选/移动同一行的地图工具栏，不再以旁边状态文字冒充入口。
2. Normal、Hover、Selected、Selected+Hover 全部使用正式深色正文 token。

3. 区域资源必须过滤零长度 primitive，区域 world-space 顶点使用单位变换，拾取必须遵守地图中心原点坐标合同。

真实链路已由 Runtime 测试覆盖：Tool 控件 → Tool Active → Viewport PointerPressed → Map Surface Picking → MapPoint → MapRegionDraft 首点 → Draft RenderProjection。F1-C 补充 Focus 保护、TryProjectWorldPoint 失败安全、相机导航与低频日志合同。

### F1-C 已完成

- 聚焦：Draft 活跃或没有可聚焦实体时相机保持不变，并给出中文底部反馈。
- 投影：新增 `TryProjectWorldPoint`；W/NDC 非法返回 false，旧 `ProjectWorldPoint` 继续严格抛错。
- PointerMoved：首点无法投影时关闭候选，不让异常逃出输入循环。
- 日志：移除 F1TRACE、临时文件和 PointerMoved/Ray/Mouse/Render 高频取证；仅保留开始、成功、取消和错误节点。
- 自动回归：C-R01～C-R06 相关聚焦、投影、区域 Draft、相机导航与日志测试已补齐。

### F1-C2 已实现

- 地图编辑器模式下“查看全部”按当前 MapBounds 构图，ObservationCenter 使用地图中心；无 Scene Entity 仍可执行。
- “聚焦”优先 Draft 顶点 AABB，其次 Selected Entity，均无对象时保持相机不变。
- 地图查看全部与 Draft 聚焦分别保持当前 Perspective / Orthographic 模式；单点 Draft 使用最小可视半径。
- C2-R01～C2-R09 自动回归已补齐；未修改 Vector Renderer、Region Domain、Validator、History、LayerPanel 或 Inspector。

禁止修改 Region Domain、Validator、History、LayerPanel、Inspector、地形、持久化或启动 D3。

## 下一步验收

- S01：既有真机日志已证明工具可见、可选中、文字可读，不重复执行旧事实确认。
- S02：修复确认真机日志已 PASS；输入/路由/Draft 首点/正式 Region 链路已打通。
- F1-C2 自动门禁通过后仍需用户执行 C2-M01～C2-M04 真机确认；通过前继续保持 OPEN，不恢复 A03～A06。
- F1-V：区域/Draft 从 StaticModel 临时路径迁移到独立 Vector Overlay，尚未实现。
- 本轮没有 F2。

## 候选主题

- Inspector 完整编辑闭环。
- 地图数据落盘与 `.xymap` 持久化。
- 区域绘制与地形表达的后续真实闭环。
- DGD 衔接最小真实入口。
