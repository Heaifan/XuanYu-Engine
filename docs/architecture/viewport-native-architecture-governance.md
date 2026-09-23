# Viewport Native Architecture and Governance

状态：R1 冻结（2026-09-23）

本文件是当前 Engine 视口原生边界的权威审计记录。它冻结未来目标，不执行生产视口迁移。

## 冻结决策

```text
Avalonia = 唯一 Editor UI / Window 所有者
Viewport = Avalonia 所有的 Control
Vulkan = 仅渲染后端
Native Host = 平台实现细节
```

因此，Editor 的工具栏、Tooltip、HUD、Context Toolbar、选择覆盖层、Diagnostic、菜单和确认框都必须由 Avalonia 负责。Native platform 代码可以创建 Vulkan surface 所需的子窗口、读取平台句柄、管理 Present/Swapchain 和输入桥接，但不能拥有 Editor UI 行为、业务布局或独立的 Editor 窗口产品线。

## 当前组合与所有权

`XuanYu.Editor.App/Program.cs` 创建 Avalonia `AppBuilder`，`EditorCompositionRoot` 只装配 `INativeHostSurfaceBridgeFactory`，随后 `XuanYu.Editor.UI/Bootstrap/App.axaml.cs` 创建唯一 `UiWin` 并设置 `desktop.MainWindow`。`UiWin.axaml` 的根 Grid 同时承载 `UiRoot`、XYUI `XYContextOverlayHost`、`DiagnosticOverlayHost` 和主窗口对话框宿主。XYUI 通过 `xmlns:xy="using:XYUI.Avalonia.Controls"` 和资源/样式装配接入同一个 Avalonia 视觉树。

## Native Window Inventory

| File | Type | Created native object | HWND? | Owner | Lifecycle owner | Input responsibility | UI responsibility | Should remain? | Future target ownership |
|---|---|---|---|---|---|---|---|---|---|
| `XuanYu.Editor.UI/Bootstrap/App.axaml.cs` | Avalonia composition root | Avalonia desktop main window | Avalonia creates platform HWND | `IClassicDesktopStyleApplicationLifetime` / `UiWin` | Avalonia application lifetime | Avalonia Window routed keyboard/focus | Sole Editor main window | Yes | Avalonia |
| `XuanYu.Editor.UI/Win/UiWin.axaml(.cs)` | Avalonia `Window` | Main Editor `TopLevel` | Platform HWND, not app-created | `desktop.MainWindow` | Avalonia Window lifetime | Avalonia routed input, focus, deactivation | Root Editor UI, dialogs, XYUI overlays | Yes | Avalonia |
| `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml(.cs)` | Avalonia `UserControl` | No independent native object | No direct HWND creation | `UiRoot` / `UiWin` visual tree | Avalonia visual attachment | Hosts the Viewport control path | Viewport boundary and fallback only | Yes | Avalonia-owned Control |
| `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs` | Avalonia `NativeControlHost` | Delegates one child surface HWND to `Win32ViewportHost` | Yes, child HWND | Avalonia `NativeControlHost` | `CreateNativeControlCore` / `DestroyNativeControlCore`, attach/detach | Receives native pointer messages and forwards them to existing route | No toolbar, tooltip, HUD, or business window | Yes | Avalonia owns lifecycle; native child remains backend detail |
| `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs` | Explicit Win32 adapter | `CreateWindowExW` child class `XuanYuVulkanViewport` | Yes, `WS_CHILD` | Parent handle supplied by Avalonia NativeControlHost | VulkanNativeHost destruction path | WndProc forwards pointer/capture messages to registered sink | None | Yes, allowlisted | Platform adapter only |
| `XuanYu.Render.Abstractions/NativeHostSurfaceHandle.cs` | Render handoff value | No object creation | Carries `Hwnd`/`Hinstance` only | NativeHost boundary | Value lifetime | None | None | Yes | Render-neutral handoff |
| `XuanYu.Render.Vulkan/VulkanSurfaceOwner.cs` | Vulkan backend owner | `VkSurfaceKHR` from supplied Win32 handle | Consumes HWND; does not create one | Vulkan bridge | Attach/Detach | None | None | Yes | Vulkan surface/present implementation detail |
| `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.cs` | Vulkan backend bridge | Instance/device/surface/swapchain/session | No Editor HWND creation | Composition root factory | NativeHost Attach/Resize/Detach | None | None | Yes | Vulkan backend only |
| `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.Sync.cs` | Avalonia diagnostic projection | Avalonia `Popup` per diagnostic target | Avalonia may create `PopupRoot` | DiagnosticOverlayHost / `PopupOwner` | `IsOpen`, host Loaded/Unloaded | Avalonia PopupRoot routed input | Diagnostic badge/bounds projection | Yes | Avalonia Popup owned by Editor Window |
| `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.NativeOverlay.cs` | Avalonia diagnostic projection | Two Avalonia `Popup` objects for probe card/highlight | PopupRoot may have a platform handle | DiagnosticOverlayHost | `IsOpen`, host lifecycle | Avalonia Popup input pass-through | Diagnostic-only overlay; no business UI | Yes, as Avalonia path | Avalonia Popup/Overlay; no app-created native window |
| `XuanYu.Editor.UI/Diagnostic/DiagnosticNativeWindowProbe.cs` | Read-only diagnostic probe | None | Reads PopupRoot/owner HWND facts | Diagnostic instrumentation | Probe capture call | None | No UI ownership | Yes, probe only | Read-only platform observation |
| `XuanYu.Editor.UI/Win/LayerDeleteConfirmationWindow.axaml(.cs)` | Avalonia owner-modal window | Avalonia `Window` | Platform HWND | Explicit `UiWin` owner via `ShowDialog` | Avalonia dialog lifetime | Avalonia dialog keyboard/buttons | Editor confirmation UI | Yes | Avalonia-owned modal UI |
| `XuanYu.Editor.UI/Win/UnsavedChangesConfirmationWindow.axaml(.cs)` | Avalonia owner-modal window | Avalonia `Window` | Platform HWND | Explicit `UiWin` owner via `ShowDialog` | Avalonia dialog lifetime | Avalonia dialog keyboard/buttons | Editor confirmation UI | Yes | Avalonia-owned modal UI |
| `XuanYu.Editor.Win/MainForm.cs` | Legacy WinForms skeleton | `Form` and child WinForms controls | Yes | No current `Editor.App` composition path | WinForms Form lifetime if separately launched | WinForms | Legacy alternate Editor UI path | No as active product path; not deleted in R1 | Retire/exclude; Avalonia remains sole Editor UI |

