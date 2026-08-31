# XYUI0 Source Audit

> XYUI-A1-R1-F1 · Source Intake + Evidence Mapping 审计稿（F1 分类修正版，给人审）
>
> - 源文件：`xyui/source/XYUI0/XYUI-0.md`（不可变 Evidence Source）
> - SHA-256：`e564a4b4bffc518b5df3144ca5834995ce7abd6b649853f2fa845afa3658d0b6`（F1 后未变）
> - source_id：`XYUI-SOURCE-000` · source_type：xmind-markdown-export
> - 状态：`READY FOR USER ACCEPTANCE`（本稿只建 Evidence Chain，不产生正式 APPROVED）
> - F1 修正依据：用户人工复核裁定（2026-08-13）

## 一、统计（F1 修正后）

| 指标 | 数值 |
|---|---|
| 总项数（一级 32 + 子节 12） | 44 |
| CLEAR（设计语义可确定） | **44** |
| DESIGN_AMBIGUOUS | **0** |
| MISSING | 0 |
| SOURCE_FORMATTING_DEFECT（Source 层级异常，非设计未定） | **3** |
| TOKEN_LAYER_OVERLAP（Token 分层重叠，非 CONFLICT） | **2** |
| FORMAL APPROVED / TOKEN GENERATED | 0 / 0 |

> Formatting Defect ≠ 设计没定；两者不得混入同一分类。

## 二、SOURCE_FORMATTING_DEFECT 登记（3 处，Source 原文不修改）

| ID | 位置 | 原文异常 | 语义裁定 |
|---|---|---|---|
| XYUI-SFD-001 | 0.13 L1212-1215 | `XY.Shadow.Control` 12 空格缩进嵌在 `XY.Shadow.Panel`(8 空格) 之下 | Panel 与 Control 语义同级（均 Value=None） |
| XYUI-SFD-002 | 0.3-A L709-712 | `XY.Font.Mono` 12 空格缩进嵌在 `XY.Font.Default`(8 空格) 之下 | Mono 为独立 Font Token（FontFamily=Source Code Pro），与 UI/Default/Technical 同级 |
| XYUI-SFD-003 | 0.24 L1792-1793 | `Role = Tooltip` 与 `XY.Overlay.TooltipHost` 同级(8 空格)，未缩进为子级（对比 ModalHost Role 为 12 空格） | 语义关系：`XY.Overlay.TooltipHost` → `Role = Tooltip` |

## 三、TOKEN_LAYER_OVERLAP 登记（2 处，非 CONFLICT）

| ID | 重叠对 | 分层说明 | 处置 |
|---|---|---|---|
| XYUI-TLO-001 | `XY.Color.App=#F1F4F6` ↔ `XY.Surface.App=#EEF2F5` | 0.2-A 基础色彩母版 vs 0.2-C Semantic Surface；0.12 明确 `XY.SurfaceRole.App → XY.Surface.App` 为实际消费 | R1 不改名/不删/不合并；A2 Registry/Token 定 canonical mapping |
| XYUI-TLO-002 | `XY.Color.Panel=#F7F9FA` ↔ `XY.Surface.Panel=#F5F8FA` | 同上（Panel 语义） | 同上 |

> 实际代码调用路径：`Panel → XY.SurfaceRole.Panel → XY.Surface.Panel`，而非 `XY.Color.Panel`。

## 四、逐项审计

### XYUI0-0.1 · Design Principles｜设计原则

- 位置：L1-169 · 类型：foundation-principle
- 最终选择语句：设计宪法 / Design Constitution
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL18-19 — 方案名称：设计宪法 / Design Constitution
  - `COMMENT` LL20-66 — 核心原则 P1~P8（编辑任务优先/语义辨识优先/高信息密度/视觉克制/Canvas 优先/状态明确即时可预测/自适应离散稳定可恢复/工程可执行可验证）
  - `COMMENT` LL67-84 — 决策优先级 Priority.1~6 + 冲突规则（高优先级覆盖低优先级；禁为装饰牺牲正确性；禁为形式统一牺牲语义辨识；禁为紧凑牺牲热区）
  - `COMMENT` LL85-96 — 强制等级 MUST/SHOULD/MAY/FORBIDDEN
  - `COMMENT` LL97-117 — 禁止项 10 条（Cardification/DecorationFirst/ColorOnlyState/MagicNumber/RandomSpacing/SemanticAm…
  - `COMMENT` LL118-136 — 默认底线（Accessibility 默认能力/DPI 不破坏结构/Localization 不破坏布局）+ 工程落地五对象

### XYUI0-0.2 · Color System｜色彩系统（总纲）

- 位置：L170-677 · 类型：foundation-color
- 最终选择语句：见子节 0.2-A~I
- 判断：**CLEAR**
- Evidence：
  - `COMMENT` LL170 — 总节，含 9 子节：0.2-A 核心色彩方向 / B 文字颜色层级 / C 背景层级 / D 边框与分割线 / E 强调色层级 / F 交互状态色 / G 语义色 / H 禁用只读…
