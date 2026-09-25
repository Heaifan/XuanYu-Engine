# WAVE-2.5 Production Input Smoke Report

- Date: 2026-09-25 (Asia/Shanghai)
- Baseline: `64a8c2c55a27a42097928417a49bdd21c4799638`
- Branch: `audit/wave-2-5-production-input-smoke`
- Entry: `run.bat` only
- Scope: observation only; no Input/Router/Capture/Viewport or other production code changed

## Evidence

- `run.bat` restore: PASS.
- `run.bat` build: PASS; app project and referenced projects reported successful.
- Vulkan instance/surface/device/swapchain/first frame: PASS by console output. Observed Native surface HWNDs `0xB0668` and, on the second launch, `0x30A70`; NVIDIA GeForce RTX 3050 was selected.
- Launch screenshot: [launch.png](./launch.png).
- A first launched run later returned exit code `1` through `run.bat`; this is recorded as a runtime FAIL, not hidden.
- A second launch remained responsive while a synthetic pointer sequence was sent. The OS focus could not be evidenced: screenshots taken after `SetForegroundWindow` still showed the covering terminal rather than the editor. Therefore the sequence is not accepted as product-input evidence.

## IPO smoke matrix

| Area | Result | Evidence / limitation |
|---|---|---|
| Camera Orbit | NOT RUN | Pointer was sent, but editor foreground/viewport target was not observable. |
| Camera Pan | NOT RUN | Same limitation. |
| Camera Dolly / wheel | NOT RUN | Same limitation. |
| Camera Release | NOT RUN | No trustworthy viewport capture observation. |
| Camera FocusLost / Alt+Tab | NOT RUN | Window focus could not be observed reliably. |
| Picking single select | NOT RUN | No visible selection proof. |
| Gizmo Move | NOT RUN | No visible gizmo/transform proof. |
| Gizmo Rotate | NOT RUN | No visible gizmo/transform proof. |
| Gizmo Scale | NOT RUN | No visible gizmo/transform proof. |
| Gizmo Release / focus loss / window switch | NOT RUN | No trustworthy capture lifecycle observation. |
| MapEdit | NOT RUN | No visible business-state proof. |
| Region single creation | NOT RUN | No visible draft/commit proof. |
| Road single creation | NOT RUN | No visible draft/commit proof. |
| Marker single creation | NOT RUN | No visible placement proof. |
| Ctrl residual state | NOT RUN | Modifier cleanup was sent, but UI state was not observable. |
| Shift residual state | NOT RUN | Modifier cleanup was sent, but UI state was not observable. |
| Alt residual state | NOT RUN | Modifier cleanup was sent, but UI state was not observable. |
| Esc residual state | NOT RUN | No trustworthy business-state observation. |
| PointerDown → Drag → Deactivate / FocusLost | NOT RUN | No reliable editor foreground evidence. |
| Workspace / Tool switch recovery | NOT RUN | Menu interaction could not be proven to target editor. |
| Native HWND vs Avalonia double response | NOT RUN | No reliable event/visual evidence; must not infer PASS. |

## Conclusion

This run does not establish production-input readiness. Build and Vulkan startup are evidenced, but the requested real viewport smoke is **NOT RUN** because the desktop focus/foreground condition prevented trustworthy observation. The first run's exit code `1` remains a runtime failure requiring separate diagnosis. No production source was modified.