### Answers frozen by this inventory

- HWNDs are created by the Avalonia desktop platform for Avalonia `Window`/`PopupRoot`, and by the explicit `Win32ViewportHost` adapter for the one Vulkan child surface. The application does not create a second Editor main window.
- The main HWND is owned by Avalonia desktop lifetime; the viewport child HWND is owned by `VulkanNativeHost` through `NativeControlHost`; PopupRoot handles are owned by Avalonia Popup infrastructure; Vulkan handles are owned by the Vulkan bridge.
- Win32 input enters the viewport child WndProc only for the Vulkan child path. Avalonia owns all normal Editor input, PopupRoot input, keyboard, focus, and dialog input.
- PopupRoot is created by Avalonia when an Avalonia `Popup` opens. `DiagnosticOverlayHost` creates Popup objects, not Win32 windows.
- Viewport lifecycle is owned by Avalonia visual attachment plus `NativeControlHost` create/destroy callbacks. UI lifecycle is owned by Avalonia application/window/visual lifetimes.
- Bypass paths are the direct Win32 viewport adapter and the disconnected WinForms `XuanYu.Editor.Win/MainForm.cs`. The former is legitimate rendering/platform interop; the latter is a documented legacy violation and must not become an active entry point.
- Legitimate native objects are the viewport child HWND, Win32 capture state, Vulkan `VkSurfaceKHR`, swapchain/device/session, and read-only platform probes. Native objects leaked into Editor UI responsibility would be any toolbar, tooltip, HUD, diagnostic business window, or renderer-created `Window`/`Popup`.

## Input Ownership

| Input type | Entry point | Receiving object | Conversion layer | Business consumers | Future target |
|---|---|---|---|---|---|
| Pointer Move | `Win32ViewportHost.RouteWndProc` `WM_MOUSEMOVE`; Avalonia `OnPointerMoved` for non-native path | Viewport child WndProc or `VulkanNativeHost` | `NativePointerMessage` → DPI logical coordinates → `NativePointerRoutePolicy` | Camera preview, Region/Road/Marker preview, map geometry drag, Gizmo hover/drag, selection picking | Avalonia owns the control; native WndProc remains a backend adapter until input migration is separately approved |
| Pointer Down | `WM_LBUTTONDOWN` / `WM_MBUTTONDOWN`; Avalonia `OnPointerPressed` | Win32 child or `VulkanNativeHost` | Native message fields / Avalonia `PointerPoint` → route arbitration | Navigation Gizmo, map vertex/geometry, Region/Road/Marker, transform Gizmo, picking, camera | One gesture, one owner; keep current route unchanged |
| Pointer Up | `WM_LBUTTONUP` / `WM_MBUTTONUP`; Avalonia `OnPointerReleased` | Win32 child or `VulkanNativeHost` | `NativePointerRoutePolicy` / pointer release path | Commit pointer/Gizmo/map geometry or end camera | Commit then release capture through owner |
| Wheel | `WM_MOUSEWHEEL` | Win32 child WndProc | `Buttons >> 16` → `WheelDelta` | `UiVm.DollyCamera` | Native adapter forwards; camera remains domain owner |
| Keyboard | `UiWin`/Avalonia routed `KeyDown`; viewport `OnKeyDown`/`OnKeyUp` for Alt snap | Avalonia Window/Viewport control | Avalonia `KeyEventArgs` and `KeyModifiers` | Editor shortcuts, Alt snap suppression, cancel/focus behavior | Avalonia owns keyboard/focus |
| Pointer Capture | `SetCapture` on native left/middle down; `Pointer.Capture` on Avalonia path | Win32 child or Avalonia Viewport | `GetCapture`, `ReleaseCapture`, capture-lost/cancel messages | Single active camera/Gizmo/map geometry gesture | Central owner with complete release lifecycle |
| Focus | `WM_KILLFOCUS`, `WM_CANCELMODE`, Window deactivation, Avalonia capture-lost | Native host / UiWin / Avalonia pointer | Cancel route and lifecycle callbacks | Cancel camera, Gizmo, map geometry, native pointer transaction | Avalonia Window lifecycle cancels UI sessions; native adapter only reports platform loss |

