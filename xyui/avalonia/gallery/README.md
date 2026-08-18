# XYUI.AVALONIA-R4 · Component Catalog & Documented Gallery

本轮 Gallery 是可执行的组件目录入口，不是第二份规范。运行时由
`XYUI.Avalonia.Catalog.XyuiCatalogSource` 读取：

- `xyui/registry/foundation/identity-map.json`：XYUI-0 的稳定 `canonical_id`；
- `xyui/specs/XYUI<n>/XYUI-<n>.mapping.json`：XYUI-1～8 的 `component_id`、名称和真实 API/token 引用；
- `xyui/specs/XYUI<n>/XYUI-<n>.canonical.md`：用途、场景、变体、状态和视觉规则原文。

目录每行显示 Module、Source Item ID、Canonical ID、显示名称、Avalonia 类型、Spec 路径和覆盖状态。状态顺序固定为
`DESIGNED / CANONICAL / AVALONIA / GALLERY / DOCUMENTED / READY`；没有实现类型时不填写推测名称。

当前真实 Avalonia 组件入口来自 XYUI-2：`XYButton`、`XYIconButton`、`XYToggleButton`、`XYCheckbox`、`XYTextField`。
它们的 Gallery 示例位于 `Views/ComponentSamplesView.axaml`，只调用已存在的 Avalonia API；颜色、边框和 Typography 继续从既有 XYUI 资源消费。

XYUI-9 的 source、spec、pack 和 Avalonia 路径当前均不存在，目录会保留一行
`SOURCE NOT PRESENT IN CURRENT REPOSITORY`，不将其标为 READY。
