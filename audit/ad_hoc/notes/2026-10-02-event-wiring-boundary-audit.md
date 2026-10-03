# XYE-SRP-R1-B2-C-EVENT-WIRING-AUDIT

- Read-only audit at HEAD a29c47f2.
- EditorShellEventWiring remains a connection layer: it subscribes control events and forwards to routes; its constructor receives routes/context/log route, not a domain Service or State owner.
- VulkanViewportHostPanel and InspectorPanel route through RawInput/Redraw/Picking/Overlay/Ground/Transform/Scrub routes. StatusBarPanel is not an event source; it is a downstream presentation sink passed to routes, Feedback, or runtime composition.
- ToolPalette is not wired by EventWiring; it is passed through input requests and updated by EditorViewportInputRoute/EditorTransformInputRoute as presentation state. Canonical interaction state is not owned by EventWiring.
- Boundary exceptions: the PickRequested lambda composes Picking -> SelectionSync -> ViewFocus, and project-content selection calls ContentSelectionPresenter then PanelApplyRoute/log. These are small adapters, not Service -> State hidden centers, but remain complexity watchpoints.
- No Control -> EventWiring -> Service -> State hidden center found. EventWiring does not retain mutable state or implement domain operations. CompositionRuntime is the larger orchestration center outside EventWiring; do not conflate it with EventWiring.
- No code changes.
