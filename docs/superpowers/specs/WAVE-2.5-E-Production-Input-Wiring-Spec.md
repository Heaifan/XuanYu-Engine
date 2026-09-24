# WAVE-2.5-E Production Input Wiring Spec

## 1. Goal

把 Native 与 Avalonia 的真实生产输入统一转换为 `EditorPointerEvent`，只经一个 `ViewportInputRouter` 进入 explicit arbitration、唯一 `GestureOwner`、唯一 consumer 和 `ViewportGestureLifecycle`。Host 不得直接调用 UiVm 输入入口；Diagnostic 只能旁路观察。

## 2. Target graph

```text
Win32 WndProc -> NativePointerMessage -> NativePointerEventAdapter
                                                |
Avalonia events -> AvaloniaPointerEventAdapter ----------------> EditorPointerEvent
                                                                  |
                                                        ViewportInputRouter
                                                                  |
                                                     explicit arbitration
                                                                  |
                                                        GestureOwner
                                                                  |
                                                     unique Consumer
                                                                  |
                                                   ViewportGestureLifecycle
                                                                  |
                                                   Camera/Picking/Gizmo/
                                                   MapGeometry/Region/Road/Marker
```

Composition Root owns exactly one router, one coordinator, and the consumer set. `VulkanNativeHost` owns source attachment and platform forwarding only. `UiWin` owns window event translation only. VM methods remain execution cores behind consumers.

## 3. Contracts

### 3.1 Source boundary

- Native adapter consumes `NativePointerMessage`, converts physical pixels to logical/DIP exactly once, maps `CaptureChanged` to `CaptureLost` and `KillFocus` to `FocusLost`.
- Avalonia adapter consumes `PointerEventArgs` and `PointerWheelEventArgs`, uses viewport-relative coordinates, pointer id, buttons, modifiers, and wheel delta.
- Both adapters output the existing `EditorPointerEvent`; do not add timestamp/device/horizontal delta during E.
- Source identity must be explicit: e.g. `ViewportPointerSource("native-hwnd")` and `ViewportPointerSource("avalonia")`.

### 3.2 Arbitration

Pressed candidates are evaluated by `CanBegin` and explicit `BeginPriority`; once a consumer claims a gesture, later move/release events are delivered only to that owner and pointer id. Idle Move/Wheel may be observed or consumed; idle cancellation events are ignored. No Host-side route policy may select a consumer.

Required priority semantics preserve current behavior:

1. Navigation Gizmo hit / transform Gizmo hit where applicable.
2. Map Geometry vertex/drag.
3. Region/Road/Marker authoring according to active tool.
4. Picking fallback.
5. Camera navigation for middle/gesture input; wheel camera consumption when no gesture is active.

The exact numeric priorities belong in the consumer set, not in Native/Avalonia Host partials.

### 3.3 Lifecycle and capture

`ViewportGestureLifecycle` is the only business lifecycle. On begin it calls `IViewportPointerCaptureCoordinator.Capture(pointerId, owner)` when the result is `Captured`; on commit/cancel it calls exactly one `Release`. The coordinator may call Win32 `SetCapture/ReleaseCapture` or Avalonia `Pointer.Capture`, but must expose one logical owner and suppress expected release callbacks.

### 3.4 Cancellation

All cancellation sources dispatch one event to the router:

| Source | Event | Lifecycle reason |
|---|---|---|
| Native `WM_CAPTURECHANGED` / Avalonia `PointerCaptureLost` | `CaptureLost` | `CaptureLost` |
| Native `WM_KILLFOCUS` | `FocusLost` | `FocusLost` |
| `UiWin.Deactivated` | `WindowDeactivated` | `WindowDeactivated` |
| viewport Escape | `Escape` | `Escape` |
| active tool transition | `ToolChanged` | `ToolChanged` |
| editor mode transition | `ModeChanged` | `ModeChanged` |
| NativeHost detach/destroy | `ViewportDisposed` | `ViewportDisposed` |
| `WM_CANCELMODE` or explicit cancel | `Cancel` | `ExplicitCancel` |

Non-viewport Escape targets such as Inspector text editing and dialogs remain in their controls; only the viewport shortcut bridge dispatches the viewport Escape event.

## 4. Diagnostic boundary

`DiagnosticNativePointerProbe`, `DiagnosticOverlayHost*`, `DiagnosticPlacement*`, and related diagnostic bridge code may observe a message or snapshot after source capture. They must not:

- be a Router consumer;
- call `ViewportInputRouter.Dispatch`;
- call a VM input method;
- acquire/release platform or business capture;
- determine arbitration order.

## 5. Required production wiring changes

The implementation plan is authoritative for sequencing. The expected file families are:

- Composition Root / `UiVm` construction: create router, consumers, coordinator, and expose a narrow input sink.
- `VulkanNativeHost.Pointer.cs`, `AvaloniaPointer.cs`, `CameraPointer.cs`, `Gizmo.cs`, `Picking.cs`, `RegionSnapInput.cs`: replace direct dispatch with adapter + router forwarding. These are E files; they are not modified by E0.
- `Win32ViewportHost.Input.cs` and `NativePointerMessage.cs`: preserve native source semantics; only add fields if a tested E contract requires them.
- `UiWin` input/lifecycle partials and `UiVm.Tool.cs`/`Mode.cs`: translate cancellation transitions to router events, retaining non-viewport control behavior.
- Consumers/Map adapters: connect existing VM domain operations behind `IViewportInputConsumer`; do not put platform event logic into VM.

## 6. Acceptance invariants

1. A production pointer event has exactly one source adapter and one Router dispatch.
2. No production Host file directly calls a viewport pointer VM method after E6.
3. No production Window file directly calls viewport cancel VM methods after E5.
4. Active Router state has one `GestureOwner`, one pointer id, and one lifecycle context.
5. Native and Avalonia platform capture can differ, but business lifecycle owner cannot.
6. Capture lost, focus lost, deactivation, Escape, tool/mode change, and disposal each cancel at most once.
7. Diagnostic observers do not affect dispatch result, consumer selection, lifecycle state, or capture.
8. The existing logical/DIP and normalized wheel contracts remain unchanged during E.

## 7. Verification contract

- Unit: Native/Avalonia adapter mapping, source identity, wheel and cancellation mapping.
- Router: arbitration, owner stickiness, mismatched pointer id, commit/cancel and coordinator release.
- Integration: Native and Avalonia source events converge to equivalent consumer/lifecycle results.
- Static: no forbidden direct Host/Window -> viewport VM calls; no Diagnostic -> Router path; one production Router composition root.
- Runtime: run.bat Native, Avalonia, wheel, capture loss, focus loss, deactivation, Escape, tool/mode changes, and disposal.

