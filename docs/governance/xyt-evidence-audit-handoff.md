# XYT-K Evidence Expiry Audit Handoff

## SEARCH EXISTING

Existing `docs/governance/test-evidence-policy.md` defines evidence tiers,
runtime/product gates, scale limits, and temporal evidence. It does not define
change-triggered expiry, capability isolation, Development versus Closure
semantics, or a minimum revalidation plan.

## Candidate decision

`CREATE` a separate Evidence Expiry / Revalidation rule. It complements the
existing tier policy and does not replace historical evidence or convert stale
evidence into Product Failure.

## Evidence from this change

- `scripts/governance/xyt-evidence.ps1` implements four-state records,
  capability-scoped triggers, Development allowance, Closure blocking, and
  minimum-sufficient revalidation planning.
- `scripts/governance/xyt-evidence.selftest.ps1` covers Render, documentation,
  unrelated P3 isolation, Test Set Major, Test Oracle, plan selection,
  Development allowance, Closure blocking, and state preservation.
- Relevant selftests and `git diff --check` passed in the current checkout.

## XYK writeback status

The requested remote `Heaifan/XuanYu-Knowledge` probe returned `Repository not
found`; no authenticated writable XYK target is available in this workspace.
Therefore no remote Knowledge/Experience record or fabricated K Event was
written. This file is the local audit handoff for a later ChatGPT audit after
the XYK repository is provisioned.

## Candidate lessons

1. Triggered stale evidence should become `REVALIDATION_REQUIRED`, not
   `PRODUCT FAIL`.
2. Closure must require current critical evidence while Development remains
   unblocked.
3. Capability-scoped invalidation is required to prevent unrelated P0/P3
   evidence contamination.
