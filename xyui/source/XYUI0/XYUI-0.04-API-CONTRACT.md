# XYUI-0.04 API CONTRACT

## Sizing

`XyuiSizeRole`：`Compact`、`Default`、`Comfortable`、`Touch`。

`XyuiSizingScope.SizeRoleProperty` 是可继承 Attached Property。页面或容器设置一次后，已接入的 XYUI 控件消费对应尺寸。

```xml
<Panel xy:XyuiSizingScope.SizeRole="Compact">
    <XYButton Content="应用" />
</Panel>
```

| SizeRole | ControlHeight | IconSize | MinimumHitTarget |
| --- | ---: | ---: | ---: |
| Compact | 28 DIP | 14 DIP | 28 DIP |
| Default | 32 DIP | 16 DIP | 32 DIP |
| Comfortable | 36 DIP | 20 DIP | 36 DIP |
| Touch | 44 DIP | 24 DIP | 44 DIP |

Width remains content-driven for ordinary controls. IconButton separates visual control size from its minimum hit target.
