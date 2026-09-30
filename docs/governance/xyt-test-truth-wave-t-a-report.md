# XYT-T2 / WAVE-T-A Final Convergence Report

Task: `XYT-T2`
Lane: `GOVERNANCE / COORDINATOR`
Workspace: `E:\MyDoc\project-VSCode\XuanYuEngine`
TestSet: `T1 LEGACY`
Candidate: `T2-CANDIDATE / WAVE-T-A`

## 1. Truth Record Update

Updated the same TestId: `terrain.context.hidden-region-host-slot`.

```text
TruthStatus: VERIFIED
Decision: KEEP
RegressionWitness: PRE-FIX FAIL / POST-FIX PASS
WitnessStatus: GREEN_CONFIRMED
PRE-FIX: same test, Expected 0, Actual 432, RED_CONFIRMED
POST-FIX: same test, Expected 0, Actual 0, PASS, GREEN_CONFIRMED
```

The historical RED evidence is retained in `WitnessEvidence`; it was not deleted or replaced by the POST-FIX result.

## 2. Registry and Truth Statistics

```text
TOTAL: 8
REVIEWED: 8
UNREVIEWED: 0
KEEP: 8
RENAME: 0
STRENGTHEN: 0
REWRITE: 0
SPLIT: 0
RETIRE: 0
ESCALATE: 0
WRONG_ORACLE: 0
MISLEADING_CLAIM: 0
TIER_OVERCLAIM: 0
RED_INSENSITIVE: 0
ESCALATED_TEST_TRUTH blocker: 0
```

Capability mapping remains five entries: Terrain, Camera, Viewport/Input, Region/Ground Binding, and Render.

## 3. Candidate Tree Purity

All current Dirty Files were classified as follows; no file was cleaned, restored, reset, stashed, or overwritten.

### A. Wave T-A product fix

`Directory.Build.props`, `XuanYu.Core.Tests/Render/TerrainVisibilitySelectorTests.cs`, `XuanYu.Editor.UI/Vm/Mode/UiVm.Mode.cs`, `XuanYu.Editor.UI/Vm/Workspace/UiVm.TerrainContext.cs`, the four Terrain acceptance-to-contract/integration test replacements, `TerrainMultiSourceImportTests.cs`, the three TerrainAutoFrame test files, `TerrainContextRuntimeFixTests.cs`, `TerrainTopContextContractTests.cs`, `changelog.md`, `docs/governance/test-registry.json`, and `docs/governance/version-events.tsv`.

### B. Wave T-A Test Truth governance assets

`docs/governance/xyt-test-truth-policy.md`, `docs/governance/xyt-test-truth-registry.json`, `docs/governance/xyt-test-truth-schema.json`, `docs/governance/xyt-test-truth-wave-t-a-report.md`, `tools/governance/xyt-test-truth.ps1`, and `tools/governance/xyt-test-truth.selftest.ps1`.

### C. Known ForeignDirty

None in the final Candidate Tree classification. Handoff ownership reports 21 XYE-owned files and 6 GOVERNANCE-owned files; all 27 are explicitly accounted for in A or B.

### D. UNKNOWN

`0`.

Candidate Tree Match: `YES`.

## 4. Final Regression and Gates

Required final tests must be executed from this same Candidate Tree. Historical Lane PASS from another tree is not reused as final evidence.

```text
Terrain / Mode / Context / Workspace / Viewport/Input / Render / Camera / Region-Ground scoped: PASS; XuanYu.World.Tests 1012/1012
Core Terrain / Viewport / Render / Camera scoped: PASS; XuanYu.Core.Tests 351/351
ARCH-A: PASS
5+100: PASS (included by ARCH-A)
git diff --check: PASS
```

Until these commands complete, final convergence remains blocked.

## 5. Red Sensitivity Ledger

```text
Terrain: VERIFIED
Region/Ground Binding: VERIFIED
Viewport Capture Lifecycle: VERIFIED
Render Draw Plan: VERIFIED
Camera: VERIFIED only for ObservationCenter invariant; no Cursor Anchor sensitivity claimed
```

Render boundary remains explicit:

```text
CPU / Render List contract: VERIFIED
Command runtime: NOT PROVEN
QueueSubmit runtime: NOT PROVEN
GPU execution: NOT PROVEN
Framebuffer: NOT PROVEN
Present: NOT PROVEN
User-visible pixels: NOT PROVEN
```

## 6. Capability Gaps

```text
Viewport T3: GAP
Render Vulkan T3: GAP
T4 Product Acceptance: PENDING
```

These are recorded as GAP/PENDING, never as PASS. Wave T-A may close only if all required automated gates pass and these gaps remain honestly represented.

## 7. Version Event

```text
Product Version: v0.3.0.3-fix
Event: XYT-T2-T-A-FIX-A-TERRAIN-CONTEXT
Ledger status: APPLIED; product candidate CommitId 115b4ea9
```

`Directory.Build.props`, Window Title source, `changelog.md`, and `docs/governance/version-events.tsv` are aligned to the same candidate version. Truth audit/rename/registry changes do not consume a second FIX event. The provisional event must not become APPLIED until the final Candidate / Commit Gate passes.

## 8. Final Report Status

```text
XYT-T2 / WAVE-T-A STATUS: CLOSED
PRODUCT REGRESSION: NONE KNOWN from current recorded evidence
UNRESOLVED UNKNOWN: 0
GATE STATUS: PASS
CANDIDATE TREE MATCH: YES
COMMIT ELIGIBILITY: YES
EXECUTION STATUS: World scoped 1012/1012; Core scoped 351/351; Truth selftest/registry validation PASS
TRUTH AUDIT STATUS: PASS for all 8 imported records
TOTAL REVIEWED: 8
WRONG ORACLE: 0
MISLEADING CLAIM: 0
TIER OVERCLAIM: 0
RED INSENSITIVE: 0
REGRESSION WITNESS: Terrain Context RED -> GREEN, SAME TEST, GREEN_CONFIRMED
TESTSET VERSION: T1 LEGACY
T2 STATUS: T2-CANDIDATE / WAVE-T-A CLOSED; global TestSet remains T1 LEGACY and is not globally T2 truth-reviewed
COMMIT SHA: cd0613c159c1985b494bb945a76d15dae84056e5
REMOTE SHA: cd0613c159c1985b494bb945a76d15dae84056e5
AHEAD/BEHIND: 0/0
STAGED: 0
```

## KNOWLEDGE / EXPERIENCE AUDIT HANDOFF

请 ChatGPT 审计本任务结果，并判断是否有可沉淀的知识库/经验库内容；如有，提炼并按治理规则维护入库。

Candidate lessons: Execution 与 Truth 必须独立；同一测试的真实 PRE-FIX RED 与 POST-FIX GREEN 才能形成 Regression Witness；控制性 Mutation 只能证明 RED sensitivity；Capability coverage 与 TruthStatus 不得混为一谈。

CHATGPT KNOWLEDGE AUDIT REQUIRED
