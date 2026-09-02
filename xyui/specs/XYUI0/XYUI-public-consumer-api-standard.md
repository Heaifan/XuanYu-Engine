# XYUI Public Consumer API & Syntax Standard v1

适用范围：XYUI-0.03～0.06，以及后续 XYUI 组件的普通业务用法。

## 统一命名空间

```xml
xmlns:xy="using:XYUI.Avalonia"
xmlns:c="using:XYUI.Avalonia.Controls"
```

`xy:` 只承载 Foundation Scope；`c:` 承载具体 XYUI 控件。

## Foundation Scope

横切规则使用可继承的 Scope：

```xml
xy:XY.Size="Default"
xy:XY.Density="Compact"
```

`Size` 表达控件自身尺寸，`Density` 表达信息组织密度，二者可以组合，互不改变对方语义。

## 0.03 Spacing

普通业务优先使用封装控件（例如 `XYToolbar`、表单控件和面板）；业务不应依赖 `XY.Space.*`、`XY.Panel.*` 等原始 Token。原始 Token 属于 Component Author API。

## 0.04 Sizing

```xml
<c:XYButton
    xy:XY.Size="Default"
    Content="保存" />
```

合法档位：`Compact`、`Default`、`Comfortable`、`Touch`。

## 0.05 Density

```xml
<Border xy:XY.Density="Compact">
  <c:XYTextField Placeholder="搜索实体" />
</Border>
```

Density 不改变 Control Height、Icon Size 或 Hit Target。

## 0.06 Iconography

```xml
<c:XYIcon Icon="Search" />
<c:XYIconButton Icon="Locate" ToolTip.Tip="定位" />
<c:XYButton Icon="Save" Content="保存" Variant="Primary" />
```

业务端只表达 `Icon` 意图。Registry、Geometry、Viewport、Stroke 和 Optical Offset 由 Runtime 隐藏处理。

Toolbar 的当前稳定 C# Consumer API：

```csharp
var toolbar = new XYToolbar(
    new XYToolbarTool { ToolId = "search", Label = "搜索", Icon = XyuiVectorIcon.Search },
    new XYToolbarTool { ToolId = "locate", Label = "定位", Icon = XyuiVectorIcon.Locate });
```

## 示例属性顺序

```xml
<c:XYButton
    x:Name="SaveButton"
    xy:XY.Size="Default"
    xy:XY.Density="Compact"
    Icon="Save"
    Content="保存"
    Variant="Primary"
    Command="{Binding SaveCommand}"
    IsEnabled="{Binding CanSave}" />
```

顺序：名称 → Foundation Scope → 核心意图 → Variant/Mode → Binding/Command → State → 少量高级属性。

## Author / Runtime API

以下 API 不属于普通业务推荐写法：`XyuiVectorIcons`、`PathData`、`GetMetrics`、`Geometry`、`XyuiIconSizeMetrics`、Raw Resource Token。它们只供组件作者、Runtime 和诊断使用。

## Consumer Review 门禁

新组件宣布可用前必须确认：最短常见用法、是否隐藏内部组合、是否泄漏 Raw Token/Geometry、是否重复 Foundation Scope，并通过 `CONSUMER API PASS`。
