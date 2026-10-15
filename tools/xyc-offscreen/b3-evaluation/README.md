# XYC-B3 performance evaluation

This directory holds only offline analysis tooling and normalized inputs/reports. It does not start a renderer or GPU run. Parent owns all GPU execution.

## Reproduce the B2 reference snapshot

From repository root, using Python 3.10+ standard library:

```powershell
python tools/xyc-offscreen/b3-evaluation/import_b2_reference.py `
  artifacts/diagnostics/xyc-b2/20261015-192609 `
  tools/xyc-offscreen/b3-evaluation/b3-input.json
python tools/xyc-offscreen/b3-evaluation/analyze_b3.py `
  tools/xyc-offscreen/b3-evaluation/b3-input.json `
  tools/xyc-offscreen/b3-evaluation/reference-report
```

Both commands refuse to overwrite existing paths. The B2 output is an explicitly historical reference. It is not B3 evidence. All four SSE rows are emitted as `NOT RUN`, with null timing/error fields. Delete neither source experiment nor prior report to rerun; choose a new output path.

## Input data dictionary (`xyc-b3-evaluation/v1`)

Top-level fields: `dataset` identifies source, device, build and code fingerprint; `b2Artifact` records the actual source path and manifest SHA-256; `pairing.baseline` names the reference config; `configurations[]` contains one row per algorithm/threshold.

| Field | Meaning / rule |
|---|---|
| `name`, `family`, `thresholdPx` | Stable config identity; use `SSE` family and 0.5/1/2/4 px thresholds for candidates. |
| `status`, `evidenceClass` | `NOT RUN`, `MEASURED`, `DERIVED`, `HYPOTHESIS`, or `REFERENCE_ONLY`; never infer a measurement from nulls. |
| `processRuns[]` | One object per independent process. Frame sample counts belong inside each process, not as independent replicates. Include `replicate`, actual interleave `order`, process-level frame P50/P95, valid frame count, triangles, visible chunks, draw calls and estimated bytes. |
| `meshError` | Keep `sourceToHeightfieldP95M`, `heightfieldToFinalMeshP95M`, and `finalMeshWorldP95M` separate, with common XY sampling protocol recorded externally or in an extension field. |
| `screenError` | Pixel estimate and validity/provenance; label estimates as estimates, never as a proven bound without GEO evidence. |
| `pixels` | Same-pose GPU-origin metrics such as mask XOR, RGB MAE, edge MAE and ridge-specific errors. |
| `sourceArtifact` | Path to the source manifest or raw evidence. Store its SHA-256 in evidence metadata. |

Pairing only uses same family and replicate ID. It records both run orders; inspect orders and environment snapshots before interpreting a paired ratio. For actual B3, add process-level records from Parent's B3 run artifact and replace the importer-produced NOT RUN entries only in a new input copy, preserving its source as evidence.

## B2 provenance and independent audit

Snapshot source: `artifacts/diagnostics/xyc-b2/20261015-192609/run-manifest.json`; SHA-256 is captured by the importer in `b2Artifact.rootManifestSha256`. Dataset HGT SHA-256: `BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1`. B2 reported 2049-auto GPU process-P50 median 21.984 ms and 1,910,496 triangles; 2049 LOD-bias-1 5.572 ms and 486,256 triangles. These are B2 measurements only. B2 sampled HGT-to-heightfield P95 of 6.552 m is not final rendered mesh error. Per-process frame samples are nested and cannot be represented as independent processes. `EstimatedGpuBytes` is an allocation estimate, not resident VRAM; device-wide `nvidia-smi` snapshots are not attributable to this process.

Planning's “4x and no quality loss” claim is not supported by the available B2 evidence: the cited near-oblique result is about 3.95x from rounded process medians (21.984/5.572), while image mask/RGB/edge proxies do not establish no quality loss, nor do they evaluate ridge-specific final mesh error. B2 has five independent processes per configuration, but its 30 timestamp frames inside a process are not 30 independent runs. This is a scoped evidence correction, not a STOP on unrelated B3 work.

## Outputs

The analyzer emits `analysis.json`, `processes.csv`, `summaries.csv`, `paired-ratios.csv`, Markdown report and three SVGs: GPU cost vs triangle count, eligible measured final-mesh Pareto, and supplied error layers. Pareto SVG deliberately contains only measured SSE configurations with a provided final mesh error and GPU process statistic; it is empty until valid B3 evidence is supplied.

Classification: raw process fields are `MEASURED` only when read from the source run manifest; medians/ratios are `DERIVED`; forecasts are `HYPOTHESIS`. No field can prove user visual acceptance or product default readiness.
