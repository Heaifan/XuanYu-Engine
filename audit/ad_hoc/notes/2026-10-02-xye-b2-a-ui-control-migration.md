# XYE B2-A UI Control Migration

- Scope: remove the seven Avalonia control references from `EditorShellContext` while preserving existing UI composition behavior.
- Boundary: reuse `EditorShellControlRefs`; do not introduce a UI service, manager, or new context. `EditorShellComposition` remains the composition root, `EditorShellEventWiring` receives the existing control-reference record, and `EditorShellCompositionRuntime` receives it explicitly for UI-dependent callbacks.
- Moved controls: `Inspector`, `DebugDock`, `StatusBar`, `ViewportPlaceholder`, `VulkanViewportHost`, `DockPanel`, and `ToolPalette`; Context now has zero direct declarations or `ctx.*` consumers for these controls.
- Preserved behavior: event subscriptions, route constructor arguments, feedback attachment, transform display, viewport host probing, and AXAML files remain behaviorally unchanged; only the source of the same control references changed.
- Verification: solution build passed with 0 errors and 4 pre-existing warnings; Architecture tests passed 17/17; `git diff --check` passed.
- General lesson: an existing control-reference record is a sufficient composition boundary when the migration changes ownership only; pass the same boundary explicitly to wiring/runtime helpers instead of storing controls in a broad shell context.
