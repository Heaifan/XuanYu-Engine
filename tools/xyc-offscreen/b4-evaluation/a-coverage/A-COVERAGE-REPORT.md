# XYC-B4-A-COVERAGE-FORENSICS

Date: 2026-10-15 (Asia/Shanghai)  
Checkout: `recovery/editor-center-navigation-r1` @ `10af215415a33cb26c4fccf81cb7452ef800d70a`  
Dataset: NASADEM `n24e121.hgt`, SHA-256 `BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1`  
Capture: 1024×768, RTX 3060; source PNG `artifacts/diagnostics/xyc-b3/20261015-202400/trajectory-sse-4/near-oblique.png`.

Recorded near-oblique pose: position `(49848.444336988396, 55060, 1778)`, forward `(0.7954116786684059, 0.5784812208497497, -0.1807753815155468)`, up `(0.14619957418529833, 0.10632696304385332, 0.9835244081556433)`, FOV `45°`, near `0.1`, far `445280`.

## Result

**Blue-pixel source is confirmed as Vulkan ClearColor in the saved B3 PNG.** The image contains exactly 267,015 pixels (33.95%) with RGB `(89, 140, 204)`, the 8-bit quantization of the render-pass clear `(0.35, 0.55, 0.80, 1.0)`. The complementary 519,417 pixels (66.05%) are terrain-colored. The offscreen capture projection contains terrain only; it has no sea/water draw. Therefore this blue is not rendered sea.

This establishes which color produced the blue region; **it does not establish that a real product defect exists**. The clear color is also the renderer's documented sky-failure fallback. The saved capture is from the offscreen terrain pass, and its `runtime.json` declares `HEAD=9429be62288cd58c56ff7a9b6608857afb4c6175`, not the task's B3 HEAD `10af215415a33cb26c4fccf81cb7452ef800d70a`. The current specified HEAD's tracked clear/depth code agrees with the observed RGB, but the artifact's runtime identity mismatch prevents attributing the full image to that exact build. The pixel color is confirmed; final product defect status remains **NOT CONFIRMED**.

There is a likely explanation in the captured pose: mapping the same HGT through the logged 2049 grid and LOD0 mesh triangle pattern estimates terrain Z at camera XY as 1790.10 m, while camera Z is 1778 m (about 12.10 m below the modeled surface). This is inferred from DEM samples and mesh interpolation, not GPU depth. A camera below the terrain looking slightly downward can leave rays directed into the ground with no terrain surface intersection, exposing ClearColor. This makes an underground camera pose more likely than a false cull or near-plane failure, but does not establish whether the interactive product is expected to prevent terrain penetration.

## Evidence and classification

| Candidate | Finding | Evidence strength |
|---|---|---|
| ClearColor/background | Exact clear RGB accounts for 267,015 pixels. | CONFIRMED for this saved PNG |
| Sea | The B3 offscreen projection is terrain-only; no water draw exists in that harness. | EXCLUDED for this capture |
| NoData | Source HGT has 12,967,201 samples and zero `-32768` NoData values; source SHA matches B3. | EXCLUDED as source-tile NoData |
| Terrain boundary | Camera is at `(49848.444, 55060, 1778)` in the interior of the 1° tile; source has full extent and no NoData. | Unlikely; not independently ruled out at every pixel |
| Near-plane clipping | Near-oblique has 21 recorded visible chunks. Camera-containing chunk `(4,4)` is still drawn and reports `ClipNearOutsideCorners=2`; this is AABB-corner telemetry, not proof that clipping caused the blue region. | No causal evidence; not fully excluded |
| Chunk/frustum culling | At camera revision 3, 21/81 chunks were `DRAW_RECORDED`, 60/81 were rejected by frustum planes. Projected bounds/reasons are in the CSV. No false-rejection witness is present. | Culling is active; causality UNKNOWN |
| Camera below terrain | 2049-grid/LOD0 triangle estimate: 1790.10 m ground vs camera Z 1778 m. Downward rays may point into the terrain interior and miss its surface. | LIKELY explanation; DEM/mesh inference, not GPU depth |
| Other geometry/projection cause | Raw depth and a matching product-window capture are absent. | UNKNOWN |

### Depth and coverage

The pass clears D32 depth to `0.0`; terrain depth testing/writes use Reverse-Z `GreaterOrEqual`. Thus exact clear-color pixels are a strong binary proxy for depth remaining at 0, and non-clear terrain pixels indicate a terrain color/depth write in this terrain-only pass. `depth-occupancy-proxy.png` records this binary classification (white = inferred terrain depth written, black = clear depth). It is **not a raw D32 readback**. The attachment is created with `DepthStencilAttachmentBit` usage and `StoreOp=DontCare`; the existing readback path copies only the color attachment. Raw numeric per-pixel depth was unavailable without changing renderer code, which was outside this task's ownership.

