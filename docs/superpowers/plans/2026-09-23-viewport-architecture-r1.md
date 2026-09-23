# Viewport Native Architecture Audit and Governance Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Document and mechanically guard the frozen Avalonia-owned Editor viewport boundary without migrating production viewport behavior.

**Architecture:** Avalonia owns the Editor window, controls, Popup/PopupRoot, dialogs, diagnostic UI, and Viewport lifecycle. The viewport remains an Avalonia `NativeControlHost`; its Win32 child HWND and Vulkan surface are implementation details of the rendering bridge. A structural PowerShell guard will allow only the existing viewport surface adapter to create a native child and will reject UI-framework leakage into renderer/platform layers.

**Tech Stack:** C#/.NET 10, Avalonia 12.0.4, Win32 P/Invoke, Vulkan/Silk.NET, PowerShell architecture gates, Markdown governance documentation.

**Spec:** User-provided `Viewport Native Architecture Audit and Governance` task specification.

## Global Constraints

- Do not perform production viewport migration or alter Camera, Picking, Gizmo, Region, Road, Marker, Snap, or map-domain behavior.
- Preserve unrelated dirty/untracked material in the canonical checkout.
- Keep all handwritten `.cs` / `.axaml` / `.js` files at or below 100 physical lines.
- Do not add dependencies or upgrade Avalonia/Vulkan packages.
- Do not claim gates that were not executed.

## Review Focus

- A new `Popup` or `Window` under `Viewport/` must fail the guard; covered by the viewport UI creation scan.
- A new Avalonia or WinForms dependency under `Render.*` must fail; covered by the renderer UI-framework scan.
- Diagnostic may inspect native PopupRoot facts but must not create or own independent Win32 UI; covered by the diagnostic native-API and HWND-dependency scan.
- The legitimate `Win32ViewportHost` child HWND path must remain allowed; covered by the explicit adapter allowlist.
- The legacy `XuanYu.Editor.Win/MainForm.cs` must remain documented as disconnected and not become a second active Editor entry point; covered by the explicit legacy boundary assertion.

### Task 1: Source-backed architecture decision

**Files:**
- Create: `docs/architecture/viewport-native-architecture-governance.md`

- [ ] Inventory current HWND/PopupRoot/TopLevel creators, owners, lifecycle, input, UI responsibility, and future ownership.
- [ ] Document the main-window composition chain and XYUI integration points.
- [ ] Document Pointer Move/Down/Up/Wheel, keyboard, capture, and focus routes with source paths.
- [ ] Record current violations, legitimate native interop, false-positive/false-negative boundaries, and A1/B1/C1 integration status.

### Task 2: Structural architecture guard

**Files:**
- Create: `scripts/arch-a-guard-viewport.ps1`
- Modify: `scripts/arch-a-guard.ps1`

- [ ] Add an explicit allowlist for `Win32ViewportHost.cs` native child creation.
- [ ] Reject Popup/Window/tooltip/dialog creation under `XuanYu.Editor.UI/Viewport`.
- [ ] Reject Avalonia/WinForms/UI creation and UI framework references under `XuanYu.Render.*`.
- [ ] Reject diagnostic native-window creation and direct HWND dependencies outside native probe files.
- [ ] Assert the legacy WinForms project remains disconnected from `XuanYu.Editor.App`.
- [ ] Dot-source the new guard from the existing ARCH-A entry point.

### Task 3: Verification and delivery

**Files:**
- No additional production files.

- [ ] Run the new guard directly and through `scripts/arch-a-guard.ps1`.
- [ ] Run relevant architecture/build/tests serially as available.
- [ ] Run `git diff --check`, 5+100, and scope review.
- [ ] Commit the documentation and guard atomically, push the task branch, and verify remote tip/ahead-behind.
