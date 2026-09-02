# XYUI-0.07 CANONICAL FACTS

状态：FACTS READY / RADIUS RESOLVED。本文只记录当前 Runtime 真值；07 Gallery 的展示稿不得反向定义规则。

## RADIUS

- `XYButton`：4 DIP；来源 `XyuiSpatialTokens.RadiusButton`，由 `XyuiControlStyles.Chrome` 消费。
- `XYIconButton`：4 DIP；来源同上，由 `AddGhostIconButton` 消费。
- `XYTextField`：4 DIP；基础输入样式消费 `XY.Radius.Input`。
- `XYToolbar`：Runtime 没有设置 CornerRadius；`XYToolbar` 是 `Border`，默认有效值为 0 DIP。工具组另有显式 4 DIP 样式，不等于 Toolbar 圆角。
- Panel：`xyui-surface-panel` 消费 `XY.Radius.Panel`，值为 0 DIP；`xyui-surface-raised` 消费 `XY.Radius.Control`，值为 4 DIP。
- 其他已注册角色：Toolbar token 2、Popup token 6、Full token 999、Row token 0。
- 结论：不存在当前 Runtime 的统一 `4 / 6 / 8 DIP` 阶梯；8 DIP 不是本表中的 Radius token。

### XYTextField Radius Drift

原 `3 DIP` 来自 `XyuiControlStyles.Input` 的历史硬编码 `new CornerRadius(3)`；`4 DIP` 来自 `XyuiSpatialTokens.RadiusInput` 及其 `XY.Radius.Input` Resource。现已移除输入族顶层样式中的第二真值，统一消费该 Resource；复合控件内部局部 0/局部圆角不属于顶层 Input Radius。

## BORDER

正式的语义边框名称确实存在：

| Name | Thickness | Brush source | Role/state |
| --- | ---: | --- | --- |
| Subtle | 1 | `XY.Brush.Border.Color.Subtle` | 弱边界、作者语义样式 |
| Default | 1 | `XY.Brush.Border.Color.Default` | 常规控件/表面边界 |
| Strong | 2 | `XY.Brush.Border.Color.Strong` | 强结构边界 |
| Focus | 2 | `XY.Brush.Border.Color.Focus` | Focus 状态 |
| Selected | 2 | `XY.Brush.Border.Color.Selected` | Selected 状态 |

`Hover` 主要改变状态背景/前景，不是独立的 Canonical Border 等级。Divider 使用独立的
`XY.Brush.Divider.Default`，基础高度为 1 DIP。

## SEPARATOR

- Public API 只有 `XYSeparator.Variant`：`Default`、`Header`、`Panel`、`Section`、`ListRow`、`VerticalSplit`。
- `Header`/`Panel`：横向 1 DIP、左右 0；`Section`：横向 1 DIP、左右 8；`ListRow`：横向 1 DIP、左右 16；`VerticalSplit`：纵向 1 DIP。
- `Orientation`、`Inset`、`FullBleed`、`Group`、`Toolbar`、`List` 都不是 `XYSeparator` 的独立公共属性或枚举项；`ListRow` 是语义变体，不是 `List` 属性。
- 内部消费者包括 `XYMenuBar`、`XYContextMenu`、`XYToolGroup`、`XYNavigationMenu` 等显式布局组件。`XYToolbar` 本身不自动插入 Separator。

## CONSUMER API

Button：

```xml
<c:XYButton Content="确认" />
<c:XYButton Content="保存" Variant="Primary" Icon="Save" />
```

TextField：

```xml
<c:XYTextField Text="Player_Main" Placeholder="名称" />
```

Toolbar 的真实稳定写法是 C# 构造参数，不是 XAML `ItemsSource`：

```csharp
var toolbar = new XYToolbar(
    new XYToolbarTool
    {
        ToolId = "search",
        Label = "搜索",
        Icon = XyuiVectorIcon.Search
    });
```

## AUTO-CONSUMPTION

- Button 和 IconButton 自动消费自己的 Radius、BorderBrush、BorderThickness；普通 Consumer 不需要手写这些属性。
- TextField 自动消费输入背景、Default/Focus/Error/Disabled 边框及内部模板；普通 Consumer 不需要手写 CornerRadius 或 BorderThickness。
- Toolbar 自动管理工具排列和 compact/comfortable 间距，但不自动管理 Separator；`XYToolGroup` 的 VerticalSplit 是显式内部布局。
- `XYSeparator` 是 Author/layout-level 显式组件，不是 Toolbar 的隐式 API。

## AUTHOR API

组件作者可使用 `XyuiSpatialTokens`、`XyuiShapeStyles`、真实 `XY.*` DynamicResource、`XYSeparator.Variant` 以及显式 `XYToolbarTool` 组合。业务 Consumer 应优先使用 XYUI 控件和语义属性。

## INVALID GALLERY ASSUMPTIONS

- `4 / 6 / 8 DIP`：假设错误；当前 token 没有 8 DIP Radius。
- `Subtle / Default / Strong`：名称真实，但不能展示成三档同为 1 DIP；Strong 是 2 DIP，Focus/Selected 另属状态语义。
- `Full / Inset / Local`：不是 `XYSeparator` 公共 API；必须改用真实 Variant。
- `XYToolbar ItemsSource`：不存在；不得作为 Consumer 示例。
