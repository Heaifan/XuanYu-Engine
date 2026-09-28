# SESSION HANDOFF — MANDATORY

> **本节优先于后续 Repository Bootstrap、Build/Test、任务规划和代码修改规则。**

每个 Workspace / Development Wave：Coordinator 执行一次 `tools\handoff\handoff.cmd prepare` 建立 Workspace Baseline；同一 Workspace 的其他并行 Session 执行 `tools\handoff\handoff.cmd join`。JOIN 为只读操作，允许 Workspace 因其他 Ownership Task 而 dirty。Active Wave 期间禁止再次 PREPARE；Convergence / Commit / Push / Clean 后执行 `tools\handoff\handoff.cmd close`。不得自行猜测 Active Branch、Canonical Workspace、SDK 路径或本地 dirty 文件的保留优先级。

交接权威顺序：

```text
本地独有正式 Commit > GitHub Remote > 本地未提交 / untracked 残留
```

当本地相对 Active Remote **Ahead = 0 且 Behind > 0** 时，执行 **REMOTE WINS**：本地未提交 tracked 修改和 untracked 残留不得阻断同步，由交接程序清理并精确对齐远端。只有本地存在 Remote 没有的正式 Commit 时才阻断自动覆盖。

.NET SDK 必须由交接程序通过正式 `scripts/resolve-dotnet.ps1` Resolver Chain 解析。已登记首选路径：

- 私人电脑：`D:\MyApp\sdk-dotnet\dotnet.exe`
- 工作电脑：`E:\MyApp\sdk-dotnet\dotnet.exe`

PATH 中没有 `dotnet` 不等于 SDK 不存在。

# AGENTS.md — XuanYu Engine（玄域引擎）AI 协作入口

> 仓库内 AI 编码工具统一入口。  
> **唯一权威规则：`docs/玄域引擎_AI开发宪法.md`（3.1）。**
> 代码硬规则：`docs/CODE_CONSTITUTION.md`。  
> 执行手册：`docs/dev-rules.md`。

## 项目

- C# 游戏编辑器（Avalonia UI + Vulkan 渲染）
- 解决方案：`XuanYu.Engine.slnx`
- GitHub 是正式跨设备事实源；当前分支和远端关系每轮以实际仓库核对为准
- XYUI 是仓库内一等内置子系统，Canonical 根固定为 `xyui/`

## Lane / Ownership

每个任务先声明 `Lane: XYE / XYUI / INTEGRATION / GOVERNANCE`，再执行 `tools\handoff\handoff.cmd join --scope <lane>`。唯一 Lane 事实源为 [`docs/governance/development-lanes.md`](docs/governance/development-lanes.md)。

- `P0 XYE FAST LANE`：主线优先；XYE 拥有 Engine 产品与测试文件。
- `P1 XYUI CONTROLLED SUPPORT LANE`：默认仅 `xyui/**`，继续开发、审计、进入同一 Git/GitHub，但默认禁止自行 Commit/Push/Branch mutation。
- `P0.5 INTEGRATION`：必须明确声明，并提供 XYE Consumer 与 XYUI Runtime/Public Contract 的 Ownership 文件表；真实 Consumer 破坏才是 Blocker。
- `GOVERNANCE`：仅治理文档、Handoff 与 Lane 规则；不借治理 Lane 修改产品代码。

`ForeignDirty != OwnershipConflict`：其他 Lane 的 dirty 可以使 JOIN 通过；只有即将写入的文件与当前 Ownership 冲突才阻断。共享编译出现 `Compile-RED` 时 XYE 优先恢复 Green，XYUI 必须修复自身造成的红灯。Convergence 时 Coordinator 独占 Workspace，其他 Lane Freeze；普通 Agent 禁止创建 / 切换 Branch 或额外 Worktree。

## 不可侵犯红线

1. **5+100**：所有手写 `.cs` / `.axaml` / `.js` 单文件 ≤100 行；无临时例外、无复杂文件例外、无“单职责即可超限”例外。
2. **事实真实性**：Evidence Before Claim；未执行不得称通过，局部不得冒充全量，本地不得冒充远端，推测不得冒充根因。
3. **唯一事实源**：UI / Renderer / Inspector / Snapshot / 索引只能投影或派生领域事实。
4. **分层边界**：`Editor.UI` 不得直接依赖 Vulkan 实现；`Render.Abstractions` 不得引用 `Silk.NET.Vulkan`。
5. **高频性能**：PointerMoved / Hover / DragPreview / RenderFrame 等正式主链不得依赖可预见的大规模 O(N) 全量扫描或重型副作用。
6. **失败不得掩盖**：空 catch、弱化断言、删测试、跳门禁、伪造结果均禁止。
7. **敏感信息与 AI 私有过程禁入库**。

## 每轮入口

