# XYE-C0 Gate Debt Cleanup R1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Clear the non-UNKNOWN Core/World stale, Headless environment, and verified UI baseline debt without changing Task A/B production behavior.

**Architecture:** Replace source-location and literal assertions with semantic source aggregation or observable behavior. Keep platform stubs inside the World test fixture. Resolve UI debt by removing actual violations or documenting each formally approved exception with evidence.

**Tech Stack:** C#, xUnit, Avalonia Headless, PowerShell, repository Resolver Chain.

## Global Constraints

- Lane is XYE; use the canonical workspace and preserve existing dirty files.
- Production files owned by Task A/B are read-only for this task.
- A suspected production regression is reported and not hidden by weakening a test.
- Every gate claim requires fresh command output.
- All hand-written C# and AXAML files remain within 100 lines.

## Review Focus

- Partial-file movement must not remove Vulkan behavior: tests search the formal type behavior across the split source set.
- Native-host and scale-indicator contracts must assert composition/behavior, not obsolete XAML spelling.
- Headless platform services must be registered only in test infrastructure.
- Baseline changes must enumerate exact violations and classify each as debt or governance-approved.

### Task 1: Core semantic contracts

**Files:** `XuanYu.Core.Tests/Render/Diagnostics/RenderLogNoiseContractTests.cs`, `XuanYu.Core.Tests/Render/DrawPlan/ViewportScaleIndicatorContractTests.cs`, `XuanYu.Core.Tests/Render/DrawPlan/ViewportChromeContractTests.cs`

- [ ] Replace the trace guard literal assertion with a semantic guard assertion.
- [ ] Replace both `<local:VulkanNativeHost/>` assertions with named-host/composition semantics.
- [ ] Run the focused Core tests and verify exactly the three stale failures disappear.

### Task 2: World stale contracts

**Files:** affected `XuanYu.World.Tests` Vulkan partial/source-location, UI static, vector-color, and baseline tests only.

- [ ] Aggregate split Vulkan partial sources by formal type and preserve behavior assertions.
- [ ] Update static UI contracts only where current frozen composition is evidenced.
- [ ] Assert vector RGB byte quantization semantically at the encoding boundary.
- [ ] Run focused stale tests and classify any remaining failures as regression or environment.

### Task 3: Headless environment

**Files:** `XuanYu.World.Tests/UiRuntime/UiHeadlessFixture.cs` and test-only support files.

- [ ] Reproduce the two platform-service failures.
- [ ] Add test-only service registration/stubs through the fixture/app builder.
- [ ] Verify affected tests without changing production registration.

### Task 4: Verified baseline debt

**Files:** `XuanYu.World.Tests/UiTokens/UiDebtBaseline*.cs` and only the corresponding UI files if an actual violation is repaired.

- [ ] Print each CsHexColor occurrence and compare it with the baseline entry.
- [ ] Remove genuine debt or add governance-backed entries only for approved exceptions.
- [ ] Prove scan count from the directory, never by changing a constant alone.

### Task 5: Closeout gates

- [ ] Run Core full tests, World full tests, Editor.UI build, ARCH-A, 5+100, and `git diff --check` through the Resolver Chain.
- [ ] Freeze and repeat the convergence snapshot before any commit decision.
- [ ] Report PRODUCT REGRESSION, UNRESOLVED UNKNOWN, GATE STATUS, CANDIDATE TREE MATCH, COMMIT ELIGIBILITY, changed-file counts, and Knowledge/Experience audit handoff.
