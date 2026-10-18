# XYC-UI-V0.1-P1-B runtime evidence

## Final binary identity

- Candidate base: `d2d011ac98314f172e0e3d729583e67832099f1f`
- App SHA-256: `DE13C9CF9C40C876B9F62C49D864F0FD665E77FE73EA5CCC1EE8896E0538B74C`
- Editor UI SHA-256: `7C685D237CC9C1E0EF385444B7E6028EC3286D22628E1A5BEDCFDEBCF1C8EE94`
- Runtime manifest: `evidence/candidate-runtime-final9/runtime-evidence.json`
- Comparison manifest: `evidence/candidate-capture-comparison-final9.json`
- Isolation report: `evidence/isolation-runtime-final6/isolation-report.json`

## Approved TOP fidelity matrix

Each profile independently meets the approved exact-pixel ≥94.5%, RGB MAE ≤0.50, PSNR ≥35 dB, and >16-delta pixel ≤0.9% thresholds. Dimensions match the P1-A reference; candidate alpha is fully opaque and actual theme state matches the requested theme.

| Profile | TOP px | Exact | RGB MAE | PSNR | >16 delta | Theme | Result |
|---|---:|---:|---:|---:|---:|---|---|
| Light-1360 | 1330×98 | 95.2348% | 0.1040 | 48.2278 dB | 0.0583% | Light | PASS |
| Light-1920 | 1890×98 | 96.6467% | 0.0732 | 49.7564 dB | 0.0410% | Light | PASS |
| Dark-1360 | 1330×98 | 94.7783% | 0.4197 | 35.3377 dB | 0.8171% | Dark | PASS |
| Dark-1920 | 1890×98 | 96.3255% | 0.2953 | 36.8639 dB | 0.5750% | Dark | PASS |

The desktop was 1920×1080 at 96 DPI, so DIP and physical pixel measurements were equal. Every final manifest entry includes PID, HWND, image hash, theme, DPI scale, BuildIdentity, foreground and Z-order observations, and backend.

## Isolation and lifecycle

The final isolation runtime placed a magenta overlay over TOP and confirmed the RTB image SHA remained unchanged; foreground and Z order remained unchanged. Minimized capture succeeded and restore returned to Normal. Reusing an evidence name returned `EVIDENCE_ALREADY_EXISTS`. Editor exited with code 0. All four matrix sessions closed with code 0; see the runtime manifest for session records.

The Editor Window is live and visible. RTB avoids reading the desktop surface, but still runs on the Window's UI Dispatcher. This evidence does not claim fully offscreen Editor execution or capture of the native Vulkan viewport.

## Silent Host

`evidence/silent-host-final6/` records PASS for hidden HWND, preserved foreground, fixed viewport, successful Vulkan Present, accepted close, clean process exit, and no Editor process residue. `readbackCaptured=false`; this smoke did not exercise GPU readback. The Silent Host source and lifecycle were not changed.

## Negative and prior exploratory results

The scoped test run passed 17/17, including capture backend validation, unsafe path rejection, cancellation before/during backend work, and timeout mapping. The four WGC comparison results remain failed against the approved threshold and are retained under `evidence/rtb-probe/wgc-standard/`; WGC is not selected as the production capture backend. Earlier failed script runs remain archived in their uniquely named evidence folders and are not used as PASS evidence.
