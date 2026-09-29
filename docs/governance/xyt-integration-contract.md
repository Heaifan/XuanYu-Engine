# XYT-INTEGRATION-R1 Contract

状态：`INTEGRATION CONTRACT: READY`

运行时状态：`RUNTIME CONTRACT PENDING`

本文件只定义 A–K 模块之间的公开接口、结果形状和未来中央入口的责任边界。
本轮不接入中央入口，不修改 `xyt.ps1`、`xyt.bat` 或 `XYT/Runtime/**`。

## 1. 公开边界

每个模块只通过下列四项与其他模块通信：

1. Input Contract：调用方提交的 JSON 对象。
2. Output Contract：模块产生的 JSON 对象。
3. Exit Code：进程级结果；`0` 表示契约处理成功，非 `0` 表示调用失败或结果不可消费。
4. Result Schema：输出对象的 `schema` 字段和字段约束。

模块不得读取另一个模块的私有文件、私有变量或私有实现。文件路径不是契约；只有本文列出的字段是契约。

## 2. 统一 envelope

所有跨模块结果使用以下 envelope。模块可以增加自己的 `data` 字段，但不得删除必需字段。

```json
{
  "schema": "XYT-INTEGRATION.v1",
  "kind": "planner|execution|runtime|incident|witness|report|ipo|evidence",
  "runId": "string",
  "status": "PASS|FAIL|BLOCKED|PENDING|UNCLASSIFIED",
  "exitCode": 0,
  "data": {},
  "references": [],
  "warnings": []
}
```

`status=PASS` 只表示当前模块的契约处理成功；它不自动表示真实 Runtime、P4 或最终产品通过。

统一退出码：

| Exit Code | 含义 |
|---:|---|
| 0 | 输出符合本模块 Contract，可供下游消费 |
| 1 | 业务结果为 FAIL；结果对象仍可供 Report/Incident 消费 |
| 2 | BLOCKED、PENDING 或依赖未满足；不得升级为 PASS |
| 3 | 输入、Schema 或模块自身执行错误；结果不可作为产品证据 |
| 4 | Witness/证据完整性错误；必须保留为治理故障 |

## 3. 模块合同登记

| 模块 | Input Contract | Output Contract | Exit | Result Schema |
|---|---|---|---|---|
| A Entry | `change`, `mode`, `runId`, `owner` | `EntryRequest`，仅描述待执行请求 | 0/2/3 | `XYT-INTEGRATION.v1#entry` |
| B Registry | `EntryRequest` 或 `Change` | `CapabilitySet`：能力、版本、Owner、依赖 | 0/2/3 | `XYT-INTEGRATION.v1#registry` |
| C Planner | `Change`、`CapabilitySet`、约束 | `TestPlan`：`planId`、tests、依赖、预算、是否需要 P3 | 0/2/3 | `XYT-INTEGRATION.v1#planner` |
| D Executor | `TestPlan` | `ExecutionResult`：每项状态、attempt、exit、output、rootCause | 0/1/2/3 | `XYT-D-EXEC-v1.0`，外层为 integration envelope |
| E Incident | `ExecutionResult`、`RuntimeResult`、Witness/ownership 线索 | `IncidentResult`：T0–T3、classification、lock、rootCause | 0/1/2/3 | `XYT-INTEGRATION.v1#incident` |
| F Report | `ExecutionResult`、`RuntimeResult`、`IncidentResult`、`WitnessResult`、identity | `ReportResult`：T Report、status、evidence refs | 0/1/2/3 | `XYT-T-Report.v1`，外层为 integration envelope |
| G Legacy Registry | 旧 Registry 输入 | 只读归一化 `CapabilitySet` 或迁移阻断 | 0/2/3 | `XYT-INTEGRATION.v1#legacy-registry` |
| H Runtime | P3 capability、identity、harness 参数 | `RuntimeResult`：P3 真实运行结果及 markers | 0/1/2/3 | `XYT-P3-R1/1`；统一字段映射见 §5 |
| I IPO | `CapabilitySet`、Change、Required P4 scope、identity | `IpoResult`：中文 IPO 条目，默认 `P4 PENDING` | 0/2/3 | `XYT-INTEGRATION.v1#ipo` |
| J Witness | Incident/failed test、ownership、diff、环境、可复现步骤 | `WitnessResult`：证据快照、重现、完整性、是否需要附入 Report | 0/2/3/4 | `XYT-INTEGRATION.v1#witness` |
| K Evidence | Report、IPO verdict、Witness、identity | `EvidenceState`：OPEN/P4_PENDING/CLOSED/BLOCKED 及 closure reasons | 0/2/3/4 | `XYT-INTEGRATION.v1#evidence` |

A Entry 的未来实现必须由单一 Owner 持有：`Central Entry = Single Owner`。本轮只登记接口，不修改入口。

## 4. 统一 Pipeline

```text
Change
  -> Registry / Legacy Registry
  -> Planner
  -> Required Tests
  -> Executor
  -> P0/P1/P2
  -> [requiresRealRuntime]
       YES -> Runtime (P3)
       NO  -> continue
  -> Incident Classification
  -> Witness (when required)
  -> Report
  -> IPO (P4 Pending)
  -> User Verdict
  -> Evidence State / Closure Gate
```

