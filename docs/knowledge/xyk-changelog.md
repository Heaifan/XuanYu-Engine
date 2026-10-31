# XYK Candidate Changelog

本文件只登记知识候选、审核轨迹和每轮 XYK 结果，不是正式 Knowledge / Lesson / Decision，也不是产品发布 Changelog。

## 规则

- 每轮 Codex 开发、修复、架构审计、工程事故及重要技术验证任务，结束前追加候选或 `XYK: NONE` 记录。
- 只追加，不覆盖、删除或改写既有候选。审核与正式入库的后续状态另追加事件。
- Candidate ID 与正式 XYK ID 分离；Related Knowledge 仅表示建议去重或强化的目标。
- 先检索既有知识再建议 CREATE。静态风险必须标注未验证；不得写成已发生故障。
- 正式知识文件只由 ChatGPT / XYK Coordinator 审核维护。Changelog 登记不代表正式入库。
- 重复执行同一任务时，先核对 Candidate ID，避免重复登记。

## 状态词汇

- `CANDIDATE LOGGED`：候选已追加登记。
- `REVIEWED`：ChatGPT 已完成去重和采纳裁定。
- `WORKTREE WRITTEN`：正式知识文件已写入工作树。
- `GIT COMMITTED`：正式知识改动已由 Git Commit 固化。
- `PUSHED`：对应 Commit 已推送并核对远端。
- `PENDING`：尚待审核或后续授权。
- `REJECTED`：审核后不采纳；保留原候选记录。

## 候选记录格式

```text
### <Candidate ID> — <short title>
- Task:
- Time: YYYY-MM-DD HH:mm:ss (UTC±HH:MM)
- Source Identity: HEAD=<hash>; Worktree=<clean/dirty>; Files=<path: blob or omitted>
- Category:
- Problem:
- Evidence:
- Confidence: VERIFIED FACT / STATIC RISK / UNVERIFIED HYPOTHESIS
- Proposed Knowledge:
- Suggested Action: CREATE / UPDATE / STRENGTHEN / SUPERSEDE / REJECT / NONE
- Related Knowledge:
- Review Status: PENDING
- Lifecycle: CANDIDATE LOGGED
```

## 任务记录格式

```text
### TASK <task id> — XYK NONE
- Time: YYYY-MM-DD HH:mm:ss (UTC±HH:MM)
- Source Identity: HEAD=<hash>; Worktree=<clean/dirty>
- XYK: NONE
- De-duplication / Rationale:
```

## 首批登记

### XYK-CAND-20261031-R1H-R2-001 — 目录容量审计必须按唯一归属复算
- Task: XYE-EDITOR-R1-H-R2
- Time: 2026-10-31 10:58:58 (UTC+08:00)
- Source Identity: HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty; Files=R1-H 审计与用户复核记录
- Category: Engineering / Architecture Governance
- Problem: R1-H 候选树将 77 个唯一文件映射 86 次，且多个目标末级目录超过 5 文件；只算理论容量会掩盖重复归属与实际超限。
- Evidence: R1-H 静态审计报告及用户复核；此项证据确认的是报告/目标树计数问题，不代表生产代码故障。
- Confidence: VERIFIED FACT（关于候选映射计数）；Proposed Knowledge 为治理规则，仍待审核。
- Proposed Knowledge: 5+100 目标树必须逐文件建立唯一最终路径，并按实际直接文件数、直接子目录数和真实深度逐层复算；目录总容量不能代替实际映射核算。
- Suggested Action: STRENGTHEN
- Related Knowledge: K-GOV-002
- Review Status: PENDING
- Lifecycle: CANDIDATE LOGGED

### XYK-CAND-20261031-R1H-R2-002 — 手工属性通知清单存在同步遗漏风险
- Task: XYE-EDITOR-R1-H-R2
- Time: 2026-10-31 10:58:58 (UTC+08:00)
- Source Identity: HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty; Files=XuanYu.Editor.UI/Vm/Map/UiVm.MapRender.cs; XuanYu.Editor.UI/Vm/Map/UiVm.MapDataset.Selection.cs
- Category: UI / Architecture
- Problem: AXAML 派生属性依赖多处手写 `OnPropertyChanged` 列表和显式刷新路径；新增依赖项若未同步更新通知列表，静态绑定存在显示过期风险。
- Evidence: R1-H 对 AXAML Binding、`OnPropertyChanged`、MapSession 和 Dataset 刷新路径的只读审计；未执行运行时验证，未确认存在实际漏通知故障。
- Confidence: STATIC RISK
- Proposed Knowledge: 对手工通知的派生 UI 属性，应能从 owner 变更追踪到绑定刷新责任；通知覆盖需由测试或可审计映射保护。
- Suggested Action: CREATE
- Related Knowledge: K-UI-001（仅供去重检索，不表示该条目内容已覆盖本候选）
- Review Status: PENDING
- Lifecycle: CANDIDATE LOGGED

### XYK-CAND-20261031-R1H-R2-003 — Manifest 双份可写状态的知识去重评估
- Task: XYE-EDITOR-R1-H-R2
- Time: 2026-10-31 10:58:58 (UTC+08:00)
- Source Identity: HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty; Files=XuanYu.Editor.UI/Vm/Map/UiVm.MapManifest.cs; XuanYu.Editor.UI/Vm/Map/UiVm.MapDataset.Selection.cs
- Category: Architecture / State Ownership
- Problem: Registry 与 ManifestOwner 当前都持有可写的 Manifest 值，存在同步/过期风险；该风险来自静态代码路径，尚无运行时故障证据。
- Evidence: R1-H 对 `MapDatasetRegistry.CurrentManifest`、`MapManifestOwner.CurrentManifest` 及 UiVm 显式同步路径的只读审计。
- Confidence: STATIC RISK
- Proposed Knowledge: NONE — 单一事实源原则已由开发宪法第五条规定；本候选尚无证据支持新增通用知识，先保留为本地架构裁定项。
- Suggested Action: NONE
- Related Knowledge: K-ARCH-001、K-ARCH-002（去重核对线索；不据此宣称已有条目覆盖该具体实现）
- Review Status: PENDING
- Lifecycle: CANDIDATE LOGGED

