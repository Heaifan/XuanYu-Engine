# MAP-A-CLOSE · 修订后收口计划

> 计划状态：OPEN · 仅完成收口流程修订，不执行 MAP-A 产品或架构收口。
>
> 当前事实：`MAP-A-R3-D2-F1` 仍为 `OPEN · FINAL ACCEPTANCE FAILED · 5 ITEMS REMAIN`；本计划不改变该状态，也不解锁 D3、F2 或其他后续产品工作。
>
> 适用治理：`docs/玄域引擎_AI开发宪法.md` 第八十六条《里程碑知识沉淀门禁》、`docs/dev-rules.md` 第 18 节。

## 一、TASK FREEZE

| ID | 任务 | 初始状态 |
|---|---|---|
| T00 | Fresh Git / Governance 基线确认 | OPEN |
| C1 | MAP-A 状态冻结 | OPEN |
| C2 | MAP-A 架构收口文档 | OPEN |
| C3 | MAP-A 知识沉淀审计 | LOCKED |
| C4 | 全量门禁 + Git 收口 | LOCKED |
| T05 | 收口报告与证据汇总 | LOCKED |
| T06 | 用户收口验收 | LOCKED |
| NEXT | `EDITOR-A-R1` | FORBIDDEN |

本计划四个核心目标：

1. `C1`：准确结束 MAP-A 当前状态；
2. `C2`：冻结下一代架构方向和未决边界；
3. `C3`：提炼 MAP-A 可复用知识与教训；
4. `C4`：以正式门禁和 Git 证据完成收口。

任何一项缺失，`MAP-A-CLOSE != CLOSED`。

## 二、阶段顺序与禁止事项

```text
C1 状态冻结
↓
C2 架构收口文档
↓
C3 MAP-A 知识沉淀审计
↓
C4 最终门禁 + Git 收口
↓
T05 报告与证据汇总
↓
T06 用户收口验收
↓
MAP-A-CLOSE CLOSED
```

本轮只修订治理和计划文档。不得借计划修订或知识复盘：

- 修改产品代码、测试代码或 Vulkan 资产；
- 顺手修复 F1、重构 Renderer、迁移 Region UI 或启动 D3；
- 改变 Schema、数据格式、公共依赖或持久化合同；
- 把自动测试结果写成真机验收结果；
- 把未验证猜想写成长期 Knowledge；
- 自动将 `CONSTITUTION_CANDIDATE` 升格为宪法。

## 三、C1 · MAP-A 状态冻结

C1 必须以当前仓库事实为准，对账：

- `changelog.md`；
- `docs/milestones/current/MAP-A/R3-backlog.md`；
- `docs/milestones/current/MAP-A/R3-F1-closeout.md`；
- 已推送 Commit 与远端 tip；
- 自动门禁结果；
- 用户真机 IPO 逐项结果；
- 未验收项、失败项和明确阻塞项。

C1 的输出必须保留 `OPEN`、`READY FOR USER ACCEPTANCE`、`ACCEPTANCE FAILED` 等真实状态，不得以自动门禁通过替代用户验收。

## 四、C2 · MAP-A 架构收口文档

C2 只冻结已经有证据的归属和下一阶段边界，不实现下一阶段能力。至少记录：

- World、Shared Infrastructure、Workspace、Map Editor Tool 的职责边界；
- Region、Draft、Pointer、Picking、Render、Commit、History 链的事实所有权；
- 已验证的 Vector Overlay、Depth Policy、latest-state-wins、动态 Buffer 和 Metric 合同；
- 旧 Region Drawing UI、Legacy UI、Region Editor Migration 等未完成迁移；
- 当前不应推广的临时 workaround 和待审批的架构候选。

若证据不足，只记录为未决项或 `BACKLOG`，不制造确定性架构结论。

## 五、C3 · MAP-A 知识沉淀审计

### 5.1 审计输入

必须优先使用真实项目材料：MAP-A changelog、可核验 Commit、开发计划、验收记录、失败项、返工记录、架构决策、测试/门禁证据和最终保留实现。禁止凭 AI 记忆编写知识条目。

