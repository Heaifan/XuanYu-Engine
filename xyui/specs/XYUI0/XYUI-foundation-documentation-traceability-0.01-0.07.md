# XYUI Foundation Documentation Traceability 0.01–0.07

状态：TRACEABILITY FACTS READY。本文是 Gallery 说明书的唯一事实来源；页面可以改变布局，不能改变本文的 Layer、API、Syntax 或 Result。

统一链路：Foundation Name → Consumer / Author / Reference / Internal → 真实入口 → 可复制语法 → Runtime 结果。

## Layer 约定

- **CONSUMER**：业务可以直接使用的公共控件、Variant 或 attached property。
- **AUTHOR**：组件作者用于组合样式、布局和语义资源；业务通常不填写。
- **REFERENCE**：供查阅的 Token、原始值、字体/颜色/几何诊断资料；不是直接 Consumer 输入。
- **INTERNAL**：控件模板或复合控件内部实现；不构成公共入口。
- **AUTO**：Consumer 触发后由控件/容器自动消费，Consumer 不再重复填写。

## XYUI-0.01 · Palette / Color

颜色 Token 的 Runtime 来源是 `XyuiColorTokens` 和 `XyuiTheme` Brush 资源。`XY.Color.*` 是 Core Palette Reference，不是控件属性名。

| Foundation Name | Layer | Consumer API / Owner | Copyable Syntax | Actual Result | Status |
| --- | --- | --- | --- | --- | --- |
| `XY.Color.App` | REFERENCE | Theme / Surface owner | `DynamicResource XY.Brush.Surface.App`（Author） | App 背景色 | PASS |
| `XY.Color.Panel` | REFERENCE | Theme / Surface owner | `DynamicResource XY.Brush.Surface.Panel`（Author） | Panel 背景色 | PASS |
| `XY.Color.Raised` | REFERENCE | Theme / Surface owner | `DynamicResource XY.Brush.Surface.Raised`（Author） | Raised 背景色 | PASS |
| `XY.Color.Border` | REFERENCE | Palette only | 不直接给 Consumer | Core border reference | PASS |
| `XY.Color.Accent` | REFERENCE | Variant / semantic brush | `<c:XYButton Variant="Primary" />` | Primary 语义色 | PASS |
| `XY.Color.Hover` | REFERENCE | Control state styles | `XYUI Control` + pointer over | Hover 状态色自动出现 | PASS |
| `XY.Color.Selected` | REFERENCE | Selected state / Variant | `IsSelected="True"`（具体控件） | Selected surface/state | PASS |
| `XY.Color.Success` | REFERENCE | Semantic component / `XYStatusDot.State` | `<c:XYStatusDot State="Success" />`（如适用） | Success semantic channel | PASS |
| `XY.Color.Warning` | REFERENCE | Semantic component | semantic control API（如适用） | Warning semantic channel | PASS |
| `XY.Color.Error` | REFERENCE | `XYButton Variant="Danger"` / validation | `<c:XYButton Variant="Danger" />` | Danger/error semantic channel | PASS |
| `XY.Text.Primary` | REFERENCE | Text controls / theme | `<c:XYText Text="说明" />` | Primary text brush | PASS |
| `XY.Text.Secondary` | REFERENCE | Text controls / theme | `<c:XYCaption Text="辅助" />` | Secondary text brush | PASS |
| `XY.Text.Tertiary` | REFERENCE | Theme / specialized component | Consumer does not set raw token | Tertiary text brush | PASS |
| `XY.Text.Placeholder` | REFERENCE | Field template | `<c:XYTextField Placeholder="名称" />` | Placeholder brush automatic | PASS |
| `XY.Text.Disabled` | REFERENCE | Disabled state | `IsEnabled="False"` | Disabled text automatic | PASS |
| `XY.Text.Link` | REFERENCE | `XYLink` | `<c:XYLink Content="帮助" />` | Link text brush | PASS |
| `XY.Icon.Mark` | REFERENCE / INTERNAL | Mark-capable components | Consumer uses component; mark is automatic | Mark geometry/brush | PASS |

Author example, only when authoring a custom surface:

```xml
<Border Background="{DynamicResource XY.Brush.Surface.Panel}" />
```

Consumer example:

```xml
<c:XYButton Content="保存" Variant="Primary" />
```

## XYUI-0.02 · Typography

