# UI 用户界面知识

## K-UI-001 冷启动错位或“操作一次恢复”应优先检查 Measure/Arrange 与真实命中热区

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Avalonia、Measure、Arrange、ScrollViewer、Star Column、Hit Target、Cold Start
**适用范围**：LayerPanel、Inspector、侧栏列表、可拖拽图标、任何首次布局与交互区域。

**首次根因确认**：2026-08-09 16:18:16（UTC+08:00）
**版本**：`v0.2.24.49-fix`
**Commit**：`74576e5eb722188cc4bcd7f11d973c8e764a129e`
**最终收口**：`v0.2.24.50-fix` · 2026-08-09 19:42:41 · `60fd339`
**来源**：`changelog.md`

### 问题

UI 在冷启动时错位，但点击“添加”、切换面板或 Resize 后突然恢复，常常不是 ViewModel 数据变对了，而是第一次 Measure/Arrange 的约束不正确，第二次布局才得到合理宽度。另一个相关误区是把视觉图标尺寸直接当作 Pointer 命中尺寸。

### 真实历史示例 A：无限测量

LayerPanel 外层 `ScrollViewer` 没有禁止横向无限测量，Inspector 隐藏时名称 `*` 列失去可用宽度。用户观察到冷启动异常、操作一次后恢复。修复通过禁用横向滚动、保持页面/ListBox 横向拉伸，并在 `v0.2.24.50-fix` 使用 Auto/Auto/* Grid 与 Avalonia.Headless Runtime Gate 锁定冷启动宽度稳定性。

### 真实历史示例 B：视觉尺寸 ≠ 命中尺寸

拖拽事件直接绑在 14 DIP Path 上，实际命中区太小。修复把 14 DIP 图标保留为视觉子元素，用 24×28 DIP 透明 Border 承担真实 Pointer 热区。

### 工程规则

出现以下症状时，优先审计布局而不是先重写业务状态：

- 冷启动错，Resize 后正确；
- 切换 Tab 后正确；
- 添加/删除一项后突然正确；
- Inspector 展开/隐藏改变其它列宽；
- 文本 `*` 列在 ScrollViewer 内异常收缩/扩张。

检查顺序：

```text
Measure constraint
→ ScrollViewer horizontal policy
→ Grid Auto/* allocation
→ MinWidth / MaxWidth
→ HorizontalAlignment / Stretch
→ Arrange result
→ Runtime bounds
```

交互上，Visual Size 和 Hit Target Size 应分离设计。

### 未来应用示例

若新的“图层锁”图标视觉要求只有 14×14 DIP，可以保持精致外观，但外层使用至少 24×28 DIP 的透明/无视觉 Border 接收 Pointer；不要把可点击性压到图标本体。

### 禁止做法

- 看到“添加后恢复”就加一次强制 Refresh/Invalidate 作为永久修复。
- 通过固定超大 Width 掩盖 Star Column 测量问题。
- 为扩大命中区直接把图标画得巨大。
- 只有静态 XAML 字符串测试，没有实际 Runtime Measure/Arrange。

### 验证方法

- Avalonia.Headless 实例化真实控件；
- 冷启动尺寸断言；
- 添加/删除前后宽度稳定性；
- Inspector 显隐；
- 多窗口尺寸/DPI 真机验证；
- Pointer 命中测试使用真实 Bounds，而非视觉 Path 尺寸。

**关联 Incident**：INC-2026-08-09-001
**关联 Knowledge**：K-VAL-002

## K-UI-002 同功能 XYUI 子控件必须复用现有 XYUI 控件合同

**状态**：Active
**优先级**：P1
**证据等级**：E2
**标签**：Avalonia、XYUI-1、XYUI-2、Composition、Reuse、Inheritance、Source of Truth
**适用范围**：所有复合 XYUI 控件、Popup 面板、编辑器属性面板及 Gallery 示例。

**首次确认**：2026-08-29 18:40:31（UTC+08:00）
**来源**：`changelog.md` · XYUI-2-19/20 复用审计

### 工程原则

复合控件内部若出现与现有 XYUI 控件功能相同的子控件，必须直接实例化并复用该公开 XYUI 控件，使 UI、状态、键盘、Pointer、Scrub、焦点和错误提示继承同一份控件合同。允许复用，不允许在复合控件内重新实现一套等价的原生 Avalonia 控件交互。

标准复用关系包括：`XYNumberField : XYTextField`；滑块功能使用 `XYSlider`；文本输入使用 `XYTextField`；布尔属性使用 `XYSwitch`。`XYSlider` 内部的原生 `Slider` 与 `XYNumberField` 是它自己的实现细节，复合控件不应绕过 `XYSlider` 直接创建原生滑块。

### 本轮 XYUI-1/2 审计结论

- XYUI-1 全部 24 项已检查。文本类使用 `XyuiTextComponent`、`XyuiTextSurface`、`XyuiVectorTextSurface` 等共享基类；`SelectableTextBlock`、快捷键帽、状态点和分割线属于专用视觉/选择原语，没有发现可替换为现有 XYUI 公共控件的同功能子控件。
- XYUI-2 已检查全部 20 项。`XYComboBox` 复用 `XYTextField`；`XYNumberField` 继承 `XYTextField`；`XYTextArea`、`XYSearchField`、`XYPasswordField` 复用共享可编辑文本基类；`XYBoolProperty` 复用 `XYSwitch`；`XYColorPicker` 的色相/透明度使用 `XYSlider`，HEX 使用 `XYTextField`，R/G/B/A 使用 `XYNumberField`。
- `XYSplitButton`、`XYDropDownButton`、`XYSearchField`、`XYPasswordField` 的清除/筛选/眼睛/菜单等 Button 是专用操作槽，不是完整的同功能 `XYButton`；`XYDatePicker`、`XYTimePicker` 的日期/时间分段和日历/时钟按钮是专用值编辑入口。这些属于有记录的低层例外，不得借“复用”改变其事件 Owner 或重复触发语义。
- Checkbox、Radio、Switch 以及 XYUI-1 的 Border/TextBlock/Ellipse 等仅承担外观原语；只有存在同功能公共 XYUI 控件时才要求替换。

### 门禁

新增 `XYUICompositionReuseTests`，锁定公开复合控件的真实子控件类型与 XYUI-1 文本共享基类；ColorPicker 测试继续验证 HEX、透明度和颜色变化。后续新增复合控件必须在测试中列出复用子控件与专用槽例外，并通过 Avalonia.Headless 运行时检查。
