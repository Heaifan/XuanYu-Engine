# 玄域引擎工程知识库

> 文档状态：V1 正式入库
> 整理时间：2026-08-10 17:56（UTC+08:00）
> 证据基线：`changelog.md`、`docs/archive/changelog/changelog-2026-07.md`、`docs/archive/changelog/changelog-2026-06.md` 与可核验 Git Commit
> 目录原则：`docs/knowledge/` 下禁止创建子目录；按大类集中到单一 Markdown 文件。

## 1. 目的

知识库保存“以后遇到类似问题应该如何判断、设计、修复与验证”的长期工程经验。它不是 changelog 的副本，也不是 AI 开发宪法、UI 规范或架构文档的重复抄写。

四类文档的职责严格区分：

- **宪法 / 规范**：必须遵守什么。
- **Changelog**：某个版本发生了什么。
- **Incident**：某次事故发生了什么、如何收口。
- **Lesson**：为什么会沿着错误前提投入，以及何时必须停止局部修补。
- **Knowledge**：从事故与教训中提炼出的可复用工程规则。

知识闭环：

```text
Changelog / Git 事实
        ↓
      Incident
        ↓ 反思
       Lesson
        ↓ 提炼
     Knowledge
        ↓ 固化
Tests / Runtime Gate / Architecture Gate
```

## 2. 文件结构

```text
docs/knowledge/
├─ README.md
├─ knowledge-index.md
├─ engineering.md
├─ architecture.md
├─ rendering.md
├─ input.md
├─ ui.md
├─ data.md
├─ performance.md
├─ incidents.md
└─ lessons.md
```

**硬规则：不得新增 `docs/knowledge/<category>/...`、`incidents/2026/...` 等嵌套目录。** 细分依靠知识 ID、标签和 `knowledge-index.md`，不依靠继续套文件夹。

## 3. 条目类型与最低字段

每条正式 Knowledge 至少包含：

1. ID、状态、优先级、证据等级、标签、适用范围；
2. 首次确认的**绝对日期/时间**、版本、Commit、来源；
3. 如有后续修正，记录最近验证版本；
4. 问题与根因；
5. 工程规则；
6. 禁止做法；
7. 正确做法；
8. **真实历史示例**；
9. **未来应用示例**；
10. 验证方法；
11. 边界 / 例外；
12. 关联 Incident / Knowledge。

每条正式 Lesson 还必须明确区分“已确认事实”和“高置信机制解释（尚未直接证明）”，并包含停止条件、禁止做法、正确做法、关联 Incident / Knowledge / Gate。

禁止使用“今天、昨天、刚才、这次、上一轮、前几天”等相对时间作为证据时间。若原始历史只记录到日期或月份，必须原样说明“原文未记录时分/具体日”，**禁止补造精度**。

## 4. Commit 证据规则

- 能从 Git 核验时，优先使用完整 Commit SHA；正文可同时给短 Hash。
- Changelog 只有短 Hash 时，允许暂存短 Hash。
- Changelog 写“以本轮最终 Git 记录为准”且当前资料无法可靠定位时，写：`待补证（Codex 入库前由本地 Git 追溯）`。
- 禁止根据版本号、文件时间或相邻提交猜 Hash。
- 历史版本号冲突时，Commit Hash 优先于版本号。

## 5. 证据等级

- **E1 — 单次事故经验**：至少有一份真实项目证据，但尚未跨版本复现。
- **E2 — 重复验证经验**：同类问题在多个版本、修复阶段或独立路径中重复得到验证。
- **E3 — 工程合同**：经验已被稳定自动测试、Runtime Gate、Architecture Gate 等机器门禁固化。

证据等级描述成熟度，不表示知识重要性。P0/E1 可以比 P1/E3 更紧急。

## 6. 优先级

- **P0**：违反后可能导致用户数据损坏、错误验收、输入失控、渲染/空间语义错误、重大返工或阶段错误关闭。
- **P1**：明显影响稳定性、性能、可维护性或调试效率，但通常不会立即破坏核心状态。
- **P2**：经验性优化，可按任务相关性采用。

## 7. 生命周期

知识状态使用：

- `Active`：当前适用。
- `Superseded`：已被新知识替代，保留历史链接。
- `Deprecated`：架构或技术栈变化后不再适用。

同一问题出现新证据时，优先更新原条目的“最近验证”和示例，不重复创建近义知识。只有工程规则本身不同，才新建 ID。

## 8. Milestone Knowledge Review 分类

每个正式 Milestone 在 `CLOSED` 前必须执行一次知识沉淀审计。审计不复制 changelog，而是基于 Milestone 的计划、Commit、验收、失败/返工记录、架构决策、测试证据和最终实现筛选可复用结论。

候选必须且只能归入以下分类：

- `KNOWLEDGE`：稳定、可复用且有真实工程证据，更新对应主题文件和索引。
- `LESSON`：有上下文的失败或教训，更新 `lessons.md`，必要时同步 `incidents.md`。
- `CHANGELOG_ONLY`：只保留发生历史，不进入长期知识。
- `BACKLOG`：真实但不在当前范围，写入现有 Milestone Backlog 或架构债务事实源。
- `REJECTED`：一次性现象、未经验证猜想或已被事实推翻，不入库。
- `CONSTITUTION_CANDIDATE`：单独报告，必须经明确批准后才能修改宪法。

审计未完成、分类未明确或落库结果未完成时，Milestone 不得标记 `CLOSED`。同一根因优先更新既有条目；知识条目仍必须满足本文件的证据、边界、历史示例和未来应用要求。

## 9. 当前验证口径提醒

历史 changelog 中存在早期“0 Error / 存量 Warning”记录，这些只作为历史事实引用。**当前玄域引擎正式构建门禁为全解决方案 0 Warning / 0 Error；任何 Warning 均视为阻塞。** 知识库不得把历史容忍口径恢复成当前规则。

## 10. 本轮入库复核

1. 本轮保持正式知识库扁平结构，不创建子目录。
2. 已搜索全文 `待补证`，只对本地 Git 能可靠定位的历史补齐 Hash；无法定位处继续保留待补证说明。
3. 已按项目治理规则同步 `file-tree.md`、`changelog.md` 与宪法条款。
4. 本轮经用户授权修订 AI 开发宪法，新增第十六章知识治理制度。
5. 正式门禁结果以本轮最终 changelog 条目和交付报告为准。
6. Commit + Push 后必须重新核验远端分支 tip 与本地 HEAD 一致，再报告交付。
