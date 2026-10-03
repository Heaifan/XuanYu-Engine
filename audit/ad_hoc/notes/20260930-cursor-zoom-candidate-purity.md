# Cursor-anchored zoom lane audit

- In the XuanYuEngine `feat/v0.3-world-authoring-r1` candidate, a passing Camera wheel test and Editor.UI build were insufficient evidence when the dedicated CursorAnchoredZoom regression test was deleted by ForeignDirty and the workspace contained unrelated dirty Vulkan/Core/World files.
- Cursor anchor resolution must explicitly implement `Terrain valid -> ReferencePlane valid -> ObservationCenter`; returning null immediately after an invalid Terrain query violates the product contract and silently falls back to ObservationCenter.
- Treat the candidate as blocked until the Owned regression oracle is restored by its owner, RED/GREEN is witnessed on the same test, and Candidate Tree purity is re-established. Do not restore/stash/reset foreign dirty material from an isolated lane.
