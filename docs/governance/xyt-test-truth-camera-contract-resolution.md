# XYT-T2 / PRE-CLOSEOUT / A-CURSOR-CONTRACT

Scope: `XuanYu.World.Tests/Viewport/InputIntegration/CursorAnchoredZoomRegressionTests.cs`  
Decision: `RETIRE`  
Camera Truth status: `KNOWN CONTRACT CONFLICT = 0`

## Evidence review

The current approved Camera Truth record is:

```text
Camera contract: Wheel Dolly preserves the ObservationCenter invariant.
Cursor-anchor behavior: not a current approved contract.
```

Supporting records:

- `docs/governance/xyt-test-truth-registry.json` identifies the Camera truth
  record as `camera.frame-all.observation-center` and explicitly says it does
  not prove cursor-anchor zoom.
- `docs/governance/xyt-test-truth-wave-t-a-report.md` records Camera as
  `ObservationCenter only` and Cursor Anchor as `NOT CLAIMED`.
- `docs/governance/xyt-t2-final-closeout-r3-report.md` classifies the committed
  cursor test as `CONTRACT-CONFLICT`, with `Contract changed: NO`.
- The P1-FIX5 version event is marked `PROVISIONAL`; it is not approval of a
  replacement Camera Truth contract.

The repository therefore contains implementation/provisional evidence for a
cursor-aware path, but no approved contract replacing ObservationCenter. That
is insufficient to retain a cursor-anchor regression test as current truth.

## Retired claim

The deleted test file asserted that perspective, orthographic, and terrain
wheel operations preserve a reference-plane point at the cursor. Its Claim
was cursor-anchor behavior. Its Oracle was screen-pixel anchor drift. This is
an internally coherent test, but it protects an unapproved contract and must
not remain as a current PASS/FAIL Camera contract.

The associated `.Helpers.cs` partial contains no `[Fact]` or `[Theory]`
definitions after this retirement; it is not a Truth test and does not create
a Cursor Anchor result.

## Reconciliation

```text
Decision: RETIRE
Product behavior modified: NO
Cursor Anchor test result: NONE / RETIRED
Current Camera contract: ObservationCenter invariant
KNOWN CONTRACT CONFLICT: 0 for the scoped World.Tests inventory
World.Tests scoped status: GREEN after removal of the unapproved test claim
Verification: `XuanYu.World.Tests` 2156/2156 PASS; the retired file's seven
runtime cases are no longer part of the current scoped execution set.
```

No central Truth Registry entry was added or modified. A future Cursor Anchor
test requires an explicit contract decision first; it must not be inferred
from provisional implementation evidence or from the retired test itself.

请 ChatGPT 审计并沉淀知识经验。
