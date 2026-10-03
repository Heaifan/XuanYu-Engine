# XYUI Test Truth Remediation — 2026-09-30

- Renamed XYUI test classes/files that claimed Runtime, Fidelity, VisualState, Visual, or Real while executing Headless/property/structure assertions to explicit HeadlessContract, StateContract, PropertyContract, StructureContract, or TokenContract names.
- Updated Headless test host comments to state that synthetic pointer, style, layout, and lifecycle evidence is not desktop or pixel-rendering acceptance.
- Mutation witness for `XYMenu.Close`: baseline focused test passed; temporary `IsOpen = true` mutant failed with expected `Assert.False` failure; restoring `IsOpen = false` passed. Production mutation was restored and left clean.
- Final full XYUI suite after remediation: 706 passed, 0 failed, 0 skipped. Visual fidelity, desktop usability, and pixel correctness remain capability gaps.
- Full-suite ordering showed an intermittent existing Headless failure in `XYUI3FinalNavigationTests.Bottom_navigation_primary_action_floating_hit_target_does_not_change_destination`; focused rerun passed and a later full rerun passed. Do not hide this kind of flake by weakening the oracle.
