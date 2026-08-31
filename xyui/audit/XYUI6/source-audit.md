# XYUI-6 Source Audit & Inventory / 源审计与清点

- 阶段：`XYUI-PILOT-R6 · FAST-CLOSE · T2 Inventory`
- 依据：`xyui/source/XYUI6/XYUI-6.md`（不可变证据源）
- 原稿出处：Hermes desktop-attachments `XYUI-6.md`（2026-08-13 19:30，161,785 bytes）
- 冻结 SHA-256：`15dcf491e7d2512994e2c140b1ea4a526b58aec21f70935d5faeef8f8e92e358`（原始稿 SHA `a52fd6cd…`，仅清理 19 处行尾空白，语义零改动，原始 SHA 记入 commit message）
- 上游：A2 Foundation Registry（VALIDATED + AMEND-A/B）+ XYUI-1/2/3/4/5 canonical

## 20 组件清点

| 编号 | 组件 | 行范围 | 存在 |
|---|---|---|---|
| 6.01 | List / 列表 | 1–285 | ✅ |
| 6.02 | Table / 表格 | 286–543 | ✅ |
| 6.03 | Data Grid / 数据网格 | 544–790 | ✅ |
| 6.04 | Property Grid / 属性网格 | 791–1050 | ✅ |
| 6.05 | Hierarchical Data View / 层级数据视图 | 1051–1311 | ✅ |
| 6.06 | Item View / 条目视图 | 1312–1554 | ✅ |
| 6.07 | Asset Grid / 资源网格 | 1555–1832 | ✅ |
| 6.08 | Collection Header / 集合头部 | 1833–2032 | ✅ |
| 6.09 | Collection Toolbar / 集合工具栏 | 2033–2287 | ✅ |
| 6.10 | Column / 列 | 2288–2523 | ✅ |
| 6.11 | Row / 行 | 2524–2778 | ✅ |
| 6.12 | Cell / 单元格 | 2779–3124 | ✅ |
| 6.13 | Sorting / 排序 | 3125–3333 | ✅ |
| 6.14 | Filtering / 筛选 | 3334–3615 | ✅ |
| 6.15 | Grouping / 分组 | 3616–3826 | ✅ |
| 6.16 | Expandable Row / 展开行 | 3827–4028 | ✅ |
| 6.17 | Inline Editing / 行内编辑 | 4029–4317 | ✅ |
| 6.18 | Bulk Operations / 批量操作 | 4318–4577 | ✅ |
| 6.19 | Collection State / 集合状态 | 4578–4905 | ✅ |
| 6.20 | Virtualized Collection / 虚拟化大数据集合 | 4906–5269 | ✅ |

- 编号连续 6.01~6.20，无缺失、无重复：**20/20 清点完成**。

## 逐项责任 / 方案结构 / 上游依赖（Initial Classification）

