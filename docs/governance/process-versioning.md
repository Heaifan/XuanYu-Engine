# Process Version Governance

## Historical audit

The current source of truth is `Directory.Build.props`, exposed through `scripts/resolve-version.ps1`; the observed current value is `v0.3.0.0-r1`. Historical records show that the four numeric fields are not a mathematically stable SemVer contract: `A.B` marks a broad product generation, `C` commonly identifies a work/release line, and `D` is the observed within-line iteration counter. These meanings are historical observations, not permission to rewrite old entries.

Suffixes such as `-r1`, `-rz`, `-fix`, `-fix3`, and `-stab` historically mixed release stage, work-line label, fix chain ordinal, and maturity. They must remain historical evidence. The future machine rule is only that every counted event produces a distinct valid version string; the suffix communicates event family where useful.

The audit found duplicate headings, non-monotonic historical dates, and repeated use of `v0.2.28.77-fix3`. K-GOV-001 records the wider archive finding: at least three version values were assigned to multiple rounds and 18 date/order anomalies exist. Old records are not rewritten and are never disambiguated by date alone. Historical coverage is therefore `HISTORICAL PARTIAL`.

## Contract

### Version Event

Each counted change is one event and one ledger row. Supported types are `FEATURE`, `FIX`, `STABILIZATION`, and `GOVERNANCE`.

`FEATURE` means one independently describable and independently acceptable capability increment. It advances Process Version once. `FIX` means one observed Bug, Regression, or Acceptance Failure repaired into an independently verifiable result. Each real repair round advances once; FIX1/FIX2/FIX3 are never compressed. `STABILIZATION` advances once for a distinct hardening result. Under the audited XYE history, a governance-only change does not advance the product Process Version; it is still recorded as a `GOVERNANCE` ledger event when it is a material governance result.

The version calculator preserves the repository format `vA.B.C.D[-suffix]`. A new FEATURE/FIX/STABILIZATION increments `D`; a current `-fixN` chain increments `N`. This is a compatibility rule for the current line, not a universal string format for other projects.

### Build and acceptance identity

Formal records must contain:

```text
Version: vA.B.C.D-suffix
Commit: <full or short SHA>
Branch: <branch>
Dirty: NO
Identity: <Version>@<SHA>
```

`Dirty=YES` is allowed only for an explicitly named Dirty Runtime Probe. It is not a formal Milestone, Release, or Product Acceptance Baseline. K-VAL-001 requires the same Version and Commit SHA on the user-run artifact and the validation artifact.

### Preflight and closeout

Planning and Execution Knowledge Preflight are both mandatory. Before formal acceptance or commit, print `VERSION EVENT` with Change Type, Previous, Next, and Reason. If no product increment applies, print `Version Event: NONE` and explain the historical rule. Never default to NONE.

The tools are read-only except for no file mutation at all: `version-audit.ps1` reports identity and collisions, `version-next.ps1` calculates without editing, `version-selftest.ps1` verifies the contract, and `version-metrics.ps1` counts only ledger rows.

## Acceptance identity

```text
ACCEPTANCE IDENTITY
Version: <Process Version>
Commit: <SHA>
Branch: <Branch>
Dirty: NO
Identity: <Version>@<SHA>
```

Automatic gates do not replace human visual or real-device acceptance; they must retain the identity of the exact artifact tested.

## Development friction review

The TSV ledger is deliberately small and one-event-per-row. It contains no AI private reasoning. Missing historical rows are not reconstructed from guesses; metrics state `HISTORICAL PARTIAL`. Monthly or milestone review reports Feature Count, Fix Count, Fix/Feature, highest-friction Domain, longest fix chain, T0/T0+ incidents, New Knowledge, New Lesson, New EXP, and New Machine Gate. The result feeds Knowledge/Lesson/EXP and future regression or machine gates.

## Dogfood sequence

With current `v0.3.0.0-r1`, the required non-compressible simulation is:

```text
Previous: v0.3.0.0-r1
Feature:  v0.3.0.1-r1
Fix1:     v0.3.0.2-fix
Fix2:     v0.3.0.3-fix
Feature:  v0.3.0.4-r1
Fix:      v0.3.0.5-fix
```

This is a tool simulation, not a claim that these product versions were committed or that historical events were recovered.