- possible_relation：XYUI0-0.2-A, XYUI0-0.2-B, XYUI0-0.2-C, XYUI0-0.2-D, XYUI0-0.2-E, XYUI0-0.2-F, XYUI0-0.2-G, XYUI0-0.2-H, XYUI0-0.2-I

### XYUI0-0.2-A · Color System｜核心色彩方向

- 位置：L171-238 · 类型：foundation-color
- 最终选择语句：冷灰湖蓝 / Cool Gray Lake Blue
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL180 — 方案名称：冷灰湖蓝 / Cool Gray Lake Blue
  - `COMMENT` LL182-193 — 结构特征：冷灰基础+湖蓝 Accent、浅色高明度冷灰白、深色深蓝灰非纯黑、主色低饱和、状态色低饱和绿/金褐/红、Light/Dark 同一套语义 Token、禁控件硬编码颜色
  - `COMMENT` LL194-238 — UI代码 12 组双主题色：App/Panel/Raised/Border/Text.Primary/Text.Secondary/Accent/Hover/Selected/Su…
- **TOKEN_LAYER_OVERLAP**：XYUI-TLO-001（XY.Color.App/#F1F4F6 ↔ XY.Surface.App/#EEF2F5）→ 0.2-A 为基础色彩母版(Base Palette)，0.2-C 为 Semantic Surface；0.12 已明确 XY.SurfaceRole.* →…
- 备注：TOKEN_LAYER_OVERLAP（非 CONFLICT）：职责分层不同（色彩母版 vs Semantic Surface），由 0.12 SurfaceRole 决定实际消费；R1 忠实记录，A2 处理。
- possible_relation：XYUI0-0.2-C, XYUI0-0.2-G

### XYUI0-0.2-B · Text Color｜文字颜色层级

- 位置：L239-281 · 类型：foundation-color
- 最终选择语句：柔和层级 / Soft Hierarchy
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL247 — 方案名称：柔和层级 / Soft Hierarchy
  - `COMMENT` LL249-255 — 结构特征：Primary 正文清晰/Secondary 弱于正文可读/Tertiary 用于 ID 元数据/Placeholder 再弱化/Disabled 最低/Link 低饱和…
  - `COMMENT` LL256-281 — UI代码六档双主题：Primary/Secondary/Tertiary/Placeholder/Disabled/Link
- possible_relation：XYUI0-0.2-A, XYUI0-0.2-E

### XYUI0-0.2-C · Surface｜背景层级

- 位置：L282-339 · 类型：foundation-color
- 最终选择语句：清晰四层 / Clear Four-Level Surface
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL290 — 方案名称：清晰四层 / Clear Four-Level Surface
  - `COMMENT` LL292-304 — 结构特征：App 最低/Panel 普通工具区/PanelAlt 轻微抬升/Raised 输入框与属性块/Overlay 浮层/Canvas 与工具面板明显区分/明度逐层抬升/不依…
  - `COMMENT` LL305-339 — UI代码十档双主题：App/Panel/PanelAlt/Raised/Canvas/Toolbar/Input/Overlay/Selected/BorderReference
- **TOKEN_LAYER_OVERLAP**：XYUI-TLO-002（XY.Color.Panel/#F7F9FA ↔ XY.Surface.Panel/#F5F8FA）→ 0.2-A 为基础色彩母版(Base Palette)，0.2-C 为 Semantic Surface；0.12 已明确 XY.SurfaceRole.* →…
- 备注：TOKEN_LAYER_OVERLAP（非 CONFLICT）：职责分层不同（色彩母版 vs Semantic Surface），由 0.12 SurfaceRole 决定实际消费；R1 忠实记录，A2 处理。
- possible_relation：XYUI0-0.2-A, XYUI0-0.12

### XYUI0-0.2-D · Border / Divider｜边框与分割线

- 位置：L340-381 · 类型：foundation-color
- 最终选择语句：编辑器强边界 / Strong Editor Border
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL348 — 方案名称：编辑器强边界 / Strong Editor Border
  - `COMMENT` LL350-358 — 结构特征：普通边框清晰/Strong 强于 Default/Selected 边界独立/Focus Ring 与 Selected 分离/Section 分割线清楚不喧宾夺主/输入…
  - `COMMENT` LL359-381 — UI代码六档双主题：Subtle/Default/Strong/Divider.Default/Focus/Selected
- possible_relation：XYUI0-0.10, XYUI0-0.11

### XYUI0-0.2-E · Accent｜强调色层级

- 位置：L382-427 · 类型：foundation-color
- 最终选择语句：均衡双层强调 / Balanced Dual-Level Accent
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL390 — 方案名称：均衡双层强调 / Balanced Dual-Level Accent
  - `COMMENT` LL392-402 — 结构特征：Accent 默认强调/AccentSoft 浅色选中与标签背景/AccentStrong 主按钮与当前工具/Link 与 Accent 一致/避免大面积蓝色/强调层级 …
  - `COMMENT` LL403-427 — UI代码七项双主题：Default/Soft/Strong/Link/Tool.Active/Button.Primary/Tag.Accent