| Foundation Name | Layer | Consumer API / Owner | Copyable Syntax | Actual Result | Status |
| --- | --- | --- | --- | --- | --- |
| `Caption` | CONSUMER | `XYCaption` | `<c:XYCaption Text="辅助说明" />` | Caption size/line-height/secondary brush | PASS |
| `Auxiliary` | REFERENCE | Typography token | Consumer does not directly select this role | 13/18 auxiliary scale | PASS |
| `Body` | CONSUMER | `XYText` | `<c:XYText Text="正文" />` | Body size/line-height/primary brush | PASS |
| `Label` | CONSUMER | `XYLabel` | `<c:XYLabel Text="名称" />` | Label size/weight | PASS |
| `Section` | AUTHOR / REFERENCE | `XYSectionTitle` or author style | `<c:XYSectionTitle Text="区块" />` where appropriate | Section title composition | PASS |
| `PanelTitle` | CONSUMER | `XYHeading` | `<c:XYHeading Text="面板" Variant="PanelTitle" />` | Panel title typography | PASS |
| `PageTitle` | CONSUMER | `XYHeading` | `<c:XYHeading Text="地图编辑" Variant="PageTitle" />` | Page title typography | PASS |
| `Mono` | AUTHOR / REFERENCE | Mono data styles/components | Consumer does not set raw font token | Mono font/size for technical data | PASS |
| `XY.Font.UI` | REFERENCE / AUTO | Typography styles | Consumer uses text control | UI font automatic | PASS |
| `XY.Font.Mono` | REFERENCE / AUTO | Mono component/style | Use mono-capable component | Mono font automatic | PASS |
| Font weight | REFERENCE / AUTO | Component style | Use component/variant | Role-specific weight | PASS |

Compatibility classes such as `Classes="xyui-text-body"` or `Classes="xyui-heading-panel"` are Author/compatibility syntax, not the preferred Minimal Consumer API.

## XYUI-0.03 · Spacing

| Foundation Name | Layer | Consumer API / Owner | Copyable Syntax | Actual Result | Status |
| --- | --- | --- | --- | --- | --- |
| `Padding` | AUTHOR / AUTO | XYUI surface/container | `<Border Classes="xyui-surface-panel" />` | Container padding owned by style | PASS |
| `Gap` | AUTO | Composite control | `new XYToolbar(tool1, tool2)` | Toolbar spacing 2 compact / 4 comfortable | PASS |
| `Margin` | AUTHOR | External layout relation | `<Control Margin="..." />` when composition requires | Relationship to siblings/viewport | PASS |
| `XY.Space.*` | AUTHOR / REFERENCE | `XyuiSpatialTokens` / resources | `DynamicResource XY.Space.2` | 4-DIP base token family | PASS |
| `XY.Panel.*` | AUTHOR / AUTO | Panel/layout styles | `DynamicResource XY.Panel.Padding` | Panel padding/section/row layout | PASS |
| Compact spacing | AUTO | `XYToolbar.IsCompact` / density helpers | `IsCompact="True"` or default | Toolbar item gap 2 DIP | PASS |
| Default spacing | AUTO | Component defaults | Use component default | Current default baseline | PASS |
| Comfortable spacing | AUTO | `XYToolbar.IsCompact="False"` / density | `<c:XYToolbar IsCompact="False" />` | Toolbar item gap 4 DIP | PASS |

There is **no `xy:XY.Spacing` public Consumer API**. Business consumers should prefer composite XYUI controls; authors may consume `XY.Space.*` and `XY.Panel.*`. The Toolbar C# example demonstrates automatic Gap consumption, not a Spacing property:

```csharp
var toolbar = new XYToolbar(
    new XYToolbarTool { ToolId = "search", Label = "搜索", Icon = XyuiVectorIcon.Search });
```

## XYUI-0.04 · Sizing

| Foundation Name | Layer | Consumer API / Owner | Copyable Syntax | Actual Result | Status |
| --- | --- | --- | --- | --- | --- |
| `Compact` | CONSUMER | `XY.Size` | `<StackPanel xy:XY.Size="Compact">` | Control 28 DIP, icon 14 DIP | PASS |
| `Default` | CONSUMER | `XY.Size` | `<StackPanel xy:XY.Size="Default">` | Control 32 DIP, icon 16 DIP | PASS |
| `Comfortable` | CONSUMER | `XY.Size` | `<StackPanel xy:XY.Size="Comfortable">` | Control 36 DIP, icon 20 DIP | PASS |
| `Touch` | CONSUMER | `XY.Size` | `<StackPanel xy:XY.Size="Touch">` | Control 44 DIP, icon 24 DIP | PASS |
| Width | AUTO / DERIVED | `XyuiSizingMetrics` + control layout | Consumer does not set sizing Width for the role | Derived by control/context | PASS |
| Hit target | AUTO / DERIVED | Control template and size | Use `XY.Size` | Derived interaction area | PASS |
| Icon size | AUTO / DERIVED | `XyuiSizingMetrics` → icon metrics | Use `XY.Size` | 14/16/20/24 DIP | PASS |

## XYUI-0.05 · Density

