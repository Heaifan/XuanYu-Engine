# XYUI.AVALONIA-R5-F4 · XYUI-1 Fidelity Matrix

审计基线：`43096c96`。Runtime / Gallery / Tests 指当前 R5-F4 工作树；Visual Accepted 在用户真机验收前统一为 0。

| ID | Component | Canonical | Mapping | Avalonia Type | Properties | Variants | States | Typography | Color | Geometry | Accessibility | Vector/Glyph | Gallery | Documentation | Tests | GAP | Verdict |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|01|Text|对齐|对齐|XYText|Text|—|—|Primary Body|Primary|—|—|—|真实 Preview|有|R5F4|—|READY FOR VISUAL ACCEPTANCE|
|02|Label|对齐|对齐|XYLabel|Text|—|—|Medium 500|Primary|—|—|—|真实 Preview|有|R5F4|—|READY FOR VISUAL ACCEPTANCE|
|03|Caption|对齐|对齐|XYCaption|Text|—|—|Caption|Secondary|—|—|—|真实 Preview|有|既有|—|READY FOR VISUAL ACCEPTANCE|
|04|Heading|对齐|对齐|XYHeading|Text|Panel/Page|—|Semibold/Bold|Primary|—|—|—|真实 Preview|有|既有|—|READY FOR VISUAL ACCEPTANCE|
|05|SectionTitle|对齐|对齐|XYSectionTitle|Text|—|—|Section|Primary|Divider|—|无默认 Vector Mark|真实 Preview|有|R5-F4-F1|—|RECONCILIATION COMPLETE · BUILD BLOCKED|
|06|Link|对齐|对齐|XYLink|Content|—|Native Button|Body Medium|Link|—|—|—|真实 Preview|有|既有|—|READY FOR VISUAL ACCEPTANCE|
|07|CodeText|对齐|对齐|XYCodeText|Text|—|—|Mono|Tertiary|32 DIP|—|Code mark 8/1|真实 Preview|有|既有|—|READY FOR VISUAL ACCEPTANCE|
|08|MonoText|对齐|对齐|XYMonoText|Text|—|—|Foundation Mono|Secondary|无 Surface|—|—|M-05A|有|既有|—|READY FOR VISUAL ACCEPTANCE|
|09|Badge|已回写|已回写|XYBadge|Text/Variant|Default/Accent|—|Caption Medium|PanelAlt/Accent|22/11 DIP|—|Tag Geometry|真实 Preview|有|R5F4|—|READY FOR VISUAL ACCEPTANCE|
|10|StatusBadge|对齐|对齐|XYStatusBadge|Text/State|5 states|semantic|Caption Medium|Semantic|Dot + Text|GAP|StatusDot|真实 Preview|有|GAP-004|READY WITH GAP|
|11|StatusDot|对齐|对齐|XYStatusDot|State|5 states|semantic|—|Semantic|8 DIP|GAP|—|真实 Preview|有|GAP-004|READY WITH GAP|
|12|Icon|对齐|对齐|XYIcon|Icon/Size|4 sizes|Active/Disabled|—|Foreground|Stroke|GAP|StreamGeometry|真实 Preview|有|GAP-004|READY WITH GAP|
|13|IconLabel|对齐|对齐|XYIconLabel|Icon/Label|—|—|Body|Primary|Inline|—|Vector|真实 Preview|有|—|READY FOR VISUAL ACCEPTANCE|
|14|Separator|对齐|对齐|XYSeparator|Variant|6 layouts|—|—|Divider|Thickness/Inset/Orientation|—|—|真实 Preview|有|R5F4|READY FOR VISUAL ACCEPTANCE|
|15|HelpText|对齐|对齐|XYHelpText|Text|—|—|Caption|Info|Inline mark|—|Vector|真实 Preview|有|—|READY FOR VISUAL ACCEPTANCE|
|16|ErrorText|对齐|对齐|XYErrorText|Text|—|error|Caption Medium|Error|Inline mark|GAP|Vector|真实 Preview|有|GAP-004|READY WITH GAP|
|17|WarningText|对齐|对齐|XYWarningText|Text|—|warning|Caption Medium|Warning|Inline mark|GAP|Vector|真实 Preview|有|GAP-004|READY WITH GAP|
|18|ShortcutHint|对齐|对齐|XYShortcutHint|Shortcut/Mode|SeparateKeycaps|—|Mono Caption|Secondary|Keycaps|—|—|真实 Preview|有|R5F4|READY FOR VISUAL ACCEPTANCE|
|19|Tooltip|部分|部分|XYTooltip|6 behavior props|—|Hover contract|Caption|Overlay|280 DIP|GAP|—|真实 Preview|有|GAP-004/005|READY WITH GAP|
|20|RichText|部分|对齐|XYRichText|Text/Strong/Mono|—|—|Mono run fixed|Primary/Secondary|Inline|—|—|真实 Preview|有|GAP-003|READY WITH GAP|
|21|SelectableText|对齐|对齐|XYSelectableText|Text/Variant|Default/Technical|Selection/Hover|Body/Mono|Primary|No surface|GAP|Copy vector|真实 Preview|有|GAP-004|READY WITH GAP|
|22|EmptyText|对齐|对齐|XYEmptyText|Text|—|—|Caption|Tertiary|None|—|无默认 Vector Decoration|真实 Preview|有|R5-F4-F1|—|RECONCILIATION COMPLETE · BUILD BLOCKED|
|23|SearchHighlight|已回写|已回写|XYSearchHighlight|Text/Match/Mark|—|Match|Body Medium|Primary/Accent|8/1 RightTop|—|Search vector（仅搜索语义）|真实 Preview|有|R5-F4-F1|RECONCILIATION COMPLETE · BUILD BLOCKED|
|24|TruncatedText|对齐|对齐|XYTruncatedText|Text/Mode|End/Middle|—|Body/Mono pending|Primary|End/Middle API|—|—|真实 Preview|有|GAP-002|READY WITH GAP|

结论：24/24 已有真实类型、Gallery Preview、文档入口和测试基础；其中 11 个组件仍带有已登记 GAP 或待补 Accessibility/Tooltip/RichText Link 合同，全部等待用户真机验收，未宣称 CLOSED。
