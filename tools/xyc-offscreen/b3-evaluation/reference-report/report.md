# XYC-B3 offline evaluation

Input: `tools\xyc-offscreen\b3-evaluation\b3-input.json`

All SSE thresholds with absent runs remain **NOT RUN**; B2 values are historical reference only and are never copied into SSE rows.

## Configuration status

| Configuration | Family | Status | Evidence | Independent processes | GPU P50 median ms | Final mesh P95 m |
|---|---|---|---|---:|---:|---:|
| 1025-auto | B2_REFERENCE | REFERENCE_ONLY | MEASURED_IN_B2_NOT_B3 | 5 | 6.513376 | None |
| 2049-auto | B2_REFERENCE | REFERENCE_ONLY | MEASURED_IN_B2_NOT_B3 | 5 | 21.983552 | None |
| 2049-chunk240-lod0 | B2_REFERENCE | REFERENCE_ONLY | MEASURED_IN_B2_NOT_B3 | 5 | 23.899776 | None |
| 2049-chunk480-auto | B2_REFERENCE | REFERENCE_ONLY | MEASURED_IN_B2_NOT_B3 | 5 | 23.891808 | None |
| 2049-chunk480-lod0 | B2_REFERENCE | REFERENCE_ONLY | MEASURED_IN_B2_NOT_B3 | 5 | 26.114752 | None |
| 2049-lod-bias-1 | B2_REFERENCE | REFERENCE_ONLY | MEASURED_IN_B2_NOT_B3 | 5 | 5.571648 | None |
| 2049-lod0 | B2_REFERENCE | REFERENCE_ONLY | MEASURED_IN_B2_NOT_B3 | 5 | 24.352608 | None |
| SSE-0.5px | SSE | NOT RUN | NOT RUN | 0 | None | None |
| SSE-1px | SSE | NOT RUN | NOT RUN | 0 | None | None |
| SSE-2px | SSE | NOT RUN | NOT RUN | 0 | None | None |
| SSE-4px | SSE | NOT RUN | NOT RUN | 0 | None | None |

## Figures

- [GPU cost and triangles](gpu-cost.svg)
- [Eligible final-mesh Pareto](pareto-final-mesh.svg)
- [Separated error layers](error-layers.svg)

## Interpretation limits

- GPU frame samples are nested within a process. Independent-process summaries use one process statistic per process; pooled frame samples are not independent process replications.
- Source HGT→heightfield reconstruction error is not final rendered mesh error. The three error fields are separate and must share a documented world-XY sample protocol before comparison.
- `estimatedGpuBytes` is an estimate, not measured resident VRAM. Device-wide `nvidia-smi` memory is not process attribution.
- Pareto eligibility requires measured SSE status and final mesh error; derived estimates and B2 reference rows are excluded.
- SSE configurations listed: SSE-0.5px, SSE-1px, SSE-2px, SSE-4px.
