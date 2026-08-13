# XYUI-8 Source Audit & Inventory / 源审计与清点

- 阶段：`XYUI-PILOT-R8 · FAST-CLOSE · T2 Inventory`
- 依据：`xyui/source/XYUI8/XYUI-8.md`（不可变证据源）
- **原稿出处（如实声明）**：本轮指令消息内附件全文（`@file:.hermes/desktop-attachments/XYUI-8.md`）。桌面应用未将附件落盘到磁盘附件目录（目录中无 XYUI-8.md），故按消息内附件全文逐字落盘为冻结源。**无原始磁盘文件 SHA 可记录**；如需字节级复核，用户重发附件后可用原始文件 SHA 与本冻结 SHA 比对（无需重做本轮管线）。
- 冻结 SHA-256：`4a5a36cc2d6240a17bc3eddfd1468a556bef686901791dc86c33258021928661`（3,765 行，115,342 bytes；转录落盘后仅清理 16 处行尾空白，语义零改动）
- 名称说明：路线图名称「XYUI-8 · Media & Visualization」，但 Source 实际内容仅 Visualization（图表/指标/交互），**无 Media（图像/音频/视频）组件**。以 Source 为唯一依据，本轮范围为 `XYUI-8 · Visualization`。
- 上游：A2 Foundation Registry（VALIDATED + AMEND-A/B）+ XYUI-1/2/3/4/5/6 canonical

## 16 组件清点

| 编号 | 组件 | 行范围 | 存在 |
|---|---|---|---|
| 8.01 | Visualization Container / 可视化容器 | 1–233 | ✅ |
| 8.02 | Metric / 指标值 | 234–490 | ✅ |
| 8.03 | Metric Group / 指标组 | 491–791 | ✅ |
| 8.04 | Progress & Range / 进度与区间 | 792–1075 | ✅ |
| 8.05 | Sparkline / 微型趋势图 | 1076–1291 | ✅ |
| 8.06 | Line Chart / 折线图 | 1292–1559 | ✅ |
| 8.07 | Area Chart / 面积趋势图 | 1560–1804 | ✅ |
| 8.08 | Bar Chart / 柱状图 | 1805–2023 | ✅ |
| 8.09 | Distribution / 分布图 | 2024–2243 | ✅ |
| 8.10 | Scatter Plot / 散点图 | 2244–2470 | ✅ |
| 8.11 | Heatmap / 热力图 | 2471–2676 | ✅ |
| 8.12 | Timeline / 时间轴可视化 | 2677–2894 | ✅ |
| 8.13 | Gauge / 仪表与阈值指标 | 2895–3089 | ✅ |
| 8.14 | Legend / 图例系统 | 3090–3239 | ✅ |
| 8.15 | Chart Tooltip & Crosshair / 图表检查器 | 3240–3450 | ✅ |
| 8.16 | Visualization Interaction / 可视化交互 | 3451–3765 | ✅ |

- 编号连续 8.01~8.16，无缺失、无重复：**16/16 清点完成**。

## 逐项责任 / 方案结构 / 上游依赖（Initial Classification）

