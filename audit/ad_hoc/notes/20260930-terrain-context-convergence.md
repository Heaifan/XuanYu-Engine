# Terrain convergence audit — 2026-09-30

- `Terrain_context_collapses_the_hidden_region_host_slot` reproduced as a stable product-side failure: `RegionEditor` remains active and `EditToolsModule` binds visibility only to `IsRegionEditMode`, so entering Terrain context leaves the 432 DIP region host visible.
- The Terrain-owned test harness was not the root cause; no Expected value was weakened. Product repair is outside the Terrain test ownership boundary and must be escalated as BLOCKED.
- Terrain viewport assertions should require non-empty `TerrainResources`, build projection from `CurrentViewport`, and assert against the current logical dimensions; fixed 800x600 is a misleading oracle.
- Historical regression witness remains unavailable unless the same test is actually executed pre-fix as RED and post-fix as GREEN. A passing post-fix test alone is not a historical witness.