## 审核事件

### REVIEW XYK-CAND-20261031-R1H-R2-001
- Time: 2026-10-31 10:58:58 (UTC+08:00)
- Review Status: REVIEWED
- Decision: 保留为建议 STRENGTHEN K-GOV-002 的候选；不在本任务修改正式知识。K-GOV-002 已要求机器可判定治理边界建立自动门禁，本候选的增量点是迁移目标树必须验证唯一归属和真实目录容量。

### REVIEW XYK-CAND-20261031-R1H-R2-002
- Time: 2026-10-31 10:58:58 (UTC+08:00)
- Review Status: REVIEWED
- Decision: 保留为静态风险候选，等待可复现的属性通知回归证据后再决定是否 CREATE。K-UI-001 聚焦布局与命中区域，不覆盖 PropertyChanged 同步风险。

### REVIEW XYK-CAND-20261031-R1H-R2-003
- Time: 2026-10-31 10:58:58 (UTC+08:00)
- Review Status: REJECTED
- Decision: 不单独沉淀。其通用原则已由宪法第五条“同一事实不得存在两个可写权威所有者”覆盖；本次 Manifest 双写仅登记为 R1-H 局部静态风险，待 C 类审计处理。

## R1-I 审核与入库事件

### XYK-REVIEW-20261031-R1I-001 — STRENGTHEN K-GOV-002
- Time: 2026-10-31 11:05:14 (UTC+08:00)
- Candidate: XYK-CAND-20261031-R1H-R2-001
- Decision: STRENGTHEN；已对照 K-GOV-002，补入目标目录树必须以完整真实源文件清单建立唯一最终路径，并逐层核对真实深度、直接源码文件数和直接子目录数的规则；补入 R1-H 静态审计证据及验证边界。
- Knowledge Receipt: K-GOV-002; `docs/knowledge/engineering.md`; Git blob=1fa4359826ecb5c8fc4dda27976394a833fa751a; WORKTREE WRITTEN / GIT UNCOMMITTED。

### XYK-REVIEW-20261031-R1I-002 — 保留属性通知候选待证
- Time: 2026-10-31 11:05:14 (UTC+08:00)
- Candidate: XYK-CAND-20261031-R1H-R2-002
- Decision: 保持 STATIC RISK / PENDING。当前没有运行时或属性通知回归证据，不认定为已验证故障，不写入正式 Knowledge。

### XYK-REVIEW-20261031-R1I-003 — 确认拒绝 Manifest 通用知识新建
- Time: 2026-10-31 11:05:14 (UTC+08:00)
- Candidate: XYK-CAND-20261031-R1H-R2-003
- Decision: 维持 REJECT 新建；保留为 Editor C 类状态所有权整改事项，不重复沉淀通用知识。

### TASK XYE-EDITOR-R1-I — XYK: NONE
- Time: 2026-10-31 11:05:14 (UTC+08:00)
- Source Identity: HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty
- XYK: NONE（本轮没有超出既有 R1-H 候选的新长期规则；001 已作为既有候选强化 K-GOV-002，002 待证，003 维持拒绝。）
- Changelog: 以上是新追加审核事件；既有候选原文及先前审核事件未改写。

### TASK XYE-EDITOR-R1-J — XYK: NONE
- Time: 2026-10-31 11:31:32 (UTC+08:00)
- Source Identity: HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty; Protected Knowledge: engineering.md=1fa4359826ecb5c8fc4dda27976394a833fa751a; data.md=ac2e620362d4b90b2e30203cf43ff8803c39d728; lessons.md=4a459a3b7f6af2ff7960209085e9ca2a796b1caf; knowledge-index.md=451a321ca7c47a7481538f6aa1f28e697b8c21fc
- XYK: NONE
- De-duplication / Rationale: K-GOV-002 已覆盖目标树唯一映射、实际目录深度和容量核算；本轮根目录 29 项、509 文件及 project-file 路径约束是 Editor.UI 局部审计证据，没有形成需要重复沉淀的新通用规则。MapVectorOverlayBuilder partial 风险由现有 5+100/SRP 与 K-GOV-002 治理，不另建重复条目。既有 K-DATA-001、L-TEST-001、K-GOV-002 与候选待证/拒绝状态均保留。

### TASK XYE-EDITOR-R1-K — 2026-10-31
- XYK: NONE — 本轮为获批的纯路径迁移；目录容量、项目根计数和验证债务属于本轮执行状态，不新增正式知识候选。
- Editor 源码迁移 92 个，SHA-256 核对 258 个；映射清单：`docs/milestones/EDITOR-R1-K/editor-migration-map.csv`。
- 未改生产源码行为；本轮未执行 Restore、Build、Test、Clean 或 Rebuild。项目根直接项计数规则仍待裁定。

### TASK XYE-ARCH-DEPTH-R1 — XYK: NONE
- Time: 2026-10-31 12:28:30 (UTC+08:00)
- Source Identity: HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty (1954 paths at baseline); Changelog SHA256 before append=3FCC66D2390D5F3E2CBA4E78A8A55BCBB33E9D58252B54C1921D747759740025
- XYK: NONE
- De-duplication / Rationale: 本轮是当前工程物理目录、源码数量及文件行数的静态盘点；没有形成超出开发宪法 5+100 / SRP、知识索引及 K-GOV-002 治理原则的新长期规律。违规路径和计数是本轮审计证据，不是已验证故障；不新建重复候选。既有候选与审核历史均未改写。

### TASK XYE-EDITOR-R1-L — 2026-10-31
- XYK: NONE — 5+100 容量与 SRP 联合门禁已有宪法及 K-GOV-002 依据；状态权威边界属于已登记的 MapDocument C 类事项；partial 路径风险本轮通过有限纯移动改善，无新增通用知识。
- 修复 file-tree.md 中 24 条 K3 历史路径，并登记 Authoring 3 个纯路径重归档；源码 SHA-256 保持不变。
- R1-K 映射清单已更新到最终目标路径；未修改正式 XYK 知识。


