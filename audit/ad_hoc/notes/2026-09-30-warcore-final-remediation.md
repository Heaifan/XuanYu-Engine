# XYT-T2 C-WARCORE Final Remediation

- Scope changed only `XuanYu.WarCore.Tests/**/*.cs`; unrelated pre-existing untracked audit material was preserved. Central `docs/governance/test-registry.json` was not changed, and no commit or push was performed.
- Renamed overclaiming tests to match their actual claims: whitespace display-name rejection and preservation of distinct input values for identities/states. Strengthened SoldierState boundary assertions for BodyCondition, Stamina, Morale, and Suppression.
- Renamed dependency tests as assembly/project dependency contracts and changed the csproj check from substring matching to parsed XML `ProjectReference` inspection.
- Fresh verification: WarCore tests 22/22 PASS; WarCore architecture guard PASS; old misleading names absent; central Registry diff empty; `git diff --check` PASS.
- Enum decision: source documents declare R1 only supports Soldier, but no formal rule states whether arbitrary enum casts must be rejected. Current contract is UNSPECIFIED; observed constructor behavior is permissive. Register `CONTRACT_DECISION_REQUIRED`, not PRODUCT BUG.
- Capability classification: FactionId, OrganizationId, and direct UnitKind contract tests remain COVERAGE GAP. Combat resolution, rounds, randomness, and rule-calculation capabilities are NOT_IMPLEMENTED_CAPABILITY and must not receive fabricated tests.
