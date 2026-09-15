# XYENGINE-DIAG-V1 Diagnostic Mode Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add stable Editor-owned UI DebugIds, a non-layout diagnostic overlay, bounded snapshots, clipboard interaction, and the frozen initial registrations.

**Architecture:** Editor-level attached properties and a registry define identity. A zero-layout popup host projects interactive badges over registered controls, including the native Vulkan viewport, while a snapshot factory reads only approved Editor/window state.

**Tech Stack:** C# 14, .NET 10, Avalonia 12.0.4, XYUI existing menu controls, xUnit, Avalonia Headless.

**Spec:** `docs/superpowers/specs/2026-09-15-xyengine-diagnostic-mode-v1-design.md`

## Global Constraints

- V1 is only for UI real-device acceptance positioning.
- IDs are manually defined stable semantic IDs beginning with `XYE.`.
- DebugId and EntityId remain separate; visual-tree paths are never formal IDs.
- Diagnostic UI must not change target Bounds, Inspector height, or ScrollViewer extent.
- Only badge surfaces may consume pointer input.
- Missing snapshot fields use `N/A` and never throw.
- Do not modify XYUI public contracts or add DIAG-V2 capabilities.
- Every handwritten `.cs` and `.axaml` file remains at or below 100 lines.
- Run all `dotnet` commands serially and run the full solution build only once.

---

### Task 1: Diagnostic Identity Foundation

**Files:**
- Create: `XuanYu.Editor.UI/Diagnostic/XYDiagnostic.cs`
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticId.cs`
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticRegistry.cs`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticIdentityTests.cs`

**Interfaces:**
- Produces: `XYDiagnostic.DebugIdProperty`, `XYDiagnostic.AreaIdProperty`, `DiagnosticId.Validate(string)`, `DiagnosticRegistry.Register(Control)`, and `DiagnosticRegistry.Clear()`.
- Consumes: Avalonia attached-property and visual-tree APIs only.

- [ ] **Step 1: Write failing identity tests**

```csharp
[Theory]
[InlineData("XYE.INSPECTOR.ROAD.STATE")]
[InlineData("XYE.AREA.RIGHT")]
public void Semantic_ids_are_valid(string value) => DiagnosticId.Validate(value);

[Theory]
[InlineData("Grid1")]
[InlineData("XYE.RIGHT3")]
[InlineData("XYE.inspector.road")]
public void Generated_or_noncanonical_ids_are_rejected(string value) =>
    Assert.Throws<ArgumentException>(() => DiagnosticId.Validate(value));
```

```csharp
var target = new Border();
XYDiagnostic.SetDebugId(target, "XYE.VIEWPORT");
XYDiagnostic.SetAreaId(target, "XYE.AREA.CENTER");
Assert.Equal("XYE.VIEWPORT", XYDiagnostic.GetDebugId(target));
Assert.Equal("XYE.AREA.CENTER", XYDiagnostic.GetAreaId(target));

var registry = new DiagnosticRegistry();
registry.Register(target);
var duplicate = new Border();
XYDiagnostic.SetDebugId(duplicate, "XYE.VIEWPORT");
Assert.Contains("XYE.VIEWPORT",
    Assert.Throws<InvalidOperationException>(() => registry.Register(duplicate)).Message);
