# XYT-H P3 Real Runtime Harness R1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 建立不把 Headless 冒充 P3 的真实 XYE Runtime Harness。

**Architecture:** 独立 PowerShell Harness 启动真实 App，采集进程输出和真实窗口 resize，按 Capability 校验严格 marker 契约，生成 JSON/LOG evidence。Selftest 使用受控子进程验证 Harness 生命周期，不触碰产品代码。

**Tech Stack:** PowerShell、Win32 `SetWindowPos`、现有 `resolve-dotnet.ps1`、JSON。

**Spec:** `docs/governance/xyt-runtime-harness.md`

## Global Constraints

- P3 PASS 只能声明 `REAL RUNTIME AUTOMATED EVIDENCE PASS`。
- 不修改共享产品代码；缺能力输出 `BLOCKED_BY`。
- Harness 自身故障输出 `UNCLASSIFIED`，不得冒充 Product FAIL。
- 记录完整 P3 身份字段；不解除 `INC-2026-09-29-002`。

## Review Focus

- Headless marker injection must never satisfy missing real App markers: selftest only uses fake child for Harness mechanics.
- Timeout must kill the process tree and still emit evidence.
- Non-zero product exit and Harness exception must remain distinct.
- Resize evidence must come from a real top-level HWND, not a synthetic state.
- P3-03/P3-04 missing product probes must remain BLOCKED_BY.

### Task 1: Runtime contract and collector

**Files:** `XYT/Runtime/xyt-runtime.ps1`, `XYT/Runtime/xyt-runtime.schema.json`, `scripts/governance/xyt-runtime.ps1`

- [x] Add capability marker contracts and identity/evidence writer.
- [x] Add real App launch through Resolver Chain and scoped process-tree cleanup.
- [x] Add real HWND resize pulse for P3-02.

### Task 2: Harness selftest and governance documentation

**Files:** `XYT/Runtime/xyt-runtime.selftest.ps1`, `scripts/governance/xyt-runtime.selftest.ps1`, `docs/governance/xyt-runtime-harness.md`, `docs/governance/xyt-runtime-capabilities.json`

- [x] Cover normal exit, timeout, crash, evidence generation, and Harness-fault isolation.
- [x] Document P3 boundary, status semantics, evidence fields, and T0 lock behavior.

### Task 3: Verification

- [ ] Run Runtime Harness selftest.
- [ ] Run `git diff --check` and 5+100/architecture guards as applicable.
- [ ] Attempt real P3-01/P3-02 only after build/runtime availability is verified; report evidence result without Product PASS.