## Source locations

- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Commands.cs:72-74` — ClearColor `(0.35, 0.55, 0.80, 1)` and depth clear `0`.
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.Depth.cs:9-14` — Reverse-Z `GreaterOrEqual` depth state.
- `XuanYu.Render.Vulkan/Render/VulkanDepthAttachment.cs:10, 17-23` — D32 depth image; image usage is depth attachment only.
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Resources.cs:22-30` — depth attachment `StoreOp=DontCare`.
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Readback.cs:47-66` — existing readback copies color aspect only.
- `XuanYu.Render.Vulkan/Render/Terrain/VulkanClearFrameOwner.Terrain.cs:51-75` — per-chunk frustum rejection and recorded terrain draw.
- `XuanYu.Render.Vulkan/Shaders/terrain.frag:4-6` — terrain fragment color is green, distinct from clear RGB.
- `XuanYu.Editor.UI/Vm/Workspace/TerrainRenderProjectionAdapter.cs:8-35` and `XuanYu.Render.Abstractions/Terrain/Build/TerrainChunkMeshBuilder.cs:25,49-54` — source-to-2049 sample mapping and LOD0 triangle construction used for the camera-ground estimate.
- `tools/xyc-offscreen/B3TrajectoryCaptureRunner.cs:49` — B3 offscreen projection has no scene objects and includes only terrain.

## Reproduction

Run the offline analyzer from the repository root (Python with Pillow required). Choose a new output directory because the tool refuses to overwrite evidence:

```powershell
python tools/xyc-offscreen/b4-evaluation/a-coverage/analyze_a_coverage.py `
  --png artifacts/diagnostics/xyc-b3/20261015-202400/trajectory-sse-4/near-oblique.png `
  --terrain-log artifacts/diagnostics/xyc-b3/20261015-202400/trajectory-sse-4/probes/terrain.log `
  --captures-json artifacts/diagnostics/xyc-b3/20261015-202400/trajectory-sse-4/captures.json `
  --hgt artifacts/diagnostics/wp2-p4-r3/n24e121.hgt `
  --out artifacts/diagnostics/xyc-b4/a-coverage/<new-run-id>
```

No Vulkan reproduction was run: the saved GPU image, same-frame chunk probes, and tracked render-pass source were sufficient to classify the pixel color. No B3 GPU benchmark was rerun.

## Diagnostic artifacts

`artifacts/diagnostics/xyc-b4/a-coverage/20261015-b3-existing-offline-verified/` contains:

- `coverage-summary.json` — pixel counts, percentages, and chunk counts.
- `terrain-coverage-mask.png` — white non-clear (terrain-colored), black clear RGB.
- `depth-occupancy-proxy.png` — binary depth occupancy proxy, not raw depth.
- `chunk-visibility.csv` — all 81 chunk decisions with LOD and probe-space projected bounds for camera revision 3.
- `chunk-visibility-grid.png` — green recorded draws, red culls, chunk coordinates in a 9×9 chunk grid.
- `camera-ground-estimate.json` — DEM/LOD0 triangle estimate at the captured camera XY.

The `20261015-b3-existing-offline-verified` directory is the authoritative completed analysis output; earlier `20261015-b3-existing-offline*` directories were intermediate script iterations.

Inputs retained at `artifacts/diagnostics/xyc-b3/20261015-202400/trajectory-sse-4/`; source PNG SHA-256 is `601AB7B95540D2687D0443A69F224BBC9CCD197BFA89B7EBC63D68DF4712F791`.

## Residual uncertainty

1. B3 `runtime.json` says HEAD `9429be…`, while the task identifies B3 as `10af215…`; the discrepancy is unresolved.
2. The offscreen image does not prove what the interactive product viewport, its sky path, or a corresponding presented frame showed.
3. No per-pixel D32 values were retained/read back, and there is no same-pose culling-disabled or near-plane A/B evidence. The underlying cause of missing terrain-colored pixels remains unknown.

Changed: added only this analyzer and report under `tools/xyc-offscreen/b4-evaluation/a-coverage/`; diagnostics are only under `artifacts/diagnostics/xyc-b4/a-coverage/`. No product rendering files or shared configuration changed. No commit/push. Existing Foreign Dirty files remain untouched.

```text
XYK CANDIDATE
Candidate: For a terrain-only Vulkan offscreen pass, an exact match to the clear RGB plus binary reverse-Z depth semantics can identify uncovered pixels as clear/background, but it cannot identify why terrain was absent; numeric depth and product defect claims require matching runtime evidence.
Evidence: XYC-B4-A-COVERAGE-FORENSICS, 2026-10-15; B3 saved PNG and terrain probes; clear/depth attachment source.
Disposition requested: coordinator review and deduplication; no direct XYK write.
```
