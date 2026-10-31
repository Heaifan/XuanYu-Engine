# XYE-EDITOR-UIVM-R5 Implementation Plan

## Scope and invariants

Three ordered ownership extractions: Selection, Camera, Transform/Gizmo. Preserve `EditorStateOwner` entity selection, `MapEditSession` map selection, `ProjectTreePresenter` tree projection, camera math/terrain constraints, and existing transform/history transactions. Keep current notification thread semantics. Never change AXAML contracts, weaken assertions, touch root untracked path `5`, or expand into Scene/MapManifest/History redesign.

## Batch A — Selection

Inspect only Selection/Picking partials and direct callers/tests. Add a behavioral contract test for selection projection/clear and selection-triggered interaction cancellation. Extract coordination into an independently-owned Selection coordinator using existing canonical owners and `ProjectTreePresenter`; retain only AXAML-facing bindings in UiVm. Preserve viewport pick sequence and validity behavior unless evidence supports safe ownership transfer. Run Editor.UI Release build and focused selection/picking/tree/inspector tests; commit only this batch after pass.

## Batch B — Camera

Inspect camera/navigation/framing partials, renderer snapshot publication, and established camera tests. Add/extend behavior tests before migration for ObservationCenter Dolly, 0.25m floor, terrain clearance/recovery, Pan, Orbit, cancellation, framing, resize/DPI. Extract navigation state and operations to a CameraController with explicit environment/publish callbacks; preserve `CameraState` math and `EditorCameraFraming`, keep UiVm binding façade. Run Editor.UI Release build and focused camera/terrain tests; commit after pass.

## Batch C — Transform/Gizmo

Inspect move/rotate/scale partials and pointer lifecycle dispatch. Add/extend tests for preview/cancel/no-history, single commit, CaptureLost/FocusLost/PointerUp cleanup. Extract only safely-separable interaction session ownership into a TransformInteractionController, continuing to use existing `EditorStateOwner`, `SceneStateOwner`, and History commit boundary. Leave cross-domain transactions in UiVm and report PARTIAL where needed. Build and focused transform/gizmo/lifecycle tests; commit after pass.

## Integrated closeout

Run one final Editor.UI Release build, deduplicated focused suites, and Editor startup smoke. Report UiVm declaration counts before/after, actual state/behavior ownership, per-batch commits, remaining coupling, binding and runtime evidence, Git state (preserve `5`), Changelog receipt, and unverified notification-thread risks. Do not claim full-suite or visual acceptance without evidence.