```

- [ ] **Step 2: Run the identity filter and verify RED**

Run: `dotnet build XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-restore -m:1 -nr:false -p:BuildInParallel=false -p:UseSharedCompilation=false`

Expected: compilation fails because the diagnostic identity types do not exist.

- [ ] **Step 3: Implement the minimum identity API**

Use an uppercase semantic regex equivalent to:

```csharp
^XYE\.[A-Z]+(?:_[A-Z]+)*(?:\.[A-Z]+(?:_[A-Z]+)*){0,3}$
```

Register nullable string attached properties on `Control`. Validate non-empty values when a registry accepts a target. Store registered controls by ordinal DebugId and throw a clear duplicate error.

- [ ] **Step 4: Run the identity filter and verify GREEN**

Run the targeted build above, then run `dotnet test XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore --filter FullyQualifiedName~DiagnosticIdentityTests` and require zero failures.

### Task 2: Overlay, Snapshot, Clipboard, and Mode Toggle

**Files:**
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticSnapshot.cs`
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticSnapshotFactory.cs`
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticClipboard.cs`
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticBadge.axaml`
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticBadge.axaml.cs`
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.axaml`
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.axaml.cs`
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.Sync.cs`
- Create: `XuanYu.Editor.UI/Top/ViewOptionsMenu.axaml`
- Create: `XuanYu.Editor.UI/Top/ViewOptionsMenu.axaml.cs`
- Create: `XuanYu.Editor.UI/Vm/Diagnostic/UiVm.DiagnosticMode.cs`
- Modify: `XuanYu.Editor.UI/Top/ViewModule.axaml`
- Modify: `XuanYu.Editor.UI/Vm/Scene/UiVm.Scene.cs`
- Modify: `XuanYu.Editor.UI/Win/UiWin.axaml`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticSnapshotTests.cs`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticOverlayRuntimeTests.cs`

**Interfaces:**
- Consumes: Task 1 identity and registry APIs, `UiVm.InspectorIdentity`, `UiVm.SelectedMapGeometry`, `UiVm.DocumentWindowTitle`, `TopLevel.Clipboard`, and Avalonia `Popup` placement.
- Produces: `UiVm.IsDiagnosticMode`, `UiVm.ToggleDiagnosticMode()`, `DiagnosticSnapshotFactory.Capture(Control)`, and a zero-space `DiagnosticOverlayHost`.

- [ ] **Step 1: Write failing snapshot tests**

Create a literal expected string beginning with `[XYengine Diagnostic]`. Assert all V1 field names and line breaks, and assert an empty selection emits `EntityId:` followed by `N/A`.

- [ ] **Step 2: Run the snapshot filter and verify RED**

Run the targeted build command from Task 1, then run `dotnet test XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore --filter FullyQualifiedName~DiagnosticSnapshotTests`.

Expected: compilation fails because snapshot types do not exist.

- [ ] **Step 3: Implement snapshot capture and formatting**

Capture effective visibility/enabled state, bounds translated to the containing `TopLevel`, client window size, render scale, actual theme, version parsed from `DocumentWindowTitle`, inspector selection type, and `SelectedMapGeometry.FeatureId`. Walk visual ancestors for AreaId. Use `N/A` for missing values.

- [ ] **Step 4: Run snapshot tests and verify GREEN**

Run the targeted build, then the same snapshot test with `--no-build --no-restore`, and require zero failures.

- [ ] **Step 5: Write failing overlay invariance and interaction tests**

Show a registered target and `DiagnosticOverlayHost` in `UiRuntimeTestHost`. Record target Bounds and a surrounding `ScrollViewer.Extent`, enable diagnostics, update layout, and assert both are unchanged. Exercise a badge click callback with and without Shift and assert the literal copied payload. Exercise a normal button after diagnostics are enabled and assert its command still runs.

- [ ] **Step 6: Run the overlay filter and verify RED**

Run the targeted build command from Task 1, then run `dotnet test XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore --filter FullyQualifiedName~DiagnosticOverlayRuntimeTests`.

Expected: compilation fails because overlay and badge types do not exist.

- [ ] **Step 7: Implement the minimum overlay and menu path**

The host owns a zero-size panel of `Popup` controls. Each popup is anchored to a visible registered control, is not light-dismissed, does not take focus from a native control, and contains only one `DiagnosticBadge`. The badge tooltip names the DebugId; click copies the ID and Shift+click copies a snapshot. Extract the existing environment items into `ViewOptionsMenu` so `ViewModule.axaml` remains below 100 lines, then expose `视图 → 诊断模式` through the same menu.

- [ ] **Step 8: Run overlay tests and verify GREEN**

Run the targeted build, then the same overlay test with `--no-build --no-restore`, and require zero failures.

### Task 3: Initial Editor Registration and Closeout

**Files:**
- Create: `XuanYu.Editor.UI/Diagnostic/FeatureDiagnosticIds.cs`
- Create: `XuanYu.Editor.UI/Vm/Diagnostic/UiVm.DiagnosticIdentity.cs`
- Modify: `XuanYu.Editor.UI/Root/UiRoot.axaml`
- Modify: `XuanYu.Editor.UI/Top/Top.axaml`
- Modify: `XuanYu.Editor.UI/Right/Right.axaml`
- Modify: `XuanYu.Editor.UI/Right/InspectorPanel.axaml`
- Modify: `XuanYu.Editor.UI/Right/MapEditorPanel.axaml`
- Modify: `XuanYu.Editor.UI/Right/MarkerInspectorPanel.axaml`
- Modify: `XuanYu.Editor.UI/Right/FeatureInspectorPanel.axaml`
- Modify: `XuanYu.Editor.UI/Left/ProjectWorkspace.axaml`
- Modify: `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml`
- Modify: `XuanYu.Editor.UI/Right/EditorLayerDock.axaml`
- Modify: `XuanYu.Editor.UI/Foot/Foot.axaml`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticRegistrationRuntimeTests.cs`
- Modify: `changelog.md`
- Rebuild: `file-tree.md`
- Modify: `run.bat`
- Modify: `XuanYu.Editor.UI/Win/UiWin.axaml`
- Modify: `XuanYu.Editor.UI/Vm/Scene/UiVm.SceneDocument.cs`

