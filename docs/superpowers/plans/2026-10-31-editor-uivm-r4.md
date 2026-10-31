# XYE-EDITOR-UIVM-R4 Implementation Plan

> **For agentic workers:** Execute the three approved slices sequentially in this task; no per-slice approval is needed.

**Goal:** Transfer ProjectTree, Layer Presentation, and Inspector Draft state/behavior to independent Owners while preserving the UiVm binding surface.

**Architecture:** Keep domain/model authority in existing owners and services. New UI owners may hold only presentation state and unfinished Inspector input drafts; UiVm remains a compatibility facade only where bindings require it.

**Tech Stack:** C#, Avalonia, xUnit, repository `xye-build.ps1` / `xye-dotnet.ps1` gates.

**Spec:** User-approved XYE-EDITOR-UIVM-R4 in the current task.

## Global Constraints

- Preserve existing Dirty paths, especially untracked root path `5`.
- Preserve AXAML binding names, commands, parameters, behavior, and synchronous notification contract.
- No duplicate mutable authority, MapManifest transaction work, History work, or Scene save transaction work.
- Handwritten `.cs` / `.axaml` / `.js` files ≤100 lines; direct directories ≤5 items; maximum depth three.
- Test source files use `test_` prefix. No push, merge, reset, or clean.

## Review Focus

- Tree expansion/selection and entity authority remain singular: ProjectTree presentation tests.
- Layer selection/projection does not mutate MapEditSession authority: layer selection and notification tests.
- Invalid/incomplete Inspector text remains a draft and never changes the model before commit: draft parse/validation/cancel tests.
- Existing binding and command contracts survive facade forwarding: targeted binding/source contract tests.
- Event subscriptions are detached with the owning view model/presenter lifecycle: lifecycle tests or explicitly documented residual.

## Tasks

### Task 1: ProjectTree

- [ ] Trace cache, displayed selection/expansion, refresh, commands and source authority from the indexed tree members and callers.
- [ ] Add failing `test_` contract coverage, extract presentation state/behavior, leave entity selection and node operations with existing Owners.
- [ ] Run the focused Release build/test gate and verify binding names plus single-owner state.

### Task 2: Layer Presentation

- [ ] Trace the three approved UiVm layer files, data providers, MapEditSession and selection/Inspector/viewport event chain.
- [ ] Add failing `test_` contracts and extract only UI projection/selection display; leave layer data, edit and ordering authority unchanged.
- [ ] Run the focused Release build/test gate and verify notifications and cross-view synchronization.

### Task 3: Inspector Draft

- [ ] Trace InspectorInput parse/commit callers, AXAML TwoWay bindings, validation and cancellation behavior.
- [ ] Add failing `test_` draft contracts; move only unfinished input state, parsing/validation and pre-commit projection; retain commit transaction with existing business Owner.
- [ ] Run the focused Release build/test gate and verify invalid, cancel, preview and commit behavior.

### Final Gate

- [ ] Run final Editor.UI Release build, aggregated targeted tests, and one Editor startup smoke test.
- [ ] Record counts, migrated/remaining ownership, 5+100/SRP result, Git and XYK receipt without overstating visual acceptance.
- [ ] Append the dedicated XYK Changelog receipt; formal XYK files remain Coordinator-owned.
