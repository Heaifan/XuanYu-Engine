# XYC-API/1 control contract

| Command | Read/write | Contract |
|---|---|---|
| `ui.control.list` | Read | Enumerates actual TOP Visual descendants; ID source priority is `XYDiagnostic.DebugId`, Automation ID, then Avalonia Name. Missing IDs are reported as unlocatable, never synthesized. Duplicate IDs are marked. |
| `ui.control.inspect` | Read | Resolves one unique ID and returns type, visibility, enabled state and region. |
| `ui.control.invoke` | Write | Whitelist is only `XYE.TOP.VIEW.ALL` + `invoke`; requires current session, unique target, TOP membership, visible/enabled state and Avalonia `IInvokeProvider`. Captures before/after automatically. |
| `ui.state.inspect` | Read | Reads actual active tool, edit flags, footer state and camera revision/position. |
| `ui.waitUntil` | Read/wait | Asynchronous dispatcher sampling with 1–30000 ms timeout and cancellation; no blocking UI-thread sleep. |
| `ui.capture` | Read | P0 capture reused. P1 adds session-generated non-overwriting filenames. Optional temporary TOPMOST uses `SWP_NOACTIVATE` and restores prior z-order. |

Errors are structured; invocation is reported separately from visibility/enabled checks and verified effects. SAVE and other destructive operations are not in the allowlist.