### TASK XYE-EDITOR-R1-L — FINAL CLOSEOUT
- Candidate ID: NONE；`XYK: NONE`。
- 证据：258 个唯一映射及 SHA-256 均匹配；本轮实际纯路径移动合计 13 个；file-tree.md 的 24 条 K3 旧路径已修正，全部 258 个 Editor C# 路径与工作树一致。
- 去重与建议：目录容量/SRP 联合门禁由宪法 3.6 和 K-GOV-002 覆盖；MapDatasetRegistry/MapManifestOwner 的重复状态及保存事务风险已列入既有 C 类整改与 K-DATA-001/R1-E；不重复建知识。
- 正式 Knowledge：NONE（未修改正式知识文件）。报告：`docs/milestones/EDITOR-R1-K/editor-structure-audit.md`；映射：`docs/milestones/EDITOR-R1-K/editor-migration-map.csv`。
- Restore / Build / Test / Clean / Rebuild 均为 0；无 Stage / Commit / Push。

### XYK-CAND-20261031-R1FIX-001 — 全仓架构审计汇总需与逐工程明细闭合
- Task: XYE-ARCH-DEPTH-R1-FIX
- Time: 2026-10-31 13:09:38 (UTC+08:00)
- Source Identity: HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty (1955 paths); Audit Scope=952 directories / 2954 project source and metadata files; Scope SHA256=7A729EBB4931AF0C7032D74D117736AF0F058E9BC923A54AF3BBAF488D48B8F4
- Category: Engineering / Architecture Governance
- Problem: 全仓审计表的工程源码行合计与摘要不符，超深目录和直接源码目录汇总也未与逐工程明细闭合；嵌套工程、生成源码及构建目录的归属/排除口径不一致会改变统计结果。
- Evidence: 只读复核 21 个实际 csproj；唯一归属 C# 数为 2727；全路径去重超深目录为 304；按手写 C# 口径的直接超 5 目录为 52；超行文件为 5。旧报告摘要与其明细存在可复算差异。本候选只描述报告/统计错误，不表示生产代码故障。
- Confidence: VERIFIED FACT（关于旧报告和本轮复核的静态计数差异）
- Proposed Knowledge: 全仓架构盘点应使用唯一工程/文件清单和统一排除清单，分别核对项目行合计、全路径唯一问题数与摘要；生成目录及嵌套工程归属应作为明确分类，防止逐工程明细与总数发生漂移。
- Suggested Action: STRENGTHEN
- Related Knowledge: K-GOV-002
- Review Status: PENDING
- Lifecycle: CANDIDATE LOGGED
### TASK XYE-EDITOR-R1-M — XYK: NONE
- Time: 2026-10-31（Asia/Shanghai）
- Source Identity: HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty；本轮未暂存、未提交。
- XYK: NONE
- De-duplication / Rationale: Editor 六职责目录冻结、项目根计数口径及 BUILD-GATE 静态前置检查均为本轮工程状态与规范解释，不构成新的通用长期规则。目录唯一映射和逐层容量规则由宪法 3.6 / K-GOV-002 覆盖；Manifest 持久化事务由 K-DATA-001 覆盖；MapDocument 状态权威问题继续作为既有 C 类整改，不写成已实施事实。本轮未修改正式 Knowledge。
- Verification Boundary: 未执行 Restore、Build、Test、Clean 或 Rebuild；BUILD-GATE 仅静态检查，尚无工程验证结果。
### TASK XYE-EDITOR-R1-N — XYK: NONE
- Time: 2026-10-31（Asia/Shanghai）
- Source Identity: HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty；Changelog 在本轮前已含 R1-M 记录。
- XYK: NONE
- De-duplication / Rationale: 5+100 直接目录容量与唯一映射由宪法 3.6 / K-GOV-002 既有规则覆盖；覆盖型持久化事务由 K-DATA-001 覆盖。本轮提出的项目元数据计数口径和执行隔离设计仍是待批准的规范/门禁方案，未写成正式知识或已实施事实；不新建候选。
- Verification Boundary: 本轮仅静态检查规范、门禁脚本和相关测试路径；未执行 Restore、Build、Test、Clean 或 Rebuild；未修改生产源码、规范或门禁脚本。
### TASK XYE-EDITOR-UI-01 — 2026-10-31
- XYK: NONE
- De-duplication / Rationale: 本轮 17 个 Editor.UI 文件纯路径迁移、唯一目标映射与直接目录容量仍由宪法 3.6 / K-GOV-002 覆盖；测试路径和 file-tree 同步属于迁移实施记录，没有新增通用长期规则。UiVm/Map 状态权威问题沿用既有候选与 K-DATA-001，不登记为已修复事实。未修改正式 Knowledge。
- Migration Receipt: Batch 1 12 个指定文件；Batch 2 5 个低风险独立类型；逐文件 SHA-256 移动前后匹配。2 个测试文件只改 3 处源码路径常量，断言语义未变；file-tree 同步 17 条路径。
- Verification Boundary: 未执行 Restore、Build、Test、Clean 或 Rebuild。MapUI 子树静态容量通过；Editor.UI 全局根目录和既有直接源码超限仍未解决，STRUCTURE 为 PARTIAL。
### TASK XYE-EDITOR-UI-SRP-R1 — XYK: NONE
- Time: 2026-10-31 (Asia/Shanghai)
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Tree=23f983e12b71a4f31321df370f2c94eab6ce1dfc; Worktree=dirty.
- Audit Scope: XuanYu.Editor.UI physical C# / AXAML source snapshot; UiVm partials, typed bindings, related editor state owners and persistence/history paths reviewed read-only.
- XYK: NONE
- De-duplication / Rationale: Summary-to-detail count consistency is already explicitly covered by K-XYT-AUDIT-002; target-tree/direct-capacity accounting is covered by K-GOV-002 and its 5+100 section. This audit's UiVm ownership and lifecycle findings are local risks, not a new reusable rule. The pending XYK-CAND-20261031-R1FIX-001 should be reviewed against K-XYT-AUDIT-002 before any strengthening; no duplicate candidate proposed here.
- Formal Knowledge: No write. Per docs/governance/knowledge-audit-handoff.md, Codex may append candidate/NONE records but must not edit formal Knowledge or index; any STRENGTHEN decision is handed to ChatGPT / XYK Coordinator.
- Verification Boundary: Static syntax/source/XAML inspection only; no Restore, Build, Test, runtime validation, source/test/config/AXAML edit, staging or commit.
### TASK XYE-EDITOR-UI-SRP-R1B — XYK review recommendation + NONE
- Time: 2026-10-31 (Asia/Shanghai)
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Tree=23f983e12b71a4f31321df370f2c94eab6ce1dfc; Worktree dirty (1991 paths at audit start/end).
- Existing Candidate Review: XYK-CAND-20261031-R1FIX-001 is NOT fully duplicated by K-XYT-AUDIT-002. K-XYT-AUDIT-002 requires risk/action/P-Level bucket sums to equal unique IDs and blocks inconsistent batch writeback. R1FIX additionally proposes reconciling per-project totals, de-duplicating full-path issue counts across nested project scopes, and recording generated/nested project ownership/exclusions. These are material uncovered dimensions.
- Recommended Action: STRENGTHEN K-XYT-AUDIT-002 with project-scope/file-ownership/exclusion reconciliation and full-path unique issue counts. This is a reviewer recommendation only; formal status remains PENDING XYK Coordinator review. No rejection or formal Knowledge update claimed.
- New R1B Candidate: NONE. UiVm ownership and lifecycle observations are task-local static risks; no validated new general rule.
- Formal Knowledge: No write. Per docs/governance/knowledge-audit-handoff.md, formal Knowledge/index changes are reserved to ChatGPT / XYK Coordinator.
- Verification Boundary: Static review only; no Restore/Build/Test/runtime, no source/test/AXAML/project edits, no stage/commit/push.
### TASK XYE-EDITOR-UI-02 — 2026-10-31
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty; UI-02 exclusive writer session verified.
- XYK: NONE
- De-duplication / Rationale: 目录归属、唯一映射和 5+100 容量核算由宪法 3.6 / K-GOV-002 覆盖。本轮没有形成新的可推广工程知识；正式 Knowledge 未修改。
- Evidence: 509/509 文件一对一映射（443 C#、63 AXAML、3 项元数据）；目标树模拟与实际目录均为 C# 超限 0、直接子目录超限 0、超深 0。源代码根职责目录 20 → 5（Infrastructure、MapUI、Panels、Shell、ViewportUI）。本轮实际移动 493 个文件，16 项已处目标路径（含 csproj）；508 个非项目文件全部落位，映射记录的 SHA-256 后验核对 509/509。14 个 AXAML 资源 URI 文件记录前后哈希及精确路径差异；63 个 AXAML 中 46 个有 code-behind，46/46 同目录，另 17 个为无 code-behind 资源。UI-01 的 17 项成果保留并纳入完整映射，逐项 post-move SHA-256 17/17 一致。测试、守卫和资源生成脚本中的物理路径合同已更新；file-tree.md 同步 509 项。
- Verification Boundary: 静态容量、深度、文件哈希、AXAML 配对、avares URI 解析和测试路径字符串扫描完成。未运行 Restore、Build、Test、Clean 或 Rebuild；程序集、资源加载和运行时集成未验证。未改变 C# 业务行为、namespace、公共 API 或 AXAML 布局/Binding；未暂存、提交或推送。UiVm 类型级 SRP 仍属后续阶段。
### TASK XYE-BUILD-GATE — 2026-10-31
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty.
- XYK: NONE
- De-duplication / Rationale: 仓库内路径约束、Dirty 保全及验证边界由现有开发宪法与 K-GOV-002 / K-VAL-001 覆盖。本轮没有新增可推广规则；正式 Knowledge 未修改。
- Changes: Build 移除外部 NuGet fallback 并显式清空 RestoreFallbackFolders；Test 入口在 SDK Resolver 前验证项目/输出路径，设置仓库内 CLI、NuGet、TEMP/TMP 与测试结果目录环境，拒绝外部路径、输出重定向及日志文件输出，并恢复调用进程环境。
- Verification Boundary: PowerShell parser 与仓库内模拟自测通过（本地路径、外部路径拒绝、junction 拒绝、日志/输出覆盖拒绝、调用进程环境恢复）；不启动 SDK。未执行 Restore、Build、Test、Clean 或 Rebuild；未暂存、提交或推送。脚本不能限制测试代码显式向任意绝对路径写文件。

