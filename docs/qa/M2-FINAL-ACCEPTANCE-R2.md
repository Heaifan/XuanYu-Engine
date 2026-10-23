# M2-FINAL-ACCEPTANCE — R2 Candidate Results

Date: 2026-10-23
Disposition: **PARTIAL**. CPU/source gates and the focused RTX 3060 resource-retirement device test pass. The required Editor Swapchain stress cycles, live performance sampling, and formal Git release remain incomplete. No canonical commit or push was made. This report records the state before the authorized GitHub publication; the candidate branch and PR carry the delivery record.

## Candidate identity and boundary

- Candidate path: `E:\MyDoc\project-VSCode\XuanYuEngine\.xye-workspace\Tasks\M2-FINAL-ACCEPTANCE-R1`
- Bootstrap: `READY`; detached at parent commit `4b6f0ad0e7dfb098d08f171867e15b8a7396decc`; Git common directory is the canonical repository’s `.git`; SDK Resolver selected .NET SDK `10.0.400`.
- Restored R8 tree verification remains valid: tree `b56b2995ff2cce28d58d8eeaaefa1e7fbef09993`; 115 manifest paths, 0 blob mismatches.
- Unified staged Candidate Tree: `499f9fd7940dfdda684100057bad07f832ccd4d3` (`git write-tree`). It is not a commit.
- Source fingerprint: `git-tree-sha1:8ec62506b3f27b01728c087450c236cad724b12a;worktree-source-sha256:03F65F4DC76CBB4AABF5EA1DACEF94D87764C2D1DE20A7269E67D43033B747ED`.
- Source overlay: 195 source/config/script paths; SHA-256 of sorted `path|file SHA-256` entries is `2A9C6F60630B73213027369C9FED9F81AE93BB85011BCD2466D380DA3BB2E68A`. Manifest: `M2-FINAL-ACCEPTANCE-R2-dirty-overlay.txt`.
- Candidate Git status: 198 lines, including one untracked candidate-local `.xye-workspace` output root. Build/test output remains under that root and was not staged. No unresolved conflicts. `git diff --check` passed.
- BuildIdentity context for the last full World.Tests run: `HEAD=4b6f0ad0e7dfb098d08f171867e15b8a7396decc;DIRTY=YES;TREE=8ec62506b3f27b01728c087450c236cad724b12a|B9409C21283E4624A401AD2704242EE1EB76B3FC05B6B00DF451F8C6A1952A2D;BUILD=2026-10-23T14:10:22.5460373Z;VERSION=0.3.0.28`. The runner’s dirty digest includes candidate-local `.xye-workspace` outputs and therefore differs from the source-only fingerprint above; this identity-source mismatch must be resolved before claiming T01 fully closed. No live renderer event supplied a runtime `BuildIdentity` sample.
- Canonical was read-only: HEAD remains `28d49a17b0138f520aee1be59e64491b4fdd7d26`; current `git status --short` has 331 lines. Historical evidence preserves the earlier count 316 but not its raw path list, so a trustworthy path-by-path delta cannot be reconstructed. No canonical file was reset, cleaned, staged, or overwritten.

## R2 fixes and complete 5+100 inventory

The device-test failure was not an early GPU release. With a 1-byte cache budget, four terrain GPU chunks were recorded; three were evicted while command-buffer references remained. Before and after the first readback the state was `Retired=3 / Disposed=0 / recordedReferences=4`. After replacing the command buffer, the scheduler safely disposed four resources, but A’s cache diagnostic counter had not received the retirement completion notification. The fix forwards an `onDisposed` callback through B’s fence-based retirement scheduler and increments the cache count only at actual disposal. The test’s safety assertions were preserved.

Evidence: `.xye-workspace/Diagnostics/7d5c7b51346a4adaabf4e13c85a9771e/terrain-gpu-retirement-device.txt` and the three `terrain-gpu-retirement-*.txt` phase snapshots. Device `NVIDIA GeForce RTX 3060`, driver `2559967232`; after command-buffer replacement: `Disposed=4`, `recordedReferences=4`, `frameLeases=[0,0]`, `pendingRetirement=0`; three newly evicted resources remained retired.

