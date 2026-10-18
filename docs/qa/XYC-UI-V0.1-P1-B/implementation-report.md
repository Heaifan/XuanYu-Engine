# XYC-UI-V0.1-P1-B implementation report

## Result

**P1-B: PASS for the authorized TOP capture scope.** The production Avalonia `Window` can be rendered into an in-memory `RenderTargetBitmap`, then cropped to the measured TOP bounds without screen sampling, foreground activation, input injection, or Z-order changes. The existing GDI desktop capture remains available as an explicit compatibility/reference backend.

The capture result and sidecar report backend, window identity, bounds, DPI, theme, UTC time, SHA-256, foreground/Z-order observations, and fidelity role. Capture names are bounded to approved profile names and optional safe suffixes; arbitrary paths are rejected. Existing artifacts are not overwritten. Timeout and cancellation return structured errors; cancellation is checked before and after the synchronous render, before saving.

## Evidence

- Four real production Editor sessions passed against the approved P1-A GDI references. Per-profile metrics are in `evidence/candidate-capture-comparison-final9.json`.
- The occlusion/minimized/restore/duplicate-name probe passed on the final Release capture implementation; see `evidence/isolation-runtime-final6/isolation-report.json`.
- Four P1-A control regression profiles passed on the final Release assembly; see `evidence/candidate-runtime-final9/runtime-evidence.json`.
- The explicit timeout request returned `TIMEOUT` without producing a PNG in the earlier final runtime probe; see `evidence/candidate-runtime-final5/runtime-evidence.json`.
- Candidate Release and Debug App/CLI builds succeeded. Scoped XYC tests passed 17/17.
- Candidate source tree identity: `4c70b5d3d79c2a1f94ace342868255528740458d` (owned code, tests, and XYC scripts over the formal baseline).
- Silent Host smoke passed on the final Debug build. It confirmed hidden HWND, no foreground activation, fixed viewport, Vulkan Present, clean close, and no process residue. Readback was not exercised in this smoke.

## Limits

`RenderTargetBitmap` re-renders the live Avalonia Window on its UI Dispatcher; it is not a Vulkan-presented frame and does not capture the central native viewport. It requires a live, visible Window. The tested backend remains usable when the Window is occluded or minimized, but UI-thread rendering is synchronous and cannot be interrupted mid-render; the timeout is observed before/after that operation. Windows Graphics Capture produced real HWND frames but failed the approved fidelity thresholds and is not integrated.

The four-profile matrix ran on Windows 11 Enterprise build 26300, Avalonia 12.0.4, RTX 3060, 96 DPI (scale 1). P1-B does not establish full desktop-free/offscreen Editor execution. The GDI compatibility backend can alter Z order temporarily and remains explicitly marked `REFERENCE_ONLY`.

## Governance

Formal baseline is P1-A commit `d2d011ac98314f172e0e3d729583e67832099f1f`. Product version remains the baseline version `0.3.0.24-fix`; no version number was inferred and no independent Version Event ledger was created, following Constitution Article 34 and the user's ruling. The Candidate changelog note is unversioned. Foreign dirty files, including WP2/XYUI/map/terrain work and the physical shared changelog/version files, are excluded from the Candidate and preserved.

No XYK write was made. A plain-text knowledge candidate is included in the final report.
