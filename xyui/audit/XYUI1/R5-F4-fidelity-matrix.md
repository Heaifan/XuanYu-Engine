# XYUI.AVALONIA-R5-F4 · XYUI-1 Fidelity Matrix

审计基线：`43096c96`。Runtime / Gallery / Tests 指 XYUI-1 Final Closeout；用户已完成 Light / Dark Gallery 人工视觉验收。

| ID | Component | Canonical | Mapping | Avalonia Type | Properties | Variants | States | Typography | Color | Geometry | Accessibility | Vector/Glyph | Gallery | Documentation | Tests | GAP | Verdict |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|01|Text|对齐|对齐|XYText|Text|—|—|Primary Body|Primary|—|—|—|真实 Preview|有|R5F4|—|USER VISUAL ACCEPTED|
|02|Label|对齐|对齐|XYLabel|Text|—|—|Medium 500|Primary|—|—|—|真实 Preview|有|R5F4|—|USER VISUAL ACCEPTED|
|03|Caption|对齐|对齐|XYCaption|Text|—|—|Caption|Secondary|—|—|—|真实 Preview|有|既有|—|USER VISUAL ACCEPTED|
|04|Heading|对齐|对齐|XYHeading|Text|Panel/Page|—|Semibold/Bold|Primary|—|—|—|真实 Preview|有|既有|—|USER VISUAL ACCEPTED|
|05|SectionTitle|S-05 对齐|S-05 对齐|XYSectionTitle|Text|—|—|14/600/18|#243744|SoftHeader 28/3 + LeftMark 3×16 #526873|—|Left Mark|真实 Preview|有|R5-F4-F1|—|USER VISUAL ACCEPTED|
|06|Link|对齐|对齐|XYLink|Content|—|Native Button|Body Medium|Link|—|—|—|真实 Preview|有|既有|—|USER VISUAL ACCEPTED|
|07|CodeText|对齐|对齐|XYCodeText|Text|—|—|Mono|Tertiary / Icon.Mark|32 DIP|—|Code mark 8/1.25|真实 Preview|有|既有|—|USER VISUAL ACCEPTED|
|08|MonoText|结构化数据|结构化数据|XYMonoText|Label(Auto) / Value(Auto) / Unit(Auto)|—|—|UI 600 / Mono 400 / UI 600|Secondary / Secondary / Secondary|无 Surface|—|—|M-05A Shared Columns；Gap 20 / 8 DIP|有|既有|—|USER VISUAL ACCEPTED|
|09|Badge|内容自适应标签|内容自适应标签|XYBadge|Text/Variant|Default/Accent|—|Caption Medium|PanelAlt/Tag Accent|Auto × 22；Pointer 11|—|单一 Left Pointer Tag Geometry|真实多实例 Preview|有|R5F4|—|USER VISUAL ACCEPTED|
|10|StatusBadge|对齐|对齐|XYStatusBadge|Text/State|5 states|semantic|Caption Medium|Semantic|Dot + Text|GAP|StatusDot|真实 Preview|有|GAP-004|USER VISUAL ACCEPTED|
|11|StatusDot|对齐|对齐|XYStatusDot|State|5 states|semantic|—|Semantic|8 DIP|GAP|—|真实 Preview|有|GAP-004|USER VISUAL ACCEPTED|
|12|Icon|对齐|对齐|XYIcon|Icon/Size|4 sizes|Active/Disabled|—|Foreground|Stroke|GAP|StreamGeometry|真实 Preview|有|GAP-004|USER VISUAL ACCEPTED|
|13|IconLabel|对齐|对齐|XYIconLabel|Icon/Label|—|—|Body|Primary|Inline|—|Vector|真实 Preview|有|—|USER VISUAL ACCEPTED|
|14|Separator|对齐|对齐|XYSeparator|Variant|6 layouts|—|—|Divider|Thickness/Inset/Orientation|—|—|真实 Preview|有|R5F4|USER VISUAL ACCEPTED|
|15|HelpText|对齐|对齐|XYHelpText|Text|—|—|Caption|Info|Inline mark|—|Vector|真实 Preview|有|—|USER VISUAL ACCEPTED|
|16|ErrorText|对齐|对齐|XYErrorText|Text|—|error|Caption Medium|Error|Inline mark|GAP|Vector|真实 Preview|有|GAP-004|USER VISUAL ACCEPTED|
|17|WarningText|对齐|对齐|XYWarningText|Text|—|warning|Caption Medium|Warning|Inline mark|GAP|Vector|真实 Preview|有|GAP-004|USER VISUAL ACCEPTED|
|18|ShortcutHint|对齐|对齐|XYShortcutHint|Shortcut/Mode|SeparateKeycaps|—|Mono Caption|Secondary|Keycaps|—|—|真实 Preview|有|R5F4|USER VISUAL ACCEPTED|
|19|Tooltip|部分|部分|XYTooltip|6 behavior props|—|Hover contract|Caption|Overlay|280 DIP|GAP|—|真实 Preview|有|GAP-004/005|USER VISUAL ACCEPTED|
|20|RichText|部分|对齐|XYRichText|Text/Strong/Mono|—|—|Mono run fixed|Primary/Secondary|Inline|—|—|真实 Preview|有|GAP-003|USER VISUAL ACCEPTED|
|21|SelectableText|对齐|对齐|XYSelectableText|Text/Variant|Default/Technical|Selection/Hover|Body/Mono|Primary/Secondary + Disabled Gray Mark|No surface / Selected surface|GAP|独立 8 DIP Copy vector / Uniform scale；8 DIP gap|真实双变体 Preview|有|GAP-004|USER VISUAL ACCEPTED|
|22|EmptyText|对齐|对齐|XYEmptyText|Text|—|—|Caption|Tertiary|None|—|无默认 Vector Decoration|真实 Preview|有|R5-F4-F1|—|USER VISUAL ACCEPTED|
|23|SearchHighlight|已回写|已回写|XYSearchHighlight|Text/Match/Mark|—|Match|Body Medium|Primary/Accent + Disabled Gray Mark|8/1 RightTop + 8 gap|—|Search vector（8 DIP Uniform）|真实 Preview|有|R5-F4-F1|USER VISUAL ACCEPTED|
|24|TruncatedText|对齐|对齐|XYTruncatedText|Text/Mode|End/Middle|—|Body/Mono pending|Primary|End/Middle API|—|—|真实 Preview|有|GAP-002|USER VISUAL ACCEPTED|

结论：XYUI-1 24/24 已完成 Canonical、Runtime、Gallery、Light / Dark 与用户人工视觉验收，正式 FROZEN。GAP-002～005 继续保留，不因视觉验收伪装为已解决。XYUI-2 仅解除下一阶段冻结，本轮未开始实装。
