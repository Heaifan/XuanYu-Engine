# Knowledge / Experience Audit Handoff

本文件规定施工任务报告中的 XYK 交接字段。每轮候选登记和 NONE 记录必须同时追加到 `docs/knowledge/xyk-changelog.md`；该 Changelog 不是正式知识条目，也不代表已完成正式知识写回。

固定交接块之前，报告正文必须包含 `Root Cause`、`Changed Files`、`Tests`、`Evidence`、`Known Risk` 与 `Candidate Lessons`。没有候选时，`Candidate Lessons` 明确写 `NONE`；存在候选时说明可复用规则及其证据，不得直接写入知识库。

```text
==================================================
KNOWLEDGE / EXPERIENCE AUDIT HANDOFF
==================================================

请 ChatGPT 审计本任务结果，并判断是否存在可沉淀的知识/经验。
如有，必须先按宪法格式登记到 XYK Changelog，再在本报告 `XYK CANDIDATE` 区块引用 Candidate ID；禁止写入正式知识库、经验库、Codex memory 或 ad-hoc note。正式入库由 ChatGPT / XYK Coordinator 后续决定。

XYK CANDIDATE:
NONE

若存在候选，替换上面的 `NONE`，并仅在本报告中按下列字段输出：
- ID/Category: <可选候选标识>
- Problem: <发生了什么问题>
- Root Cause: <根因>
- Rule / Knowledge: <可复用知识>
- Applies To: <适用范围>
- Evidence: <本任务证据>
- Expiry / Revalidation: <如适用>
- Suggested Action: <后续治理/优化建议>

AI AGENTS MAY APPEND CANDIDATE LOG ENTRIES; THEY DO NOT HAVE DEFAULT FORMAL XYK WRITE AUTHORITY.
候选日志登记不得被宣称为正式知识已沉淀、已写入或已更新。正式 XYK 维护由 ChatGPT / XYK Coordinator 审核并执行。

CHATGPT KNOWLEDGE AUDIT REQUIRED
```

每轮最终报告必须附加以下状态回执，并按 Changelog 当前事实填写：

```text
XYK CHANGELOG
- Changelog Path: docs/knowledge/xyk-changelog.md
- Candidate IDs: <IDs or NONE>
- New Entries: <number and short description>
- Review Status: <PENDING / REVIEWED / REJECTED>
- File Identity: <current Changelog blob or not yet recorded>
- XYK: CANDIDATE / NONE
```

## 职责边界

施工 Agent / Codex 提供事实、Root Cause、修复方式、Changed Files、Tests、Evidence、Known Risk，并可向专用 XYK Changelog 追加候选或 NONE 任务记录；不得把 Candidate 宣布为正式 Knowledge / Experience，也不得修改正式知识文件。

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
