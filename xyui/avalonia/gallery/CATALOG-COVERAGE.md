# XYUI-1～9 覆盖矩阵说明

覆盖矩阵由 Gallery 运行时从 Registry/Mapping/Spec 读取，避免手工复制组件清单。矩阵字段为：

`DESIGNED | CANONICAL | AVALONIA | GALLERY | DOCUMENTED | READY`

当前已进入 Avalonia/Gallery 的真实 XYUI-2 条目：

| Source Item ID | Canonical ID | Avalonia type | Canonical 入口 |
|---|---|---|---|
| XYUI-2-01 | XYUI-2-01 | `XYUI.Avalonia.Controls.XYButton` | `XY.Button` |
| XYUI-2-02 | XYUI-2-02 | `XYUI.Avalonia.Controls.XYIconButton` | `XY.IconButton` |
| XYUI-2-03 | XYUI-2-03 | `XYUI.Avalonia.Controls.XYToggleButton` | `XY.ToggleButton` |
| XYUI-2-06 | XYUI-2-06 | `XYUI.Avalonia.Controls.XYCheckbox` | `XY.Checkbox` |
| XYUI-2-09 | XYUI-2-09 | `XYUI.Avalonia.Controls.XYTextField` | `XY.TextField` |

其余 XYUI-1～8 条目已纳入 Catalog 的设计、canonical 和文档层；没有对应的当前 Avalonia 类型就保持非 READY。
XYUI-9 在当前仓库没有 source/spec/pack，矩阵唯一行明确为 `SOURCE NOT PRESENT IN CURRENT REPOSITORY`。

变体、状态、用途和 API 详情必须回到对应 canonical spec 查看；Gallery 不重复定义未经裁决的数值或语义。
