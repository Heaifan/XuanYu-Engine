# 玄域引擎 · 开发硬规则（执行手册）

> 本文件是 `docs/玄域引擎_AI开发宪法.md` 的执行手册。冲突时以宪法为准。
> 目标：保持硬红线不松动，同时按风险使用最小充分流程，减少低风险任务的治理税。

---

## 0. 每轮开工最小检查

写入前确认：

```text
Task
Risk: LOW / MEDIUM / HIGH
Goal
Scope
Gate: GATE-L / GATE-M / GATE-H
Stop Condition
Prohibited
Knowledge Preflight
Candidate Tree
Deferred Capability Preflight
Capability Tags
```

任务必须先声明 `Lane: XYE / XYUI / INTEGRATION / GOVERNANCE`，再读取对应 `tools/governance/**` Authority owner 与 Candidate 依赖；不得调用旧 Handoff JOIN。Lane、Ownership、Audit、Convergence 和跨 Lane 升级的唯一事实源是 `docs/governance/development-lanes.md`。

开始时一次声明 `TaskId + Lane + Owner + WriteScope + BaseSHA + DependsOn`；WriteScope 同时列入直接相关测试与必要审计文件。不得对同一有界任务逐文件重复索取授权。扩展到无关 Owner、生产 Authority 状态、高风险数据、Release、历史改写或 P4 验收仍走对应的明确授权。

Feature 写入前必须完成 Candidate Tree、Planning/Execution Knowledge Preflight、Deferred Capability Preflight 和 Capability Tags。若命中的 Deferred Capability 为 `BLOCKING`，则 `CODING = BLOCKED`；不得用局部测试 PASS 绕过。

共享 Workspace 只保留一个当前分支和一个 HEAD；普通 Agent 禁止创建 / 切换 Branch 和额外 Worktree。`ForeignDirty != OwnershipConflict`：只有写入范围与 Ownership 重叠或实际消费冲突依赖才阻断，未知归属只暂停受影响路径。

必须确认当前分支、HEAD、远端 tip、工作区与任务范围一致。未知修改不得覆盖。

不再强制“普通目标 ≤3”，也不要求每条中间回复重复完整 TODO；限制的是未解决依赖链和失控扩围。

---

## 1. 5+100 绝对红线

所有手写 `.cs` / `.axaml` / `.js` 单文件 **≤100 行**。

- 无临时例外；
- 无“复杂文件”例外；
- 无“单一职责所以可以超限”例外；
- 历史债务不产生继续超限的权利；
- 不得用 `partial`、压缩格式、多语句挤行、无意义转发类或伪装生成文件规避；
- 复杂度只能通过职责分解解决。

`arch-a-guard` / 代码预算测试是机器保护；即使机器检查暂时未覆盖某扩展名，也不解除红线。

---

## 2. 风险分级

### LOW

局部、可回退、契约不变、影响范围明确。

### MEDIUM

功能行为、状态流、交互行为或多个相关文件发生变化，但不改变重大公共契约、Schema 或架构边界。

### HIGH

涉及架构边界、权威状态、公共 API / Schema、存档 / 迁移、坐标 / 单位、新依赖、跨层主链、大范围重构、重大性能地基、数据完整性或冻结 UX。

HIGH 中需要产品 / 架构决策的部分必须先获得用户批准。

---

## 3. 验证门禁

### GATE-L

LOW 至少执行：

- 相关项目 Build 或等价最小编译验证；
- 相关专项测试；
- 5+100；
- `git diff --check`；
- Scope 检查。

纯文档 LOW 任务用 Markdown / 链接 / 内容一致性 / diff 检查替代无意义代码 Build。

### GATE-M

MEDIUM 至少执行：

- 受影响项目 Build；
- 受影响测试集；
- 相关架构检查；
- 专项回归；
- 5+100；
- `git diff --check`。

### GATE-H

HIGH、阶段可信基线、正式验收 / Release 前至少执行：

- 完整解决方案 Build，0 Warning / 0 Error；
- 当前适用的正式测试套件；
- Architecture Gate；
- 5+100；
- 专项回归；
- `git diff --check`；
- 任务需要的运行、真机或保存闭环。

全量门禁没有取消，只在真正需要建立可信基线的节点执行。

三个治理子 Gate（Deferred Capability、Test Evidence T0-T4、Regression Witness/Harness Integrity）只能作为现有 GATE-L/M/H 的子 Gate；不得建立第二套主 Gate、主 Constitution 或主 Report System。

