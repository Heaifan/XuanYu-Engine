# XYE B1-C2 EditorShellContext Weight Audit

- Baseline: compare HEAD `a29c47f2` (BEFORE) with the live B1 working tree (AFTER), read-only.
- Measured result: public members 65 -> 64; Route-typed members 37 -> 36; Presenter members unchanged at 3; public methods unchanged at 1 (`ApplyRouteSet`).
- The removed member is only `WindowCommandsRoute`. The `EditorShellWindowRoute` window-lifecycle owner remains in Context; `EditorShellComposition` now locally constructs `EditorShellWindowCommandsRoute` and wires existing menu controls.
- Behavior permission shift: Context no longer stores or exposes Window Commands; Composition gains command construction/event-wiring responsibility. Window open/activate/Closed behavior remains in `EditorShellWindowRoute`.
- Hidden composition remains: Context still combines seven UI references, three domain/project values, 36 Route references, three Transform appliers, three Presenters, render/session/input state, and `ApplyRouteSet`; B1 is a narrow weight reduction, not Context closure.
- Audit lesson: count both declared members and retained authority. A one-member reduction can remove a meaningful ownership edge while leaving the larger composition hub unchanged.