| 编号 | 组件 | 核心责任 | 主要方案与 Variant | 上游依赖 |
|---|---|---|---|---|
| 8.01 | Visualization Container | 图表统一外壳：分层状态框 + 联动区（底部快照带/侧边检查器） | 方案4 Layered Frame 主 + 方案1/2/3 吸收；Variant layered/workbench/canvas/inspector | 6.19 状态语义；XYUI-4 反馈视觉；XYUI-5 布局 |
| 8.02 | Metric | 关键数据当前值的紧凑观察节点（数值优先） | 方案4 联动快照指标块 主 + 方案1 主指标块；Small/Medium/Large | XYUI-1 文本；XY.Semantic.* 状态；8-03/8-13/8-15/8-16 |
| 8.03 | Metric Group | 多指标组织与比较（非卡片墙） | 方案4 Linked Compare Group 主；Rail/Matrix/Priority Variant | 8-02；8-04/8-05/8-15/8-16 |
| 8.04 | Progress & Range | 连续值位置/目标区间/阈值/完成度 | 方案3 Target Band 主；Progress/Threshold/Compare Variant | XYUI-4 ProgressBar/ProgressRing；8-13 共享阈值语义 |
| 8.05 | Sparkline | 极简趋势表达（非完整图表） | 方案1 Inline + 方案4 Compare 主；Threshold/Locked 辅助 | 8-02/8-03/8-06/8-15/8-16 |
| 8.06 | Line Chart | 连续变量趋势主分析图 | 方案1~4 全部保留：single/multi/compare/focusBrush | 8-01/8-02/8-03/8-05/8-14/8-15/8-16 |
| 8.07 | Area Chart | 累计量/构成/区间带（面积必须有语义） | 方案1~4 + comparableStacked 增强；量面积≠区间面积 | 8-01/8-02/8-03/8-14/8-15/8-16 |
| 8.08 | Bar Chart | 离散对象比较（排序/分组/构成/偏差） | 方案1~4 全保留：ranked/groupedCompare/stackedComposition/delta | 8-01/8-02/8-03/8-14/8-15/8-16 |
| 8.09 | Distribution | 样本分布（集中/离散/偏斜/长尾/多峰） | 方案2 Compare Distribution 主；histogram/boxSummary/percentileBand | 8-01/8-02/8-03/8-14/8-15/8-16 |
| 8.10 | Scatter Plot | 双变量关系/聚类/象限/选区分析 | 方案2 Grouped 主（吸收方案1）；quadrant/selection 高级 | 8-01/8-03/8-09/8-14/8-15/8-16；XYUI-4 Marquee/Lasso |
| 8.11 | Heatmap | 二维矩阵颜色强弱扫读（含空间热力） | 方案2/3/4 主（entityTime/parameterGrid/spatial）+ 方案1 timeMetric | 8-01/8-02/8-03/8-06/8-12/8-15/8-16 |
| 8.12 | Timeline | 事件/阶段/状态变化时间轴（时间锁定入口） | 方案1 Compact + 方案4 Inspector 主；lane/interval 可选 | 8-01/8-02/8-03/8-06/8-11/8-15/8-16 |
| 8.13 | Gauge | 当前值与阈值/目标/基线关系（小型紧凑） | 方案1/2/4 正式（threshold/bullet/deviation）+ 方案3 arc 可选 | 8-02/8-03/8-04（共享阈值语义）/8-15 |
| 8.14 | Legend | Series 说明 + 基础 Series 控制（focus/hide/select） | 仅 Interactive Legend 单套逻辑，场景自动退化 | 8-06/8-07/8-08/8-09/8-10/8-15/8-16 |
| 8.15 | Chart Inspector | Crosshair/Tooltip/时间锁定/边缘检查统一入口 | 方案2 Locked + 方案4 Edge 主；Hover 默认态；Compare 模式 | XYUI-1 Tooltip 基础合同；8-02/8-03/8-05/8-16 |
| 8.16 | Visualization Interaction | 全图表统一交互合同（Zoom/Pan/Lock/Compare/Range/FollowLive） | 方案1 Direct + 方案2 Compact Bar + 方案4 Responsive 主；方案3 Contextual 低频辅助 | XYUI-5 ViewportContainer（视图变换）；XYUI-4（框选视觉）；全部 8-xx |

## Initial Classification 汇总

- 全部 16 项为 `NEW`（XYUI-8 可视化语义层），与 1~6 大量组合，不重新拥有上游 Primitive。
- 用户已定交互合同（写入 8-16 并全局生效）：`Direct Manipulation + Compact Interaction Bar + Responsive Interaction` 为主，`Contextual Interaction` 仅低频辅助。
- 预判跨组件裁定点（进入 T4 对账）：8-04 vs XYUI-4 ProgressBar/ProgressRing；8-10 框选 vs XYUI-4 Marquee/Lasso；8-12 vs XYUI-3 Steps；8-15 vs XYUI-1 Tooltip；8-16 视口交互 vs XYUI-5 ViewportContainer；8-01 状态 vs 6-19；8-13 vs 8-04 阈值语义共享；Blender 式工作区内容不在本 Source（视口操作合同归 8-16，工作区布局若出现归 XYUI-5 WorkspaceLayout）。
- 源缺陷登记：8-10「使用原则」段存在原文复制粘贴错乱（「优先 Plot 不作为所有实验默认图表」「只有当存在两个连续 Bar Chart」），按 A2 先例登记为 `SOURCE_FORMATTING_DEFECT`（SFD），Source 原文不动；意图明确（比较对象数值 → Bar Chart），canonical 按意图落并记录 defect ref。

## 状态

`INVENTORY COMPLETE · READY FOR RECONCILIATION & CANONICALIZATION`
