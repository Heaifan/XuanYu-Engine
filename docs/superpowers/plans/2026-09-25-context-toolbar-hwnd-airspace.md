# XYUI Context Toolbar HWND Airspace Fix Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Make Context Toolbar root and child menus render in one real Native Popup so they can cover the Vulkan child HWND.

**Architecture:** Keep `XYContextDropdownBoard` as the menu model and interaction owner. Add a focused popup host that owns one `Popup` and one surface canvas; all root and child surfaces remain ordinary Avalonia visuals inside that popup. Placement is screen/owner-work-area based, never viewport-obstacle based.

**Tech Stack:** C#, Avalonia Popup/TopLevel, xUnit, .NET solution.

**Spec:** User-approved task brief `XYUI-CONTEXTTOOLBAR-FIX-HWND-AIRSPACE` in the conversation.

## Global Constraints

- Do not modify Input, Vulkan renderer, NativeHost resize, Diagnostic floating card, Inspector, Project Tree, Log Panel, Map/Region/Road/Marker business logic, Snap, Schema, or public API.
- One Native Popup Host only; no per-submenu Popup windows.
- `ShouldUseOverlayLayer = false`; production menus must not be added to `XYContextOverlayHost`.
- Root/child width 128 DIP, item/header height 32 DIP, gap 4 DIP.
- Every hand-written `.cs`/`.axaml`/`.js` file remains at or below 100 lines.

## Review Focus

- Popup child must be the real menu surface, not a compatibility 1×1 child; test: runtime production-host contract.
- Root and child must remain in one popup while sibling menus switch; test: board host and cascade tests.
- Anchor movement, resize, scroll, and screen edges must re-place without viewport avoidance; test: geometry/runtime placement tests.
- Escape, outside click, action execution, and Diagnostic surface discovery must remain intact; test: existing UI/runtime suites.
- DPI conversion must not use Window-local overlay coordinates; test: screen-placement helper contract and manual 175% run.

### Task 1: Freeze the production popup contract

**Files:**
- Modify: `XuanYu.World.Tests/UiRuntime/ContextToolbarPopupHostRuntimeTests.cs`
- Modify: `XYUI/avalonia/tests/XYUI.Avalonia.Tests/XYUI3ContextToolbarBoardTests.cs`

- [ ] Add assertions that opening a production board makes `Popup.IsOpen` true, `Popup.Child` contain `RootMenuSurface`, and `OverlayHost` remain null.
- [ ] Run the focused tests and record the expected failure against the current compatibility shell.

### Task 2: Add the single native popup host

**Files:**
- Create: `XYUI/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-17-ContextToolbar/XYContextPopupHost.cs`
- Modify: `XYContextDropdownBoard.cs`

- [ ] Implement host lifecycle around one `Popup` and one `Canvas`; expose only board-needed placement/lifecycle operations.
- [ ] Set `ShouldUseOverlayLayer = false` and use the real surface canvas as `Popup.Child`.
- [ ] Move root/child placement to screen-aware popup coordinates with work-area clamping and left/up fallback.
- [ ] Rewire open/close, owner move/resize, anchor layout/scroll, keyboard, outside dismissal, and child cascade without changing menu data or actions.
- [ ] Remove production calls to `XYContextOverlayHost.Attach/AddOverlay`; preserve compatibility properties only if existing tests/API require them, with no production ownership.
- [ ] Run focused XYUI and World runtime tests until green.

### Task 3: Regression and gate verification

**Files:**
- Modify only tests if a directly evidenced contract gap remains.

- [ ] Run Context Toolbar tests, Diagnostic probe, E1–E5 input regression, Core.Tests, World.Tests, and the affected build serially.
- [ ] Run architecture gates, 5+100, `git diff --check`, and scope review.
- [ ] Run `run.bat` and record real UI status as passed or pending manual evidence; do not infer visual coverage from headless tests.
- [ ] Commit implementation and tests as two atomic commits if the validation boundary supports it, then push only after validation and verify remote tip.