- possible_relation：XYUI0-0.2-B, XYUI0-0.2-F

### XYUI0-0.2-F · Interaction States｜交互状态色

- 位置：L428-479 · 类型：foundation-color
- 最终选择语句：编辑器语义状态 / Semantic Editor States
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL436 — 方案名称：编辑器语义状态 / Semantic Editor States
  - `COMMENT` LL438-450 — 结构特征：普通交互蓝灰系/Hover 与 Pressed 连续层级/Selected 明显区别于 Hover/Active 当前工具/Focus 独立边界色不与 Selected …
  - `COMMENT` LL451-479 — UI代码八项双主题：Hover/Pressed/Selected/Active/Focus/Dragging/DropTarget.Background/DropTarget.Bo…
- possible_relation：XYUI0-0.2-D, XYUI0-0.20

### XYUI0-0.2-G · Semantic Colors｜语义色

- 位置：L480-548 · 类型：foundation-color
- 最终选择语句：均衡语义 / Balanced Semantic Colors
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL488 — 方案名称：均衡语义 / Balanced Semantic Colors
  - `COMMENT` LL490-501 — 结构特征：Success 低饱和绿/Warning 金褐/Error 克制红/Info 冷蓝/每类含 Text/Border/Background/文字承担主要辨识/背景轻量提示/…
  - `COMMENT` LL502-548 — UI代码四类 × Text/Border/Background × 双主题
- possible_relation：XYUI0-0.2-A, XYUI0-0.2-H

### XYUI0-0.2-H · Disabled / ReadOnly / Locked｜禁用、只读、锁定

- 位置：L549-598 · 类型：foundation-color
- 最终选择语句：锁定强调 / Emphasized Locked State
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL557 — 方案名称：锁定强调 / Emphasized Locked State
  - `COMMENT` LL559-566 — 结构特征：Disabled 明显弱化/ReadOnly 保持正常可读性不做坏掉灰色/Locked 独立暖金褐明显区别于前两者/锁定同时配合锁图标或状态文字/Locked 暖色不等于…
  - `COMMENT` LL567-598 — UI代码三类 × Background/Text/Border × 双主题
- possible_relation：XYUI0-0.2-G, XYUI0-0.20

### XYUI0-0.2-I · Editor Colors｜编辑器专用颜色

- 位置：L599-677 · 类型：foundation-color
- 最终选择语句：地图编辑语义 / Map Editing Semantics
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL607 — 方案名称：地图编辑语义 / Map Editing Semantics
  - `COMMENT` LL609-617 — 结构特征：Selection 蓝色体系/普通顶点暖色边界/Anchor 暖橙/Handle 低饱和暖褐/Snap Candidate 青绿/Dirty 金褐/Grid 低存在感/X…
  - `COMMENT` LL618-677 — UI代码十六项双主题：Grid.Minor/Major、Guide、Selection、MultiSelection、BoundingBox、Vertex.Fill/Border、…
- possible_relation：XYUI0-0.2-F, XYUI0-0.2-G

### XYUI0-0.3 · Typography｜字体系统（总纲）

- 位置：L678-805 · 类型：foundation-typography
- 最终选择语句：见子节 0.3-A~C
- 判断：**CLEAR**
- Evidence：
  - `COMMENT` LL678 — 总节，含 3 子节：0.3-A Font Family / B Font Weight / C Font Size
- possible_relation：XYUI0-0.3-A, XYUI0-0.3-B, XYUI0-0.3-C

### XYUI0-0.3-A · Font Family｜字体家族

- 位置：L679-722 · 类型：foundation-typography
- 最终选择语句：思源黑体 + Source Code Pro
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL696-697 — 方案名称：思源黑体 + Source Code Pro
  - `COMMENT` LL698-705 — 结构特征：中文与普通 UI 统一思源黑体/英文普通界面随 UI Sans 用思源黑体西文字形/ID 路径坐标数值技术数据用 Source Code Pro/满足可商用字体准入/随包…
  - `COMMENT` LL706-722 — UI代码：UI=Source Han Sans SC、Default=UI、Mono=Source Code Pro、Technical=Mono、Fallback.CJK=Not…
- **SOURCE_FORMATTING_DEFECT**：XYUI-SFD-002（原文缩进异常：XY.Font.Mono 以 12 空格缩进出现在 XY.Font.Default(8 空格) 之下）→ 语义裁定：XY.Font.Mono 为独立 Font Token（FontFamily=Source Code Pro），与 UI/Default/Technical 同级
- 备注：SOURCE_FORMATTING_DEFECT（XYUI-SFD-002）：XY.Font.Mono 应视为独立 Token，语义同级；层级显示歧义不影响取值。Source 原文不修改。

### XYUI0-0.3-B · Font Weight｜字重体系

