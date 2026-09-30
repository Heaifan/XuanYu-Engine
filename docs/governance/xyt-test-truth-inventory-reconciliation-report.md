# XYT-T2 / FINAL-RECONCILIATION / A-TRUTH-INVENTORY

Lane: GOVERNANCE  
Workspace: `E:\MyDoc\project-VSCode\XuanYuEngine`  
HEAD: `45fd7e636bfeb74701b4d192cfbedd6f4ddeb1d5`  
Scope: source inventory and Truth-ledger reconciliation only.

## Executive result

The value `2903` is a dirty-candidate working-tree Fact/Theory count. It is
not the count of the committed `HEAD`. The committed `HEAD` contains `2906`
Fact/Theory definitions. The difference is caused by the final A remediation
and unrelated candidate-tree changes: Core is `362` at HEAD and `354` in the
working tree; World is `1867` at HEAD and `1872` in the working tree; WarCore
is `17` in both; XYUI is `660` in both.

The previously quoted `1540` is not a TruthRecord total. It adds unlike
units: approximately 362 Core method definitions, 500 World file-level
records, 18 historical WarCore review units, and 660 XYUI definitions.

No truthful evidence currently supports global `UNREVIEWED = 0`. This report
therefore does not write the central registry and records reconciliation as
`BLOCKED / NOT CLOSED` rather than manufacturing records.

## Authoritative inventory

The source-definition identity is one attribute occurrence, not one runtime
case and not one source file:

`TestDefinitionId = TD/<project>/<relative-file>#attribute-line`

The attribute-line component is required because method names can repeat in
partial files or across classes. The scan counts only source `[Fact]` and
`[Theory]` definitions.

| Source | HEAD C# files | HEAD Fact | HEAD Theory | HEAD definitions | Candidate definitions |
|---|---:|---:|---:|---:|---:|
| Core | 88 | 337 | 25 | 362 | 354 |
| World | 550 | 1804 | 63 | 1867 | 1872 |
| WarCore | 3 | 12 | 5 | 17 | 17 |
| XYUI | 147 | 647 | 13 | 660 | 660 |
| **Total** | **788** | **2800** | **106** | **2906** | **2903** |

The candidate tree currently has 90 Core files, 555 World files, 6 WarCore
files, and 153 XYUI files (804 files total). Those extra candidate files are
dirty-tree material and do not alter the candidate definition total above.

The candidate values are the authoritative scan previously reported as 2903.
The HEAD values were independently counted from `git grep` against the
committed tree. Neither count is a Truth review count.

## Three-level identity model

| Level | Identity | Meaning | Current evidence |
|---|---|---|---|
| Definition | `TestDefinitionId` | One Fact/Theory source attribute | Reproducible for HEAD and candidate tree |
| Runtime | `RuntimeCaseId` | One executed Fact invocation or one expanded Theory case | Existing candidate execution report says Core 441, World 2163, WarCore 22, XYUI 706; total 3332 |
| Truth | `TruthRecordId` | One Truth decision for one definition, or an explicitly bounded aggregate with member IDs | Not globally materialized |

Runtime cases must never be counted as additional definitions. A Theory with
N data rows is one TestDefinitionId and N RuntimeCaseIds. The current reports
give the runtime total, but do not persist a complete definition-to-runtime
case manifest.

## Existing ledger reconciliation

| Artifact | Count | Actual unit | Reconciled meaning |
|---|---:|---|---|
| Central `xyt-test-truth-registry.json` | 8 | T-A candidate Truth records | Reviewed subset only; all eight are World records |
| Legacy `test-registry.json` | 696 | Test-bearing source files | Legacy file registry, not Fact/Theory Truth records |
| Legacy attributed-test audit | 2552 | Historical working-tree definitions | Historical inventory; not current HEAD and not a Truth registry |
| B final-sweep report | 500 | World file-level aggregate records | 500 explicit file boundaries; their table attributes 1824 definitions, not all current World definitions |
| A final remediation | 8 retired | Core implementation-mirror definitions | Removes eight candidate definitions; does not create replacement Truth records |
| Prior four-domain headline | about 1540 | Mixed file/method/review units | Arithmetic sum only; not an authoritative TruthRecord total |

