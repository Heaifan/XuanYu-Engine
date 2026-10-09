# 玄域引擎开发执行手册

> 本手册提供宪法的执行细节，不建立独立审批流程。开发规则以 `docs/玄域引擎_AI开发宪法.md` 为准；测试与验收证据由 XYT 管理；长期知识、经验和决策由独立 XYK 管理。

## 1. 开始任务

开始前写明任务、目标、风险、Owner、写入范围、基线和停止条件。一个文件在同一时段只能有一个写入者。需要并行时，先拆开不重叠的写入范围，并确认测试、构建和运行资源不会冲突。

读取 `docs/knowledge/knowledge-index.md`，按任务域加载相关 XYK 条目。记录实际加载内容与约束；发现与有效决策冲突时，先停止冲突范围并报告，不得静默改写 XYK。

任何 Build、Test、Run 或 SDK 判断前，先执行 `scripts/xye-bootstrap.ps1`。该程序经 `scripts/resolve-dotnet.ps1` 解析 SDK；PATH 中没有 `dotnet` 不能证明 SDK 缺失。正式 .NET 命令通过 `scripts/xye-dotnet.ps1` 执行。

## 2. 工作区与 Git 安全

同步前先 fetch，再只读核对 branch、HEAD、upstream、Ahead/Behind、index、tracked 与 untracked Dirty，以及相关写入者。任何来源不明的 Dirty 都必须保留；只暂停与其重叠或依赖该文件的操作。不得自动 reset、clean、stash、checkout 或覆盖。

仅当 index 和 worktree clean、Ahead=0 且 Behind>0 时允许 `git pull --ff-only`。本地领先、分叉或事实不明时停止同步并报告。

子任务不得 Commit / Push。Parent 汇总任务负责精确集成、Stage、Commit、Push 和远端核验。未经用户明确批准不得 Force Push、改写历史、删远端分支、创建 / 合并 PR、Tag 或 Release。

## 3. 风险与验证

风险等级和主验证范围由宪法确定：

- **LOW**：相关构建或编译、专项测试、文档检查、`git diff --check` 与范围核对。
- **MEDIUM**：受影响项目构建、相关测试集、专项回归、架构边界检查与 `git diff --check`。
- **HIGH**：完整解决方案构建、当前适用的 XYT 测试、相关架构检查、专项回归，以及任务要求的运行或用户验收证据。

按风险选择充分的验证范围。纯文档任务不运行无关代码构建。测试命令、测试选择、执行结果、回归证据和用户验收记录通过 XYT 的当前入口处理；不得以部分测试冒充全量，也不得把自动化结果写成用户验收结论。

所有手写 `.cs`、`.axaml`、`.js` 单文件不得超过 100 行。不得删减断言、跳过失败测试、隐藏异常或伪造通过结果。测试或构建失败时保留 stdout、stderr 和退出码，并区分产品、测试设施和环境原因。

## 4. .NET 串行执行

`scripts/xye-dotnet.ps1 build` 与 `scripts/xye-dotnet.ps1 test` 必须串行执行。先 Bootstrap；根据风险只运行相关范围，正式基线再运行完整验证。

```powershell
Set-Location 'E:\MyDoc\project-VSCode\XuanYuEngine'
$dotnet = '.\scripts\xye-dotnet.ps1'
& $dotnet build-server shutdown
$env:MSBUILDDISABLENODEREUSE = '1'
& $dotnet restore .\XuanYu.Engine.slnx
& $dotnet build .\XuanYu.Engine.slnx `
  --no-restore `
  -m:1 `
  -nr:false `
  -p:BuildInParallel=false `
  -p:UseSharedCompilation=false
# 按验证范围选择 XYT 测试，保持串行
& $dotnet build-server shutdown
```

## 5. 报告与知识交接

任务报告写明 `Root Cause`、`Changed Files`、`Tests`、`Evidence` 和 `Known Risk`。施工 Agent 只可在报告中提交 XYK 候选，不得自行写入正式知识、经验或决策。候选交接格式见 `docs/governance/knowledge-audit-handoff.md`；XYK 正式维护按其独立职责处理。

报告必须区分自动验证、运行时证据和用户验收。未执行的项目明确标记未验证；真实 UI、渲染、输入或生命周期验收未完成时，不得宣告对应产品结果完成。

## 6. 文档同步

`changelog.md` 记录实际的重要变化，不记录过程直播。只有新增、删除、移动、重命名正式文件或主要职责变化时才更新 `file-tree.md`。治理改动不自动构成产品版本变化；是否需要产品版本或 Changelog 记录，按宪法和现存历史事实判断，并在报告中说明依据。
