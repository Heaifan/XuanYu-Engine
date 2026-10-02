# XYE-HANDOFF-SLIMMING-R1-FINAL

> HISTORICAL：R1 收口记录。当前 Event Ledger 属于 Handclap，Handoff 仅为 ZERO-Authority Compatibility Shell。

## Status

`Handoff Slimming = FINALIZED`

最终模型：

```text
Handoff = Event Ledger + Responsibility Record + History
```

明确边界：

```text
Handoff != Controller
Handoff != Gate
Handoff != Lock
Handoff != Commit Authority
Handoff != Release Authority
```

本收口只处理 Handoff 控制面的职责边界，不关闭当前 Active Wave，不改变产品代码、Task Registry、Remote 或 Candidate 资格。

## Final acceptance rule

Legacy Control 只允许以下任一终态：

1. Legacy 存在但不可执行；旧入口只能返回明确的 retired/removed 结果，并不得写入生命周期状态。
2. Legacy 已移除且迁移记录完整；迁移记录必须能证明职责已由现行入口承接。

“仍可执行但未被调用”不通过最终验收。

## BEFORE POWER MODEL

```text
Handoff
├─ Lane lifecycle state machine
├─ Task/lane transitions
├─ Coordinator authority fields
├─ Gate / Candidate / Commit eligibility decisions
├─ Legacy lane-state control
└─ History / handoff facts
```

Handoff 与多个生命周期权威重叠，事件、状态和资格容易被混为一体。

## AFTER POWER MODEL

```text
Handoff
├─ Event Ledger
├─ Responsibility Record
└─ History

Independent authorities
├─ Task Registry      → task lifecycle
├─ Wave state         → workspace/wave control
├─ Ownership/Mutex    → file and commit authority
├─ Candidate Gate     → candidate/evidence eligibility
└─ Git/Remote         → commit and synchronization truth
```

事件是事实记录，不是状态转换；记录事件不产生 owner、release、gate 或 commit 权限。

## REMOVED AUTHORITIES

- Lane lifecycle state transitions
- Task lifecycle transitions
- Coordinator-driven lifecycle status writes from Handoff
- Gate status、Candidate Tree Match、Commit Eligibility 的裁决
- 通过旧 `lane-state` 入口恢复生命周期控制
- 通过事件记录伪造完成、释放或接管

## RETAINED RESPONSIBILITIES

- 追加 `CREATED`、`STARTED`、`TRANSFERRED`、`COMMENTED`、`COMPLETED_REPORTED`、`CANCELLED_REPORTED`
- 记录 Scope、Task、Actor、Comment 等责任事实
- 查询并审计事件历史
- 保留旧记录供迁移和审计读取
- 对 Legacy Control 提供明确的 retired/removed 行为
- 维持受控迁移入口的可追溯性

## FINAL DEPENDENCY

```text
Handoff → handoff-events.jsonl
        ├─ Event Ledger
        ├─ Responsibility Record
        └─ History
```

Handoff 不依赖并不拥有 Task Registry、Wave state、Lane state、Gate、Candidate、Lock、Commit 或 Release 资格；这些系统可以被 Handoff 查询用于审计，但不由 Handoff 写入或推进。

## Evidence

- `tools/handoff/HANDOFF-PROTOCOL.md` 已定义 Event Ledger 和旧 `lane-state` 的不可执行兼容入口。
- `tools/handoff/handoff.ps1` 已提供事件追加入口，且事件不修改 state/registry。
- `tools/handoff/handoff.selftest.ps1` 覆盖事件记录、state 不变和 Task Registry 不变。
- 现有 R4-B 治理材料保留在工作区，未被本报告覆盖。

## Scope exclusions

- 当前 Active Wave 不在本报告中关闭。
- 当前 C Task、产品 dirty、Remote lag 和 Candidate validity 不因本报告改变。
- 不恢复旧控制脚本，不新增 Coordinator、Lock 或 Lifecycle State。

## XYK UPDATE

更新既有 `K-HANDOFF-001`：补充 Handoff Slimming R1 边界，明确 Event Ledger 与 Commit Mutex recovery 是两个独立职责面。
