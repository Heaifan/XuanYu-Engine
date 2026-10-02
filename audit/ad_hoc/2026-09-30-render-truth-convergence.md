# Render Truth Convergence / XYT-T2 / T-A-CONVERGENCE / D-RENDER

- `Terrain_stats_sum_selected_chunks` is a valid unit contract for `TerrainRenderStats.From(...)` aggregation. Its truth is `VERIFIED` and decision is `KEEP`; inability to detect Vulkan draw failure is a capability-coverage limitation, not a bad test.
- The nearest render-critical RED-sensitive test is `TerrainMultiTileRenderContractTests.Draw_plan_contains_one_entry_per_terrain_in_stable_order`: baseline PASS, controlled mutation suppressing `RenderDrawPlan` Terrain entries produced FAIL (expected 3, actual 0), restore PASS.
- Keep the Claim Ladder explicit: CPU data / stats and render-list tests do not claim command recording, submission, GPU execution, framebuffer, presentation, or user visibility.
- Source inspection found Terrain plan -> `DrawTerrain` -> `CmdDrawIndexed`, plus `WaitForFences`, `QueueSubmit`, and `QueuePresent`; without fresh real Vulkan runtime evidence for Instance/Device/Swapchain/CommandBuffer/Submit/Fence/Present, T3 remains GAP and T4 user-visible continuity remains GAP. Static/source-contract tests must not fill those gaps.
- For this task, the canonical workspace was dirty with unrelated user changes; preserve them, avoid commit/push, and obey the narrow Render/test ownership boundary.
