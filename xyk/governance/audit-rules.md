# Knowledge / Experience Audit Handoff

本文件是所有施工任务最终报告尾部的唯一规范文本。施工 Agent / Codex 必须在报告末尾原样附加以下交接块；该块不是知识条目，也不代表施工 Agent 已完成正式知识写回。

```text
==================================================
KNOWLEDGE / EXPERIENCE AUDIT HANDOFF
==================================================

本任务施工结果已完成，请 ChatGPT 作为审计 AI：

1. 审计本任务的 Root Cause、修复方式、测试证据和治理过程；
2. 判断是否存在值得长期沉淀的 Knowledge / Experience / Incident Lessons；
3. 已有同类知识优先更新，不重复创建；
4. 新结论推翻旧知识时，必须保留历史追溯并修订状态；
5. 无长期价值时输出 NO KNOWLEDGE DEPOSIT；
6. 未经 ChatGPT 审计的施工结论不得直接作为正式知识库/经验库条目。

CHATGPT KNOWLEDGE AUDIT REQUIRED
```

## 职责边界

施工 Agent / Codex 只提供事实、Root Cause、修复方式、Changed Files、Tests、Evidence、Known Risk，并可提出 Candidate Lessons；不得自行把 Candidate Lesson 宣布为正式 Knowledge / Experience。

ChatGPT 审计 AI 负责搜索既有条目，判断 `CREATE / UPDATE / STRENGTHEN / RETIRE / NO DEPOSIT`，维护正式 Knowledge、Lesson、Incident、ERR、EXP 与索引。

审计顺序固定为：

```text
SEARCH EXISTING → MATCH → UPDATE / STRENGTHEN
                             ↓ no match
                           CREATE
```

Knowledge 是长期稳定的架构事实、接口契约、系统规律或验证后的技术约束；Experience 是踩坑记录、错误路径、诊断方法、修复方法、返工原因或过程治理经验。两者不得混为同一类，也不得为每个 Bug 无脑新建条目。

适用范围覆盖 FIX、FEATURE、REFACTOR、PERFORMANCE、GOVERNANCE、INCIDENT、UI、RENDER、TERRAIN、CAMERA 及后续所有施工任务。

该交接块与 Version Event、5+100、Handoff、Commit Mutex、精确 Stage、`git diff --check` 同时生效；它不改变既有产品验收、版本计数或 Git 收口规则。


## XYK Authority

XYK 正式知识平面位于同仓库独立分支 `xyk/main`。ChatGPT 是正式 Knowledge Audit / Writeback Owner；Codex、Gemini 与其他施工 Agent 仅拥有只读消费权。

施工 Agent 禁止 checkout、merge、rebase、cherry-pick `xyk/main`，也禁止直接修改 XYK。任务开始只通过 fetch/show/search 读取远端最新 Knowledge SHA 与相关条目；任务结束只提交 Candidate Knowledge / Experience 与 `CHATGPT KNOWLEDGE AUDIT REQUIRED`。

K Commit 不推进 XYE Product Version，也不得改变当前产品分支的 ahead/behind。
# XYK C0 / Forensic / Convergence / Integration Reporting Contract

本合同适用于所有 C0、Forensic、Convergence、Integration 最终报告，并覆盖报告正文、Summary、Acceptance 与 Commit 裁决。Lane 报告是局部证据；只有 Coordinator 可以输出全局裁决。

## Lane 与 Coordinator 权限

Lane 可以输出：

- `Local Regression`
- `Consumer Impact`
- `Local Gate`
- `Ownership`
- `Classification`

Lane 禁止输出具有全局含义的：

- `Global Commit Eligibility`
- `Product Closed`
- `Release Ready`
- `Integration PASS`

`Lane PASS` 不等于 `Integration PASS`。`XYE Consumer Impact: NO` 或 `Blocking Failures: 0` 只能证明对应 Lane 的局部判断，不能证明全局无阻塞或可以 Commit。

## 强制顶层状态合同

所有适用报告末尾必须存在以下字段：

```text
PRODUCT REGRESSION:
CONFIRMED / NONE / UNKNOWN

UNRESOLVED UNKNOWN:
<number>

GATE STATUS:
GREEN / RED

CANDIDATE TREE MATCH:
YES / NO

COMMIT ELIGIBILITY:
YES / NO
```

