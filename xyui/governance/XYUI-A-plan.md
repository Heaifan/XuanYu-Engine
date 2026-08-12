# XYUI-A · XYUI 独立 UI 体系旁路线正式工程计划

> **落库信息**
> - 来源：用户 2026-08-12 两轮裁定（XYUI-A0 治理原则 + 阶段规划收缩为 XYUI0 单块样板）。
> - 状态：`正式计划`。A0 治理条款于本文档冻结；A1~A5 锁定待解锁；XYUI1/2、Components、Gallery、XuanYu Integration `⛔ NOT AUTHORIZED`。
> - 落库前核查：主仓库无任何 XYUI-A0 落库物；`docs/milestones/current/EDITOR-A/XYUI-backlog.md` 仅登记 XYUI-001（RegionPanel Binding 文本显示异常，NON-BLOCKING BACKLOG）。**本文档是 XYUI 旁路线的第一个仓库事实源**（位于独立 XYUI worktree）。
> - 关联：`docs/milestones/current/EDITOR-A/XYUI-backlog.md`（XYUI 债务登记，玄域主仓库）；`docs/ui/玄域引擎_UI规范_1.0.md`（现有 Editor UI 规范，与 XYUI 旁路线关系见 §10）。

## 0. 定位与五角色分工

XYUI 是与玄域主开发完全隔离的独立 UI 体系旁路线，直到用户批准 `START XYUI-B` 才允许接入玄域。五角色互不替代：

| 角色 | 负责 |
|---|---|
| XMind / 聊天 | 人类总览、设计审查、理解体系（设计输入，**不是事实源**） |
| JSON Spec（`xyui/registry/` + `xyui/specs/`） | 机器读取、版本、验证（**唯一 Source of Truth**） |
| AXAML / C# | 真实实现（本阶段禁止） |
| Gallery | 人类真机验收（本阶段禁止） |
| Guard | 自动治理 |

Agent 不得靠搜索聊天记录重新猜测规范。

## 1. 阶段总览

```text
XYUI-A0  Governance & Workspace Isolation   治理 / 隔离 / 权限 / Gate
XYUI-A1  XYUI0 Foundation Intake            XMind → Source Audit + Decision Matrix
XYUI-A2  XYUI0 Foundation Registry          manifest / decisions / unresolved
XYUI-A3  XYUI0 Token Draft                  Primitive + Semantic Token 草案
XYUI-A4  XYUI0 Validation                   自我检查 + 门禁
XYUI-A5  XYUI0 Foundation CLOSED            XYUI0 Foundation Package 收口
------------------------------------------
🔒 XYUI1 / XYUI2 / Components / Gallery / XuanYu Integration（NOT AUTHORIZED）
```

推进原则：**XYUI0 是第一块真实样板**，先把「Intake → 分类 → 审计 → Registry → Token → 验证」流程跑通；后续 XYUI1/2 复制同一流程。当前阶段禁止实现正式控件。

## 2. XYUI-A0 · 治理条款（已冻结）

### 2.1 Workspace Isolation（最高优先级，A0 硬门禁）

```text
XuanYuEngine/        → 开发 Agent（MAP-DATA-A / F3-A / Spatial Index）
XuanYuEngine-XYUI/   → XYUI Agent（feat/XYUI-A）
```

- XYUI Agent **不得进入开发 Agent 正在工作的物理目录**。
- 开发端未提交改动（Spatial Index、MapEditSession、MAP-DATA 等）对 XYUI Agent 禁止一切操作：修改 / stage / commit / stash / restore / reset / checkout / 删除。
- XYUI Workspace 若出现其他 Agent 未提交业务改动 → **立即 STOP 并报告 Workspace Isolation FAIL**。

### 2.2 A0 硬验收标准

```text
开发 Workspace ≠ XYUI Workspace（物理路径不同）
XYUI Workspace git status 不得出现：
  SpatialIndex / MapEditSession / MAP-DATA-A / F3-A
```

出现即 STOP，不得继续。

