# XYE / XYUI / XYK 三位一体知识平面

**ID：** DEC-XYK-KNOWLEDGE-PLANE  
**状态：** ACTIVE  
**批准日期：** 2026-09-28  
**适用范围：** XuanYu-Engine GitHub 仓库、产品开发、知识审计、多 Agent 协作。

## 决策

玄域项目形成三位一体：

- XYE：Engine 产品开发平面；
- XYUI：UI 组件开发平面；
- XYK：Knowledge 长期知识平面。

XYK 与产品代码位于同一 GitHub 仓库，但使用独立分支 `xyk/main` 和 `xyk/**`。产品施工分支不合并 XYK K Commit；施工 Agent 通过远端只读获取知识。

## 权限

- ChatGPT：XYK READ + WRITE，负责正式 Knowledge Audit、K Event 与 K Commit。
- Codex / Gemini / 其他施工 Agent：XYK READ ONLY，可提出 Candidate，不得正式写回。

## 开工合同

Planning 与 Execution 两阶段都应读取最新 `origin/xyk/main`，记录 Knowledge SHA、Loaded IDs 与 Conflict Check。禁止仅依赖 Agent Memory。

## 收工合同

施工报告必须以 `CHATGPT KNOWLEDGE AUDIT REQUIRED` 结束。ChatGPT 按 `SEARCH EXISTING → UPDATE/STRENGTHEN → CREATE` 审计，必要时产生 K Commit。

## Git 隔离

K Commit：
- 不推进产品 Process Version；
- 不要求产品工作区 clean；
- 不改变产品分支 ahead/behind；
- 不得通过 checkout/merge/rebase/cherry-pick 进入产品工作树。

## 目的

让开发事实持续沉淀为可复用知识，同时避免知识写回与正在进行的产品 Dirty/Convergence 相互阻塞。
