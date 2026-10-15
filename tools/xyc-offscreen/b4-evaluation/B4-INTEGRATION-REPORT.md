# XYC-B4 Parent Integration Report

Date: 2026-10-15 (Asia/Shanghai)
Requested baseline: `10af215415a33cb26c4fccf81cb7452ef800d70a`
Checkout at integration: `recovery/editor-center-navigation-r1` @ `10af215415a33cb26c4fccf81cb7452ef800d70a`; upstream tip matched, Ahead/Behind `0/0`.

## Integrated result

**B4 status: PARTIAL — diagnostic comparison completed, product root cause and user acceptance remain open.** All four task reports are present and reviewed. No product rendering or LOD code was changed. The product default remains Auto LOD; SSE 4 px remains a diagnostic candidate only.

| Task | Status | Integrated conclusion |
|---|---|---|
| A — Coverage forensics | Complete; cause not confirmed | The saved B3 offscreen image has 33.95% exact Vulkan ClearColor pixels, not sea. A terrain-only pass and missing raw D32 readback cannot explain why terrain is absent. The likely below-surface camera estimate is inference only. B3 artifact runtime identifies `9429be6…`, not the requested `10af215…`; a product-window same-pose capture is absent. |
| B — SSE quality analysis | Complete; conditional on supplied B3 artifacts | At the near-oblique sample, Auto is 22.565 ms GPU P50 / 23.459 ms P95, 1.910M triangles, 8.08 m source-to-final P95. SSE 1 px is 20.349 / 21.353 ms, 1.747M triangles, 7.84 m P95; SSE 2 px is 17.960 / 18.950 ms, 1.500M triangles, 8.29 m P95; SSE 4 px is 12.907 / 13.872 ms, 1.092M triangles, 11.96 m P95. LOD+1 is 5.568 / 6.269 ms, 0.486M triangles, 14.38 m P95. These are offscreen results with unresolved B3 runtime lineage and coverage; they are not product-performance claims. |
| C — Camera trajectory | Complete; 4 PASS, 2 INCONCLUSIVE, 0 FAIL | Auto and SSE 4 px each captured 229 frames (458 total) with per-frame camera, 81 chunk decisions, triangles, GPU timestamp and PNG. Continuous zoom, near-plane pressure, return-A→B→A and 20 km-to-near passed. Chunk-entry and orbit are inconclusive after adjacent coverage changes of 79.91% and 35.77%; these are shared/pose-related observations and do not establish an LOD defect. A→B→A returned exact camera and PNG for both variants. This is offscreen Vulkan, without a window, swapchain or Present. |
| D — Native control audit | Complete; static audit only | The CLI can launch a separate windowed Editor, set test camera poses and Orbit, load the guarded NASADEM, and capture a Present-linked Vulkan color frame. This makes a controlled native replay feasible; it was not run. Editor depth export and stable same-frame Chunk/LOD linkage are missing. No P4 pass is claimed. |

## Ownership and evidence limits

- No paths overlap across the four tasks and no shared renderer or product files were changed. B's analyzer and report were placed at the B4 evaluation root rather than its declared `b-quality/` source scope; its generated data is under the B quality evidence directory. This is a scope-placement deviation, not a write collision.
- A/B source evidence comes from B3 artifacts whose embedded runtime HEAD is `9429be6…`; the task baseline is `10af215…`. Treat their quantitative and image conclusions as conditional until provenance is reconciled.
- C's final report and PNG/JSON frame evidence are under ignored `artifacts/diagnostics/xyc-b4/c-trajectory/20261015-222609/`. They remain available in this checkout but are not included in this source commit. The tracked runner and this summary preserve the reproduction method and integrated findings.
- The two pre-existing Foreign Dirty files were preserved with their original SHA-256 values: `XuanYu.Editor.UI/Diagnostic/TerrainCameraReplayRunner.cs` = `924AAD614DBD993EDF751F3268387D668AD971DE9E48ED7DC47DDC9399A1A14F`; `tools/terrain/Invoke-WP2-P4-VulkanReplay.ps1` = `28D47B62FC9FF8FB70E77E757C350EE62EF6203C86ED992320BAA830142668D8`.

## Verification and version decision

- Bootstrap: READY; repository, branch, HEAD and .NET SDK 10.0.400 confirmed.
- Parent reran the C trajectory runner's restore/build-only path: succeeded, 0 warnings, 0 errors. No GPU capture was started by this verification.
- Parsed A coverage summary and C frame JSON; confirmed 229 frames for each variant, 229 paired comparisons, 1,133 PNG evidence images, and offscreen-only report classification. The final staged `git diff --check` reports trailing Markdown hard-break whitespace in the delegated A report and a blank line at EOF in the delegated D report. Those files were left byte-for-byte as delivered to respect task ownership; the Parent report itself has no whitespace errors.
- No product-code regression suite was applicable because this integration changes diagnostic reports/tools only; no product algorithm or default changed. The four task-level validations are described in their reports.
- Product version remains `0.3.0.21-fix` (`Directory.Build.props`). Per Constitution 3.4 and `docs/dev-rules.md`, diagnostic tooling and analysis do not justify a product version bump. No Changelog product entry is warranted.

## Root cause and next steps

The available evidence does **not** establish a product LOD defect or confirm that SSE 4 px is an acceptable quality/performance trade-off. The main blocker for interpreting A/B is the B3 runtime identity mismatch; A also lacks matching product-window evidence and raw depth, while C's orbit and chunk-entry trajectories remain inconclusive. No product modification is recommended from this evidence. Keep SSE opt-in and the production default unchanged.

Priority: reconcile B3 provenance and run a bounded native Editor replay with same-pose captured color and frame-linked camera/Chunk/LOD evidence; then review the two inconclusive trajectories and obtain user visual acceptance. Do not classify offscreen results as user P4.

## XYK CANDIDATE

Candidate: Distinguish clear-color pixel classification from the cause of missing terrain. For LOD studies, keep source-to-render error, render-to-selected-mesh error, source-to-final-mesh error, screen-pixel difference, and coverage as separate evidence layers; pooled sample quantiles must not be conflated with per-chunk distributions. Bind all visual/performance conclusions to the captured build identity and runtime mode, and keep offscreen Vulkan evidence separate from native Editor P3 and user P4 acceptance.

Evidence: XYC-B4 A/B/C/D reports and their source/runtime evidence, 2026-10-15.
Disposition: candidate for independent ChatGPT review; no direct XYK write.