### 2.3 禁止脑补规范（最高优先级）

已裁决内容只覆盖已知字段；未知字段不得按「行业一般做法」补值。必须显式登记：

```text
RESOLVED         已裁决
UNRESOLVED       未裁决（Agent 不得自行填值）
NOT_APPLICABLE   不适用
```

示例：已定「Button 高度 32、小圆角、方案 2」但未定 Focus Ring / Disabled opacity 时，Agent 不得写 `"disabledOpacity": 0.5`，必须写 `{ "disabled_opacity": { "status": "UNRESOLVED" } }`。

> **不知道，就是不知道。** 所有未知内容显式 `UNRESOLVED`；Agent 自行推测数必须为 0。禁止根据行业惯例 / Material Design / Bootstrap / Avalonia 默认值 / 相邻组件 / Agent 审美补全。

### 2.4 禁止把现有数字「标准化」

XMind 写 `Padding = 6`，Agent 不得因「设计系统一般按 4/8 Grid」改成 8。**XYUI Agent 的职责是忠实结构化，不是优化设计。**

### 2.5 Conflict Zero Rule（提交前强制检查）

本轮 `touched paths` 只能属于 `xyui/**`；出现任何其他路径（含 `src/XuanYu.*`、`docs/milestones/current/MAP-*`、玄域 changelog/版本文件）→ 任务立即 **FAIL**（不是 Warning）。

### 2.6 路径预算

```text
Allowed root: xyui/**
```

任务确实需要修改其他路径时，必须在计划中提前登记 `EXCEPTION PATH`，否则禁止修改。

### 2.7 技术栈冻结

禁止 Agent 自行升级/新增依赖：升级 Avalonia、CommunityToolkit、ReactiveUI、Icon Library、Storybook 类依赖、Tailwind、npm 工具等一律禁止（除非后续专门批准）。优先**尽量靠现有技术栈实现**。禁止修改主 Solution、禁止新增第三方依赖。

### 2.8 门禁分级（纯文档/JSON 轮不抢全量 Build）

若 XYUI 本轮仅修改 `.md` / `.json` / `.svg` / `yaml` / 纯 `xyui/**` 非项目文件，且满足：未修改 `.sln`/`.slnx`、未修改 `.csproj`、未进入 `src/`、未建立主工程 ProjectReference、未修改玄域代码——则：

- **不要求 XuanYu.Engine 全量 Build**，不运行 Core / World / WarCore / MAP-DATA 测试（避免与开发 Agent 抢占 MSBuild、内存与磁盘资源；「不影响开发线原则」优先于「形式完整的全量门禁」）。
- 只需：文件格式验证、Schema 验证（存在时）、链接/引用验证、changed-path gate、`git diff --check`、Git remote consistency。

未来 XYUI 开始有真正 `.cs`/`.axaml` 独立代码时，再构建 **XYUI 自己的项目**；只有到 `XYUI-B` 正式接入玄域后，才重新要求玄域 Solution 完整 0W0E。

### 2.9 Decision Packet 同步机制

聊天里每定稿一项，形成最小 Decision Packet（Decision ID / 模块 / 方案 / 状态 / 主要参数 / 交互 / 特殊说明），由 UI Agent 正式转入 Registry。流程：

```text
讨论 → 用户选择方案 → Decision = APPROVED → UI Agent Consume
→ Registry → Spec → Contract → 以后实现
```

### 2.9 开发端代码统一规则

```text
看到 → 不碰 → 不处理 → 不提交 → 不 stash → 报告
```

XYUI Agent 处于独立 Workspace，理论上不应看到开发端未提交修改；若看到 → Workspace Isolation FAIL，直接停。

## 3. XYUI-A0-R1 · Workspace Isolation（当前执行轮）

完整 TODO：

