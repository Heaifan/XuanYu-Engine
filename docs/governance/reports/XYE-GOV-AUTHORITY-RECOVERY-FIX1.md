# XYE-GOV-AUTHORITY-RECOVERY-FIX1

**Status:** Source implementation and ADDENDUM-A scope corrections are uncommitted locally; formal commit/push BLOCKED by the unchanged global ARCH-A failure. No second-stage state recovery was performed.

## Authorization And Baseline

- Lane: GOVERNANCE.
- Authorization: GitHub Issue #8 and owner comment `6056042074`; exact paths are limited to the Issue allowlist.
- Addendum: owner comment `6059455929`, author `Heaifan` / account `86356120` / `OWNER`; it adds exactly seven test, ledger, Gate, and Gate-documentation paths. It preserves the same baseline and explicitly excludes Camera tests, product-version advancement, Work Release, and second-stage state recovery.
- GitHub API evidence: `Heaifan`, account `86356120`, association `OWNER`; created `2026-10-08T08:37:06Z`, unedited. The authorization window ends `2026-10-10T08:37:06Z`.
- The owner comment is source-only, not an operation grant or Work Release Token. A live check rejected it as a `wave-init` grant (`AUTH_EVIDENCE_INVALID`).
- Canonical workspace: `E:\MyDoc\project-VSCode\XuanYuEngine`.
- Pre-Fix HEAD / branch: `956acdc109e331712bd6307b7ee09f1573d8ffc9` / `recovery/editor-center-navigation-r1`.
- Bootstrap: READY; Resolver `scripts/resolve-dotnet.ps1`; SDK `10.0.400`.
- Initial worktree: clean; upstream was `origin/recovery/editor-center-navigation-r1`, remote tip matched baseline, Ahead/Behind `0/0`. No other active Codex repo writer was listed. No worktree was created.

## Pre-Fix Evidence

| TestId | Same-test RED evidence |
|---|---|
| `WORK-RELEASE-AUTH-01` | Isolated repository accepted `-CentralIssuer` and emitted an ACTIVE Work Release token. |
| `TASK-LIFECYCLE-AUTH-01` | Isolated repository accepted `close` without owner evidence and removed the RELEASED task from the live Registry. |
| `WAVE-INIT-BOOTSTRAP-01` | Required `wave-init.ps1` entrypoint was absent. |

## Changes

## Root Cause

- Lifecycle and release tools previously treated caller flags/strings as identity and permitted transitions without externally verifiable, operation-scoped Owner evidence; Registry/History atomicity and the compatibility projection lacked an enforced shared transaction boundary.
- Existing task and fingerprint tests encoded the obsolete unauthenticated success path, so they failed after the security boundary was tightened.
- The Version Event Gate accepted only FIX/FEATURE despite the audited ledger contract documenting GOVERNANCE as a non-product event. Its EventId comparison was also case-insensitive under PowerShell defaults.
- The Gate treated `-RequireClean` as part of the same event decision used before commit. A prepared Candidate necessarily leaves the worktree dirty, so pre-commit event verification could never satisfy that clean-tree predicate. Event metadata validation and post-commit clean identity are separate phases.

- Owner authorization now comes from an unedited GitHub Issue comment fetched from the canonical GitHub API. The verifier binds repository, owner account and association, exact action/target/detail/baseline, and a maximum 48-hour expiry. Caller-provided `-CentralIssuer` and `-Coordinator` strings are not credentials.
- Accepted comments are single-use: a locked, append-only authorization-consumption record is written immediately before an authorized mutation. The source-only Issue #8 comment is rejected as an operation grant.
- Work Release issue/revoke require that evidence; issue also requires a unique ACTIVE Task Registry entry. Version Event IDs must be nonempty and match the exact first TSV cell; blank, substring, and header identifiers fail.
- Task Close/Reap require exact owner evidence and released task resources. `transfer` changes the Owner with a durable history snapshot and does not discard `ExpectedDependencies`.
- Task Registry is the lifecycle source. Optional `state.activeTasks` is checked as a projection, not used as authority. Registry/History updates use a lock, transaction journal, rollback snapshot, and recovery event; recovery runs before the next Task command.
- `close-authority.ps1` no longer edits Task statuses directly. Lane/Wave close require owner evidence, and close operations share lifecycle locks.
- `wave-init.ps1` validates clean HEAD/branch, existing Wave, Task Registry, OPEN dependencies, Candidate freeze, Release, ownership locks, and commit mutex. It installs state/release/ownership as a journaled file set and restores prior files on failure. It preserves previous State in Wave history. This tool was tested only on temporary repositories.