The current business route is `VulkanNativeHost.OnNativePointerMessage` → `NativePointerRoutePolicy` → `UiVm` camera/Gizmo/map geometry/Region/Road/Marker/picking consumers. This document does not alter that route. `K-INP-001` and `K-INP-002` remain applicable: one gesture has one owner and Win32 capture must be released from the complete lifecycle.

## Guarded boundaries

The architecture guard (`scripts/arch-a-guard-viewport.ps1`) enforces these structural rules:

1. Only `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs` may contain the viewport child-window creation primitives.
2. `XuanYu.Editor.UI/Viewport` may not create `Window`, `Popup`, tooltip, or dialog UI. `VulkanNativeHost` remains a host control, not a UI owner.
3. `XuanYu.Render.Abstractions` and `XuanYu.Render.Vulkan` may not reference Avalonia, WinForms, Popup, Window, NativeControlHost, or dialog APIs.
4. `XuanYu.Editor.UI/Diagnostic` may inspect native handles only in the explicit read-only probe files; it may not create windows, register classes, capture input, or own business HWND logic.
5. The legacy `XuanYu.Editor.Win` WinForms skeleton is allowlisted only as a known disconnected path; it may not become an additional app entry point or spread into renderer code.
6. Native surface/capture interop is allowed only when its consumer is the existing viewport/Vulkan handoff; this is not a blanket `HWND` string ban.

## Current violations and integration readiness

`XuanYu.Editor.Win/MainForm.cs` is the only located alternate Editor UI path. It uses WinForms and is present as a solution project, but `XuanYu.Editor.App` composes Avalonia and no active reference to `XuanYu.Editor.Win` was found in the current composition chain. It is therefore a known legacy boundary violation, not a reason to alter production behavior in this audit. It must be retired or explicitly excluded before it can be considered part of a single-UI closeout.

The viewport child HWND and the Vulkan surface bridge are not violations: they are the minimum platform/rendering interop required to present Vulkan. The Diagnostic `Popup` path is not an independent native UI owner: it is Avalonia Popup infrastructure with a read-only native probe. XYUI overlays and menus remain Avalonia projections in the same Editor Window.

A1/B1/C1 may integrate only if their UI remains in Avalonia controls/Popup/owned Avalonia Windows, their renderer code consumes render-neutral contracts, and they do not add HWND creation or UI framework dependencies to `XuanYu.Render.*`. This R1 is not a declaration that the legacy WinForms path has been removed, nor that real-device visual acceptance is complete.

## False-positive and false-negative considerations

- The guard uses namespace/project/path boundaries and explicit API patterns, not a blanket prohibition on `HWND`, `nint`, or Win32. This permits surface handles and capture facts where the architecture needs them.
- A new native API with a different spelling could evade a string-based rule; new native adapter code must therefore be added only in the allowlisted platform adapter and reviewed against this document.
- Avalonia may internally implement PopupRoot with a native handle. That platform fact is allowed; application code must not treat the handle as an Editor UI owner.
- The legacy WinForms file is intentionally allowlisted as a documented exception. The guard prevents it from becoming an app entry point but does not delete or migrate it.
- Static guards cannot prove runtime Z-order, airspace, focus, or real-device visibility. `K-NATIVE-001` and real-device IPO remain required for any visual acceptance involving a native surface or PopupRoot.

## Scope and acceptance boundary

Completed in this task: source-backed inventory, ownership/input tables, frozen architecture decision, and structural guard. Not done: production viewport migration, Camera/Picking/Gizmo/Region/Road/Marker/Snap changes, renderer visual changes, deletion of legacy WinForms code, and real-device UI acceptance.