```text
A0-R1-01  只读检查当前仓库状态
A0-R1-02  确认开发 Agent 当前 Workspace
A0-R1-03  不得处理 Spatial Index / MapEditSession 外部改动
A0-R1-04  建立独立 XYUI Workspace / Worktree
A0-R1-05  建立或切换 feat/XYUI-A
A0-R1-06  确认 XYUI Workspace 没有继承开发端未提交改动
A0-R1-07  确认两个 Workspace 物理路径不同
A0-R1-08  确认 Dev Agent 当前工作区保持原样
A0-R1-09  输出隔离证据
A0-R1-10  停止，不进入 XYUI0 整理，等待门禁满足
```

## 4. XYUI-A1 · XYUI0 Foundation Intake

> 状态：`XYUI-A1-R1-F1 · READY FOR USER ACCEPTANCE`（A1-R1 已完成 Source Intake + Evidence Mapping；F1 按人工复核修正分类：0.13/0.3-A/0.24 = SOURCE_FORMATTING_DEFECT ×3，0.29 恢复 CLEAR，0.2-A↔0.2-C 改 TOKEN_LAYER_OVERLAP ×2；Source SHA 未变。R2 待批准后解锁。）

**本轮输入**：只允许 XYUI0 XMind / XYUI0 大纲（XMind = 人类原始证据；Outline = 辅助机器解析）。**禁止读取 XYUI1/2 反推、补全 XYUI0；禁止查玄域现有 UI、行业规范、Material Design、Bootstrap、Avalonia 默认视觉、其他设计系统、Agent 自身设计经验补答案。**

### A1-R1 · Source Intake + Evidence Mapping（当前轮）

**不抢跑正式裁决。** 唯一目标：准确、可追溯地读取 XYUI-0.md，证明正确理解了设计资料。

1. **冻结 Source**：`XYUI-0.md` 原样保存为不可变 Evidence Source（`xyui/source/XYUI0/XYUI-0.md`），禁止改写/格式化/合并/删减/纠错/统一术语；记录 `source_id` / 原始文件名 / SHA-256 / 导入时间 / source_type。
2. **重建真实目录树**：识别全部结构项（0.1 / 0.2 / … / 0.N，含 0.2-A~I 等子节）。每项提取：ID、名称、类型、用途、候选方案、最终选择语句、后续修订、UI 参数、UI 代码、响应式要求、交互要求、例外规则、备注；原文没有的字段写 `NOT_PRESENT`，不得补全。
3. **Evidence Ledger（Decision Chain）**：不只看首次方案选择，必须记录完整链：初始选择 → 修订 → 组合 → 覆盖 → 确认。Evidence 类型仅允许：

```text
SELECT   选择方案
MODIFY   修订参数
COMBINE  组合方案
OVERRIDE 覆盖先前选择
CONFIRM  确认 / 进入下一项
REJECT   否决
COMMENT  补充说明
```

4. **保留 Source Location**（硬要求）：每条 Evidence 记录 `source_file` / `section` / `heading` / `line-range`，可反查原始证据。
5. **本轮判断只允许三态**（≠ 正式五态）：

```text
CLEAR      证据链完整清晰
AMBIGUOUS  有选择但后续表述模糊
MISSING    未找到最终选择
```

`CLEAR ≠ CONFIRMED_APPROVED`；正式五态分类（CONFIRMED_APPROVED / PROBABLE_APPROVED / UNRESOLVED / CONFLICT / HISTORICAL_ONLY）留到 A1-R2。
6. **重复项 / 矛盾项不处理**：同一主题出现多处 → 分别记录 + `possible_relation`，等待 R2 判断覆盖；出现矛盾数值（如 Radius 4 vs 3）→ 两条 Evidence 都记 + `potential_conflict = true`，R2 再判定 OVERRIDE 或 CONFLICT。禁止擅自合并、禁止修改。
7. **输出仅三个核心文件**（+ 治理计划必要小幅更新）：

```text
xyui/source/XYUI0/XYUI-0.md       不可变证据源
xyui/audit/XYUI0/source-audit.md  给人审（逐项 Evidence + 判断 + 统计）
xyui/audit/XYUI0/evidence-index.json  给机器用
```

