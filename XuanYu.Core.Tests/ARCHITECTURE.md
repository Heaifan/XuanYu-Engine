# XuanYu.Core.Tests — 5×5×5+100 navigation

All C# source files in this project are test-side source files; their file names start with `test_`. Test class names, method names, namespaces and source contents are preserved.

Rules:
- At most 5 first-level responsibility directories.
- At most 5 immediate child directories in every responsibility directory.
- At most 5 `.cs` files directly in any directory.
- At most 3 responsibility directory levels beneath `XuanYu.Core.Tests`.
- At most 100 physical lines per `.cs` file.
- Tests remain in the separate `XuanYu.Core.Tests` project; none are moved into production `XuanYu.Core`.
- The SDK-style `.csproj` uses recursive default inclusion; no explicit `Compile Include` paths were changed.

## First-level test domains

| Folder | Responsibility |
| --- | --- |
| `Diagnostics` | Core smoke checks, terrain replay, viewport probe |
| `Scene` | History, undo/redo, editor transform capture and scene snapshot preview guards |
| `Interaction` | Move/rotate/scale Gizmo behavior, viewport picking |
| `Space` | Camera navigation, projection, geometric spatial queries, terrain SSE, scale truthfulness |
| `Render` | Rendering geometry, draw plans, terrain surfaces, overlays, Vulkan and runtime contracts |

## Third-level finder

- `Diagnostics/Terrain/Replay`, `Viewport/Probe`, `System`: replay, viewport probe and smoke tests.
- `Scene/History`, `Transform`, `Snapshots`: history, transform and render snapshot preview guards.
- `Interaction/Gizmo/MoveDrag`, `MoveLayout`, `Rotate`, `Scale`; `Picking`: manipulators and picking.
- `Space/Camera/Basis`, `Navigation`, `Ui`, `Orbit`: camera state and navigation.
- `Space/Projection/ReverseZ`, `ViewState`: depth and projection.
- `Space/Spatial/Geometry`, `Rays`; `Terrain/Mesh`, `Sse`; `Viewport/Scale`: geometric and diagnostic checks.
- `Render/Grid/Geometry`, `Metrics`, `Lifecycle`: grid and scale contracts.
- `Render/Draw/Projection`, `Frame`, `Viewport`: rendering draw plans.
- `Render/Map/Geometry`, `Resources`, `Horizon`: map surface rendering.
- `Render/Overlay/NavLayout`, `NavOverlay`, `Scale`: navigation overlay and scale.
- `Render/Runtime/Models`, `Pipeline`, `View`: rendering execution and device contracts.

Migration: test file basename prefix and directories changed, original test blobs are identical. The full .NET test suite and any path-sensitive test discovery/integration have NOT yet been executed. Do not label this migration Integration PASS without real execution evidence.
