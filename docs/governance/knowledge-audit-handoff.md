# Knowledge / Experience Audit Handoff

本文件是所有施工任务最终报告尾部的唯一规范文本。施工 Agent / Codex 必须在报告末尾原样附加以下交接块；该块不是知识条目，也不代表施工 Agent 已完成正式知识写回。

```text
==================================================
KNOWLEDGE / EXPERIENCE AUDIT HANDOFF
==================================================

请 ChatGPT 审计本任务结果，并判断是否存在可沉淀的知识/经验。
如有，必须仅在本报告的 `XYK CANDIDATE` 区块中以纯文本输出，
禁止写入任何知识库、经验库、Codex memory、ad-hoc note 或其它文件；
正式入库由 ChatGPT / XYK Coordinator 后续决定。

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

AI AGENTS DO NOT HAVE DEFAULT XYK WRITE AUTHORITY.
候选不得被宣称为已沉淀、已写入或已更新；只有明确授予 XYK WRITE AUTHORITY
时，ChatGPT / XYK Coordinator 才能另行审计并维护正式 XYK。

CHATGPT KNOWLEDGE AUDIT REQUIRED
```

## 职责边界

施工 Agent / Codex 只提供事实、Root Cause、修复方式、Changed Files、Tests、Evidence、Known Risk，并只能在最终报告纯文本中提出 XYK Candidate；不得自行把 Candidate Lesson 宣布为正式 Knowledge / Experience，也不得写入任何持久化知识存储。

ChatGPT 审计 AI 负责搜索既有条目，判断 `CREATE / UPDATE / STRENGTHEN / RETIRE / NO DEPOSIT`，维护正式 Knowledge、Lesson、Incident、ERR、EXP 与索引。

正式 XYK 唯一权威位置固定为 `xyk/main` 分支的 `xyk/**`；产品分支 `docs/knowledge/**` 仅为历史只读镜像，不得继续写入正式知识。

审计顺序固定为：

```text
SEARCH EXISTING → MATCH → UPDATE / STRENGTHEN
                             ↓ no match
                           CREATE
```

Knowledge 是长期稳定的架构事实、接口契约、系统规律或验证后的技术约束；Experience 是踩坑记录、错误路径、诊断方法、修复方法、返工原因或过程治理经验。两者不得混为同一类，也不得为每个 Bug 无脑新建条目。

适用范围覆盖 FIX、FEATURE、REFACTOR、PERFORMANCE、GOVERNANCE、INCIDENT、UI、RENDER、TERRAIN、CAMERA 及后续所有施工任务。

该交接块与 Version Event、5+100、Handoff、Commit Mutex、精确 Stage、`git diff --check` 同时生效；它不改变既有产品验收、版本计数或 Git 收口规则。