禁止创建 `registry/` `tokens/` `components/` `gallery/`。

### A1-R1 门禁（G1~G10）

```text
G1  Source SHA 可重复            G2  Source 原文未修改
G3  所有一级/二级项目都有记录     G4  Evidence 都能反查 Source
G5  不存在正式 APPROVED 输出      G6  不存在 Token 输出
G7  不存在 XYUI1/2 来源           G8  changed paths ⊆ xyui/**
G9  无 XuanYu 业务代码变化        G10 Git remote 一致（Ahead/Behind = 0/0）
```

资源隔离：本轮仅 Markdown + JSON，**不运行玄域全量 Build / Core / World / WarCore / MAP-DATA 测试**；仅轻量检查（JSON parse、Source hash、source-reference integrity、duplicate ID、changed-path check、git diff --check、git status）。

完成状态：`XYUI-A1-R1 · READY FOR USER AUDIT`，**必须 STOP，不得自动进入 A1-R2**。

### A1-R2 · 分类五种状态

```text
CONFIRMED_APPROVED  XMind 明确存在最终方案、明确选择、后续无推翻
PROBABLE_APPROVED   看起来已进入下一项但文档结构不足以百分百确认
                    （如「3，下一项」）；不得擅自升级
UNRESOLVED          没有具体数值 / 没有最终选择 / 某状态没有定义
CONFLICT            前后出现矛盾方案但无法判断哪个最终
HISTORICAL_ONLY     明确被否决的候选方案
```

只有能由最终裁定链明确证明的项目才可标记 CONFIRMED_APPROVED。

### A1-R3 · 生成 XYUI0 审计表（第一份交付物）

| ID | Foundation 项 | 最终方案 | 状态 | 缺失 | 冲突 |
|---|---|---|---|---|---|
| 0.1 | xxx | 方案4 | Confirmed | 无 | 无 |
| ... | ... | ... | ... | ... | ... |

同时统计：XYUI0 总项数 / Confirmed / Probable / Unresolved / Conflict / Historical。

**此步结束：STOP。不写 Token。** 第一次 Agent 解析 XMind，须人工审核确认 Approved Matrix 后（Agent 整理 → 用户审核 → 确认）才进入 Registry。

## 5. XYUI-A2 · Foundation Registry

> 状态：`XYUI-A2 · CLOSED`（用户验收，44/44 VALIDATED）；`XYUI-A3-R1 · READY FOR USER ACCEPTANCE`（Token Namespace Audit：426 occurrences / 418 distinct / NTC 5 + IDENTICAL 3 + VALUE 0 + ALIAS 35 候选；A3-R2 Canonical Token Architecture 待批准后解锁）。

只有经过 A1 人工批准的项目进入这里。目录与文件（**禁止一项规范五六个 JSON**）：

```text
xyui/
└─ registry/
   └─ foundation/
      ├─ manifest.json    有哪些 Foundation、状态、当前版本、Decision ID
      ├─ decisions.json   最终裁定：最终方案、最终修订、语义、UI 参数、交互、例外规则
      └─ unresolved.json  未定项：不猜、不填默认值、不查 Material Design、不抄 Bootstrap、不参考 Avalonia 默认值
```

示例：

```json
{ "id": "XYUI-0.17", "status": "APPROVED", "decision": "XYUI-D-0.17" }
```

```json
{
  "id": "XYUI-U-004",
  "source": "XYUI-0.xx",
  "field": "focus-ring-width",
  "reason": "No explicit approved value in XYUI0"
}
```

## 6. XYUI-A3 · Token Draft

只有 Registry 完成后才开始 Token，且第一轮只是 **Token Draft**（JSON Token Source），不是正式实现：

- **禁止生成并接入 XuanYu.Editor / 改 App.axaml / 改现有 Theme**；standalone AXAML export 也可以稍后。当前重点：Token 模型对不对。
- 分两层：

```text
Primitive   color.xxx / size.xxx / spacing.xxx / radius.xxx
Semantic    surface.app / surface.panel / text.primary / text.secondary
            border.default / accent.primary / interaction.hover
```

