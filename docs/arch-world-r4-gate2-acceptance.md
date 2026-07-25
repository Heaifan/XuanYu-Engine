# ARCH-WORLD-R4 Gate 2 真机验收清单（操作手册）

- 版本：`v0.2.19.5-fix`（维持，不升版）
- 状态：**⏳ AWAITING 用户真机验收**（Gate 0 PASS / Gate 1 全 PASS 已落 `25bd66b`；本清单为 Gate 2）
- 日期：2026-07-25
- 验收方式：真机 `run.bat` 启动编辑器，逐条手工操作 + 脚本门禁核对
- 环境：Windows + Vulkan + Avalonia 编辑器
- 配套图：`arch-world-r4-gate2-acceptance.svg`（验收门控图）

> R4 = 建立 `XuanYu.Editor` 领域层（把 `TransformSession` 由 `World.Transform` 迁入 `XuanYu.Editor.Transform`、`EditorCameraFraming` 由 `Core.Space` 迁入 `XuanYu.Editor.Camera`；`Editor.UI` 改为引用 `XuanYu.Editor`）。其后 FIX1 仅做纯 partial 物理拆分（零行为变化），run.bat 仅关 Roslyn 共享编译（规避 csc 崩溃）。**本验收只验证"机械拆分 + 构建配置变更后，R4 既有的编辑器交互与门禁是否仍成立"，不验证新功能。**

## 一、验收范围与核心风险

R4 把两个编辑器领域类型从 `Core`/`World` 迁出到新程序集 `XuanYu.Editor`，FIX1 又把 4 个超 100 行文件做 partial 拆分。最危险的风险是：

- 新程序集 `XuanYu.Editor` 未正确加载 / 构造路径断裂导致启动崩溃
- 迁移后 `TransformSession` / `EditorCameraFraming` 的调用入口断链（Move Gizmo、Frame 命令失效）
- partial 拆分误改行为（Selection 面板不刷新、Undo/Redo 漏记或错记）
- 构建配置改动后 csc 仍崩溃、或测试/守卫被打断
- 5+100 门禁在真机工作树被重新触发

## 二、前提条件（开验收前必做）

1. 拉取最新远端：`git fetch origin` → `git checkout refactor/ARCH-WORLD-layer-boundary` → `git reset --hard origin/refactor/ARCH-WORLD-layer-boundary`，确认本地 HEAD = **`25bd66b`**。
2. 确认 `run.bat` 已含安全构建配置（`MSBUILDDISABLENODEREUSE=1` + build 行 `-p:UseSharedCompilation=false`）。若构建仍报 `csc.exe 已退出，代码为 1`，先 `dotnet clean` 清 `obj/bin` 再跑，或带 `-v diag` 取真实 `CSxxxx`。
3. 双击 `run.bat`：还原（0 警告即够）→ 构建（0 错误 0 警告）→ 编辑器正常启动进入主视口。**任何一步失败则 Gate 2 直接阻断，先回 B 组排查。**

## 三、验收总览（11 项 → A 组交互 6 项 + B 组门禁 5 项）

