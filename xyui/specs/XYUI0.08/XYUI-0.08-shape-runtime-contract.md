# XYUI-0.08 · Shape Runtime Contract

状态：`RUNTIME CONTRACT IMPLEMENTED / VERIFIED`（无通用 Shape 几何公共 API）。

本合同只冻结 Shape 的几何形态语义。`Radius` 是转角语义，`Border` 是边界语义，`Surface` 是承载面语义；三者可以组合，但互不成为另一者的别名。

## T1 真实能力审计

| Capability | Exists | Canonical Owner | Public API | Consumer | Test |
| --- | --- | --- | --- | --- | --- |
| Base rectangular geometry | Yes | Avalonia `Border` | `Border` | Panel、控件 Chrome | `ShapeRuntimeTests` |
| Ellipse / circle geometry | Yes | Avalonia `Ellipse`，组件模板持有 | 无通用 Shape API | Radio、Switch、Menu indicator | `XYUI2ChoiceControlsTests` |
| Capsule / pill semantic | Partial | `XY.Radius.Full` 仅表达 Full radius | 无独立 Capsule/Pill API | 现有 Tag/Badge 组合 | `SpatialTokenTests` |
| Custom Geometry | Yes, component-local | 具体控件的 `Path`/`Geometry` | 无通用 Custom Geometry API | `XYBadge` Tag path、Icon | `BadgeRuntimeTests` |
| Shape + Radius | Yes | `XY.Radius` / `CornerRadius` | `XY.SetRadius`、`XY.Radius.*` | Border、TemplatedControl | `XYUI08ShapeContractTests` |
| Shape + Border | Yes | `XY.Border` / native Border | `XY.SetBorder`、`XY.Border.*` | Border、控件 Chrome | `XYUI08ShapeContractTests` |
| Shape + Surface | Yes | `XY.Surface` / `XyuiShapeStyles` | `XY.SetSurface`、`xyui-surface-*` | Border、Panel | `XYUI08ShapeContractTests` |
| Shape + Size | Yes | `XY.Size` / native Width/Height | `XY.SetSize`、Width/Height | 所有控件 | `ConsumerApiTests` |
| Clip / HitTest consistency | Component-owned | 具体控件模板 | 无全局 Shape 入口 | Checkbox、Badge、Popup 等 | 组件专项测试 |

不存在的通用能力统一记为：`NO CANONICAL CAPABILITY`，不得由 Gallery 示例反向创建。

## Public API 决策

当前不存在 `XY.Shape`、`XYShape`、`ShapeResolver` 或 `XyuiShapeKind`。本轮保持：

- `NO GENERAL CONSUMER API`：没有两个以上真实 Consumer 需要同一个新的 Shape 几何解析器。
- 业务 Consumer 使用现有 `Border`、`XY.Radius`、`XY.Border`、`XY.Surface` 与具体 XYUI 控件。
- 组件作者可以使用 Avalonia `Ellipse`、`Rectangle`、`Path`/`Geometry`，但几何所有权留在组件模板或专用内部构件。
- `XyuiShapeStyles` 是 Border 的组合样式层，不是第二套几何事实源，也不提供 Shape 类型枚举。

## 组合与限制

- Shape 不读取或改写 Radius；Border 不生成第二套 Radius；基础 Surface 不自动生成轮廓。现有 `xyui-surface-raised` 是明确的 Border + Surface 组合样式，包含 Default border，不代表 Surface 自己拥有 Border 语义。
- Shape 样式只消费既有 Width/Height/`XY.Size`，不因切换形态偷偷改变布局尺寸。
- `Clip`、`HitTest`、Focus outline、Border rendering 和 Desired/Actual Bounds 仍由实际 Consumer 的视觉树负责；当前不能宣称它们由全局 Shape 统一控制。
- Capsule/Pill 目前只有 `XY.Radius.Full` 这一现有圆角入口；不得把它升级为独立 Shape 语义。
- Custom Geometry 仅在真实组件需要时由该组件持有；不得以“通用”名义泄漏 Raw Geometry API。

## Gemini Handoff

- 可展示真实入口：`Border` + `XY.Radius.*` / `XY.Border.*` / `XY.Surface.*`，以及现有 XYUI 控件的真实圆/椭圆/Path 视觉。
- Shape + Radius 示例：`XY.SetRadius(border, "XY.Radius.Control")`。
- Shape + Border 示例：`XY.SetBorder(border, "XY.Border.Strong")`。
- Shape + Surface 示例：`XY.SetSurface(border, "XY.Surface.Panel")` 或 `xyui-surface-panel`。
- Circle / Ellipse：只能展示已有 Radio、Switch、Menu 等真实组件入口；不存在独立公开 Shape 控件。
- Capsule/Pill：`XY.Radius.Full`；没有独立公共入口。
- Custom Geometry：由 `XYBadge` 等具体组件内部持有；没有通用公共入口。
- Gallery 页面装修、Shape SVG 视觉比对和用户视觉验收归 Gemini；本合同不声明 `USER VISUAL ACCEPTED` 或 `FINAL CLOSEOUT`。

## Verification record

- Created：本文件与 `XYUI08ShapeContractTests`。
- Discovered：现有 Radius/Border/Surface facade、Spatial styles、原生几何组件消费者。
- Executed：定向 XYUI-0-08 Shape tests；完整门禁结果以本轮 changelog 为准。
