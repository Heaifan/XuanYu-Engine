# XYUI-6 Reconciliation & Closeout / 全量对账收口

- 状态：`XYUI-6 · RECONCILED · READY FOR USER ACCEPTANCE`
- 阶段：`XYUI-PILOT-R6 · FAST-CLOSE ONE ROUND`
- Source：`xyui/source/XYUI6/XYUI-6.md`（IMMUTABLE，SHA `15dcf491…`，161,709 bytes，5,269 行；原始稿 SHA `a52fd6cd…`，仅清理行尾空白）
- Canonical：`xyui/specs/XYUI6/XYUI-6.canonical.md`（1,376 行，20/20）
- 上游：Foundation Registry（VALIDATED + AMEND-A/B）+ XYUI-1/2/3/4/5 canonical

## 20/20 对账矩阵

| 项 | Source | Canonical | Mapping | 冲突处置 | GAP | 第二真值 |
|---|---|---|---|---|---|---|
| 6.01 List | ✅ | ✅ | ✅ | 四形态同一组件 Variant；Selection/滚动/虚拟化全 REF 上游 | — | 0 |
| 6.02 Table | ✅ | ✅ | ✅ | NEW（≠ XYUI-5 Grid 空间布局；Zebra=Surface 交替语义） | — | 0 |
| 6.03 Data Grid | ✅ | ✅ | ✅ | NEW（XYUI DataGrid 正式组件；修正 XYUI-5 mapping 错误引用「XYUI-1 DataGrid」→ 本项） | — | 0 |
| 6.04 Property Grid | ✅ | ✅ | ✅ | 输入 16 类全 REF XYUI-2；Mixed Value 规则与 6.18 共享 | — | 0 |
| 6.05 Hierarchical Data View | ✅ | ✅ | ✅ | DATA RELATIONSHIP OWNERSHIP（数据关系 vs XYUI-3 TreeNavigation 导航，语义分离不合并） | — | 0 |
| 6.06 Item View | ✅ | ✅ | ✅ | 信息模型层（被各集合组件复用）；P0~P3 为 XYUI-6 集合层机制 | — | 0 |
| 6.07 Asset Grid | ✅ | ✅ | ✅ | Preview 语义归本项；Mini Progress 场景 REF 4-16 | — | 0 |
| 6.08 Collection Header | ✅ | ✅ | ✅ | 身份/状态/核心操作；观察整理工具归 6.09 | — | 0 |
| 6.09 Collection Toolbar | ✅ | ✅ | ✅ | VIEW SWITCH REF（ViewMode 切换控制 = XYUI-3 ViewSwitcher；本项只定义数据集视图模式集合） | — | 0 |
| 6.10 Column | ✅ | ✅ | ✅ | 列布局规则层；冻结边界 = XY.Border.Color.Subtle（不引入阴影） | — | 0 |
| 6.11 Row | ✅ | ✅ | ✅ | Row ≠ Cell 容器（代表完整数据对象）；QuickAction 最多 1~2 | — | 0 |
| 6.12 Cell | ✅ | ✅ | ✅ | DataType × InteractionMode 独立建模；Mini Bar REF 4-16 Inline Compact | — | 0 |
| 6.13 Sorting | ✅ | ✅ | ✅ | SEMANTIC SORT EXPRESSION（字段+方向语义为主；↑↓ 仅辅助 → GAP-001） | 1 | 0 |
| 6.14 Filtering | ✅ | ✅ | ✅ | SEARCH/FILTER 分离（Search=XYUI-2 全文语义；Filter=本项结构化条件） | — | 0 |
| 6.15 Grouping | ✅ | ✅ | ✅ | DATA GROUP OWNERSHIP（vs XYUI-4 SelectionGroup 选择集合；No Card Nesting；多级强制 Tree Guide Line） | — | 0 |
| 6.16 Expandable Row | ✅ | ✅ | ✅ | Expanded 与 Selected 分离；展开语义不变、承载可变 | — | 0 |
| 6.17 Inline Editing | ✅ | ✅ | ✅ | 编辑流程统一规则层（Cell 承载、本项流程）；风险比例保护 | — | 0 |
| 6.18 Bulk Operations | ✅ | ✅ | ✅ | Applicable Set 必算；Partial 必须显示实际数量；Undo 单一逻辑单元 | — | 0 |
| 6.19 Collection State | ✅ | ✅ | ✅ | STATE SEMANTICS OWNERSHIP（13 态语义归本项；视觉承载 REF XYUI-4 EmptyState/Loading/Skeleton/InlineFeedback） | — | 0 |
| 6.20 Virtualized Collection | ✅ | ✅ | ✅ | DATA-LAYER CONTRACT（数据层合同归本项；机制 REF XYUI-5 5.14 VirtualizedLayout；禁 DataVirtualizationEngine） | — | 0 |

## 全量统计

```text
Source accounted        20/20
Canonical accounted     20/20（1,376 行）
Mapping accounted       20/20（200 refs）
  CANONICAL_REF         26
  NAMESPACE_REF         62
  COMPONENT_SPECIFIC    107
  COMPOSE               4
  GAP                   1
GAP reconciled          1（XYUI6-GAP-001，NON-BLOCKING；0 项遗漏 Token 复用）
A-Class unresolved      0（无已 CLOSED 核心合同互斥，无需改 Foundation）
Second Truth            0（hex 0 / rgb 0 / hsl 0 / 旧字体 0 / 旧命名空间 0；px 16 处叙述→DIP 收敛）
Broken Ref              0（200 引用全部解析；上游 1~5 组件清单逐项核对）
Source Mutation         0
Duplicate Contract      0
Semantic Ambiguity      0（9 对跨组件语义逐对裁定，见下）
```