| Foundation Name | Layer | Consumer API / Owner | Copyable Syntax | Actual Result | Status |
| --- | --- | --- | --- | --- | --- |
| Density `Compact` | CONSUMER | `XY.Density` | `<StackPanel xy:XY.Density="Compact">` | Row gap 4, section gap 8, panel padding 8 | PASS |
| Density `Default` | CONSUMER | `XY.Density` | `<StackPanel xy:XY.Density="Default">` | Current baseline; shares compact metric baseline | PASS |
| Density `Comfortable` | CONSUMER | `XY.Density` | `<StackPanel xy:XY.Density="Comfortable">` | Row gap 8, section gap 12, panel padding 12 | PASS |
| 一级信息 | AUTO / BEHAVIOR | Density-aware composition | No `PrimaryInfo` property | Primary information retains priority | PASS |
| 二级信息 | AUTO / BEHAVIOR | Density-aware composition | No `SecondaryInfo` property | Secondary information is compressed/relocated by composition | PASS |
| 元数据 | AUTO / BEHAVIOR | Density-aware composition | No metadata property | Metadata uses supporting spacing/typography | PASS |
| 辅助操作 | AUTO / BEHAVIOR | Density-aware composition | No auxiliary-action density API | Supporting actions follow container density | PASS |
| 文本换行 | AUTO / BEHAVIOR | Text/layout engine | No density-specific wrap property | Wrap follows available space and layout | PASS |

Density is space organization; Size is the control's own size. Behavioral labels above are results, not additional API names.

## XYUI-0.06 · Iconography

| Foundation Name | Layer | Consumer API / Owner | Copyable Syntax | Actual Result | Status |
| --- | --- | --- | --- | --- | --- |
| `Compact` | CONSUMER | `XY.Size` → `XyuiIconSize.Compact` | `<StackPanel xy:XY.Size="Compact"><c:XYIcon Icon="Search" /></StackPanel>` | 14 DIP / stroke 1.25 | PASS |
| `Default` | CONSUMER | `XY.Size` → Default | `Size="Default"` or inherited Size | 16 DIP / stroke 1.5 | PASS |
| `Comfortable` | CONSUMER | `XY.Size` → Comfortable | `Size="Comfortable"` or inherited Size | 20 DIP / stroke 1.75 | PASS |
| `Touch` | CONSUMER | `XY.Size` → Touch | `Size="Touch"` or inherited Size | 24 DIP / stroke 2 | PASS |
| `Search` | CONSUMER | `XYIcon.Icon` | `<c:XYIcon Icon="Search" />` | Registry Search geometry | PASS |
| `Locate` | CONSUMER | `XYIcon.Icon` / IconButton | `<c:XYIconButton Icon="Locate" />` | Registry Locate geometry | PASS |
| `ChevronRight` | CONSUMER | `XYIcon.Icon` | `<c:XYIcon Icon="ChevronRight" />` | Registry ChevronRight geometry | PASS |
| `Eye` | CONSUMER | `XYIcon.Icon` | `<c:XYIcon Icon="Eye" />` | Registry Eye geometry | PASS |
| `Code` | CONSUMER | `XYIcon.Icon` | `<c:XYIcon Icon="Code" />` | Registry Code geometry | PASS |
| `MoreHorizontal` | CONSUMER | `XYIcon.Icon` | `<c:XYIcon Icon="MoreHorizontal" />` | Registry MoreHorizontal geometry | PASS |
| Viewport | AUTHOR / DIAGNOSTIC | Vector registry | Consumer does not set | 24 DIP logical viewport | PASS |
| Geometry Bounds | AUTHOR / DIAGNOSTIC | `XyuiVectorIconMetrics` | Consumer does not set | Derived geometry bounds | PASS |
| Optical Offset | AUTHOR / DIAGNOSTIC | `XyuiVectorIconMetrics` | Consumer does not set | Derived optical alignment | PASS |
| Stroke | AUTO / AUTHOR | Icon size metrics / style | Consumer selects size, not stroke | Size-derived 1/1.25/1.5/1.75/2 | PASS |

`XYIconButton.Icon` and `XYButton.Icon` are Consumer conveniences; the controls compose the icon and preserve the semantic component styling.

## XYUI-0.07 · Geometry

### Radius

| Foundation Name | Layer | Consumer API / Owner | Copyable Syntax | Actual Result | Status |
| --- | --- | --- | --- | --- | --- |
| Panel | AUTHOR / ROLE | `xyui-surface-panel` | `<Border Classes="xyui-surface-panel" />` | Radius 0 DIP | PASS |
| XYToolbar | CONSUMER / AUTO | `XYToolbar` | `new XYToolbar(tool)` | Effective Toolbar radius 0 DIP; no automatic Separator | PASS |
| XYButton | CONSUMER / AUTO | Button style | `<c:XYButton Content="确认" />` | Radius 4 DIP, border auto | PASS |
| XYIconButton | CONSUMER / AUTO | IconButton style | `<c:XYIconButton Icon="Search" />` | Radius 4 DIP, border auto | PASS |
| XYTextField | CONSUMER / AUTO | Input family style | `<c:XYTextField Text="Player_Main" />` | `XY.Radius.Input` = 4 DIP | PASS |
| Raised Surface | AUTHOR / ROLE | `xyui-surface-raised` | `<Border Classes="xyui-surface-raised" />` | Radius 4 DIP | PASS |

