# Test Evidence Audit 2026-09-26 — 2026-09-28

## Scope and evidence rule

This is a governance audit of the eight recent FIX commits requested by `XYE-GOV-R2-CONVERGENCE`. It does not claim a new product run, DEM visual acceptance, or real Vulkan acceptance. Commit-local tests and source diffs are recorded as the highest evidence directly observable from the commit; missing historical, runtime, or product evidence remains `UNKNOWN` / `NOT_EXECUTED`.

The tier vocabulary is T0 Static Guard, T1 Unit/Pure Logic, T2 Integration/Headless, T3 Real Runtime, and T4 Product Acceptance. T0–T2 cannot be promoted to T3/T4 by test count.

## Recent fix matrix

| Commit | PRODUCT ROOT CAUSE | TEST HARNESS ROOT CAUSE | ROOT CAUSE CLASSIFICATION | ORIGINAL EVIDENCE TIER | REQUIRED EVIDENCE TIER | RED/GREEN WITNESS STATUS | TEST ORACLE QUALITY | RUNTIME EVIDENCE GAP | PRODUCT ACCEPTANCE GAP |
|---|---|---|---|---|---|---|---|---|---|
| `7e7dc6ca` | Long-range terrain auto-framing/render projection mixed world extents, camera framing, and terrain placement; commit adds large-range framing and placement behavior. | No independent harness failure evidence in commit; test setup uses UiVm/import fixtures. | PRODUCT (harness integrity not independently audited) | T2: UiVm integration tests, including finite camera planes and viewport resize | T3 for real Editor/Vulkan/DEM; T4 for user-visible long-range product acceptance | INCOMPLETE: no same-test PRE-FIX FAIL + POST-FIX PASS record found | Good for UiVm state and finite-value invariants; not a pixel/runtime oracle | Real Vulkan, GPU, DEM runtime, temporal sequence not evidenced | No user IPO or visual acceptance evidence |
| `b949df52` | Tile origin was applied twice at the mesh/instance boundary. | No harness root cause evidence. | PRODUCT | T1: deterministic mesh contract tests | T1 for mesh math; T3 if claiming rendered placement | INCOMPLETE: post-fix contract coverage only; PRE-FIX RED not recorded | Strong for local vertex/origin arithmetic; does not prove frame output | No real renderer draw/runtime evidence | No product visual/position acceptance |
| `bee02894` | Physical resize and swapchain lifecycle allowed incorrect resize/churn ordering around present and DPI conversion. | No independent dispatcher/native-host harness witness; source contract tests inspect implementation text. | PRODUCT (harness gap unresolved) | T0/T1: source contracts and DPI conversion tests | T3 for HWND/NativeControlHost/Vulkan swapchain/present | INCOMPLETE: no historical RED/GREEN witness; source contracts are coverage tests | Good for explicit source invariants and conversion arithmetic; weak for lifecycle timing | No real HWND, surface, swapchain, fence, present or resize sequence | No user resize/DPI acceptance |
| `81a8ecde` | Reference grid depth state was coupled to map-ground bias; fix makes the grid depth-independent and preserves draw order. | No harness root cause evidence. | PRODUCT | T0/T2: shader/pipeline source contracts and draw-plan integration | T3 for real GPU pixels; T4 for final visual result | INCOMPLETE: contract regression only; PRE-FIX RED/GREEN not recorded | Strong for declared pipeline/source contracts; not a visual oracle | No GPU frame capture or temporal visual evidence | No real visual acceptance |
| `f09245a8` | Terrain resource identity/revision did not consistently represent reimported content while camera navigation must preserve revision. | Fixture uses temp files and UiVm; independent fixture-integrity record is absent. | PRODUCT (harness integrity unresolved) | T2: UiVm integration with temp-file lifecycle | T3 for real resource/render runtime; T4 if acceptance claims visible update | INCOMPLETE: post-fix tests only; no executed PRE-FIX FAIL record | Good for revision and reimport state; no GPU/resource lifetime oracle | No real Vulkan resource replacement or frame evidence | No user reimport/visual acceptance |
| `aa285b53` | Terrain visibility/LOD/cache keys and retention did not fully converge on chunk, revision, LOD, and scale state. | No harness failure evidence; pure logic tests do not establish runtime harness validity. | PRODUCT | T1: pure visibility/LOD/cache tests; T2: draw contract | T3 for GPU cache and render frame pacing; T4 for product stability | INCOMPLETE: no same-test RED/GREEN witness | Good for deterministic selector/cache contracts; no frame-time or GPU oracle | No real GPU cache churn, frame pacing, or long temporal sequence | No user orbit/LOD acceptance |
| `3e09d6df` | Camera navigation rebuilt or replaced terrain render resources instead of reusing imported resources on the hot path. | UiRuntime fixture exists, but Dispatcher/platform/lifecycle integrity is not independently recorded. | PRODUCT (harness integrity unresolved) | T2: UiVm/UiRuntime integration and resource identity assertions | T3 for real UI/render hot path; T4 for product interaction acceptance | INCOMPLETE: no historical PRE-FIX FAIL captured | Strong for resource identity/build-count invariant; does not measure real frame behavior | No real Editor/GPU timing or sustained sequence evidence | No user camera-navigation acceptance |
| `b95b2e79` | Reverse-Z/reference-grid pipeline and shader contract had a depth-dependent path that conflicted with the intended independent world-reference layer. | Tests read source and shader files; no real GPU harness evidence. | PRODUCT | T0/T1: shader/source contracts and pure reverse-depth formula | T3 for real Vulkan depth/pixels; T4 for final visual effect | INCOMPLETE: contract tests do not prove historical RED/GREEN | Strong for source contract and formula; cannot oracle rendered pixels | No GPU capture, depth buffer, or temporal evidence | No user visual acceptance |

