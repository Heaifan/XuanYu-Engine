# 玄域引擎工程知识库

> 最后治理更新：2026-09-16  
> 目的：把经过工程证据验证的知识变成未来任务的主动输入，而不是被动归档。

---

## 1. 职责

知识治理采用以下分工：

```text
开发宪法        = 不可违反的长期底线
DEC             = 用户已经批准的重要长期决策
Knowledge       = 经工程实践验证的系统 / 工程规律
Lesson          = 错误前提、停止条件与复盘教训
ERR             = Agent 实际犯错事实与根因
EXP             = 从一个或多个 ERR 提炼的防复发规则
Knowledge Index = 任务域 → 开发前应读取什么
Changelog       = 实际发生的有效变化
File Tree       = 当前文件与职责
Plan / Audit    = 过程材料，不自动成为长期知识
```

知识库不是 changelog 副本，也不复制宪法和正式架构文档全文。

## 2. 当前结构

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
├─ lessons.md
└─ decisions/
   └─ 已批准的长期决策文档
```

`decisions/` 是当前仓库已经存在并正式使用的 DEC 容器，不再使用“知识目录绝对禁止子目录”的旧规则。

除 `decisions/` 或以后经用户批准的新长期类别外，不得为了单条 Knowledge 机械创建大量小目录和小文件。工程知识仍优先按主题集中维护。

Agent 错误与防复发经验不放入本目录，权威位置固定为：

```text
docs/governance/agent-error-log.md
docs/governance/agent-experience-rules.md
```

## 3. 知识闭环

长期知识必须进入开发前和开发后的闭环：

```text
真实项目事实 / Git / 验收 / 测试
            ↓
 Incident / ERR / Lesson
            ↓
 Knowledge / EXP / DEC
            ↓
     knowledge-index.md
            ↓
      Knowledge Preflight
            ↓
          开发
            ↓
        验证与回写
            ↓
 Regression Test / Runtime Gate / Architecture Gate
```

只写入而不在下一次任务加载的知识，视为治理未完成。

## 4. Dual Knowledge Preflight

MEDIUM / HIGH 任务，以及 `knowledge-index.md` 已经建立任务域映射的工作，必须执行两个独立阶段：

```text
Planning
↓
Knowledge Preflight
↓
Task Plan
↓
Handoff / Task State
↓
Execution Knowledge Preflight
↓
Code
↓
Validation
↓
Knowledge Writeback
```

### Planning Preflight

发生在任务书、设计方案或修改计划生成之前，防止计划本身违反历史合同：

1. 识别任务域；
2. 查 `knowledge-index.md`；
3. 读取该域直接相关的 DEC / Knowledge / Lesson / EXP；
4. 在 Task State 中列出实际加载 ID；
5. 提取 Hard Constraints；
6. 对计划执行 Active Knowledge Conflict Gate；
7. 不读取与任务无关的全部知识库。

### Execution Preflight

发生在 Handoff JOIN 与 Task State 之后、第一处文件写入之前。执行 Agent 必须基于当前 Repository HEAD 重新完成上述检查，不能把 Planner 已经查过作为替代。

### Active Knowledge Conflict Gate

计划或实现若与 ACTIVE P0 Knowledge 冲突，必须停止并输出：

```text
KNOWLEDGE CONFLICT
Conflicting ID:
Planned Change:
Existing Contract:
Evidence Required To Supersede:
```

只有修改计划服从既有知识，或显式完成 Governance Supersede（更新原条目状态并提供替代证据）后，才允许继续。

推荐格式：

```text
Knowledge Preflight
Phase: PLANNING | EXECUTION
Task Domain: Data / Save
Loaded:
- K-DATA-001
- K-DATA-002
- EXP-DATA-xxx
Hard Constraints:
- ...
Conflict Check: PASS | BLOCKED
```

没有匹配项时写 `Loaded: none`。

## 5. 任务结束的知识回写

任务完成前只做一次最小判断：

```text
新增长期决策？       → DEC
形成长期工程规律？   → Knowledge
出现值得复盘的教训？ → Lesson / Incident
Agent 真正犯错？      → ERR
产生防复发通用规则？ → EXP
```

全部为否时：

```text
Knowledge Writeback: none
```

不得为了“完成知识流程”制造空洞条目。

### 5.1 ChatGPT Knowledge / Experience Audit Handoff

施工 Agent / Codex 的最终报告必须附加 `docs/governance/knowledge-audit-handoff.md` 固定尾部。施工 Agent 只提供事实与 Candidate Lessons；ChatGPT 审计 AI 负责正式维护，并且必须先执行：

```text
SEARCH EXISTING → MATCH → UPDATE / STRENGTHEN
                             ↓ no match
                           CREATE