### TASK XYE-EDITOR-BUILD-01 — XYK NONE（补录 BLOCKED 回执）
- Time: 2026-10-31 15:32:52 (UTC+08:00；本轮补录时间)
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty (1635 paths，含既有未跟踪 XYK Changelog)
- XYK: NONE
- De-duplication / Rationale: 上轮按用户授权边界在真实写入隔离无法证明处停止，未执行 Restore/Build。该执行边界由开发宪法、K-GOV-002 与 K-VAL-001/002 等既有验证治理覆盖；仅为先前漏记补录，不提出新的长期知识候选。
- Verification Boundary: 构建未启动；仅报告既有隔离阻断事实。

### TASK XYE-EDITOR-BUILD-01-ISO — XYK NONE
- Time: 2026-10-31 15:32:52 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty (1635 paths，计数含既有未跟踪 XYK Changelog)
- XYK: NONE
- De-duplication / Rationale: 本轮只读环境能力结果是当前机器/权限状态，不构成可普遍化的工程规律；隔离与写入边界受现有开发宪法、K-GOV-002 及 K-VAL-001/002 覆盖。不重复创建候选，不修改正式 Knowledge。
- Verification Boundary: 未执行 Restore、Build、Test、Clean、Rebuild；未启用 Windows 功能、安装组件、创建 VM 或修改系统设置。