- 位置：L724-765 · 类型：foundation-typography
- 最终选择语句：均衡四档 / Balanced Four-Level Weight
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL736-737 — 方案名称：均衡四档 / Balanced Four-Level Weight
  - `COMMENT` LL738-744 — 结构特征：400 正文与字段值/500 字段标签/600 Section Panel Button/700 仅页面级或最高标题/不依赖字号单独制造层级/避免大量 700 过重
  - `COMMENT` LL745-765 — UI代码：Regular=400/Medium=500/Semibold=600/Bold=700 + Body=400/Label=500/Section=600/PanelTi…
- possible_relation：XYUI0-0.3-C

### XYUI0-0.3-C · Font Size｜字号等级

- 位置：L767-805 · 类型：foundation-typography
- 最终选择语句：舒适可读 / Comfortable Readability
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL777-778 — 方案名称：舒适可读 / Comfortable Readability
  - `COMMENT` LL779-788 — 结构特征：正文 14 DIP/字段标签 15/Section 17/Panel 标题 20/页面标题 24/Caption 不低于 12/技术数据 13 等宽/优先长时间阅读舒适度…
  - `COMMENT` LL789-805 — UI代码：Caption=12/Auxiliary=13/Body=14/Label=15/Section=17/PanelTitle=20/PageTitle=24/Mono=1…
- possible_relation：XYUI0-0.4, XYUI0-0.3-B

### XYUI0-0.4 · Line Height｜行高

- 位置：L806-851 · 类型：foundation-typography
- 最终选择语句：紧凑行高 / Compact Line Height
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL817-818 — 方案名称：紧凑行高 / Compact Line Height
  - `COMMENT` LL819-826 — 结构特征：正文 14/20、保留阅读空间但不舒展、属性面板高密度、Caption/Auxiliary 更紧凑、标题随字号增加、行高不等于控件高度（Button/Input/Tree…
  - `COMMENT` LL827-851 — UI代码：Caption 12/16、Auxiliary 13/18、Body 14/20、Label 15/20、Section 17/22、PanelTitle 20/26、P…
- possible_relation：XYUI0-0.3-C

### XYUI0-0.5 · Letter Spacing｜字间距

- 位置：L852-879 · 类型：foundation-typography
- 最终选择语句：语义字距 / Semantic Letter Spacing
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL859-860 — 方案名称：语义字距 / Semantic Letter Spacing
  - `COMMENT` LL861-868 — 结构特征：中文正文默认字距/Label 轻微收紧/Title 默认/CAPS 英文轻微扩展/Mono 原生字距/只在有明确语义价值处调整/避免中文正文大幅拉宽
  - `COMMENT` LL869-879 — UI代码：Body=0、Label=-0.10、Title=0、Caps=+0.40、Mono=0

### XYUI0-0.6 · Spacing｜间距系统

- 位置：L880-918 · 类型：foundation-spacing
- 最终选择语句：4 基数紧凑 / Compact 4-Base Spacing
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL891-892 — 方案名称：4 基数紧凑 / Compact 4-Base Spacing
  - `COMMENT` LL893-901 — 结构特征：以 4 DIP 为主要基础单位/Token 数量较少/高频编辑器区域紧凑/小间距 4/8/中型控件与 Panel 12/16/Section 与大分组 24+/避免 5/…
  - `COMMENT` LL902-918 — UI代码八档：Space.1~12 = 4/8/12/16/24/32/40/48 DIP

### XYUI0-0.7 · Indentation｜缩进系统

- 位置：L919-945 · 类型：foundation-spacing
- 最终选择语句：16 DIP 紧凑缩进 / Compact 16 DIP Indentation
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL928-929 — 方案名称：16 DIP 紧凑缩进 / Compact 16 DIP Indentation
  - `COMMENT` LL930-936 — 结构特征：每深入一级固定增加 16 DIP/适合复杂层级高密度/三四级仍保留横向空间/Tree Guide 与 Disclosure 位置统一/图标文字固定小间距/避免不同 Tre…
  - `COMMENT` LL937-945 — UI代码：PerLevel=16、IconTextGap=4、TreeGuide=16 Step、Disclosure=Follow PerLevel

### XYUI0-0.8 · Sizing｜基础尺寸系统

- 位置：L946-1000 · 类型：foundation-sizing
- 最终选择语句：紧凑尺寸 / Compact Sizing
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL959-960 — 方案名称：紧凑尺寸 / Compact Sizing
  - `COMMENT` LL961-968 — 结构特征：高频编辑器控件整体紧凑/Tree Row 高密度/Toolbar 不过高/Input 与中型控件统一 32 DIP 左右/常规图标 16 DIP/Checkbox Rad…
  - `COMMENT` LL969-1000 — UI代码：Control XS/S/M/L=24/28/32/36、TreeRow=28、Toolbar=30、Input=32、Icon S/M/L=14/16/20、Check…
- 备注：与 0.19 关系：0.8 尺寸与 0.19 Compact 档一致（TreeRow 28/Toolbar 30/Input 32），0.19 额外定义 Comfortable 档（32/34/36）；原文未明示 0.8 即 Compact 档，关系留待 R2 裁决。
- possible_relation：XYUI0-0.19

### XYUI0-0.9 · Radius｜圆角系统

