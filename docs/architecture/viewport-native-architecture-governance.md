# Viewport Native Architecture Governance

状态：D1-FIX1 冻结（2026-09-23）

本文件修正 D1 对 NativeControlHost、Win32 child HWND 和 Diagnostic Native Popup 的治理定义。它只更新架构事实与守卫，不执行生产迁移。

## 冻结的终局原则

```text
Avalonia = 唯一 Editor UI / Window Owner
Viewport = Avalonia-owned Control
Vulkan = Renderer Only
Native Host / HWND = 迁移期实现细节，不是终局 UI 架构
```

## CURRENT：临时遗留架构

当前生产组合真实为：

```text
Avalonia MainWindow
└─ UiRoot / Main
   └─ VulkanViewport : Avalonia UserControl
      └─ VulkanNativeHost : NativeControlHost
         └─ Win32 WS_CHILD
            └─ Vulkan Surface / Swapchain / Present
```

该路径必须标记为：

```text
TEMPORARY LEGACY / TRANSITIONAL
```

原因是 A1 Composition Spike 已证明：

```text
NativeControlHost
→ WS_CHILD 子 HWND
→ Airspace
→ Avalonia Overlay 无法可靠覆盖 Vulkan
```

证据：`Commit 2687c917b04cce396c065ee3c85999d8d51dcbf8`。

因此 `VulkanNativeHost`、`Win32ViewportHost` 和 Diagnostic Native Popup 只允许作为迁移债务存在，不是长期合法终局，也不得以 airspace 修补继续扩建。

## TARGET：Avalonia-owned Composited Viewport

目标架构冻结为：

```text
Avalonia MainWindow
└─ XYViewportControl : Avalonia Control
   ├─ Vulkan Composited Image / GPU Surface
   └─ Avalonia Overlay
      ├─ Toolbar / Context Toolbar
      ├─ Tooltip / HUD
      ├─ Selection Overlay / Gizmo UI
      └─ Diagnostic Overlay
```

目标要求：

- Avalonia 继续是唯一 Editor UI / Window Owner。
- `XYViewportControl` 拥有 Viewport UI 生命周期、布局、输入与 Overlay 组合。
- Vulkan 只提供 Renderer、Render Target 或可组合 GPU 结果，不创建 Editor UI。
- 旧 NativeControlHost 路线必须先经过 `A1.5 Vulkan Offscreen + Avalonia GPU Composition Spike`，成功后才能逐项退休旧 allowlist。

## A1 决策记录

```yaml
spike: A1 Composition Spike
commit: 2687c917b04cce396c065ee3c85999d8d51dcbf8
result: FAIL
reason: NativeControlHost + WS_CHILD cannot satisfy reliable Avalonia overlay composition over Vulkan
next: A1.5 Vulkan Offscreen + Avalonia GPU Composition Spike
```

结论：禁止继续以 NativeControlHost airspace 修补作为长期方案。A1 失败不等于当前生产路径已迁移；它只冻结了当前路径的 Transitional 语义和下一条验证路线。

## Native Window Inventory：CURRENT 与 TARGET 分离

| File | CURRENT object / owner | CURRENT status | UI/input responsibility | TARGET ownership |
|---|---|---|---|---|
| `XuanYu.Editor.UI/Bootstrap/App.axaml.cs` | Avalonia desktop `UiWin` / `desktop.MainWindow` | Current Avalonia root | Main Window lifecycle and routed input | Avalonia remains sole owner |
| `XuanYu.Editor.UI/Win/UiWin.axaml(.cs)` | Avalonia `Window` | Current legitimate UI root | Editor UI, dialogs, XYUI overlays | Avalonia remains sole owner |
| `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml(.cs)` | Avalonia `UserControl` containing `VulkanNativeHost` | Current transitional composition boundary | Viewport fallback and host placement | `XYViewportControl` with composited image plus Avalonia Overlay |
| `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs` | Avalonia `NativeControlHost`, creates/owns child through adapter | **TEMPORARY LEGACY / TRANSITIONAL** | Forwards native input; must not own Editor UI | Retire after A1.5; Avalonia Control owns Viewport |
| `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs` | `CreateWindowExW` `WS_CHILD` and WndProc | **TEMPORARY LEGACY ALLOWLIST** | Native pointer/capture bridge only | Delete with child HWND route after A1.5 |
| `XuanYu.Render.Abstractions/NativeHostSurfaceHandle.cs` | Render-neutral HWND/HINSTANCE handoff | Transitional bridge contract | No UI or business input | Replace with offscreen/composited render-target contract if A1.5 succeeds |
| `XuanYu.Render.Vulkan/VulkanSurfaceOwner.cs` | `VkSurfaceKHR` from supplied Win32 handle | Legitimate current renderer interop, tied to transitional host | No Editor UI/input | Retire for this viewport if offscreen composition replaces Win32 surface; retain only for separately approved platform consumers |
| `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.cs` | Vulkan instance/device/surface/swapchain/session | Renderer implementation detail | No Editor UI/input | Renderer-only composition backend |
| `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.NativeOverlay.cs` | Avalonia `Popup` objects whose PopupRoot may be native | **TEMPORARY LEGACY ALLOWLIST** | Migration diagnostic card/highlight only | Delete native Popup bridge; use Avalonia Overlay inside `XYViewportControl` |
| `XuanYu.Editor.UI/Diagnostic/DiagnosticNativeWindowProbe.cs` | Reads PopupRoot/owner HWND facts | **MIGRATION-ONLY INSTRUMENTATION** | Investigates old native path; no ownership | Delete when Native Diagnostic route retires |
| `XuanYu.Editor.UI/Diagnostic/DiagnosticNativePointerProbe.cs` | Reads old native pointer messages | **MIGRATION-ONLY INSTRUMENTATION** | Investigates old input path; no ownership | Delete when native input route retires |
| `XuanYu.Editor.UI/Win/LayerDeleteConfirmationWindow.axaml(.cs)` | Avalonia owner-modal `Window` | Current Avalonia UI | Confirmation input and lifecycle | Avalonia-owned modal UI remains valid |
| `XuanYu.Editor.UI/Win/UnsavedChangesConfirmationWindow.axaml(.cs)` | Avalonia owner-modal `Window` | Current Avalonia UI | Confirmation input and lifecycle | Avalonia-owned modal UI remains valid |
| `XuanYu.Editor.Win/MainForm.cs` | Disconnected WinForms `Form` | Legacy alternate UI violation | No active `Editor.App` ownership | Retire/exclude; never become a second Editor entry point |

