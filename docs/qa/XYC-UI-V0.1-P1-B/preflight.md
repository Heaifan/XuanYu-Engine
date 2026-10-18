# Preflight — XYC-UI-V0.1-P1-B-R1

Date: 2026-10-18 (Asia/Shanghai)
Formal baseline: `d2d011ac98314f172e0e3d729583e67832099f1f`

| Check | Result | Evidence |
|---|---|---|
| Current branch / HEAD | PRESENT | `recovery/editor-center-navigation-r1` / `d2d011ac98314f172e0e3d729583e67832099f1f` |
| Remote tracking HEAD | PRESENT | `origin/recovery/editor-center-navigation-r1` / `d2d011ac98314f172e0e3d729583e67832099f1f` |
| Ahead / Behind | PRESENT | 0 / 0 at bootstrap; rerun before candidate closeout |
| Worktree state | DIRTY, preserved | Exact status snapshot in `foreign-dirty-manifest.txt`; unrelated dirty paths are not candidate inputs |
| XYC P0/P1-A formal baseline | PRESENT | P1-A commit `d2d011ac`; accepted manifest at `docs/qa/XYC-UI-V0.1-P1-A/evidence/candidate-final/runtime-evidence.json` |
| Existing `ui.capture` | PRESENT | GDI BitBlt against DWM desktop composition; calls `SetWindowPos` to HWND_TOP or TOPMOST and restores prior Z order |
| P1-A before/after captures | PRESENT | `ui.control.invoke` captures using `temporaryTopmost=true`; sidecars record capture method and identity |
| Production TOP Visual Tree | PRESENT | `FindTop()` resolves one production `Top`; accepted runtime manifest records 203 nodes |
| RenderTargetBitmap | PRESENT, REAL WINDOW RTB VERIFIED | Avalonia 12.0.4; production Window render/crop tested on all four profiles |
| Windows Graphics Capture | PRESENT, REAL HWND FRAMES VERIFIED | Windows 11 build 26300; supported=true, nonforeground frames captured; four TOP comparisons below approved threshold |
| Display | PRESENT | Primary display 1920x1080; NVIDIA RTX 3060; Windows 11 Enterprise build 26300 |
| Current Editor process | ABSENT at preflight | No `XuanYu.Editor` process found |
| Build tooling | PRESENT | `scripts/xye-bootstrap.ps1`; .NET SDK 10.0.400 through resolver |
| Shared `bin` outputs vs P1-A accepted binary | MISMATCH | App and UI DLL hashes differ from accepted manifest; rebuilding clean formal source snapshot before experimentation |

Ownership: this lane owns `docs/qa/XYC-UI-V0.1-P1-B/**`, its unique plan file, and any later explicitly scoped XYC capture/runtime/tests/scripts. All other dirty paths remain foreign and preserved.

P1-B implementation is in the XYC control/runtime/tests scope only. UI layout, renderer, map/terrain and Silent Host code are untouched. Candidate builds are materialized from this formal HEAD plus the eight exact XYC source/test files; current shared `bin` products and unrelated dirty files are excluded.