| 编号 | 分组 | 验收项 | 类型 | 通过标准 |
|------|------|--------|------|----------|
| A1 | 交互 | Frame All / Frame Selected | 手工 | 相机平滑构图到目标，实体身份不变 |
| A2 | 交互 | Move Gizmo 拖拽 + 轴向约束 | 手工 | 拖拽实时预览，仅命中轴移动，其他轴不变 |
| A3 | 交互 | Undo / Redo 变换（含零位移不计历史） | 手工 | Ctrl+Z/Y 恢复正确；零位移拖拽不入历史 |
| A4 | 交互 | Viewport Picking 选中 / 反选 | 手工 | 视口点击命中实体；点空白反选清空 |
| A5 | 交互 | Selection 面板刷新 | 手工 | 标题/副标题/路径/Key/Inspector 字段随选随刷 |
| A6 | 交互 | Resize 视口 | 手工 | 拖拽缩放窗口，Swapchain 自愈，无崩溃无幽灵 |
| B1 | 门禁 | 构建 0W0E + 168 测试 0 fail | 脚本 | `run.bat` 0 错误 0 警告；`dotnet test` 全绿 |
| B2 | 门禁 | 三架构守卫 EXIT=0 | 脚本 | `scripts/arch-a-guard*.ps1` 三个全部 0 |
| B3 | 门禁 | 5+100 重扫 = 0 | 脚本 | 全仓库手写 .cs/.axaml/.js 均 ≤100 行 |
| B4 | 门禁 | SVG 47/47 XML 合法 | 脚本 | `docs/*.svg` 全部可被 XML 解析 |
| B5 | 门禁 | git status clean + 远端 tip | 脚本 | 工作树无未提交改动；远端 = `25bd66b` |

> A 组为"真机才看得出来"的交互行为，必须由人在编辑器里操作确认；B 组为可脚本化/确定性的门禁，可在真机用相同命令复跑。两者全 PASS 才允许把 R4 裁定为 CLOSED。

## 四、A 组：编辑器交互验收（手工真机）

### A1. Frame All / Frame Selected

- **操作**：在编辑器里分别触发「看全场景」（Frame All）与「聚焦选中实体」（Frame Selected）。触发入口以真机 UI 为准（顶部工具栏对应按钮或快捷键；若不确定，用顶部相机命令按钮）。先选中一个实体，再触发 Frame Selected；之后触发 Frame All。
- **预期结果**：Frame Selected 后相机平滑平移/缩放到该实体包围盒，使其居中可见；Frame All 后相机回到能看见全部 10 个实体的视野。整个过程中**没有任何实体的身份、位置、类型发生变化**（构图只改相机，不改实体）。
- **通过判定**：两次构图后视口内容合理、实体可见、底部日志出现对应相机命令证据（若日志无独立标记，以画面尺度变化 + 实体快照未变为准，不伪称有日志）。
- **风险盯防**：构图后实体"消失"或"串位"＝`EditorCameraFraming` 迁移断链，直接 FAIL。

### A2. Move Gizmo 拖拽 + 轴向约束

- **操作**：切到 Move 工具（顶部工具高亮为"移动"），选中一个实体，Gizmo 三轴出现；分别拖动 X / Y / Z 轴手柄做明显位移，松开提交；再试一次"贴着轴但垂直方向乱晃"的拖拽，验证只沿命中轴移动。
- **预期结果**：拖拽期间实体实时预览跟随；仅被拖动的轴方向产生位移，另外两个轴坐标保持不变；提交后实体落位稳定，日志出现 `移动工具会话开始 → 变换捕获开始 → 移动工具会话结束`（或等价提交链）。
- **通过判定**：三轴各自独立生效、互不串轴；提交后位置即预览终值。
- **风险盯防**：拖 X 轴却 Y/Z 也变＝轴向约束断链；Gizmo 完全不出现＝`TransformSession`/`MoveGizmo` 迁入后捕获入口断链，FAIL。

### A3. Undo / Redo 变换（含零位移不计入历史）

- **操作**：
  1. 选实体 → Move 拖出明显位移 → 提交；按 Undo（Ctrl+Z）应回到位移前；按 Redo（Ctrl+Y）应回到位移后。
  2. 再选同一实体，按下 Gizmo 手柄但**不移动**（或回到原位的零位移拖拽）后松开；之后立即 Undo，观察是否"什么都没发生"（即零位移不进历史）。
- **预期结果**：正常位移的 Undo/Redo 往返正确，视口持续正常绘制、选择链不崩；零位移拖拽**不应**在 History 中留下记录，Undo 对它表现为 no-op（不会把实体"抖"一下或误回退）。
- **通过判定**：步骤 1 往返一致；步骤 2 的 Undo 不改变实体状态（可借底部日志"编辑历史已记录"是否出现来判断：零位移时不应出现该记录）。
- **风险盯防**：零位移也进历史（Undo 抖一下）＝R4 迁移破坏"无变化提交忽略"合同，FAIL；Undo 后视口崩溃＝History/World 回写断链，FAIL。

