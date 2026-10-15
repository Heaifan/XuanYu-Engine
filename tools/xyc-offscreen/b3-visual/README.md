# XYC-B3 deterministic visual trajectory

`trajectory.json` defines fixed world-space camera poses for the `n24e121` test terrain. Coordinates are captured from the known B2 pose/evidence domain; they must not be recomputed from mesh sample density or an AutoFrame operation. The B2 near-oblique anchor uses the camera recorded in the B2 run manifest. All poses use explicit camera fields and a fixed terrain SHA-256.

## Trajectory coverage

- `top-down`: fixed nadir view.
- `near-oblique`: B2 near-oblique anchor pose.
- `high-relief-sensitive`: low oblique field of view targeting the known high-relief observation region.
- `chunk-entry`: low view at the fixed terrain center to inspect chunk boundary/frustum entry evidence.
- `near-plane-stress`: near-ground forward view with the explicitly fixed 0.05 m near plane.
- `sequence-A-before`, `sequence-B`, `sequence-A-return`: ordered zoom/orbit-style camera-state transitions with exact return to A. Apply the explicit poses in order; do not invoke AutoFrame between steps. Capture each after a presented frame.

The trajectory is an input contract for the Parent-owned XYC runner. This subtask does not add a renderer, launch the app, exercise the GPU, or claim Vulkan output. In particular, the high-relief and chunk-entry names identify intended stress views; their actual terrain coverage and chunk membership must be confirmed by captured runtime evidence.

## Capture bundle contract

The external runner supplies a directory containing `captures.json` and one PNG per capture. Minimal schema:

```json
{
  "schema": "XYC-B3-CAPTURES/1",
  "terrainSha256": "BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1",
  "captures": [
    {
      "id": "top-down",
      "image": "top-down.png",
      "imageSha256": "UPPERCASE_SHA256",
      "camera": {
        "position": {"x": 50648.444336988396, "y": 55660, "z": 12000},
        "forward": {"x": 0, "y": 0, "z": -1},
        "up": {"x": 0, "y": 1, "z": 0},
        "fovDegrees": 45,
        "nearPlane": 0.1,
        "farPlane": 445280
      }
    }
  ]
}
```

Include every capture ID from `trajectory.json`. Values must be observed after the frame is presented, and image paths must stay within the bundle. Keep the raw runner logs, app/build identity, viewport dimensions, frame/camera revisions, GPU/device identity, and capture acknowledgements alongside the bundle. Those raw provenance fields are required for a real Vulkan claim even though the analyzer only needs the camera, terrain hash, PNGs, and PNG hashes.

## Analyze and difference images

```powershell
& tools/xyc-offscreen/b3-visual/Invoke-B3-Analyze.ps1 -Bundle <Parent-produced-bundle>
```

The analyzer checks required captures, terrain identity, image hashes/dimensions, fixed camera values, and exact A-to-B-to-A camera return (camera revision may advance). It writes `analysis.json` and `diff-sequence-A-before-vs-A-return.png`; a zero pixel difference is strong repeatability evidence for identical pose/state, while a nonzero diff needs interpretation against frame timing and runtime state. It does not assert subjective visual quality.

## Report template

Copy `report-template.md` beside the capture bundle and fill it from actual outputs. Keep unrun or unavailable checks marked `UNKNOWN`/`NOT RUN`; do not infer GPU behavior from the offline analyzer.