Planner 必须把 `requiresRealRuntime` 写入 `TestPlan`，Executor 不得自行猜测。
当它为 `true` 而 Runtime 未提供可消费结果时，Pipeline 为 `BLOCKED`，不能用 Fake Runtime 结果替代。

## 5. 关键连接合同

### 5.1 Planner → Executor

Planner 输出至少包含：

```json
{
  "schema": "XYT-D-PLAN-v1.0",
  "planId": "string",
  "requiresRealRuntime": false,
  "tests": [{
    "testId": "string",
    "command": "string",
    "dependsOn": [],
    "workingDirectory": "string",
    "maxDurationSeconds": 0,
    "timeoutSeconds": 300
  }]
}
```

Executor 必须原样保留 `planId`、测试身份、依赖关系和每项状态；`FAIL`、`TIMEOUT`、`FLAKY`、`BLOCKED_BY`、`UNCLASSIFIED` 不得被压缩为 PASS。

### 5.2 Executor/Runtime → Report

Report 消费两个独立输入：`ExecutionResult` 和 `RuntimeResult`。两者必须以 `sourceKind=AUTOMATED_TEST` 或 `sourceKind=REAL_RUNTIME` 标识，并保留 `runId`、Version、full Commit、Branch、Dirty、Timestamp、EvidencePath。

Runtime 结果与普通测试结果不可互相替代。Fake/fixture Runtime 只能标为 `sourceKind=FIXTURE_RUNTIME`，只能验证串接，不得产出真实 Runtime PASS。

当前 H 的公开结果字段为 `Schema=XYT-P3-R1/1`、`Result`、`ExitCode`、`RequiredMarkers`、`MissingMarkers`、`HarnessFailure`、`BlockedBy` 等；该字段映射已经登记，但 H 的总体 Contract 因 `T0-LOCK` 保持 `RUNTIME CONTRACT PENDING`。

### 5.3 Incident/Witness → Report

Incident 至少输出 `severity`、`classification`、`lockDecision`、`rootCause`、`evidenceRefs`。Witness 不是必经步骤；当 Incident 要求复现、Ownership/依赖锁定或证据完整性证明时，Report 必须引用 `WitnessResult`。

`UNKNOWN`、`DEPENDENCY-UNCERTAIN`、`UNCLASSIFIED`、缺失 Witness 必需字段只能进入 `BLOCKED`/`UNVERIFIED`，不得静默放行。

### 5.4 IPO → P4 Pending

IPO 只生成真实中文 IPO 的 `序号/路径/输入 I/过程 P/输出 O/判定`。自动化过程只能生成 `P4 PENDING`；只有用户显式 Verdict 才能写 `P4 PASS` 或 `P4 FAIL`。P0–P3 结果不得自动升级为 P4。

### 5.5 Evidence → Closure Gate

Evidence 只有在以下条件同时满足时才能输出 `CLOSED`：

1. 自动化结果已被 Report 固化并具有完整 identity；
2. 所有 Required Tests 已有可消费结果；
3. 若 `requiresRealRuntime=true`，存在真实 `sourceKind=REAL_RUNTIME` 的 P3 结果；
4. Incident 已分类，必要 Witness 已关联且完整；
5. IPO 已有用户 Verdict，不再是 `P4 PENDING`；
6. 无未解除的 T0 lock、BLOCKED、UNCLASSIFIED 或证据完整性错误。

否则状态依次保留为 `OPEN`、`P4_PENDING` 或 `BLOCKED`，并输出可审计的 `closureReasons[]`。

## 6. Fake/fixture selftest 边界

`XYT/Integration/xyt-module-contract.selftest.ps1` 只使用内存 fixture，验证字段映射、Pipeline 顺序、状态降级和 Closure Gate。它不调用中央入口，不启动真实应用，不读取 `XYT/Runtime/**`，也不声明真实 Runtime PASS。

Selftest 的成功含义仅为：`INTEGRATION CONTRACT: READY`。真实 Runtime、真实 Vulkan、真实输入、P4 用户验收仍需独立证据。

## 7. 当前结论

其他 A–G、I–K 的接口可以继续按本文收口；H 的公开结果形状已登记但 Contract 状态为 `RUNTIME CONTRACT PENDING`。因此本轮不等待 H 空转，也不把 H 的 pending 转写成整体失败：统一 Contract READY，真实 Integration Runtime Gate 尚未 CLOSED。

## KNOWLEDGE / EXPERIENCE AUDIT HANDOFF

Candidate Lessons：

- Integration Contract 的 READY 只证明模块边界和 fixture 串接可消费，不证明真实 Runtime 或 P4 用户验收。
- Runtime Contract 可在 PENDING 时先登记稳定的结果字段映射；下游必须显式保留 Pending/Blocked，不得以 Fake 结果升级真实证据。
- Closure Gate 必须由 Evidence 汇总 Required Tests、Runtime、Incident/Witness 和用户 IPO Verdict，单个 Report PASS 不足以关闭。

请 ChatGPT 审计本任务的 Root Cause、修复方式、测试证据和以上 Candidate Lessons，先 SEARCH EXISTING，再决定 `UPDATE / STRENGTHEN / CREATE / RETIRE / NO DEPOSIT`。

`CHATGPT KNOWLEDGE AUDIT REQUIRED`