The initial full-tree scan found all eight over-limit files below. Each was introduced/modified by this Candidate relative to its parent; none was an inherited baseline exception. The responsible functions were moved into focused partial files. No guard rule or 100-line limit was changed.

| Original path | Before | Minimal responsibility split |
|---|---:|---|
| `Render/ClearFrame/VulkanClearFrameOwner.cs` | 103 | `Initialize` → `VulkanClearFrameOwner.Initialize.cs` |
| `Render/Map/VulkanClearFrameOwner.MapSurface.cs` | 136 | draw and resource-retirement methods → `MapSurface.Draw.cs`, `MapSurface.Lifetime.cs` |
| `Render/Present/VulkanPresentLoop.Frame.cs` | 123 | submit path → `VulkanPresentLoop.Submission.cs` |
| `Render/Present/VulkanPresentLoop.Lifecycle.cs` | 106 | generation retirement → `VulkanPresentLoop.GenerationRetirement.cs` |
| `Render/Present/VulkanPresentLoop.cs` | 122 | frame loop → `VulkanPresentLoop.RunFrames.cs` |
| `Render/Terrain/Offscreen/VulkanOffscreenTerrainRenderer.cs` | 112 | disposal → `VulkanOffscreenTerrainRenderer.Dispose.cs` |
| `Render/Terrain/VulkanClearFrameOwner.Terrain.cs` | 102 | statistic merge → `VulkanClearFrameOwner.TerrainStats.cs` |
| `Session/VulkanRenderSession.cs` | 142 | factory → `VulkanRenderSession.Create.cs` |

After the splits, full `scripts/arch-a-guard.ps1` completed with exit code 0, including 5+100. Two source-contract tests were updated to follow the moved methods; full suite then passed.

## CPU and contract gates

- Release solution build: exit 0, **0 warnings / 0 errors**. Final run `15a58eaa5828436a93dec93decaa5885`; workspace log records 0/0.
- `XuanYu.World.Tests`, unfiltered: **2461 passed, 0 failed, 0 skipped**; final run `cd5fdb41a32049fd86c924c2cd7ed28d`.
- `XuanYu.Render.Abstractions.Tests`: **18 passed, 0 failed, 0 skipped**; run `4df10b2da63c4cae8aac6ae41a23fc17`.
- Combined A/B/C applicable contracts are included in those suites; retirement device test passed within the World.Tests full run. Earlier focused compatibility run: 6/6.
- World.Tests DLL SHA-256: `9E2491609C22853866738F4E68DD545C2F8164185621D658B5C8A34E33128B67`.
- Release default remains one frame in flight; explicit 1/2 reconfiguration remains opt-in.

## Device and live Editor evidence

- RTX 3060 Vulkan Offscreen terrain retirement test: PASS on real device; 65,536-byte readback succeeded. This verifies the targeted cache/reference retirement scenario, not the Editor Swapchain stress matrix. The Offscreen instance path does not enable the Khronos validation layer, so this run provides no zero-VUID claim.
- The Candidate Editor executable launched and created a real window (`HWND` observed); title showed `v0.3.0.28-fix+4b6f0ad0e7dfb098d08f171867e15b8a7396decc`. A subsequent Editor session imported the verified `n24e121.hgt`; a DEM surface was visibly rendered in the viewport. The runtime log showed `PresentSync generation created: generation=1; imageCount=3`, repeated `Fence complete` records for generation 1 and the active Swapchain, and zero warning/error log entries at observation. This is live Swapchain/Present evidence for the production-default single-frame session. A terrain surface was visible, but no explicit ray-hit result was captured.
- Expanding the runtime log panel triggered one viewport size update; this was not a controlled 20-cycle Resize/OutOfDate stress run. No double-frame Present, 20 mode switches, 20 Resize/OutOfDate cycles, 20 Orbit cycles, Vulkan Validation VUID capture, controlled timeout/device-loss injection, or normal-renderer shutdown test was completed. Those remain NOT RUN.
- `n24e121.hgt` fixture SHA-256: `BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1`; the verified fixture was loaded into the Candidate Editor.

