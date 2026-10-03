# SRP-GRAND-R1-B World Terrain Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Establish World-owned typed surface/elevation results and split Terrain storage, lookup, query, revision, and render metadata without introducing a Render dependency.

**Architecture:** World owns query status/result types. TerrainWorld delegates storage and elevation query to focused domain collaborators while retaining compatibility entry points. TerrainTileSet becomes a facade over storage, lookup, and query collaborators. Projection remains outside World.

**Tech Stack:** C# / .NET 10 / xUnit.

**Spec:** User task `SRP-GRAND-R1-B-IMPLEMENT`.

## Global Constraints

- WorldPosition<double> remains World authority.
- RenderOrigin remains Render-only.
- Query failure is INVALID and never a successful Z=0 fallback.
- No universal WorldDirtyState.
- XuanYu.World must not reference Render.Abstractions or Vulkan.
- Preserve unrelated dirty and untracked workspace material.
- No commit or push in this implementation turn.

## Review Focus

- NoData must remain distinguishable from out-of-bounds.
- No terrain must remain distinguishable from invalid binding.
- Zero elevation must remain a valid elevation.
- Terrain revision must change on edit but not be Region identity.
- World project references must remain free of Render.Abstractions.

### Task 1: World-owned query result contracts

**Files:**
- Create: `XuanYu.World/WorldQueryStatus.cs`
- Create: `XuanYu.World/ElevationQueryResult.cs`
- Create: `XuanYu.World/SurfaceQueryResult.cs`
- Test: `XuanYu.World.Tests/Terrain/WorldQueryResultTests.cs`

- [ ] Write failing tests for all five statuses and valid zero elevation.
- [ ] Run the focused test and observe the expected missing-type failure.
- [ ] Implement immutable World-owned result types with explicit status and value access.
- [ ] Run the focused test and verify green.

### Task 2: TerrainWorld storage/query/revision/render metadata split

**Files:**
- Create: `XuanYu.World/Terrain/TerrainWorldStorage.cs`
- Create: `XuanYu.World/Terrain/TerrainElevationQuery.cs`
- Create: `XuanYu.World/Terrain/TerrainRevision.cs`
- Create: `XuanYu.World/Terrain/TerrainRenderMetadata.cs`
- Modify: `XuanYu.World/Terrain/TerrainWorld.cs`
- Test: `XuanYu.World.Tests/Terrain/TerrainWorldAuthorityTests.cs`

- [ ] Write failing tests for typed query status, revision ownership, and render metadata isolation.
- [ ] Run the focused test and observe the expected missing collaborator/API failure.
- [ ] Implement collaborators and delegate from TerrainWorld without Render references.
- [ ] Run focused TerrainWorld tests and verify green.

### Task 3: TerrainTileSet storage/lookup/query split

**Files:**
- Create: `XuanYu.World/Terrain/Source/TerrainTileStorage.cs`
- Create: `XuanYu.World/Terrain/Source/TerrainTileLookup.cs`
- Create: `XuanYu.World/Terrain/Source/TerrainTileQuery.cs`
- Modify: `XuanYu.World/Terrain/Source/TerrainTileSet.cs`
- Modify: `XuanYu.World/Terrain/Source/TerrainElevationTile.cs`
- Modify: `XuanYu.World/Terrain/Source/ITerrainElevationQuery.cs`
- Test: `XuanYu.World.Tests/Terrain/TerrainTileSetAuthorityTests.cs`

- [ ] Write failing tests for typed status propagation and separated lookup/query behavior.
- [ ] Run the focused test and observe the expected contract failure.
- [ ] Implement storage, lookup, and query collaborators while preserving existing TileSet behavior.
- [ ] Run all Terrain tests and verify green.

### Task 4: Surface authority and projection boundary verification

**Files:**
- Modify: `XuanYu.World/Map/WorldMapState.cs`
- Modify: `XuanYu.World/Map/WorldMapStateOwner.cs`
- Modify: `XuanYu.Editor/MapEditing/TerrainWorldGroundSurface.cs`
- Test: `XuanYu.World.Tests/MapEditing/SurfaceQueryAuthorityTests.cs`
- Test: `XuanYu.World.Tests/Architecture/WorldRenderDependencyBoundaryTests.cs`

- [ ] Write failing tests for no-terrain, out-of-bounds, invalid binding, and typed terrain failures.
- [ ] Run the focused tests and observe the expected API failure.
- [ ] Add typed query entry points and keep old adapters as compatibility wrappers.
- [ ] Run World tests, architecture tests, diff checks, and solution build through the repository resolver.

## Completion Gate

Return only `HANDOFF_READY` when implementation, focused tests, architecture boundary, solution build, and scope checks pass. Return `BLOCKED` with evidence otherwise.
