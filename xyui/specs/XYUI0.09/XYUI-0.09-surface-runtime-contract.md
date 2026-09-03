# XYUI-0.09 · Surface Runtime Contract

状态：`RUNTIME CONTRACT IMPLEMENTED / VERIFIED / READY FOR GEMINI GALLERY IMPLEMENTATION`

## Surface Definition

Surface 表示内容承载的层级语义。它不是任意灰度背景，也不拥有几何、转角、边界、阴影或浮层生命周期。

| Surface | Canonical Light / Dark | 典型承载语义 |
| --- | --- | --- |
| App | `#EEF2F5 / #151C22` | 应用底层 |
| Panel | `#F5F8FA / #1C252C` | 工作区面板 |
| PanelAlt | `#F9FBFC / #222E36` | 次级面板、工具区域 |
| Raised | `#FFFFFF / #2A3842` | 卡片、抬升内容面 |
| Canvas | `#E6ECEF / #182128` | 画布承载面 |
| Toolbar | `#F9FBFC / #222E36` | 工具栏承载面 |
| Input | `#FFFFFF / #2A3842` | 输入控件承载面 |
| Overlay | `#FFFFFF / #31424D` | 覆盖内容承载面 |
| Selected | `#D8E7F2 / #35536A` | 选中状态承载面 |
| BorderReference | `#C9D3DA / #40515C` | 边界参考承载面 |

Canonical 来源是 `XyuiColorTokens.Surface`，Light/Dark 由 `XyuiTheme` 主题资源生成。

## Public API

```xml
<Border xy:XY.Surface="XY.Surface.Panel" />
```

链路为：`XY.Surface` Attached Property → Surface 专用 Canonical Resolver → `XY.Brush.Surface.*` → Border 或 TemplatedControl 的真实 Background。

Surface Property 支持继承；子控件未设置本地值时继承父级值，本地值覆盖父级值。没有 Surface 值时，Facade 不注入默认背景；真实 Consumer 的默认值由其自身 Template/Style Contract 提供。

## Composition Boundaries

- Surface 与 `XY.Border`、`XY.Radius`、`XY.Size` 正交组合，不修改 BorderThickness、CornerRadius、尺寸或布局。
- Shape 0-08 没有全局 `XY.Shape` API；Surface 不定义 Shape。
- `Raised` 只表达表面层级。现有 `XY.Shadow.Popup` / `XY.Shadow.Tooltip` 是独立空间资源；Raised 不等于 Popup，也不自动附加阴影。
- Popup/Tooltip 是生命周期与行为边界；当前 Surface Canonical 没有 `Popup` 或 `Tooltip` 成员，不能由 Gallery 或 Consumer 伪造。
- 现有 Popup/菜单/控件模板可消费 `Overlay`、`Raised` 或 `PanelAlt`，但这属于具体 Consumer 的样式选择，不是 Surface 自动管理 Popup/Tooltip 生命周期。

## Capability Truth Table

| 能力 | 状态 | 事实 |
| --- | --- | --- |
| Default Surface | 有限支持 | 无值时不注入；Consumer 自己提供默认 |
| Inheritance | 支持 | Attached Property `inherits: true` |
| Local Override | 支持 | 子级本地值覆盖继承值 |
| Surface + Radius/Border/Size | 支持 | 各自写入独立原生属性 |
| Surface + Shape | 不提供 | `NO CANONICAL CAPABILITY` |
| Popup/Tooltip 自动接线 | 不提供 | `NO CANONICAL CAPABILITY` |
| Shadow / elevation 统一系统 | 不提供 | 仅有独立 Tooltip/Popup/DragPreview 阴影资源 |

## Limitations and Gemini Handoff

真实 Public XAML 只有 `xy:XY.Surface="..."`；真实成员名单以上述十项为准。可展示 Border、TemplatedControl、Panel 和现有控件的 Surface 消费。不得展示不存在的 `XY.Surface.Popup`、`XY.Surface.Tooltip`、`XY.SurfaceRole` 或通用 Elevation API。

验证由 `FoundationFacadeRuntimeTests` 覆盖：Canonical 成员边界、Light/Dark Token 来源、默认行为、继承、Local Override 以及与 Border/Radius 的组合。Gallery Presentation 和用户视觉验收归 Gemini/用户负责。
