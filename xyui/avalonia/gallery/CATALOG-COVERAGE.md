# XYUI.AVALONIA-R5 · XYUI-1 覆盖矩阵

覆盖矩阵由 Gallery 运行时从 Registry/Mapping/Spec 读取，避免手工复制组件清单。矩阵字段为：

`DESIGNED | CANONICAL | AVALONIA | GALLERY | DOCUMENTED | READY`

XYUI-1 本轮冻结为 canonical mapping 中的 24 个组件，全部登记到 Avalonia/Catalog/Gallery/Documentation/Usage/Tests：

| Source Item ID | Canonical ID | Avalonia type | Gallery |
|---|---|---|---|
| XYUI-1-01～24 | XYUI-1-01～24 | `XYUI.Avalonia.Controls.XY*` 24/24 | `XYUI-1 · 文本与信息` |

| Canonical | 24/24 | mapping 24/24 | — |
| Avalonia | 24/24 | stable public type + identity | — |
| Catalog | 24/24 | registry/mapping/spec driven | — |
| Gallery | 24/24 | real control Preview | — |
| Documentation | 24/24 | 中文优先文档页 + 左侧导航 + 模块索引 | — |
| Usage | 24/24 | real type/property examples | — |
| Tests | 24/24 | 64/64 PASS | — |

Gap：`XYUI1-GAP-002` Avalonia TextBlock 没有 MiddleEllipsis 原生能力，`XYTruncatedText.Mode=Middle` 保留 API 但当前运行时降级为 canonical EndEllipsis；Vector Icon Registry 已关闭 GAP-001。

R5 fidelity：17 项用户验收问题已由真实组件实现、Gallery 文档/API/Usage 与回归测试共同覆盖；自动证据不替代用户真机验收。

变体、状态、用途和 API 详情必须回到对应 canonical spec 查看；文档页只展示已登记且可由当前 Avalonia API 实例化的内容，不重复定义未经裁决的数值或语义。