| 编号 | 组件 | 核心责任 | 主要方案与 Variant | 上游依赖 |
|---|---|---|---|---|
| 6.01 | List | 同类数据对象的连续线性承载与信息组织 | 方案4 Rich List 主；Compact / Detail / Block 同组件 Variant | XYUI-4 Selection/Focus；XYUI-5 滚动；6.20 虚拟化；XYUI-1 文本 |
| 6.02 | Table | 二维结构化数据浏览、比较、理解（非重编辑） | 方案4 Compact Rich Table 主；Linear / ColumnStructured / Zebra | XYUI-4；6.13/6.14/6.20；XYUI-1 文本 |
| 6.03 | Data Grid | 大量结构化数据浏览/选择/编辑/批量操作；Row Selection·Active Cell·Editing Cell 三态分离 | 方案3 Hybrid Grid | XYUI-2 输入；XYUI-4；6.13/6.14/6.15/6.17/6.18/6.20；XYUI-5 滚动 |
| 6.04 | Property Grid | Inspector 式结构化属性查看/编辑 | 方案4 Hybrid Compact Inspector + 方案2 Adaptive Split + 方案3 Stacked 降级 | XYUI-2 输入（16 类）；XYUI-4 Validation；XYUI-5 滚动；6.17 |
| 6.05 | Hierarchical Data View | 父子/包含/归属关系数据集合（≠ XYUI-3 导航树） | 方案2 Guide Line Tree 主；Compact / Rich / TreeTable | XYUI-4；6.14/6.20；XYUI-1 |
| 6.06 | Item View | 单数据对象在集合中的信息表达模型（被 List/Grid/Search/Picker 复用） | 方案3 Rich Item 主；Single-Line / Two-Level；方案4 = Adaptive Slot 裁剪 | XYUI-1；XYUI-4 |
| 6.07 | Asset Grid | 以视觉预览为主要识别方式的资源管理 | 方案3 Rich Asset Grid 主；PreviewFirst / Compact / Dense | XYUI-4；6.13/6.14/6.15/6.20 |
| 6.08 | Collection Header | 集合身份/关键状态/最高频核心操作 | 方案4 Adaptive Command Header 主；Minimal Variant | XYUI-2；XYUI-4；6.13/6.14/6.18/6.19 |
| 6.09 | Collection Toolbar | 搜索/筛选/排序/分组/视图切换/显示控制 | 方案4 Adaptive Work Toolbar 主；Compact Linear 辅助 | XYUI-2 Search Field；XYUI-3 ViewSwitcher；6.13/6.14/6.15/6.18/6.19 |
| 6.10 | Column | 列布局规则：列宽/对齐/优先级/响应式隐藏 | 方案4 Priority Adaptive Column + 方案3 Content-Aware Width | 6.13/6.14；XYUI-5 横向滚动 |
| 6.11 | Row | 结构化集合单数据对象承载（≠ Cell 容器） | 方案4 Adaptive Rich Row + 方案2 Rich Data + 方案3 Action-Aware；方案1 Minimal | XYUI-4；6.03/6.10/6.16/6.20 |
| 6.12 | Cell | 最小数据表达单元；DataType × InteractionMode 独立建模 | 方案4 Adaptive Semantic Cell + 方案2 Semantic + 方案3 Direct Editable；方案1 Plain | XYUI-2 输入；XYUI-4 Feedback/ProgressBar；6.03/6.20 |
| 6.13 | Sorting | 集合排列顺序控制；语义化表达（↑↓ 仅辅助） | 方案2 Visible Sort Control 主；方案4 Context-Aware Entry；方案3 Multi-Sort 高级 | XYUI-2 控件；6.03/6.09/6.10 |
| 6.14 | Filtering | 结构化条件筛选；Quick Token + Advanced Builder（AND/OR/NOT） | 方案4 Adaptive Filter System + 方案3 Professional Filter Builder | XYUI-2 输入；XYUI-4 反馈；6.13/6.15/6.20 |
| 6.15 | Grouping | 按业务字段归类展示；组级摘要；禁卡片嵌套 | 方案3 Rich Summary Group 主；方案4 多级能力改用 Tree Guide Line | 6.05 Guide Line；6.09/6.13/6.14；XYUI-1 |
| 6.16 | Expandable Row | 不离集合上下文查看行扩展信息 | 方案2 Rich Expanded Panel 主视觉 + 方案4 Adaptive Expansion 行为 | XYUI-4；6.03/6.11/6.20 |
| 6.17 | Inline Editing | 集合上下文内的编辑流程：触发/提交/取消/验证/保护 | 方案4 Adaptive + 方案2 Direct + 方案3 ContextualCommit + 方案1 Explicit | XYUI-2；XYUI-4 Validation；6.03/6.12/6.18 |
| 6.18 | Bulk Operations | 多选对象的批量操作；Applicable Set（Full/Partial/None） | 方案3 Rich Selection Summary + 方案4 Bulk Workbench + 方案2 Contextual Bulk Bar | XYUI-4 Selection；6.04/6.08/6.09/6.17 |
| 6.19 | Collection State | 集合非正常状态的统一表达（13 态：InitialEmpty/FilteredEmpty/SearchEmpty/Loading/PartialFailure/FullFailure/…） | 方案4 State Diagnostic System 主；方案3 Inline 原则；方案2 Contextual Empty；方案1 仅 InitialEmpty | XYUI-4 EmptyState/LoadingIndicator/Skeleton/InlineFeedback；6.14 |
| 6.20 | Virtualized Collection | 超大数据集合的数据层虚拟化合同（Identity/Anchor/流式/增量） | 方案4 Professional Virtual Data View 主；方案1 Invisible；方案2 Range Awareness；方案3 Progressive Loading | XYUI-5 VirtualizedLayout（布局机制）；XYUI-4；6.03/6.13/6.14/6.15/6.16/6.18/6.19 |

## Initial Classification 汇总

- 全部 20 项为 `NEW`（XYUI-6 数据与集合语义层新组件），与 XYUI-1~5 大量组合，但**不重新拥有**上游 Primitive。
- 所有权关系固定：Text→XYUI-1；Input/Action/Search→XYUI-2；Navigation/Pagination/Switching→XYUI-3；Hover/Selected/Focus/Feedback→XYUI-4；Layout/Scroll/Virtualization/Container→XYUI-5；Data & Collection Semantics→XYUI-6。
- 预判跨组件裁定点（进入 T4 对账）：6.03 DataGrid 正式归属（修正 XYUI-5 mapping 的 `XYUI-1 DataGrid` 错误引用）；6.09 View 切换 vs XYUI-3 ViewSwitcher；6.20 数据层合同 vs XYUI-5 5.14 VirtualizedLayout 机制；6.19 vs XYUI-4 4.20 EmptyState；6.13 Sorting 方向指示 vs ↑↓ 原则；6.15 Data Group vs XYUI-4 4.06 SelectionGroup。

## 状态

`INVENTORY COMPLETE · READY FOR RECONCILIATION & CANONICALIZATION`
