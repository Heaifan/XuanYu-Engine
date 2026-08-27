# XYUI.AVALONIA-R5 · XYUI-1 Component Catalog & Documented Gallery

本轮 Gallery 是可执行的组件目录入口，不是第二份规范。运行时由
`XYUI.Avalonia.Catalog.XyuiCatalogSource` 读取：

- `xyui/registry/foundation/identity-map.json`：XYUI-0 的稳定 `canonical_id`；
- `xyui/specs/XYUI<n>/XYUI-<n>.mapping.json`：XYUI-1～8 的 `component_id`、名称和真实 API/token 引用；
- `xyui/specs/XYUI<n>/XYUI-<n>.canonical.md`：用途、场景、变体、状态和视觉规则原文。

目录每行显示 Module、Source Item ID、Canonical ID、显示名称、Avalonia 类型、Spec 路径和覆盖状态。状态顺序固定为
`DESIGNED / CANONICAL / AVALONIA / GALLERY / DOCUMENTED / READY`；没有实现类型时不填写推测名称。

XYUI-1 的 24 个文本与信息组件入口位于 `Controls/XYUI1/XYUI1-XX-ComponentName/`，由 `XYUI1GalleryCatalog` 创建真实 Preview；XYUI-2 组件入口位于 `Controls/XYUI2/XYUI2-XX-ComponentName/`。每项同时展示 canonical ID、中文名称、Variants、States、Usage 和 Dependencies。
组件颜色、字体、字号和行高继续消费既有 XYUI Foundation 资源，不复制 Typography Token。

XYUI-9 的 source、spec、pack 和 Avalonia 路径当前均不存在，目录会保留一行
`SOURCE NOT PRESENT IN CURRENT REPOSITORY`，不将其标为 READY。
