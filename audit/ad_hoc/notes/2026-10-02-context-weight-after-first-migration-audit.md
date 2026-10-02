# XYE-SRP-R1-B1-C-CONTEXT-WEIGHT-AUDIT

- Read-only result at active HEAD a29c47f2: the first migration was not applied in this checkout; worktree is clean and EditorShellContext is unchanged from the prior baseline.
- BEFORE/AFTER are therefore identical: 7 UI control references, 3 business-object references, 42 route-like/application references (including render store/lifecycle and secondary route fields), 3 transform appliers, 3 selection presenters, and 6 mutable state holders; public stored-member lines count is 62 under the simple source scan.
- EditorShellContext still acts as a composition context/registry with ApplyRouteSet; it is not itself a NewApplicationService. Behavior remains in EditorShellComposition, EditorShellCompositionRuntime, routes, presenters, and lifecycle objects.
- Dependency direction remains UI controls/business state/route objects -> Context references, while Composition builds and wires them. However CompositionRuntime performs orchestration through Context callbacks and mutable fields; this is an architecture weight observation, not evidence that Context became a service.
- No code changes were made. A real AFTER delta requires an applied migration commit or changed working tree, neither was present.
