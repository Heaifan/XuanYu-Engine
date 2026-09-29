# XYT-B 测试身份证与 Registry

状态：ACTIVE
任务：`XYT-B`
Registry 根：`docs/governance/test-registry.json`
能力映射：`docs/governance/test-capability-map.json`

## 1. 目的与边界

任何被称为“正式测试”的测试，必须先在 Registry 中有唯一测试身份证。Registry 是测试登记和检索事实源；测试代码、测试运行日志和门禁报告仍分别是实现、执行和结果事实源。

身份证中的 `evidenceLevel` 使用 `P0~P4` 表达测试所声明的证据等级。测试若覆盖多个层级，登记最高实际覆盖等级；不得用登记等级冒充执行结果。`T0~T3` 不属于测试证据等级，而是由 XYT Incident 子系统管理的 Incident Severity。

## 2. 测试身份证字段

| 字段 | 类型 | 必填 | 规则 |
|---|---|---:|---|
| `testId` | string | 是 | 稳定编号，格式 `XYT-B-NNNN`；编号不复用。 |
| `module` | string | 是 | 归属模块，例如 `XuanYu.Core.Tests`、`XuanYu.World.Tests`、`XYUI.Avalonia.Tests`。 |
| `evidenceLevel` | enum | 是 | `P0` 至 `P4`；语义见第 3 节。 |
| `targetCapability` | string[] | 是 | `RESOLVED` 时为一个或多个 Canonical Capability Key；`REVIEW_REQUIRED` 时必须为 `[]`。 |
| `productInvariants` | string[] | 是 | 测试保护的产品不变量；不得只写“应通过”。 |
| `prerequisites` | string[] | 是 | 执行前置；无前置写 `[]`。 |
| `blockingScope` | enum | 是 | `NONE`、`TEST_SET`、`MODULE`、`RELEASE` 之一。 |
| `evidenceType` | enum[] | 是 | `STATIC`、`UNIT`、`INTEGRATION`、`REAL_RUNTIME`、`PRODUCT_ACCEPTANCE`。 |
| `maxDurationSeconds` | integer | 是 | 单次最大允许时长，必须为正整数。超时必须记录为 FAIL 或 BLOCKED。 |
| `testSetVersion` | string | 是 | 格式 `TSET-<DOMAIN>-v<MAJOR>.<MINOR>`，例如 `TSET-CORE-MATH-v1.0`。 |
| `testPath` | string | 是 | 仓库相对测试文件或测试入口路径。 |
| `status` | enum | 是 | `ACTIVE`、`DEPRECATED`、`BLOCKED`。 |
| `owner` | string | 是 | 维护责任 Lane 或团队，不填写个人过程信息。 |

### 2.1 身份稳定性

- `testId` 是历史身份；测试文件移动、测试方法重命名或实现重写不得自动换号。
- 目标能力或产品不变量发生语义变化时，创建新 `testId`，旧记录标记 `DEPRECATED`，并在 `replacementTestId` 中指向替代项。
- `testSetVersion` 表示测试集合合同版本，不是 Commit SHA，也不是产品版本。
- 任何正式报告必须同时引用 `testId`、`testSetVersion` 和实际执行 Commit。

## 3. P0~P4 测试证据等级

Canonical definitions:

```text
P0 = 静态守卫
P1 = 单元验证
P2 = 集成 / 无头
P3 = 真实运行
P4 = 产品验收
```

| 等级 | 含义 | 典型证据类型 | 不得推出 |
|---|---|---|---|
| P0 | 静态守卫 | `STATIC` | 运行时、视觉、GPU 正确 |
| P1 | 单元验证 | `UNIT` | 模块组合、真实平台行为 |
| P2 | 集成 / 无头 | `INTEGRATION` | 真实 App、HWND、真实 Vulkan/GPU |
| P3 | 真实运行 | `REAL_RUNTIME` | 用户完成产品验收 |
| P4 | 产品验收 | `PRODUCT_ACCEPTANCE` | 超出登记范围的行为 |

