# XYE-HANDOFF-SLIMMING-R1-B2-C XYK

- Role: Tooling Owner; mode: WRITE.
- Handoff command control was removed from `tools/handoff/handoff.ps1`.
- Old commands rejected: `prepare`, `join`, `advance`, `close`, `migrate`, `release`.
- New fact-only commands: `record-event`, `query-history`, `show-owner`.
- `record-event COMPLETION_REPORTED` appends an event with Git and Dirty Scanner facts; it does not advance, close, release, migrate, commit, push, or mutate wave state.
- `query-history` reads `.git/xye-handoff/events.jsonl`; `show-owner` reads the ownership manifest.
- Verified by `handoff-b2c.selftest.ps1`: old command rejection 6/6, fact command Git-worktree mutation check PASS; PowerShell parse and `git diff --check` PASS.
- Migration rule: replace `handoff advance` used as a fact notification with `handoff record-event COMPLETION_REPORTED`.
