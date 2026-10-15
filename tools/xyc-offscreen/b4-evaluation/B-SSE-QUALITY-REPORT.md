# XYC-B4-B-SSE-QUALITY-ANALYSIS

## Scope and evidence identity

Offline analysis only; no renderer, Vulkan, or RTX 3060 work was started. Input batch: `20261015-202400`; terrain: `n24e121.hgt`; source SHA-256: `BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1`.

The task identifies B3 HEAD `10af215…`, but captured manifests identify `HEAD=9429be…; B3=VIS; DIRTY=YES`. Results below are conditional on the supplied B3 artifacts; attribution to the claimed HEAD is **UNCONFIRMED** until Parent reconciles this provenance.

All image-level conclusions have `Coverage = UNCONFIRMED`; blue areas are not presumed valid.

## Configuration comparison

GPU numbers are medians across five independent process-level P50/P95 summaries. Error P95/Max is pooled over 3,113,501 visible-footprint source samples in the near-oblique pose; it is Source→final selected mesh, not a Render→LOD decomposition.

| Configuration | GPU P50 ms | GPU P95 ms | Triangle count | GPU P50 saved vs Auto | Source→final P95 m | Delta vs Auto | Source→final Max m | Near-oblique PNG |
|---|---:|---:|---:|---:|---:|---:|---:|---|
| Auto | 22.565 | 23.459 | 1,910,496 | 0.0% | 8.08 | +0.0% | 134.80 | RGB MAE 0.0; XOR 0.0% |
| LOD+1 | 5.568 | 6.269 | 486,256 | 75.3% | 14.38 | +77.9% | 146.33 | RGB MAE 1.2602013481987846; XOR 0.5181630452473959% |
| SSE 0.5 px | 20.576 | 21.539 | 1,746,654 | 8.8% | 7.84 | -3.0% | 134.80 | RGB MAE 0.0; XOR 0.0% |
| SSE 1 px | 20.349 | 21.353 | 1,746,654 | 9.8% | 7.84 | -3.0% | 134.80 | RGB MAE 0.0; XOR 0.0% |
| SSE 2 px | 17.960 | 18.950 | 1,500,030 | 20.4% | 8.29 | +2.6% | 134.80 | RGB MAE 0.0; XOR 0.0% |
| SSE 4 px | 12.907 | 13.872 | 1,092,062 | 42.8% | 11.96 | +48.0% | 134.80 | RGB MAE 0.0; XOR 0.0% |

## Error layers and quantile availability

- Source→Render (HGT→2049 render heightfield): P95 = 10.3703 m. Source sample array and sample count are absent from this package, so P50, P99, and Max cannot be computed.
- Render→LOD: P50/P95/P99/Max cannot be computed. The normalized input stores this layer as null; Source→final mesh summaries cannot be algebraically split into the two layers.
- Source→final selected mesh: P95 and Max are available per configuration from pooled visible-footprint source samples (n=3,113,501). P50/P99 are absent; no percentile interpolation is attempted.
- Chunk-record distributions in `error-statistics.json` are quantiles of 21 per-chunk P95/Max records, not source-sample error quantiles.

## Visible chunk hotspots

For SSE 4 px, the largest per-chunk P95 is chunk (6,8), selected LOD 2, 32.43 m; its max is 93.85 m. The largest visible max remains chunk (6,4), LOD 0, 134.80 m (chunk P95 12.31 m). These identify terrain grid regions only: x/y cell intervals are in `visible-chunk-hotspots.csv`; geographic lon/lat mapping is unverified.

Per-chunk summary distributions (21 visible chunk records; these are not pooled source-sample percentiles):

| Configuration | Chunk P95 records P50/P95/P99/Max m | Chunk Max records P50/P95/P99/Max m |
|---|---:|---:|
| Auto | 6.78/12.31/12.67/12.76 | 46.23/114.88/130.82/134.80 |
| LOD+1 | 11.54/20.89/26.75/28.21 | 56.60/145.37/146.14/146.33 |
| SSE 0.5 px | 6.78/11.87/12.22/12.31 | 44.56/115.08/130.86/134.80 |
| SSE 1 px | 6.78/11.87/12.22/12.31 | 44.56/115.08/130.86/134.80 |
| SSE 2 px | 7.33/12.31/12.67/12.76 | 46.23/115.08/130.86/134.80 |
| SSE 4 px | 8.79/28.21/31.58/32.43 | 63.02/114.88/130.82/134.80 |

## Screen pixels versus geometry

For the representative near-oblique PNG, Auto/SSE 0.5/1/2/4 report RGB MAE=0 and mask XOR=0, while their selected chunk LODs and pooled geometric errors differ. In the fixed trajectory, SSE 4 differs only at sequence-B in the stored metrics: changed pixels 0.063705%, RGB MAE 0.019146, mask XOR 0.003052%; seven other captures are exactly equal. This is pixel agreement for these captures, not proof of full terrain coverage or perceived quality.

## Performance–quality Pareto

Using GPU P50 and visible-footprint Source→final P95, the non-dominated points are SSE 1 px (20.349 ms, 7.84 m), SSE 2 px (17.960 ms, 8.29 m), SSE 4 px (12.907 ms, 11.96 m), and LOD+1 (5.568 ms, 14.38 m). Auto is dominated by SSE 1 in these supplied summary metrics. LOD+1 is the fastest, with a larger error and measurable image differences; it is not an SSE tier.

## Candidate tiers and next evidence

- **SSE 1 px — conservative candidate:** about 9.8% lower GPU P50 than Auto with no representative near-oblique pixel difference and a slightly lower observed pooled P95. Validate on additional views and provenance before any default discussion.
- **SSE 2 px — balanced candidate:** about 20.4% lower GPU P50; observed P95 is 2.6% above Auto, with only sequence-B pixel differences in the fixed trajectory. Suitable for a quality/performance trade-off study, not yet a product default.
- **SSE 4 px — aggressive candidate:** about 42.8% lower GPU P50 and 42.8% fewer triangles; pooled P95 rises 48.0% while Max is unchanged in this view. The sequence-B difference is small but nonzero. Retain as an opt-in test tier pending coverage and multi-terrain evidence.
- Next: reconcile run HEAD/dirty identity; retain paired raw per-sample Source→Render and Render→LOD residuals to compute P50/P95/P99/Max; verify geographic chunk mapping; rerun representative views/terrains with same-pose PNG metrics and validated terrain coverage. User visual acceptance remains separate.

## Changed / Verified / Residual Risk

**Changed:** added this offline analyzer, report, and machine-readable B4 outputs only. No product LOD, shared renderer, or product code changed.
**Verified:** analyzer executed; three JSON outputs parsed; source error-table SHA-256 matches the raw B3 artifact; `git diff --check` returned no diagnostics. No tests or GPU/runtime commands were run.
**Residual Risk:** B3 run identity does not match the task HEAD claim; quantile gaps, unconfirmed terrain coverage, and geographic chunk transform remain unresolved.

## XYK CANDIDATE

Candidate only; do not deposit directly: preserve pooled source-sample quantiles separately by error layer, and label per-chunk summary distributions so they cannot be mistaken for pooled terrain percentiles. Keep GPU image agreement distinct from geometric error and coverage evidence.

## Files

Machine-readable tables and Pareto data: `artifacts/diagnostics/xyc-b4/b-quality/`; source analyzer: `tools/xyc-offscreen/b4-evaluation/analyze_b4.py`. No product code or rendering pipeline was changed.
