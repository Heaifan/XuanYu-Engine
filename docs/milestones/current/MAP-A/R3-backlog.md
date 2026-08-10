# MAP-A-R3 Backlog

GRID-RW-1：Reference Grid 正式路径已改为全局 `ReferenceGridFrameState`（100m 起步、10~140 DIP 回滞、相机 Step 吸附）驱动的 GPU procedural 世界线；每轴 513 条、总计 2052 顶点，Vulkan `LineList` 管线。旧 Fullscreen/Fragment-local LOD/band-pass 路径与其错误测试已删除。

R2 已关闭。本文件登记 R3 当前裁定与候选方向；每轮先冻结目标和范围。

## 当前裁定

- F1-FAR-RECOVERY-01：日志另确认 FarPlane 历史极值粘滞；Far 必须按当前距离回落，编辑器相机工作上限为 1,000km。此轮不引入 Camera-relative Rendering，保留 SAFE，并等待原 IPO 真机复验。

- D1：CLOSED。
- D2：OPEN。S01、S02 已由既有真机证据证明 PASS；A03～A06 BLOCKED。
- D2-F1-C2：CLOSED。RF-M01、RF-M02-A、RF-M03 PASS；RF-M02-B 转交 F1-V。
- D2-F1-V1：OPEN · ACCEPTANCE FAILED · REWORK。V1-REWORK-A 只恢复 Navigation Gizmo 输入；Region Overlay 视觉回修延后。
- F1-V2：DONE（`a367f89` 已推送）。
- F1-V3：DONE（`49b0677` 已推送）。
- V1-REWORK-B1：DONE（`ef12f4b` 已推送）。
- V1-REWORK-B2：DONE（`8c8dfdd` 已推送）；仅完成 Vector Overlay Depth Policy，真机重验尚未执行。
- V1-STAB-1：DONE（本轮）；Gizmo 输入隔离、可见轴线/端点命中与 Avalonia/Native 两条手势路径统一。
- V1-STAB-2：历史实现已被 STAB-5A 裁定为 `FAILED · WRONG PRESENTATION ARCHITECTURE`；Native Popup 不再作为可接受的 Viewport Overlay。
- V1-STAB-3：DONE（本轮）；Vector Overlay 使用独立无深度测试/无深度写入 Pass，真机俯视/45°/低角度稳定性待重验。
- A02 follow-up：`REWORK`；100m 参考网格保留，比例尺与相机 Zoom 已拆开，等待用户真机确认动态比例尺、缩放范围和 Overlay 尺寸。
- STAB-4A/5A：`FAILED · WRONG PRESENTATION ARCHITECTURE`；`ac5d306` 是 Native Popup 路线终点，禁止继续修 Popup Screen Rect。
- STAB-5B：`REWORK COMPLETE · READY FOR USER ACCEPTANCE`；Vulkan-native Scale Indicator 已完成固定 128×28 DIP 卡片、104 DIP 标尺、真实动态标签与浅色 Token 视觉门禁。
- STAB-4B：代码实现完成；视口公制尺度改为 X/Y 方向值，Zoom Floor 取较小方向并对 Metric 失败 fail-closed；斜视回归已补。
- STAB-4C：代码实现完成；Vector Overlay 移除过期 Clip-Z Bias，Fill / Stroke / Marker 直接消费 ViewProjection，保留无深度测试/写入 Pass 与绘制顺序。
- 本轮真机裁定：`MAP-A-R3-D2-F1 联合真机验收 = FAIL · FUNCTIONAL BUT UNSTABLE`；A02、B03、C01、C02 按现象判 FAIL，C03～C07、D01～D04 尚未完成。
- V06：鼠标滚轮缩放根因已修复；比例尺几何与视觉规范进入最终真机复验，固定卡片 128×28 DIP、标尺 104 DIP、标签随真实尺度变化。
- GRID-RW-1/CORR2：裁定为不再继续修补的历史 LineList 路线；资产暂保留，禁止恢复为正式入口。
- GRID-RW-2A：真机核心目标 PASS；World Grid 已独立于 MapGround、稳定存在，Region 当前观察稳定。
- GRID-RW-2B：已实现全帧唯一 Step，按保守 max(X,Y)、1/2/5 与 24~80 DIP 回滞切档；仅待真机确认拉远整体减密与滚轮不抖动。RW-2C（Analytical AA）与 RW-2D（视觉层级）仍 BLOCKED。
- MAP-A-R3-D2-F1-CLOSEOUT：RW-2A/RW-2B 真机 PASS 已记录；RW-2C/RW-2D 降级为 `DEFERRED · NON-BLOCKING VISUAL IMPROVEMENT`。F1 FINAL 仅在 15/15 真机 IPO PASS 与完整 0W0E 门禁后 CLOSED；A03～A06 仍待恢复原始验收合同，D3 继续禁止启动。
- F1 FINAL：10/15 PASS；M03/M04/M05/M15 为极远 `Vector3d` 相机进入 float ViewProjection 后的精度退化和不可逆 VP 崩溃，执行 `F1-FAR-SAFE-01` 先确保 Metric/投影失败安全及可读的 double 诊断；M06（四点 Region 闭合/图层删除）明确拆分为后续 `F1-REGION-CLOSE-01`，不得混修。F1 保持 `OPEN · FINAL ACCEPTANCE FAILED · 5 ITEMS REMAIN`。
- GRID-RW-1-CORR2（2026-08-10）：用户真机审计 FAIL 后冻结四组修复——Step 保守尺度 max(X,Y)、Dedicated Empty-input Line 管线 + 负 Depth Bias、Major/Minor（10:1，α 0.10/0.18）、连续远距/掠射 Fade（禁 band-pass/local LOD/discard）；新增各向异性门禁（2/30、30/2）。完成条件：用户逐项代码审计 PASS（FrameState → anisotropy → Pipeline → Shader → Depth → Major/Minor → Fade → Tests → SPIR-V → 门禁）后才进入真机；真机仍明显摩尔纹则 STOP，转 Screen-space Ribbon Triangle + Analytical AA（FrameState/Step/Anchor 保留）。
- D3：禁止启动；F2：不创建。