Radius is component-role based. There is no universal 4/6/8 ladder; Toolbar and Panel remain square.

### Border

| Foundation Name | Layer | Consumer API / Owner | Copyable Syntax | Actual Result | Status |
| --- | --- | --- | --- | --- | --- |
| Subtle | AUTHOR / INTERNAL | `xyui-border-subtle`, semantic border brush | `<Border Classes="xyui-border-subtle" />` | 1 DIP + `XY.Brush.Border.Color.Subtle` | PASS |
| Default | AUTO / AUTHOR | Standard control/surface styles | Use `XYUI Control`; author may use `xyui-border-default` | 1 DIP + `XY.Brush.Border.Color.Default` | PASS |
| Strong | AUTHOR / INTERNAL | `xyui-border-strong` | `<Border Classes="xyui-border-strong" />` | 2 DIP + `XY.Brush.Border.Color.Strong` | PASS |
| Focus | AUTO / INTERNAL | Control focus state | `Focusable` control / keyboard focus | 2 DIP focus brush where defined | PASS |
| Selected | AUTO / INTERNAL | Selected control state | `IsSelected="True"` where supported | 2 DIP selected brush where defined | PASS |

Subtle and Default both use 1 DIP; semantic brush and role distinguish them. Border names are not a generic invitation for a business Consumer to set arbitrary `BorderThickness`.

### Separator

| Foundation Name | Layer | Consumer API / Owner | Copyable Syntax | Actual Result | Status |
| --- | --- | --- | --- | --- | --- |
| Default | CONSUMER | `XYSeparator.Variant` | `<c:XYSeparator Variant="Default" />` | Horizontal 1 DIP, base margins | PASS |
| Header | CONSUMER | `XYSeparator.Variant` | `<c:XYSeparator Variant="Header" />` | Horizontal 1 DIP, 0/0 margins | PASS |
| Panel | CONSUMER | `XYSeparator.Variant` | `<c:XYSeparator Variant="Panel" />` | Horizontal 1 DIP, 0/0 margins | PASS |
| Section | CONSUMER | `XYSeparator.Variant` | `<c:XYSeparator Variant="Section" />` | Horizontal 1 DIP, 8/8 margins | PASS |
| ListRow | CONSUMER | `XYSeparator.Variant` | `<c:XYSeparator Variant="ListRow" />` | Horizontal 1 DIP, 16/16 margins | PASS |
| VerticalSplit | CONSUMER / INTERNAL | `XYSeparator.Variant` | `<c:XYSeparator Variant="VerticalSplit" />` | Vertical 1 DIP | PASS |

There are no public `Orientation`, `Inset`, `FullBleed`, `Group`, `Toolbar` or `List` properties on `XYSeparator`. `XYToolGroup` may explicitly create `VerticalSplit`; `XYToolbar` does not automatically insert one.

## Cross-page author/reference rules

| Name family | Direct Consumer? | Correct instruction |
| --- | --- | --- |
| Raw Color / Text / Font tokens | No | Reference; consume through Theme, semantic Brush or XYUI control |
| `XY.Space.*` / `XY.Panel.*` | No generic spacing property | Author token/resource; prefer composite controls for business layout |
| Size / Density | Yes | Use `xy:XY.Size` and `xy:XY.Density` |
| Radius / Border | Usually no direct numeric input | Let the control or semantic author style consume Canonical geometry |
| Separator Variant | Yes | Explicitly use `XYSeparator Variant="..."` when a divider is needed |
| Viewport / Bounds / Optical Offset / Stroke | No | Derived/author/diagnostic facts |

## Gallery acceptance checklist

- Every visible Foundation name is followed by a Consumer syntax, an Author syntax, an Auto result, or an explicit Reference/Author-only note.
- No `xy:XY.Spacing`, `XYToolbar ItemsSource`, `Full/Inset/Local` Separator API, or universal Radius ladder is shown.
- No page freezes `XYTextField` to a pre-unification value; current Runtime is `XY.Radius.Input = 4 DIP`.
- Decorative Gallery containers must not be presented as Canonical Radius examples.
- A visual demo must use the same public control/API named by its code sample.

## Current traceability verdict

`TRACEABILITY FACTS READY`

`RUNTIME CHANGES = 0`

`PUBLIC API CHANGES = 0`

`DO NOT COMMIT`

`DO NOT PUSH`
