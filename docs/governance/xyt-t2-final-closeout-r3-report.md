# XYT-T2 / FINAL-CLOSEOUT-R3

Role: Coordinator / Sole Writer  
Workspace: `E:\MyDoc\project-VSCode\XuanYuEngine`

## FINAL-CLOSEOUT-SNAPSHOT

The first read-only freeze recorded:

```text
HEAD: a41c96fc33bf1712b04c14ecdbc4c5e0dccfcaa5
Remote: a41c96fc33bf1712b04c14ecdbc4c5e0dccfcaa5
Ahead/Behind: 0/0
Dirty porcelain rows: 269
Untracked files: 120
Snapshot status SHA-256: afbc6143f109da417ccc8ceb2e2eb89f685965709f9d72a834dab3b12499f52c
```

The subsequent read-only check returned 270 rows and SHA-256
`d3bf991651f35a7f6c99fb0496efe331cd420990d18a636fe25f5187f71f43df`.
Therefore the frozen snapshot was invalidated before final closeout. The two
states are not mixed below; no final tests were run after this drift.

All 120 untracked paths were captured in the freeze output. The initial
porcelain manifest is identified by the SHA-256 above; it is not replaced by
the later 270-row state.

## Provenance adoption

Within the initial 269-row snapshot, all dirty paths were under the declared
Core, World, WarCore, XYUI, or governance/tool roots and are classified as
`CANDIDATE_ADOPTED` by the requested provenance rule. No path was reset,
restored, stashed, cleaned, or overwritten.

```text
CANDIDATE_ADOPTED: 269 snapshot rows
KNOWN_FOREIGN_NOT_CONSUMED: 1 committed contract-conflict test
GENERATED_EXCLUDED: this R3 report and any later closeout artifacts
UNKNOWN: 0 by path classification
```

This adoption classification does not override the Cursor contract conflict
or the invalidated snapshot.

## Cursor Anchored Zoom special case

`XuanYu.World.Tests/Viewport/InputIntegration/CursorAnchoredZoomRegressionTests.cs`
is not Dirty. It is already committed in HEAD `a41c96f` and its methods assert
cursor/reference-plane anchor preservation. The formal camera truth record
still states `ObservationCenter invariant`; it explicitly does not prove
cursor-anchor behavior.

```text
Classification: KNOWN_FOREIGN_NOT_CONSUMED / CONTRACT-CONFLICT
Adopted: NO
Contract changed: NO
Final Candidate may compile or run it: NO
```

Because the normal World test project includes this committed file, a clean
Candidate Tree Match cannot be established without a separate exclusion or a
contract decision. Neither is assumed here.

## Inventory reconciliation

The authoritative committed HEAD scan is 2,906 definitions. The frozen dirty
candidate scan is 2,903 definitions:

| Project | HEAD definitions | Candidate definitions |
|---|---:|---:|
| Core | 362 | 354 |
| World | 1,872 | 1,872 |
| WarCore | 17 | 17 |
| XYUI | 660 | 660 |
| **Total** | **2,906** | **2,903** |

The Core difference is consistent with the eight retired implementation-mirror
definitions; they do not appear in the current candidate definition count.

```text
HEAD TEST DEFINITIONS: 2906
CANDIDATE TEST DEFINITIONS: 2903
RUNTIME TEST CASES: not rerun on the frozen snapshot; prior evidence 3332 is historical only
TRUTH DEFINITION RECORDS: 8 persisted central records
TRUTH PROFILES: 0 persisted profile references
UNREVIEWED: 2906 HEAD definitions lack complete mapping
UNMAPPED: 2906 HEAD definitions
DUPLICATE: 0 proven only for the 8-record subset
```

No aggregate World row is promoted to definition-level coverage. The existing
World audit reports `VALID_PROTECTOR=183` and `VALID_CONTRACT_TEST=317`, but
its 500 file-level rows do not provide enumerable membership for all current
Fact/Theory definitions.

## Truth gate

```text
VALID PROTECTOR: 183 (World audit subset)
VALID CONTRACT TEST: 317 (World audit subset)
CAPABILITY GAP: allowed; not a Truth failure
WRONG ORACLE: 0 persisted records
MISLEADING CLAIM: 0 persisted records
TIER OVERCLAIM: 0 persisted records
IMPLEMENTATION MIRROR: 0 current candidate records; 8 Core definitions retired
RED INSENSITIVE: 0 persisted records
TEST TRUTH ESCALATION: 0 persisted records
NEEDS_FIX: 0 in the World audit subset
```

Global Truth is not green because `UNMAPPED` and global `UNREVIEWED` are not
zero. The central registry remains the eight-record subset.

## Final execution and gates

Final four-suite execution was intentionally not started after the snapshot
drift and Cursor conflict were proven. Running World would compile/consume the
contract-conflict test, violating the Candidate Tree rule.

```text
Core execution: NOT RUN on final snapshot
World execution: NOT RUN on final snapshot
WarCore execution: NOT RUN on final snapshot
XYUI execution: NOT RUN on final snapshot
XYUI3CompactNavigationInteractionTests flaky observation: NOT RECHECKED
Truth selftest/schema/registry/MergeGate: historical subset only
ARCH-A: NOT ELIGIBLE for this final snapshot
WarCore Guard: NOT ELIGIBLE for this final snapshot
5+100: NOT ELIGIBLE for this final snapshot
git diff --check: NOT ELIGIBLE for this final snapshot
```

## Capability boundaries

```text
T3 GAP: Viewport Native, Render Vulkan, XYUI Desktop/Pixel
T4 GAP: PENDING
WarCore NOT_IMPLEMENTED capability: allowed GAP
```

## Final decision

```text
XYT-T2 STATUS: BLOCKED / NOT CLOSED
PRODUCT REGRESSION: NOT ESTABLISHED; final regression was not run
UNRESOLVED UNKNOWN: 0 by path; snapshot provenance is invalidated
GATE STATUS: FAIL
CANDIDATE TREE MATCH: NO
COMMIT ELIGIBILITY: NO
OLD TESTSET: T1 LEGACY
NEW TESTSET: not applied; T2 TRUTH-REVIEWED prohibited
COMMIT SHA: a41c96fc33bf1712b04c14ecdbc4c5e0dccfcaa5 (HEAD only)
REMOTE SHA: a41c96fc33bf1712b04c14ecdbc4c5e0dccfcaa5
AHEAD/BEHIND: 0/0 at initial freeze
COMMIT/PUSH: NONE
```

No version event was consumed. The existing product version ledger remains
independent of this blocked Truth closeout.

## KNOWLEDGE / EXPERIENCE AUDIT HANDOFF

请 ChatGPT 审计并沉淀知识经验。

Candidate lesson: a final-closeout snapshot must be immutable; a read-only
status drift invalidates all later mixed statistics. A committed test can be
Foreign/contract-conflict even when it is not Dirty, and Candidate Tree Match
must prove that final test compilation does not consume it. File-level audit
rows cannot satisfy Fact/Theory-level `UNMAPPED=0` without an enumerable member
manifest.

CHATGPT KNOWLEDGE AUDIT REQUIRED