`CANDIDATE TREE` 是最终计划 Stage / Commit 的文件集合及其确定依赖状态；`VALIDATION TREE` 是实际参与 Build / Test / Acceptance 的工作树状态。两者不一致时，`CANDIDATE TREE MATCH = NO`，必须 fail-closed，即使局部 Build、Test、Dedicated Gate 或 Manual Acceptance PASS，也不得宣布 Commit、Release 或 Product Closed。

Dirty dependency 只要参与编译、运行、测试或行为验证，即属于 Validation Tree；不得以 Lane Ownership 排除。只能通过独立正式收口形成稳定 Commit Baseline，或在不包含该依赖的真实 Candidate Tree 重新执行同等级 Gate 并 PASS 来解除 Mismatch。

## COMMIT ELIGIBILITY 裁决条件

`COMMIT ELIGIBILITY = YES` 只有在以下全部成立且 Coordinator 明确裁决时才允许：

1. `PRODUCT REGRESSION = NONE`
2. `UNRESOLVED UNKNOWN = 0`
3. `GATE STATUS = GREEN`
4. `CANDIDATE TREE MATCH = YES`
5. `True ForeignDirty = 0`
6. 当前 Candidate 已完成要求的 Build / Test / Guard
7. Coordinator 明确裁决

任一条件失败，必须输出 `COMMIT ELIGIBILITY = NO`。Lane 不得自行宣告 YES。

本轮 `XYK-C0-GOVERNANCE-SEDIMENT-R1` 的 Incident Evidence 仅供本次追溯：`XYE Dirty Paths = 68`、`XYE OwnDirty = 53`、`XYUI known dirty = 15`、`True ForeignDirty = 0`、`Candidate Tree Match = NO`、`Confirmed Current-Wave Regression = 4`、`Unresolved Unknown = 11`。这些数字不得固化为永久默认值。

## 2026-10-31 补录常态化

Codex、Gemini 或其他 Agent 的每份任务报告如含 XYK / XYK CANDIDATE 或明确可复用经验，ChatGPT XYK Owner 必须先搜索既有条目，按 CREATE / UPDATE / STRENGTHEN / SUPERSEDE / REJECT / NONE 裁定，并说明 SourceProductCommit 与知识 K Commit 的不同。未推送产品源码的证据须明确保留 - 和局部状态。正式知识修改需同步索引、K Event、远端提交；任何 SourceFingerprint、测试、GPU、Editor、Git 发布事实未经验证不得从报告自动推断。当前补录范围及待补证清单见 xyk/governance/backfill-2026-10-r1.md。

## 2026-10-31 R2 历史对话反向审计与专用候选登记

通过定向检索10月份聊天历史生成R2审计，分别保存已批准且可追溯知识与尚未验证的Candidate，不得把候选变成ACTIVE或事故已证实。审计记录见 xyk/governance/conversation-audit-2026-10-r2.md，待核验候选见 xyk/governance/conversation-candidates-2026-10-r2.md。下一轮Codex开始之前，需核对产品仓既有文档约定，将XYK草案写入专用Changelog，而不是发布CHANGELOG；该草案由ChatGPT知识Owner去重裁决后才正式写入xyk/main并给K Commit回执。历史曾拟用K-REN-006命名ReferencePlane规则，与现正式Renderer K-REN-006冲突，已收敛至K-REN-004，禁止重新分配。

## 2026-10-31 R3｜Editor Candidate三态审计及10月1日历史MCP决策

XYE-EDITOR-R1-H-R2 的产品Changelog blob ae4834560cd3c5015a903aeceb19c3ee0ef6ae2d 属 WORKTREE WRITTEN / GIT UNCOMMITTED，不是产品Git提交。CAND-001正式强化K-GOV-002及K-ARCH-003，CAND-002无Runtime证据继续DEFERRED，CAND-003拒绝新增通用知识（K-DATA-001/002已覆盖）且保留Editor本地整改。知识与产品证据分层。另于10月1日用户批准过MCP Server-only、外部Host、Editor Bridge、MCP不可修改源码等设计条款，已收DEC-XYE-MCP-SERVER-20261001；本次只收长期设计，不称运行验收。审计报告见xyk/governance/conversation-audit-2026-10-r3.md。
