# XYE-SRP-R1-B0-C-FIRST-MIGRATION-CANDIDATES

- Read-only audit at active HEAD a29c47f2. Lowest-risk first candidate is Window Commands: EditorShellWindowCommandsRoute has three menu handlers plus one direct method, delegates to EditorShellWindowRoute, and has no world/render/session state.
- Selection Presenters are the next bounded candidates because ProjectContentSelectionPresenter and ViewportSelectionPresenter are pure conversion/projection; WorldEntitySelectionPresenter is still bounded but reads WorldState and RenderScene and should follow after the two simpler presenters.
- Feedback is not first batch: EditorFeedbackRoute owns logging, status-bar updates, viewport status, diagnostics, and multiple panel references; it is a high-fanout presentation boundary.
- UI references are not first batch: EditorShellContext stores seven panel/control references and wiring directly consumes them, especially VulkanViewportHostPanel. Moving them first risks event wiring and lifecycle behavior.
- Route references are not first batch: EditorShellContext aggregates many routes and mutable callbacks; usage is broad, with Lifecycle and SelectionRoute especially high fan-out. Treat as a later composition-root cleanup, not a behavior migration.
- First migration list: 1) Window Commands; 2) ProjectContentSelectionPresenter; 3) ViewportSelectionPresenter; 4) WorldEntitySelectionPresenter after the first two pass boundary checks. No code changes were made.
