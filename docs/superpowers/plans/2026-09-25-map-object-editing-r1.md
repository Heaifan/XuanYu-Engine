# MAP-OBJECT-EDITING-R1 Implementation Plan

> **For agentic workers:** Execute this plan inline with `superpowers:executing-plans`; the user explicitly chose direct implementation.

**Goal:** Add XYUI-powered right-click geometry commands and stable object names for Region, Road, and Marker, reusing the existing picking, selection, edit-session, undo/redo, and persistence paths.

**Architecture:** Resolve a screen-space hit into a typed geometry context with deterministic priority `Vertex > Edge/Line > Face`, update selection before opening a menu, expose command availability from existing edit-session capabilities, and route menu actions through existing commands rather than mutating models from UI code. Names are domain data edited through the Inspector and persisted by the existing document serializer.

**Tech Stack:** C#, Avalonia, XYUI `XYContextMenu`/`XYMenuItem`, existing XuanYu Editor input/router, xUnit tests, official SDK via `scripts/resolve-dotnet.ps1`.

**Spec:** `docs/superpowers/specs/2026-09-25-map-object-editing-r1-design.md`

## Global Constraints

- Preserve unrelated dirty R2 files and old audit directories.
- Do not implement colors, labels, label anchors, Boundary Trace, shared topology, or terrain features.
- Do not re-architect the viewport router or schema.
- Every production/test `.cs` and `.axaml` file remains at or below 100 lines; split files when needed.
- Context menus use XYUI, not raw Avalonia menus.
- Every production behavior starts with a failing test and ends with the relevant test plus project suite green.

## Review Focus

- Hit priority and segment endpoint behavior are deterministic and screen-space based.
- Empty-space right-click cannot steal selection or open an object menu.
- Right-click on an unselected object selects it before menu display and Inspector synchronization.
- Disabled vertex deletion is enforced before command execution.
- Menu actions use the existing edit/undo path and do not directly mutate model collections.
- Names survive undo/redo and save/reload without being presentation-only state.
- Native and Avalonia pointer paths converge on the same context-menu request.

---

## Task 1: Geometry context hit contract

**Files:** add focused geometry context result/hit-test files and tests beside existing `MapGeometryHitTester` code.

**Interfaces:** `MapGeometryContextHit`, `MapGeometryContextKind`, and a resolver accepting the current map snapshot plus screen pointer and returning Vertex, Edge/Line, Face, Marker, or Empty. Existing feature adapters remain the source of geometry.

**TDD steps:**

1. Add tests for empty hit, vertex-over-edge priority, nearest deterministic segment, closed Region edges, open Road segments, and Marker hit.
2. Run the focused test project and observe the expected missing-type failures.
3. Implement the smallest resolver using existing projection/picking helpers and screen-space tolerances.
4. Run focused tests, then the affected editor test project.

**Commit:** `feat: add deterministic map geometry context picking`

## Task 2: Domain command capabilities and default names

**Files:** existing `MapEditSession`, geometry command files, model/document naming helpers, and focused tests.

**Interfaces:** command-capability queries for delete vertex/object and add vertex; edit-session methods for add/delete/rename that create normal undoable operations; deterministic next-name allocation per object kind.

**TDD steps:**

1. Add tests for Region/Road vertex-delete legality, object deletion, add-vertex delegation, rename undo/redo, and default names for all three object kinds.
2. Run focused tests and observe failures before implementation.
3. Implement capability checks and command-backed operations, keeping model mutation out of UI.
4. Run focused tests and affected editor/world tests.

**Commit:** `feat: add map object edit commands and stable names`

## Task 3: XYUI context menu and pointer integration

**Files:** existing unified pointer/router bridge, map geometry input/VM files, XYUI menu host integration, and integration tests.

**Interfaces:** a right-click request carrying pointer coordinates/modifiers; selection-first context-menu state; XYUI menu items with enabled state and command callbacks into Task 2 operations.

**TDD steps:**

1. Add tests for empty-space no-menu, selection-before-menu, menu item sets for Region face/edge/vertex, Road line/vertex, Marker, and disabled illegal delete.
2. Run focused tests and observe failures.
3. Wire the existing Avalonia/Native unified pointer paths to one context-menu request; render through XYUI `XYContextMenu`/`XYMenuItem` only.
4. Run focused tests and affected UI/editor tests.

**Commit:** `feat: add xyui map geometry context menus`

## Task 4: Inspector name editing and persistence

**Files:** existing Inspector descriptors/panel, document persistence adapters, and focused tests.

**Interfaces:** selected Region/Road/Marker exposes editable Name property; Inspector commit routes to Task 2 rename command; serializer round-trip retains names.

**TDD steps:**

1. Add tests for Inspector name visibility/edit, undo/redo, save/reload, and restart-load round trip.
2. Run focused tests and observe failures.
3. Implement the narrow Name descriptor/editor using existing XYUI Inspector controls and persistence path.
4. Run focused tests and affected project tests.

**Commit:** `feat: expose map object names in inspector`

## Task 5: Full verification and audit handoff

**Files:** audit package only if production/test files changed, manifest and final verification notes.

**Checks:** focused context/command/name tests, Viewport Input, Map Input, affected build, full solution build, ARCH-A, 5+100, diff-check, World suite with baseline failures recorded, and official `run.bat` launch for human acceptance.

**Steps:**

1. Run all required automated gates serially with official SDK.
2. Create `MAP-OBJECT-EDITING-R1-AUDIT.zip` containing every modified production/test source file and `AUDIT-MANIFEST.md`.
3. Commit, push, verify remote tip, and report exact residual manual acceptance state.

**Commit:** `chore: verify map object editing r1`
