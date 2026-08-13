# XYUI-8 Reconciliation & Closeout / 全量对账收口

- 状态：`XYUI-8 · CLOSED（用户最终验收 2026-08-13；Pack/Manifest/README/AGENT-GUIDE 同步）`
- 阶段：`XYUI-PILOT-R8 · FAST-CLOSE ONE ROUND`
- Source：`xyui/source/XYUI8/XYUI-8.md`（IMMUTABLE，SHA `4a5a36cc…`，115,342 bytes，3,765 行）
- Source provenance（如实声明）：本轮指令消息内附件全文落盘（桌面应用未将附件落盘到磁盘附件目录，无原始磁盘文件 SHA；用户重发附件后可做字节级 SHA 复核，无需重做管线）
- Canonical：`xyui/specs/XYUI8/XYUI-8.canonical.md`（936 行，16/16）
- 上游：Foundation Registry（VALIDATED + AMEND-A/B）+ XYUI-1/2/3/4/5/6 canonical

## 16/16 对账矩阵

| 项 | Source | Canonical | Mapping | 冲突处置 | GAP | 第二真值 |
|---|---|---|---|---|---|---|
| 8.01 Visualization Container | ✅ | ✅ | ✅ | 统一外壳（非 Card）；联动区底部快照带默认/侧边检查器扩展 | — | 0 |
| 8.02 Metric | ✅ | ✅ | ✅ | 数据节点（非 Dashboard 卡片）；数值优先层级 | — | 0 |
| 8.03 Metric Group | ✅ | ✅ | ✅ | 比较关系表达（非卡片墙）；非所有 Metric 同等级 | — | 0 |
| 8.04 Progress & Range | ✅ | ✅ | ✅ | PROGRESS BOUNDARY（完成度视觉 REF XYUI-4 4.16/4.17；本项拥有 Target Band/Threshold/Compare 语义） | — | 0 |
| 8.05 Sparkline | ✅ | ✅ | ✅ | 微型趋势（非完整图表）；Locked 模式响应主图锁定 | — | 0 |
| 8.06 Line Chart | ✅ | ✅ | ✅ | 4 方案全保留（不设唯一主样式）；系列色板 → GAP | 1 | 0 |
| 8.07 Area Chart | ✅ | ✅ | ✅ | AREA SEMANTICS（量面积≠区间面积，命名实现分开；Line 禁强行改 Area） | — | 0 |
| 8.08 Bar Chart | ✅ | ✅ | ✅ | 离散比较（Horizontal 默认优先；Delta Bar 中心零线） | — | 0 |
| 8.09 Distribution | ✅ | ✅ | ✅ | 以「比较」为核心；摘要图不得替代完整分布图 | — | 0 |
| 8.10 Scatter Plot | ✅ | ✅ | ✅ | SELECTION REF（框选/套索视觉与输入 REF XYUI-4 4.07/4.08）；SFD 登记见下 | — | 0 |
| 8.11 Heatmap | ✅ | ✅ | ✅ | 推荐区禁遮挡格子；空间热力以可解释网格/区域为主；色阶 → GAP | 1 | 0 |
| 8.12 Timeline | ✅ | ✅ | ✅ | EVENT TIMELINE OWNERSHIP（≠ XYUI-3 Steps 流程导航；Timeline 只挂关键事件不塞全日志） | — | 0 |
| 8.13 Gauge | ✅ | ✅ | ✅ | THRESHOLD SHARING（阈值语义 REF 8-04 共享，不重复定义；Arc 仅可选） | — | 0 |
| 8.14 Legend | ✅ | ✅ | ✅ | 仅 Interactive Legend 单套逻辑；嵌入 Header 不独立大侧栏 | — | 0 |
| 8.15 Chart Inspector | ✅ | ✅ | ✅ | TOOLTIP BOUNDARY（浮层基础 REF XYUI-1 Tooltip + XY.Tooltip.*；本项拥有锁定/Compare/Edge 语义） | — | 0 |
| 8.16 Visualization Interaction | ✅ | ✅ | ✅ | VIEWPORT BOUNDARY（视图变换机制 REF XYUI-5 ViewportContainer 5.16；本项拥有交互合同） | — | 0 |

## 全量统计

```text
Source accounted        16/16
Canonical accounted     16/16（936 行）
Mapping accounted       16/16（132 refs）
  CANONICAL_REF         23
  NAMESPACE_REF         27
  COMPONENT_SPECIFIC    74
  COMPOSE               6
  GAP                   2（同一缺口 XYUI8-GAP-001 的两处引用：8.06 Series 色板 + 8.11 Heatmap 色阶）
GAP reconciled          1（XYUI8-GAP-001，NON-BLOCKING；0 项遗漏 Token 复用）
A-Class unresolved      0（无已 CLOSED 核心合同互斥，无需改 Foundation）
Second Truth            0（hex 0 / rgb 0 / hsl 0 / 旧字体 0；源内无 px 硬编码值）
Broken Ref              0（132 引用全部解析；上游 1~6 组件清单逐项核对）
Source Mutation         0
Duplicate Contract      0
Semantic Ambiguity      0（12 对跨组件语义逐对裁定，见下）
```

## 裁定落地清单（6 项所有权划清）

