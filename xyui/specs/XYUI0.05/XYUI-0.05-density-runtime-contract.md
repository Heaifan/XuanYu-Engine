# XYUI-0.05 API CONTRACT · Density

## 核心定义

`Density` 表达同一视口中的信息组织密度；`Spacing` 表达元素之间的空间关系；`Sizing` 表达控件自身尺寸。三者不得互相代替。

## Runtime API

```csharp
XY.Density
```

该 AttachedProperty 继承于 Avalonia `Control` 树，默认值为 `Default`。子控件可局部覆盖；覆盖只影响该子树，不影响兄弟节点。

```csharp
XY.SetDensity(panel, XYDensity.Compact);
var density = XY.GetDensity(control);
```

合法档位只有 `Compact`、`Default`、`Comfortable`。本轮不引入 `Touch`。旧 `XyuiDensityScope` 仅作为内部兼容入口，新的外部消费统一使用 `XY.Density`。

## 语义表

| 档位 | 行组织 | 区块组织 | 面板内边距 |
| --- | --- | --- | --- |
| `Compact` | `XY.Panel.Field.RowGap` | `XY.Panel.SectionGap` | `XY.Panel.Padding` |
| `Default` | `XY.Panel.Field.RowGap` | `XY.Panel.SectionGap` | `XY.Panel.Padding` |
| `Comfortable` | `XY.Space.2` | `XY.Space.3` | `XY.Space.3` |

这些值是对 0.03 Spacing 语义的组合，不新增平行的 Density Spacing Token。Compact 与 Default 当前共享既有基线，区别留给后续真实消费场景，避免用伪差异制造视觉或尺寸副作用。

## 边界

Density 不修改 `Control.Height`、`Width`、`FontSize`、图标尺寸或命中区，不隐藏关键状态，也不携带 `XY.Size`。因此 `XY.Size = Default` 与 `XY.Density = Compact` 可以同时成立。