## Performance probe and baseline

- Static path review confirms probe-enabled `VulkanRenderSession.Create` creates `VulkanFenceWaitSampleSink`; the Present loop callback forwards `VulkanFenceWaitObservation` into that sink and exposes drain/drop-count APIs. The A/B/C DTO/adapter/sink contract tests pass.
- Actual Editor `FenceWait` callback arrival at a live sink was not observed because no valid terrain scene was loaded. T11 remains INCONCLUSIVE.
- No formal single/double-frame workload, repeated segments, 1,500 valid samples per mode/scene, P50/P95/P99, discard rate, probe overhead, or GPU render-pass timestamp report was collected. T12–T13 are NOT RUN. Do not infer input-to-visible latency or GPU execution time from API timings. `DataRevision` remains `NOT AVAILABLE`.

## T01–T15 disposition

| ID | Result | Evidence / remaining condition |
|---|---|---|
| T01 | INCONCLUSIVE | Unified Candidate A/B/C Tree and source-only fingerprint are recorded, but BuildIdentity’s runner digest includes candidate-local workspace outputs and does not equal the source-only fingerprint. |
| T02 | PASS | Final Release Solution build, 0/0. |
| T03 | PASS | World.Tests 2461/2461, no skips. |
| T04 | PASS | Arch-A full guard exit 0; no >100-line source remains. |
| T05 | INCONCLUSIVE | RTX 3060 live default-mode Editor Present and Offscreen retirement passed; validation-layer VUID evidence and the full compatibility matrix are missing. |
| T06 | NOT RUN | No real double-frame Swapchain submission run. |
| T07 | NOT RUN | 20 real 1→2→1 cycles not run. |
| T08 | NOT RUN | 20 Resize/OutOfDate rebuild cycles not run. |
| T09 | INCONCLUSIVE | Targeted retirement scenario passed with real device evidence; broader Swapchain/frame-slot retirement and validation coverage is missing. |
| T10 | INCONCLUSIVE | Candidate HWND, DEM rendering, Swapchain generation, and repeated Fence completion observed; explicit terrain hit and close/shutdown evidence are missing. |
| T11 | INCONCLUSIVE | Static live-sink wiring and contracts pass; actual render callback was not observed. |
| T12 | NOT RUN | No required sample counts or percentile/overhead report. |
| T13 | NOT RUN | Required fixed-camera, distance, orbit, and repeated performance scenes not measured. |
| T14 | INCONCLUSIVE | Candidate boundary and output confinement pass; canonical 316→331 raw path delta cannot be reconstructed from the saved count-only baseline. |
| T15 | PENDING AT REPORT FREEZE | The report and candidate are being published on a separate branch; merge/release remains pending. Candidate parent `4b6f0ad0…` is a descendant of intended remote base `f3bee1ce…`, so the PR can target `渲染器2.0` without rewriting it. |

## Final R2 ruling

- A1+B1 integration CPU/source gates: **PASS**.
- B dual-frame runtime safety: **NOT RUN**.
- C probe and performance baseline: **PARTIAL** (contracts/wiring pass; live sampling and baseline NOT RUN).
- Production single-frame CPU compatibility: **PASS**; real single-frame Editor Present remains unverified.
- Dual-frame adoption: **keep experimental/optional; no performance adoption recommendation**.
- M2 Final Integration: **PARTIAL**, not CLOSED. The cache retirement accounting defect and all discovered 5+100 violations are repaired. Remaining blockers are the real RTX 3060 Editor/Swapchain test matrix, live sample collection/performance baseline, and formal Git ancestry/release approval.
- Candidate publication: **authorized and in progress at report freeze**; final commit and PR URL are recorded in the GitHub PR conversation. Canonical release branch was not modified.
