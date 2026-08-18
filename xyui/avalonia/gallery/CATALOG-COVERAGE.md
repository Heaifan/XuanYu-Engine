# XYUI.AVALONIA-R5 · XYUI-1 覆盖矩阵

覆盖矩阵由 Gallery 运行时从 Registry/Mapping/Spec 读取，避免手工复制组件清单。矩阵字段为：

`DESIGNED | CANONICAL | AVALONIA | GALLERY | DOCUMENTED | READY`

XYUI-1 本轮冻结为 canonical mapping 中的 24 个组件，全部登记到 Avalonia/Catalog/Gallery/Usage/Tests：

| Source Item ID | Canonical ID | Avalonia type | Gallery |
|---|---|---|---|
| XYUI-1-01～24 | XYUI-1-01～24 | `XYUI.Avalonia.Controls.XY*` 24/24 | `XYUI-1 · 文本与信息` |

| Canonical | 24/24 | mapping 24/24 | — |
| Avalonia | 24/24 | stable public type + identity | — |
| Catalog | 24/24 | registry/mapping/spec driven | — |
| Gallery | 24/24 | real control Preview | — |
| Usage | 24/24 | real type/property examples | — |
| Tests | 24/24 | 56/56 PASS | — |

Gap：`XYUI1-GAP-001` Icon glyph registry；`XYUI1-GAP-002` Avalonia TextBlock 没有 MiddleEllipsis 原生能力，`XYTruncatedText.Mode=Middle` 保留 API 但当前运行时降级为 canonical EndEllipsis，均不伪造为已解决。

变体、状态、用途和 API 详情必须回到对应 canonical spec 查看；Gallery 不重复定义未经裁决的数值或语义。
