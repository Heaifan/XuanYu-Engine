# Candidate build, test and ownership

Candidate tree identity: `BASE=6b1979899d7421ee63d75833ebc7ceb0648f1505;OVERLAY-SHA256=00706F8E4CD2A4DDB1E68D1E4A08253FF8E64BF07333EE42E48196C909414ECE`. The candidate was materialized from `git archive` of official HEAD and overlaid only with the P1-A owned files. No second Git Worktree was created.

| Gate | Result | Scope |
|---|---|---|
| Editor App Release | PASS, 0 warnings / 0 errors | Candidate source snapshot |
| Control CLI Release | PASS, 0 warnings / 0 errors | Candidate source snapshot |
| XYC UI tests | PASS, 12/12 | `FullyQualifiedName~XycUi` |
| Editor App Debug | PASS, 0 warnings / 0 errors | Silent regression candidate |
| Control CLI Debug | PASS, 0 warnings / 0 errors | Silent regression candidate |
| Silent Host smoke | PASS | hidden HWND; foreground unchanged; 776x650; Vulkan attach/Present; normal close; no child-process residue |
| SHA checks | PASS | 12 final PNGs matched manifest SHA-256 |
| `git diff --check` | PASS | Working-tree check; unrelated line-ending warnings only |

Candidate Runtime binary SHA-256 values are in `evidence/candidate-final/runtime-evidence.json`. Silent runtime JSON is `silent-runtime-evidence.json`.

## Ownership

P1 files are the XYC Control command/runtime/discovery code, focused XYC tests, the P1 runner and this QA directory. The UI layout, Map/Terrain/Render/Vulkan product changes, Silent Host implementation and WP2 version/changelog changes are excluded. Existing unrelated Dirty bytes remain in the shared working tree.

The shared worktree matched fetched origin at the P0 baseline before commit. Formal staging used a temporary index containing only the XYC-owned files and a clean-baseline Changelog overlay. WP2/map/terrain/render/XYUI Dirty bytes were excluded and retained. No reset, checkout, clean or stash was used.

## Superseded runtime probes

`evidence/release-candidate-1/` and `release-candidate-2/` are preserved failed capture probes and are not acceptance evidence. `release-candidate-3/` is an earlier successful runtime before the last control lookup/test changes. `evidence/candidate-final/` is the accepted final candidate runtime evidence.