共享编译进入 `Compile-RED` 时，优先恢复 XYE Fast Lane 为 Green；XYUI 必须修复自身造成的共享编译红灯。XYUI 普通失败不阻断 XYE，真实 XYE Consumer 破坏才升级为 `P0.5 INTEGRATION`。

---

## 4. .NET 稳定执行模板

`scripts/xye-dotnet.ps1 build` 和 `scripts/xye-dotnet.ps1 test` 必须串行，并且先执行 `scripts/xye-bootstrap.ps1`。

```powershell
Set-Location 'E:\MyDoc\project-VSCode\XuanYuEngine'

$dotnet = '.\\scripts\\xye-dotnet.ps1'
& $dotnet build-server shutdown
$env:MSBUILDDISABLENODEREUSE = "1"

& $dotnet restore .\XuanYu.Engine.slnx

& $dotnet build .\XuanYu.Engine.slnx `
  --no-restore `
  -m:1 `
  -nr:false `
  -p:BuildInParallel=false `
  -p:UseSharedCompilation=false

# 按 Gate 选择相关测试，全部串行，并优先 --no-build --no-restore

& $dotnet build-server shutdown
```

规则：

- 一次只运行一个 dotnet 命令；
- 完整 Solution Build 在一次有效验证链中只跑一次；
- 后续测试优先 `--no-build --no-restore`；
- 环境锁、权限或 SDK 缺失必须报告为环境阻断，不得伪装代码失败 / 成功；
- 同一失败命令只有环境事实变化后才重试；
- 禁止无限重试。

---

## 5. Scope 与受控邻接修复

允许：计划内文件、完成目标必需的关联文件、必要回归测试、本轮直接引入问题。

受控邻接修复必须同时满足：

```text
与当前根因直接相关
不改公共 API
不改 Schema
不引入新依赖
不改变无关行为
可由当前任务验证
```

否则停止扩围。

禁止借局部 Bug 顺手全模块重构、顺手引入新抽象 / 依赖 / Schema 或改变冻结 UX。

---

## 6. 两次失败后的假设重置

同一根因假设 + 同一路径修复连续失败两次：停止该假设。

继续前必须：

```text
新证据
→ 解释旧假设为何失效
→ 新根因假设
→ 再修复
```

无新证据不得第三次重复撞同一路径。

复杂 Bug 使用最小诊断链：

```text
候选路径 → 最小探针 → 一次运行 → 根因 → 修复 → 验证 → 清理临时探针
```

---

## 7. Knowledge Preflight

MEDIUM / HIGH 或 `docs/knowledge/knowledge-index.md` 已登记任务域：

1. 查 Index；
2. 读取相关 DEC / K / L / EXP；
3. 在 Task State 列出实际 Loaded ID；
4. 只读相关内容，不全文加载知识库。

```text
Knowledge Preflight
Task Domain: ...
Loaded:
- ...
```

Agent 错误经验：

```text
docs/governance/agent-error-log.md
docs/governance/agent-experience-rules.md
```

Codex / Gemini / 其他执行 Agent 对上述 ERR / EXP 正式库只读。

治理任务还必须记录 Deferred Capability Preflight：声明命中的 capability tags、有效状态、阻塞能力和 `CODING` 判定。该检查读取 `docs/governance/deferred-capabilities.json`，不改变既有主 Gate 的权威性。

---

## 8. 任务结束知识回写

只判断一次：

```text
长期决策？       → DEC
长期工程规律？   → Knowledge
复盘教训？       → Lesson / Incident
Agent 真实失误？ → ERR
防复发规则？     → EXP
```

全部为否：`Knowledge Writeback: none`。

施工任务最终报告不得在此处直接完成知识写回，必须附加 [Knowledge / Experience Audit Handoff](governance/knowledge-audit-handoff.md)。Agent 只提交事实、Root Cause、Changed Files、Tests、Evidence、Known Risk 和 Candidate Lessons；正式 Knowledge/Experience 由 ChatGPT 审计 AI 先 `SEARCH EXISTING`，再决定 `UPDATE / STRENGTHEN / CREATE / RETIRE / NO DEPOSIT`。

固定尾部：

```text
==================================================
KNOWLEDGE / EXPERIENCE AUDIT HANDOFF
==================================================
CHATGPT KNOWLEDGE AUDIT REQUIRED
```

Plan / Audit / 临时日志不自动成为长期知识。

### Feature Task Preflight

固定顺序：`Candidate Tree → Knowledge Preflight → Deferred Capability Preflight → Capability Tags`。缺少任一项时任务不得进入写入阶段；命中 `BLOCKING` 时输出 `CODING = BLOCKED`。

### Fix Closure Template

每个 Bug/Fix Closure 必须固定输出：

```text
BUG SYMPTOM
PRODUCT ROOT CAUSE
TEST HARNESS ROOT CAUSE
ROOT CAUSE CLASSIFICATION: PRODUCT | TEST_HARNESS | MIXED | UNKNOWN
REGRESSION WITNESS
PRE-FIX RESULT
POST-FIX RESULT
REQUIRED EVIDENCE TIER
ACHIEVED EVIDENCE TIER
RUNTIME ACCEPTANCE
PRODUCT ACCEPTANCE
EVIDENCE GAP
```

`UNKNOWN` 不得填成 PASS；Retroactive 只能记录 `PRE-FIX RESULT = NOT_EXECUTED`，不得伪造历史 RED。`Tests x/x PASS` 不是完整 Fix Closure。

---

## 9. 架构依赖硬隔离

- Core 不依赖 UI / Runtime / Vulkan / Windows / Android / Avalonia；
- `Editor.UI` 不得直接引用或控制 Vulkan 实现生命周期；
- `Render.Abstractions` 不得引用 `Silk.NET.Vulkan`；
- `Render.Vulkan` → `Render.Abstractions`；
- `Editor` → Core / World，不得反向依赖 UI；
- `Editor.UI` 可消费 Editor 与 Render.Abstractions；
- Vulkan 实现与资源生命周期归 Vulkan 后端 / 正式组合根。

任何过渡债务不得扩散为新依赖。

---

## 10. 状态所有权

同一长期事实只能有一个可写 Owner。

`GlobalWorld → EntityRegistry → Entity State` 是实体生命周期权威链。UI、Renderer、Hierarchy、Inspector、Snapshot、Picking、Gizmo 只能投影 / 缓存派生结果。

`SpatialIndex` 是查询加速派生结构，不是 Entity、Position、Region 或 Activity 事实源。

`QueryRadius` / `QueryBounds` 返回候选身份后，调用者回到正式 World 查询实体事实。

不得新造第二套长期 Entity ID、UI ID、Render ID 或 Registry Key。

---

## 11. 高频链路

PointerMoved / Hover / DragPreview / RenderFrame / Resize / Picking：

允许：轻量 Preview、轻量内存状态、请求重绘、增量查询。

禁止：

- 每次全场景 O(N)；
- 写正式 World；
- 提交 Undo；
- 持久化；
- 刷重型 Inspector；
- 普通日志洪泛；
- 高频重建 Swapchain；
- 每帧重建整个世界 / 地图数据。

Preview 与 Commit 重路径必须分离。

---

## 12. Preview / Commit / Cancel

### Preview

只更新临时预览；不污染正式世界、不持久化、不提交 Undo。

### Commit

一次性写权威状态，记录必要历史，进入 Undo/Redo。

### Cancel

恢复会话开始前状态，不残留部分提交，延迟输入不得复活旧会话。

Undo / Redo 恢复已提交 Before / After Snapshot，不重新模拟输入 Delta；新 Commit 清空旧 Redo 分支。

---

## 13. 输入与工具状态

- 同一 Pointer 手势只能有一个实时 Owner；
- Capture 必须有完整释放生命周期；
- `ActiveTool` 只表示持续编辑工具；
- Snap 是 Toggle，Undo / Redo / Focus 是 Command；
- UI 高亮、状态文本与 SessionTool 必须来自同一权威工具快照；
- 未实现工具不得偷偷退化成另一个已实现工具。

---

## 14. 坐标与 Picking

World Space：右手系、`+Z` Up、XY 水平，`X × Y = Z`。

Camera / Picking / Gizmo / Render 必须消费同一套正式 Camera / Viewport / ViewProjection 事实。

禁止用 `-X`、`-Y`、Swap(Y,Z) 等视觉补丁互相抵消错误。

Picking 正式主路径禁止 `GetAllEntities` 后全场景逐个扫描，禁止每次点击临时重建整个空间索引，禁止 PointerMoved 默认持续全量 Picking。

---

## 15. 日志与诊断

普通日志只记录可行动的低频事实。

高频诊断必须在源头合并 / 限流并走非阻塞路径；不得同步阻塞 UI / 输入 / Render 线程。

临时探针：中文前缀、只覆盖当前目标、根因验证后同轮清理；只有长期价值时才转正式日志。

---

## 16. Vulkan 生命周期

阶段边界仍遵守：

```text
VK1：探针
VK2：NativeHost / HWND
VK3：Surface
VK4：Swapchain 最小闭环
```

不得提前夹带下一阶段主能力。

资源创建必须同时具备销毁；所有 VkResult 分类处理；Resize 标记 dirty 后统一重建；跳过 0 尺寸 / 重复尺寸；UI 线程禁止无限等待。

---

## 17. 数据与 Schema

存档、`.xyscene`、`.xymap`、公共 Schema、单位制、坐标系、数据迁移、兼容策略、外部公共 API、新增 / 更换重大依赖：事前获得用户批准。

保存 / 加载相关任务必须按风险覆盖：

```text
保存 → 关闭 → 重开 → 数据一致
```

覆盖保存、异步危险确认、资源归一化等先查对应 K-DATA / K-ASSET。

---

## 18. 中文 IPO

IPO 强制用于：

- 人工 / 真机验收；
- HIGH 风险跨层数据流；
- 状态机；
- 持久化；
- Undo/Redo；
- 公共 API / Schema；
- 复杂事件链。

真机格式：

```text
序号：
路径：真实中文 UI 路径
输入 I：
过程 P：
输出 O：可观察、可判断
```

普通 LOW / MEDIUM 局部任务可使用：`Changed / Verified / Residual Risk`。

---

## 19. Git 与交付

Git 提交以原子、可验证成果为单位，不为每个微编辑单独 Commit。

正式交付：

```text
修改
→ 匹配风险的验证
→ diff / scope 检查
→ Commit
→ Push
→ 远端 tip 复核
```

只有实际核验远端 tip 后才能声明“已推送 / 本地远端一致”。

未经用户批准禁止 Force Push、Rebase、改写历史、删除远端分支、创建 / 合并 PR、Tag、Release。

正式验收成果不得长期只留在本地。

Convergence 前必须输出 `PRODUCT REGRESSION`、`UNRESOLVED UNKNOWN`、`GATE STATUS`、`CANDIDATE TREE MATCH`、`COMMIT ELIGIBILITY`，并证明 `ForeignDirty = 0`、`UnknownDirty = 0`、`Staged = 0`。Commit Eligibility 仅在这些条件全部满足后成立。

XYUI 默认只写 `xyui/**`，经 Audit PASS 和 Freeze 后由 Coordinator 批量入库；XYUI Agent 不得自行 `git add` / `commit` / `push`。XYE 与 Integration 的 Ownership 必须在任务声明中列出文件表。Convergence 期间 Coordinator 独占 Workspace，其他 Lane Freeze。

---

## 20. 文档最小同步

`changelog.md` 记录实际重要变化，不记录开发直播。

`file-tree.md` 只在：新增 / 删除 / 移动 / 重命名正式文件，或主要职责变化时更新；普通内部实现变化不更新。

不得为了每个 Fix 新建 audit / final / final2 文档。

正式 Milestone 关闭前做一次 Knowledge Review，小 Fix 不机械生成重型报告。

---

## 21. XYUI 与多 Agent

XYUI 唯一 Canonical 根：仓库 `xyui/`。

Engine、XYUI Runtime、Gallery、Tests 共用正式 Git、版本、构建与维护生命周期，但保持独立项目 / 程序集边界。

正式功能、架构、数据和测试优先于未推送 UI 实验；未知 worktree / recovery / 临时实验不得自动并入正式基线。

XYUI Audit 分为：L（Gallery / Visual / 非 Public Runtime Contract）、M（Runtime 行为改变且无 Public API，追加受影响 Runtime Tests 与 XYE Consumer Smoke Compatibility）、H / Integration（Public API、Default Layout Contract 或全局行为改变，追加 Public Contract Diff、受影响 XYE Consumer Build/Test、架构边界与 5+100）。

---

## 22. 最终停止条件

达到本轮 Goal、Gate、必要文档、Commit / Push / 远端复核后即停止当前开发轮。

需要真机验收的功能进入“待真机验收”，不得把自动测试通过冒充 CLOSED，也不得未经批准自动开始下一阶段。
