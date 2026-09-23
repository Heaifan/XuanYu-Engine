# DIAG-R1 Native Probes Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add two bounded runtime probes that establish Popup HWND ownership/Z-order facts and prove the Win32 viewport `WM_MOUSEMOVE` path without changing final UI architecture or existing input ownership.

**Architecture:** The Popup probe captures the actual native PopupRoot and owner-window handles after the existing Popup opens, then emits a compact diagnostic line and retains the latest snapshot for inspection. The viewport probe observes only `WM_MOUSEMOVE` at the existing `Win32ViewportHost` sink boundary, emits `XYE.VIEWPORT`, and then invokes the unchanged camera/gizmo/picking route.

**Tech Stack:** C#/.NET 10, Avalonia 12.0.4, Win32 user32 interop, xUnit/Avalonia Headless.

**Spec:** User-supplied `DIAG-R1` audit text in `C:\Users\Heai\.codex\attachments\77cee136-67f8-4212-bba3-d500011c524f\已粘贴的文本.txt`.

## Global Constraints

- Do not upgrade Avalonia or replace the Popup with an Owned Window in this round.
- Do not alter `xyui/`.
- Do not change camera, gizmo, picking, region, road, marker, or map-geometry input ownership.
- Native hover observation is read-only and must run before the existing route without consuming the message.
- Keep every hand-written `.cs` file at or below 100 physical lines.
- Preserve all pre-existing dirty and untracked user material.

## Review Focus

- PopupRoot has no platform handle in headless tests; the probe must report `HWND=0` rather than inventing ownership.
- A persistent Popup may have no owner or may be TopMost; the snapshot must expose both facts.
- `WM_MOUSEMOVE` must be observed while `UiVm` is absent or diagnostic mode is disabled without changing existing routing behavior.
- Native coordinates must remain the existing physical coordinates; no coordinate conversion belongs in the observer.
- The probe must not create a card, alter hit testing, or register another input owner.

### Task 1: Popup HWND ownership/Z-order probe

**Files:**
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticNativeWindowProbe.cs`
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.axaml.cs`
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.NativeOverlay.cs`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticNativeOverlayRuntimeTests.cs`

**Interfaces:**
- Consumes: the existing open `_nativeCardPopup` and the current `Window` top level.
- Produces: `DiagnosticNativeWindowSnapshot` containing Popup/Owner/Parent/Style/ExStyle/TopMost/Visible/WindowRect/Foreground handles and text formatting.

- [ ] Add a failing test that requires the latest Popup probe snapshot to expose all required fields and `HWND=0` safely under headless execution.
- [ ] Run the targeted test and confirm it fails because no snapshot exists.
- [ ] Implement the minimal snapshot and Win32 capture, guarded by `OperatingSystem.IsWindows()`, and emit `[POPUP-OWNER-PROBE]` through `Debug.WriteLine`.
- [ ] Capture the snapshot immediately after the existing card Popup opens; do not change Popup properties or placement.
- [ ] Run the targeted test and the existing Diagnostic native-overlay tests.

### Task 2: Win32 viewport native-hover observer

**Files:**
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticNativePointerProbe.cs`
- Modify: `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticNativePointerProbeTests.cs`

**Interfaces:**
- Consumes: the existing `NativePointerMessage` received by `VulkanNativeHost.OnNativePointerMessage`.
- Produces: a latest `DiagnosticNativePointerSnapshot` with HWND, physical X/Y, message, semantic target `XYE.VIEWPORT`, and `Observed=true`.

- [ ] Add a failing test for `WM_MOUSEMOVE` snapshot formatting and semantic target.
- [ ] Run it and confirm the observer API is absent.
- [ ] Implement a read-only observer that records and emits `[DiagNativeHover]`, then call it before the unchanged route logic.
- [ ] Ensure non-move messages are ignored by the observer and are still handled by the existing route.
- [ ] Run the targeted probe tests and the affected viewport pointer tests.

## Gate

Run targeted tests serially, then `git diff --check`, the 5+100 guard, and the affected `XuanYu.Editor.UI` build. Runtime logs are evidence only; no user visual or cross-application acceptance is claimed.
