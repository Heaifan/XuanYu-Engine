# XYE-WAVE2-B-INPUT-READINESS-R2

## 结论

本审计基于冻结 HEAD `aa0c39bae3f47f617fda44333d3bec1f1e1f1e97af39`，统一基线为
`2189e7f4a45d0bbd2b0388e6f26bed110aa2918a`。

结论是 `PARTIAL`：

- `EditorPointerEvent`、Native/Avalonia Adapter 和坐标契约具备测试证据。
- 现有消费者仍由 `VulkanNativeHost` 的 Native/Avalonia 两条直连路径分别路由。
- 当前不存在统一的 `ViewportInputRouter` / Gesture Arbitration 层。
- 因此所有 Camera、Gizmo、Picking、地图编辑消费者暂不能宣称 READY。
- 本轮未启动 Production Input Migration。

## 现状链路

### N1 Native 路径

```text
WM_* message
→ Win32ViewportHost.RouteWndProc
→ NativePointerMessage
→ VulkanNativeHost.OnNativePointerMessage
→ NativePointerRoutePolicy.Resolve
→ VulkanNativeHost owner flags / UiVm consumer
→ ButtonUp / CaptureChanged / KillFocus / CancelMode
```

Native 当前在 `VulkanNativeHost.Pointer.cs` 内执行物理像素到 logical/DIP 的转换，之后把坐标传入 UiVm。

### A1 Avalonia 路径

```text
Avalonia PointerPressed/Moved/Released/Wheel
→ VulkanNativeHost.OnPointer*
→ 直接按 NavGizmo / MapGeometry / Drawing / Transform / Camera 顺序判断
→ UiVm consumer
→ Avalonia Pointer.Capture / CaptureLost
```

当前生产 A1 路径没有调用 `AvaloniaPointerEventAdapter`，而是由 `VulkanNativeHost.AvaloniaPointer.cs` 直接消费事件。

### U1 目标链路

```text
Native or Avalonia source
→ platform adapter
→ EditorPointerEvent
→ unified router / one-owner arbitration
→ consumer
→ unified capture and cancellation lifecycle
```

U1 中间的 unified router / arbitration 尚不存在，这是本轮最大的迁移 GAP。

## Migration Readiness Matrix

状态定义：

- `READY`：仅表示统一事件语义、坐标或修饰键契约已有直接测试证据；不表示生产路由已迁移。
- `GAP`：消费者或输入生命周期仍依赖旧 Host 直连、分散 Owner 或未统一的取消路径。
- `BLOCKED`：本轮没有发现必须先改变 Schema、Renderer 或 Composition 的硬阻断；生产迁移仍需后续 Router 设计。

