# WP2 P4 R3/R4 Diagnostic Closeout

Date: 2026-10-11. Branch: `recovery/editor-center-navigation-r1`. Diagnostic baseline source HEAD: `959e1785502e4a7aa095c6faf9a540bb8dda0a6a`; product version remains `v0.3.0.16-fix`.

## Decision

- **FarPlane truncation in the Windows Vulkan replay using the user's NASADEM tile: CONFIRMED.** With a frozen camera, terrain snapshot, viewport and source build, changing only effective FarPlane restores terrain pixels monotonically. Reverse-Z clip metrics and actual chunk geometry show those pixels lie in chunks already submitted for drawing; the normal GPU far clip removes their farther fragments. At pitch-A a second effect is observed: chunk `(0,2)` transitions from `FAR_REVERSE_Z` culling to a recorded draw when FarPlane doubles.
- **Native physical wheel / user P4: UNKNOWN / pending.** Replay drives the product ViewModel Dolly and Orbit methods, not native HWND wheel messages. The terrain is the exact user-provided NASADEM tile, and the app uses its actual import and Vulkan render path, but this is not a physical-input P4 recheck.
- **LOD regression on user's NASADEM replay: REJECTED within tested near-ground and pitch-A poses.** Automatic LOD is already LOD0 for visible chunks and the viewport diff against forced LOD0 is zero. The old synthetic pitch-A fixture has a separate, localized LOD sensitivity described below; it is not evidence that NASADEM needs LOD0 or that fixed LOD0 should be shipped.
- **CPU Culling and NearPlane as the near-ground cause: REJECTED within the frozen near-ground replay.** Disabling culling and halving NearPlane did not restore the missing terrain; the FarPlane-only variant did.
- No product behavior repair is included. The diagnostic candidate preserves the existing navigation, Frozen Orbit Pivot, Reverse-Z, active chunk culling and automatic LOD policies. A product FarPlane policy change still needs its own bounded rule, RED→GREEN, Windows Vulkan comparison and user P4.

## Input and reproducibility

The user supplied `E:\MyDoc\dem-data\NASADEM_HGT_n24e121.zip`. The ZIP was read without modifying it (SHA256 `4FAD3EC397874E3F08C6FB22C53789B742FB237DBA9317D7F9B041AA6ACCD23E`). Its `n24e121.hgt` member was extracted to the ignored diagnostics directory, not into Git. It is 3601×3601 signed big-endian Int16, north-to-south rows, 25,934,402 bytes, SHA256 `BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1`. Elevation inspection: min -43 m, max 3867 m, mean 900.4255 m, center 1528 m; no Int16.MinValue samples.

Replay command (Windows PowerShell, repository root):

```powershell
./tools/terrain/Invoke-WP2-P4-VulkanReplay.ps1 `
  -TerrainPath (Resolve-Path 'artifacts/diagnostics/wp2-p4-r3/n24e121.hgt').Path `
  -Variants @('baseline','far-expanded','cull-off','lod0','near-small') `
  -FarPlaneThresholds @(800,1000,1200,1400,1600,1800,2000,2200,2400,2600,2800,3000,3200)