The B report's 500 rows sum to 1824 attributed definitions. This does not
equal World HEAD `1867` or candidate `1872`; therefore the B aggregate cannot
be promoted to complete World definition coverage without a member manifest.

## Required reconciliation totals

These values distinguish source inventory from truth inventory:

| Metric | Value | Status |
|---|---:|---|
| TOTAL TEST DEFINITIONS, committed HEAD | 2906 | VERIFIED by source scan |
| TOTAL TEST DEFINITIONS, 2903 candidate tree | 2903 | VERIFIED by candidate scan |
| TOTAL RUNTIME CASES, recorded candidate execution | 3332 | Inherited execution report; no new test run |
| Persisted central Truth Records | 8 | VERIFIED, subset only |
| 1:1 Truth Records for all current definitions | 0 proven | No complete identity manifest exists |
| Approved aggregate Truth Records covering all definitions | 0 proven | B aggregate covers only 1824 attributed World definitions |
| Unmapped definitions | 2906 at committed HEAD | Truth mapping absent, not zero |
| Duplicate mappings | 0 detected in the 8-record central subset | Global duplicate proof unavailable |
| Global unreviewed definitions | 2906 at committed HEAD | Not closed |

For the 2903 candidate tree, the same ledger rule yields 2903 unmapped
definitions until a complete TruthRecordId manifest is produced. The number
must not be reduced by subtracting runtime cases or by treating a file-level
review row as dozens of method-level records without explicit membership.

## Why 1540 differed from 2903

1. Core's `362` was a Fact/Theory definition count before eight prototype
   definitions were retired; it was not a persisted central Truth registry.
2. World `500` counted reviewed source files, not its `1867`/`1872` method
   definitions.
3. WarCore's quoted `18` is a historical review-unit estimate; current source
   inventory is `17` definitions and `22` runtime cases.
4. XYUI's `660` happens to equal its current Fact/Theory definition count,
   but no complete TruthRecordId member ledger was supplied by that headline.
5. The legacy `696` registry is also file-based. It is neither `2903`
   definitions nor `1540` Truth decisions.

Thus `1540` was a cross-unit subtotal. It was never a valid denominator for
global Truth coverage.

## Final Truth decision

```text
Truth Registry global status: NOT CLOSED
UNREVIEWED: NOT ZERO / 2906 HEAD definitions lack a complete mapping
UNMAPPED: 2906 HEAD definitions
DUPLICATE: 0 proven only for the persisted 8-record subset
Central registry mutation: NONE
Test/product code mutation: NONE
Commit/Push: NONE
```

The requested final condition `UNMAPPED = 0`, `DUPLICATE = 0`, and
`UNREVIEWED = 0` is not factually established by the current artifacts.
Achieving it requires a new explicit identity manifest or a formally approved
aggregate manifest whose member-definition boundaries are enumerable. That is
inventory construction, not a license to copy Truth Records or rerun the
previous domain audits.

## Reproducibility commands

HEAD definition counts:

```powershell
git grep -n -E '^\s*\[(Fact|Theory)(Attribute)?(\([^]]*\))?\]' HEAD -- XuanYu.Core.Tests/*.cs XuanYu.World.Tests/*.cs XuanYu.WarCore.Tests/*.cs xyui/avalonia/tests/XYUI.Avalonia.Tests/*.cs
```

Candidate-tree counts use the same expression over the four current source
roots. Existing candidate execution totals are recorded in
`docs/governance/xyt-test-truth-wave-t-a-report.md`; no tests were rerun for
this reconciliation task.

## KNOWLEDGE / EXPERIENCE AUDIT HANDOFF

请 ChatGPT 审计并沉淀知识经验。

Candidate lesson: inventory identity must be defined before Truth coverage is
computed. File-level review units, Fact/Theory definitions, and expanded
runtime cases are different ledgers and cannot be added as if they were the
same object.

CHATGPT KNOWLEDGE AUDIT REQUIRED
