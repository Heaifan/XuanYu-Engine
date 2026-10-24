# XuanYu.Core — 5×5×5+100 navigation

Scope: `XuanYu.Core` production C# sources. Tests belong in `XuanYu.Core.Tests`, not here.

Rules:
- At most 5 first-level responsibility folders directly below `XuanYu.Core`.
- Every responsibility folder has at most 5 direct child folders.
- No C# file deeper than the third responsibility folder (e.g. `Space/Camera/Projection`).
- At most 5 C# source files directly in any responsibility folder, and at most 100 physical lines in each C# source file.
- Folder names describe responsibilities; do not add `Misc`, `Helpers`, or arbitrary numbered buckets.
- `XuanYu.Core.csproj`, `.gitkeep`, and this document are root metadata, not responsibility source files.

## First-level responsibilities

| Folder | Responsibility |
| --- | --- |
| `Foundation` | Contracts, mathematics, time, logging, assembly metadata |
| `Scene` | Scene records, render snapshots, map surface, edit history and transform state |
| `Interaction` | Gizmo manipulation, viewport picking and screen-point contracts |
| `Space` | Camera/projection/rays, terrain visibility and spatial queries |
| `Diagnostics` | Terrain replay and mesh/SSE analysis, viewport probes, internal self-checks |

## Search guide

- `Foundation/Contracts`: `EntityId`, `EngineError`, `EngineResult`.
- `Foundation/Math`, `Time`, `Logging`, `Metadata`: low-level value types and reporting.
- `Scene/Entities`, `Snapshots`, `Map`, `History`, `Transform`: authored-scene state and events.
- `Interaction/Gizmo/MoveDrag`, `MoveLayout`, `RotateDrag`, `RotateLayout`, `Scale`: edit manipulators grouped by behavior.
- `Interaction/Picking`, `Screen`: viewport picking and screen-point data.
- `Space/Camera/State`, `Projection`, `Rays`: view state, Reverse-Z and ray construction.
- `Space/Terrain/Bounds`, `Culling`, `Lod`: terrain bounds, frustum and LOD.
- `Space/Spatial/Bounds`, `Intersections`, `Queries`, `Raycast`: geometric queries and hits.
- `Diagnostics/Terrain/Replay`, `MeshError`, `Sse`: terrain diagnostic data and calculations.
- `Diagnostics/Viewport/Probe`, `Frames`: viewport probe capture and frame identity.
- `Diagnostics/System`: self-test and workspace path diagnostics.

Migration note: This layout groups existing files without changing their contents or C# namespaces. Directory location is not a namespace contract. The current `.csproj` uses SDK-default recursive `*.cs` inclusion. Runtime and test integration still require a build on a suitable Windows/.NET environment.