## Viewport Overlay / Scale Indicator 架构整改

| 阶段 | 内容 | 状态 |
| --- | --- | --- |
| OVL-R0 | 知识库、错误裁定与路线冻结 | DONE |
| OVL-R1 | DIP Anchor / Rect / Layout Resolver | DONE |
| OVL-R2 | Vulkan-native Scale Indicator、GlyphLite、DrawPlan | READY FOR USER ACCEPTANCE |
| OVL-R3 | 删除比例尺专属 HWND / Popup / GDI / Probe 技术债 | DONE |
| 真机验收 | 比例尺悬浮可见、Resize/DPI 稳定、Navigation Gizmo 仍最后绘制 | PENDING |

- 唯一数据链：`ScaleIndicatorMetric → UiVm → RenderProjection → ScaleIndicatorOverlay → Vulkan`。
- 唯一布局：视口左下角 16 DIP；Visual Rect 由 `ViewportOverlayLayoutResolver` 生成。
- 自动门禁不替代用户真机验收；当前 F1 继续 OPEN。

## MAP-A-R3-D2-F1-C2：正式收口

### 自动与代码结果

- 地图模式“查看全部”按 MapBounds 构图；“聚焦”按 Draft 顶点 AABB → Selected Entity → 相机不变执行。
- Native 中键路由已由 `NativePointerRoutePolicy` 统一裁决：环绕、Shift+中键平移、滚轮缩放与 Draft Preview 互斥且可恢复。
- C2-R01～C2-R09、Native Route Policy 与 F1-C 稳定性/日志回归已补齐。
- RF-M02-B 的巨大线宽/控制点不再归入 C2，而归入 F1-V Vector Overlay。

### 真机 IPO 裁定

- RF-M01：中键环绕、Shift+中键平移、滚轮缩放均 PASS。
- RF-M02-A：Draft Framing PASS。
- RF-M02-B：转交 F1-V，不作为 C2 未完成项。
- RF-M03：导航结束释放中键后，普通移动鼠标可自动恢复 Draft Preview；无需重新选工具、无输入丢失、无崩溃，PASS。
- 正式记录：[R3-C2-closure.md](R3-C2-closure.md)。