`P0~P4` 是登记声明，不是自动 PASS。执行结果必须由对应运行记录和 XYT Gate 证明；低等级 PASS 不得升级为高等级 PASS。Incident Severity `T0~T3` 只用于事故管理，不得写入 `evidenceLevel`。


### 3.1 P4 用户最终裁决

- P4 只能表示产品验收，最终 verdict 由用户确认。
- 自动化可以生成 Capability 驱动的最小 IPO 清单，但默认状态必须是 `P4 PENDING`。
- P0~P3 的任何 PASS 都不得自动升级为 `P4 PASS`。
- Agent / Runner / Executor / Runtime Harness 不得自行填写用户验收结论。
- 仅当用户已经明确提供 PASS / FAIL 时，系统才可记录该 verdict，并绑定 Version、Commit、Branch、IPO 路径与验收项。
- IPO 生成遵守最小充分原则；过程字段 P 必须拆成可执行编号步骤，而不是一句模糊描述。

## 4. 能力映射

能力键的唯一来源是 `test-capability-map.json`。每个能力键至少定义：模块、最低 P 等级、产品不变量和阻塞默认值。

登记测试时，`targetCapability` 中每个键都必须可在映射表中找到。按能力查询时，先匹配能力键，再汇总所有 `ACTIVE` 测试；不得通过模糊标题代替能力键。

### 4.1 未解析 Capability

Legacy Migration 或新测试登记时，如果当前证据不足以可靠映射到 Canonical Capability：

- 必须记录 `capabilityResolution = REVIEW_REQUIRED`；
- `status` 必须为 `BLOCKED`；
- `REVIEW_REQUIRED` 不得写入 `targetCapability`；
- 未解析记录不得参与按 Capability 查询、Minimum Sufficient Test Set 或正式 Required Test Selection；
- 不得为了满足字段要求猜测或伪造 `CAP-*`；
- 待人工 / Governance 解析后，再填入真实 `targetCapability` 并解除对应 BLOCKED。

因此 `targetCapability` 的“一个或多个 Canonical Capability Key”要求只适用于 `capabilityResolution = RESOLVED` 的正式记录；未解析迁移记录允许为空数组，直到映射完成。

## 5. 测试集版本

格式：

```text
TSET-<DOMAIN>-v<MAJOR>.<MINOR>
```

其中：

- `DOMAIN` 使用大写 ASCII 短名，例如 `CORE-MATH`、`WORLD-INPUT`、`XYUI-RUNTIME`；
- `MAJOR` 在字段、能力语义、阻塞规则或结果解释改变时递增；
- `MINOR` 在新增测试、补充不变量或扩大覆盖而不改变既有语义时递增；
- 删除或废弃测试不回收版本号；
- 一个测试只能归属于一个当前测试集版本，历史版本保留在 Git 中。

## 6. Registry 操作约定

### 登记测试

1. 分配未使用的 `testId`。
2. 填满所有必填字段。
3. 校验能力键存在、P 等级与 `evidenceType` 一致、路径存在。
4. 将记录写入 `test-registry.json`，并更新 `updatedAt`。

### 按能力查测试

在仓库根目录执行：

```powershell
rg -n 'CAP-CORE-MATH' docs/governance/test-registry.json
```

正式工具接入后，应以 JSON 解析 `targetCapability`，只返回 `status=ACTIVE` 的记录。

### 按测试集版本输出清单

```powershell
rg -n 'TSET-CORE-MATH-v1\.0' docs/governance/test-registry.json
```

输出清单必须至少包含 `testId`、`module`、`targetCapability`、`evidenceLevel`、`testPath` 和 `status`。

## 7. 变更与审计

- 新增或修改登记必须与对应测试代码或测试集合同一变更批次提交。
- Registry 变更需要 `git diff --check` 和 JSON 解析校验。
- Registry 的存在不证明测试已执行；报告必须区分 `REGISTERED`、`EXECUTED`、`PASS`、`FAIL`、`BLOCKED`。
- 本制度只建立登记能力，不在本轮宣称现有全量测试已完成身份证迁移。
