# DIAG-VIEWPORT-R1-FIX8 Viewport Diagnostic 真入口设计

## 目标

在诊断模式下，鼠标位于 Vulkan Native Viewport 时，Diagnostic 必须以 `XYE.VIEWPORT` 作为唯一活动目标；卡片锚定 Native pointer 并留在 Window ClientRect 内，不遮挡 pointer hot zone。鼠标离开 Native Viewport 后恢复 Avalonia 元素诊断，且不改变 Camera、Picking、Gizmo、Map 或统一手势输入行为。

## 范围与约束

- 基线：`05317f597e176d7e62cc0c4d98108745e5f197fb`。
- 风险：HIGH；涉及 Native/Avalonia 诊断目标所有权与跨层事件顺序，但不改变生产输入契约。
- Native Pointer 旁路只观察，不设置 `Handled`、不 Capture、不 Consume、不改变 Gesture Owner。
- `Editor.UI` 不依赖新的 Vulkan 实现；只使用现有 `VulkanNativeHost` 旁路事件。
- 所有新增或修改的手写 `.cs` / `.axaml` 文件保持不超过 100 行；必要时按职责拆分。
- 不改 `ViewportInputRouter`、`GestureOwner`、`ViewportGestureLifecycle`、Camera/Picking/Gizmo/Map/Snap 生产链。

## 设计

### Gate-A：Native Viewport Diagnostic Target

`Win32ViewportHost` 将现有 Native pointer sink 继续送入 `VulkanNativeHost`，并由 host 产生 Diagnostic-only 生命周期通知：`Entered`、`Moved`、`Exited`。通知只描述来源 host、逻辑坐标和 Native lifecycle，不复用生产 dispatch 结果。

`VulkanNativeHost` 在首个有效 Native Move 时报告进入与移动，在离开条件明确时报告退出；销毁、失焦、取消和解除 sink 也必须清理 Native Diagnostic 状态。Diagnostic 订阅该旁路并生成固定语义目标：`DebugId = XYE.VIEWPORT`、`ControlType = VulkanViewport`、`TargetKind = Viewport`。

### Target ownership

`DiagnosticOverlayHost` 增加短生命周期的 Native Viewport override。override 存在时，Avalonia Window/UiWin/外围容器收到的 `PointerMoved` 不能调用 `ProbeHover` 覆盖当前目标；它们只能更新普通指针记录。Native Exit、关闭诊断、Unload、Window Deactivate 时清空 override 与卡片/Highlight。

普通 Avalonia pointer 事件仍通过现有 `DiagnosticProbeResolver`，因此离开 Viewport 后自动恢复普通元素诊断。

### Gate-B：Viewport Card Placement

Viewport target 使用现有 placement policy 的 pointer-anchored 四象限候选：右下、右上、左下、左上。候选只校验 Window ClientRect 与 pointer hot zone；允许卡片和 Viewport bounds 相交。候选全部不可用时，fallback 仍必须避开 hot zone并在 Window ClientRect 内。

Viewport Highlight 只作视觉标记，`IsHitTestVisible=false`，不参与卡片避让。连续 Native Move 复用当前 card/highlight，只更新位置和快照，不销毁/创建 Popup。

### 输入安全

Diagnostic observer 在 Native pointer 回调中只更新自身状态。现有生产 `OnNativePointerMessage` 路由、Capture、`ViewportInputRouter`、Gesture Owner 和各消费者保持不变。测试必须证明 observer 不改变生产 owner 与 pointer capture。

## 验证合同

自动测试至少覆盖：

1. Native Move 建立 `XYE.VIEWPORT`。
2. Native Move 后的 Avalonia UiWin Move 不能抢回目标。
3. Native Exit 清空 override，下一次 Avalonia Move 恢复普通元素目标。
4. Viewport 卡片允许与 Viewport bounds 相交。
5. Viewport 卡片不与 pointer hot zone 相交且在 Window ClientRect 内。
6. 连续 Native Move 只复用并移动卡片，不重复创建 Popup。
7. Diagnostic observer 不改变 production gesture owner 或 pointer capture。
8. 关闭 Diagnostic Mode 清空 override、card 和 highlight。

真机验收仍需使用 `run.bat`：中央 Viewport 显示 `VulkanViewport / XYE.VIEWPORT`；左右移动卡片跟随并不遮挡热点；移入 Inspector 后立即切换为普通 Avalonia 目标且无残留。