### A4. Viewport Picking 选中 / 反选

- **操作**：在视口内点击一个可见实体；再点击视口空白处（无实体处）。可重复几次切换不同实体。
- **预期结果**：点击实体 → 该实体被选中（Hierarchy/Project 树对应节点高亮、Inspector 显示其字段）；点击空白 → 当前选择被清空（Inspector 回到无选中态）。
- **通过判定**：每次视口点击都能正确命中/反选，且与树入口选择走同一投影链（日志出现 `来源=视口；键=EntityId(x)` → `选择投影同步` → `PublishSceneRenderSnapshot`）。
- **风险盯防**：视口点击无反应或误选错实体＝Picking 跨 `Editor.UI → UiVm → Core` 链路断链，FAIL。

### A5. Selection 面板刷新

- **操作**：连续选中不同实体（视口点选 + 树点选交替），观察右侧 Inspector 与顶部/相关 Selection 文案。
- **预期结果**：每次切换选择后，Selection 的**标题、副标题、路径、实体 Key、Inspector 字段**立即随新选中实体刷新；`HasSelection` / `IsEmptySelection` 状态正确翻转。
- **通过判定**：切选后面板无残留旧值、无需要手动刷新才更新；交替点选无"选了但面板没变"的延迟。
- **风险盯防**：FIX1 把 `RaiseSelectionChanged()` 从 `UiVm.Selection.cs` 迁到 `UiVm.SelectionProjection.cs`，语义等价。若发现切选后面板不刷或刷一半＝迁移误改行为，FAIL。（正常应通过，此处仅为回归盯防。）

### A6. Resize 视口

- **操作**：用鼠标拖拽编辑器主窗口边缘/角，做多次尺寸变化（放大、缩小、拉扁、拉宽），观察 Swapchain 重建。
- **预期结果**：每次尺寸变化后视口内容正确重绘，无黑屏/撕裂/崩溃；底部日志出现 Swapchain 代际递增与自愈（`Present Out-of-date → Swapchain 重建 → 恢复 Present` 等价）。
- **通过判定**：连续多次 Resize 后编辑器仍稳定运行，实体无幽灵、无残留旧帧。
- **风险盯防**：Resize 后崩溃或画面卡死＝Vulkan 生命周期被 R4 连带破坏，FAIL（R4 不碰 Render，理论上不应发生，纯回归确认）。

## 五、B 组：构建与门禁验收（脚本/自动）

> 在真机 `refactor/ARCH-WORLD-layer-boundary` 分支（HEAD=`25bd66b`）上执行。

### B1. 构建 0W0E + 168 测试 0 fail

- 命令：`run.bat`（还原 0 + 构建 0W0E）；另跑 `dotnet test XuanYu.Engine.slnx --no-build` 或等价于 `run.bat` 已含的测试步骤。
- 通过标准：构建 **0 错误 0 警告**；测试 **168 passed / 0 failed / 0 skipped**（Core 69 + World 99）。
- 失败处理：若 `csc.exe 已退出，代码为 1` 且无 `CSxxxx` → 清 `obj/bin` 重跑，或加 `-p:UseSharedCompilation=false -v diag` 取真实错误回传。

### B2. 三架构守卫 EXIT=0

- 命令：依次执行 `scripts/arch-a-guard.ps1`、`scripts/arch-a-guard-world.ps1`、`scripts/arch-a-guard-editor.ps1`（主脚本会 dot-source 后两个；建议三个都直接跑一遍确认）。
- 通过标准：三个脚本进程退出码均为 **0**，无 `guard fail` 输出。
- 关注点：确认 `Core/World` 不引用 `Editor`；`Editor` 不引用 `Editor.UI/Avalonia/Render.Vulkan/Silk`；依赖方向 `Core ← World ← Editor ← Editor.UI` 成立。

