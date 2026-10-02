# XYE Window Commands Migration

- Scope: XYE-SRP-R1-B1-A; remove `EditorShellContext` ownership of `EditorShellWindowCommandsRoute` without introducing an ApplicationService or Manager.
- Unique owner: `EditorShellWindowCommandsRoute` owns command-to-window-result translation; `EditorShellWindowRoute` owns actual Preferences/InputBindings/About window lifetime and activation; `EditorShellComposition` owns construction and menu-event wiring.
- Safe migration: remove the Context field and construct the command route as a local in `EditorShellComposition.Build`, wiring the existing `EditorShellControlRefs` menu items to the existing handlers.
- Preserve behavior: keep `EditorShellWindowCommandsRoute` and `EditorShellWindowRoute` unchanged; menu handlers still call the same commands, logging still uses `ctx.LogRoute.Info`, and window activation/Closed cleanup remains in `EditorShellWindowRoute`.
- Verification: solution build passed with 0 errors and 7 pre-existing warnings; Architecture tests passed 17/17; full suite was 638 passed / 1 unrelated existing WorldHierarchy ordering failure; `git diff --check` passed.
- General lesson: for a narrow route migration, move construction and event wiring to the existing composition root, remove only the context field, and leave the route/window lifecycle implementation untouched.