### XYK-CAND-20261031-EDITOR-BUILD-01-R2-001 — 项目元数据相对路径必须随资源迁移更新
- Task: XYE-EDITOR-BUILD-01-R2
- Time: 2026-10-31 15:45:01 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty (1635 paths); SourceFingerprint=80AC4E66A9FBBF87F18BE0DB30712C5690D2DBDD688565607D6AFCC9246CFAD2; BuildIdentity=D69C8211431F5FA5264B9B305248915229517A5ADBA09083C760CD353F1A6751
- Category: Engineering / Build Migration Governance
- Problem: UI-02 target map records `app.manifest` moved to `Infrastructure/Resources/app.manifest`, while `XuanYu.Editor.UI.csproj` retains `<ApplicationManifest>app.manifest</ApplicationManifest>`. The first Release compile fails with CS1926 because the old project-relative target does not exist.
- Evidence: Restore succeeded; Editor dependency produced `XuanYu.Editor.dll`; UI C# compilation reports CS1926 for the missing root-level manifest. The manifest exists at the mapped destination. No source fix was applied.
- Confidence: VERIFIED FACT (for the current path mismatch and compile failure).
- Proposed Knowledge: File relocation validation should inspect path-valued MSBuild properties such as `ApplicationManifest` in addition to source includes and XAML resource URIs, resolving each value against its owning project directory and confirming the target exists.
- Suggested Action: STRENGTHEN
- Related Knowledge: K-GOV-002 (general automated-gate principle; this specific project-property reference was not covered by the current move evidence).
- Review Status: PENDING
- Lifecycle: CANDIDATE LOGGED

### XYK-CAND-20261031-EDITOR-BUILD-01-R2-001 — 去重复核（BUILD-02）
- Task: XYE-EDITOR-BUILD-02
- Time: 2026-10-31 15:52:32 +08:00
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty (1636 paths); SourceFingerprint=B2493C062866D61976AA3EF507365C0A5A816D01DECA9BA68ADE18FA88B16765; BuildIdentity=9BECC8190E7FFC467DC896D9E47BD429EC06FBEDC63D2D788306FF03052B18B2
- Existing Knowledge Search: lessons.md 中 L-TEST-001 的 R1-G-R2 路径迁移小节涉及测试源码路径/文件枚举，但未覆盖 .csproj 的 ApplicationManifest 等路径属性；K-GOV-002 覆盖机器可判定治理成果应形成自动 Gate，但未规定该属性检查。
- Review Recommendation: STRENGTHEN lessons.md 中现有 R1-G-R2 路径迁移小节，将项目文件路径属性解析及真实 Build 证据纳入迁移验证规则；本轮没有发现需要另建重复条目的依据。
- Evidence: 更新 ApplicationManifest 为迁移后路径后，Editor.UI Release Build 及 Editor 项目引用均通过，0 warnings / 0 errors。Build 只证明编译期路径有效，不证明运行时资源加载。
- Formal XYK Status: PENDING。Codex 执行代理仅有 Changelog 候选/审核轨迹追加权限；未修改正式 lessons.md 或 ngineering.md。

### TASK XYE-EDITOR-BUILD-02 — XYK CANDIDATE
- Time: 2026-10-31 15:52:32 +08:00
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty (1636 paths); SourceFingerprint=B2493C062866D61976AA3EF507365C0A5A816D01DECA9BA68ADE18FA88B16765
- XYK: CANDIDATE
- Candidate IDs: XYK-CAND-20261031-EDITOR-BUILD-01-R2-001
- De-duplication / Rationale: 沿用已登记候选；其最接近的正式条目是 lessons.md 中 L-TEST-001 的 R1-G-R2 路径迁移小节，建议 STRENGTHEN。正式审核与写回待 XYK Coordinator。
- Verification Boundary: Restore 与 Release Build 通过；未执行 Test、Clean、Rebuild 或 Runtime 资源加载验证。

### CORRECTION — XYE-EDITOR-BUILD-02 XYK audit receipt
- Correction: The preceding review receipt contains a malformed shell-escape character in the final formal-file path. The intended path is `docs/knowledge/engineering.md`. The receipt's PENDING status is accurate; no formal knowledge file was modified.

### TASK XYE-EDITOR-CLOSE-R1 — Editor acceptance + Log Presenter first slice
- Time: 2026-10-31 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; BuildIdentity=32F6DA40A91C7D8B092B9381EA8F503A4E112180F99975663A0AA4AB24145DD2; SourceFingerprint=217E2EB066D943D102F47AE51BC90F74F2A23DBDE68E899A1498149B899E4253; SDK=10.0.400; configuration=Release.
- Acceptance: App Release Restore/Build succeeded 0 warnings / 0 errors (build identity 0CC5C672F1E115024FEE5DAD0FC3DA55ACAC7A22227FC864D3FB1607820E9320); real Editor window launched and rendered, status ready, log panel populated, no visible missing-resource/startup error. Runtime visual/product acceptance remains MANUAL. The 14 AXAML path migration is compile- and startup-smoke-covered; individual runtime access of every resource is not independently instrumented.
- Tests: Logging 31/31 PASS; UiTokenManifest 7/7 PASS. Initial targeted run exposed old physical paths in source-contract tests (18 logging cases and 4 manifest cases failed at file open); updated only those test input paths to current migrated locations, retained assertions, then rebuilt Release (0W0E) and reran both focused suites successfully.
- Changes: Fixed Editor.App ApplicationManifest to the moved UI manifest path. Began authorized UiVm SRP slice: added LogPresenter owning the single log buffer, filter state, projection caches, derived values and notifications; UiVm preserves binding names and command façade and forwards PropertyChanged. Added focused LogPresenter tests. No unrelated dirty paths edited; no tests weakened.
- External writes: This turn did not run a filesystem event monitor. The unified build gate redirects .NET/NuGet/TEMP outputs into repository `.xye-local`; Avalonia BuildServices Collector was observed as a separate child process in the prior monitored compile and is a residual attribution/write-path risk. No claim of zero external writes is made; no external user data was altered.
- XYK: NONE (no new reusable rule beyond the existing L-TEST-001 physical-path-migration entry). Existing candidate XYK-CAND-20261031-EDITOR-BUILD-01-R2-001 remains PENDING for XYK Coordinator; it is not promoted or duplicated here. Formal knowledge files unchanged.
- Verification Boundary: Release builds and targeted tests passed; real startup was observed before the LogPresenter slice. Post-slice startup and manual visual acceptance were not repeated. No test phase beyond the named focused suites; no Clean/Rebuild, commit or push.

