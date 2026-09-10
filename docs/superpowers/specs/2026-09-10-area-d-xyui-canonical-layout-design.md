# AREA-D-R2-FIX4 Design

## Goal

修正 XYUI canonical 的页签、按钮、标签和图标对齐/可视度契约，并把区域编辑内容收拢到 Inspector 的单一滚动页面；LayerDock 只占用独立可调整高度。

## Scope and boundaries

本轮允许修改 `xyui/**` 与 Area D 右侧 Inspector/LayerDock 组合、相关测试、Gallery 和真实执行记录。

本轮禁止修改：

- `XuanYu.Editor.UI/Win/UnsavedChangesConfirmationWindow.axaml`
- XYUI-7、Entity Inspector 业务、Hierarchy、Debug、Area C、Bottom Log、Renderer、Map/Region domain schema
- Region Dataset、绘制、Snap、Vertex、Undo/Redo、Selection、Map persistence 业务实现

图片是视觉验收参考；自动测试只声明结构/布局契约，不声明像素级用户验收。

## Canonical design

### Tab sizing

新增通用 `XyuiTabSizingMode`：

- `Equal`：兼容默认值，保留等宽页签能力。
- `Content`：页签按内容、图标、关闭槽和 canonical 水平 padding 的期望宽度排列。

`XYTabs` 保持单组页签职责，不在其内部新增业务溢出按钮；`XYTabBar` 继续负责需要滚动和溢出的宿主。`Content` 模式的 `XYTabs` 不通过 Engine 固定 Width；`XYTabBar` 通过已有 ScrollViewer 保证超出宿主时仍可滚动、前后导航和 Overflow 菜单继续可用。

玄域左侧 `项目/文件`、右侧 `检查器/层级/调试` 和地图二级 `地图基础/地图环境/数据集` 使用同一 `Content` 模式与 Compact 密度。其他 XYUI 消费者继续使用默认 `Equal`，除非明确选择 `Content`。

### Button and icon alignment

共享 `XyuiButtonChrome` 的 ContentPresenter 负责整个 content 几何中心。`XYButton` 的 text-only 与 `Icon + Text` 组合均以按钮 content 区为中心，不使用 Engine 局部 TextAlignment/Margin 修补。`XYIconButton` 保持 34×34 DIP 和 canonical icon viewport；图标使用 XYIcon 自己的统一绘制居中逻辑。

当前图层行实际使用 `XYToggleButton` 承载 `IsChecked` 双向绑定和 Path 内容。本轮不把它错误替换成命令型 `XYIconButton`；将共享 toggle content 对齐到中心，并保留文本 toggle 的既有状态、Focus、Hover、Pressed、Disabled 和 Action Edge 语义。

图层 Tag 实际使用 `XYBadge`。保留 22 DIP 左指针几何和 Default/Accent 语义，只调整 canonical Light theme 下的前景/背景/边界 token 组合，使普通、选中和禁用状态形成清晰层级；Dark theme 以现有 token 契约回归验证。

### Region Inspector and LayerDock

`InspectorPanel` 继续拥有唯一的 `ScrollViewer`。Region edit content 组合为同一滚动内容顺序：

1. 区域属性/空状态或选中区域属性
2. 内容类型
3. 当前绘制目标
4. 区域面/道路/地图标记编辑内容

`RegionalAuthoringPanel` 从 `Right` 的底部独立行移入 `InspectorPanel`，仅重组 View composition，不改变其 ViewModel 路由和绘制命令。

`Right` 的底部只承载 `EditorLayerDock` 与 GridSplitter。LayerDock 展开默认高度提高到可自然显示多个图层行，图层列表自身保留内部滚动，收起时只保留 Header。Splitter 改变 Inspector 的 viewport 与 LayerDock 的独立可用高度，不改变 Inspector 内部逻辑顺序或 ScrollViewer 归属。

## Tests and Gallery

XYUI tests will prove:

- Content and Equal sizing produce the expected differing/equal tab bounds.
- `XYTabBar` overflow remains scrollable and selectable.
- XYButton text and Icon+Text content presenter is centered horizontally and vertically.
- XYIconButton and XYToggleButton icon/content host is centered without per-icon correction.
- XYBadge Light/Dark Default/Accent/Disabled token resolution remains semantic and distinguishable.

Engine tests will prove:

- Region authoring appears inside the Inspector scroll host, after region properties and before the LayerDock.
- LayerDock resizing changes available viewport allocation while preserving Region content order and one scroll host.
- Existing Region drawing, dataset/layer selection and display/lock bindings remain routed to the same commands/properties.

Gallery will show XYTabs Content vs Equal, XYButton text and Icon+Text, XYIconButton/toggle icon sizes and states, and XYBadge Light normal/selected/disabled contrast.

## Acceptance boundary

Formal build/test/ARCH-A/XML/version/diff gates can establish automated readiness. Real-device visual and interaction acceptance remains user-owned; final status must be `READY FOR USER VISUAL + INTERACTION ACCEPTANCE`, not `CLOSED`.
