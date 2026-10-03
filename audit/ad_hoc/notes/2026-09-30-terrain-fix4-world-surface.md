# P1-FIX4 Terrain runtime visibility

- Task: P1-FIX4-TERRAIN-RUNTIME-VISIBILITY in `E:\MyDoc\project-VSCode\XuanYuEngine`.
- Evidence: old `RenderDrawPlan` emitted both `Terrain` and `MapGround` when a projection carried terrain plus a stale/visible map snapshot; the new regression witness was RED before the fix and GREEN after it.
- Fix: `RenderDrawPlan` now treats Terrain as the World Surface and suppresses `MapGround` while retaining Terrain, MapBounds, and overlay ordering. Continuity tests validate DrawPlan Terrain presence, valid-camera frustum chunks, and Region authoring draft counts 0→1→2.
- Gates: World.Tests 2163/2163 PASS; solution build 0 warnings/0 errors; scoped 5+100 and `git diff --check` PASS. ARCH-A remained blocked by pre-existing ForeignDirty `XuanYu.Editor/Camera/CameraNavigation.Try.cs` (139 lines). T3/T4 real Vulkan visual acceptance was not run.
- Candidate reusable lesson: visibility must be enforced at the render-plan/world-surface boundary, not inferred from UI context or resource non-emptiness; automated projection/frustum contracts do not close real Vulkan visual acceptance.