## 裁定落地清单（6 项所有权划清）

```text
6.03 DataGrid        正式归属本项；修正 XYUI-5 mapping「DataGrid 边界 = XYUI-1 DataGrid」
                     →「= XYUI-6 6-03 Data Grid」（XYUI-1 无 DataGrid，属上一轮错误引用，本轮跨组件审计发现并修正；
                     XYUI-5 canonical 正文无需改动——其「Grid Layout 是纯布局器」表述与本项一致）
6.05 Hierarchy       Data Relationship 归本项；Tree Navigation 导航语义归 XYUI-3（两者语义分离，不合并、不互相替代）
6.09 ViewArea        View Mode 切换控制 = REF XYUI-3 ViewSwitcher；本项只拥有数据集视图模式集合
                     （List | Grid | Table | Compact）与工作状态可见性
6.15 Data Group      Data Group（数据归类）= 本项；SelectionGroup（选择集合）= XYUI-4 4.06；Layout/Navigation Group = 上游
6.19 vs 4.20         EmptyState 视觉承载 = REF XYUI-4 4.20（仅真正 Initial Empty 使用 Simple Center State）；
                     本项拥有 13 态语义（InitialEmpty/FilteredEmpty/SearchEmpty 必须区分原因、影响与恢复路径）
6.20 vs 5.14         Virtualization 布局机制（Viewport/Overscan/Recycling/SizeCache/ScrollAnchor）= REF XYUI-5 5.14；
                     本项拥有数据层合同（Stable Identity/Anchor/状态持久化/Progressive/Streaming/Follow/TotalCount）；
                     禁止建立 DataVirtualizationEngine 第二套布局基础设施
其余 14 项            全部按设计稿内建 Ownership Check + 全局所有权边界落实 REF / REUSE / COMPOSE
```

## 跨组件语义审计（T10 · 9 对）

```text
List vs Table            List = 单对象集合（对象级信息组织）；Table = 二维结构化数据（行列比较）。
                         不合并：Table 拥有 Column/Header/Footer 结构语义，List 拥有条目信息组织。
Table vs DataGrid        Table = 浏览/比较/理解（编辑非默认职责）；DataGrid = 选择/编辑/批量操作
                         （Row Selection/Active Cell/Editing Cell 三态分离）。复杂编辑统一交 Data Grid。
Filter vs Search         Search = 全文/关键词匹配（REF XYUI-2 Search Field）；Filter = 结构化条件（本项）。
                         可同时存在（结果交集），不得混成同一个不可解释状态。
Sort vs Group            Sort = 顺序；Group = 归类。Group Order 与组内排序分离；Filtering 先决定集合，
                         Sorting 再决定顺序，Grouping 决定归类展示。
Pagination vs Virtualization  Pagination 导航合同 = XYUI-3（3.13）；数据集侧 total count / page-size 投影
                         = 本项集合语义；Virtualization = 渲染窗口机制（5.14）+ 数据层合同（6.20）。COMPOSE。
Selection vs Active Row  Selection = 对象级持久状态（XYUI-4）；Active Row/Cell = 焦点上下文（6.03 Data Grid）。
                         Row Selected ≠ Active Cell ≠ Editing Cell；Expanded 与 Selected 也分离（6.16）。
Data Group vs SelectionGroup  数据归类（6.15）vs 选择集合（XYUI-4 4.06）。不合并。
Empty vs No Results      EmptyState 视觉承载（XYUI-4 4.20）；InitialEmpty / FilteredEmpty / SearchEmpty
                         语义区分（6.19）——FilteredEmpty 必须显示原始集合数量，不得暗示数据被删除。
Loading vs Updating      Loading = 数据获取（视觉 REF XYUI-4；6.19 语义：首次/增量区分）；
                         Updating = 局部刷新（已有数据继续可用，局部更新，禁整屏 Loading 清空集合）。
```

## 第二真值扫描（T9）

```text
#hex 硬编码             0
rgb / rgba              0
hsl                    0
旧字体名                0（5 处「Inter」命中均为 InteractionMode 一词，非字体）
旧命名空间（未收敛旧名） 0
px                     16 处（全部为叙述性尺寸档位，canonical 收敛为 DIP 组件档位并声明 COMPONENT_SPECIFIC）
重复 State / Selection / Layout 真值   0（全部 REF XYUI-4 / XYUI-5 / Foundation）
```

## 状态

```text
XYUI-6 · Data & Collections
    20/20 CANONICAL COMPLETE
    MAPPING COMPLETE（200 refs）
    GAPS 1（XYUI6-GAP-001 排序方向指示符，NON-BLOCKING）
    A-CLASS 0
    SECOND TRUTH 0
    BROKEN REF 0
    → READY FOR USER ACCEPTANCE
```

唯一未 CLOSED 原因：`XYUI-A-plan.md` 明文规定该阶段须用户最终裁定才能 CLOSED（不得伪造用户验收）。