## MAP-A-R3-D2-F1-V1：Region Vector Overlay

### 真机裁定与依赖调整

- 当前正式状态：`F1-V1 = OPEN · ACCEPTANCE FAILED · REWORK`。
- 新顺序：`V1-REWORK-A Navigation Gizmo → F1-V2 100m Metric Grid → F1-V3 Scale Indicator + Zoom Floor → Metric/Picking 门禁 → V1-REWORK-B Region Overlay → V1 真机重验 → F1 Final`。
- V2/V3 提前执行是已批准的依赖调整，不代表 V1 CLOSED；Region 回修前不得宣告 V1 通过。
- 本轮真机失败现象按用户提供的六张截图归档：Region Tool 激活时 Navigation Gizmo 点击/拖动无响应；Region Draft 点击与 Gizmo 输入发生竞争；Region Overlay 在斜视/低角度下出现 Fill、Stroke、Marker 锚点或层次不稳定；放大后视觉尺度与地图编辑语义不一致。原始聊天附件不复制进仓库。

### 空间基础合同（冻结）

| ID | 合同 |
| --- | --- |
| MC-01 | 玄域世界单位保持 `1 unit = 1 meter`。 |
| MC-02 | 世界坐标继续使用 `double` 连续坐标，禁止量化为 100m 格子坐标。 |
| MC-03 | 地图编辑器最细可见参考网格为 `100m × 100m`。 |
| MC-04 | Region 顶点、实体、道路、节点、高程、DGD 数据与空间索引不受 100m 网格量化。 |
| MC-05 | 公制网格只使用 `1 / 2 / 5 × 10ⁿ` 序列。 |
| MC-06 | Zoom Floor 只限制地图编辑器视觉尺度，不修改通用 Camera 能力。 |

### 冻结范围

- `RenderVectorOverlayResource` 是 Region/Draft 唯一渲染数据合同。
- Fill 使用世界坐标三角形；Stroke 使用 Vulkan shader 屏幕空间展开；Marker 使用屏幕空间 DIP 半径。
- Draft Preview 线宽 2 DIP；正式 Region Stroke 1.5 DIP；普通点 5.5 DIP；首点 6.5 DIP；Close Candidate 8.5 DIP。
- Region/Draft 不再创建或走 `RenderStaticModelResource` / `DrawRegionModel` 正式路径。
- V2 公制网格、V3 比例尺、Inspector、History、Layer、持久化、DGD、地形和全引擎性能重构均不在本轮。

### TODO 状态

| TODO | 内容 | 状态 |
| --- | --- | --- |
| V1-T01 | 建立 MapVector Overlay 数据合同 | DONE |
| V1-T02 | 建立 Vulkan Vector Pass | DONE |
| V1-T03 | Draft Point 迁移 | DONE |
| V1-T04 | Draft Stroke 迁移 | DONE |
| V1-T05 | Formal Region Stroke 迁移 | DONE |
| V1-T06 | Formal Region Fill 迁移 | DONE |
| V1-T07 | Ear Clipping 凹多边形三角化 | DONE |
| V1-T08 | Dynamic Buffer 复用 | DONE |
| V1-T09 | PointerMove latest-state-wins | DONE |
| V1-T10 | 删除 Region StaticModel 正式路径 | DONE |
| V1-T11 | 自动专项门禁 | DONE |
| V1-T12 | 完整 0W0E 门禁 | DONE |
| V1-T13 | Commit + Push | DONE（`1e81c33` 已推送） |
| V1-T14 | 真机验收 | PENDING |

### F1-V1-REWORK-A：Navigation Gizmo 输入恢复

| TODO | 内容 | 状态 |
| --- | --- | --- |
| MF-T01 | 修改当前裁定 | DONE |
| MF-T02 | V2 改为 100m Minimum Visible Metric Grid | DONE |
| MF-T03 | 修改 V1/V2/V3 解锁顺序 | DONE |
| MF-T04 | 修正 V1-T13 Commit + Push 状态 | DONE |
| MF-T05 | 记录用户真机 FAIL 与六张截图现象 | DONE |
| MF-T06 | Navigation Gizmo LeftDown 优先于 Region Drawing | DONE |
| MF-T07 | Active Gizmo Move/Up/CaptureLost/CancelMode/KillFocus 路由 | DONE |
| MF-T08 | Gizmo 与 Region 共存自动回归 | DONE |

