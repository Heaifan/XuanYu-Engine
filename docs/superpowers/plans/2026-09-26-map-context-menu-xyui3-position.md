# Map Context Menu XYUI3 Position Fix Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Make the map right-click menu use the XYUI3-3 context-menu presentation and open at the actual right-click position.

**Architecture:** Keep the editor's existing `XYContextMenu` composition, but make its popup coordinate contract explicit: the editor supplies native viewport client DIP coordinates, the host converts them once to the Avalonia top-level space, and `XYContextMenu` anchors its popup to that point. The menu body remains an `XYMenu` populated with `XYMenuItem` controls.

**Tech Stack:** C#, Avalonia 12, XYUI.Avalonia, .NET 10, xUnit.

**Spec:** User request in the current task: use XYUI3-3 context menu and show it immediately beside the right-click point.

## Global Constraints

- Preserve unrelated working-tree changes.
- Do not modify Inspector, naming, or unrelated map-editing behavior.
- Keep hand-written `.cs` files at or below 100 lines.
- Do not claim the Windows popup is fixed without a fresh focused test and build result.

## Review Focus

- Native viewport client DIP point is converted exactly once before popup placement; regression test owns the coordinate contract.
- Popup uses the XYUI3-3 context-menu root, not a native Avalonia `ContextMenu`; component construction test owns this identity.
- Right-click menu rows remain `XYMenuItem` controls and preserve existing command callbacks; existing menu tests own this behavior.
- Popup placement flips/slides near screen edges without losing the click anchor; placement configuration test owns this behavior.
- Existing unrelated dirty input files remain untouched; git status inspection owns this check.

---

### Task 1: Pin the XYUI3 context-menu and coordinate contract

**Files:**
- Modify: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI3InteractionTests.cs`
- Read: `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.ContextMenu.cs`
- Read: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-03-ContextMenu/XYContextMenu.Interaction.cs`

- [ ] Add assertions that an opened context menu is `XYContextMenu`, contains an `XYMenu`, and opens with custom placement anchored at the supplied point.
- [ ] Run the focused test and confirm it fails for the missing XYUI3/position contract rather than because of a test setup error.

### Task 2: Implement the minimal XYUI3-3 placement fix

**Files:**
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-03-ContextMenu/Interaction/XYContextMenu.Interaction.cs`
- Modify: `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.ContextMenu.cs` only if the failing contract proves the host is converting the point more than once.

- [ ] Keep the root as `XYContextMenu`, use its `Menu` property with `XYMenu`, and ensure the popup target remains the actual visual host.
- [ ] Use one explicit top-level-to-target conversion and a custom popup anchor with slide/flip constraints.
- [ ] Run the focused test and confirm it passes.

### Task 3: Verify the scoped change

**Files:**
- Read: focused test output and git diff/status.

- [ ] Run the XYUI3 focused test project.
- [ ] Build the affected editor project.
- [ ] Run `git diff --check` and the 5+100 line check for changed hand-written files.
- [ ] Confirm only the scoped menu/position files changed in addition to the pre-existing dirty files.