```

The source and published-output fingerprints for this captured batch are `83959335BD24706F600CB5E1F0974B36A0955F189706B4D071501F7E0E784FF9` and `A17C80A414436D5A399F8325DDAD443BAFDC0F3DD7212E2460DF4658F3AF95CC`. The app used its normal HGT import and Vulkan draw pipeline. All captures use target-process `PrintWindow`; manifests retain process ID, HWND, capture frame, GPU frame, camera, terrain revision, render origin, viewport, image SHA256, and source/publish fingerprints. The 13 threshold runs completed and each held a successfully submitted and presented Vulkan frame for capture.

The five controlled runs are independently retained under ignored `artifacts/diagnostics/wp2-p4-r3/<RunId>/<variant>/run.manifest.json`:

| Variable | RunId | Baseline / changed state |
|---|---|---|
| Baseline | `ba30075c05aa42dfa02b0ccee982c21f` | Effective Far 1221.4006 m; Near 0.05 m; auto LOD; normal culling |
| FarPlane | `7caf96c7fe0c4d26b35e972adb84d05b` | Near-ground Far 2442.8013 m; pitch-A Far 63024.5312 m |
| CPU culling | `1e54c0177a1648899655a2270a08ebbb` | CPU Chunk culling disabled |
| LOD | `7b1d301506be4685bce7f69f0f411627` | Fixed LOD0 |
| NearPlane | `bf450be27d184105aa9be3e1a88f27da` | Near 0.025 m |

The R3 analytic synthetic HGT fixture remains reproducible and ignored, with its parameters and digest in `artifacts/diagnostics/wp2-p4-r3/n23e121.manifest.json`: 3601×3601 signed big-endian Int16, north-to-south rows, SHA256 `12E5B306F5DBF10731292E8A5A79120B2765172A921CEF0F7BFA27E0ACF5A487`; deterministic analytic function, no random seed; ridge step, isolated peak, deep valley, negative basin and 12×12 NoData patch.

## FarPlane experiment results

At near-ground, baseline and Far-expanded have matching camera/terrain/render metadata (CameraRevision 45, same position/forward, ObservationCenter, RenderOrigin, terrain revision 1, HGT SHA, source/publish fingerprints, 1336×839 viewport). Only effective Far changes: 1221.4006→2442.8013 m. Viewport classification (green terrain mask `G-R>20 && G-B>5`): 203,085 pixels differ; 203,008 background→terrain, 0 terrain→background, 75 terrain-color changes, 2 other pixels.

Pitch-A likewise keeps all camera and terrain state fixed (CameraRevision 24); effective Far changes 31512.2656→63024.5312 m. There are 48,010 viewport differences: 44,608 background→terrain, 0 terrain→background, 2,566 terrain-color changes and 836 other changes. The Far-expanded visible strip overlaps chunk `(0,2)`'s projected bounds; that chunk changes from `CULLED / FAR_REVERSE_Z` to `VISIBLE / DRAW_RECORDED`, IndexCount 49,344.

Near-ground Far threshold scan, same camera pose and terrain snapshot in all runs:

| Far (m) | Terrain pixels | Viewport share |
|---:|---:|---:|
| 800 | 186145 | 16.61% |
| 1000 | 253903 | 22.65% |
| 1200 | 310890 | 27.74% |
| 1400 | 356838 | 31.84% |
| 1600 | 383567 | 34.22% |
| 1800 | 416335 | 37.14% |
| 2000 | 455790 | 40.66% |
| 2200 | 494263 | 44.09% |
| 2400 | 515819 | 46.02% |
| 2600 | 533237 | 47.57% |
| 2800 | 557951 | 49.78% |
| 3000 | 574700 | 51.27% |
| 3200 | 586409 | 52.31% |

The curve is monotone over the tested range and has not plateaued at 3200 m. The final 3000→3200 m step adds 11,709 terrain pixels. This is causal evidence for the clipping mechanism, not a proposal to set FarPlane to 3200 m or expand it without bound.

For every threshold, the same two chunks `(0,1)` and `(1,1)` are recorded for draw, both LOD0 and IndexCount 351,360. Chunks `(0,2)` and `(1,2)` remain Far-rejected throughout 800–3200 m; the remaining five chunks reject against side/bottom planes. Thus the newly visible near-ground pixels are clipped fragments within the same submitted chunks, not additional whole chunks. At baseline, chunk `(0,1)` has 6/8 AABB corners outside the far clip but remains intersecting/visible; chunk `(1,1)` has 4/8 outside Far. The clip metric measures all eight actual inflated chunk-bound corners after render-origin subtraction; it does not infer that camera-in-AABB implies every corner has W≤0. The draw commands were recorded into the captured successful QueueSubmit/Present frame.

The threshold PNG SHA256 values are stored in the replay JSON manifests. For direct audit, they are listed here by Far (m):

| Far (m) | PNG SHA256 |
|---:|---|
| 800 | `68afe5c774e479cfa5fcf7e0eab16a50da129b8d34f389bf7019c9c1a91b16ce` |
| 1000 | `89484a2295004b2afc1c2df64dd39ca80f7582057f13b4462637661b2cfffb7c` |
| 1200 | `7186be20d27fbd729c9f63be6dee9ddacecd5e5a4690e1daf4e4156fb4471c0a` |
| 1400 | `ae07b492f3c6a7504ea3476af3eb20d6840f2e3d22a04aea4c9ff23245f1e3d3` |
| 1600 | `cd56aa66556a48766e781dcf63076c3f748349cf90abd4deb1f4ff1e79db7408` |
| 1800 | `cbf8505a2d5bbff34cb365adfccbe636f4619535a24b1ccb631c3c6843290584` |
| 2000 | `c7aecd3a5037e9866c944970623481c1bb2215e153af096067bdefcca9dd5195` |
| 2200 | `470df7728f82fb6e13ff5b9b8df944233a71d529ebd86446ab645f4ab8c8d1b1` |
| 2400 | `32b0e3b32d0c3c34c78613189527424005456dd004e8b74f0d32dbe6707a6d20` |
| 2600 | `aba02b7c7dd770e2c58245c66dcf99116a63cbd1d9d03af12e309a967f3eda45` |
| 2800 | `7da6e9e4a9d8a9d541e6612b7b8b01f920607869c7e03506d117ee82e027a9d9` |
| 3000 | `1b1794b4addfedffd2f5316586ca97252445d3029c78b297934addee7dfc4670` |
| 3200 | `5acb4f68ab0324021f64c643f77554fa871d1d622c703be11f1a3ba82b938345` |

Heatmaps: ignored diagnostic files `artifacts/diagnostics/wp2-p4-r3/r4-diffs/near-ground-ab-farplane-heatmap.png` and `pitch-a-farplane-heatmap.png`; generated direct baseline/expanded difference masks also exist as `near-far-heat.png` and `pitch-far-heat.png` in the same folder. Controlled near-ground screenshot SHA256: baseline `6A543BFE4FF541BA5596CDCADA40D89B29B9013308E07F5727FF632A118AEB29`, Far-expanded `8354B6AB665C21A32F05EDF7D3E670BC316B71B711EDA065CBBA259A02556806`. Pitch-A: baseline `BC1DF456E544188892271B4263C77A6FCE81E03FDAAA083098811A4310DEB770`, Far-expanded `BBDE7653FB59169BAC424FB48CEBB8CCF2CD49FD90DA6FB9040DB3586E730ED1`.

## Independent LOD evidence

On real NASADEM, forced LOD0 and automatic LOD produce identical near-ground and pitch-A render-viewport pixels. Visible chunks at both poses already select LOD0. The old synthetic pitch-A comparison has 27,473 changed terrain pixels (heatmap `artifacts/diagnostics/wp2-p4-r3/r4-derived/pitch-lod-heat.png`): only chunk `(1,0)` changes from automatic LOD1 / 89,280 indices to LOD0 / 351,360 indices. All differences are in that chunk's projected terrain area, viewport x=0..396, y=409..839. Its bounds are X 47,853..95,707 m, Y 0..52,181 m, Z -189..2,210 m; camera is (56,698, 52,301, 4,665), about 119 m beyond its Y maximum. This confirms LOD changes detail in the synthetic region. It does not demonstrate the user's NASADEM symptom or justify permanently forcing LOD0. Per-pixel triangle/world-coordinate identity is unavailable; association is by projected chunk extent, not a fragment-ID buffer.

## Diagnostic changes and governance

Diagnostic gates require both `XYE_PROBE_VIEWPORT=1` and `XYE_TERRAIN_REPLAY=1`; the default production path allocates no replay frame IDs, applies no camera overrides, and performs no diagnostic output. Replay options are cached at process startup. The captured build metadata was generated from a dirty candidate, with full source-tree and published-output fingerprints retained in ignored manifests.

5+100: all changed/added hand-written C# files are at most 100 lines (maximum 98). SRP review: `TerrainClipBoundsMetrics` owns clip-space calculations and `TerrainSnapshotFingerprint` owns snapshot hashing. `TerrainReplayCapture` owns one cohesive capture transaction: freeze presented frame, serialize request metadata, wait for the Windows capture acknowledgement, and release the frame hold in `finally`.

Registered formal focused tests: `XYT-B-0004` and `XYT-B-0005`, `TSET-TERRAIN-REPLAY-v1.0`, capability `CAP-TERRAIN-REPLAY-CLIP`. Product version event is NONE because this candidate adds diagnostic/governance capability and contains no product behavior fix. Governance event `WP2-P4-R3-R4-CLOSE` follows PRE-COMMIT → POST-COMMIT ledger reconciliation under EXP-GOVERNANCE-003.

Verification executed on the candidate source:

- Release solution build: `./scripts/xye-dotnet.ps1 build ./XuanYu.Engine.slnx --no-restore -c Release` — 0 warnings, 0 errors.
- Focused tests: `./scripts/xye-dotnet.ps1 test ./XuanYu.Core.Tests/XuanYu.Core.Tests.csproj --no-restore --filter "FullyQualifiedName~TerrainReplayOptionsTests|FullyQualifiedName~TerrainFrustumCullReasonTests" --logger "console;verbosity=minimal"` — 18/18 passed.
- Full `XuanYu.Core.Tests`: 522 passed, 2 skipped, 0 failed. Full `XuanYu.World.Tests`: 2284 passed, 0 skipped, 0 failed.
- XYT changed-file runner: `./scripts/governance/xyt-runner.ps1 -ChangedListPath artifacts/diagnostics/wp2-p4-r3/r4-changed-files.txt -OutputPath artifacts/diagnostics/wp2-p4-r3/r4-xyt-plan.json -AgentTests @('XuanYu.Core.Tests/Diagnostics/TerrainReplayOptionsTests.cs','XuanYu.Core.Tests/Space/TerrainFrustumCullReasonTests.cs')` — PASS, 13 required selections, 0 disputes. All 9 selected governance selftests passed: H2 Evidence, Regression Witness Gate, Test Evidence Gate, XYT Evidence, XYT Incident, XYT Process Runner, XYT Runner, XYT Runtime Artifact Gate, and XYT Runtime Harness.
- Registry and capability-map JSON parse; both test IDs are unique and their paths resolve. Both replay PowerShell scripts pass AST parsing. `git diff --check` passes. All changed/added C# files pass the 100-line maximum.
- Windows Vulkan replay: all five controlled variants and 13 Far thresholds completed under target-window PrintWindow capture with the same candidate source/publish fingerprints; screenshot/image hashes and manifests are retained as above.
