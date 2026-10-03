# Vulkan Draw Contract convergence

- In XuanYuEngine on `feat/v0.3-world-authoring-r1`, a Vulkan draw kind must resolve to one logical owner before readiness, binding, and dispatch consume it. This prevents Main-pipeline readiness from certifying Terrain and prevents Vector Overlay fallback to Main.
- Terrain recordability must depend on `_terrainPipeline` plus `_terrainPipelineLayout`; Vector Overlay recordability must depend on its dedicated fill and stroke pipelines/layouts. Missing dedicated pipelines are non-recordable and diagnostic, never fallback.
- Unknown `RenderDrawKind` must throw an explicit `ArgumentOutOfRangeException`; ordinal/range routing and `DrawAssist` fallback are forbidden.
- Tests that assert source literals such as `draw.Kind == ...` or absence of a type name are implementation-coupled oracles. Prefer testing owner resolution, readiness matrices, draw-plan semantics, explicit unknown-kind failure, and real handler/command evidence.
- This lane reached fresh automated evidence: Vulkan build 0 warnings/0 errors, Core 461/461, World 2159/2159, targeted Core 20/20 and World 12/12, `git diff --check` pass. It remained RED for 5+100 because `VulkanClearFrameOwner.Draw.cs` was 193 lines; do not compress or move logic into `DrawAssist` to hide the violation. No commit/push/version-source mutation.
