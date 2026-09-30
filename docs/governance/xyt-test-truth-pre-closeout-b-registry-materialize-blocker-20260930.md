# XYT-T2 / PRE-CLOSEOUT / B-REGISTRY-MATERIALIZE — BLOCKER

Date: 2026-09-30  
Workspace: `E:\MyDoc\project-VSCode\XuanYuEngine`

## Finding

Registry materialization was not performed because the requested denominator does not match the actual current HEAD.

| Ref | Core | World | WarCore | XYUI | Total |
|---|---:|---:|---:|---:|---:|
| `HEAD^` (`45fd7e63`) | 362 | 1867 | 17 | 660 | **2906** |
| `HEAD` (`a41c96fc`) | 362 | 1872 | 17 | 660 | **2911** |

`HEAD` is `fix(world): integrate terrain visibility and cursor anchored zoom`. Its five additional World definitions are committed source, not generated artifacts and not safely excludable. A 2906-entry Registry would therefore leave five current HEAD definitions unmapped and would violate `UNMAPPED=0` / unique Definition mapping.

## Existing governance evidence

- Existing central `xyt-test-truth-registry.json`: 8 historical T-A file-level records only.
- Schema validation: PASS for the existing 8-record subset.
- Existing selftest: PASS for the existing 8-record subset.
- Existing MergeGate: YES for the existing 8-record subset only.
- Existing reconciliation script: HEAD count 2911; candidate working-tree count 2904; neither supports the requested 2906 current-HEAD denominator.

## Decision

`REGISTRY MATERIALIZATION: BLOCKED`  
`UNMAPPED: NOT ZERO / BASELINE MISMATCH`  
`TEST_TRUTH_ESCALATION: NOT PROVEN FOR FULL REGISTRY`  
`CANDIDATE_ADOPTION_REQUIRED: 5 HEAD WORLD DEFINITIONS OR APPROVED BASELINE CHANGE`

No C# test or product code was modified. The central Registry was not modified. No reset, restore, stash, cleanup, worktree, commit, or push was performed.