## Changed Files

All changed paths are within Issue #8's exact allowlist:

- `tools/governance/coordinator/authority-auth.ps1` (new)
- `tools/governance/coordinator/close-authority.ps1`
- `tools/governance/coordinator/wave-init.ps1` (new)
- `tools/governance/coordinator/wave-init.selftest.ps1` (new)
- `tools/governance/release/work-release.ps1`
- `tools/governance/release/work-release.common.ps1`
- `tools/governance/release/work-release.issue.ps1`
- `tools/governance/release/work-release.assert.ps1`
- `tools/governance/release/work-release-auth.selftest.ps1` (new)
- `tools/governance/task/task-flight-plan.ps1`
- `tools/governance/task/task-flight-plan.common.ps1`
- `tools/governance/task/task-flight-plan-lifecycle.selftest.ps1`
- `tools/governance/task/task-flight-plan-auth.selftest.ps1` (new)
- `docs/governance/reports/XYE-GOV-AUTHORITY-RECOVERY-FIX1.md` (new)
- `tools/governance/task/task-flight-plan.selftest.ps1` (ADDENDUM-A)
- `tools/governance/release/dirty-content-fingerprint.selftest.ps1` (ADDENDUM-A)
- `docs/governance/version-events.tsv` (ADDENDUM-A)
- `scripts/governance/version-event-gate.ps1` (ADDENDUM-A)
- `scripts/governance/version-event-gate.lib.ps1` (ADDENDUM-A)
- `scripts/governance/version-event-gate.selftest.ps1` (ADDENDUM-A)
- `docs/governance/xyt-version-event-gate.md` (ADDENDUM-A)

No product code, production `.git/xye-handoff` state, old Candidate, P4 evidence, or formal XYK was changed. The Version Ledger contains only the ADDENDUM-A-authorized `RESERVED` GOVERNANCE event, with no product version advance. No real task was closed/reaped/transferred; no Work Release was issued/claimed/revoked; no Wave was initialized.

## Verification