```text
-1. 声明 Lane：XYE / XYUI / INTEGRATION / GOVERNANCE
-2. Session Handoff：Coordinator 用 prepare；并行 Session 用 `join --scope <lane>`（JOIN PASS 后继续）
0. Repository Bootstrap：由 prepare / join 按 Resolver Chain 执行 scripts/xye-bootstrap.ps1
1. 接管核对 Git / 工作区
2. Planning 阶段：MEDIUM / HIGH 或已登记任务域 → Knowledge Planning Preflight
3. Task State：Task / Risk / Goal / Scope / Gate / Stop / Prohibited
4. Execution 阶段：Handoff JOIN + Task State 后、第一处文件写入前 → Knowledge Execution Preflight
5. 实装
6. 按 GATE-L / GATE-M / GATE-H 验证
7. Knowledge Writeback 判断
8. 原子 Commit → Push → 远端 tip 复核
9. 需要真机时进入“待真机验收”
```

Planning / Execution Knowledge Preflight 必须输出：

```text
Knowledge Preflight
Phase: PLANNING | EXECUTION
Task Domain:
Loaded:
Hard Constraints:
Conflict Check: PASS | BLOCKED
```

如果 `Conflict Check = BLOCKED`，禁止修改代码或治理文件。ACTIVE P0 Knowledge 只有显式 Governance Supersede（更新原条目状态、提供新证据并获治理批准）才能被新实现推翻；不得静默改写历史条目。

任何 Build / Test / Run / SDK 判断前，必须先执行 Repository Bootstrap，遵守 Repository Bootstrap / Resolver First，或通过 `run.bat` 进入同一 Resolver Chain。只有 `scripts/resolve-dotnet.ps1` 实际失败后，才允许报告 .NET SDK 不可用；PATH 中没有 `dotnet` 不等于 SDK 不存在。正式 .NET 命令统一通过 `scripts/xye-dotnet.ps1`。

不再强制“普通目标 ≤3”，也不要求每条中间报告重复完整 TODO。限制未解决依赖链、失控并行和 Scope Expansion。

## 风险与验证

### LOW → GATE-L

- 最小相关 Build / 编译验证
- 相关专项测试
- 5+100
- `git diff --check`
- Scope 检查

纯文档任务不无意义运行完整代码 Build。

### MEDIUM → GATE-M

- 受影响项目 Build
- 受影响测试集
- 相关架构检查
- 专项回归
- 5+100
- `git diff --check`

### HIGH → GATE-H

- 完整 Solution Build：0 Warning / 0 Error
- 当前适用正式测试套件
- Architecture Gate
- 5+100
- 专项回归
- `git diff --check`
- 任务要求的运行 / 真机 / 数据闭环

`scripts/xye-dotnet.ps1 build` / `scripts/xye-dotnet.ps1 test` 始终串行；完整门禁按风险和可信基线节点执行，而不是每个微编辑都重复执行。

## 两次失败规则

同一根因假设 + 同一路径修复连续失败两次后，停止该假设。

只有收集新证据、说明旧假设为何失效并建立新假设后才能继续；禁止无新证据第三次重复撞同一路径。

## Scope

允许当前根因所必需的受控邻接修复，但必须：

- 与根因直接相关；
- 不改公共 API；
- 不改 Schema；
- 不引入新依赖；
- 不改变无关行为；
- 可由当前任务验证。

否则停止扩围。

## 知识与经验入口

开发前索引：`docs/knowledge/knowledge-index.md`

长期职责：

```text
DEC        已批准长期决策
Knowledge  经工程证据验证的规律
Lesson     错误前提 / 停止条件 / 教训
ERR        Agent 实际犯错事实
EXP        防复发经验规则
```

Agent 错误权威库：

- `docs/governance/agent-error-log.md`
- `docs/governance/agent-experience-rules.md`

正式 ERR / EXP 由 ChatGPT 创建、修改、合并和关闭；Codex / Gemini / 其他执行 Agent 只读并按任务加载。

## 人工验收

人工 / 真机使用中文 IPO：

```text
序号
路径
输入 I
过程 P
输出 O
```

必须使用当前 UI 真实中文路径，输出必须可观察、可判定。

普通 LOW / MEDIUM 局部任务不强制 IPO，可使用 `Changed / Verified / Residual Risk`。

## Git

正式成果以原子、可验证节点 Commit，不要求每个微小编辑单独提交。

未经用户批准禁止 Force Push、Rebase、改写历史、删除远端分支、创建 / 合并 PR、Tag、Release。

必须实际核验远端 tip 后才能声明 Push 完成和本地 / 远端一致。

## 收口

自动测试通过不等于需要真机的 UI、渲染、输入、生命周期阶段 CLOSED。

达到 Goal + Gate + 必要文档 + Commit/Push/远端复核后停止当前开发轮；需要真机时等待用户验收，不主动扩展下一阶段。