```

正式判定包括 `CREATE / UPDATE / STRENGTHEN / RETIRE / NO DEPOSIT`。Knowledge 表示长期稳定的架构事实、接口契约、系统规律或验证后的技术约束；Experience 表示踩坑、错误路径、诊断/修复方法、返工原因或过程治理经验。未经 ChatGPT 审计的施工结论不得进入正式 Knowledge / Experience 库。

`CHATGPT KNOWLEDGE AUDIT REQUIRED`

## 6. 正式 Knowledge 最低要求

每条正式 Knowledge 应至少包含：

- 唯一 ID；
- 当前状态；
- 优先级；
- 证据等级；
- 标签 / 适用范围；
- 首次确认的绝对日期、版本、Commit（能确认时）；
- 最近验证证据；
- 问题与根因；
- 工程规则；
- 禁止做法；
- 正确做法；
- 真实历史示例；
- 未来应用示例；
- 验证方法；
- 适用边界；
- 关联 Incident / Lesson / DEC / ERR / EXP。

Commit 无法确认时必须写 `待补证`，不得猜测。

## 7. 证据等级

- **E1 — 单次工程证据**：至少一次明确真实项目证据；
- **E2 — 重复工程证据**：不同版本、模块或独立事件重复证明；
- **E3 — 制度化工程合同**：已经由自动测试、Runtime Gate、Architecture Gate、静态检查等机器机制长期保护。

证据等级只能按真实证据升级。

## 8. 当前状态语义

长期治理的统一顶层状态语义：

```text
ACTIVE
SUPERSEDED
RETIRED
```

既有 Knowledge 中的 `Candidate / Active / Reinforced / Superseded / Retired` 历史成熟度可以继续保留；其有效性必须能映射到：当前有效 / 已替代 / 已退役。

旧条目不得因过时无痕删除。

## 9. 去重原则

新增前先搜索已有条目。

同一根因：

- Knowledge 优先强化原 ID；
- ERR 保留每次真实事件；
- EXP 优先累计 Occurrences，不重复造近义规则；
- 新决策若替代旧 DEC，旧 DEC 标记 SUPERSEDED 并链接新项。

数量不是知识库质量指标。

## 10. 决策知识 DEC

`docs/knowledge/decisions/` 保存已经由用户批准、未来开发必须知道的重要长期决策。

DEC 应回答：

- 决定了什么；
- 为什么；
- 适用范围；
- 哪些方案已经被否决；
- 什么条件下可以重新讨论；
- 当前状态；
- 相关 Knowledge / ERR / EXP / 架构文档。

普通实现细节、临时方案和未批准建议不得写成 DEC。

## 11. Milestone Knowledge Review

正式 Milestone 在 CLOSED 前执行一次知识沉淀审计；小 Fix 不机械创建重型报告。

候选只进入一种：

```text
KNOWLEDGE
LESSON
ERR
EXP
CHANGELOG_ONLY
BACKLOG
REJECTED
CONSTITUTION_CANDIDATE
```

`CONSTITUTION_CANDIDATE` 只能由用户 / ChatGPT 审批后进入宪法。

## 12. 自动化优先

反复出现或高风险知识，优先转化为：

```text
Knowledge / EXP
→ Regression Test
→ Static Check / Runtime Gate / Architecture Gate
```

能够机器防止的问题，不长期只依赖 AI 记住。

## 13. 月度健康检查

至少检查：

- Index 是否仍与正文一致；
- 任务是否真的执行 Knowledge Preflight；
- 是否存在长期待补证；
- 是否出现重复 Knowledge / EXP；
- ACTIVE 条目是否已被新架构推翻；
- SUPERSEDED / RETIRED 链接是否正确；
- ERR 是否存在长期未处理；
- 高风险经验是否适合自动化；
- 知识库是否开始复制 changelog、宪法或架构文档。

治理检查不要求每月创建一篇新报告，发现问题直接修正权威资料即可。
