# XYT-T2 T-A-FIX Terrain context lifecycle

- Product root cause: `EditToolsModule.axaml` bound visibility to `IsRegionEditMode`, while `IsRegionEditMode` ignored the authoritative `IsRegionContext`; `EnterTerrainContext` also failed to notify the derived property. Region host stayed at 432 DIP after Terrain activation.
- Minimal fix: define `IsRegionEditMode` as `IsEditMode && IsRegionWorkspace && IsRegionContext`, and raise `PropertyChanged(nameof(IsRegionEditMode))` with existing terrain-context bindings. Do not add a parallel context boolean.
- Same-test evidence: pre-fix `Terrain_context_collapses_the_hidden_region_host_slot` failed Expected 0 / Actual 432; post-fix passed. Terrain scoped 145/145 and Mode/Context/Workspace lifecycle 267/267 passed; Editor.UI build 0W/0E; ARCH-A and 5+100 passed.
- Product Fix Event uses next single-source version `v0.3.0.3-fix`, ledger status `PROVISIONAL` with `UNCOMMITTED`; no Commit/Push until Final Convergence. Window title derives from Assembly InformationalVersion.
