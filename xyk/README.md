# XYK · 玄域知识平面

XYK（XuanYu Knowledge）与 XYE（Engine）、XYUI（UI）共同组成玄域项目三位一体治理结构。

## Git 模型
- 产品平面：当前 XYE / XYUI 产品分支。
- 知识平面：`xyk/main`。
- XYK 只维护 `xyk/**` 的长期工程知识资产。
- K Commit 不推进 XYE 产品版本，也不要求产品工作区切换到 `xyk/main`。

## 权限
- ChatGPT：XYK 正式审计与写回 Owner。
- Codex / Gemini / 其他施工 Agent：XYK 只读消费者，只能提出 Candidate Knowledge / Experience。
- 正式写回必须经过 ChatGPT Knowledge Audit。

## 施工 Agent 读取方式
施工 Agent 禁止 checkout / merge / rebase / cherry-pick `xyk/main`，只允许：
```text
git fetch origin xyk/main
git show origin/xyk/main:xyk/index/knowledge-index.md
git show origin/xyk/main:<相关条目>
```

任务开始必须记录 Knowledge SHA、Loaded IDs 与 Conflict Check。

## 写回流程
```text
施工报告
→ CHATGPT KNOWLEDGE AUDIT REQUIRED
→ SEARCH EXISTING
→ CREATE / UPDATE / STRENGTHEN / SUPERSEDE / RETIRE / NO DEPOSIT
→ K Event
→ K Commit to xyk/main
```

首个迁移来源产品 Commit：`b0e6d397829a6cd5ec94624b1de062644c37decf`。
