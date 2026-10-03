# XYE-SRP-R1-A3-MODE-OWNERSHIP-AUDIT

- Read-only audit. Active HEAD a29c47f2 is post-UiVm migration and has no single global editor Mode state; EditorShellContext is a composition holder, not a canonical Mode owner.
- Historical true owners: EditorModeManager owns Manage/Edit; EditorWorkspaceManager owns Map/Region workspace; EditorStateOwner owns EditorToolSnapshot (active tool, snap, capture reset) and Editor interaction state.
- Historical UiVm remained a writer/orchestrator through ToggleEditorMode, SwitchWorkspace, SelectTool, and context transitions. It also held duplicate/local state: _contextDrawingKind (later wrapped by AuthoringInputSession) and _isTerrainContext. These are authoring/context states, not the global Manage/Edit owner.
- Current local owners: TransformPointerRoute owns TransformInteractionState for Move/G modal activity; Scene3dSessionLifecycle owns Scene3dSessionState; EditorInputService owns runtime input binding snapshot lifecycle; EditorShellContext aggregates route/state references but should not become canonical Mode state.
- Boundary rule: answer Mode ownership by state kind, not by UI surface. UI reads projections; routes coordinate transitions; canonical managers/states own mutation.
