# XYUI.AVALONIA-R5 · XYUI-1 Component Inventory

Source of truth：`xyui/specs/XYUI1/XYUI-1.mapping.json` + `XYUI-1.canonical.md`；Gallery 文档页由同一 Catalog 数据驱动。

| ID | Canonical / 中文 | Avalonia Type | Catalog | Gallery | Usage | Test | Status |
|---|---|---|---|---|---|---|---|
| 1.01 | Text / 普通文本 | XYText | ✓ | ✓ | ✓ | ✓ | READY |
| 1.02 | Label / 字段名称 | XYLabel | ✓ | ✓ | ✓ | ✓ | READY |
| 1.03 | Caption / 辅助信息 | XYCaption | ✓ | ✓ | ✓ | ✓ | READY |
| 1.04 | Heading / 标题 | XYHeading | ✓ | ✓ | ✓ | ✓ | READY |
| 1.05 | SectionTitle / 区块标题 | XYSectionTitle | ✓ | ✓ | ✓ | ✓ | READY |
| 1.06 | Link / 超链接 | XYLink | ✓ | ✓ | ✓ | ✓ | READY |
| 1.07 | CodeText / 代码 / ID | XYCodeText | ✓ | ✓ | ✓ | ✓ | READY |
| 1.08 | MonoText / 等宽数据 | XYMonoText | ✓ | ✓ | ✓ | ✓ | READY |
| 1.09 | Badge / 标签 | XYBadge | ✓ | ✓ | ✓ | ✓ | READY |
| 1.10 | StatusBadge / 状态标签 | XYStatusBadge | ✓ | ✓ | ✓ | ✓ | READY |
| 1.11 | StatusDot / 状态圆点 | XYStatusDot | ✓ | ✓ | ✓ | ✓ | READY |
| 1.12 | Icon / 图标 | XYIcon | ✓ | ✓ | ✓ | ✓ | READY |
| 1.13 | IconLabel / 图标 + 文字 | XYIconLabel | ✓ | ✓ | ✓ | ✓ | READY |
| 1.14 | Separator / 分割线 | XYSeparator | ✓ | ✓ | ✓ | ✓ | READY |
| 1.15 | HelpText / 帮助说明 | XYHelpText | ✓ | ✓ | ✓ | ✓ | READY |
| 1.16 | ErrorText / 错误说明 | XYErrorText | ✓ | ✓ | ✓ | ✓ | READY |
| 1.17 | WarningText / 警告说明 | XYWarningText | ✓ | ✓ | ✓ | ✓ | READY |
| 1.18 | ShortcutHint / 快捷键提示 | XYShortcutHint | ✓ | ✓ | ✓ | ✓ | READY |
| 1.19 | Tooltip / 悬浮提示 | XYTooltip | ✓ | ✓ | ✓ | ✓ | READY |
| 1.20 | RichText / 富文本 | XYRichText | ✓ | ✓ | ✓ | ✓ | READY |
| 1.21 | SelectableText / 可选择文本 | XYSelectableText : SelectableTextBlock | ✓ | ✓ | ✓ | ✓ | READY |
| 1.22 | EmptyText / 空状态文本 | XYEmptyText | ✓ | ✓ | ✓ | ✓ | READY |
| 1.23 | SearchHighlight / 搜索高亮 | XYSearchHighlight | ✓ | ✓ | ✓ | ✓ | READY |
| 1.24 | TruncatedText / 截断文本 | XYTruncatedText | ✓ | ✓ | ✓ | ✓ | READY + GAP-002 |

Foundation 与 Component 分离：Font/FontSize/FontWeight/LineHeight/LetterSpacing 是 Token；`XyuiTextStyles` 是 Style；上表 24 项才是 Component。

Coverage：Canonical 24/24；Avalonia 24/24；Catalog 24/24；Gallery 24/24；Documentation 24/24；Usage 24/24；Tests 24/24（62/62 PASS）；READY 23/24；GAP 1/24；Accounted 24/24。

R5 fidelity：17 项 canonical 视觉语义问题已纳入组件实现、真实 Preview、Usage/API/Token 文档和回归测试；自动测试通过仍保持 READY FOR USER ACCEPTANCE，不宣称 CLOSED。
