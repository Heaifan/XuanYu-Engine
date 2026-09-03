# XYUI-0.08 · States Foundation Runtime Contract

状态事实与视觉解析由 `XYUI.Avalonia.Interaction` 统一拥有。本轮不新增 Gallery 视觉真值。

## Runtime API

- `XyuiInteractionFacts`：Flags 事实集合，包含 `Hover`、`Pressed`、`Selected`、`Active`、`Disabled`、`Dragging`、`DropTarget`、`ReadOnly`、`Locked`。
- `XyuiSemanticStatus`：独立语义反馈，只有 `None`、`Success`、`Info`、`Warning`、`Error`。
- `XyuiStateSnapshot`：交互事实 + 独立 `FocusVisible` + 独立 Semantic 状态；事实可以同时成立。
- `XyuiStateResolver.Resolve`：每次只解析一个 Background、Border、Foreground，并额外返回 Focus Outline 与 Selection Identity。
- `XyuiStateResolver.ResolveSemantic`：将语义状态解析为独立的 Background/Border/Foreground token，不并入交互枚举。

## 解析真值表

| 通道 | 解析优先级/规则 |
| --- | --- |
| Background | Disabled > Pressed > Hover > DropTarget > Dragging > Locked > ReadOnly > Active > Selected |
| Border | Disabled > DropTarget > Locked > ReadOnly > Selected；Focus 不覆盖 Selection Identity |
| Foreground | Disabled > Locked > ReadOnly；其余由组件默认值决定 |
| Focus Outline | `FocusVisible` 独立输出 `XY.Border.Color.Focus`；Disabled 时不输出可见环 |
| Selection Identity | `Selected` 独立保留 `XY.Border.Color.Selected`，不因 Hover/Focus/Dragging 消失 |
| Semantic | Success/Info/Warning/Error 仅使用 `XY.Semantic.*`，不是交互状态 |

所有状态转换均不修改控件宽度、高度或布局位置；Foundation 默认 `ResizeOnChange = Forbidden`。

## Canonical tokens

- Interaction：`XY.State.Color.Hover`、`Pressed`、`Selected`、`Active`、`Dragging`。
- Drop target：`XY.State.Color.DropTarget.Background`、`XY.State.Color.DropTarget.Border`。
- Availability：`XY.State.Disabled.*`、`XY.State.ReadOnly.*`、`XY.State.Locked.*`。
- Focus：`XY.Border.Color.Focus`、`XY.Border.Width.Focus`。
- Semantic：`XY.Semantic.Success.*`、`Info.*`、`Warning.*`、`Error.*`。

以上均复用现有 `XyuiColorTokens`，不新增 Hex 真值。

## Gemini handoff

| 项目 | 当前事实 |
| --- | --- |
| 可交互展示 | `XYButton`、`XYToggleButton`、`XYTextField`、`XYComboBox` 可通过原生伪类展示 Hover/Pressed/Focus/Disabled；`ListBoxItem` 可展示 Selected |
| Foundation fixture | 给任意 `TemplatedControl` 添加 `xyui-active`、`xyui-dragging`、`xyui-drop-target`、`xyui-readonly`、`xyui-locked` 类即可消费通用样式 |
| Selected + Hover | Hover 解析为背景，Selected Identity 保留 |
| Selected + Focus | Selected Identity 保留，Focus 独立输出；现有组件 Focus 规则使用 Focus token |
| Disabled/ReadOnly/Locked | 三个事实与三组 token 分离；ReadOnly 不等同 Disabled，Locked 不别名 Warning |
| Navigation/Property | `XYNavigationItem`、`XYReferenceProperty` 保持组件专属所有权，本轮不扩大改造；Gallery 需使用其真实 API |

## Evidence

- Created：`XyuiStateSnapshot.cs`、`XyuiStateResolver.cs`、`XYUI08StateResolverTests.cs`。
- Discovered：现有 `InteractionStateTests` 与 `InteractionCombinationTests` 共 12 项通过，覆盖 Button/ToggleButton/ListBoxItem 的真实样式消费。
- Executed：XYUI-0-08 Resolver targeted tests 7 项，全部通过；代表性现有状态测试 12 项，全部通过；完整解决方案 Build、四个测试项目共 2059 项测试全部通过；`git diff --check` 通过。
- Pending：ARCH-A 仍被既有未跟踪 `XYUIProbeTests.cs` 的 149 行长度拦截；用户 Gallery 视觉/交互验收仍待执行；不得提前声明 `USER VISUAL ACCEPTED` 或 `FINAL CLOSED`。