### B3. 5+100 重扫 = 0

- 命令：扫描全仓库手写 `.cs` / `.axaml` / `.js`，排除 `bin/` `obj/` `.git/` `.workbuddy/` `artifacts/`，统计行数 >100 的文件数。
- 通过标准：**0 个**文件 >100 行（FIX1 已将 4 个违例降到 ≤100）。
- 注：本项目用 Python walk 扫（避开 shell 进程替换静默失败）；真机可用任意等价扫描。

### B4. SVG 47/47 XML 合法

- 命令：对 `docs/*.svg` 全部做 XML 解析（如 `python -c "import xml.dom.minidom,glob; [xml.dom.minidom.parse(f) for f in glob.glob('docs/*.svg')]"`）。
- 通过标准：**47/47** 个 SVG 均可被 XML 解析，无格式错误。
- 关注点：FIX1 未新增 SVG，主要确认既有 R4 边界图（`arch-world-r4-editor-boundary.svg`）等仍合法。

### B5. git status clean + 远端 tip

- 命令：`git status --short`（应为空，仅允许未跟踪的导出 zip/日志）；`git ls-remote origin refs/heads/refactor/ARCH-WORLD-layer-boundary`。
- 通过标准：工作树无已跟踪文件改动；远端 tip = **`25bd66b3da0e8532c99053875e738741e4c512ce`**。

## 六、结果记录表（真机逐项勾选）

| 编号 | 验收项 | 结果 | 备注 / 截图或日志锚点 |
|------|--------|------|----------------------|
| A1 | Frame All / Selected | ☐ PASS ☐ FAIL | |
| A2 | Move Gizmo 拖拽 + 轴向约束 | ☐ PASS ☐ FAIL | |
| A3 | Undo/Redo（零位移不计历史） | ☐ PASS ☐ FAIL | |
| A4 | Viewport Picking 选中/反选 | ☐ PASS ☐ FAIL | |
| A5 | Selection 面板刷新 | ☐ PASS ☐ FAIL | |
| A6 | Resize 视口 | ☐ PASS ☐ FAIL | |
| B1 | 构建 0W0E + 168 测试 0 fail | ☐ PASS ☐ FAIL | |
| B2 | 三架构守卫 EXIT=0 | ☐ PASS ☐ FAIL | |
| B3 | 5+100 重扫 = 0 | ☐ PASS ☐ FAIL | |
| B4 | SVG 47/47 XML 合法 | ☐ PASS ☐ FAIL | |
| B5 | git status clean + 远端 tip | ☐ PASS ☐ FAIL | |

**通过条件**：A1–A6 全 PASS 且 B1–B5 全 PASS → Gate 2 关，R4 可裁定 **CLOSED**。任一 FAIL 则阻断，回传失败现象与日志。

## 七、签署与后续

- 真机验收人：__________
- 验收日期：__________
- 裁定：☐ PASS/CLOSED ☐ FAIL（附现象）

**后续**（仅当 Gate 2 全 PASS）：
1. **Gate 3（文档 CLOSED）**：复核 `docs/arch-world-r4-editor-pollution-audit.md` 末尾的 R4-R1 实装结论与 `docs/arch-world-r4-editor-boundary.svg`，确认与代码一致；本验收文档归档。
2. **Gate 4（收口推送）**：本分支 `25bd66b` 已含全部代码+文档+门禁，无需新 push；确认远端 = `25bd66b` 即收口。
3. **进入 R5**：处理受控债务 D2（`SceneRenderSnapshot` / `ISceneRenderSnapshotSource` 含 Editor 语义的边界整理）与 `DefaultEditorCamera` 后门（R4 审计已标记，生产为死代码、UiVm 恒传 Camera）。