**Interfaces:**
- Consumes: Tasks 1 and 2 plus current production Inspector sections.
- Produces: the frozen area/module/inspector IDs and dynamic Road/Region section IDs.

- [ ] **Step 1: Write failing registration tests**

Select a real Road, show `InspectorPanel`, enable diagnostics, and assert the visible registry contains `XYE.INSPECTOR.ROAD.STATE`. Repeat for Region. Assert Map and Marker expose their module IDs. Assert no `OTHER` registration exists because production has no Other section.

- [ ] **Step 2: Run the registration filter and verify RED**

Run the targeted build command from Task 1, then run `dotnet test XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore --filter FullyQualifiedName~DiagnosticRegistrationRuntimeTests`.

Expected: the required registrations are absent.

- [ ] **Step 3: Attach frozen area and semantic IDs**

Attach AreaIds to Top, Left, Center, Right, and Bottom roots. Attach module IDs to the file/menu module, context toolbar, project tree, viewport, inspector, layer dock, and log panel. Attach map and marker inspector IDs. Bind the shared feature inspector and its four existing section containers to resolver-backed Road/Region IDs.

- [ ] **Step 4: Run registration tests and verify GREEN**

Run the targeted build, then the same registration test with `--no-build --no-restore`, and require zero failures.

- [ ] **Step 5: Update version and mandatory docs**

Increment all four version sources together, prepend one truthful changelog entry with executed verification, and rebuild `file-tree.md` from tracked files with one responsibility per file. Do not claim visual acceptance.

- [ ] **Step 6: Run the formal serial gate once**

Run, in order:

```powershell
dotnet build-server shutdown
$env:MSBUILDDISABLENODEREUSE='1'
dotnet build XuanYu.Engine.slnx --no-restore -m:1 -nr:false -p:BuildInParallel=false -p:UseSharedCompilation=false
dotnet test XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore
powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts/arch-a-guard.ps1
git diff --check
dotnet build-server shutdown
```

- [ ] **Step 7: Commit, push, and verify remote equality**

Explicitly stage only DIAG-V1 files, commit them, push the current upstream branch, verify local HEAD equals the remote tip, and verify the worktree is clean.

- [ ] **Step 8: Hand off the Chinese IPO real-device checklist**

Keep the state `READY FOR USER VISUAL ACCEPTANCE`. The checklist covers mode toggle, Road State ID copy, full snapshot copy, Road editing through the overlay, and unchanged Inspector/scroll geometry.