- Same-test Post-Fix GREEN: `WORK-RELEASE-AUTH-01` PASS; `TASK-LIFECYCLE-AUTH-01` PASS; `WAVE-INIT-BOOTSTRAP-01` PASS.
- `work-release-auth.selftest.ps1`: PASS; includes forged switch denial, no-evidence issue/revoke denial, exact Version Event matching, owner/action mismatch rejection, and replay denial.
- `task-flight-plan-auth.selftest.ps1`: PASS; includes unauthenticated Close/Reap/transfer denial, Task Registry projection mismatch rejection, dependency-preserving transfer evidence, and Registry/History failure recovery.
- `task-flight-plan-lifecycle.selftest.ps1`: PASS 10/10; includes global/lane close authorization rejection and unchanged state.
- `task-flight-plan.selftest.ps1`: PASS; unauthorized Close/Reap are denied without removing tasks; Ownership overlap retains its actual `OWNERSHIP_CONFLICT` result.
- `dirty-content-fingerprint.selftest.ps1`: PASS, 5 cases; unauthenticated issue is denied and the positive fingerprint-specific path uses a narrowly scoped synthetic authorization fixture.
- `version-event-gate.selftest.ps1`: PASS; GOVERNANCE missing/exact-case ID, stale baseline, product-version bump, wrong type, missing evidence, dirty PRE-COMMIT Candidate acceptance, invalid phase/flag combination, and dirty POST-COMMIT rejection are checked. RED was observed before implementation: PRE-COMMIT accepted the POST-COMMIT flag.
- Live Event Gate PRE-COMMIT: PASS for exact reserved event `XYE-GOV-AUTHORITY-RECOVERY-FIX1`; commit eligibility and push are reported as deferred. POST-COMMIT with `-RequireClean`: BLOCKED because this Candidate worktree is dirty, as expected before commit.
- `work-release-auth.selftest.ps1`: PASS; `task-flight-plan-auth.selftest.ps1`: PASS; `task-flight-plan-lifecycle.selftest.ps1`: PASS 10/10; `task-flight-plan.selftest.ps1`: PASS; `dirty-content-fingerprint.selftest.ps1`: PASS 5 cases; `wave-init.selftest.ps1`: PASS; `active-wave-migration.selftest.ps1`: PASS; `task-dirty-classifier.selftest.ps1`: PASS 7/7; `candidate-scoped-gate.selftest.ps1`: PASS 10/10; `version-selftest.ps1`: PASS.
- Candidate Purity: PASS, exactly 21 changed paths, all within the Issue #8 + ADDENDUM-A allowlist. PowerShell parser: PASS for 18 changed scripts. `git diff --check`: PASS.
- `wave-init.selftest.ps1`: PASS; covers missing authorization, stale baseline, successful isolated initialization, active Wave, OPEN dependencies, old Candidate preservation, and multi-file rollback audit.
- `candidate-scoped-gate.selftest.ps1`: PASS 10/10.
- `active-wave-migration.selftest.ps1`: PASS; `task-dirty-classifier.selftest.ps1`: PASS 7/7; `version-selftest.ps1`: PASS.
- PowerShell Parser: all 18 changed PowerShell scripts PASS; each is at or below 100 lines.
- `git diff --check`: PASS.
- Full Solution Build: NOT RUN; this change only touches PowerShell governance tooling and its tests.
- `scripts/arch-a-guard.ps1`: BLOCKED by the existing out-of-scope Camera test `XuanYu.World.Tests/Camera/OrbitPivotAuthorityTests.cs` at 104 lines. Camera tests were not modified and ARCH-A was not bypassed.

## Blocking Gates

- Same-test pre-fix: `task-flight-plan.selftest.ps1` failed because it expected unauthenticated Close/Reap to succeed; after ADDENDUM-A it asserts both are denied and preserve registry state. Current rerun: PASS.
- Same-test pre-fix: `dirty-content-fingerprint.selftest.ps1` failed because Work Release issuance lacked an ACTIVE Registry task and owner evidence; after ADDENDUM-A it asserts unauthenticated denial, then uses a tightly scoped synthetic Owner verifier only for its fingerprint-specific positive path. Current rerun: PASS (5 cases).
- `version-event-gate.ps1 -ChangeType GOVERNANCE -CurrentVersion v0.3.0.9-fix` previously failed for missing event. ADDENDUM-A permits an exact `GOVERNANCE` ledger row and gate changes. The Gate now requires an exact case-sensitive EventId, same current baseline, nonempty evidence, no product Candidate binding, and no product version advance. Added reserved event `XYE-GOV-AUTHORITY-RECOVERY-FIX1`; product Version Event remains NONE.
- `scripts/arch-a-guard.ps1` is BLOCKED by the existing, out-of-scope `XuanYu.World.Tests/Camera/OrbitPivotAuthorityTests.cs` at 104 lines.
- The local Governance Event gate validates the reserved event without `-FormalAcceptance` in PRE-COMMIT. `-RequireClean` is valid only in POST-COMMIT and checks clean identity; it cannot be used as the prepared-Candidate commit decision.
- Global ARCH-A remains BLOCKED by the pre-existing Camera test file at 104 lines. Per owner instruction it was not changed and ARCH-A was not bypassed.
- Therefore no commit or push was attempted. All local changes remain uncommitted at the authorized baseline; remote tip remains the baseline. This mandatory global gate prevents a legal commit.