`PopupRoot` is an Avalonia platform implementation detail. Its possible native handle does not grant application code ownership of a native Editor window. The Diagnostic Native Popup row above is nevertheless transitional because it exists to work around the current NativeControlHost airspace, not because it is an acceptable target pattern.

## Input Ownership：CURRENT 与 TARGET

| Input | CURRENT entry/receiver | Transitional conversion | CURRENT consumers | TARGET |
|---|---|---|---|---|
| Pointer Move | Win32 child WndProc or Avalonia Viewport pointer event | `NativePointerMessage` / `PointerPoint` → logical coordinates → route policy | Camera, Gizmo, map geometry, Region/Road/Marker preview, picking | `XYViewportControl` receives Avalonia pointer input and dispatches one owner |
| Pointer Down/Up | `WM_LBUTTON*`/`WM_MBUTTON*` or Avalonia pressed/released | Existing route policy and capture lifecycle | Navigation Gizmo, map geometry, drawing tools, transform Gizmo, picking, camera | Avalonia control input with the same one-owner contract |
| Wheel | `WM_MOUSEWHEEL` in child WndProc | `WheelDelta` → `UiVm.DollyCamera` | Camera | Avalonia `PointerWheelChanged` to camera command |
| Keyboard | `UiWin` and `VulkanNativeHost` Avalonia handlers | `KeyEventArgs` / Alt modifier | Shortcuts, snap suppression, cancel/focus | Avalonia Window/Viewport routed keyboard |
| Capture | Win32 `SetCapture`/`GetCapture`/`ReleaseCapture` or Avalonia pointer capture | Native cancel/focus messages | One active camera/Gizmo/map geometry gesture | Avalonia capture owned by `XYViewportControl` |
| Focus | `WM_KILLFOCUS`, `WM_CANCELMODE`, Window deactivation, capture lost | Cancel route | Cancel active interaction | Avalonia lifecycle cancels active session |

The current route is preserved in this documentation-only task. `K-INP-001` and `K-INP-002` remain applicable during migration: one gesture has one owner and native capture must release across all cancellation paths.

## Temporary Legacy Allowlist

The exact migration debt allowlist is:

```text
XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs
XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs
XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.NativeOverlay.cs
```

This is a **temporary legacy allowlist**, not a long-term architecture license. A1.5/WAVE-2 success must remove these entries one by one. No new file may join the list through an implicit pattern or directory-wide exception.

## Guard policy

`scripts/arch-a-guard-viewport.ps1` enforces:

1. Only the exact `VulkanNativeHost.cs` file may be the Viewport `NativeControlHost` implementation.
2. Only the exact `Win32ViewportHost.cs` file may contain viewport child-window creator APIs such as `CreateWindowEx`, `RegisterClass`, or `DestroyWindow`.
3. Only the exact Diagnostic Native Overlay file may contain the transitional `ShouldUseOverlayLayer = false` route.
4. Diagnostic files outside the exact migration probes/allowlist may not create windows, own HWNDs, call Win32 window/capture APIs, or add native owner/z-order UI systems.
5. `XuanYu.Render.Abstractions` and `XuanYu.Render.Vulkan` may use legitimate Vulkan/platform surface interop but may not reference Avalonia, WinForms, `Window`, `Popup`, `NativeControlHost`, or dialog APIs.
6. The disconnected WinForms skeleton cannot be composed by `XuanYu.Editor.App` and cannot become an executable entry point.

The guard distinguishes renderer interop (`VkSurfaceKHR`, Swapchain, Device, Present, render targets, and render-neutral platform handles) from Editor UI ownership. It does not use a blanket `HWND` ban.

## Integration boundary

Allowed for WAVE-1 only as documented transitional debt: current production rendering may continue to use the exact allowlist while A1.5 is pending. New features must not depend on it, extend it, or add UI over it as a long-term solution. A1.5 must prove the target composited route before the legacy paths can be retired.

明确未完成：生产迁移、A1.5 Spike、删除旧 Native 路径、删除 migration-only probes、Camera/Picking/Gizmo/Region/Road/Marker/Snap 行为修改、真机视觉验收。
