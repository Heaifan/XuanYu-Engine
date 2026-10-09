# XYT Test Selection Contract

## Purpose

`scripts/governance/xyt-runner.ps1` converts a changed-file list or Git diff range into the smallest test set supported by XYT's fixed mapping. It selects tests and reports ambiguity; it does not grant approval or commit authority.

## Fixed mapping

1. `docs/governance/xyt-test-mapping.json` applies fixed path-to-owner, capability, and test rules.
3. Capability matches select test rules; the union is the mandatory set.
4. An unresolved path, owner, or capability produces `REVIEW_REQUIRED` and never silently reduces the set.

## Add-only rule

`-AgentTests` and valid supplement items are unioned with the fixed set. No input can subtract a fixed test. A supplement item must contain `test`, `reason`, and `source`; malformed items are reported as disputes.

## Inputs and output

- `-ChangedListPath`: UTF-8 file containing one repository-relative path per line.
- `-DiffRange`: passed to `git diff --name-only`.
- `-AgentTests`: explicit additions from an Agent.
- `-SupplementPath`: JSON object with `items`; each item is an AI/additional judgment.
- `-OutputPath`: JSON result path.

Output fields are `changedFiles`, `requiredTests`, `supplementalTests`, `agentAdditions`, `disputes`, `status`, and `rule`. `PASS` means the fixed mapping is complete. `REVIEW_REQUIRED` means the user must resolve an ambiguity before treating the result as a minimum set.

## Acceptance examples

```powershell
scripts\governance\xyt-runner.ps1 -DiffRange 'HEAD~1..HEAD' -OutputPath .xyt\plan.json
scripts\governance\xyt-runner.ps1 -ChangedListPath .xyt\changed.txt -AgentTests 'XuanYu.World.Tests\Map\MapMarkerTests.cs'
```
