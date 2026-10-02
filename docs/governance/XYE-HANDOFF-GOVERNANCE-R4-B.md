# XYE-HANDOFF-GOVERNANCE-R4-B

> HISTORICAL：R4-B 治理记录。当前 Authority owner 位于 `tools/governance/**`，不得按本文旧路径调用 Handoff Authority。

## R4-B STATUS

Implemented in the Handoff governance lane. `UNKNOWN_DIRTY` is replaced as a live classification by `UNAUTHORIZED_DIRTY`. The classifier is responsibility-chain based: Git author, file time, and code similarity are never ownership evidence.

## DIRTY CLASSIFICATION MODEL

| Classification | Required evidence | Coding | Candidate | Evidence | Release |
|---|---|---:|---:|---:|---:|
| `FRIENDLY_ACTIVE` | ACTIVE Task + non-empty Owner + matching WriteScope | YES | YES | YES | YES |
| `FRIENDLY_COMPLETED` | RELEASED Task + Owner + matching WriteScope | YES | YES | NO | YES |
| `FOREIGN_KNOWN` | Explicit external/foreign ownership record | NO | NO | NO | NO |
| `UNAUTHORIZED_DIRTY` | No complete Task + Owner + WriteScope chain, or ambiguous claims | NO | NO | NO | NO |

## RULE TABLE

1. A dirty path is friendly only when a registry Task, its Owner, and its matching WriteScope all exist.
2. Multiple active claims are ambiguous and fail closed as `UNAUTHORIZED_DIRTY`.
3. A released ownership chain is `FRIENDLY_COMPLETED`; it is not active evidence.
4. A recorded foreign owner is `FOREIGN_KNOWN`; it cannot be claimed by the current task.
5. Git author, timestamps, and code similarity cannot create or transfer ownership.

## RECOVERY WINDOW

`UNAUTHORIZED_DIRTY` enters `RECOVERY_WINDOW`. `unauthorized-dirty-recovery.ps1 claim` may record an Owner Claim only when TaskId, Owner, and WriteScope are supplied. The claim remains non-certifying: Candidate, Evidence, and Release are all explicitly `false` until the normal registry/WriteScope chain is independently established.

## SWEEP POLICY

After the recovery window expires with no valid claim, the Coordinator may run `unauthorized-dirty-recovery.ps1 sweep`. The sweep is audit-first and records the decision; it does not silently delete or rewrite shared workspace material. Every record contains `PATH`, `TIME`, `REASON`, `ACTOR`, `BEFORE HASH`, and `AFTER RESULT`. The default safe result is escalation/preservation; an actual removal requires a separately authorized operator action and its resulting hash/result must be recorded.

## CURRENT DIRTY RECLASSIFICATION

The live case was audited before the governance files were added:

- `XuanYu.Editor.UI/Vm/Map/MapVectorOverlayBuilder.cs` → `FRIENDLY_ACTIVE`; owner chain is Task `XYE-REGION-PREVIEW-RENDER-CLOSEOUT-R1`, Owner `C`, matching WriteScope.
- The five RenderOrigin/VectorOverlay Vulkan files → `UNAUTHORIZED_DIRTY`; no Task + Owner + WriteScope chain was present.
- The untracked `VectorOverlayRenderOriginContractTests.cs` was also `UNAUTHORIZED_DIRTY` at that snapshot.

No product dirty file was modified by R4-B. The governance implementation changed only the files listed in `FILES CHANGED` below.

## AUDIT RECORD FORMAT

```text
PATH | TIME | REASON | ACTOR | BEFORE HASH | AFTER RESULT
```

The JSONL implementation additionally stores `Event`, `TaskId`, `Owner`, `WriteScope`, `RecoveryEnds`, and the three non-certifying flags.

## TEST EVIDENCE

- `task-dirty-classifier.selftest.ps1` — `PASS 7/7`; covers friendly active, unauthorized dirty, completed, ambiguous claims, and the four live class fields.
- `unauthorized-dirty-recovery.selftest.ps1` — `PASS 2/2`; covers Owner Claim restrictions and Sweep audit record format.
- `task-flight-plan-lifecycle.selftest.ps1` — `PASS 10/10`; existing lifecycle behavior remains valid under the new class names.

## FILES CHANGED

- `tools/handoff/task-dirty-classifier.ps1`
- `tools/handoff/task-dirty-classifier.selftest.ps1`
- `tools/handoff/unauthorized-dirty-recovery.ps1`
- `tools/handoff/unauthorized-dirty-recovery.selftest.ps1`
- `tools/handoff/task-flight-plan-lifecycle.selftest.ps1`
- `tools/handoff/candidate-gate.ps1`
- `tools/handoff/candidate-scoped-gate.selftest.ps1`
- `tools/handoff/task-flight-plan.handoff.ps1`
- `tools/handoff/authority-state.ps1`
- `tools/handoff/handoff.ps1`
- `tools/handoff/handoff.selftest.ps1`
- `tools/handoff/dependency-handoff.ps1`
- `tools/handoff/dependency-handoff.selftest.ps1`
- `tools/handoff/dirty-convergence.selftest.ps1`
- `tools/handoff/adversarial-selftest.ps1`
- `tools/handoff/work-release.issue.ps1`
- this audit record

## XYK HANDOFF

Candidate lesson for ChatGPT audit: responsibility-chain provenance must be evaluated from the live Task Registry and WriteScope. Unknown provenance is not a neutral waiting state: it is `UNAUTHORIZED_DIRTY`, enters a bounded recovery window, and remains barred from Candidate/Evidence/Release until independently authorized.

`XYK CANDIDATE: CREATE_OR_STRENGTHEN` — ChatGPT must first search existing XYK knowledge for the dirty-provenance/classifier contract, then choose `CREATE`, `UPDATE`, `STRENGTHEN`, `RETIRE`, or `NO DEPOSIT` based on duplicate and conflict evidence.
