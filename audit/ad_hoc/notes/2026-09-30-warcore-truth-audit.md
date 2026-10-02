# XYT-T2 C-WARCORE Truth Review

- Target: `E:\MyDoc\project-VSCode\XuanYuEngine`, `XuanYu.WarCore.Tests/**/*.cs`.
- Reviewed all 3 tracked test files and 22 discovered xUnit cases; `dotnet test` passed 22/22 and `arch-a-guard-warcore.ps1` passed. Target worktree was clean before and after.
- Current WarCore production scope is R1 identity/state value objects plus dependency boundaries. Combat formulas, state transitions, battle resolution, randomness, rounds/sequences, and fixtures/fakes are absent; record these as deferred capability GAPs, not passing coverage.
- Truth findings: the whitespace display-name test name overclaims empty-string coverage; the two “do not share state” tests only prove supplied values differ and should be renamed or strengthened; SoldierState boundary acceptance asserts only BodyCondition despite constructing all four fields; the csproj dependency test uses brittle substring/path assertions and should parse project references.
- No evidence of copied production formulas, same-algorithm expected values, no-op exception-only tests, constant-green booleans, or uncontrolled randomness in the current C scope.
- Contract escalation candidate: `MilitaryIdentity` accepts arbitrary enum casts although `UnitKind` currently declares only Soldier; resolve whether invalid enum values must be rejected before adding a truth test.
