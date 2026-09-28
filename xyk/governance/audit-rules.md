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
