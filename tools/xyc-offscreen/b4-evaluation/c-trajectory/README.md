# XYC-B4-C-CAMERA-TRAJECTORY

Independent continuous-motion evidence runner. It consumes the B3 `n24e121` HGT and SSE error table, uses the same 2049 sample heightfield, 240 cell chunks, 1024×768 offscreen Vulkan target, camera keyframes, and terrain coverage color as B3. It does not change production LOD policy or camera code.

## Prepare and run

```powershell
./tools/xyc-offscreen/b4-evaluation/c-trajectory/Invoke-C-Trajectory.ps1
```

This restores/builds only the runner project and does not start Vulkan. After the shared RTX 3060 slot is released, run:

```powershell
./tools/xyc-offscreen/b4-evaluation/c-trajectory/Invoke-C-Trajectory.ps1 -ExecuteGpu
```

The default run captures only `baseline` (production Auto LOD) and `sse-4`. Use `-Variant baseline` or `-Variant sse-4` for a single lane. Each execution gets a new timestamped root under `artifacts/diagnostics/xyc-b4/c-trajectory/`; existing roots are never overwritten.

## Trajectory and evidence

`trajectory.json` is fixed input. Frames use deterministic linear position/scalar interpolation, normalized forward/up interpolation with orthogonalization, and fixed 70 degree pivot orbit. It includes 20 km → 2 km → near-ground, continuous zoom in/out, orbit, chunk entry across the B3-observed x=47,482.917 m boundary into chunk (4,4), near-plane pressure, and A→B→A return. Source hash, viewport, sampling, chunks, and interpolation policy are explicit.

Each variant writes a `frames.json`, one PNG per frame, process log, and chunk probes. A frame's unique CameraRevision links camera state, terrain chunk visibility/LOD, triangles, GPU timestamp, coverage mask, and PNG SHA. The analyzer writes paired PNG diffs, `frame-comparisons.json`, and `C-TRAJECTORY-REPORT.md`. GPU timestamps are per-frame observations; do not infer B3-style benchmark statistics from them.

Track PASS requires nonempty geometry and coverage with available per-frame GPU timestamps and no extreme adjacent coverage discontinuity. Empty geometry/coverage is FAIL. Missing timestamps or an extreme coverage jump is INCONCLUSIVE and requires reviewing the retained frame sequence. LOD transitions are reported as evidence, not automatically treated as popping. A→B→A additionally requires exact camera and PNG return for Auto LOD.

## B3 evidence review notes

B3 provides eight fixed-pose captures per configuration, 2049 samples, 240 cell chunks, fixed `n24e121` SHA, and same-pose PNG comparisons. Its trajectory frames are not a continuous motion replay. The checked-in runtime manifests identify their build as `HEAD=9429be6…;DIRTY=YES`, while the requested B3 handoff HEAD is `10af215…`; reuse the B3 poses and parameters, but treat runtime lineage as unconfirmed. B4 records its actual HEAD and dirty state at execution.

The local `n23e121.hgt` file is present, but this runner's camera coordinates and hash guard are intentionally pinned to B3 `n24e121`; the existing B3 harness imports exactly one tile. A multi-tile stitched scene is also not part of the current harness. Therefore n23e121 and stitching remain documented coverage gaps rather than silently reusing mismatched world coordinates.
