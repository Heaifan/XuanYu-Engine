# XYUI-0.10 · States Runtime / Public API Truth

本文件纠正此前的编号错误：States 的正式编号是 `XYUI-0-10`，本文件取代旧的 States Runtime 合同。附件任务书是本轮执行范围，源码与自动化证据是 API 真值来源。

## Runtime 可见性

- `XyuiInteractionFacts`、`XyuiStateSnapshot`、`XyuiStateResolver` 当前源码声明为 `public`。
- `XyuiStateResolver.Resolve` 与 `ResolveSemantic` 是可调用的 Public API，但普通业务 Consumer 不应直接调用；Gallery 应使用真实控件 Public API 或原生伪类/组件样式。
- 源码目前不是 `internal`。因此不得把交接文档写成“Resolver 是 Internal”；准确口径是：`PUBLIC IN SOURCE / DO NOT CALL DIRECTLY`。
- `XyuiInteractionStyles` 通过 `xyui-active`、`xyui-dragging`、`xyui-drop-target`、`xyui-readonly`、`xyui-locked` Class 提供 Foundation 消费入口；这些 Class 不是统一的 Public 状态属性。

## Public API 真值表

| State | Public Trigger / API | Source | Auto? | Notes |
|---|---|---|---|---|
| Rest | 无；默认无状态 | Runtime default | 是 | 不应由 Consumer 伪造视觉属性 |
| Hover | `:pointerover` | Avalonia Runtime Pointer | 是 | `XyuiInteractionState.Hover` 只匹配原生伪类 |
| Pressed | `:pressed` | Avalonia Runtime Pointer | 是 | 与 Hover 共存时 Pressed 视觉优先 |
| Focus | `Control.Focus()`、`Focusable`、`:focus` | Avalonia Runtime Focus | 是 | 当前源码没有 `:focus-visible`；指针焦点与键盘焦点走同一 `:focus` 显示路径 |
| Disabled | `Control.IsEnabled = false`、`:disabled` | Consumer + Avalonia Runtime | 是 | Disabled 为最高视觉优先级；不是 ReadOnly 的别名 |
| Selected | `XYToggleButton.IsChecked`；组件的 `IsSelected` / `SelectedItem` / `Select(...)` | ToggleButton 或组件 Selection owner | 否 | 真实来源包括 `XYNavigationItem.IsSelected`、`XYTab.IsSelected`、`XYMenuItem.IsSelected`、`XYToolbarTool.IsSelected`、`XYTreeNode.IsSelected`；不得手写视觉属性模拟 Selected |
| ReadOnly | `TextBox.IsReadOnly`；`XYBoolProperty.IsReadOnly`、`XYNumberProperty.IsReadOnly`、`XYEnumProperty.IsReadOnly`、`XYReferenceProperty.IsReadOnly`、`XYVectorProperty.IsReadOnly` | Consumer | 否 | 多个真实 XYUI 控件支持；它改变编辑/操作能力，不等同 Disabled |
| Locked | `NO GENERAL CONSUMER API` | Foundation Semantic / Token；组件可有专属语义 | 否 | 只有 `XyuiInteractionFacts.Locked` 与 `XY.State.Locked.*` Foundation 合同；不得冒充 Warning |
| Active | `XYMenuBarItem.IsActive`；部分组件有专属 Active 属性/派生标识 | Component | 否 | 没有统一 Consumer Active API；`ActiveToolId` 是组件派生结果，不是通用状态写入口 |
| Dragging | `XYDockTab.IsDragging`、`XYSliderTrack.IsDragging` | Component Lifecycle | 否 | 仅组件/内部生命周期拥有；`XyuiInteractionFacts.Dragging` 是 Foundation 解析事实 |
| DropTarget | `NO GENERAL CONSUMER API` | Foundation Lifecycle / Token | 否 | 只能由拥有拖放生命周期的组件决定是否消费 `xyui-drop-target` |

## Focus 真值

