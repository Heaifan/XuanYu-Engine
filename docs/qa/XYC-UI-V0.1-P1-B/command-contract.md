# Command contract — XYC UI capture

Schema remains `XYC-API/1`; existing command name `ui.capture` is unchanged.

## Request

```json
{"fileName":"Light-1360-before.png","backend":"avaloniaWindow","timeoutMs":5000}
```

`backend` accepts `avaloniaWindow` (default) or `gdiCompatibility`. File names are limited to `Light|Dark` + `1360|1920` + optional 1–32 character safe suffix + `.png`; directory separators and arbitrary paths are rejected. An existing image or JSON sidecar returns `EVIDENCE_ALREADY_EXISTS`. The evidence directory remains owned by the XYC session.

`temporaryTopmost=true` is accepted only with `backend="gdiCompatibility"`. `timeoutMs` is bounded to 1–30000 ms and defaults to 5000. The new default never moves the Editor in Z order. GDI is an explicit compatibility reference and retains its documented desktop-composition behavior.

## Result fields

`XycUiCaptureState` returns image and sidecar paths, PNG SHA-256, dimensions, actual TOP DIP/pixel bounds, window PID/HWND/client dimensions/DPI/state/foreground, actual theme, BuildIdentity, UTC capture time, capture method/backend, foreground and Z-order unchanged flags, whether the backend changes Z order or takes foreground, whether it requires a live visible Window, whether occlusion was verified, and fidelity validation status.

- `avaloniaWindow`: `changesZOrder=false`, `takesForeground=false`, `requiresVisibleWindow=true`, `supportsOcclusion=true` (verified on the tested Windows runtime), fidelity is checked against the approved external comparison manifest.
- `gdiCompatibility`: `changesZOrder=true`, `takesForeground=false`, `requiresVisibleWindow=true`, `supportsOcclusion=false`, fidelity role `REFERENCE_ONLY`.

Cancellation is checked before dispatch and after rendering, before image save. `timeoutMs` cancels the capture request and returns structured `TIMEOUT`; external session cancellation returns `CANCELLED`. Avalonia's synchronous UI-thread render cannot be interrupted mid-call, so a timeout can be observed after the render returns but prevents the PNG from being saved. Unsupported backend maps to `UNSUPPORTED_BACKEND`; duplicate evidence maps to `EVIDENCE_ALREADY_EXISTS`; invalid names/parameters map to `INVALID_INPUT`; UI/window readiness failures map to `NOT_READY`.
