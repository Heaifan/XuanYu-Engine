# XYC-UI-V0.1-P0 Command Contract

Protocol: existing JSON-lines `XYC-API/1`. UI commands are enabled only in the explicit `--xyc-ui` production Editor session. The existing `--xyc-silent` mode remains separate.

| Command | Kind | Arguments | Result / failure behavior |
|---|---|---|---|
| `ui.window.inspect` | Read | none | Process ID, owned HWND, visibility/foreground/state, client size in DIP and physical pixels, DPI and scale. HWND must belong to the child Editor process. |
| `ui.window.resize` | State change | `widthDip`, `heightDip` | Requests a no-activate resize and returns actual client geometry. Invalid/out-of-range dimensions or native resize errors return structured failure. |
| `ui.theme.inspect` | Read | none | Reports actual production `ThemeVariantScope` theme. |
| `ui.theme.set` | State change | `theme`: `Light` or `Dark` | Invokes the existing production theme-toggle mechanism and reads back the resulting actual theme. |
| `ui.region.inspect` | Read | none | Returns production TOP visual bounds in DIP and pixels after layout. |
| `ui.capture` | State change (writes evidence) | `fileName`: one of `Light-1360.png`, `Dark-1360.png`, `Light-1920.png`, `Dark-1920.png` | Captures the real DWM-composed Editor window, crops the TOP bounds, writes PNG and JSON sidecar with SHA-256 and provenance. Errors are returned; no placeholder is created. |
| `session.close` | Lifecycle | none | Existing session shutdown contract; UI runner waits for clean CLI exit. |

All requests use the existing schema version, request ID and session ID envelope. Unsupported commands/arguments and runtime failures use the command processor's structured error response. No arbitrary OS command execution is exposed. The CLI accepts `--xyc-ui` (mutually exclusive with `--xyc-silent`) and optional `--evidence-dir`.

Capture temporarily raises the owned HWND to the top of the non-topmost z-order using `SWP_NOACTIVATE` so DWM desktop pixels can be sampled, then restores z-order. It checks that the foreground HWND did not change. This is a visible window capture path, not a silent/offscreen capture guarantee.