- Hover、Pressed、Focus 的原生选择器分别是 `:pointerover`、`:pressed`、`:focus`，由 Avalonia Runtime 产生。
- `XYSelect` 的指针按下路径会调用 `Focus()`；一般控件的键盘 Tab/显式 `Focus()` 也进入同一个 `:focus` 样式路径。
- `XyuiStateSnapshot.FocusVisible` 是 Resolver 的独立输入/输出合同，不代表当前控件已经提供 keyboard-only `:focus-visible` 策略。
- Focus Outline 独立于 Selected；不会覆盖 Selection Identity，也不应改变 Width、Height、Margin 或布局位置。

## Selected 真值

- `XYToggleButton` 继承 Avalonia `ToggleButton`，公共状态源是 `IsChecked`，由原生 `:checked` 参与组件视觉。
- Navigation、Tabs、Menu、Toolbar、Tree 等组件分别由各自的 `IsSelected`、`SelectedItem`、`Select(...)` 或 Selection Model 管理选择。
- Consumer 应设置这些语义 API，由组件/样式产生 Selected 视觉；不应直接写 Background、BorderBrush 等视觉属性假造 Selected。

## ReadOnly / Locked 真值

- ReadOnly 已在 `XYTextField` / `XYTextArea` 的继承链 `TextBox.IsReadOnly`，以及五个 Property 控件的专属 `IsReadOnly` 上落地。
- Locked 没有统一 Public Consumer API；当前只有 Foundation token、Resolver fact 和 Class 样式入口。Gallery 不得展示一个不存在的 `IsLocked` 公共属性。

## Active / Dragging / DropTarget 真值

- Active、Dragging、DropTarget 由组件或拖放生命周期产生；普通 Consumer 不应把它们当作全局通用属性设置。
- 组件作者在拥有持久激活、拖拽生命周期或拖放命中判定时，才负责把事实映射到组件专属 API/Class；Foundation Resolver 不替代组件所有权。

## Gemini 精简交接

```text
Hover        AUTO（:pointerover）
Pressed      AUTO（:pressed）
Focus        AUTO（:focus；没有 :focus-visible 分流）
Disabled     Control.IsEnabled = false
Selected     XYToggleButton.IsChecked 或组件自己的 IsSelected/Selection API
ReadOnly     TextBox.IsReadOnly 或各 Property 控件的 IsReadOnly
Locked       NO GENERAL CONSUMER API；Foundation semantic/token only
Active       Component-specific；无统一 Consumer API
Dragging     Component lifecycle；无统一 Consumer API
DropTarget   NO GENERAL CONSUMER API；由拖放拥有者决定
Resolver     PUBLIC IN SOURCE / DO NOT CALL DIRECTLY
编号         XYUI-0-10
```

Gallery 当前 `StatesView.axaml` 仍发现“已锁定 · 0.08”标签；该文件属于 Gemini Gallery 范围，已作为编号纠正事项上报，不由 Codex 越权修改。

## 测试数量与证据

- Canonical test command：`E:\MyApp\sdk-dotnet\dotnet.exe test .\xyui\avalonia\tests\XYUI.Avalonia.Tests\XYUI.Avalonia.Tests.csproj --no-build --no-restore`
- 当前 checkout：Discovered `412`，Passed `412`，Failed `0`，Skipped `0`。
- `367/367` 只出现在 `v0.2.28.61-rz`（2026-09-01）的历史 changelog；它对应更早修订的测试集合，不是与当前 `412/412` 同一次命令、同一 checkout 的结果。两者差异属于历史 revision drift；没有 Gemini 的原始命令/checkout 证据，不能进一步归因。
- Full solution Build：0 Warning / 0 Error；Core `339/339`、WarCore `22/22`、World `1286/1286`、XYUI.Avalonia `412/412` 均已通过。
- ARCH-A：`ARCH-A BLOCKED BY PRE-EXISTING UNTRACKED FILE`，原因是 `XYUIProbeTests.cs` 149 行；本轮新增/改名 C# 文件均不超过 100 行。`git diff --check` 已通过。
- SDK 偏离：`D:\MyApp\sdk-dotnet\dotnet.exe` 当前不存在，实际使用 `E:\MyApp\sdk-dotnet\dotnet.exe`，版本 `10.0.400`。

## 完成状态

```text
XYUI-0-10 STATES
NUMBERING CORRECTED
RUNTIME TRUTH TABLE CONFIRMED
TEST COUNT RECONCILED
READY FOR GEMINI DEVELOPER-DOC GALLERY UPDATE
```