- 位置：L1001-1045 · 类型：foundation-radius
- 最终选择语句：语义圆角 / Semantic Radius
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1012-1013 — 方案名称：语义圆角 / Semantic Radius
  - `COMMENT` LL1014-1022 — 结构特征：圆角表达组件角色非现代感/Panel 方正/Tree Row 不做圆角卡片/Toolbar 极轻圆角/Input Button 轻微圆角/Popup 稍圆/Tag Bad…
  - `COMMENT` LL1023-1045 — UI代码：None=0、Toolbar=2、Control=4、Input=4、Button=4、Popup=6、Panel=0、Row=0、Full=999、Tag=Full、B…

### XYUI0-0.10 · Border｜边框系统

- 位置：L1046-1094 · 类型：foundation-border
- 最终选择语句：语义边框 / Semantic Border
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1056-1057 — 方案名称：语义边框 / Semantic Border
  - `COMMENT` LL1058-1066 — 结构特征：Container/Panel 不默认完整外框/Panel 之间依赖 Divider/Input Button Control 1 DIP 完整边框/关键结构 2 DIP…
  - `COMMENT` LL1067-1094 — UI代码：Width None/Default/Strong/Focus/Selected=0/1/2/2/2、Style=Solid、Container=0+UseDivider…
- possible_relation：XYUI0-0.2-D, XYUI0-0.11

### XYUI0-0.11 · Divider｜分割线规则

- 位置：L1095-1131 · 类型：foundation-border
- 最终选择语句：分层语义分割 / Hierarchical Semantic Divider
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1102-1103 — 方案名称：分层语义分割 / Hierarchical Semantic Divider
  - `COMMENT` LL1104-1113 — 结构特征：Header 全幅/Inspector Section 左右 16 DIP 内缩/Tree List Row 左右 16 DIP 内缩/垂直 Panel Split 全高…
  - `COMMENT` LL1114-1131 — UI代码：Header=0/1、Panel=0/1、Section=16/16/1、ListRow=16/16/1、VerticalSplit=0/1
- possible_relation：XYUI0-0.2-D, XYUI0-0.10

### XYUI0-0.12 · Surface｜表面层级规则

- 位置：L1132-1175 · 类型：foundation-surface
- 最终选择语句：编辑器语义 Surface / Semantic Editor Surface
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1142-1143 — 方案名称：编辑器语义 Surface / Semantic Editor Surface
  - `COMMENT` LL1144-1154 — 结构特征：App 最底层/Navigation 与普通 Panel 用 Panel/Inspector 次级用 PanelAlt/Canvas 独立/Input 用 Input 或…
  - `COMMENT` LL1155-1175 — UI代码：SurfaceRole 十项映射 + Raised.Policy=InteractiveLayerOnly
- possible_relation：XYUI0-0.2-C

### XYUI0-0.13 · Shadow｜阴影系统

- 位置：L1176-1215 · 类型：foundation-shadow
- 最终选择语句：轻量层级 / Lightweight Elevation
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1183-1184 — 方案名称：轻量层级 / Lightweight Elevation
  - `COMMENT` LL1185-1193 — 结构特征：普通 Panel/Button/Input 不使用阴影/Tooltip 轻阴影/Popup Menu 中轻阴影/Drag Preview 略强/阴影只表达 Z 轴脱离关系…
  - `COMMENT` LL1194-1215 — UI代码：None、Tooltip(0,3,10,0.12)、Popup(0,6,18,0.14)、DragPreview(0,6,18,0.14)、Panel=None、Cont…
- **SOURCE_FORMATTING_DEFECT**：XYUI-SFD-001（原文缩进异常：XY.Shadow.Control 以 12 空格缩进出现在 XY.Shadow.Panel(8 空格) 之下）→ 语义裁定：XY.Shadow.Panel 与 XY.Shadow.Control 语义上为同级 Token（均 Value=None）
- 备注：SOURCE_FORMATTING_DEFECT（XYUI-SFD-001）：层级表达异常，非设计未定；语义同级已由人工裁定。Source 原文不修改。

### XYUI0-0.14 · Opacity｜透明度系统

- 位置：L1216-1246 · 类型：foundation-opacity
- 最终选择语句：克制透明度 / Restrained Opacity
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1224-1225 — 方案名称：克制透明度 / Restrained Opacity
  - `COMMENT` LL1226-1233 — 结构特征：普通控件优先实色 Token/透明度服务弱化遮罩临时状态/Disabled 明显弱化仍可辨认/Drag Ghost 足够可见/Modal Backdrop 较轻遮罩/Hi…
  - `COMMENT` LL1234-1246 — UI代码六档：Subtle=0.72/Disabled=0.48/DragGhost=0.68/Backdrop=0.28/Hidden=0.18/Overlay=0.92

### XYUI0-0.15 · Icon｜图标系统

