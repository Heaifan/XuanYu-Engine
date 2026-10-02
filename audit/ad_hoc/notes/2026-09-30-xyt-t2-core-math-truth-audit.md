# XYT-T2 Core Math Truth Audit

- Scope was read-only `E:\MyDoc\project-VSCode\XuanYuEngine`, `XuanYu.Core.Tests/**/*.cs`; 88 files and 362 Fact/Theory attributes were reviewed; full Core test execution was 449/449 PASS.
- A green unit suite is Tier-2 execution evidence only. It does not prove GPU depth behavior, Vulkan runtime rendering, or real large-world precision.
- `Camera\ReverseZProjectionPrototypeTests.cs` duplicates projection formulas and their inverse locally; treat as implementation-mirror oracle and retire rather than use for production truth.
- Matrix round-trip tests prove consistency/invertibility only; they must not be used as sole proof of projection correctness, RenderOrigin precision, or coordinate-chain correctness.
- RenderOrigin coverage at 50 km proves relative-origin selection and a local round-trip, but not measurable large-world precision improvement; require an independent absolute-vs-relative error comparison at materially large coordinates.
- Shader/pipeline source-string tests prove source contracts only; they must be named and tiered as source-contract evidence and escalated when the claim is actual GPU depth or runtime rendering.
- LOD hysteresis requires boundary perturbation across the threshold and repeated alternating samples; one repeated identical input is not a stability proof.
- Critical capability review result: camera, projection, coordinates, picking, frustum, and LOD have math/integration evidence; render-origin is weak and reverse-Z GPU depth remains explicitly unproven, so those gaps must be ESCALATE rather than silently promoted to PASS.
- Final remediation preserved the separation: eight implementation-mirror prototype facts were retired; projection and orthographic inverse claims now use known screen/extent expectations; LOD tests cross a real hysteresis sequence and isolate chunk/revision state; CPU depth and shader checks were renamed to CPU/source-contract truth.
- A billion-scale RenderOrigin fixture now verifies absolute world identity, camera-relative coordinates, and screen placement with independent expectations. This remains math evidence only; Vulkan/GPU large-world and depth runtime behavior stay ESCALATE.
- Cursor-anchor contract resolution: the committed cursor-anchor regression test was not an approved replacement for the ObservationCenter wheel-dolly contract; retire the seven-case test rather than changing Camera behavior. Provisional implementation evidence does not establish a current product contract.