### 5.2 六类审计问题

| 类别 | 必须审计的内容 |
|---|---|
| Architecture Knowledge | World、Shared Infrastructure、Workspace、Map Editor Tool 的正确归属和越界证据 |
| Rendering Knowledge | Vector Overlay、Stroke/Marker/Fill、Ear Clipping、动态 Buffer、latest-state-wins、Depth Policy |
| Input / Picking | Pointer → Picking → MapPoint → Draft → Renderer → Commit → History 链路及拆分难点 |
| UI / Acceptance | 自动测试通过与真实 UI 可用之间的差异，Viewport/Input/视觉行为的真机要求 |
| Failed Approaches / Lessons | 失败方案、错误根因、连续返工、过大切片、假通过路径 |
| Technical Debt / Migration | Legacy UI、未验收项、Workspace 迁移、Region Editor Migration 和后续阻塞项 |

### 5.3 候选分类表

C3 必须先产出候选清单，再写入正式事实源。每行只能使用一种分类：

| ID | 候选 | 类型 | 证据 | 决定 |
|---|---|---|---|---|
| K01 | 待基于 MAP-A 证据填写 | `KNOWLEDGE` / 其他 | 待审计 | LOCKED |
| L01 | 待基于失败记录填写 | `LESSON` / 其他 | 待审计 | LOCKED |
| B01 | 待基于债务与路线填写 | `BACKLOG` / 其他 | 待审计 | LOCKED |
| X01 | 待基于证据不足项填写 | `REJECTED` / 其他 | 待审计 | LOCKED |

允许的类型只有：

```text
KNOWLEDGE
LESSON
CHANGELOG_ONLY
BACKLOG
REJECTED
CONSTITUTION_CANDIDATE
```

同一根因优先更新既有 `docs/knowledge/` 条目，不创建近义副本。`BACKLOG` 写入现有 MAP-A backlog 或正式债务事实源；不建立重复的 Backlog 目录。`CONSTITUTION_CANDIDATE` 单独报告，不自动修改宪法。

### 5.4 C3 最低输出

C3 PASS 必须产生：

- 候选知识清单和逐项分类结果；
- 正式 Knowledge 更新；
- 正式 Lessons / Incidents 更新；
- Backlog 更新；
- `REJECTED` 与 `CHANGELOG_ONLY` 判定；
- Constitution Candidate 单独报告（若存在）；
- 新增、更新、拒绝和候选数量统计。

若没有条目通过筛选，必须证明审计已执行，并说明每项被拒绝或仅保留历史的原因，不能只写 `N/A`。

## 六、C4 · 最终门禁与 Git 收口

所有 Knowledge、Lessons、Backlog 和收口文档修改完成后，才能 fresh 执行最终门禁：

```text
C1 → C2 → C3 → 最终 Diff
→ Solution Build 0W0E
→ Core.Tests / World.Tests / WarCore.Tests
→ 专项测试（按实际范围）
→ ARCH-A / 5+100 / 版本一致性 / diff-check
→ Commit → Push → Remote HEAD Verify
```

测试项目必须串行，解决方案只完整构建一次，测试使用 `--no-build --no-restore`。专项测试不得替代要求的全量测试；环境阻断必须按真实状态记录。

## 七、用户收口验收扩展

在既有 AC-U01～AC-U06 之后增加：

| 编号 | 验收内容 | 结果 |
|---|---|---|
| AC-U07 | MAP-A 知识沉淀审计已执行 | 待验收 |
| AC-U08 | Knowledge / Lessons / Backlog 分类合理 | 待验收 |
| AC-U09 | 未验证猜想未写成长期知识 | 待验收 |
| AC-U10 | Constitution Candidate 未被擅自升格 | 待验收 |

只有既有真机验收、AC-U07～AC-U10、C3、C4 和 Git 证据全部满足，才允许报告：

```text
MAP-A-CLOSE CLOSED
MAP-A CLOSED
```

随后才允许评估 `EDITOR-A-R1`，且仍需遵守下一阶段单独的目标冻结和验收门禁。