## Second Stage

Second-stage authorization is still required before operating on real `.git/xye-handoff` state. The existing Owner C task and OPEN dependencies must remain traceable; Legacy TEMP remains RELEASED; the Drawing Candidate remains frozen with `P4_PENDING`. The new Wave entrypoint deliberately rejects unresolved tasks/dependencies and any existing Candidate freeze. R2-A has not been initialized or released.

## Known Risk / Closeout Identity

- HEAD remains `956acdc109e331712bd6307b7ee09f1573d8ffc9`, branch `recovery/editor-center-navigation-r1`; origin tip was verified equal with Ahead/Behind `0/0` before changes. No commit or push occurred because global ARCH-A is blocked.
- The new Governance ledger row remains `RESERVED` until controlled commit/closeout; product Process Version remains `v0.3.0.9-fix` and was not advanced.
- Candidate Tree Purity: 21 changed paths are within the original Issue #8 allowlist plus the exact seven ADDENDUM-A paths. No Camera/product files, production `.git/xye-handoff` state, Candidate/P4, or formal XYK were changed.
- `git diff --check`: PASS. Candidate-scoped Gate selftest: PASS 10/10. Real Candidate certification was not run because no production Candidate state may be mutated in this phase.
- Commit: NONE. Push: NONE. Remote tip remains the bound baseline.

==================================================
KNOWLEDGE / EXPERIENCE AUDIT HANDOFF
==================================================

Please have ChatGPT audit this implementation and decide whether a Knowledge or Experience candidate should be deposited. No Knowledge store was modified.

XYK CANDIDATE:
- ID/Category: GOVERNANCE-AUTH-01 / Experience
- Problem: Local command flags and caller-supplied Coordinator strings were accepted as lifecycle authority; Task Registry and state projection could diverge.
- Root Cause: The previous tools did not bind mutations to externally verifiable, operation-scoped authorization, and Registry/History had no recoverable transaction boundary.
- Rule / Knowledge: Authority evidence must be fetched from a trusted source, bound to the exact action, target, detail, and baseline, and consumed once. A lifecycle Registry remains authoritative; compatibility state is only a checked projection, and audit plus state writes need detectable rollback/recovery.
- Applies To: `tools/governance/**` lifecycle and release operations.
- Evidence: Same-test RED/GREEN records above; isolated auth, replay, transaction rollback, projection mismatch, and Wave init tests.
- Expiry / Revalidation: Revalidate after the formal governance ledger and owner-evidence protocol are integrated.
- Suggested Action: SEARCH EXISTING before deciding UPDATE / STRENGTHEN / CREATE / NO DEPOSIT.

- ID/Category: GOVERNANCE-EVENT-01 / Experience
- Problem: The Event Gate applied a clean-worktree acceptance condition while a valid prepared Candidate was necessarily dirty, conflating pre-commit preparation with post-commit identity verification.
- Root Cause: `-RequireClean` was modeled as part of the same event decision without an explicit lifecycle phase.
- Rule / Knowledge: Governance Event preflight validates exact event identity, baseline, evidence, and non-advancement before commit. Candidate purity, staged diff, tests, and architecture remain independent pre-commit checks. Clean-tree identity is checked only after commit; neither phase grants remote push authorization.
- Applies To: `scripts/governance/version-event-gate.*`, XYT governance event checks, and commit closeout sequencing.
- Evidence: Same selftest RED before phase separation and GREEN for dirty PRE-COMMIT, invalid phase/flag pairing, and dirty POST-COMMIT rejection; live Gate output defers commit and push eligibility.
- Expiry / Revalidation: Revalidate when XYT's formal closeout contract adds staged Candidate and remote-tip orchestration.
- Suggested Action: SEARCH EXISTING and STRENGTHEN `EXP-GOVERNANCE-003` if its current scope does not already capture phase separation.

AI AGENTS DO NOT HAVE DEFAULT XYK WRITE AUTHORITY.
CHATGPT KNOWLEDGE AUDIT REQUIRED
