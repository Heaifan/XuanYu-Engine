# AGENTS.md — XuanYu Engine（玄域引擎）AI 协作入口

> 本文件是仓库内所有 AI 编码工具（Claude Code / Codex / Cursor / Hermes 等）的统一入口。
> **唯一权威规则：`docs/玄域引擎_AI开发宪法.md`（2.2 版，17 章 86 条）。**
> 本文件只做索引与红线摘要；两者冲突时以宪法为准。执行手册见 `docs/dev-rules.md`。

## 项目

- C# 游戏编辑器（Avalonia UI + Vulkan 渲染），解决方案 `XuanYu.Engine.slnx`
- 正式开发分支跟随当前主里程碑；实际分支与远端关系以本轮 Git 接管核对为准

## 多 Agent 通道（DEV-FIRST）

- 正式开发 Agent 使用有 upstream 的里程碑分支，每轮必须 Commit + Push；GitHub 是正式事实源。
- UI Agent 默认使用独立 `local/<任务>` 分支与 worktree，可本地 Commit，但不设 upstream、禁止 Push。
- 双方冲突时正式功能、架构、测试和共享元数据优先；UI 基于最新正式远端 HEAD 重新适配，不得阻塞主开发。
- 正式开发只显式暂存本轮文件；禁止把 UI 本地 Commit 放在正式分支上，以免被后续 Push 传递到 GitHub。

## 硬红线（违反即违宪）

1. **5+100 行**：每个手写 `.cs` / `.axaml` ≤ 100 行（含生成物），arch-a-guard 硬门禁
2. **分层边界**：`Editor.UI` 不得直接依赖 Vulkan；`Render.Abstractions` 不得引用 `Silk.NET.Vulkan`
3. **串行 dotnet 门禁**：一次只运行一个 dotnet 命令；解决方案只完整构建一次；测试用 `--no-build`
4. **禁止掩盖失败**：空 catch、弱化断言、删测试、跳过门禁一律禁止
5. **敏感信息禁入库**：密钥、AI 聊天记录、本地工具状态（`.agents/`、`.workbuddy/`、`.hermes/`）一律不进 git

## 每轮流程

冻结目标（≤3 项）→ 只读调查（禁凭计划猜文件名）→ 实装 → 快速验证 → 正式门禁 → 最小文档同步 → commit + push → 等待真机验收

## 正式门禁（严格串行）

```bash
dotnet build-server shutdown
export MSBUILDDISABLENODEREUSE=1
dotnet build XuanYu.Engine.slnx --no-restore -m:1 -nr:false -p:BuildInParallel=false -p:UseSharedCompilation=false
dotnet test <测试项目> --no-build --no-restore   # 每个测试项目串行
scripts/arch-a-guard.ps1                          # 架构守卫（依赖边界 + 5+100）
git diff --check
dotnet build-server shutdown
```

## 文档同步（每轮必做）

- `changelog.md`：顶部新条目（版本 / 日期精确到秒 / 目标 / 变化 / 验证 / Hash / 遗留），只记真实执行结果
- `file-tree.md`：从 `git ls-files` 重建，每个 tracked 文件一句话职责；无版本号、无阶段号、无职责索引
- 版本号**四处**一致：`changelog.md` / `run.bat`(title) / `XuanYu.Editor.UI/Win/UiWin.axaml`(Title) / `XuanYu.Editor.UI/Vm/Scene/UiVm.SceneDocument.cs`(DocumentWindowTitle)
- SVG 按宪法第六十八条按需生成（非每轮强制），生成后必须 XML 校验

## 验收

- 自动测试通过 ≠ CLOSED；真机验收由用户负责，未验收不得启动下一阶段
- 真机/人工验收清单写 IPO 格式（序号 / 路径 / 输入 / 过程 / 输出），界面文字用中文
- 服务层能力若没有 UI 入口，不进真机清单（归自动测试）