## Witness and harness verdict

- No requested commit contains an auditable same-test PRE-FIX `FAIL` and POST-FIX `PASS` pair in the evidence inspected here.
- The eight rows are therefore `REGRESSION WITNESS = INCOMPLETE`, not PASS.
- No independent harness record proves all required Dispatcher, Click, Pointer, PlatformServices, fixture isolation, and consecutive-state checks for the UiRuntime/Headless cases. Where such evidence is absent, runtime/product claims remain blocked or unknown; it is not silently relabeled as PRODUCT FAIL.
- Retroactive reconstruction is prohibited. Historical commit/diff evidence may explain the change, but it cannot fabricate `PreFixResult = FAIL`.

## Requested governance decisions

### A — Debt Contract / RENDER-LARGE-WORLD-001

The debt has explicit expiry conditions: terrain/DEM runtime, geographic-world runtime, world extent or camera distance at 10 km and above. Its valid envelope limits absolute-float rendering to small/medium temporary use, and its required closure includes camera-relative/render-origin work, 50/100/500/1000 km tests, real Vulkan + DEM runtime, and user acceptance. It is correctly `BLOCKING` for large-world product closeout and related capability tags. This is not a claim that DEM jitter is fixed.

The contract is not a renamed TODO: it has a state, activation triggers, blocked capabilities, test debt, evidence, and closure evidence requirements enforced by the gate.

### B — T0–T4 Evidence Gate

T0 through T4 are strictly monotonic. T2 explicitly states `HEADLESS ≠ REAL APP`; it cannot establish HWND, NativeControlHost, Vulkan, GPU, swapchain, present, or product acceptance. The matrix includes scale evidence from 1 m through 1000 km and a temporal distinction between a single frame and 100/1000-frame stable sequences.

### C — Regression Witness / Harness Integrity

The policy distinguishes PRODUCT, TEST_HARNESS, MIXED, and UNKNOWN. `UNKNOWN` is blocked. Retroactive records require `PreFixResult = NOT_EXECUTED` and a reason; they cannot be used as a complete witness. Harness failure produces `EVIDENCE INVALID`, not an invented product regression.

## Ownership Manifest P1

`tools/handoff/ownership-manifest.json` still contains old B/C task files. Current Handoff resolves one repository-level manifest from `state.ownershipManifest` or `HandoffConfig`; it does not provide a natural Wave-scoped generated manifest. This round therefore does not modify Handoff Core. `GOV-OWNERSHIP-LIFECYCLE-001` is registered with a valid envelope, activation trigger, blocked capability, and required closure so the static manifest has an explicit expiry boundary.

## Knowledge / Experience audit handoff

SEARCH EXISTING found K-GOV-001, K-GOV-002, K-GOV-003, K-VAL-001, K-VAL-002, L-VAL-001, L-TEST-001, and EXP-GOVERNANCE-001/002/003. No existing formal K-GOV-004, K-GOV-005, Deferred Capability / Expiry Governance, Evidence Tier Governance, or Regression Witness / Harness Integrity entry was found in the current index. Candidate dispositions for ChatGPT audit are:

- `K-GOV-004 Live Git Fact 高于 Transient Coordination State`: CREATE or STRENGTHEN only after ChatGPT confirms overlap with K-GOV-001/K-GOV-003.
- `K-GOV-005 Control Plane 必须有受限维修通道`: CREATE or STRENGTHEN only after ChatGPT confirms overlap with existing Handoff maintenance rules.
- Deferred Capability / Expiry Governance: CREATE candidate; the machine gate and explicit expiry envelope are new evidence, but formal deposition is not performed by this construction agent.
- Evidence Tier Governance: STRENGTHEN candidate for K-VAL-001/K-VAL-002; existing knowledge already covers artifact/runtime separation.
- Regression Witness / Harness Integrity: STRENGTHEN candidate for L-TEST-001 and K-VAL-002; no duplicate formal entry is created here.

`CHATGPT KNOWLEDGE AUDIT REQUIRED`.

