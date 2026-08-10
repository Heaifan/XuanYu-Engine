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
