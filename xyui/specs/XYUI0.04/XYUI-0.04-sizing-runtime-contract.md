# XYUI-0.04 API CONTRACT · Sizing

## Runtime API

```csharp
XY.Size
XY.SetSize(target, XYSize.Default);
var size = XY.GetSize(target);
```

`XY.Size` 是可继承的 AttachedProperty，默认值为 `Default`。子级可覆盖，覆盖只影响当前子树；兄弟节点保持父级值。新的 XAML 与业务代码统一使用 `XY.Size`、`XYSize`，旧的完整命名只允许作为内部兼容实现。

## 尺寸语义

| 档位 | 控件高度 | 图标尺寸 | 触控命中基线 |
| --- | ---: | ---: | ---: |
| `Compact` | 28 DIP | 14 DIP | 不由此档位改变 |
| `Default` | 32 DIP | 16 DIP | 不由此档位改变 |
| `Comfortable` | 36 DIP | 20 DIP | 不由此档位改变 |
| `Touch` | 44 DIP | 24 DIP | 44 DIP |

Sizing 只表达控件自身尺寸语义。它不修改 `XY.Density`、Spacing、字体或内容驱动宽度。`XY.Size = Default` 与 `XY.Density = Compact` 可以同时存在。
