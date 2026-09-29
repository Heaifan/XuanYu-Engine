# Test Evidence Policy

## Purpose

Test counts are evidence, not product verdicts. `Tests 2135/2135 PASS` may only support the tier actually exercised; it must never be reported as Product PASS without the required runtime and product evidence.

Every PASS report states:

1. tier (`T0` to `T4`);
2. what it proves;
3. what it does not prove;
4. the issue's minimum required tier;
5. the remaining evidence gap.

## Evidence tiers

| Tier | Name | Proves | Does not prove |
|---|---|---|---|
| T0 | Static Guard | Source shape, file size, 5+100, dependency and architecture rules, shader shape, string contracts | Runtime, visual output, GPU behavior |
| T1 | Unit / Pure Logic | Local math, algorithms, state machines, coordinates, LOD selection, and data models in the tested input domain | Module composition, real platform behavior, visual or GPU correctness |
| T2 | Integration / Headless | UiVm, Avalonia Headless, importer, cross-module resources, render projection, and non-real-GPU integration in the test environment | **HEADLESS ≠ REAL APP**; no HWND, NativeControlHost, real Vulkan, GPU, swapchain or present proof |
| T3 | Real Runtime | Real Editor, HWND, NativeControlHost, Vulkan, GPU, swapchain, present, fence, pointer capture, DPI, and real DEM behavior | User acceptance or final product experience |
| T4 | Product Acceptance | A real user completes the acceptance actions on the real product and observes behavior matching the requirement | Nothing beyond the accepted scope; it is not a substitute for missing technical evidence |

Evidence is monotonic only in the sense that a higher tier may include lower-tier evidence. A lower-tier PASS never derives a higher-tier PASS: `T0 ≠ T1`, `T1 ≠ T2`, `T2 ≠ T3`, and `T3 ≠ T4`.

## Minimum tier matrix

The machine-readable source is `test-evidence-matrix.json`.

| Issue or claim | Minimum evidence |
|---|---|
| Pure math | T1 |
| Business logic | T1/T2, according to whether composition is in scope |
| UiVm | T2 |
| Avalonia focus or pointer | T3 |
| NativeControlHost | T3 |
| Vulkan swapchain | T3 |
| GPU rendering | T3 |
| Frame pacing | T3 |
| Final visual effect | T4 |
| DEM orbit jitter | T1 large-coordinate math + T2 terrain integration + T3 real Vulkan DEM + T4 user acceptance |

## Scale evidence

Coordinate, space, camera, and depth issues must not be tested only with a small scene. Unless the issue explicitly narrows the domain, evidence should cover `1m`, `100m`, `1km`, `10km`, `100km`, `500km`, and `1000km`. A PASS at one scale is limited to that tested scale.

## Temporal evidence

LOD, cache, revision, pointer, camera, GPU-resource, and frame-state issues must distinguish single-frame correctness from temporal stability. One passing frame does not prove 100 or 1000 consecutive frames. The evidence record must state the frame count and whether the sequence was stable.

## Gate semantics

`test-evidence-gate.ps1` compares `RequiredTier` with `AchievedTiers`, then independently checks runtime and product acceptance. Headless and static contracts are never mapped to T3 or T4. A missing required tier, missing required runtime acceptance, or missing product acceptance returns `BLOCKED`.

## Reporting contract

Every gate report contains `REQUIRED EVIDENCE TIER`, `ACHIEVED EVIDENCE TIER`, `AUTOMATION STATUS`, `RUNTIME STATUS`, `PRODUCT ACCEPTANCE`, `EVIDENCE GAP`, and `TEST EVIDENCE GATE`. Automated PASS is not Product PASS unless the required T4 evidence and acceptance are present.