| Consumer / concern | Current source | Unified event | Routing / owner | Capture / cancel path | DPI safe | Avalonia ready | Status | Gap |
|---|---|---|---|---|---|---|---|---|
| Camera Navigation | N1 / A1 middle press | Pressed / Move / Released | `UiVm._cameraSession`；Host 分支选择 | Native release/cancel；Avalonia release/capture lost；Window Deactivated | logical 坐标可用 | Adapter 有语义，生产未接入 | GAP | 缺统一 Router 与共享 Owner |
| Wheel Dolly | N1 Wheel / A1 Wheel | Wheel | `UiVm.DollyCamera` | 无 drag capture；相机 capture 时拒绝 | wheel 单位已归一化 | Adapter 有语义，生产未接入 | GAP | 两个入口仍分别调用 Camera |
| Navigation Gizmo | N1 / A1 press/move/up | Pressed / Move / Released | `_navGizmoPressed`；随后可能转 Camera | `CancelNavGizmo`；CaptureLost/Native cancel | DIP 几何命中 | 两条路径均有逻辑，未统一 | GAP | Gizmo 与 Camera Owner 转换仍在 Host |
| Transform Gizmo | N1 / A1 left press/move/up | Pressed / Move / Released | `EditorState.InteractionSnapshot.OwnerTool` | Pointer release、CaptureLost、Escape、CancelMode | Consumer 接收 logical | Adapter 有语义，生产未接入 | GAP | 无统一 Gesture Owner 入口 |
| Picking | N1 / A1 left press | Pressed | `UiVm.PickViewportPointer` / Selection | Camera/Transform capture 时拒绝 | ViewportState 同时带 logical/physical | Adapter 有语义，生产未接入 | GAP | Picking 仍由 Host 优先级决定 |
| Selection | N1 / A1 Picking result | Pressed | `ApplyViewportSelection` | 无独立 drag capture | logical x/y 合同已有 | 间接可用 | GAP | Selection 不是 unified sink consumer |
| Map Geometry | N1 / A1 left press/move/up | Pressed / Move / Released | `_mapGeometryDrag` | Release commit；CaptureLost/Cancel 恢复 preview | `CaptureViewportState` 已有 DPI | Adapter 有语义，生产未接入 | GAP | Owner 在 Host flag 与 UiVm 状态之间分裂 |
| Region | N1 / A1 left press/move | Pressed / Move | `_regionDrawing` | Escape；Host detach/cancel 对 draft 不完整 | logical ViewportState | Adapter 有语义，生产未接入 | GAP | Window Deactivated 未覆盖 Region draft |
| Road | N1 / A1 left press/move | Pressed / Move | `_roadDrawing` | Escape；Host detach/cancel 对 draft 不完整 | logical ViewportState | Adapter 有语义，生产未接入 | GAP | Window Deactivated 未覆盖 Road draft |
| Marker | N1 / A1 left press | Pressed | `MarkerPlacementPointerPressed`；立即 commit | 无持续 capture | logical ViewportState | Adapter 有语义，生产未接入 | GAP | 入口仍由 Host 的 Drawing 分支选择 |
| Snap | N1 / A1 Alt + Region/Geometry preview | Move / Modifiers | RegionSnap / GeometrySnap 派生状态 | 随 Region/Geometry cancel | Adapter 能携带 Alt | Adapter 有语义，生产未接入 | GAP | Alt suppression 未进入统一 Router |
| Pointer Capture | Win32 Set/Get/Release 或 Avalonia Capture | Pressed / Released / CaptureLost | Native HWND 或 Avalonia Pointer，各自拥有 | 各自释放，非共享 Owner | 不适用 | 语义可表达 | GAP | 两套 OS/framework capture 生命周期 |
| Pointer Release | N1 ButtonUp / A1 Released | Released | Host 分支分别 commit/end | Native `ReleaseExpectedCapture`；Avalonia `Capture(null)` | logical 可用 | Adapter 有语义 | GAP | 没有统一 release dispatch |
| FocusLost | WM_KILLFOCUS / Avalonia capture chain | FocusLost | Native `CancelNativeInput`；Window/UI 另一路 | Native cancel；Avalonia无统一 FocusLost adapter | 不适用 | 模型可表达 | GAP | Focus source 未进入 sink |
| CancelMode | WM_CANCELMODE | Cancel | Native `CancelNativeInput` | 释放 Native capture 并取消 Camera/Gizmo/Geometry | 不适用 | 无 Avalonia对应 adapter | GAP | 仅 Native source 有此事件 |
| CaptureLost | WM_CAPTURECHANGED / PointerCaptureLost | CaptureLost | Native Host / Avalonia Host 各自清理 | 两条路径分别取消 | 不适用 | 模型可表达 | GAP | 没有共享 cancellation sink |
| Window Deactivation | `UiWin.Deactivated` | WindowDeactivated | `CancelInteractionFromWindowDeactivated` | Transform/Camera；Region/Road draft 不完整 | 不适用 | 已补纯模型枚举 | GAP | 没有 lifecycle adapter 与全消费者闭环 |
| Alt | Native `AltDown` / Avalonia KeyModifiers | Modifiers.Alt | Region snap suppression | 随 preview | 不适用 | Adapter 测试覆盖 | READY* | 生产路由尚未消费 unified event |
| Shift | Native MK_SHIFT / Avalonia KeyModifiers | Modifiers.Shift | Camera Pan 规则 | 随 Camera gesture | 不适用 | Adapter 测试覆盖 | READY* | 生产路由尚未消费 unified event |
| Ctrl | Native MK_CONTROL / Avalonia KeyModifiers | Modifiers.Control | 当前 Viewport consumer 未使用 | 不适用 | 不适用 | Adapter 测试覆盖 | READY* | 需要未来 Router 保持透传 |
| Right Button | Native constants / Avalonia button state | Buttons.Right | 当前 Viewport Host 无右键 consumer route | 无统一 capture | logical 可用 | Sample 可表达 | GAP | Win32 input source 未转发 WM_RBUTTON* |
| Middle Button | N1 / A1 middle press | Buttons.Middle | Camera owner | Release/CaptureLost/Cancel | logical 可用 | Adapter 可表达 | READY* | Camera 生产路径仍未统一 |
| DPI / RenderScaling | `GetDpiScale` / TopLevel.RenderScaling | DpiScale + logical Position | Native adapter boundary | 不适用 | 已测试 1.0/1.25/1.5/2.0 | logical 不重复转换 | READY* | 生产 Native 路径仍有独立转换 |
| Logical Coordinate | Avalonia Point / Native physical÷DPI | Position | 所有 Consumer 目标坐标 | 不适用 | 统一模型已冻结 | 已测试等价位置 | READY* | Consumer 尚未改成只接 unified event |
| Physical Pixel | Native `PhysicalX/Y` | Adapter-only input | 不应进入 Consumer | 不适用 | Native adapter 一次转换 | N/A | READY* | 旧 Host 仍持有 physical source |