### F1-V2：100m Minimum Visible Metric Grid

| TODO | 内容 | 状态 |
| --- | --- | --- |
| V2-T01 | MinSpacing 固定为 100m | DONE |
| V2-T02 | 1/2/5 序列覆盖至 10,000km | DONE |
| V2-T03 | TargetCell 语义统一为 48 DIP | DONE |
| V2-T04 | 提取唯一 ViewportMetricScale | DONE |
| V2-T05 | Grid 消费 metersPerDip | DONE |
| V2-T06 | DPI 1.00/1.25/1.50/2.00 自动回归 | DONE |
| V2-T07 | F1-V2 正式门禁与 Commit + Push | DONE（`a367f89` 已推送） |

### F1-V3：Scale Indicator + Zoom Floor

| TODO | 内容 | 状态 |
| --- | --- | --- |
| V3-T01 | 视口内右下角 12～16 DIP 悬浮比例尺 | DONE |
| V3-T02 | 1/2/5 m/km 格式器 | DONE |
| V3-T03 | MapEditorZoomPolicy 独立于通用 Camera | SUPERSEDED：V06 解耦轮删除该策略 |
| V3-T04 | Perspective Zoom Floor | DONE |
| V3-T05 | Orthographic Zoom Floor | DONE |
| V3-T06 | 比例尺与 Zoom Metric 自动回归 | DONE |
| V3-T07 | F1-V3 正式门禁与 Commit + Push | DONE（`49b0677` 已推送） |

### Metric/Picking：Screen → Pick → World → Screen

| TODO | 内容 | 状态 |
| --- | --- | --- |
| MP-T01 | 统一地图投影与拾取的双精度 CPU 路径 | DONE |
| MP-T02 | 100m/10km/10,000km、DPI 与斜视往返专项 | DONE（108/108） |
| MP-T03 | 正式 0W0E 门禁与 Commit + Push | DONE（`d90ef4b` 已推送） |

### V1-REWORK-B1：Region 世界锚点统一

| TODO | 内容 | 状态 |
| --- | --- | --- |
| B1-T01 | 删除 Stroke 世界坐标 `height + 0.03` 偏移，Fill / Stroke / Marker 共享 `BaseHeightMeters` | DONE |
| B1-T02 | Fill / Stroke / Marker 世界锚点一致性合同测试，禁止 epsilon 回潮 | DONE（1/1） |
| B1-T03 | 完整正式门禁、版本更新、Commit + Push、local HEAD == remote HEAD | DONE（`ef12f4b` 已推送，文档随本轮收口） |

### V1-REWORK-B2：Vector Overlay Depth Policy

| TODO | 内容 | 状态 |
| --- | --- | --- |
| B2-T01 | 建立 Vector Overlay 专属裁剪空间 Depth Policy，Ground / Fill / Stroke / Marker 按视觉层级绘制 | DONE |
| B2-T02 | 俯视、45°、80°、89°、极近合法 Zoom、B1 锚点与 shader/pipeline/draw-order 合同 | DONE（14/14） |
| B2-T03 | 完整 0W0E 门禁、版本更新、Commit + Push、local HEAD == remote HEAD | DONE（`8c8dfdd` 已推送，文档随本轮收口） |

### V1 自动验收

- 已覆盖 V1-R01～R11：首点 Marker、双点 Stroke、Cursor 更新、正式 Region Fill/Closed Stroke、凹多边形三角化、屏幕空间宽度/半径、无 StaticModel 区域路径、容量足够时缓冲复用与 latest-state-wins。
- 自动门禁已通过；F1-V1 仍等待 V1-M01～M05 真机验收，不提前宣布完成。

## 后续解锁顺序

`OVL-R0 → OVL-R1 → OVL-R2 → OVL-R3 → 比例尺真机验收 → V1 联合真机重验 → F1 Final → A03～A06 → D2 Closeout`
