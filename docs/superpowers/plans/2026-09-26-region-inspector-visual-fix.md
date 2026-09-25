# Region Inspector Visual Fix Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Show the real Region name, use the Region semantic viewport colors, and prevent read-only Inspector values from being clipped in the narrow right panel.

**Architecture:** Keep Region data authoritative in `MapDefinition` and project its `DisplayName` into the Inspector header. Keep rendering in the existing vector-overlay projection, replacing anonymous Region colors with named semantic constants. Let the read-only XYUI text value wrap inside the existing value/copy layout without adding a new interaction surface.

**Tech Stack:** C#/.NET 10, Avalonia, XYUI, xUnit.

**Spec:** Current user request and existing Region/Inspector UI contracts.

## Global Constraints

- Do not modify map schema or unrelated editor behavior.
- Preserve existing dirty input files and do not include them in the commit.
- Keep hand-written `.cs`/`.axaml` files at or below 100 lines.
- Verify focused tests, affected build, full applicable tests, and `git diff --check` before claiming completion.

## Review Focus

- Region header uses the selected model object's `DisplayName`, not the generic tool title.
- Region fill and unselected border use named semantic colors while selected borders keep selection emphasis.
- Long read-only values wrap instead of being clipped beside the copy button and retain the existing copy action.

### Task 1: Regression contracts

**Files:**
- Modify: `XuanYu.World.Tests/UiRuntime/InspectorPropertyMutabilityTests.cs`
- Modify: `XuanYu.World.Tests/UiRuntime/MapVectorOverlayV1Tests.cs`
- Modify: `XuanYu.World.Tests/Workspace/EditorWorkspaceUiCompositionTests.cs`

- [x] Add failing assertions for authoritative feature header names and Region semantic colors.
- [x] Run the focused tests and observe the expected failures.
- [x] Add the read-only value layout source contract.

### Task 2: Visual projection fixes

**Files:**
- Modify: `XuanYu.Editor.UI/Vm/Map/UiVm.MapDataset.Inspector.cs`
- Modify: `XuanYu.Editor.UI/Vm/Map/MapVectorOverlayBuilder.cs`
- Modify: `XuanYu.Editor.UI/Right/InspectorReadOnlyValuePresenter.axaml`

- [x] Bind feature headers to the authoritative feature name and identity.
- [x] Replace anonymous Region overlay colors with named semantic colors.
- [x] Wrap read-only values while preserving the existing copy action.

### Task 3: Verification and delivery

- [ ] Run focused World tests and the XYUI layout/visual tests.
- [ ] Build `XuanYu.Editor.App` with zero warnings/errors.
- [ ] Run applicable full test suites, `git diff --check`, and 5+100 line checks.
- [ ] Commit only scoped files, push, and verify local/remote tip equality.