`READY*` 是契约层 READY，不得解释为 Production Migration READY。

## Owner 与取消闭环审计

现有 Owner 证据：

- Camera：`UiVm.BeginCameraNavigation` 拒绝已有 Camera/Interaction capture。
- Transform Gizmo：`MoveTransformUiTests.Session` 已证明第二 Pointer、Camera、Picking 在已有 Move capture 时被拒绝。
- Navigation Gizmo：`_navGizmoPressed` 在 Host 内拥有独立状态，并在 `CancelNavGizmo` 清理。
- Map Geometry：`_mapGeometryDrag` 在 commit/cancel/capture lost 清理。
- Region/Road：各自拥有 draft state，但取消主要依赖 Escape；Window Deactivation 闭环不足。

现有取消源：

| Source | Current handling | Unified readiness |
|---|---|---|
| Released | Native route / Avalonia release | GAP：各自直接 dispatch |
| WM_CAPTURECHANGED | `HandleNativeCaptureChanged` | GAP：无共享 sink |
| PointerCaptureLost | Avalonia Host override | GAP：无共享 sink |
| WM_KILLFOCUS | Native route → `CancelNativeInput` | GAP：模型可表达，source 未统一 |
| WM_CANCELMODE | Native route → `CancelNativeInput` | GAP：Avalonia 无对应 adapter |
| Window Deactivated | `UiWin` → `CancelInteractionFromWindowDeactivated` | GAP：Region/Road draft 未统一清理 |
| Host detach | `VulkanNativeHost.OnDetachedFromVisualTree` | GAP：仍是 Native Host 生命周期 |

## 测试证据

本轮新增：

- `UnifiedPointerReadinessContractTests`：Native physical→logical 一次转换，覆盖 DPI 1.0/1.25/1.5/2.0。
- Native/Avalonia 等价 Right + Shift/Ctrl/Alt 语义比较。
- `Released / CaptureLost / FocusLost / Cancel / WindowDeactivated` 契约枚举覆盖。
- Window Deactivation 对真实 Move Gizmo session 的取消与无历史残留验证。

已有相关证据：

- `NativePointerEventAdapterTests`：Move、Left/Right/Middle Down/Up、Wheel、修饰键、Capture/Focus/Cancel。
- `AvaloniaPointerEventAdapterTests`：Avalonia logical position、Wheel、Modifier 与 Native 等价语义。
- `NativePointerRoutePolicyTests`：Middle camera route 与 active-owner 优先级。
- `MoveTransformUiTests.Session`：单 Owner、stale Pointer、PointerCaptureLost、WM_CANCELMODE。
- `CameraNavigationUiTests`：Camera capture 阻止 Dolly/Picking，取消恢复起始状态。

## 本轮明确不做

- 不切换 `VulkanNativeHost`、`Win32ViewportHost` 或 Production Viewport Host。
- 不删除 NativeControlHost、WS_CHILD、Win32 bridge 或 Native capture。
- 不修改 Camera/Picking/Gizmo/Region/Road/Marker/Snap 的生产行为。
- 不修改 Vulkan renderer、Composition、Composition Root 或 A1.5 Spike 文件。
- 不引入新的生产 Input Owner 或 global input singleton。

## WAVE-1 / WAVE-2 进入条件

本分支只证明统一事件契约的输入语义和现状 GAP。进入真正消费者迁移前必须另行完成：

1. 建立统一 Viewport Input Router 与唯一 Gesture Owner arbitration。
2. 将 Native/Avalonia 两个 Adapter 都接入同一 sink，不再由 Host 直接调用 Consumer。
3. 把 Released、CaptureLost、FocusLost、Cancel、WindowDeactivated、Host detach 收入同一 cancellation lifecycle。
4. 先补 Region/Road Window Deactivation 取消闭环，再迁移其 Consumer。
5. 增加真实 Avalonia Pointer Capture 与 Window Deactivated runtime regression。

本报告不得被解释为 Production Migration 已开始。
