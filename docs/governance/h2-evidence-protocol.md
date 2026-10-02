# H2 Evidence Packet and Candidate Tree Protocol

H2 evidence is a bounded observation of one Candidate Tree. A test result is
retained even when the observation is no longer admissible governance evidence.
`TestResult=PASS` therefore does not imply `EvidenceStatus=VALID`.

## Packet contract

Every packet contains `EvidenceId`, `LaneId`, `SessionId`,
`WorkspaceIdentity`, `BaselineHead`, `CandidateHead`, `DirtyDigest`,
`OwnedFiles`, `ConsumedDependencies`, `TestCommand`, `TestScope`,
`TestResult`, `EvidenceClass`, `Timestamp`, and
`ProductAcceptanceState`. The packet also stores `FingerprintBefore`,
`FingerprintAfter`, `EvidenceStatus`, and `Reason`.

Evidence status is exactly one of `VALID`, `STALE`, or `INVALID`.

- `VALID`: the packet's current Candidate Tree equals its accepted fingerprint.
- `STALE`: the test observation is historical and a later validation found a
  change such as `HEAD_CHANGED`, `DIRTY_CONTENT_CHANGED`,
  `DEPENDENCY_CHANGED`, or `OWNERSHIP_CHANGED`.
- `INVALID`: the test window changed unexpectedly, or an explicit/manual
  invalidation occurred. Test output is retained; it is not rewritten.

Supported invalidation reasons are `HEAD_CHANGED`, `DIRTY_CONTENT_CHANGED`,
`DEPENDENCY_CHANGED`, `OWNERSHIP_CHANGED`, `CANDIDATE_TREE_CHANGED`, and
`MANUAL_INVALIDATION`.

## Candidate fingerprint

`Fingerprint` binds `HEAD`, `StagedFiles`, `DirtyFiles`,
`TrackedDirtyContentHash`, `RelevantUntrackedFiles`,
`ConsumedDependencySnapshot`, and `OwnershipSnapshot`, then computes a
canonical `FingerprintId` and `DirtyDigest`. Tracked and untracked content is
hashed; a matching filename is never sufficient.

The scope is packet-local: dirty paths are included only when they are owned
files or consumed dependencies. An unrelated ForeignDirty change therefore
does not invalidate every packet. An UnknownDirty file becomes relevant when
the packet consumes it as a dependency, and its content is then fingerprinted.

## Lifecycle and commands

Use the H2 command in `scripts/governance/h2-evidence.ps1`:

1. `-Operation Begin` records `FingerprintBefore`.
2. Run the declared `TestCommand` outside this store.
3. `-Operation Complete -TestResult PASS|FAIL` records
   `FingerprintAfter`. If the fingerprints differ, it preserves the result
   and emits `EvidenceStatus=INVALID` with
   `Reason=CANDIDATE_TREE_CHANGED`.
4. `-Operation Validate` compares the packet with the current scoped tree and
   emits `VALID` or `STALE` with the specific change reason.
5. `-Operation ManualInvalidate` emits `INVALID` with
   `MANUAL_INVALIDATION`.
6. `-Operation Reuse` rejects any packet that is not current `VALID` evidence.

A fast-forward changes `HEAD` even when the consumed dependency content is
unchanged; under the formal rule it is `STALE` with `HEAD_CHANGED`. This is
distinct from a test-window mutation, which is `INVALID`.

## Handoff Runtime binding

The Handoff Work Release path uses the same content-binding rule in
`tools/governance/release/work-release.common.ps1`. Its `ownershipSet` is the Candidate
scope passed to the Runtime Fingerprint Builder at both release issue and
validation. Each scoped dirty path contributes its porcelain status and
SHA-256 content hash. This keeps H2 evidence checks and Handoff release checks
from accepting a same-name/same-status file whose bytes changed, while an
unconsumed ForeignDirty path remains outside the Candidate identity.

## Selftest

Run:

```powershell
powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File scripts/governance/h2-evidence.selftest.ps1
```

The selftest uses an isolated temporary Git repository and covers same
Candidate validity, dirty-content drift, test-window mutation, HEAD
fast-forward, dependency drift, ForeignDirty isolation, consumed
UnknownDirty, manual invalidation, and old-evidence reuse rejection.
