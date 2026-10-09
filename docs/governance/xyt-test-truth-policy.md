# XYT-T2 Test Truth Control Plane

> HISTORICAL: this document describes the retired Wave / Lane / Merge
> Eligibility control model. It is preserved as historical XYT review material
> and does not define current execution authority or approval gates.

状态：`T2-CANDIDATE / WAVE-T-A`，不是 `T2 truth-reviewed`。

## 1. Record contract

每个 `[Fact]` / `[Theory]` 必须由 `xyt-test-truth-registry.json` 的一条 Record 表达。字段定义固定在 `xyt-test-truth-schema.json`：身份、声明、真实生产路径、Oracle、Fixture/Fake/Mock、证据层、证明边界、执行结果、Truth 判定、Decision、RED 敏感性和 Regression Witness。

`Decision` 只有：`KEEP`、`RENAME`、`STRENGTHEN`、`REWRITE`、`SPLIT`、`RETIRE`、`ESCALATE`。

`TruthStatus` 至少包括：`UNREVIEWED`、`VERIFIED`、`WEAK_ORACLE`、`WRONG_ORACLE`、`MISLEADING_CLAIM`、`TIER_OVERCLAIM`、`RED_INSENSITIVE`、`DUPLICATE`、`IMPLEMENTATION_COUPLED`、`ESCALATED`。

## 2. Execution and Truth are separate

总表必须同时显示 `ExecutionStatus` 与 `TruthStatus`。允许并且必须诚实表达：`PASS + WRONG_ORACLE`、`PASS + MISLEADING_CLAIM`、`FAIL + VERIFIED`。`PASS` 不是好测试，`FAIL` 也不自动否定 Truth。

## 3. Frozen evidence tiers

本控制面的 `EvidenceTier` 固定为 `T0`～`T4`：

| Tier | 含义 | 明确不能推出 |
|---|---|---|
| T0 | Static Guard：源形状、依赖、架构和字符串守卫 | Runtime、Visible、GPU |
| T1 | Unit / Pure Logic：局部数学、算法、状态、数据模型 | 模块组合、真实平台、视觉/GPU |
| T2 | Integration / Headless：UiVm、Headless、导入器、跨模块与非真实 GPU 集成 | Real App、HWND、真实 Vulkan、GPU、Swapchain、Present |
| T3 | Real Runtime：真实 Editor、平台窗口、Vulkan/GPU、Swapchain、Present、输入与 DPI | Product Acceptance |
| T4 | Product Acceptance：真实用户在真实产品中完成 IPO 并观察符合要求的行为 | 超出明确验收范围的行为 |

禁止从名称、目录或测试数量自动升级 Tier：Fake Surface 不得成为 T3，Headless VM 不得成为 Real Runtime，Projection Math 不得成为 Visible，Object Exists 不得成为 Rendered，Draw Submission 不得成为 Presented Pixels，单帧不得成为 Stability，小 Fixture 不得成为 Large World。

仓库既有 `test-registry.json` 的 `evidenceLevel=P0-P4` 是历史 Registry 命名空间，本轮不改写、不自动映射、不把它当作本 Record 的 `EvidenceTier`。

## 4. Version and evidence expiry

全局正式状态仍为 `XYT TestSet = T1 LEGACY`。本轮只建立 `T2-CANDIDATE / WAVE-T-A`。只有 `UNREVIEWED TESTS = 0` 才允许切换全局 TestSet Version。

每个 Evidence Record 必须包含 `ProductVersion`、`Commit`、`TestSetVersion`、`TestId`、`EvidenceTier`。修改 Oracle 或 Test Definition 后，旧证据必须变为 `REVALIDATION_REQUIRED`，触发类型为 `TEST_CHANGED`；不得保留为当前有效证据。

## 5. Ledger and merge gate

统计必须同时输出 `TOTAL`、`REVIEWED`、`UNREVIEWED`、七种 Decision、六类 Truth debt，并支持 Capability 聚合。`RETIRE` 不得因保数量而拒绝，旧测试数量不是 KPI。

Merge Eligibility 只有同时满足以下条件才可为 `YES`：

1. Registry、Schema、统计和 Capability Mapping 可解析且数量恒等；
2. `UNREVIEWED TESTS = 0`；
3. `WRONG_ORACLE`、`MISLEADING_CLAIM`、`TIER_OVERCLAIM`、`RED_INSENSITIVE` 均为 0，或每条都有明确 `ESCALATE` 阻断记录；
4. Cross-Lane 文件冲突为 0；
5. Mutation/Fault Injection 全部恢复，Regression Witness 不得伪造；
6. Handoff、Gate、Candidate Tree、Commit 与远端事实均可验证。

## 6. Cross-Lane file registry

跨 Capability 共用测试文件不得抢改。登记 `CROSS_LANE_FILE`、`PRIMARY_CAPABILITY`、`FOREIGN_CAPABILITIES`，由 Coordinator 后续分配。归属不明确时 Merge Eligibility 为 `NO`。

## 7. Non-goals

本 Lane 不判定 Capability 的业务 Oracle，不修改生产行为来迎合错误 Oracle，不删除 Required Tests，不提交未恢复的 Mutation 工作树，也不因局部 Lane PASS 宣布 XYT-T2 完成。