- 位置：L1247-1289 · 类型：foundation-icon
- 最终选择语句：语义混合 / Semantic Hybrid Icon
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1256-1257 — 方案名称：语义混合 / Semantic Hybrid Icon
  - `COMMENT` LL1258-1267 — 结构特征：默认 Outline 为主/Active Selected 允许局部填充/Active 用 Accent 色/普通轻量/Disabled 弱化色/尺寸沿用 0.8（14/…
  - `COMMENT` LL1268-1289 — UI代码：Style.Default=Outline、Style.Active=Outline+LocalFill、Stroke=1.5、LineCap/Join=Round、Si…
- possible_relation：XYUI0-0.8

### XYUI0-0.16 · Layout｜整体布局骨架

- 位置：L1290-1333 · 类型：foundation-layout
- 最终选择语句：中央画布优先 + 可切换布局
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1298-1299 — 方案名称：中央画布优先 + 可切换布局
  - `COMMENT` LL1300-1310 — 结构特征：默认 Left/Canvas/Right/Bottom 四区/中央 Canvas 优先/左导航较窄/右 Inspector 基础可用宽度/Bottom 日志区压缩/三区均…
  - `COMMENT` LL1311-1333 — UI代码：Top.Height=32、Left.DefaultWidth=180、Right.DefaultWidth=250、Bottom.DefaultHeight=100、C…
- possible_relation：XYUI0-0.27

### XYUI0-0.17 · Panel｜面板结构

- 位置：L1334-1409 · 类型：foundation-panel
- 最终选择语句：语义面板 / Semantic Panel
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1343-1344 — 方案名称：语义面板 / Semantic Panel
  - `COMMENT` LL1345-1375 — 结构特征：Panel 不采用固定结构/五种结构部件 Header Toolbar Content Section Footer/不同 Panel 按职责组合（Navigation=…
  - `COMMENT` LL1376-1380 — 示例：道路 road-ffcb76；避免标题一行 ID 再占一行；高密度布局；普通 Inspector 不默认要求显式保存
  - `COMMENT` LL1381-1409 — UI代码：Structure=Semantic、Header=Visible、Toolbar=Optional、Content=FillRemaining、Section=Titl…

### XYUI0-0.18 · Alignment｜对齐规则

- 位置：L1410-1459 · 类型：foundation-alignment
- 最终选择语句：统一左对齐 / Unified Left Alignment
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1422-1423 — 方案名称：统一左对齐 / Unified Left Alignment
  - `COMMENT` LL1424-1433 — 结构特征：Label/Value/数字/ID/Path/状态文字默认左对齐/不因数值类型自动右对齐/稳定左侧阅读基线/标题与辅助 ID 可同行
  - `COMMENT` LL1434-1438 — 示例：道路 road-ffcb76；Footer 为特殊操作区，按钮组居中不受正文左对齐影响
  - `COMMENT` LL1439-1459 — UI代码：Text/Label/Value/Number/ID/Path/Status/SectionContent=Left、TitleMeta=InlineLeft、Foote…

### XYUI0-0.19 · Density｜密度策略

- 位置：L1460-1536 · 类型：foundation-density
- 最终选择语句：Auto + 手动锁定 / Adaptive Density with Manual Lock
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1470-1471 — 方案名称：Auto + 手动锁定 / Adaptive Density with Manual Lock
  - `COMMENT` LL1472-1490 — 结构特征：两套完整密度 Compact/Comfortable/默认 Auto/Auto 按可用空间整组切换/禁同一界面混用/可手动锁定任一档/锁定后禁 Auto 切换/离散 To…
  - `COMMENT` LL1491-1502 — 两档参数：Compact=TreeRow 28/Toolbar 30/Input 32/BaseGap 4/SectionGap 8；Comfortable=32/34/36/8/…
  - `COMMENT` LL1503-1536 — UI代码：Mode=Auto(Allowed Auto|Compact|Comfortable)、Auto.Strategy=GlobalDiscreteSwitch、Auto.S…
- possible_relation：XYUI0-0.8

### XYUI0-0.20 · Interaction State｜交互状态规则

- 位置：L1537-1584 · 类型：foundation-interaction
- 最终选择语句：单状态覆盖 / Single State Override
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1549-1550 — 方案名称：单状态覆盖 / Single State Override
  - `COMMENT` LL1551-1563 — 结构特征：同一控件同一时刻只展示一个最终视觉状态/禁多层背景叠加/高优先级覆盖低优先级/Disabled 最高/Pressed 优先于 Hover/Hover 优先于普通 Sele…
  - `COMMENT` LL1564-1584 — UI代码：ComposeMode=Single、Layering=Forbidden、Disabled.Priority=Highest、Pressed.Priority=Abov…
- possible_relation：XYUI0-0.2-F, XYUI0-0.21

### XYUI0-0.21 · Focus｜焦点规则

- 位置：L1585-1632 · 类型：foundation-focus
- 最终选择语句：Focus Visible + 编辑器双层 Focus
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1596-1597 — 方案名称：Focus Visible + 编辑器双层 Focus
  - `COMMENT` LL1598-1611 — 结构特征：键盘导航明确显示 Focus/鼠标点击普通控件后默认不持续显示 Focus Ring/减少蓝色焦点框/普通控件独立 Focus Outline/Focus Outline…
  - `COMMENT` LL1612-1632 — UI代码：DisplayMode=FocusVisible、Keyboard=Visible、Mouse.PersistentRing=False、Control.OutlineW…