### TASK XYE-EDITOR-UIVM-R2 — XYK NONE
- Time: 2026-10-31 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD=a88a06aa05608bfc279d9028650749c83bae62e7; Worktree=dirty (3025 paths at report time; full pre-existing dirty content preserved; cross-task path-count delta is not attributed).
- Changes: LogPresenter now owns the synchronous refresh reaction for the single EditorLogBuffer; removed 48 explicit UiVm refresh call sites and redundant LogSummary notifications. Added NotificationPresenter to own notification state, priority, merge count, sequence, lifetime evaluation, and events; UiVm keeps existing binding/API façade and forwards property changes and NotificationChanged.
- Evidence: Release build XuanYu.World.Tests.csproj succeeded, 0 warnings / 0 errors; BuildIdentity=89E5B84D4C4F6194D1B8C7DC31702F8BB9B87ACA5D5EE173DBF6321DB73EFA40. Presenter/notification contract tests 19/19; Logging + UiTokenManifest tests 39/39.
- Known Risk: Presenter notifications remain synchronous on the mutating caller thread. UI-thread scheduling safety was not established; callers are not marshalled through Dispatcher. No user visual acceptance or post-change startup smoke was run.
- Existing Handoff: XYK-CAND-20261031-EDITOR-BUILD-01-R2-001 remains PENDING; proposed STRENGTHEN target L-TEST-001 in docs/knowledge/lessons.md, with index path docs/knowledge/knowledge-index.md. Submitted to XYK Coordinator for formal entry and receipt; no formal knowledge files changed by this task.
- XYK: NONE (no additional reusable rule identified; existing pending handoff is tracked separately).
- Verification Boundary: Targeted release compile and tests only. A broader initial UiD5 filter selected 19 unrelated source-contract tests that still point at the removed XuanYu.Editor.UI/Win/UiWin.SceneCommands.cs path; exact responsibility filters passed. No full suite, startup smoke, staging, commit, or push.

### TASK XYE-EDITOR-UIVM-R2-DOCSTATUS — XYK NONE
- Time: 2026-10-31 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; Parent=6232519f0a1481259a48f6cba3564d0187f782d4; Root untracked file `5` remains preserved and excluded.
- Changes: Added DocumentStatusPresenter owning footer status, scene-busy state, transient status lifetime, and derived save/status projections. UiVm retains existing AXAML-facing properties and forwarding methods; footer storage and busy/token state now have a single presenter owner. Scene save transaction remains in UiVm.
- Evidence: Release build of XuanYu.World.Tests.csproj succeeded with 0 warnings / 0 errors. Focused DocumentStatusPresenter, SceneDocument, and UiMapDatasetRegionToolActivation tests passed 15/15. Presenter tests assert dirty/busy projection and notification forwarding.
- Known Risk: Presenter notifications remain synchronous on the mutating caller thread; UI-thread dispatch safety remains unproven. No startup smoke or visual acceptance was run.
- XYK: NONE (task-local ownership extraction; no new reusable rule identified). Existing L-TEST-001 STRENGTHEN handoff remains pending Coordinator receipt.
- Verification Boundary: One scoped Release build and named focused tests only; no full suite or post-change application launch.

### TASK XYE-EDITOR-UIVM-R4-PROJECTTREE — XYK NONE
- Time: 2026-10-31 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; Parent=a120c087df62ea4d673ab9f0b2b71a7babd1fb3a; pre-existing root untracked path `5` preserved.
- Changes: Added ProjectTreePresenter owning project tree node cache, current-scene node, collapsed keys and selected display-node reference. SceneStateOwner remains entity source; EditorStateOwner remains canonical selection owner. UiVm retains ProjectItems / SelectedProjectItem bindings and selection coordination. Hierarchy ownership and commands remain unchanged.
- Evidence: XuanYu.World.Tests Release build succeeded 0 warnings / 0 errors; tree guide, toggle, project selection, workspace and selection re-entry contracts passed 21/21. Added `test_ProjectTreePresenter` contracts; no XAML binding names changed.
- Known Risk: No Editor startup smoke in this slice; notification remains synchronous on caller thread as before. Visual behavior remains for final combined runtime pass.
- XYK: NONE (task-local ownership extraction; no reusable rule change). Existing L-TEST-001 STRENGTHEN receipt remains pending.
- Verification Boundary: Focused Release build/tests only; no full suite or visual acceptance.

### TASK XYE-EDITOR-UIVM-R4-LAYER — XYK NONE
- Time: 2026-10-31 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; Parent=ea1d407d; root untracked path `5` preserved.
- Changes: Added LayerPresenter owning the layer row projection, selected LayerId, active-row projection, and move/reorder availability calculations. MapEditSession remains the only layer data authority and is queried live for derived availability. Kept row command callbacks and existing UiVm binding/selection notifications. Moved EditorLayerProviderAdapter into the existing Layout directory to preserve the LayerRows direct-file capacity.
- Tests: Editor.UI dependency Release build succeeded 0 warnings / 0 errors. Layer/panel/MapLayerSession/visual contract focused suites passed 52/52. Updated stale AXAML source paths in two contract test files only; all test assertions remain unchanged.
- Known Risk: No Editor startup smoke in this slice; visual runtime acceptance is part of the combined final pass. Synchronous notifications remain on the original caller thread.
- XYK: NONE (task-local presenter ownership; no reusable rule change). Existing L-TEST-001 STRENGTHEN remains pending formal Coordinator receipt.
- Verification Boundary: Focused build/tests only; no full suite or product visual acceptance.

