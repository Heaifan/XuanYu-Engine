# Deferred Capability Contract

## Purpose

“现在先不做”“以后再做”“先铺接口”“未来支持”都必须登记为可审计的 Deferred Capability Debt。延期不是永久 TODO、Future 或 Later；它是一个有边界、有触发器、有解除条件的正式状态机。

## State machine

```text
DEFERRED-SAFE -> ARMED -> BLOCKING -> CLOSED
```

- `DEFERRED-SAFE`: 当前临时方案仍处于明确有效范围。
- `ARMED`: 已接近失效边界；当前 Milestone 必须处理。
- `BLOCKING`: 至少一个显式 Activation Trigger 已成立；对应能力禁止继续扩建。
- `CLOSED`: Required Closure 已完成，并有 ClosureEvidence 验证。

Gate 不会从自然语言猜测触发器。任务必须显式提供 `CapabilityTags`；自动关键词只能作为 Warning，不能成为唯一事实来源。

## Required debt record

注册表位于 `docs/governance/deferred-capabilities.json`，根对象必须有 `debts` 数组。每条 debt 必须具有以下字段：

| 字段 | 规则 |
| --- | --- |
| `DebtId` | 全局唯一、非空 |
| `Title` | 非空的人类可读标题 |
| `CurrentTemporarySolution` | 当前临时方案，非空 |
| `DeferredCapability` | 被延期的能力，非空 |
| `ReasonForDeferral` | 延期原因，非空 |
| `ValidEnvelope` | 明确保质期与适用边界，非空；缺失即 FAIL |
| `ActivationTriggers` | 非空数组；每项含 `TriggerId`、`Description`、`CapabilityTags`；缺失即 FAIL |
| `BlockedCapabilities` | 非空数组；每项含 `Name`、`CapabilityTags`；`BLOCKING` 时不可为空 |
| `RequiredClosure` | 非空数组，描述解除条件 |
| `TestDebt` | 非空数组，列出测试/验收债务 |
| `IntroducedAt` | 对象，含 `Commit`、`Version`、`Date` |
| `LastReviewedAt` | 非空日期/审查标识 |
| `Status` | 仅允许 `DEFERRED-SAFE`、`ARMED`、`BLOCKING`、`CLOSED` |
| `Evidence` | 非空数组，证明延期决策的事实 |
| `ClosureEvidence` | 必须存在；`CLOSED` 时必须为非空数组 |

所有 capability tag 都必须是显式数据。建议使用稳定的小写 kebab-case，例如 `terrain-dem-runtime`、`world-extent-ge-10km`。

## Gate contract

调用 `scripts/governance/deferred-capability-gate.ps1` 时，使用 `-RegistryPath` 指定注册表，并使用 `-CapabilityTags` 指定当前任务声明的能力标签。Gate 执行：

1. Registry Validation、DebtId 唯一性、必填字段与状态合法性验证。
2. 对每个 debt 以显式 tag 交集判断 Activation 状态；命中时，未关闭 debt 的有效状态至少为 `BLOCKING`。
3. 对 `BLOCKING` debt 以显式 tag 交集判断 Blocked Capability；任务命中时整体输出 `BLOCKED`。
4. 不相关任务不因仓库存在其他 `BLOCKING` debt 而阻塞，输出 `PASS`。

输出至少包含：`DEFERRED CAPABILITY GATE`、`DEBT ID`、`STATUS`、`TRIGGER`、`VALID ENVELOPE`、`BLOCKED CAPABILITY`、`REQUIRED CLOSURE`。

## Registered case

`RENDER-LARGE-WORLD-001` 当前为 `BLOCKING`。这表示 Camera-relative Rendering / Render Origin 的延期能力已到达治理阻塞状态；它不表示 DEM Bug 已修复。其解除条件是 Camera-relative Rendering、Render Origin / Rebasing、50/100/500/1000 km 自动测试、Real Vulkan + Real DEM Runtime 验收及最终用户验收全部具备验证证据。
