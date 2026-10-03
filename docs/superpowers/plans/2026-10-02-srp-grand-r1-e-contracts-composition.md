# SRP-GRAND-R1-E Contracts and Composition Audit Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Audit the real cross-domain dependency graph and resolve Shared Contracts / Composition Root ownership without inventing absent A/B/C/D requests.

**Architecture:** Treat `XuanYu.Core` and `XuanYu.Render.Abstractions` as existing low-level contract candidates. Keep World, Render, Editor, and UI implementations behind their current project boundaries; move concrete construction only after the missing contract inbox supplies exact signatures and owners.

**Tech Stack:** C#/.NET 10, MSBuild project references, Avalonia UI, Vulkan backend, xUnit contract tests, PowerShell resolver chain.

**Spec:** `TASK: SRP-GRAND-R1-E-CONTRACTS-COMPOSITION` supplied in the task message.

## Global Constraints

- Do not Commit or Push.
- Do not add GlobalService, ServiceLocator, universal Context, universal ApplicationState, or shared mutable global state.
- Contracts must not depend on Render, World, Editor, or UI implementations.
- Preserve all existing ForeignDirty and untracked material.
- Every new hand-written `.cs` / `.axaml` / `.js` file must remain within the repository 5+100 rule.

## Review Focus

- Missing A/B/C/D request records: report BLOCKED instead of inventing DTOs or ownership.
- UI-instantiated World/Editor state: classify as Composition leakage, not silently re-label it as DI.
- Project-reference direction: test actual `.csproj` edges, not a diagram-only target.
- Immutable snapshots versus mutable services: retain existing snapshot/event boundaries.
- Real application composition: distinguish bridge construction from World/Editor/UI state construction.

### Task 1: Baseline and Before Graph

- [x] Run `scripts/xye-bootstrap.ps1` and record Resolver/SDK evidence.
- [x] Capture branch, HEAD, remote divergence, and dirty state without synchronization or cleanup.
- [x] Inspect project references and concrete construction sites.
- [x] Produce the required Before Graph and classify cycles, reverse dependencies, concrete edges, service locators, and cross-domain construction.

### Task 2: Contract Inbox and Owner Resolution

- [x] Search the repository for A/B/C/D `CROSS_DOMAIN_CONTRACT_REQUEST` records.
- [ ] Do not fabricate absent request fields or public contract signatures.
- [x] Resolve the four named owners only from existing implementation evidence; mark unproven ownership as unresolved/blocking.

### Task 3: Architecture Gate Decision

- [x] Inspect existing architecture/contract tests.
- [ ] Add or change architecture tests only after the request inbox and target contracts are available.
- [x] Report the current Composition Root boundary and residual leakage with exact file evidence.
- [x] Stop with `BLOCKED` when required contracts are not supplied.