### TASK XYE-EDITOR-UIVM-R4-INSPECTOR — XYK NONE
- Time: 2026-10-31 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; Parent=fab5ae60; root untracked path `5` preserved.
- Changes: Replaced the UiVm Inspector parse partial with InspectorDraft, which owns entity/layer name draft values, stable entity-name edit target, cancel restoration, numeric parsing, transform candidate validation, and formatting. UiVm retains command/transaction/history orchestration and existing AXAML properties. Escape restores the initial entity-name draft and suppresses the ensuing focus-loss commit. SceneStateOwner and MapEditSession remain authoritative models.
- Evidence: World.Tests Release build succeeded 0 warnings / 0 errors. Inspector draft, stable target, commit, scene document, property mutability/target/color, and layer rename tests passed 41/41. Inspector AXAML/token/focus/layout contracts passed 28/28 after correcting legacy source paths only; assertions were unchanged.
- Known Risk: Editor startup smoke is still pending final combined pass. Transform controls remain OneWay numeric bindings with direct commit calls; no persistent per-axis text draft was introduced. UI-thread scheduling remains unverified.
- XYK: NONE (local extraction; no new reusable rule). L-TEST-001 STRENGTHEN formal receipt remains pending XYK Coordinator.
- Verification Boundary: Focused Release build and named test groups only; startup/visual acceptance not yet claimed.

### TASK XYE-EDITOR-UIVM-R4 — Final closeout / XYK NONE
- Time: 2026-10-31 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; HEAD before closeout=7b8ae28c05bfdab56181b414bafc82cab72e046c; root untracked path `5` preserved.
- Combined verification: Editor.App Release build gate reused matching Release identity 060681A0C0FFC9F95779EB83B409AAA132B2A8CB6F51CD6E645FCF669CA4472A (SDK 10.0.400); the prior per-batch Release builds succeeded 0 warnings / 0 errors. Editor launched from the final build; window rendered the unnamed scene and initialized Project, Layer, and Inspector panes. No startup/resource/binding error was visible in the smoke observation. Automated UI-log instrumentation was not run; product visual acceptance remains MANUAL.
- Focused test evidence across the three batches: ProjectTree 21/21; Layer 52/52; Inspector behavior/commit 41/41; Inspector AXAML/token/focus/layout 28/28. Suites overlap; counts are reported separately and not summed.
- Ownership result: ProjectTreePresenter owns display-node projection/cache and tree-only display state; SceneStateOwner remains entity source and EditorStateOwner canonical selection. LayerPresenter owns list projection and derived selection/availability; MapEditSession remains layer authority. InspectorDraft owns input draft, parsing/validation, and cancel state; SceneStateOwner/MapEditSession remain models while UiVm retains commit/history orchestration.
- Residual: caller-thread synchronous notification contract is preserved; UI-thread dispatch safety is unverified. Transform fields remain direct numeric commit controls rather than persistent per-axis drafts. Visual acceptance remains MANUAL.
- XYK: NONE for this extraction. Existing L-TEST-001 STRENGTHEN remains pending formal XYK Coordinator receipt; no formal knowledge entry was changed.
- Verification boundary: Named focused suites and Release build only; no full test suite. Root untracked path `5` remains untouched and excluded from task commits.

### TASK XYE-EDITOR-UIVM-R5-A — Selection coordination / PARTIAL
- Time: 2026-10-31 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; Parent=5a6ebd5445d4756c9cb35936ca330ff783e3ef2b; root untracked path `5` preserved.
- Changes: Added `SelectionCoordinator` to own canonical Editor selection commits/clears and synchronization of selected entity identity into `SceneStateOwner`. The canonical selection remains `EditorStateOwner`; `MapEditSession` map selection remains independent; `ProjectTreePresenter` remains tree display owner. UiVm retains dual-tree projection, interaction cancellation, Inspector/render notification cascade, and AXAML binding façade.
- Evidence: `XuanYu.Editor.UI` Release build succeeded 0 warnings / 0 errors (identity=909FCAE91949F29DE91F9D694B3441D937BA0EC9963BA0EFD3C1E8E95066FDCE). `XuanYu.World.Tests` Release build succeeded 0 warnings / 0 errors (identity=40C7EBC152D93543395D34CCF811CFE3FFA65B4B9F1D8FF074EF40570D5B33DA). Focused Selection/Scene reentry/Selection tool state/Inspector selection contracts passed 17/17 using Release test output.
- Boundary: The extracted owner has actual state-transition behavior, but Selection's view/lifecycle coordination remains in UiVm; therefore Selection is PARTIAL, not CLOSED. No map selection authority or third entity-selection source was added.
- XYK: NONE (task-local extraction). Existing L-TEST-001 STRENGTHEN formal receipt remains pending XYK Coordinator.

### TASK XYE-EDITOR-UIVM-R5-B — Camera controller extraction / PARTIAL
- Time: 2026-10-31 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; Parent=c1e291198de0ebff09e5e6e7a844d88b0d6646d0; root untracked path `5` preserved.
- Ownership analysis: Camera state and session references span 114 use sites across navigation, camera snapshot/render publication, Dolly/terrain clearance, scene/map/terrain framing, terrain Inspector policy, gizmo sizing, diagnostics and replay. Mutations are shared across those paths; a state-holder façade would not transfer behavior ownership, while moving the navigation core alone would leave competing state writes and lifecycle ownership in UiVm.
- Changes: No camera behavior/state code moved. Frozen camera/Dolly/terrain behavior remains unchanged.
- Evidence: The successful Editor.UI Release build for the current source identity succeeded 0 warnings / 0 errors. Focused Release camera/orbit/Dolly/terrain far-plane tests passed 124/124.
- Boundary: Camera extraction is PARTIAL because equivalent behavior ownership cannot be proved without coordinated changes across the listed paths. No changes to ObservationCenter Dolly, 0.25m floor, terrain constraint/recovery, resize/DPI, or navigation cancellation semantics.
- XYK: NONE. Existing L-TEST-001 STRENGTHEN formal receipt remains pending XYK Coordinator.

