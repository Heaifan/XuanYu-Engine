# Agent 防复发经验规则库

> 路径：`docs/governance/agent-experience-rules.md`  
> 唯一职责：保存从一个或多个真实 ERR 中提炼出的、可复用、可注入未来任务的 Agent 防复发规则。

本文件不是错误事实库。错误事实统一记录在 `agent-error-log.md`。

---

## 1. 权限

- **ChatGPT**：可创建、修改、合并、强化、替代、退役 EXP。
- **Codex / Gemini / 其他执行 Agent**：只读；必须在相关任务中遵守已加载的 EXP，可提交新增 / 修改建议和验证证据。
- **用户**：拥有最终裁定权。

## 2. ID

格式：

```text
EXP-<TYPE>-NNN
```

TYPE 与 ERR 主类型一致：

```text
LOGIC / ARCH / TEST / SCOPE / GOVERNANCE / REPORT / REGRESSION / UI / DATA
```

## 3. 状态

```text
ACTIVE
SUPERSEDED
RETIRED
```

- `ACTIVE`：当前有效，相关任务必须加载。
- `SUPERSEDED`：已被更准确规则替代，必须写明替代 EXP-ID。
- `RETIRED`：当前技术 / 流程已不再适用，保留历史原因。

历史 EXP 不得无痕删除。

## 4. 标准模板

```markdown
## EXP-LOGIC-001 提交分层

状态：ACTIVE
适用范围：Editor / Inspector / Commit Pipeline
触发条件：涉及 Dispatcher、Command Routing、统一提交入口或领域 Commit Core
Occurrences：3

规则：
统一 Dispatcher 只负责路由、校验和记录；领域 Core 只执行领域修改；Core 不得反向调用 Dispatcher。

根因模式：
统一入口与底层执行函数职责倒置或互调，造成递归、重复提交或重复副作用。

禁止：
- Core 回调统一入口；
- 为统一记录而让底层执行层重新进入 Dispatcher。

正确做法：
- Dispatcher → Core 单向调用；
- 记录 / 路由放在入口；
- 领域修改只发生在 Core。

来源 ERR：
- ERR-YYYYMMDD-NNN

任务注入：
涉及上述触发条件时，在 Knowledge Preflight 中加载本规则。

验证 / 自动化：
说明相关回归测试、静态检查或门禁；没有时写“尚无自动门禁”。

Superseded by：
无
```

## 5. 创建规则

只有满足以下条件才创建新 EXP：

1. 至少存在一个真实 ERR；
2. 根因已经有足够证据；
3. 结论具有跨单次修复的复用价值；
4. 与现有 EXP 的根因 / 防复发机制实质不同。

一次性实现失误但无通用规律时，只保留 ERR，不强行创建 EXP。

## 6. 合并与累计

同一根因再次发生：

```text
新 ERR
→ 关联原 EXP
→ Occurrences + 1
→ 补充来源 ERR
→ 必要时强化触发条件 / 验证方式
```

不得为措辞不同但本质相同的问题创建多条近义 EXP。

## 7. 任务注入

EXP 必须能真正进入未来开发上下文。

`docs/knowledge/knowledge-index.md` 负责将任务域映射到相关长期知识；EXP 本身必须写清“适用范围”和“触发条件”。

MEDIUM / HIGH 任务或 Index 已登记任务域在开始前执行 Knowledge Preflight，并加载命中的 ACTIVE EXP。

执行 Agent 不需要全文读取所有 EXP，只加载与任务直接相关的规则。

## 8. 自动化升级

重复发生、Critical、P0 等高风险经验，应优先评估：

```text
EXP
↓
Regression Test
↓
Static Check / Runtime Gate / Architecture Gate
```

能够机器防止的错误，不应长期只依赖 Agent 记忆。

自动化门禁建立后，EXP 保留并记录对应 Gate；机器门禁不会使历史经验失去价值。

## 9. 宪法候选

符合以下任一条件，可由 ChatGPT 提出 `CONSTITUTION_CANDIDATE`：

- 同类经验跨两个及以上独立任务重复发生；
- 单次 Critical 且风险具有普遍性；
- 直接威胁架构、数据完整性或事实可信度。

执行 Agent 不得自行把 EXP 写入宪法。

宪法升级必须由用户批准并由 ChatGPT 正式修改。

## 10. 经验库健康检查

月度治理至少检查：

- ACTIVE EXP 是否仍适用；
- 是否存在重复规则可以合并；
- Occurrences 是否与 ERR 事实一致；
- SUPERSEDED 是否正确链接替代项；
- 是否存在长期依赖人工记忆但适合自动化的 EXP；
- 是否有已失效规则应 RETIRED；
- 是否有高频规则值得提出宪法候选。

---

## 当前规则

## EXP-UI-001 稳定 Inspector 编辑目标

状态：ACTIVE
适用范围：Editor / Inspector / 属性提交 / Recent MRU
触发条件：属性编辑可能在提交前经历 Selection 切换，或同一属性存在 Entity 与 Feature 多条提交入口。
Occurrences：1

规则：
编辑开始时必须捕获 `InspectorEditTarget = ObjectKind + ObjectId + PropertyKey`。Identity、Commit、Recent 必须消费同一个目标；提交不得重新读取当前 Selection 决定目标。

根因模式：
把当前 Selection 当作编辑会话身份，导致 A 开始编辑、切换到 B 后提交到 B；Recent 只按类型而不按对象身份隔离；专用入口重新进入统一入口造成递归。

禁止：
- 用提交瞬间的 `_selectedMapGeometry` 覆盖已捕获目标；
- 只按 `ObjectKind` 存储 Recent；
- 让领域 Commit Core 反向调用统一 Dispatcher；
- 只用单一 Road Happy Path 测试代表全部 Inspector 对象类型。

正确做法：
- 行编辑器或输入框获得焦点时捕获稳定目标；
- Commit 按目标 `ObjectId` 调用对应领域操作；
- Recent 按对象身份隔离；
- 覆盖 Entity、Road、Region、Marker 以及 A→B Selection 切换回归。

来源 ERR：
- ERR-20260916-001

任务注入：
涉及 Inspector、属性提交、Recent 或 Selection 切换时，在 Knowledge Preflight 中加载本规则。

验证 / 自动化：
`InspectorEntityEditTargetTests`、`InspectorPropertyTargetTests` 与 Inspector 专项回归；当前已通过 16/16。后续可继续评估静态检查或更高层运行时门禁。

Superseded by：
无
