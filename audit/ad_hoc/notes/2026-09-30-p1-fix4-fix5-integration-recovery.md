# P1-FIX4/FIX5 integration recovery lessons

- In the shared `E:\MyDoc\project-VSCode\XuanYuEngine` checkout, FIX4/FIX5 could be isolated with exact staging while preserving a large pre-existing ForeignDirty wave. The integration commit was `a41c96fc` and remote equality was verified; do not interpret the remaining dirty tree as release-ready.
- FIX4 contract is structural and semantic: when World Terrain resources exist, `RenderDrawPlan` must emit Terrain and suppress MapGround while retaining MapBounds and assists. Non-empty TerrainResources is not visual acceptance; T3/T4 remain required.
- FIX5 must preserve both `EditorPointerEvent.Position` and `WheelDelta` through the consumer to cursor-anchored Dolly. Terrain is preferred, ReferencePlane is the fallback, and the old ObservationCenter-unchanged assertion was a wrong oracle. Perspective and Orthographic anchor drift tests must cover center/corners and repeated zoom in/out.
- Two independent product bugs require two consecutive FIX version events even when carried by one commit. The live ledger advanced from `v0.3.0.3-fix` through provisional `v0.3.0.4-fix` and `v0.3.0.5-fix`; historical reconciliation remains required.
- ARCH-A/5+100 failures can be caused by owned integration growth. Split partial production/test files without weakening the 100-line gate; re-run full build and gates after each split.
- A full build/test can pass product lanes while a ForeignDirty XYUI lane fails. Report the exact foreign failure separately; do not convert a local candidate PASS into Integration or Acceptance PASS.
- New untracked files appearing during a long validation run are state drift until ownership is proven. Preserve them and classify UNKNOWN/CONFLICT rather than cleaning or staging them.