- possible_relation：XYUI0-0.20, XYUI0-0.2-D

### XYUI0-0.22 · Hit Target｜点击 / 抓取热区

- 位置：L1633-1679 · 类型：foundation-hit-target
- 最终选择语句：语义热区 / Semantic Hit Target
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1643-1644 — 方案名称：语义热区 / Semantic Hit Target
  - `COMMENT` LL1645-1656 — 结构特征：视觉尺寸与 Hit Target 分离/小控件允许更大隐形热区/扩大热区不改变视觉尺寸/Icon 扩展热区/Tree List 默认整行可点击/Vertex Anchor…
  - `COMMENT` LL1657-1679 — UI代码：Mode=Semantic、VisualSizeIndependent=True、Icon.Min=28、TreeRow/ListRow=FullRow、Vertex/A…

### XYUI0-0.23 · Motion｜动效规则

- 位置：L1680-1736 · 类型：foundation-motion
- 最终选择语句：语义动画 / Semantic Motion
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1691-1692 — 方案名称：语义动画 / Semantic Motion
  - `COMMENT` LL1693-1709 — 结构特征：不使用装饰性动画/不允许动画拖慢编辑操作/不同交互职责不同 Motion Token/Hover Pressed=Fast/Popup Tooltip=Normal/Se…
  - `COMMENT` LL1710-1736 — UI代码：Instant=0ms、Fast=80ms、Normal=140ms、Slow=220ms + Hover/Pressed→Fast、Popup/Tooltip→Norm…

### XYUI0-0.24 · Z-Index｜层级与叠放顺序

- 位置：L1737-1801 · 类型：foundation-zindex
- 最终选择语句：上下文堆栈 / Context Stack
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1747-1748 — 方案名称：上下文堆栈 / Context Stack
  - `COMMENT` LL1749-1769 — 结构特征：普通组件原则上不直接填写具体 ZIndex/浮层统一由专用 Host 管理/避免 999 9999 魔法数字/Host 体系：Content→Overlay Host（D…
  - `COMMENT` LL1770-1778 — 临时说明：Tooltip 位于普通应用浮层体系最高层/各 Host 内部允许局部排序/局部排序不得跨越 Host 语义边界/普通业务组件不得绕过 Host 强行提高 ZIndex/…
  - `COMMENT` LL1779-1801 — UI代码：Mode=ContextStack、DirectValue=Forbidden、ContentHost=BaseContent、OverlayHost=PopupAndO…
- **SOURCE_FORMATTING_DEFECT**：XYUI-SFD-003（原文缩进异常：Role = Tooltip 与 XY.Overlay.TooltipHost 同级(8 空格)，未缩进为其子级（对比 ModalHost 的 Role 为 12 空格））→ 语义裁定：语义关系为 XY.Overlay.TooltipHost → Role = Tooltip
- 备注：SOURCE_FORMATTING_DEFECT（XYUI-SFD-003）：Role=Tooltip 应归属 TooltipHost 子级；语义关系已由人工裁定。Source 原文不修改。
- possible_relation：XYUI0-0.12, XYUI0-0.13

### XYUI0-0.25 · Scroll｜滚动规则

- 位置：L1802-1844 · 类型：foundation-scroll
- 最终选择语句：悬停显现 / Hover-Reveal Scrollbar
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1811-1812 — 方案名称：悬停显现 / Hover-Reveal Scrollbar
  - `COMMENT` LL1813-1823 — 结构特征：Scrollbar 平时弱化/Track 默认隐藏/鼠标进入或滚动时增强/停止离开后重新弱化/Tree Inspector Log 各自独立滚动/Popup 内滚动优先自…
  - `COMMENT` LL1824-1844 — UI代码：DisplayMode=HoverReveal、Track=Hidden、Thumb=Subtle、Hover/Scrolling=Emphasized、Width=10…
- possible_relation：XYUI0-0.8

### XYUI0-0.26 · Drag & Drop｜拖放规则

- 位置：L1845-1890 · 类型：foundation-dnd
- 最终选择语句：Drag Handle + Semantic Drop Zone + Drag Threshold
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1853-1854 — 方案名称：Drag Handle + Semantic Drop Zone + Drag Threshold
  - `COMMENT` LL1855-1871 — 结构特征：Drag Handle 正式拖拽入口/普通文本区域负责点击选择编辑/Pointer Down 不立即进入 Drag/移动超过阈值才 Drag Start/默认阈值 6 D…
  - `COMMENT` LL1872-1890 — UI代码：Entry=Handle、Threshold=6、Start=AfterThreshold、DropZone=Before|Into|After、InvalidTarge…

### XYUI0-0.27 · Resize / Splitter｜面板缩放与分隔条