### TASK XYE-EDITOR-UIVM-R5-C — Transform/Gizmo interaction owner / CLOSED
- Time: 2026-10-31 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; Parent=0b0685a20c6837d8e92c061a3ebe40565bda023d; root untracked path `5` preserved.
- Changes: Added `TransformInteractionController` owning the existing `TransformSession`, Move constraint, Rotate drag, Scale drag, and their Preview calculations. UiVm retains pointer routing, capture termination, business transaction application, history recording, notifications and snapshot publication. Preview remains non-authoritative; Cancel clears session/drag state without history; Commit still creates/applies one existing TransformCommand through the prior history path.
- Evidence: Editor.UI Release build succeeded 0 warnings / 0 errors (identity=9E55F98F4CE5C8AC47CDD9C552B4CE07DBB8CF2BFD4C1DDA121EF9715E4E8060). World.Tests Release build succeeded 0 warnings / 0 errors (identity=DD7AB6F1C202BDA7C571B6B6009385597E612616F78DAA397CE35DB9635C9374). Focused Move/Rotate/Scale/TransformSession/boundary/projection suite passed 70/70. Updated an existing reflection-based drag cleanup test to inspect the relocated owner; its null-after-commit/cancel assertions are unchanged.
- Known risk: Pointer lifecycle remains coordinated by UiVm/ViewportInput; full native capture-loss behavior remains covered only by the named automated contracts, not separate manual pointer instrumentation.
- XYK: NONE (task-local ownership extraction). Existing L-TEST-001 STRENGTHEN formal receipt remains pending XYK Coordinator.

### TASK XYE-EDITOR-UIVM-R5 — Final closeout / Selection PARTIAL, Camera PARTIAL, Transform CLOSED
- Time: 2026-10-31 (UTC+08:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; implementation HEAD=aba83472cd04f20b3df6385b391a5aed87640c30; root untracked path `5` preserved.
- Final build: Release `XuanYu.World.Tests` dependency build succeeded 0 warnings / 0 errors, compiling `XuanYu.Editor.UI` and `XuanYu.Editor.App` (identity=3ACAE18AFBBB815302689DB576E1DEA91F7AE63B0FDF1D987647FF8598FDF618). Final union of Selection, Camera/Orbit/Dolly/TerrainFarPlane, Move/Rotate/Scale/TransformSession and projection suites passed 209/209 without duplicate counting.
- Runtime: Editor App launched from final Release outputs with implementation HEAD in title; unnamed scene, Project pane, Inspector empty state, navigation Gizmo and ready status rendered; visible error count 0. Product visual acceptance remains MANUAL. Camera test suites passed, but manual Pan drag did not produce an unambiguous observed view change and is not claimed PASS.
- Owners: `SelectionCoordinator` owns canonical selection transition and active entity synchronization, while `EditorStateOwner` remains canonical entity selection and `SceneStateOwner` active entity authority. `TransformInteractionController` owns TransformSession plus Move/Rotate/Scale drag states and Preview calculations; UiVm retains input routing, commit/history, cancellation event coordination and publication. Camera state remains in UiVm because its 114 references span render publication, terrain constraints, framing, gizmo sizing, diagnostics and replay; camera extraction remains PARTIAL to protect existing contracts.
- Counts: UiVm partial 154→154; fields 152→150; methods 518→518; properties 413→413. Independent Owner count increases from R4's 6 to 8 (SelectionCoordinator and TransformInteractionController); Camera adds none.
- 5+100 local scope: Input and Interaction each have 5 direct subdirectories; SelectionCoordination has 3 direct files; TransformTools has 5 direct files. New coordinator/controller sources are 27/83 lines. Updated `file-tree.md` for the related path moves/new files.
- Risks: Existing synchronous notification/caller-thread behavior is preserved; UI Dispatcher safety remains unverified. Pointer lifecycle terminal routing remains in UiVm/ViewportInput. No full test suite or manual visual acceptance claimed.
- XYK: NONE. Existing L-TEST-001 STRENGTHEN formal receipt remains pending XYK Coordinator; no formal knowledge entry changed.

### TASK XYE-EDITOR-UIVM-R6 — Selection CLOSED / Camera PARTIAL
- Time: 2026-10-31 11:26:38 (UTC+00:00)
- Source Identity: Branch=codex/world-r1-migration-20261028; implementation HEAD=9740c46c8062d45a59314851d4a66e6caaec4d87; root untracked path `5` preserved.
- Changes: SelectionCoordinator now owns selection transition depth, tree projection synchronization, ordered selection/Inspector/command/render notifications and viewport hit/miss resolution; canonical entity, scene-active entity, map selection and project-tree display remain owned by EditorStateOwner, SceneStateOwner, MapEditSession and ProjectTreePresenter. CameraController is sole mutable camera/navigation state owner, with controlled Pan/Orbit/Dolly/frame APIs; CameraTerrainConstraint owns DEM clearance calculation. UiVm direct camera mutation sites reduced from 17 to 0; framing math and render snapshot publication remain in their existing owners/adapters.
- Evidence: Editor.UI and Editor.App Release builds each succeeded with 0 warnings / 0 errors (identities recorded in task evidence); the de-duplicated Selection/Camera union passed 220/220 before the test file's path-only move. After that move, World.Tests Release dependency build succeeded 0 warnings / 0 errors (identity=7EBAB5690D2BBD898E60990173E64AAF809C740444FFA78EFA2DB0EAC0FA1760). Editor startup smoke showed Project, Inspector, viewport, Navigation Gizmo and ready status with visible error count 0. Camera Pan state contract passed via the public VM route; native visual Pan could not be reliably driven/observed and remains PARTIAL.
- Counts: UiVm partial 154→154; fields 150→138; methods 518→520; properties 413→420. Selection CLOSED. Camera actual state ownership and controlled mutation path transferred; integrated visual Pan acceptance remains PARTIAL.
- Residual: synchronous caller-thread notification behavior is preserved; UI Dispatcher safety remains unproven. Native pointer Pan visual result remains unverified. No repeated 220-test run was performed after the test source path-only move.
- XYK: NONE (task-local ownership extraction; no new durable reusable rule established). Existing L-TEST-001 STRENGTHEN formal receipt remains pending XYK Coordinator; no formal knowledge entry changed. Coordinator follow-up remains required for the formal receipt.
