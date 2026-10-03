# XYE-SRP-SLIMMING-R1-0-E-TEST-OWNERSHIP

- Scope: read-only audit of `C:\Users\Heai\.codex\worktrees\e598\XuanYuEngine` at `a29c47f2` (`origin/main`), 2026-10-01 Asia/Shanghai.
- The checkout has one test project, `XuanYu.Engine.Tests`, with 74 C# test files; it does not contain separate `Core.Tests`, `World.Tests`, `XYUI Tests`, or `Render Tests` projects.
- Current path counts are Architecture 2, Bridge 1, Core 9, root smoke 1, Editor 19, Engine/World 2, Project 5, Render 35.
- No XYUI test project, XYUI namespace, or XYUI test file was found in this checkout. Do not infer XYUI ownership from the absent category.
- Core tests under `XuanYu.Engine.Tests\Core` are the cleanest future `Core.Tests` candidates. `CoreSmokeTests` is also Core-only; Architecture tests are governance/infrastructure, not Core.
- World state/position tests are currently under `Engine\World`, but `WorldToRenderSceneBuilderTests`, `WorldHierarchyTreeBuilderTests`, and `ProjectContentWorldSeederTests` cross World with Render, Editor, or Bridge/Project and should remain boundary/integration tests unless split by assertion responsibility.
- Render tests are mostly Render-owned, but `PerspectiveOrthographicPickingTests`, presented/pointer picking tests, and `WorldToRenderSceneBuilderTests` exercise multiple Render sub-responsibilities or World→Render boundaries. They should not be used as evidence for a single lower-level unit contract.
- Strongest multi-domain test candidates requiring future separation or explicit integration ownership: `ProjectContentWorldSeederTests`, `TransformDragRouteTests`, `WorldHierarchyTreeBuilderTests`, `WorldToRenderSceneBuilderTests`, and `PerspectiveOrthographicPickingTests`.
- Future migration rule: classify by the production responsibility directly asserted, not by directory name or the broadest namespace imported. Keep one test method focused on one responsibility; use a separately named integration suite for intentional boundary proofs.
- No product/test files were modified, deleted, or added by the audit. The only persistent artifact is this memory note, explicitly requested as knowledge sedimentation.