- 位置：L1891-1953 · 类型：foundation-resize
- 最终选择语句：语义缩放 + 关键宽度吸附 + 双击复位
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1898-1899 — 方案名称：语义缩放 + 关键宽度吸附 + 双击复位
  - `COMMENT` LL1900-1920 — 结构特征：Left/Right/Bottom 独立尺寸约束/禁共用同一 Min Default Max/Splitter 连续拖动/靠近关键尺寸自动吸附（Compact/Defau…
  - `COMMENT` LL1921-1953 — UI代码：Mode=Semantic、Continuous=True、Snap=Enabled、SnapPoints=Compact|Default|Wide、DoubleClic…
- possible_relation：XYUI0-0.16, XYUI0-0.22

### XYUI0-0.28 · Cursor｜鼠标指针语义

- 位置：L1954-2008 · 类型：foundation-cursor
- 最终选择语句：状态语义指针 / Semantic State Cursor
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL1966-1967 — 方案名称：状态语义指针 / Semantic State Cursor
  - `COMMENT` LL1968-1983 — 结构特征：Cursor 预告下一次操作/标准输入沿用系统基础 Cursor/Select=Arrow/Text=IBeam/Resize=标准/Pan=Hand/Draw=Cros…
  - `COMMENT` LL1984-2008 — UI代码：Select=Arrow、Text=IBeam、Pan=Hand、Draw=Crosshair、Move=Move、Resize.H=SizeWE、Resize.V=Si…

### XYUI0-0.29 · Tooltip｜轻量提示规则

- 位置：L2009-2054 · 类型：foundation-tooltip
- 最终选择语句：自适应内容 Tooltip / Adaptive Content Tooltip
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL2017-2018 — 方案名称：自适应内容 Tooltip / Adaptive Content Tooltip
  - `COMMENT` LL2019-2033 — 结构特征：短内容单行/较长自动换行/较多允许双层结构/MaxWidth 受限默认 280 DIP/首次 Hover 延迟约 400ms/靠边缘自动调整方向（可左右上下翻转）优先避免…
  - `COMMENT` LL2034-2054 — UI代码：ContentMode=Adaptive、ShortContent=SingleLine、LongContent=WrapOrTwoLevel、MaxWidth=280、…
- 备注：人工裁定（A1-R1-F1）：叙述『约 400 ms』属人类描述语气；正式工程值由 XY.Tooltip.ShowDelay=400 ms 冻结，不得进入 UNRESOLVED。

### XYUI0-0.30 · Text & Naming｜文本与命名规范

- 位置：L2055-2116 · 类型：foundation-naming
- 最终选择语句：编辑器双层命名 / Editor Dual-Layer Naming
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL2066-2067 — 方案名称：编辑器双层命名 / Editor Dual-Layer Naming
  - `COMMENT` LL2068-2091 — 结构特征：主界面优先中文 Display Name/技术 ID Key 次级/中文与 ID 允许同行/示例：道路主干线 road-ffcb76/禁把技术 Key 当主界面名称/上下…
  - `COMMENT` LL2092-2116 — UI代码：Primary=DisplayName、DisplayLanguage=Localized、TechnicalId=Secondary、TitleIdLayout=Inl…

### XYUI0-0.31 · Accessibility｜可访问性

- 位置：L2117-2171 · 类型：foundation-a11y
- 最终选择语句：语义可访问性 / Semantic Accessibility
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL2128-2129 — 方案名称：语义可访问性 / Semantic Accessibility
  - `COMMENT` LL2130-2148 — 结构特征：高频功能必须键盘可达/Tab 顺序稳定可预测/Focus 视觉沿用 0.21/图标按钮必须具备 Accessible Name/控件暴露 Role Name State/…
  - `COMMENT` LL2149-2171 — UI代码：KeyboardReachable=Required、TabOrder=Predictable、IconButton.Name=Required、Role/Name/St…
- possible_relation：XYUI0-0.21, XYUI0-0.22

### XYUI0-0.32 · Localization / DPI｜本地化与 DPI

- 位置：L2172-2236 · 类型：foundation-dpi
- 最终选择语句：Per-Monitor DPI 自适应
- 判断：**CLEAR**
- Evidence：
  - `SELECT` LL2180-2181 — 方案名称：Per-Monitor DPI 自适应
  - `COMMENT` LL2182-2207 — 结构特征：所有几何尺寸统一 DIP/禁业务 UI 依赖物理像素/支持 100/125/150/200%/跨显示器实时响应/不要求重启/DPI 变化不得改变语义结构/不得偷偷切换 C…
  - `COMMENT` LL2208-2236 — UI代码：Unit=DIP、PhysicalPixelHardcode=Forbidden、PerMonitor=Enabled、LiveMonitorSwitch=Enabled…
- possible_relation：XYUI0-0.19

## 五、R1 边界声明

- 本稿未产生任何正式 APPROVED 状态（五态分类属 A1-R2）。
- 本稿未产生任何 Token 输出。
- 本稿未读取 XYUI1/2、玄域现有 UI、行业规范或任何外部设计系统。
- Evidence 均可通过行号反查 `xyui/source/XYUI0/XYUI-0.md` 原文；Source SHA 与原始文件一致。