后续组件只能消费 Semantic Token。

## 7. XYUI-A4 · Validation

把 XYUI0 自己检查一遍：

```text
A4-01  所有 Approved Foundation 有 Registry
A4-02  所有 Registry 有来源
A4-03  所有 Token 有 Decision 来源
A4-04  没有 Agent-generated design
A4-05  所有未知值进入 Unresolved
A4-06  没有引用 XYUI1/2
A4-07  没有玄域业务代码修改
A4-08  没有 Spatial Index / MapEditSession 修改
A4-09  没有 Solution 接入
A4-10  没有第三方依赖变化
```

## 8. XYUI-A5 · XYUI0 Foundation CLOSED

形成第一份 `XYUI0 Foundation Package`：

```text
xyui/
├─ governance/        治理与计划文档（本计划所在）
├─ source/XYUI0       人类原始证据（XMind 导出 / Outline）
├─ registry/foundation/
├─ specs/foundation/
├─ tokens/draft/
└─ reports/XYUI0
```

状态：`SOURCE AUDITED · REGISTRY COMPLETE · TOKEN DRAFT COMPLETE · VALIDATED · READY FOR USER ACCEPTANCE`。**仍然不是 XYUI COMPONENT IMPLEMENTED。**

## 9. 每轮执行规则

### 9.1 统一报告格式（UI Agent 强制模板）

```text
当前阶段：XYUI-A?-R?
完整 TODO：✅ ▶ ⬜
已完成 / 进行中 / 下一步 / 剩余轮次
阻塞项
允许修改路径：xyui/**
实际修改路径：...
禁止路径修改数量：0
XYUI0 解析进度
Confirmed 数 / Probable 数 / Unresolved 数 / Conflict 数 / Historical 数
测试 / 自动验证
人工验收：READY / NOT READY
Git：Branch / HEAD / Origin HEAD / Ahead / Behind / Working Tree
总体进度：XX%
```

### 9.2 提交策略

不要整理一半一直 commit。每个 CLOSED Round：实现 → Validate → diff check → 确认 touched paths（`changed paths ⊆ xyui/**`，否则 FAIL）→ Commit → Push → 核对 origin。必须 `Ahead = 0 / Behind = 0`。

### 9.3 本阶段严格禁止

```text
❌ 整理 XYUI1 / XYUI2                      ❌ 根据 XYUI1 补 XYUI0
❌ 开发 Button / Slider / Tree             ❌ 开发 Gallery
❌ 修改 XuanYu.Editor.UI / App.axaml / 当前主题 / 全局替换现有颜色
❌ 修改 MAP-DATA / Spatial Index / MapEditSession / GlobalWorld
❌ 新增依赖 / 修改主 Solution
❌ 自行设计未明确参数 / 主动接入玄域
```

## 10. 与现有 UI 体系的关系

- 现有 `docs/ui/玄域引擎_UI规范_1.0.md` + `XuanYu.Editor.UI/Design/UiTokenManifest.json`（112 Token）属于玄域 Editor 现状基线，**不在本旁路线范围内**；XYUI Token 体系在 `xyui/**` 独立建设。
- XYUI Token 与 Editor 现有 Token 的映射/迁移关系属 **XYUI-B** 裁决内容，本阶段不讨论。
- `EDITOR-A/XYUI-backlog.md` 的 XYUI-001 等 backlog 项在 A1 登记时纳入。

## 11. 本计划的 UNRESOLVED 登记（禁止脑补的示范）

```text
XYUI-PLAN-U001  XYUI0 大纲的 XMind 原始文件位置：本机未确认（A1 输入以用户提供为准）
XYUI-PLAN-U002  A0 在聊天中的更早裁定细节：本机仓库不可见，以本文档 §2 为唯一事实源
XYUI-PLAN-U003  xyui/ 目录内 governance/source/registry 等的最终命名：以 A5 Package 结构为准
```