```text
8.04 Progress & Range   PROGRESS BOUNDARY：完成度视觉 = REF XYUI-4 ProgressBar（4.16）/ProgressRing（4.17）；
                        本项拥有 Target Band（合理目标范围）/ Threshold Range（多段区间）/ Comparative Range（A/B）语义；
                        Threshold 档位映射 Neutral + XY.Semantic.Info/Warning/Error（文字+颜色双通道）
8.10 Scatter Plot       SELECTION REF：Rectangle/Lasso 框选视觉与输入 = REF XYUI-4 MarqueeSelection（4.07）/
                        LassoSelection（4.08）；本项只拥有选区统计与联动语义（Selected Count/Mean/Range + 联动）
8.12 Timeline           EVENT TIMELINE OWNERSHIP：XYUI-3 Steps = 流程步骤导航（导航语义）；本项 = 事件/阶段时间轴
                        （分析语义，时间锁定入口）；两者不合并、不互相替代
8.15 Chart Inspector    TOOLTIP BOUNDARY：浮层基础合同 = REF XYUI-1 Tooltip（1.19）+ Foundation XY.Tooltip.*；
                        本项拥有 Locked Inspector / Edge Inspector / Compare 模式 / Crosshair 语义；
                        Compare 是 Inspector 的模式，不是独立组件
8.16 Visualization Interaction  VIEWPORT BOUNDARY：Zoom/Pan 视图变换机制 = REF XYUI-5 ViewportContainer（5.16，
                        不改 Logical Position）；本项拥有交互合同（手势/状态机/FollowLive/缩放限制/设备适配）；
                        用户已定合同落地：Direct Manipulation + Compact Interaction Bar + Responsive Interaction
                        为主，Contextual Interaction 仅低频辅助
8.01 vs XYUI-6 6.19     STATE SEMANTICS REF：图表数据缺失/连接断开/采样停止/离线/过期 = REF XYUI-6 6.19 状态语义
                        （PartialFailure/Offline/Stale）+ XYUI-4 视觉承载；已有数据不得因局部失败清空图表
其余 10 项              全部按设计稿内建联动关系 + 全局所有权边界落实 REF / REUSE / COMPOSE
```

## 跨组件语义审计（T10 · 12 对）

```text
Line vs Area            折线 = 趋势读数；面积 = 只有面积本身有语义（累计/构成/区间）时才用；
                        禁止只为饱满把 Line 强行改 Area
量面积 vs 区间面积       累计量/总量/构成/差异（方案1/2/4）vs 上下限/波动带/置信区间（方案3）；
                        语义不同，实现和命名必须分开，不得混成一个模糊组件
Bar vs Area/Line        离散对象比较 vs 连续时间轴趋势；连续时间轴构成优先 Area Chart
Scatter vs Distribution 双变量关系/聚类/象限 vs 单变量分布；Scatter 非默认图表（比较对象数值→Bar、
                        时间趋势→Line、分布→Distribution）
Histogram vs Box vs Percentile   完整形状 vs 多组摘要 vs 紧凑摘要；摘要图不得强行替代完整分布图
Gauge vs ProgressRange  共享阈值语义（8-13 REF 8-04）；Gauge 不重复定义 Normal/Notice/Warning/Critical
Timeline vs Steps       事件时间轴（8-12）vs 流程步骤导航（XYUI-3 3.14）；不合并
Metric vs Card          Metric = 紧凑数据节点（数值优先、信息层级固定）；Card = 装饰容器（禁止化）
Tooltip vs Edge Inspector   Hover 轻量浮动 vs 信息多转边缘检查器；同一 Chart Inspector 体系内切换
Lock vs Compare         Compare 建立在 Lock 之上（先锁定 A 再选 B，输出 A/B/Delta）；不是独立组件
Range Select vs Zoom    区间选择（统计/比较/导出）vs 视图缩放；语义与交互入口分离
Follow Live vs Locked   实时跟随（最新数据）vs 锁定检查（固定时间点）；状态必须明确且互斥表达
```

## 源缺陷登记（SFD）

```text
XYUI8-SFD-001（NON-BLOCKING）
位置：Source 8-10 Scatter Plot「使用原则」段
缺陷：原文复制粘贴错乱——「如果只是比较几个对象数值」下出现「优先 Plot 不作为所有实验默认图表」，
      且另有孤行「只有当存在两个连续 Bar Chart」
裁定：按 A2 先例登记 SOURCE_FORMATTING_DEFECT（≠ DESIGN_AMBIGUITY）；Source 原文不动；
      Canonical 按明确意图落（比较对象数值 → 优先 Bar Chart），canonical 内标注 defect ref
```

## 第二真值扫描（T9）

```text
#hex 硬编码             0
rgb / rgba              0
hsl                    0
旧字体名                0
px                     0 处硬编码值（Source 尺寸为叙述性档位，canonical 收敛为 DIP 组件档位）
重复 State / Selection / Layout 真值   0（全部 REF XYUI-4 / XYUI-5 / Foundation）
```

## 状态

```text
XYUI-8 · Visualization
    16/16 CANONICAL COMPLETE
    MAPPING COMPLETE（132 refs）
    GAPS 1（XYUI8-GAP-001 图表系列色板，NON-BLOCKING）
    SFD 1（XYUI8-SFD-001，非阻塞，Source 原文不动）
    A-CLASS 0
    SECOND TRUTH 0
    BROKEN REF 0
    → CLOSED（用户最终验收 2026-08-13；XYUI-PILOT-R7 轮同步）
```

CLOSED 依据：用户正式验收（「可以判定 XYUI-8 正式 CLOSED」）；Source 非原始附件字节锚定不阻塞 CLOSED（provenance 已记录，Source 保留为不可变证据；重发原文件只做 SHA 复核，不重跑 canonical 管线）。
附：Blender 式工作区拆分/多视口内容不在本 Source；视口操作交互合同已按职责归 8-16（视图变换机制 REF XYUI-5 5.16），若未来出现工作区拆分布局，职责归 XYUI-5 WorkspaceLayout（5.12），不在可视化层重建。
