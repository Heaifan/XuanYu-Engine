# changelog

## v0.2.16.8-rz
ARCH-B-R3：交互捕获与 Preview / Commit / Cancel 事务边界（2026-07-17 20:15:16，实施）

- 原历史编号：ARCH-B-R3
- 日期：2026-07-17 20:15:16
- 任务目标：在选择状态与活动工具状态封版后，建立最小交互捕获所有权和 Preview / Commit / Cancel 事务边界，避免后续 Gizmo、Picking、视口输入和正式状态写入各自维护拖动状态。
- 主要改动：新增 `EditorInteractionSnapshot`、交互命令、交互变更结果和 `EditorStateOwner.Interaction` 分部，统一管理 Idle / Captured、SessionId、OwnerTool、开始快照和最新 Preview；`UiVm` 拆出 `UiVm.Tool.cs` 与 `UiVm.Interaction.cs`，工具切换、选择切换、Escape、窗口关闭和 NativeHost Detach 均汇聚到同一个 Cancel 路径；右侧调试页新增最小事务测试面板，可触发 Begin / Preview / Commit / Cancel；主窗口标题与 `run.bat` 同步到 `v0.2.16.8-rz`。
- 修改范围：`XuanYu.Editor.UI/EditorState/EditorInteraction*.cs`、`EditorStateOwner.Interaction.cs`、`XuanYu.Editor.UI/Vm/UiVm.cs`、`UiVm.Tool.cs`、`UiVm.Interaction.cs`、`UiVm.Selection.cs`、`UiVm.Logging.cs`、`XuanYu.Editor.UI/Right/Right.axaml`、`Left.axaml.cs`、`UiWin.axaml(.cs)`、`VulkanNativeHost.cs`、`run.bat`、`changelog.md`、`file-tree.md`。未修改 Render.Vulkan、Swapchain、Resize、Present、自愈、Picking、真实 Gizmo、WorldState、Undo / Redo、存档格式或第三方依赖。
- 验证结果：`powershell -ExecutionPolicy Bypass -File scripts/arch-a-guard.ps1` 通过；`dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 6 项目 0 warning / 0 error；`git diff --check` 通过（仅 LF/CRLF 工作区提示）；全仓 5+100 扫描无超限输出。项目暂无测试工程，本轮未引入测试框架或第三方依赖；重复 Begin、非 Owner 拒绝、Commit / Cancel 回 Idle 等需通过右侧调试面板和后续人工验收确认。
- Commit Hash：主实现提交 `a706e975795fc694abc3cc9fc481bfc0f3efb6ba`；不追记回填提交自身 Hash。
- Push 状态：待本轮验证、提交后推送；未创建 Tag / Release。
- 遗留问题：本轮只建立交互事务地基，尚未开发完整移动 Gizmo、真实 Picking、Transform Preview、WorldState 写入、Undo / Redo 或场景存档。

## v0.2.16.7-fix
Vulkan 既有 5+100 超限文件纯结构拆分（2026-07-14 22:41:08，修复）

- 原历史编号：ARCH-B-R2-POST-5+100
- 日期：2026-07-14 22:41:08
- 任务目标：在进入 ARCH-B-R3 前，先治理全仓已发现的两个既有 5+100 硬红线文件，让 `VulkanNativeHost.cs` 与 `VulkanSwapchainOwner.cs` 回到 100 行以内。
- 主要改动：`VulkanNativeHost.cs` 将后台 Present 泵日志回 UI 线程的两个方法拆入 `VulkanNativeHost.Log.cs`；`VulkanSwapchainOwner.cs` 将只读访问器与内部 `Log` 辅助拆入 `VulkanSwapchainOwner.Accessors.cs`；主窗口标题与 `run.bat` 同步到 `v0.2.16.7-fix`。
- 修改范围：`XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`、`VulkanNativeHost.Log.cs`、`XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.cs`、`VulkanSwapchainOwner.Accessors.cs`、`XuanYu.Editor.UI/Win/UiWin.axaml`、`run.bat`、`changelog.md`、`file-tree.md`。仅做结构拆分，不改变 Attach、Resize、Present、自愈、释放顺序、公开契约、Picking、Gizmo、Transform Preview、场景存档或第三方依赖。
- 验证结果：开发中预检目标文件行数为 `VulkanNativeHost.cs` 92 行、`VulkanNativeHost.Log.cs` 15 行、`VulkanSwapchainOwner.cs` 99 行、`VulkanSwapchainOwner.Accessors.cs` 17 行；`dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 6 项目 0 warning / 0 error；人工验收（2026-07-14 23:13:39）正式通过：启动通过，`XuanYu.Editor.App` 注入路径正常，Vulkan Instance / Surface / LogicalDevice / Swapchain / Pipeline 创建成功，首帧 Present 成功；日志栏展开 Resize 回归通过，Swapchain 从 `1248×1110` 实际重建到 `1248×478`，代际 `gen=1 → gen=2`，Framebuffer 重建并重录 CommandBuffer，随后 UI 合并 Resize 正确跳过第二次重建，未出现 DeviceLost、Fatal 或黑屏；正常关闭释放链通过，Present 泵、GraphicsPipeline、RenderPass + Framebuffer、RenderSession、Swapchain、LogicalDevice、Surface、Instance 与 Bridge 均按序释放。
- Commit Hash：主实现提交 `4ac7d977b4dcf13356039cfcecbdb17f9f115d8a`；不追记回填提交自身 Hash。
- Push 状态：主实现与 Hash 回填已推送到 `origin/fix/RZ-VK3-A-surface-contract`；本条人工验收收口记录待提交后推送；未创建 Tag / Release。
- 遗留问题：`v0.2.16.7-fix` 正式验收通过，可进入 `v0.2.16.8-rz — ARCH-B-R3`；R3 仍禁止开发完整移动 Gizmo、真实 Picking、修改 Vulkan、通用事件总线、Undo / Redo 或场景存档格式。

## v0.2.16.6-rz
ARCH-B-R2：活动工具状态与工具捕获状态唯一所有权（2026-07-14 22:25:24，实施）

- 原历史编号：ARCH-B-R2
- 日期：2026-07-14 22:25:24
- 任务目标：在 ARCH-B-R1 选择状态封版后，建立活动工具状态和工具捕获状态的唯一所有权，让工具栏只提交工具切换意图，状态栏明确区分“工具”和“编辑器交互阶段”。
- 主要改动：`EditorStateOwner` 改为 partial 并新增工具状态分部；新增 `EditorToolId`、`EditorToolSnapshot`、`ChangeEditorToolCommand`、`EditorToolChangedResult` 和工具文本映射；`UiVm` 移除活动工具可变字段和工具布尔字段，工具名称、高亮和 `FooterMode` 全部从 Owner 的只读工具快照派生；顶部工具按钮 `IsChecked` 改为单向绑定，只通过命令提交切换意图；选择项目或层级节点后不再把 `FooterState` 写成“状态：聚焦”，避免与“聚焦”工具混淆；主窗口标题与 `run.bat` 同步到 `v0.2.16.6-rz`。
- 修改范围：`XuanYu.Editor.UI/EditorState/EditorStateOwner.cs`、`EditorStateOwner.Tool.cs`、`EditorToolId.cs`、`EditorToolText.cs`、`EditorToolSnapshot.cs`、`EditorToolCommand.cs`、`EditorToolChangedResult.cs`、`XuanYu.Editor.UI/Vm/UiVm.cs`、`UiVm.Selection.cs`、`XuanYu.Editor.UI/Top/Top.axaml`、`XuanYu.Editor.UI/Win/UiWin.axaml`、`run.bat`、`changelog.md`、`file-tree.md`。未修改 Vulkan、Resize、Present、Bridge、Picking、Gizmo、Transform Preview、场景存档、通用事件总线或第三方依赖。
- 验证结果：开发中预检 `dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 6 项目 0 warning / 0 error；本轮触碰 `.cs/.axaml/.js` 文件均未超过 100 行；全仓 5+100 扫描发现既有禁区文件 `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs` 101 行、`XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.cs` 103 行，本轮按 R2 禁区未修改 Vulkan / Swapchain，作为既有范围风险如实记录；人工验收（2026-07-14 22:37:29）正式通过：九种活动工具切换、当前工具唯一高亮、顶部与底部工具文字同步、工具与“就绪 / 运行中”语义分离、运行/停止后保持原活动工具、选择对象不污染工具、Vulkan Resize 回归和正常关闭释放链均通过；正常关窗出现 Present 泵停止、GraphicsPipeline / RenderPass + Framebuffer / VulkanRenderSession / Swapchain / LogicalDevice / Surface / Instance / Bridge 依次释放，未出现 DeviceLost、Fatal、`[ERROR] Editor failed` 或 `Exit code: -1`。
- Commit Hash：主实现提交 `ad1894e450de70ffd362912092ced16aadd7cc94`；不追记回填提交自身 Hash。
- Push 状态：主实现与 Hash 回填已推送到 `origin/fix/RZ-VK3-A-surface-contract`；本条人工验收收口记录待提交后推送；未创建 Tag / Release。
- 遗留问题：R2 正式封版；本轮准确成果边界为活动工具状态唯一所有权。尚未开发指针捕获、CaptureOwner、CapturePhase、BeginCapture、CommitCapture、CancelCapture、失焦取消、Escape 取消交互事务、真实 Gizmo 拖动、Picking、Transform Preview 或场景存档；这些进入后续 ARCH-B-R3。全仓仍有两个既有 5+100 超限文件，建议下一轮 `v0.2.16.7-fix` 先做 Vulkan 既有超限文件纯结构拆分。

## v0.2.16.5-fix
ARCH-B-R1-R3：跨树选择同步与 Inspector 刷新收口（2026-07-14 21:15:02，修复）

- 原历史编号：ARCH-B-R1-R3
- 日期：2026-07-14 21:15:02
- 任务目标：修复层级树视觉选中后未同步到正式选择和 Inspector 的阻断问题，确保项目树与层级树共享同一个 `EditorStateOwner` 正式选择状态。
- 主要改动：左侧项目树与层级树 `SelectedItem` 绑定显式设为 `Mode=TwoWay`，确保视觉选中回写 `UiVm` 并提交 Owner；Escape 清空时同时清空两棵树的 `SelectedItem`，触发既有 Clear 路径，让 Inspector 回到无选择状态；主窗口标题与 `run.bat` 同步到 `v0.2.16.5-fix`。
- 修改范围：`XuanYu.Editor.UI/Left/Left.axaml`、`XuanYu.Editor.UI/Left/Left.axaml.cs`、`XuanYu.Editor.UI/Win/UiWin.axaml`、`run.bat`、`changelog.md`、`file-tree.md`。未修改树视觉、Owner 架构、Vulkan、Resize、Present、Bridge、Picking、Gizmo、场景存档或第三方依赖。
- 验证结果：`scripts/arch-a-guard.ps1` 通过；`git diff --check` 通过（仅 LF/CRLF 工作区提示）；5+100 扫描无超限；首次构建因上一轮启动的 `XuanYu.Editor.App` 仍占用输出 DLL 失败，关闭该进程后 `dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 6 项目 0 warning / 0 error；人工验收通过：项目树“构建”、层级树“地面/主相机”、项目树“MainWorld”均能通过同一个 `EditorStateOwner` 更新 Inspector；Escape 清空正式选择、两棵树 UI 选择和 Inspector 空状态；正常关窗释放链完整（Present 泵停止、Pipeline / Framebuffer / Swapchain / LogicalDevice / Surface / Instance / Bridge 依次释放），未出现 DeviceLost、Fatal、`[ERROR] Editor failed` 或 `Exit code: -1`。
- Commit Hash：以 Git 记录和本轮交付报告为准。
- Push 状态：未执行；未创建 Tag / Release。
- 遗留问题：ARCH-B-R1 已正式验收通过；后续进入 `v0.2.16.6-rz — ARCH-B-R2`，聚焦活动工具状态与工具捕获状态的唯一所有权。

## v0.2.16.4-fix
ARCH-B-R1-R2：项目树 / 层级树视觉恢复与 Inspector 元数据修正（2026-07-14 20:47:57，修复）

- 原历史编号：ARCH-B-R1-R2
- 日期：2026-07-14 20:47:57
- 任务目标：保留 ARCH-B-R1 的 `EditorStateOwner` / Snapshot / Command 链路，恢复左侧项目树和层级树的树形视觉表达，统一左右字体层级，并修正 Inspector 类型与路径元数据。
- 主要改动：新增 UI 专用 `EditorTreeNode` 与 `UiVm.Selection.cs`；项目树 / 层级树改为带层级缩进、节点图标和稳定 Key 的树节点列表；选择命令携带 Key / 标题 / 类型 / 路径；Snapshot 增加 `SelectionPath`；Inspector 路径改为绑定真实选择元数据；左侧页签与树项字号收敛到右侧体系；主窗口标题和 `run.bat` 同步到 `v0.2.16.4-fix`。
- 修改范围：`XuanYu.Editor.UI/EditorState/*`、`XuanYu.Editor.UI/Vm/EditorTreeNode.cs`、`XuanYu.Editor.UI/Vm/UiVm.Selection.cs`、`XuanYu.Editor.UI/Vm/UiText.cs`、`XuanYu.Editor.UI/Vm/UiVm.cs`、`XuanYu.Editor.UI/Left/Left.axaml`、`XuanYu.Editor.UI/Right/Right.axaml`、`XuanYu.Editor.UI/Win/UiWin.axaml`、`run.bat`、`changelog.md`、`file-tree.md`。未修改 Vulkan、Resize、Present、Bridge、Picking、Gizmo、场景存档或第三方依赖。
- 验证结果：首次构建预检因上一轮启动的 `XuanYu.Editor.App` 仍占用输出 DLL 失败；关闭该进程后重跑通过：`scripts/arch-a-guard.ps1` 通过；`git diff --check` 通过（仅 LF/CRLF 工作区提示）；5+100 扫描无超限；`dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 6 项目 0 warning / 0 error。
- Commit Hash：以 Git 记录和本轮交付报告为准。
- Push 状态：未执行；未创建 Tag / Release。
- 遗留问题：仍需真机人工确认左侧树视觉层级、Inspector 元数据、Escape 清空选择、日志栏/工具按钮和正常关闭释放链。

## v0.2.16.3-fix
ARCH-B-R1-R1：选择状态幂等性与清空选择真实入口（2026-07-14 20:26:21，修复）

- 原历史编号：ARCH-B-R1-R1
- 日期：2026-07-14 20:26:21
- 任务目标：补齐 ARCH-B-R1 收口缺口，让重复选择和重复清空成为 NoChange，并让现有左侧选择控件具备真实清空选择路径。
- 主要改动：`EditorSelectionSnapshot` 增加稳定 `SelectionKey`；`EditorStateOwner.Select/Clear` 在状态未变化时返回 `null`，不递增 Revision、不发布伪变化；`UiVm` 将项目/层级选择缓存接入同一个 Owner，null 选择进入 `ClearEditorSelectionCommand`；`Left.axaml` 改用现有 `ProjectItems` / `HierarchyItems` 的 `ListBox` 绑定，Escape 清空当前列表选择；主窗口标题与 `run.bat` 同步到 `v0.2.16.3-fix`。
- 修改范围：`XuanYu.Editor.UI/EditorState/*`、`XuanYu.Editor.UI/Vm/UiVm.cs`、`XuanYu.Editor.UI/Left/Left.axaml`、`XuanYu.Editor.UI/Left/Left.axaml.cs`、`XuanYu.Editor.UI/Win/UiWin.axaml`、`run.bat`、`changelog.md`、`file-tree.md`。未修改 Vulkan、Resize、Present、Bridge、Picking、Gizmo、场景存档或第三方依赖。
- 验证结果：待本轮最终验证回填；已完成一次构建预检：`dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 6 项目 0 warning / 0 error。
- Commit Hash：以 Git 记录和本轮交付报告为准。
- Push 状态：待本轮裁定；未创建 Tag / Release。
- 遗留问题：仍需真机交互验收选择 A / 重复选择 A / 选择 B / Escape 清空 / 重复清空 / 日志栏与工具按钮 / 正常关闭释放链。

## v0.2.16.2-rz
ARCH-B-R1：最小 Editor State Owner 与只读快照边界（2026-07-14 19:35:40，实施）

- 原历史编号：ARCH-B-R1
- 日期：2026-07-14 19:35:40
- 任务目标：按 ARCH-B-Plan 建立第一个真实编辑器状态闭环，让当前选择状态由唯一 `EditorStateOwner` 持有，UI / ViewModel 只提交明确命令并从不可变快照读取显示结果。
- 主要改动：新增 `EditorStateOwner`、`EditorSelectionSnapshot`、`SelectEditorItemCommand` / `ClearEditorSelectionCommand`、`EditorStateChangedResult`；`UiVm` 移除选择显示字段的正式所有权，项目树 / 层级树选择 setter 仅转发为具体选择命令，检查器标题、类型和空选择状态从 Owner 快照派生；Owner 写入由 UI 线程门禁校验，非 UI 线程写入快速失败。
- 修改范围：`XuanYu.Editor.UI/EditorState/*` 与 `XuanYu.Editor.UI/Vm/UiVm.cs`；同步 `changelog.md`、`file-tree.md`。未修改 Vulkan、Resize、Present、Bridge 生命周期、渲染请求系统、Inspector 布局、真实 Picking、Gizmo、存档格式或第三方依赖。
- 验证结果：`scripts/arch-a-guard.ps1` 通过；`git diff --check` 通过（仅 `UiVm.cs` LF/CRLF 工作区提示，无空白错误）；本轮触碰 `.cs` 文件均不超过 100 行；`dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 6 项目 0 warning / 0 error。
- Commit Hash：主实现提交 `a1bfe8a716c99e0a8d1c624972f51d6b158a3295`；本条 Hash 回填随 ARCH-B-R1-R1 文档修复提交进入历史。
- Push 状态：按本轮用户裁定暂不执行 Push / Tag / Release。
- 遗留问题：当前只迁移选择状态；当前工具、工具捕获、交互事务和视口请求状态仍在后续 ARCH-B-R2/R3/R4 收口；未做真实交互启动验收。

## v0.2.16.2-rz
DOC-GIT-PUSH-1：开发提交必须 Push 到 GitHub 远端工作分支（2026-07-13 23:41:18，规范修订）

- 原历史编号：DOC-GIT-PUSH-1
- 日期：2026-07-13 23:41:18
- 任务目标：将“每轮本地提交后必须 Push 到 GitHub 远端当前工作分支”写入开发宪法，满足多电脑开发的远端备份需求，并同步当前可见版本号。
- 主要改动：`docs/玄域引擎_AI开发宪法.md` 默认流程新增 Push 步骤；Git Push 章节改为验证通过并 Commit 后必须 Push 当前工作分支，同时保留 main 合并、PR、Tag、Release、强推、Rebase、重写历史必须明确确认的红线；主窗口标题和 `run.bat` 标题更新为 `v0.2.16.2-rz`。
- 影响范围：仅开发规范、版本标题和同步文档；不修改 UiVm、状态所有权实现、NativeHost、Bridge、Vulkan、Resize、Present 或项目依赖结构。
- 验证结果：`scripts/arch-a-guard.ps1` 通过；`git diff --check` 通过（仅 Git LF/CRLF 工作区提示，无空白错误）；5+100 扫描无 `.cs/.axaml/.js/.ps1` 超过 100 行；`dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 6 项目 0 warning / 0 error。
- Commit Hash：主提交 `b32e760a2fedc6cad2de919edb9c51c89bc8298e`；哈希回填修正以 Git 记录和交付报告为准。
- Push 状态：已推送到 `origin/fix/RZ-VK3-A-surface-contract`；首次推送区间 `c823312..f9c6953`；Push 状态回填提交以 Git 记录和交付报告为准。
- 遗留问题：后续 ARCH-B-R1 版本号需顺延到下一个有效开发版本。

## v0.2.16.1-rz
ARCH-B-Plan：编辑器状态所有权与交互事务边界审计（2026-07-13 23:33:18，规划文档）

- 原历史编号：ARCH-B-Plan
- 日期：2026-07-13 23:33:18
- 任务目标：在 ARCH-A 总收口后进入 ARCH-B，审计当前 ViewModel、Viewport、层级树、Inspector、工具、日志和 NativeHost 状态写入路径，并规划最小状态所有权与 Preview / Commit / Cancel 事务边界。
- 主要改动：新增 `docs/arch-b-plan.md`，记录当前状态所有权清单、写入路径、风险判断、ARCH-B 禁区、R1-R4 分轮方案和中文 SVG 状态流图；主窗口标题与 `run.bat` 标题同步推进为 `v0.2.16.1-rz`。
- 影响范围：仅规划文档、版本标题和同步索引；不修改 UiVm 运行行为、NativeHost、Bridge、Swapchain、Resize、Present、Vulkan 生命周期或项目依赖结构。
- 验证结果：`scripts/arch-a-guard.ps1` 通过；`git diff --check` 通过（仅出现 Git LF/CRLF 工作区提示，无空白错误）；5+100 扫描无 `.cs/.axaml/.js/.ps1` 超过 100 行；`dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 6 项目 0 warning / 0 error。
- Commit Hash：主提交 `cab420a02d25e403802eb8460605163c29b10613`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：ARCH-B-R1 仍需在实现前决定最小 Editor State Owner 放置位置；本轮不创建新项目、不引入状态框架、不实现 Picking / Gizmo / 场景存档。

## v0.2.15.8-fix
ARCH-A-R4-R2：版本格式守卫与 ARCH-A 最终封版修正（2026-07-13 23:19:16，修正）

- 原历史编号：ARCH-A-R4-R2
- 日期：2026-07-13 23:19:16
- 任务目标：修正无效开发期版本号 `v0.2.15.7-r1-rz`，将“R1/R2”保留在任务编号而非版本号中，并让 ARCH-A 守卫同时检查版本一致性与版本格式合法性。
- 主要改动：主窗口标题和 `run.bat` 标题更新为 `v0.2.15.8-fix`；`scripts/arch-a-guard.ps1` 增加开发期版本格式校验，当前允许 `rz` / `fix` / `vk` 类型；本条 changelog 作为 ARCH-A 最终封版修正条目。
- 影响范围：仅版本字符串、版本格式守卫和同步文档；不修改 Swapchain、Resize、Present、Vulkan 生命周期、依赖结构或 `Editor.Win` 项目结构。
- 验证结果：`scripts/arch-a-guard.ps1` 通过，已同时校验版本一致性与版本格式合法性；`git diff --check` 通过；5+100 扫描无 `.cs/.axaml/.js/.ps1` 超过 100 行；放开网络后 `dotnet restore XuanYu.Engine.slnx` 通过，随后普通权限 `dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 6 项目 0 warning / 0 error。当前 Codex 普通沙箱仍会阻止访问 `api.nuget.org:443`，属于网络权限限制而非仓库缺陷。
- Commit Hash：主提交 `b3d5d7941aeea0c3e740705ea025d9213b3097f7`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：版本号仍分散在标题、run.bat 和 changelog 中；后续可考虑单一版本来源。

## v0.2.15.7-r1-rz
ARCH-A-R4-R1：唯一启动入口守卫与普通权限构建收口（2026-07-13 23:04:38，修正；版本号格式已由 v0.2.15.8-fix 接续修正）

- 原历史编号：ARCH-A-R4-R1
- 日期：2026-07-13 23:04:38
- 任务目标：补齐 ARCH-A-R4 守卫缺口，确保只有 `XuanYu.Editor.App` 是可执行入口，并按普通权限清理缓存后重新验证完整构建；同步标题版本号。
- 主要改动：`XuanYu.Editor.Win` 移除 `OutputType=WinExe` 并删除旧 `Program.cs` 启动入口，降为非独立启动项目；`scripts/arch-a-guard.ps1` 增加 `OutputType` 检查，强制只有 `Editor.App` 可为 `WinExe/Exe`；主窗口标题和 `run.bat` 标题更新为 `v0.2.15.7-r1-rz`。
- 影响范围：仅启动入口守卫、旧 WinForms 壳输出类型、标题版本号和同步文档；不修改 Swapchain、Resize、Present、Vulkan 释放链或 App 注入逻辑。
- 验证结果：`scripts/arch-a-guard.ps1` 通过，已覆盖唯一可执行入口；5+100 扫描通过；`git diff --check` 通过；`XuanYu.Editor.Win` 普通权限 `dotnet build --no-restore` 0 warning / 0 error；清理 `XuanYu.Editor.UI/bin,obj` 后修复用户级 `NuGet.Config` 对 `CodexSandboxUsers` 的读取权限，`dotnet restore XuanYu.Engine.slnx` 不再出现 `Access denied`，但当前 Codex 普通沙箱仍因 socket 权限阻止访问 `api.nuget.org:443`；放开网络后 restore 通过，随后普通权限 `dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 6 项目 0 warning / 0 error。
- Commit Hash：主提交 `ab4fecdd50da6ef9deb0a315193abb424284122e`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：版本号仍分散在标题、run.bat 和 changelog 中；后续可考虑单一版本来源，但本轮仅用守卫防漂移。

## v0.2.15.7-rz
ARCH-A-R4：架构守卫、标题版本号与 ARCH-A 总收口（2026-07-13 22:53:06，实施）

- 原历史编号：ARCH-A-R4
- 日期：2026-07-13 22:53:06
- 任务目标：在 ARCH-A-R3 真机关闭与交互回归验收通过后，将 UI 禁止依赖 Vulkan/Silk、App 唯一组装入口、run.bat 启动 App、解决方案六项目、5+100 与窗口标题版本号等边界固化为可重复执行的自动守卫，并补充开发宪法。
- 主要改动：新增 `scripts/arch-a-guard.ps1`；主窗口标题更新为 `玄域引擎编辑器 v0.2.15.7-rz`；`run.bat` 控制台标题同步版本号；开发宪法新增窗口标题版本号规则，并强化 `changelog.md` 日期必须精确到秒、不得只写日期或分钟。
- 影响范围：仅架构守卫脚本、窗口标题、启动脚本标题与同步文档；不修改 Swapchain、Resize、Present、渲染资源释放或 Vulkan 主链。
- 验证结果：`scripts/arch-a-guard.ps1` 通过；`git diff --check` 通过；5+100 扫描无 `.cs/.axaml/.js/.ps1` 超过 100 行；`dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 在提升权限下 0 warning / 0 error。普通权限构建仍会被 Avalonia UI `obj` 资源缓存写入权限阻断，属于本机权限环境问题。
- Commit Hash：主提交 `b72993ba5fcfb33b81183c1693b50cb208376154`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：R4 为守卫与总收口轮，不处理启动 16x16 Swapchain 后自愈到真实尺寸的后续优化项。

## v0.2.15.6-rz
ARCH-A-R3：移除 Editor.UI 对 Vulkan / Silk 的旧直接依赖与 fallback 链路（2026-07-13 22:38:14，实施）

- 原历史编号：ARCH-A-R3
- 日期：2026-07-13 22:38:14
- 任务目标：在 ARCH-A-R2 真机 Resize / 代际一致性验收通过后，移除 `XuanYu.Editor.UI` 对 `XuanYu.Render.Vulkan` / `Silk.NET.Vulkan` 的旧直接依赖，删除旧 fallback 和历史 Vulkan 探针 / ClearSession 死链，让 UI 只依赖 `XuanYu.Render.Abstractions`。
- 主要改动：`XuanYu.Editor.UI.csproj` 移除 `Silk.NET.Vulkan`、`Silk.NET.Vulkan.Extensions.KHR` 和 `XuanYu.Render.Vulkan` 引用；`VulkanNativeHost.CreateBridge` 在缺少应用注入 factory 时明确拒绝旧 fallback；删除 `VulkanSurfaceBridgeProvider`、`VulkanProbeRoute`、`UiVm.VulkanProbe` 和 4 个旧 `VulkanClearSession.*` 文件；UI 启动不再运行 VulkanProbe；视口 fallback 文案改为后端中性表达。
- 影响范围：仅 `XuanYu.Editor.UI` 依赖边界、旧死链删除与同步文档；不修改 `XuanYu.Editor.App` 组装根，不修改 `XuanYu.Render.Vulkan` 的 Resize / Swapchain / Present 主逻辑，不删除 R2 代际探针。
- 验证结果：`Editor.UI` 内对 `XuanYu.Render.Vulkan` / `Silk.NET.Vulkan` / `VulkanSurfaceBridgeProvider` / `VulkanApiProbe` / `VulkanClearSession` 的扫描为 0 命中；5+100 扫描无 `.cs/.axaml/.js` 超过 100 行；`dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 在提升权限下 0 warning / 0 error；受控启动 `XuanYu.Editor.App` 10 秒显示桥接工厂来源为“应用注入（XuanYu.Editor.App）”，未触发旧 fallback，Instance / Surface / Swapchain / Present 正常启动。
- Commit Hash：主提交 `4bf4c67827c51c714d783950225e35ad66e01af8`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：受控启动使用 `Stop-Process` 结束，不构成手动关闭释放顺序验收；仍建议用户在真机手动关闭一次，确认 Present 停止、Pipeline / Framebuffer / Swapchain / Device / Surface / Instance 释放顺序完整。

## v0.2.15.5-fix
ARCH-A-R2-R2：Swapchain 代际依赖与重复 Resize 修复（2026-07-13 22:24:18，修复）

- 原历史编号：ARCH-A-R2-R2
- 日期：2026-07-13 22:24:18
- 任务目标：修复日志栏 Resize 后可重复出现的 `QueueSubmit ErrorDeviceLost`，根因是 Swapchain 实际重建并生成新 ImageView 后，仅因 extent 相同而错误跳过 Framebuffer 重建与 CommandBuffer 重录。
- 主要改动：`VulkanSwapchainOwner` 新增资源代际 `ResourceGeneration`，仅在 Swapchain / ImageView 实际换代后推进；`VulkanClearFrameOwner.RebuildFramebuffers` 增加 `force` 参数，Swapchain 换代时即使 extent 相同也强制销毁旧 FB、创建新 FB 并重录 CB；`VulkanRenderSession.Resize` 改为查询当前 Surface extent，若 Present 自愈已完成目标尺寸则在 Recreate 前快速跳过；Present 自愈与 UI 合并 Resize 均输出低频中文代际日志。
- 影响范围：仅 `XuanYu.Render.Vulkan` 的 Swapchain / RenderSession / ClearFrame 代际一致性路径，以及 `changelog.md`、`file-tree.md`；不修改 ARCH-A 注入链，不删除 fallback，不进入 ARCH-A-R3。
- 验证结果：`git diff --check` 通过；5+100 扫描无 `.cs/.axaml/.js` 超过 100 行；`dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 在释放本项目 App 文件锁后 0 warning / 0 error。普通权限构建曾被残留 `XuanYu.Editor.App (29216)` 锁定输出 DLL 阻断，停止该进程后通过。
- Commit Hash：主提交 `7822a1761200f9d294687b414cfd68ddf4d8f86c`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：仍需用户按 175% DPI 真机验收清单复验：展开/收起日志栏 10 次、拖动日志栏分隔线至少 5 秒、改变主窗口尺寸、最小化恢复、确认不再出现 `ErrorDeviceLost`，且 Swapchain / Framebuffer / CommandBuffer 代际不落后。

## v0.2.15.4-r2-fix
ARCH-A-R2-R2：日志栏布局同步后的 Swapchain 高度滞后修复（2026-07-13 22:05:10，修复）

- 原历史编号：ARCH-A-R2-R2
- 日期：2026-07-13 22:05:10
- 任务目标：修复日志详情栏展开/收起后，Win32 子窗口已经同步到正确物理尺寸，但 Vulkan 交换链/Framebuffer 仍可能沿用旧高度，导致视口下半部分黑屏的问题。
- 主要改动：`VulkanNativeHost.LayoutSync.SyncFinalSize` 不再在 `Win32ViewportHost.Resize` 后立即调用 `_bridge.Resize`；改为先完成 HWND 物理尺寸同步和探针日志，再把同一逻辑尺寸交回现有 `NativeHostResizeCoalescer` 延后合并触发，让 Surface CurrentExtent 有机会稳定到新的物理尺寸。
- 影响范围：仅 `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.LayoutSync.cs`、`changelog.md`、`file-tree.md`；不修改 Vulkan Attach/Present/Swapchain 主链，不删除 fallback，不进入 ARCH-A-R3。
- 验证结果：`git diff --check` 通过；5+100 扫描无 `.cs/.axaml/.js` 超过 100 行；`dotnet build XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false` 在提升权限下 0 warning / 0 error；受控启动 `XuanYu.Editor.App` 12 秒日志显示桥接工厂来源为“应用注入（XuanYu.Editor.App）”、未触发旧 fallback，Surface CurrentExtent 与 Swapchain/Framebuffer 自愈并稳定到 `1248x1110`。
- Commit Hash：主提交 `2e03a58dd303fa78fa554975b864fc7005f43d67`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：仍需用户真机复验展开/收起日志详情栏后的画面完整性，确认蓝灰背景覆盖完整 Vulkan 视口、黄色三角形正常、底部黑屏不再出现。

## v0.2.15.4-r1-fix
ARCH-A-R2-R1：run.bat 批处理编码修复（2026-07-13 21:51:44，修复）

- 原历史编号：ARCH-A-R2-R1-bat
- 日期：2026-07-13 21:51:44
- 任务目标：修复 `run.bat` 在 Windows `cmd.exe` 下因 UTF-8 中文正文 / 换行解析导致的半截命令、乱码命令和错误启动问题。
- 主要改动：将 `run.bat` 脚本正文改为 ASCII-only 命令与提示，保留仓库根切换、`XuanYu.Editor.App` 唯一启动入口、NuGet.Config restore、build、run、退出码透传和失败 pause 逻辑。
- 影响范围：仅 `run.bat` 与 `changelog.md`；不修改 App / UI / Vulkan 代码，不改变 DPI 修复逻辑。
- 验证结果：`cmd /c type run.bat` 输出脚本结构正常，无乱码命令；`git diff --check` 通过；`dotnet build XuanYu.Editor.App\\XuanYu.Editor.App.csproj --no-restore` 0 warning / 0 error。未完整执行 `run.bat`，因为它会启动窗口并等待用户关闭。
- Commit Hash：主提交 `9b514c4d6e8e3357ff10502f9b71f074dc32c3f5`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：仍需用户双击或命令行运行 `run.bat` 做真实启动确认。

## v0.2.15.4-fix
ARCH-A-R2-R1：新启动入口 DPI / 物理像素一致性修复（2026-07-13 21:42:14，修复）

- 原历史编号：ARCH-A-R2-R1
- 日期：2026-07-13 21:42:14
- 任务目标：修复 `XuanYu.Editor.App` 新启动入口下 175% DPI 环境中 Vulkan 只绘制左上角的问题，并迁移 `run.bat` 到唯一 App 入口。
- 主要改动：`app.manifest` 补 PerMonitorV2 DPI awareness 并将 manifest 身份改为 `XuanYu.Editor.App.app`；`VulkanNativeHost.OnSizeChanged` 与日志栏同步路径统一使用 `逻辑尺寸 × DPI` 后的物理像素调整 Win32 子窗口；新增 `VulkanNativeHost.Dpi.cs` 承载物理尺寸换算；`run.bat` 改为还原/构建/启动 `XuanYu.Editor.App`，切换到仓库根、UTF-8 输出并透传退出码。
- 影响范围：仅 App 启动配置、Native HWND 物理尺寸、run.bat 与同步文档；不修改 Vulkan Attach/Resize/Present/Fatal/Detach 行为，不修改 RenderArea/Viewport/Scissor，不进入 ARCH-A-R3。
- 验证结果：`dotnet build XuanYu.Engine.slnx --no-restore` 0 warning / 0 error；`git diff --check` 通过；5+100 扫描无超 100 行 `.cs/.axaml/.js`；受控运行 `XuanYu.Editor.App` 12 秒后自动停止，日志显示应用注入路径生效、未触发 fallback、Instance / Surface / Swapchain / Present 启动成功，Swapchain / Framebuffer 重建到 `1248x1110` 物理 extent；无残留 `XuanYu.Editor.App` 进程。因本次为定时停止，蓝灰背景是否覆盖完整视口、日志栏拖动和关闭释放顺序仍需用户真机确认。
- Commit Hash：主提交 `3a403d4cc768bf582e8054aa076f66d736985cb7`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：仍需用户在 175% 缩放率下真机确认蓝灰背景覆盖完整 Vulkan 视口、黄色三角形正常、展开/收起日志栏与窗口 Resize 后无黑边。

## v0.2.15.3-r3-rz
DOC-CONSTITUTION-2：SVG 中文化规则补充（2026-07-13 21:37:28，纯文档）

- 原历史编号：DOC-CONSTITUTION-2
- 日期：2026-07-13 21:37:28
- 任务目标：补充开发宪法中的可视化输出规范，要求后续收口报告 SVG 尽量使用中文表达。
- 主要改动：在 `docs/玄域引擎_AI开发宪法.md` 第十八章“可视化与人工验收”中新增规则：SVG 内标题、节点、说明和标注应尽量使用中文；仅代码标识、项目名、文件名、协议名等确需精确引用时保留英文。
- 影响范围：仅 `docs/玄域引擎_AI开发宪法.md` 与 `changelog.md`；不修改代码、不修改构建配置、不进入 ARCH-A-R2-R1。
- 验证结果：待验证后补齐。
- Commit Hash：主提交 `e56c729bf989da28d8f5fe2325a9ef80c0dd0615`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：历史报告中的英文 SVG 不在本轮批量改写；后续新报告按本规则执行。

## v0.2.15.3-r2-rz
ARCH-A-R2-Verify：桥接工厂来源日志与解决方案入口说明（2026-07-13 20:57:24，验收补充）

- 原历史编号：ARCH-A-R2-Verify
- 日期：2026-07-13 20:57:24
- 任务目标：补足 R2 真机验收前的可观测性，明确区分 `XuanYu.Editor.App` 注入路径与旧 fallback 路径，并说明当前唯一解决方案入口。
- 主要改动：`VulkanNativeHost.CreateBridge` 在创建 Bridge 前输出一条低频中文日志，分别标明“桥接工厂来源：应用注入（XuanYu.Editor.App）”或“桥接工厂来源：旧兼容回退（VulkanSurfaceBridgeProvider）”；`file-tree.md` 明确当前分支只有 `XuanYu.Engine.slnx`，没有 `XuanYu.Engine.sln`。
- 影响范围：仅 UI 低频生命周期日志与同步文档；不修改 Vulkan Attach/Resize/Present/Fatal/Detach 行为，不删除 fallback，不进入 R3。
- 验证结果：`dotnet build XuanYu.Engine.slnx --no-restore` 0 warning / 0 error；`git diff --check` 通过；5+100 扫描无超 100 行 `.cs/.axaml/.js`；`Render.Abstractions` 对 `Silk.NET.Vulkan` / `XuanYu.Render.Vulkan` 的命中仅为历史说明注释，无实际 using / PackageReference / ProjectReference；受控运行 `XuanYu.Editor.App` 12 秒并自动停止，stdout 首行证明桥接工厂来源为“应用注入（XuanYu.Editor.App）”，未出现“旧兼容回退”文本，且日志显示 Instance / Surface / Swapchain / Present 已启动。因本次为定时停止，不判定关闭释放顺序和人工视觉验收通过。
- Commit Hash：主提交 `5414dc7839826da0051390541e66bdaa66b972be`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：仍需从 `XuanYu.Editor.App` 真机启动并确认日志出现“应用注入”且不出现“旧兼容回退”；R3 在真机验收通过前不得开始。

## v0.2.15.3-rz
ARCH-A-R2：建立 Avalonia 应用组装层（2026-07-13 20:44:15，实装）

- 原历史编号：ARCH-A-R2
- 日期：2026-07-13 20:44:15
- 任务目标：新增 `XuanYu.Editor.App` 作为 Avalonia 启动与依赖组装入口，由 App 层创建 `VulkanNativeHostSurfaceBridgeFactory` 并以 `INativeHostSurfaceBridgeFactory` 抽象身份传入 UI；防止双启动入口、双 Bridge、双渲染线程。
- 主要改动：新增 `XuanYu.Editor.App` 项目与 `EditorCompositionRoot`；`AppBuilder` 通过工厂函数创建带抽象 factory 的 UI `App`；`UiVm` 接收并保存 `INativeHostSurfaceBridgeFactory`；`VulkanNativeHost` 优先使用注入 factory 创建 Bridge，旧 `VulkanSurfaceBridgeProvider` 仅保留为兼容 fallback；`XuanYu.Editor.UI` 改为类库；新增 `XuanYu.Engine.slnx` 纳入 6 个项目。
- 影响范围：`XuanYu.Editor.App`、`XuanYu.Editor.UI` 启动/组装入口、`changelog.md`、`file-tree.md` 与解决方案文件；不修改 Vulkan Attach/Resize/Present/Fatal/Detach 行为，不删除 UI 旧 Vulkan 链路，不调整 UI 布局。
- 验证结果：`dotnet restore XuanYu.Engine.slnx` 通过；Core、Render.Abstractions、Render.Vulkan、Editor.UI、Editor.Win、Editor.App 六个项目分别 `dotnet build --no-restore` 全部 0 warning / 0 error；`dotnet build XuanYu.Engine.slnx --no-restore` 0 warning / 0 error；`git diff --check` 通过；5+100 扫描无超 100 行 `.cs/.axaml/.js`；`Render.Abstractions` 对 `Silk.NET.Vulkan` / `XuanYu.Render.Vulkan` 的命中仅为历史说明注释，无实际 using / PackageReference / ProjectReference。真机启动与渲染验收待用户执行。
- Commit Hash：主提交 `ded3af5c64bb32ef88a9f1fe526a014b8dc3efdd`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：`Editor.UI` 项目引用层面仍保留 `Render.Vulkan` / `Silk.NET.Vulkan` 旧依赖，旧 `VulkanProbeRoute`、`UiVm.VulkanProbe`、`VulkanClearSession.*` 仍待 R3+ 独立处理；真机启动、Resize、关闭释放需用户验收。

## v0.2.15.2-rz
ARCH-A-R1：最小渲染生命周期契约与 Vulkan 工厂适配（2026-07-13 20:30:44，实装）

- 原历史编号：ARCH-A-R1
- 日期：2026-07-13 20:30:44
- 任务目标：在 `XuanYu.Render.Abstractions` 建立现有 NativeHost 渲染生命周期所需的最小契约，并让 Vulkan 实现开始适配；不删除 `Editor.UI` 旧 Vulkan 调用链，不新增 `Editor.App`。
- 主要改动：`INativeHostSurfaceBridge` 继承 `IDisposable`，把释放纳入抽象生命周期契约；新增 `INativeHostSurfaceBridgeFactory`；新增 `VulkanNativeHostSurfaceBridgeFactory` 返回 `VulkanNativeHostSurfaceBridge`。
- 影响范围：仅 `XuanYu.Render.Abstractions`、`XuanYu.Render.Vulkan` 与同步文档；未修改 `XuanYu.Editor.UI`，未移除任何旧 Vulkan/Silk 引用，未改变 Attach/Resize/Detach 行为。
- 验证结果：5 个项目按顺序 `dotnet build --no-restore` 全部 0 warning / 0 error；`git diff --check` 通过；5+100 扫描无超 100 行 `.cs/.axaml/.js`；`Render.Abstractions` 对 `Silk.NET.Vulkan` / `XuanYu.Render.Vulkan` 的命中仅为历史说明注释，无实际 using / PackageReference / ProjectReference；`Editor.UI` 旧链路未改动。
- Commit Hash：主提交 `a8c9672729e49a113924f9b128856139e3f25c12`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：UI 侧仍由 `VulkanSurfaceBridgeProvider` 直接装配 Vulkan 实现；`XuanYu.Editor.UI.csproj` 仍直接引用 `Render.Vulkan` / `Silk.NET.Vulkan`；旧 `VulkanClearSession.*` 仍待后续独立轮次清理。

## v0.2.15.1-rz
ARCH-A-Plan：Editor.UI Vulkan 直接依赖边界审计与迁移计划（2026-07-13 20:27:01，规划文档）

- 原历史编号：ARCH-A-Plan
- 日期：2026-07-13 20:27:01
- 任务目标：确认 `Editor.UI` 直接依赖 `Render.Vulkan` / `Silk.NET.Vulkan` 的真实边界，并规划后续 ARCH-A-R1 的最小契约迁移顺序。
- 主要改动：新增 `docs/arch-a-plan.md`，记录活跃直接依赖文件、历史旧探针依赖、R1 允许/禁止范围与架构 SVG；同步 `file-tree.md`。
- 影响范围：仅 `docs/arch-a-plan.md`、`changelog.md`、`file-tree.md`；不修改代码、不修改项目引用、不改变运行逻辑。
- 验证结果：纯文档计划；已确认 `Editor.UI` 活跃直接依赖清单包含 `XuanYu.Editor.UI.csproj`、`VulkanSurfaceBridgeProvider.cs`、`UiVm.VulkanProbe.cs`、`VulkanProbeRoute.cs`；`Render.Abstractions` 无实际 Silk.NET / Vulkan 依赖。
- Commit Hash：主提交 `db041ae9e845d2a810b73adfb09214b4cafdcf50`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：正式依赖迁移留给 `v0.2.15.2-rz` 起的独立轮次；本轮不删除旧 `VulkanClearSession.*`，不新增 `Editor.App`。

## v0.2.14.12-rz
DOC-CONSTITUTION-1：changelog 时间精度规则补充（2026-07-13 20:24:59，纯文档）

- 原历史编号：DOC-CONSTITUTION-1
- 日期：2026-07-13 20:24:59
- 任务目标：补充最高开发宪法中的 `changelog.md` 时间记录规则，要求后续日志时间记录到提交发生时的分秒级时间。
- 主要改动：在 `docs/玄域引擎_AI开发宪法.md` 第十七章 `changelog.md` 字段清单中，将“日期”明确为本轮提交发生时的本地时间，并规定格式为 `YYYY-MM-DD HH:mm:ss`，精确到秒。
- 影响范围：仅 `docs/玄域引擎_AI开发宪法.md` 与 `changelog.md`；不修改代码、不修改构建配置、不改变既有版本编号规则。
- 验证结果：纯文档修改；已确认规则位置在最高规范的文档同步章节，`file-tree.md` 无需更新，因为未新增、删除、移动、重命名文件，也未改变文件职责或依赖边界。
- Commit Hash：主提交 `080e05e572683a2236d0df116d18d706fc3d6ea7`；哈希回填修正以 Git 记录和交付报告为准。
- 遗留问题：历史 changelog 条目仍保留原有日期精度，不在本轮批量改写；后续新条目按本规则执行。

## v0.2.14.11-rz
VK-LIFE-1-R2：Present Fatal 状态跨线程发布收口（2026-07-13，修复）

- 原历史编号：VK-LIFE-1-R2
- 日期：2026-07-13
- 任务目标：在 VK-LIFE-1-R1 正常路径真机通过后，只补 Present Fatal 状态跨线程发布契约；不再修改 Resize、Swapchain、Framebuffer 正常路径。
- 主要改动：`VulkanRenderSession` 将 `_failed` 改为 `int` 发布位；`MarkFailed` 通过 `Interlocked.CompareExchange` 原子抢占首个失败原因，成功后再 `Volatile.Write` 发布失败状态，并保证 `PresentFatal` 日志只输出一次；`IsFailed` 与 `FailureReason` 使用 `Volatile.Read`。
- 影响范围：仅 `XuanYu.Render.Vulkan/Session` 失败状态发布相关代码及两份同步文档；不修改 `.axaml/.csproj`，不触碰已验收通过的 Resize 自愈路径。
- 验证结果：5 个当前分支项目按顺序 `dotnet build --no-restore` 全部 0 warning / 0 error；5+100 全量检查无超 100 行文件；空 `catch` 扫描 0 命中；Fatal 状态扫描确认 `MarkFailed` 使用 `Interlocked.CompareExchange` 抢占首个 `_failureReason`，成功后 `Volatile.Write` 发布 `_failed`，读侧 `IsFailed` / `FailureReason` 走 `Volatile.Read`；`git diff --check` 通过。沿用 VK-LIFE-1-R1 真机日志作为正常 Resize / 自愈路径验收证据。
- Commit Hash：R2 主提交 `90128dd0f7aacd0f8ca0edfa07a10e5009fb90b5`；首因所有权补正提交以 Git 记录和交付报告为准。
- 遗留问题：PresentFatal 人为失败路径仍需后续按需构造故障注入验证；ARCH-A-PLAN/IMPL 与 VK5-E 仍在后续。

## v0.2.14.10-rz
VK-LIFE-1-R1：Resize 自愈竞态与失败状态传播补正（2026-07-13，修复）

- 原历史编号：VK-LIFE-1-R1
- 日期：2026-07-13
- 任务目标：在 VK-LIFE-1 真机部分通过的基础上补正 Resize 与 Present 自愈竞态、generation 语义、Present 致命退出状态传播和重复释放日志，不扩大到 ARCH-A / VK5-E / VulkanClearSession。
- 主要改动：`VulkanRenderSession.Resize` 改为先等待自愈锁、锁内复查尺寸并标记 Resize 接管，再锁外 Stop Present，避免自愈已完成后 UI Resize 仍无意义停泵；generation 仅在 Swapchain extent 实际变化后增加；`VulkanPresentLoop` 增加致命退出回调，Wait/Reset/Submit/Present 等失败会使 RenderSession 进入 Failed 状态；Bridge 后续 Resize 会识别 Failed Session 并拒绝按正常状态继续；Session 释放日志改为 `【VulkanRenderSession】释放完成`，ClearFrame 释放日志只由 ClearFrameOwner 输出一次。
- 影响范围：仅 `XuanYu.Render.Vulkan` 生命周期相关文件与 `changelog.md`、`file-tree.md`；未修改 `.axaml/.csproj`；未处理 Editor.UI → Render.Vulkan 依赖迁移；未删除 VulkanClearSession；未新增渲染功能。
- 验证结果：5 个当前分支项目 `dotnet build --no-restore` 全部 0 warning / 0 error；5+100 全量检查无超 100 行文件（最大 100）；空 `catch` 扫描 0 命中；`git ls-files -- 111.ps1` 仍为 0 命中；`git diff --check` 通过；状态传播扫描确认 Present 致命错误进入 `PresentFatal` / `SessionFailed` 路径；generation 递增仅保留在 Resize extent 实际变化与自愈 rebuilt 两处。仍需用户真机复验连续快速展开/收起日志栏至少 10 次。
- Commit Hash：待提交后补齐。
- 遗留问题：本轮自动验证通过后，VK-LIFE-1 是否封版取决于用户真机复验；ARCH-A-PLAN/IMPL 与 VK5-E 仍在后续。

## v0.2.14.9-rz
VK-LIFE-1：Vulkan 生命周期失败安全与仓库收尾（2026-07-13，修复）

- 原历史编号：VK-LIFE-1
- 日期：2026-07-13
- 任务目标：正常 Vulkan 成功路径不退化；失败路径可回滚；Present 线程能够可靠停止；异常不再静默吞掉；顺带删除已批准删除的 `111.ps1`。
- 主要改动：删除 tracked 的 `111.ps1`；`VulkanPresentLoop` 拆分为主循环与生命周期 partial，补 Semaphore/Fence 创建结果、WaitForFences/ResetFences/QueueSubmit/QueuePresent 等关键 Result 检查，`Stop()` 检查 `Join(2000)` 返回值，日志回调异常改为受限 Debug 兜底；`VulkanClearFrameOwner` 拆分创建/命令/生命周期 partial，补 RenderPass、CommandPool、Framebuffer、CommandBuffer 创建与录制结果检查；`VulkanRenderSession.Create` 对 ClearFrame/Pipeline/PresentLoop 做失败逆序释放，Resize 失败进入明确释放路径；`VulkanNativeHostSurfaceBridge.Attach` 改为全成功后再写字段，失败按现有释放顺序回滚，Resize 失败时不假装可用。
- 影响范围：仓库收尾 `111.ps1` 删除；Vulkan 生命周期文件限于 `XuanYu.Render.Vulkan`；同步 `changelog.md` 与 `file-tree.md`。不处理 Editor.UI → Render.Vulkan 依赖迁移，不处理 ARCH-A，不删除 VulkanClearSession，不新增渲染功能。
- 验证结果：5 个当前分支项目 `dotnet build --no-restore` 全部 0 warning / 0 error；5+100 全量检查无超 100 行的 `.cs/.axaml/.js`；空 `catch` 扫描 0 命中；`git ls-files -- 111.ps1` 0 命中；`111.ps1` 引用仅剩 changelog/file-tree 与历史审计文档说明；Vulkan 关键 Result 扫描确认 Wait/Reset/Submit/Present 与同步对象、RenderPass、CommandPool、Framebuffer、CommandBuffer、Pipeline、ShaderModule 创建均进入 `Check`/`Ok`/显式 `Result` 处理；`git diff --check` 通过。
- Commit Hash：仓库收尾与 Vulkan 生命周期修复分别以本轮实际 Git 提交为准。
- 遗留问题：需用户真机验收启动、Resize、日志栏、关闭释放顺序与人为失败路径；Editor.UI 活跃 Vulkan 依赖留给 ARCH-A-PLAN/IMPL；VulkanClearSession 清理留给后续 VK5-E。

## v0.2.14.8-rz
SAFE-1：仓库危险脚本与误提交风险收口（2026-07-13，仓库安全）

- 原历史编号：SAFE-1
- 日期：2026-07-13
- 任务目标：只处理 ORG-1 后续指定的两项仓库安全问题：tracked 的 `111.ps1` 与 untracked、未忽略的 `qizheng-mvp-fixed/`。
- 主要改动：将 `111.ps1` 安全隔离为立即报错退出的占位脚本，移除原脚本中的 `.git` 删除、`git init`、`git add -A`、提交和推送流程；在 `.gitignore` 中加入 `qizheng-mvp-fixed/`，避免误 `git add -A` 纳入独立 MVP 项目。
- 影响范围：仅 `111.ps1`、`.gitignore`、`changelog.md`、`file-tree.md`；未修改任何 `.cs/.axaml/.csproj`；未删除或迁移 `qizheng-mvp-fixed/`；未继续修改 ORG-1 审计报告。
- 验证结果：`git diff --check` 通过；`git check-ignore -v qizheng-mvp-fixed` 命中根 `.gitignore`；`111.ps1` 中 `Remove-Item`、`git init`、`git add -A`、`git commit`、`git push`、`Read-Host` 0 命中；`.cs/.axaml/.csproj` 改动 0。纯仓库安全修改，不重新构建五个项目。
- Commit Hash：本轮最终提交 Hash 以 Git 记录和交付报告为准。
- 遗留问题：`111.ps1` 当前为安全隔离状态，如需彻底删除需按删除流程另行批准；`qizheng-mvp-fixed/` 仍留在本地工作区但已被根 `.gitignore` 覆盖。

## v0.2.14.7-rz
ORG-1：项目基线审计最终文档收口（2026-07-13，纯文档）

- 原历史编号：ORG-1 收口
- 日期：2026-07-13
- 任务目标：将 ORG-1 收口为可指导后续开发的已验收基线；不重新审计项目、不增加新风险项，下一轮直接进入 SAFE-1。
- 主要改动：仅修改 `docs/project-baseline-audit-org-1-r1.md` 与 `changelog.md`。① 非构建扫描不再使用 Shell/GNU 工具自然退出码作为结论，A1 直接记录 111 个文件、0 个超限，A5 直接记录 tracked 源码范围与 0 个 FluidWarfare 命中；② 后续工作统一拆为 ORG-1、SAFE-1、VK-LIFE-1、ARCH-A-PLAN、ARCH-A-IMPL、ORG-2 六个独立轮次；③ 密钥结论收窄为“指定模式未发现真实凭据泄漏”，明确不替代完整 secret scanning；④ 5+100 明确只统计 tracked 手写 `.cs/.axaml`，且 `.gitignore` 不会使已跟踪文件自动消失；⑤ 债务 A 明确须经过 ARCH-A-PLAN 设计、ARCH-A-IMPL 实装、构建与真机验收后才能收口；⑥ Hash 身份统一为代码基线、原报告、修正过程和 ORG-1 收口提交。
- 状态结论：**ORG-1 已完成并通过文档验收。** R1、R2 仅作为历史提交过程保留，不再作为持续状态标签。
- 影响范围：仅两份文档；未修改 `file-tree.md`，未修改任何 `.cs/.axaml/.csproj`，未删除 `111.ps1`，未处理 `qizheng-mvp-fixed/`，未新增测试、报告或 SVG。
- 验证结果：`git diff --check` 通过；旧错误措辞 0 命中；Markdown 标题、表格、代码围栏格式通过；未新增 Markdown 超链接；内容一致性、文件引用和事实准确性检查通过。纯文档修改不重新构建五个项目。
- Commit Hash：ORG-1 收口提交 `0667f2a`；其后宪法复核采用独立补正提交，不 amend、不强推，补正 Hash 以实际提交和最终交付报告为准。
- 文档同步：`changelog.md` 已更新；`file-tree.md` 未更新，因为本轮没有新增、删除、移动、重命名文件，也没有文件职责或依赖边界变化。
- 遗留问题：tracked 的 `111.ps1` 与 untracked、未忽略的 `qizheng-mvp-fixed/` 留给 SAFE-1；空 `catch`、Vulkan 失败回滚与 Present 线程可靠性留给 VK-LIFE-1；活跃 Vulkan 依赖留给 ARCH-A-PLAN/IMPL；本分支测试缺口、旧治理文档及 VK5-E 均未在本轮处理。
- 下一轮：SAFE-1，只处理 tracked 的 `111.ps1` 与 untracked、未忽略的 `qizheng-mvp-fixed/`。

## v0.2.14.6-rz
ORG-1-R1：项目基线审计修正版（2026-07-12，审计文档修正）

- 原历史编号：ORG-1-R1
- 日期：2026-07-12
- 任务目标：退回修正 ORG-1 审计报告（f187174）的 11 项误判；后续修正与收口完成后，该报告已作为 ORG-1 已验收基线。纯文档修正，不改代码。
- 主要改动：新增 `docs/project-baseline-audit-org-1-r1.md`（修正版，17 节含审计命令与结果附录）；原 `docs/project-baseline-audit-org-1.md` 顶部加"已退回，见 R1" superseded 注。修正要点：① 分支范围——所有"无 .sln/无测试"限定为 `fix/RZ-VK3-A-surface-contract` 分支 `f187174` 快照（实测 `origin/main` 含 `XuanYu.Engine.sln` + `XuanYu.Engine.Tests/`）；② 5+100——物理行数通过（111 文件 0 超限，最大 100）但质量条件不通过（VulkanRenderSession.cs / VulkanPresentLoop.cs 压行），红线总判定不成立；③ 空 catch（VulkanPresentLoop.cs:96-97）列为 P1；④ Editor.UI→Vulkan 为活跃组合根违反（VulkanSurfaceBridgeProvider + UiVm.VulkanProbe + VulkanProbeRoute 共 3 活跃 .cs + csproj）；⑤ Vk 所有权表重写（VulkanNativeHostSurfaceBridge 唯一持有/释放，Session 无 `_vk`）；⑥ Vulkan 失败路径回滚+Present 线程 Stop 可靠性列为 P1；⑦ 能力表数字修正 A=10/B=1/D=1 + 新增"未规划/不在当前阶段"类；⑧ 基线与报告身份分离；⑨ 新增标准命令、范围与结果记录；⑩ 111.ps1 修正为"非强推"（普通 git push -u）；⑪ 后续拆为 6 个独立轮次（ORG-1 / SAFE-1 / VK-LIFE-1 / ARCH-A-PLAN / ARCH-A-IMPL / ORG-2）。
- 影响范围：仅四份文档（changelog.md / file-tree.md / 新增 R1 审计文档 / 原审计文档加注）；零源码改动；不改 Git 历史/Commit/分支/Tag。
- 验证结果：5 项目 `dotnet build` 全 0W0E EXIT=0（复跑确认）；git ls-files 统计 111 手写 .cs/.axaml 全 ≤100（0 超限）；origin/main 实测含 .sln 与 Tests；密钥扫描（`git grep` 全 tracked 文本、指定模式）命中 3 处均复核为误报（0 真实凭据），FluidWarfare 源码层 0 命中、全文本 15 处均旧文档（**指定范围与指定模式未命中真实泄漏，非全仓安全结论**）；Editor.UI 活跃 Render.Vulkan 引用 3 处确认。
- Commit Hash：R1 主体 `e6f96b5` + R1 文字补正 `ef0ca11`。
- 历史修正过程：后续提交继续修正证据范围、Hash 身份、密钥扫描边界、Vulkan 生命周期风险、独立 ARCH-A-IMPL、能力计数口径与 SAFE-1 范围；ORG-1 收口 Hash 最终统一记录在 `v0.2.14.7-rz`，后续补正不改写已推送历史。
- 遗留问题（修正后仍仅报告不修复，排入对应轮）：P1 四项（债务A 活跃收口 / 空 catch / Vulkan 失败路径回滚 / 111.ps1 删除）；P2 多项（本分支无测试 / qizheng-mvp-fixed / 旧治理文档 / codex_log）；VK5-E 待实装。

## v0.2.14.5-rz
ORG-1：项目真实基线审计（2026-07-12，审计文档）

- 原历史编号：ORG-1
- 日期：2026-07-12
- 任务目标：建立可作为后续开发依据的项目基线——当前真实状态 / 已完成并真机验收的能力 / 仅有代码或计划未验收的能力 / 架构依赖与 Vulkan 生命周期是否符合宪法 / 代码·文档·仓库卫生债务。纯审计，不改代码、不重构、不删文件、不实装 VK5-E。
- 主要改动：新增 `docs/project-baseline-audit-org-1.md`（16 节：Git 状态 / 构建测试 / 项目依赖 / 5+100 / Vulkan 生命周期 / 能力状态 / 真机证据 / 文档基线 / 三份治理文档规则矩阵 / 仓库卫生 / 风险分级 P0-P3 / ORG-2 建议）。
- 影响范围：仅三份文档（changelog.md / file-tree.md / 新增审计文档）；零源码改动；不改 Git 历史/Commit/分支/Tag。
- 验证结果（关键数字）：5 项目 `dotnet build` 全 **0 警告 0 错误 EXIT=0**；本分支**无测试项目**（0 自动化测试覆盖）；5+100 审计 **111 个 tracked 手写 .cs/.axaml 全部 ≤100 行（0 超限）**；红线2（Abstractions 不依赖 Silk）成立；红线1（Editor.UI 不直接依赖 Vulkan）**当前违反=债务 A**（csproj 直接引用 Render.Vulkan + Silk.NET.Vulkan）；VulkanClearSession 死代码 4 文件确认真实（无外部引用）；关闭 RenderPass/Framebuffer 仅日志重复一行（非双重释放）；源码层 FluidWarfare 旧命名 0 命中；在本次指定模式扫描范围内未发现真实凭据泄漏，该检查不替代完整 secret scanning。
- Commit Hash：f187174（ORG-1 原始审计报告提交；后被 ORG-1-R1 取代，原报告已加 superseded 注）。
- 遗留问题：P1 两项（债务 A 收口 / `111.ps1` 危险脚本已入库待删）；P2 多项（零测试 / `qizheng-mvp-fixed` 未忽略 / 旧治理文档待裁决）；VK5-E 待实装。均仅报告不修复。

## v0.2.14.4-rz
DOC-VERSION-1：历史版本号标准化（含本轮 R1 修正）

- 原历史编号：DOC-VERSION-1
- 日期：2026-07-12
- 任务目标：将 changelog 多套历史版本号统一为 `v0.<里程碑>.<模块>.<修订次数>-<类型>`；本轮 R1 修正 changelog 自身登记缺失、将类型标签绝对表述改为暂用三类、将版本说明与映射表从 changelog 移出至配套 docs 文件、修正统计措辞。
- 主要改动：146 条历史条目标题统一为新版本号，正文首行保留「原历史编号」；changelog 恢复为纯更新日志（移除版本号说明与历史编号映射表），配套说明与映射索引另置于 docs/版本号规范与历史映射.md。
- 影响范围：changelog.md（纯文档）、新增 docs/版本号规范与历史映射.md；零源码改动。
- 验证结果：146 条标题全部唯一、原历史编号保留、原正文事实（日期/Commit/失败记录/真机验收）未改写；git diff 仅触达 changelog.md 与新增 docs 文件。
- Commit Hash：原始实施 3f03725；本轮 R1 修正提交：9bc210e。
- 遗留问题：无。

## v0.2.14.3-rz 新增《玄域引擎 AI 开发宪法》总治理文档（2026-07-12，文档）
- 原历史编号：[DOC]（宪法入库）

类型：纯文档补充，无代码改动。

- **新增** `docs/玄域引擎_AI开发宪法.md`（826 行，从用户 Downloads 移入 docs/）：作为后续所有开发、审计、修复、规划与 Codex 协作的**统一最高执行标准**，覆盖二十章——总执行原则 / 绝对红线（5+100、分层边界、敏感信息禁入库、聊天记录禁入库、禁止掩盖失败）/ AI 自主权限 / 计划与范围控制 / Bug 排查 / 日志 / 测试验证 / 异常处理 / 存档兼容 / 依赖配置 / 生成文件 / Git 规范 / 删移重命名 / 注释与 TODO / 警告静态检查 / 版本规范 / 文档同步 / 收口报告 / 重大事项请示。
- **关系说明**：原 `docs/AI_DEVELOPMENT_RULES.md`、`docs/CODE_CONSTITUTION.md` 内容已被本宪法统辖；本轮保留作历史参考，不删除、不重写原意（是否正式标记 deprecated 待用户定夺）。
- **文档同步**：`file-tree.md` 已更新（新增文件登记 + 文档补充快照 + 文件总数更新为 121）；`changelog.md` 本条。
- **验证**：纯文档任务按"文档同步规范"校验——格式（Markdown 标题层级完整）/ 链接（无失效内部链接）/ 内容一致性（5+100、分层边界与 `docs/dev-rules.md` 红线表述一致）/ 文件引用（指向 file-tree/changelog 机制）/ 事实准确性。无编译、无代码改动、无新增警告。
- **Commit Hash**：见本次提交（docs 文档补充，未动源码）。
- **遗留问题**：无阻断项。`AI_DEVELOPMENT_RULES.md` / `CODE_CONSTITUTION.md` 是否归档待用户决定，本轮不擅自删改。

## v0.2.12.1-vk 清理 VulkanClearSession 死代码（债务 B）规划（2026-07-11，规划·待确认实装）
- 原历史编号：RZ-VK5-E-Plan

分支：fix/RZ-VK3-A-surface-contract；基线 HEAD：139c748（RZ-VK5-C 封版）。
结论：**VulkanClearSession 为确定无引用的死代码，可安全删除，收口债务 B**。经审计：`VulkanClearSession`（Editor.UI 4 个 partial 文件）是 VK3-A 前早期探针，已被 `VulkanRenderSession` 正式链路取代；全仓 grep 确认无任何 `.cs` 外部引用或 `TryCreate` 调用方。
- 新增 `docs/rz-vk5-e-plan.md`：9 项规划（死代码确认 / 文件+调用方+替代链路+删除影响 / 正式链路由 VulkanRenderSession 承担 / 只删无引用死代码 / 不改三角形·Resize·PresentLoop·Pipeline / 不新增 / 全 .cs ≤100 / 双项目 0W0E / 实装步骤 + 风险回滚）+ 红线。
- 实装步骤（确认后）：`git rm` 4 文件 → 低内存构建验证 0W0E → 更新 changelog/file-tree → 独立 commit + push。
- 红线守住：只删死代码；不碰 VulkanRenderSession 链路 / UI / NativeHost / LOG-UX；不扩大 Editor.UI→Render.Vulkan 引用；双项目 0W0E；全 .cs ≤100。

## v0.2.10.1-vk viewport/scissor 与 Resize 关系验证收口（2026-07-11，验证收口·已封版）
- 原历史编号：RZ-VK5-C-Plan

分支：fix/RZ-VK3-A-surface-contract；基线 HEAD：c53b7a8（RZ-VK5-D-R3 封版）。
结论：**VK5-C 无需改代码，改为「验证收口轮」**。经源码取证，viewport/scissor 已使用动态状态、Resize 后 CommandBuffer 必然重录且取最新 Swapchain extent、GraphicsPipeline 不随 Resize 重建——三项诉求全部已满足。
- 新增 `docs/rz-vk5-c-plan.md`：8 问逐答（带文件/行号源码证据）+ 验证收口方案 + 真机 run-list。
- **审计通过（2026-07-11 用户回传）**：确认三件事成立（viewport/scissor 用动态状态、Resize 重录取最新 extent、Pipeline 不重建）。**修正目标表述**：禁止声称「三角形在不同宽高比下不变形」——viewport/scissor 仅决定绘制/裁剪区域，不负责几何宽高比保持；当前 NDC 固定坐标三角形随视口比例变宽/变扁属预期，宽高比修正留待 Camera/Projection，不纳入 VK5-C。
- 红线守住：不进 VK5-E；不新增渲染能力；不改三角形/shader/UI/NativeHost；不清 VulkanClearSession；不扩大 Editor.UI→Render.Vulkan 引用；全 .cs ≤100；双项目 0W0E；**本轮 0 代码改动**（无 .cs/.axaml/.csproj 变更）。
- 进度指针：VK5-C 验证收口后推进到 **VK5-E**（清 VulkanClearSession 死代码 = 债务 B）。

## v0.2.11.4-fix Resize 同尺寸快速跳过 Present 泵停启（2026-07-10，实装）
- 原历史编号：RZ-VK5-D-R3

分支：fix/RZ-VK3-A-surface-contract
R2 真实成绩：同尺寸 Swapchain/Framebuffer 重建已跳过、gen 追踪已真实化、三角形仍显示——但日志仍反复出现"自愈成功后又 Present 泵停止/启动"。根因进一步缩小：Resize 流程在调用 `PresentLoop.Stop()` 之前**未判断尺寸是否已经一致**，无论 Swapchain/Framebuffer 是否跳过重修，泵都被无意义地停一下再开，造成视觉停顿（慢半拍）。

- **快速跳过（核心）**：`VulkanRenderSession.Resize` 在 Stop 泵之前新增短路——若目标尺寸 `(width,height)` 已等于当前 `Swapchain.Extent`，直接打一条低频中文日志 `【VulkanClearFrame】Resize 快速跳过：尺寸已由自愈恢复（WxH）；generation=N` 并 `return`，**不** Stop/Start 泵、**不**重建 Swapchain/Framebuffer、**不**重录 CommandBuffer。尺寸一致时三者必然同源一致（Framebuffer extent 源自 Swapchain，CommandBuffer 已在上一轮对应重录），故快速返回安全。
- **保留 R2 去重**：同尺寸 Swapchain/Framebuffer 重建跳过逻辑原样保留（Resize 真正走全量时仍受益于去重）。
- **保留自愈**：Present 自愈机制（OutOfDate → RecoverFromOutOfDate）原样保留，未被删除。
- **日志新增**：`VulkanClearFrameLogFormatter.ResizeFastSkipped(uint generation, int w, int h)` 输出上述中文日志。
- 红线守住：不进 VK5-C/E；不新增渲染能力；不改三角形绘制/shader/UI/NativeHost；不清 VulkanClearSession；双项目 0W0E；全 .cs ≤100 行。
- **真机验收通过（2026-07-11，用户 run.bat 回传 trace）**：全部 11 项验收 PASS。
  - ①启动不重复 714x639：首帧 Present 成功（imageIndex=0）后，仅由 OutOfDate 自愈 16x16→714x639 一次，无重复 714x639 重建。
  - ②展开只 1 次 714x274 全量：自愈 714x639→714x274 一次（gen=2），符合预期。
  - ③同尺寸 Resize 快速跳过、不再 Stop/Start 泵：日志出现两条 `【VulkanClearFrame】Resize 快速跳过：尺寸已由自愈恢复（714x639）；generation=1` 与 `（714x274）；generation=2`，**均无** `Present 泵已停止/启动` 配对——同尺寸 Resize 导致的无意义停泵已消除；真实尺寸变化时仍会正常停泵重建。
  - ④唯一一次 `Present 泵已停止`/`已启动` 出现在收尾真实的窗口展开 Resize（714x274→714x639，gen=3→4），属尺寸真变化的正确必要行为，非慢半拍。
  - ⑤关闭释放顺序正确：Present泵停止→GraphicsPipeline释放→RenderPass+Framebuffer释放→Swapchain释放→LogicalDevice释放→Surface释放→Instance释放→Bridge分离。三角形全程持续 Present（未报丢失）。
  - 遗留非阻断：`RenderPass + Framebuffer 释放成功` 关闭时仍重复一行（LOG-CLEANUP，不影响功能）；UI 250ms Coalescer 防抖不在红线内。
  - 改动 .cs 行数 RenderSession 98→100、LogFormatter 21→23，均 ≤100；双项目 0W0E（Editor.UI 锁 dll 仅环境，改临时目录构建复验 0W0E）。
  - **正式封版（2026-07-11，用户拍板）**：RZ-VK5-D-R3 验收全过，文档封版。不追慢半拍后续；若仍疑有极轻视觉延迟，归因 UI 布局/防抖/日志面板刷新层，另开 UI 体验轮，不在 VK5-D 继续挖掘。下一步候选 VK5-C（viewport/scissor 边界收口）先于 VK5-E（清 VulkanClearSession 死代码）。

## v0.2.11.3-fix Resize / Present 重复重建去重 + 追踪 gen 修正（2026-07-10，实装）
- 原历史编号：RZ-VK5-D-R2

分支：fix/RZ-VK3-A-surface-contract
R1 真机 trace 铁证：3 处同尺寸重复 Swapchain 重建（启动 16x16→714x639 后 Resize 又建 714x639；展开时两次 714x274；幻影自愈 714x639→714x274 本已是 714x274）。根因=Resize 重建与 Present 自愈重建撞车，目标 extent 相同却各建一次。本轮最小修复（用户选方案1）。

- **去重（核心）**：`VulkanSwapchainOwner.Recreate/TryRecreateToCurrent` 与 `VulkanClearFrameOwner.RebuildFramebuffers` 均新增「目标 extent == 当前 extent 时跳过重建」短路（同尺寸打 Skipped 日志直接返回）。Resize 与自愈两条路径都受益，彻底消除重复重建。
- **修追踪 gen 硬编码 0**：`VulkanPresentLoop` 的 OutOfDate 日志原本 `gen=0` 硬编码，已移除；改在 `VulkanRenderSession.RecoverFromOutOfDate` 顶部用真实 `_generation` 打 `Present.OutOfDate` 日志。`Recreate/TryRecreateToCurrent/RebuildFramebuffers` 新增 `generation` 参数，所有 Stage 日志透传真实 gen。
- 红线守住：不建 VertexBuffer/DescriptorSet/Mesh/Camera/Scene；不改 UI/NativeHost/Shader；不清 VulkanClearSession；不进 VK5-C/E。
- 验证：Render.Vulkan 构建 0W0E；Editor.UI 无 C# 编译错误（仅因运行中的编辑器 PID 7236 锁 dll 致拷贝失败，非代码问题）；改动 .cs 行数 98/96/98/99 均 ≤100。

## v0.2.11.2-fix Resize / Present 慢半拍全链路诊断（2026-07-10，实装）
- 原历史编号：RZ-VK5-D-R1

分支：fix/RZ-VK3-A-surface-contract
VK5-D 边界收口后，用户真机发现"展开/收起日志栏时视口画面慢半拍"——功能最终正确（三角形保留、自愈恢复），但切换时有明显延迟。
本轮目标：**不修、先诊断**——给整条 Resize/Present 链路加 T+elapsedMs 追踪日志，定位慢在哪一段。

- 新增 `Diagnostic/VulkanResizeTracer.cs`（48 行）：共享 Stopwatch 诊断工具，提供 StartTrace()/ElapsedMs()/Stage()/HealStage()/DuplicateWarning() 方法。每次 Resize/自愈生成 `[T+XXXms gen=N]` 前缀的阶段日志。
- 修改 4 个文件加追踪点（行为零变化）：
  - `VulkanRenderSession.cs`（97 行）：Resize() 打 T+0 起点 + 完成日志；RecoverFromOutOfDate() 打自愈阶段日志（旧→新 extent）。LogProbe 内联消除。
  - `VulkanSwapchainOwner.cs`（96 行）：Recreate() 打请求尺寸 + 重建完成；TryRecreateToCurrent() 打旧 extent + Surface 查询。
  - `VulkanClearFrameOwner.cs`（95 行）：RebuildFramebuffers() 打开始 + FB创建完成+重录CB。
  - `VulkanPresentLoop.cs`（100 行）：AcquireNextImage/QueuePresent 的 OutOfDate 事件打来源追踪。
- 追踪覆盖的完整链路：UI Coalescer(250ms) → Bridge.Resize → RenderSession.Resize(T+0) → Stop泵 → Swapchain.Recreate(T+) → Framebuffer.Rebuild(T+) → CB重录(T+) → Start泵 → Present.OutOfDate(T+) → 自愈(T+) → 恢复Present(T+)。
- 红线守住：不进 VK5-C/D-E；不新增渲染能力；不改 UI/NativeHost/Shader/三角形逻辑；不清 VulkanClearSession；双项目 0W0E；全 .cs ≤100 行。
- 下一步：用户 run.bat 真机操作（展开/收起日志栏），回传完整 trace 日志，定位慢半拍根因后决定是否 R2 修复。

## v0.2.11.1-vk 清屏/绘制/录制/管线注入/Resize 重录 职责边界收口（2026-07-10，实装）
- 原历史编号：RZ-VK5-D

分支：fix/RZ-VK3-A-surface-contract
VK5-B 封版后，对"第一个三角形"链路做职责边界整理——只动 `VulkanClearFrameOwner.cs`，不新增任何渲染能力。
- 边界整理（仍 1 个文件、行为不变）：
  - 抽出 `RecordDraw(CommandBuffer cb)`：把"绘制"（BindPipeline + SetViewport + SetScissor + CmdDraw(3)）从 `RecordOne` 的"清屏"（BeginRenderPass 带 ClearValue）中拆出，`RecordOne` 变为 `清屏 → RecordDraw → 结束`。清屏与绘制成为两个一眼分开的方法。
  - 类头注释如实改写为"持有 RenderPass+CommandPool+CommandBuffer[]+Framebuffer[]"，并显式列出 VK5-D 职责边界：帧缓冲管理（BuildRenderPass/RebuildFramebuffers）｜命令录制（RecordCommandBuffers/RecordOne/RecordDraw）｜绘制（RecordDraw）｜管线注入（SetPipeline 触发重录）。
  - `SetPipeline`（管线注入）、`RebuildFramebuffers`（Resize 重建）两个重录触发入口保持各自直调 `RecordCommandBuffers` 的现状，但在类注释中统一说明"最终只走 RecordCommandBuffers 一处"，厘清两条重录路径的意图差异。
- 不参与（红线）：不建 VertexBuffer/IndexBuffer/UniformBuffer/DescriptorSet；不建 Mesh/Camera/Material/Scene；不改 UI/NativeHost/LOG-UX；不改 SwapchainOwner/PipelineOwner/PresentLoop/RenderSession 对外行为；不清 VulkanClearSession 死代码（留 VK5-E）。`SwapchainOwner`/`PipelineOwner` 经审计确认已是干净 SRP，本轮零改动。
- 验收：双项目 `dotnet build` **0W0E**；`VulkanClearFrameOwner.cs` 99→100（≤100 硬门禁守住，靠局部变量取地址 + 精简注释 + 去空行达成）；全仓 `.cs` 无新增超 100 行文件；`dotnet build` 0W0E；功能行为零变化（清屏+三角形显示、Resize 自愈、关闭释放顺序均与 VK5-B 封版一致）。
- 决策（用户拍板）：仅内部整理，**不重命名** `VulkanClearFrameOwner`（保守、零跨文件风险）。

## v0.2.9.1-vk 固定三角形绘制（gl_VertexIndex + CmdDraw，2026-07-10，实装）
- 原历史编号：RZ-VK5-B

分支：fix/RZ-VK3-A-surface-contract
在 VK5-A 已创建好的 GraphicsPipeline 基础上，画出蓝灰背景上的第一个固定三角形。不建 VertexBuffer / IndexBuffer / UniformBuffer / DescriptorSet，顶点由 `gl_VertexIndex` 在顶点着色器内生成。
- 着色器（`glslangValidator -V` 重新编译，内嵌 SPIR-V 字流，无 .vert/.frag/.spv 源文件入库）：
  - `ShaderBytecode.Vert.cs` 重写：顶点着色器用 `gl_VertexIndex` 索引 3 个常量顶点位置，输出 `gl_Position`（三角形居中、底边长、尖朝上）。
  - `ShaderBytecode.Frag.cs` 重写：片元着色器输出固定琥珀色 `vec4(1.0, 0.85, 0.2, 1.0)`（蓝灰背景上醒目）。
- 录制 + 装配：
  - `VulkanClearFrameOwner.cs` 95→99：新增 `_pipeline` 字段与 `SetPipeline(Pipeline)`（由 RenderSession 注入后重录 CommandBuffer，含 Draw）；`RecordOne` 在 `CmdBeginRenderPass` 之后、`CmdEndRenderPass` 之前插入 `CmdBindPipeline` + `CmdSetViewport` + `CmdSetScissor` + `CmdDraw(3,1,0,0)`；Resize 重建 Framebuffer 后重录自然带上 Draw（沿用原有 RebuildFramebuffers 路径，未改 Resize/Present 泵）。
  - `VulkanGraphicsPipelineOwner.cs` 96→97：新增 `public Silk.NET.Vulkan.Pipeline Pipeline => _pipeline` 供注入。
  - `VulkanRenderSession.cs` 97→98：`Create` 把 Pipeline 创建提前到 `loop.Start()` 之前，创建后 `clear.SetPipeline(pipeline.Pipeline)`（泵启动前注入，避免首帧竞态）；PresentLoop 完全未改。
- 验收：双项目 `dotnet build` **0W0E**；全改动 `.cs` ≤100（最大 99）；蓝灰清屏背景上出现琥珀色固定三角形；Resize 后三角形仍显示且 Present 自愈能力保留（RZ-VK5-A-R2 未破坏）；关闭释放顺序不变。
- 红线守住：不建 VertexBuffer/IndexBuffer/UniformBuffer/DescriptorSet；不接 Scene/Camera/Mesh/Material/Gizmo；不改 UI/NativeHost/LOG-UX；不扩大 Editor.UI→Render.Vulkan 引用；不清 VulkanClearSession；不破坏 RZ-VK5-A-R2 的 Resize 后 Present 恢复。
- **真机最终验收通过（2026-07-10，正式封版）**：①蓝灰背景上已显示琥珀色固定三角形；②展开底部日志栏后三角形仍显示；③Resize/日志栏变化后 Present 自愈恢复正常（控制台日志 `Swapchain 自愈成功，已恢复 Present；generation=3`）；④关闭窗口释放顺序正确（Present泵停止→GraphicsPipeline释放→RenderPass+Framebuffer释放→Swapchain→LogicalDevice→Surface→Instance→分离完成）；⑤未进入复杂渲染器（无 VertexBuffer/Scene/Camera/Mesh/Material/Gizmo）。遗留非阻断项：`RenderPass + Framebuffer 释放成功` 关闭日志重复打印一行，留待 LOG-CLEANUP 清理。**RZ-VK5-B 正式收口，Vulkan 最小图形渲染闭环成立。**

## v0.2.8.4-fix Present 泵 OutOfDate 受控自愈（Resize 后 Present 恢复，2026-07-10，实装）
- 原历史编号：RZ-VK5-A-R2

分支：fix/RZ-VK3-A-surface-contract
修复 Resize（拖窗口/调整日志栏）后 Present 泵重启即 `ErrorOutOfDateKhr` 并永久 `break`、最终停在"Swapchain 已过期"的问题。本轮只收口 Present 恢复，不 Draw、不画三角形、不进 VK5-B。
- 根因（用户真机 + 代码审计双重确认）：Windows 上 `VulkanSwapchainCapabilities.ChooseExtent` 直接 `return caps.CurrentExtent`，Swapchain extent = 创建/重建时刻 Surface 报告的尺寸；Resize 触发 `WM_SIZE` 异步、Avalonia 逻辑×DPI 与 Surface 实际客户区存在残余偏差，导致新 Swapchain 基于"旧时刻"尺寸建出，Present 时 Surface 已变 → OutOfDate。
- 修复设计：OutOfDate 不再永久 `break`，改经 RenderSession 统一入口 `RecoverFromOutOfDate` 自愈——查 Surface 当前 `CurrentExtent`、重建 Swapchain + 重建 Framebuffer + 重录 CommandBuffer、更新 `_generation`，PresentLoop `continue` 不退出线程（满足红线：PresentLoop 线程不 join 自身 Stop/Dispose）。
- 新增/修改：
  - `VulkanPresentLoop.cs` 99→100：构造函数新增 `Func<string,bool>? onOutOfDate`；Acquire/QueuePresent 遇 OutOfDate 调 `onOutOfDate(source)`（返回 false 才 break）；移除原 `_outOfDateLogged`/`OutOfDatePaused` 永久暂停分支。
  - `VulkanRenderSession.cs` 63→97：`RecoverFromOutOfDate(string source)` 统一自愈入口（返回 true=继续 / false=放弃暂停）；`_rebuildLock` 锁住 Resize 与自愈路径防并发重建；`_generation` 标记重建代次；连续自愈上限 `MaxRecoverTries=5`，超上限输出 `OutOfDateRecoverFailed` 中文日志并暂停（防刷屏/无限重建）；`Resize` 走 `lock(_rebuildLock)` 统一入口；`Create` 新增 `NativeHostSurfaceHandle?` 形参（供探针取 DPI）；lambda 绑定规避静态方法组引用实例方法。
  - `VulkanSwapchainOwner.cs` 88→100：新增 `TryRecreateToCurrent(out Extent2D)`——按 Surface 当前 `CurrentExtent` 重建（0/uint.MaxValue 尺寸跳过）。
  - `VulkanClearFrameLogFormatter.cs` 17→21：移除 `OutOfDatePaused`；新增 `OutOfDateProbe`（来源/旧 extent/新 Surface CurrentExtent/DPI/逻辑尺寸/generation）、`OutOfDateRecovered`、`OutOfDateRecoverFailed`。
  - `VulkanBridgeRenderSessionAttachStep.cs` 15→16 + `VulkanNativeHostSurfaceBridge.cs` 83：Attach 把 `NativeHostSurfaceHandle` 透传给 RenderSession（走 Abstractions 契约，不扩大 Editor.UI→Render.Vulkan）。
- 低频中文探针日志（自愈时一次）：OutOfDate 来源 / 旧 Swapchain extent / 新 Surface CurrentExtent / DPI / 逻辑尺寸 / generation / 自愈次数 / 成败。
- 验收：双项目 `dotnet build` **0W0E**；全改动 `.cs` ≤100（最大 100）；Resize 后 Present 自愈恢复（不再永久停在"Swapchain 已过期"）；关闭释放顺序不变（PresentLoop→GraphicsPipeline→ClearFrame→Swapchain→LogicalDevice→Surface→Instance）。
- 红线守住：不 Draw / 不画三角形 / 不建 VertexBuffer·DescriptorSet / 不接 Scene·Camera·Mesh·Material·Gizmo / 不改 UI overlay / 不扩大 Editor.UI→Render.Vulkan 引用（handle 走 Abstractions）/ 不清 VulkanClearSession / 不无限重建（守护上限）/ PresentLoop 线程不 join 自身 Stop/Dispose。

## v0.2.13.1-rz 移除视口内部顶部/底部 overlay，只留纯 Vulkan 视口（2026-07-09，UI 只改）
- 原历史编号：视口 UI 收口

分支：fix/RZ-VK3-A-surface-contract
- 修改 `XuanYu.Editor.UI/Main/Main.axaml`：移除视口内部顶部（透视 / NativeHost Probe）与底部（左键选择 / 中键环绕 / 右键平移 / 工具：选择）两组叠加条，UserControl 内容简化为仅 `<local:VulkanViewport/>`，中间区域只显示纯视口画面。
- 不删除选择/环绕/平移功能本身（`VulkanViewport` 交互逻辑未动），仅移除可见提示条。
- 不改 Vulkan / NativeHost 渲染逻辑；不改外部主工具栏、左侧项目树、右侧检查器、底部总状态栏；不碰 LOG-UX / Resize。
- 验收：双项目 `dotnet build` **0W0E**；视口内部顶部/底部叠加条消失，中间只剩纯视口，清屏与 Resize 不受影响。
- 同轮 `RZ-VK5-A-R1`：静态验证 Detach 释放顺序正确（PresentLoop → GraphicsPipeline 资源 → ClearFrame(RenderPass+Framebuffer) → Swapchain → LogicalDevice → Surface → Instance）；ShaderModule 短生命周期已落地（关闭链不含 ShaderModule）；释放顺序无破坏，**未改代码**。

## v0.2.8.3-vk ShaderModule + GraphicsPipeline 最小接入（2026-07-09，实装）
- 原历史编号：RZ-VK5-A

分支：fix/RZ-VK3-A-surface-contract
在 VK4-D Clear+Present 闭环上新增最小 Graphics Pipeline 创建/释放能力。不 Draw、不画三角形。
- 新增 `XuanYu.Render.Vulkan/Pipeline/`：
  - `ShaderBytecode.Vert.cs` / `ShaderBytecode.Frag.cs`：内嵌 SPIR-V `uint[]`（glslangValidator 本地编译，vertex+fragment passthrough，entry main）。CodeSize = Code.Length * 4。
  - `VulkanShaderModuleOwner.cs`：`unsafe` 助手，用 `uint[]` 建/销 vert+frag 两个 ShaderModule。
  - `VulkanGraphicsPipelineOwner.cs`：建空 PipelineLayout + 绑 RenderPass 的 GraphicsPipeline（动态 viewport/scissor、空 vertex input、TriangleList）；**建 Pipeline 后立即释放两个 ShaderModule**（短生命周期）；Dispose 释放 Pipeline→Layout。
  - `VulkanPipelineLogFormatter.cs`：中文日志格式器（经注入的 `Action<string> log`，日志单出口）。
- 修改：
  - `VulkanClearFrameOwner.cs`：+1 只读 getter `RenderPass => _renderPass`（供 Pipeline 绑定；构造时建一次，Resize 稳定）。
  - `VulkanRenderSession.cs`：+pipeline 字段；`Create` 中 ClearFrame 之后建 Pipeline；`Dispose` 中最先释放 Pipeline。
- 关键结论沿用规划：PresentLoop 提交 ClearFrameOwner 录好的 CommandBuffer，VK5-A 未绑定 Pipeline（无 Draw）→ PresentLoop 零改动；RenderPass 构造时建一次、Resize 只重建 Framebuffer → GraphicsPipeline Resize 稳定。
- 验收：双项目 `dotnet build` **0W0E**；所有改动 `.cs` ≤100（最大 96）；`Pipeline/` 4 文件。
- 红线守住：不 Draw / 不画三角形 / 不建 VertexBuffer·DescriptorSet / 不接 Scene·Camera·Mesh·Material·Gizmo / 不改 UI·NativeHost·LOG-UX·Resize / 不扩大 Editor.UI→Render.Vulkan 引用 / 不清 VulkanClearSession。

## v0.2.8.2-vk 采纳两条实装前修正（2026-07-09，仅文档）
- 原历史编号：RZ-VK5-A 规划修正

分支：fix/RZ-VK3-A-surface-contract
本轮仅修正 `docs/rz-vk5-a-plan.md`（RZ-VK5-A 规划），未改任何代码。
- 修正 1：Shader 字节码由 `byte[]` 改为 `uint[]`（`ShaderBytecode.Vert.cs` / `ShaderBytecode.Frag.cs`）。`PCode` 直接按 `uint*` 传入，`CodeSize = Code.Length * 4`，免 unsafe 字节转换/对齐/长度换算；SPIR-V 由 glslangValidator 本地编译生成，不引入运行时编译工具链。
- 修正 2：ShaderModule 生命周期由「持有到会话结束」改为「短生命周期」——创建 GraphicsPipeline 成功后立即释放两个 ShaderModule；Detach 只释放 GraphicsPipeline + PipelineLayout。Detach 顺序更短、更不易埋雷（采纳用户修正）。
- 同步更新 §2/§3/§6/§7/§9/§10/§11。

## v0.2.8.1-vk VK5-A 规划：ShaderModule + GraphicsPipeline 最小接入（2026-07-09，仅规划）
- 原历史编号：RZ-VK5-A-Plan

分支：fix/RZ-VK3-A-surface-contract
本轮**只规划、不写代码**（未改任何 `.cs` / `.axaml` / `.csproj`）。
- 新增 `docs/rz-vk5-a-plan.md`：RZ-VK5-A 规划——在当前 VK4-D Clear+Present 闭环上接入 ShaderModule + PipelineLayout + GraphicsPipeline 最小方案。
- 必读已读：`rz-vk4-closure.md` / `rz-vk5-plan.md` / `file-tree.md` / `docs/dev-rules.md`（已存在，不新建）/ 当前 Vulkan Clear+Present 源。
- 关键结论：①`PresentLoop` 提交 `ClearFrameOwner` 录好的 CommandBuffer，VK5-A/B 加 `CmdBindPipeline`+`CmdDraw` 会被自动提交，PresentLoop 零改动；②`RenderPass` 构造时建一次、Resize 只重建 Framebuffer，故绑它的 GraphicsPipeline 在 Resize 时无需重建。
- 输出 10 项：当前 Vulkan 文件职责 / VK5-A 新增+修改清单 / ShaderModule 创建释放 / PipelineLayout 创建释放 / GraphicsPipeline 创建释放 / RenderPass·Swapchain·Framebuffer·Pipeline 依赖 / ≤100 拆分 / 禁止事项 / 验收 / 风险与回滚；含 3 个决策点（内嵌 SPIR-V byte[] / 动态 viewport-scissor / ShaderModule 持有到会话结束）。
- VK5-A 实装禁止事项（承 red lines）：不 Draw、不画三角形、不建 VertexBuffer/DescriptorSet、不接 Scene/Camera/Mesh/Material/Gizmo、不改 UI/NativeHost/LOG-UX/Resize、不扩大 Editor.UI→Render.Vulkan 引用、不清 VulkanClearSession。

## v0.2.14.2-rz 仓库记忆文件收口：.workbuddy/memory 移出追踪并 gitignore（2026-07-09，仅仓库卫生）
- 原历史编号：VK5-Plan-R1

分支：fix/RZ-VK3-A-surface-contract
本轮不触碰任何代码，仅做仓库卫生收口。
- 将 `.workbuddy/memory/`（4 文件：2026-07-07.md / 2026-07-08.md / 2026-07-09.md / MEMORY.md）从 Git 追踪移除（`git rm --cached`），本地文件保留。
- `.gitignore` 新增 `.workbuddy/`，使本地 AI 工作记忆不再进入仓库历史，避免污染项目文档与协作历史。
- 理由：项目文档（`docs/`）面向仓库与协作者；AI 本地工作记忆面向工具/会话/用户偏好，二者混库会污染历史。
- 重要项目信息无损：架构债务 A/B 的收口节奏已固化于 `docs/rz-vk5-plan.md` §12，不依赖 `.workbuddy/memory`。
- 报表格式等跨项目用户偏好改为写入用户级 `~/.workbuddy/MEMORY.md`（本地，不进仓库）。
- `file-tree.md` 无需更新（无 `.workbuddy` 登记项）。

红线：未改任何 `.cs`/`.axaml`/`.csproj`/Vulkan/UI/NativeHost/LOG-UX 代码。

## v0.2.14.1-rz 两条已知债务升格为架构债务并锁死节奏（2026-07-09，仅文档）
- 原历史编号：VK5-Plan 债务升格

分支：fix/RZ-VK3-A-surface-contract
本轮仅对 `docs/rz-vk5-plan.md` 增补「架构债务升级」节（§12，原 §12 规划图顺延为 §13），未改任何代码。
- **债务 A：`Editor.UI` 仍直接引用 Render.Vulkan** 升格为架构债务：VK5-A~D 可暂缓但**禁止扩大**；VK6 / 独立 ARCH 轮必须收口到 `Render.Abstractions` 契约层。
- **债务 B：`VulkanClearSession` 死代码** 清理排在 **VK5-E**（VK5-A/B/C/D 之后），不在 VK5-A 前清，以免弄乱已验证的 Clear+Present 闭环；清理独立 commit。
- 提交复核：`fe6d5d3` 仅含 6 文件（`.workbuddy/memory/` 项目级工作记忆 + 两份新 doc + changelog + file-tree），无 `.codex/`/`.ai-memory/`/密钥；`.workbuddy/memory/` 为本项目惯例追踪，非用户级 AI 文件。

## v0.2.6.8-vk VK4 收口归档 + VK5 最小几何渲染规划（2026-07-09，仅文档）
- 原历史编号：VK4-Closure + VK5-Plan

分支：fix/RZ-VK3-A-surface-contract
本轮**只写文档，不写实装代码**（未改任何 `.cs` / `.axaml` / `.csproj` / Vulkan 实装 / LOG-UX / NativeHost 代码）。

### 新增文档
- `docs/rz-vk4-closure.md`  # VK4 阶段正式收口确认：VK4-A（PhysicalDevice）/ VK4-B（LogicalDevice+Queue）/ VK4-C（Swapchain+ImageView）/ VK4-D（Clear+Present 单色清屏）+ VIEWPORT-RESIZE-R2（DPI 逻辑/物理尺寸错配修复）逐项收口表；已验证清单（首帧 Present、蓝灰覆盖、Resize 恢复、详情栏不慢半拍、日志单出口、双项目 0W0E、全 .cs ≤100）；长期硬规则（Bounds 逻辑尺寸 / Win32+Surface 物理像素 / physical=round(logical×DPI) / Render.Vulkan 不引用 Avalonia / UI 不接触 Silk.NET.Vulkan）；已知债务与下一阶段指向。
- `docs/rz-vk5-plan.md`  # VK5 最小几何渲染规划（只规划不实装）：从「单色清屏」进入「最小图元渲染」，先画固定三角形/测试图元，不接场景/相机/网格/材质/Gizmo。分阶段 VK5-A（Shader+Pipeline）/ VK5-B（gl_VertexIndex 画固定三角形，暂不建 VertexBuffer）/ VK5-C（viewport/scissor 动态状态同步 extent，Resize 不重建 Pipeline）/ VK5-D（Clear/Draw 职责收进清晰 RenderPass/FrameRenderer 边界，不进场景系统）；资源创建/释放顺序、文件结构、12 条红线、逐阶段验收、SVG 规划图。

### 更新
- `changelog.md` / `file-tree.md` 同步登记两份新文档。

红线：本轮不触碰任何代码；VK5 第一步不是场景渲染，而是「固定三角形 / 最小 Pipeline / 最小 Draw」；规划通过后再开 VK5-A 实装。

## v0.2.6.7-vk 正式收口确认（2026-07-09）
- 原历史编号：VK4-D

VK4-D（最小 Clear+Present 单色清屏闭环）经三轮收口全部完成：VK4-D-R3（Render.Vulkan 侧 OutOfDate 优雅降级 + Resize 日志顺序 + 物理像素诚实日志）、VIEWPORT-RESIZE-R1（Editor.UI 日志详情栏切换后布局稳定主动同步最终尺寸）、VIEWPORT-RESIZE-R2（修正 R1 的 DPI 错配，物理像素 = round(逻辑×DPI)）。双项目均 0 warning / 0 error，全 .cs ≤100。

运行态验收（用户真机，2026-07-09）：
1. 蓝灰铺满整个 NativeHost 区域，不再半屏黑。✅
2. 打开/关闭日志详情栏立即同步，不慢半拍。✅
3. Win32 子窗口物理尺寸 = 逻辑尺寸 × DPI（DPI=1.75 时 713×188 → 1248×330）。✅ R2 修复。
4. Swapchain / Framebuffer extent 与 Win32 子窗口物理尺寸一致。✅ R3 同源物理像素。
5. 无闪退 / 未响应。✅ VK4-D-R2 已修后台线程日志回调线程派发导致闪退。
6. 关闭释放顺序：Present 泵停止 → ClearFrame 释放 → Swapchain 释放 → LogicalDevice 释放 → Surface 释放 → Instance 销毁 → 分离完成。✅（VK4-D-R3 真机验证；R2 未触碰 Release 路径，释放顺序不变；建议收口后跑一次关闭日志做最终确认。）

结论：VK4-D 正式收口。下一阶段：VK4-D 收口文档归档 / VK5 规划（场景渲染 / 相机 / 网格 / 材质 / Gizmo / UI 叠加，均不在 VK4-D 红线内）。

红线全程守住：不进场景渲染；Resize 不重建 Surface/Instance/Device/Queue；Editor.UI 不接触 Silk.NET.Vulkan；日志单出口；全 .cs ≤100。

## v0.2.6.6-fix 修复 R1 的 DPI 错配：Win32 子窗口 Resize 必须收物理像素（2026-07-09）
- 原历史编号：VIEWPORT-RESIZE-R2

分支：fix/RZ-VK3-A-surface-contract
前提：VK4-D-R3 + VIEWPORT-RESIZE-R1 后用户真机验收，R1 解决了「慢半拍」，但引入更关键的 DPI 尺寸错配。R1 的 `VulkanNativeHost.LayoutSync.SyncFinalSize` 把 Avalonia `Bounds` 的**逻辑尺寸**（713×188）直接当物理像素喂给 `Win32ViewportHost.Resize`（裸 `SetWindowPos`，不乘 DPI），把子 HWND 缩成逻辑尺寸；探针日志显示 `Win32子窗口=713x188`、`Surface CurrentExtent=713x188`，蓝灰画面只占左上角，右侧/下方露黑。真因：`Bounds` 是逻辑像素，Win32/Vulkan 要物理像素；R1 少了 `×DPI` 换算、绕过了 Avalonia 本来的 DPI 感知摆放。
状态：已收口。双项目 0W0E，全 .cs ≤100；2026-07-09 用户真机验收通过（蓝灰覆盖整个 NativeHost 可视区、逻辑×DPI≈目标物理≈Win32子窗口≈Surface CurrentExtent）。

### 修复点（Editor.UI 侧，VIEWPORT-RESIZE-R2）
1. **`SyncFinalSize` 物理像素换算**：`Bounds` 逻辑尺寸先 `physical = max(1, round(logical × GetDpiScale()))`，`Win32ViewportHost.Resize(_hwnd, physicalW, physicalH)` 收**物理像素**；`_bridge.Resize(logicalW, logicalH)` 仍收逻辑尺寸（供日志与请求尺寸，Render.Vulkan 侧最终用 `caps.CurrentExtent` 建 Swapchain，与 VK4-D-R3 同源）。
2. **探针补「目标物理」字段**：`LogNativeHostProbe` 现输出「日志详情栏；逻辑；DPI；目标物理=CWxCH；Win32子窗口=AWxAH」与 detail「逻辑×DPI≈…；目标物理=…；子窗口实际=…」；验收必须看到 `目标物理≈1248x330` 且 `Win32子窗口≈1248x330`，若仍 `713x188` 即未修好。
3. **`_resizer.Cancel()` 仍保留**：布局稳定后主动同步立即取消待处理的 250ms Coalescer，避免重复重建；拖动窗口仍走 `OnSizeChanged → NativeHostResizeCoalescer`（高频合并），互不干扰。

### 红线守住（均未触碰）
- 不新增场景渲染 / 相机 / 网格 / 材质 / Gizmo / UI 叠加。
- 不修改日志 UX（LOG-UX 自动滚动 / 多选复制保持）。
- 不让 Editor.UI 直接接触 Silk.NET.Vulkan 类型（仅动 Win32 `user32` P/Invoke 与 Avalonia 调度）。
- 不重建 Surface / Instance / LogicalDevice / Queue（Resize 只到 HWND + Swapchain/Framebuffer 重建）。
- 不把 RenderPass/CommandBuffer/PresentLoop 塞进 Bridge。
- 全 .cs ≤100：`VulkanNativeHost.cs` 98 / `VulkanNativeHost.LayoutSync.cs` 49 / `Win32ViewportHost.cs` 67 / `ViewportNativeHostRoute.cs` 18 / `UiVm.NativeHostLifecycle.cs` 38。

### 验收
1. 启动不闪退、不未响应。
2. 蓝灰覆盖整个 NativeHost 可视区，不再半屏黑。
3. 打开/关闭日志详情栏，视口立即同步，不慢半拍。
4. 探针日志：逻辑×DPI ≈ 目标物理 ≈ Win32子窗口 ≈ Surface CurrentExtent（DPI=1.75 时 713×188 → 1248×330）。
5. Resize 不重建 Surface / Instance / Device / Queue；控制台日志不重复。

## v0.2.6.5-fix Editor.UI 日志详情栏展开/收起后 NativeHost 最终尺寸主动同步（2026-07-09）
- 原历史编号：VIEWPORT-RESIZE-R1

分支：fix/RZ-VK3-A-surface-contract
前提：VK4-D-R3 后用户真机验收，OutOfDate 刷屏已止，但「半屏蓝灰、下半黑」仍在；日志证明 Swapchain/Framebuffer/RenderArea 已同源物理像素（Surface CurrentExtent=1248x961 = 713x549×1.75 DPI）。根因转向 Editor.UI：日志详情栏展开/收起是低频离散布局变化，但 Vulkan Swapchain 重建只走 250ms Coalescer，导致 Present 泵停（OutOfDate）后等 250ms 才重建，旧小 Swapchain 帧停在顶部、下方黑；且离散变化可能不被 Coalescer 及时捕获。
状态：修复完成，双项目 0W0E，全 .cs ≤100。待用户真机验收「蓝灰覆盖整个 NativeHost 可视区、不再半屏黑；关闭日志详情栏视口立即恢复不慢半拍；逻辑×DPI 与 Surface CurrentExtent 对齐」。

### 修复点（Editor.UI 侧，VIEWPORT-RESIZE-R1）
1. **日志详情栏切换不等 Coalescer，布局稳定后立即同步最终尺寸（修法 A/C）**：`VulkanNativeHost` 改为 `partial`，新增 `VulkanNativeHost.LayoutSync.cs`；在 `DataContextChanged` 时订阅 `UiVm.IsLogOpen` 的 `PropertyChanged`，变化后以 `Dispatcher.UIThread.InvokeAsync(SyncFinalSize, DispatcherPriority.Render)` 调度——等布局稳定后读 `Bounds` 最终值，立即 `Win32ViewportHost.Resize` + `_bridge.Resize(w, h)`，并先 `_resizer.Cancel()` 取消待处理的 250ms debounce，避免重复重建。拖动窗口仍走 `OnSizeChanged → NativeHostResizeCoalescer`（高频合并），互不干扰。
2. **中文探针核对四者对齐**：`SyncFinalSize` 经 `ViewportNativeHostRoute.ReportProbe` → `UiVm.LogNativeHostProbe` 打印「日志详情栏=展开/收起；逻辑=WxH；Win32子窗口=CWxCH；DPI」与「逻辑×DPI≈…；子窗口物理=…」，与 Render.Vulkan 侧已有的「Surface CurrentExtent / Swapchain chosen extent / Framebuffer extent」交叉核对：Avalonia 逻辑尺寸 × DPI ≈ Win32 子窗口物理尺寸 ≈ Surface CurrentExtent ≈ Swapchain/Framebuffer extent。
3. **Win32 子窗口真实尺寸可读**：`Win32ViewportHost` 新增 `GetClientSize(hwnd)`（P/Invoke `GetClientRect` + `RECT`），供探针取子窗口物理像素。

### 红线守住（均未触碰）
- 不新增场景渲染 / 相机 / 网格 / 材质 / Gizmo / UI 叠加。
- 不修改日志 UX（LOG-UX 自动滚动 / 多选复制保持）。
- 不让 Editor.UI 直接接触 Silk.NET.Vulkan 类型（本轮仅动 Win32 `user32` P/Invoke 与 Avalonia 调度，未引入 Vulkan 类型；遗留的 `VulkanClearSession.cs` 属死代码、非活跃链路，本轮不动）。
- 不重建 Surface / Instance / LogicalDevice / Queue（Resize 只到 HWND + Swapchain/Framebuffer 重建）。
- 不把 RenderPass/CommandBuffer/PresentLoop 塞进 Bridge。
- 全 .cs ≤100：`VulkanNativeHost.cs` 99 / `VulkanNativeHost.LayoutSync.cs` 38 / `Win32ViewportHost.cs` 67 / `ViewportNativeHostRoute.cs` 18 / `UiVm.NativeHostLifecycle.cs` 40。

### 关键实现细节
- `SyncFinalSize` 读取的 `Bounds` 为 Avalonia 逻辑像素；`_bridge.Resize(w, h)` 传入逻辑尺寸，Render.Vulkan 侧 `VulkanSwapchainOwner.Recreate` 用 `caps.CurrentExtent`（HWND 真实物理像素）建 Swapchain，故 extent 始终以物理像素为准，与 VK4-D-R3 同源。
- `DispatcherPriority.Render` 确保 `SyncFinalSize` 在布局 pass 完成后运行，读到最终 `Bounds`；若布局仍微抖，Coalescer 作为兜底会在末次 `OnSizeChanged` 后 250ms 再同步一次（冗余但无害）。
- `HookLayoutSync`/`UnhookLayoutSync` 用 `_layoutSyncHooked` 守卫避免重复订阅；`OnDetachedFromVisualTree`/`DestroyNativeControlCore` 中 `UnhookLayoutSync()` 防泄漏。

### 验收
1. 启动不闪退、不未响应。
2. 蓝灰覆盖整个 NativeHost 可视区，不再半屏黑。
3. 关闭日志详情栏，视口立即恢复，不慢半拍。
4. 打开日志详情栏，视口缩小正常。
5. 日志显示「布局同步探针」：逻辑×DPI ≈ Win32 子窗口 ≈ Surface CurrentExtent。
6. Resize 不重建 Surface / Instance / Device / Queue；控制台日志不重复。

## v0.2.6.4-fix Present 泵 OutOfDate 降级 + Resize 日志顺序 + 物理像素诚实日志（2026-07-09）
- 原历史编号：VK4-D-R3

分支：fix/RZ-VK3-A-surface-contract
前提：VK4-D-R2 收口后用户真机验收发现（1）蓝灰清屏只覆盖上半部分（半屏）；（2）`QueuePresent` 反复返回 `ErrorOutOfDateKhr` 刷屏；（3）日志详情栏展开/收起后 NativeHost 视口尺寸同步慢半拍；（4）Resize 时序竞争。用户给出两阶段方案：VK4-D-R3（Render.Vulkan 侧）修 ①②，VIEWPORT-RESIZE-R1（Editor.UI 侧）修 ③。
状态：VK4-D-R3 修复完成，双项目 0W0E，全 .cs ≤100（VulkanPresentLoop 99 / VulkanRenderSession 59 / VulkanSwapchainOwner 88 / VulkanSwapchainCapabilities 81 / VulkanSwapchainLogFormatter 15 / VulkanClearFrameLogFormatter 17）；待用户真机验收「全区域蓝灰不再半屏 + Resize 单次重建 + OutOfDate 不再反复刷 + 关闭/打开日志详情栏后视口立即同步」。VIEWPORT-RESIZE-R1 尚未开始。

### 修复点（Render.Vulkan 侧，VK4-D-R3）
1. **OutOfDate 优雅降级（不再刷屏）**：`VulkanPresentLoop.Run` 中 `QueuePresent` 返回 `ErrorOutOfDateKhr` 时，仅在首次（`_outOfDateLogged` 守卫）记录一次 `【VulkanClearFrame】Swapchain 已过期，暂停 Present，等待 Resize 重建`，随后 `break` 退出 Present 泵；不再当作错误反复刷屏并 break。新增字段 `_outOfDateLogged`，`Start()` 重置为 false。
2. **Acquire 的 OutOfDate 仍 sleep continue**：`AcquireNextImage` 返回 `ErrorOutOfDateKhr` 保持 `Thread.Sleep(1); continue;` 等下次 Resize 重建（与 VK4-D-R1 一致，不当失败）。
3. **Resize 日志顺序收口**：`VulkanRenderSession.Resize` 改为 `Stop 泵 → Swapchain Recreate → RebuildFramebuffers → 记 Rebuilt（用 _swapchainOwner.Extent 物理像素）→ Start 泵`，确保「Framebuffer 重建成功」日志在「泵启动」之前，且 extent 来源统一为物理像素。
4. **物理像素诚实日志**：`VulkanSwapchainCapabilities.Query` 能力日志同时打印「请求逻辑尺寸 / Surface CurrentExtent（物理像素）/ 选择 extent（物理像素）」；`VulkanSwapchainLogFormatter.Created/Recreated` 与 `VulkanClearFrameLogFormatter.Rebuilt` 改收 `Extent2D` 打印「（物理像素）」；`VulkanSwapchainOwner` 创建/重建改传实际 `_extent`。

### 关于「半屏蓝灰」的判断
- 经核对：`VulkanSwapchainOwner._extent`（= `caps.CurrentExtent` 物理像素）、`VulkanClearFrameOwner` 的 Framebuffer/RenderPass/RenderArea 均以此 `Extent2D` 为唯一来源，三者同源。用户方案中「Framebuffer 用逻辑尺寸」的假设在代码中不成立，故本阶段不盲改已正确的 extent 统一，只通过诚实日志让半屏真因在真机运行日志中暴露，交由 VIEWPORT-RESIZE-R1 视 probe 结果决定。

### 严禁（均未触碰）
- 不新增场景渲染 / 相机 / 网格 / 材质 / Gizmo / UI 叠加 / 持续动画。
- 不修改日志 UX（LOG-UX 成果保持）。
- 不让 Editor.UI 直接接触 Silk.NET.Vulkan 类型（VK4-D-R3 仅动 Render.Vulkan）。
- 不把 RenderPass/CommandBuffer/PresentLoop 塞进 Bridge（Bridge 83 行仅委托）。

### 关键实现细节
- `Result.SuboptimalKhr` 仍是成功码（`res != Success && res != SuboptimalKhr` 才判失败）；`ErrorOutOfDateKhr` 反映 Swapchain 过期，需重建而非当错误。
- `Stop()` 局部捕获 `_thread` 引用（`var t = _thread;`），避免 Join 后外部置 null 造成 NRE。
- 日志单出口：`VulkanClearFrameLogFormatter` 纯文本格式器，经 Bridge `Emit` 统一输出；新增 `OutOfDatePaused()` 走同一通道。

### 验收
1. XuanYu.Render.Vulkan 0 warning / 0 error。
2. XuanYu.Editor.UI 0 warning / 0 error（VK4-D-R3 未动 Editor.UI）。
3. 所有 .cs 文件 ≤100 行（VulkanPresentLoop 99 / VulkanRenderSession 59 / VulkanSwapchainOwner 88 / VulkanSwapchainCapabilities 81 / VulkanSwapchainLogFormatter 15 / VulkanClearFrameLogFormatter 17）。
4. 真机：全区域蓝灰不再半屏（以物理像素 extent 日志核对）。
5. 真机：Resize 一次只重建一次 Swapchain；日志顺序「Framebuffer 重建成功」在「Present 泵已启动」之前。
6. 真机：OutOfDate 不再反复刷屏，仅记一次「Swapchain 已过期，暂停 Present」。
7. 关闭时释放顺序正确（ClearFrame→Swapchain→Device→Surface→Instance）。

### Commit
见交付报告（本 commit 哈希在回复中给出）。

## v0.2.6.3-fix Present 泵后台线程日志回调线程派发修复（2026-07-09）
- 原历史编号：VK4-D-R2

分支：fix/RZ-VK3-A-surface-contract
前提：VK4-D-R1 修复后用户真机启动即闪退，退出码 -532462766；异常为 `System.InvalidOperationException: The calling thread cannot access this object because a different thread owns it`，堆栈指向 `XuanYu.Editor.UI.VulkanNativeHost.<OnAttachedToVisualTree>b__6_0` 访问 `DataContext` 时崩溃，该日志来自 `VulkanPresentLoop.Run` 独立后台线程。
根因：VK4-D 引入独立 Present 线程后，PresentLoop 后台线程经 `VulkanBridgeLogFormatter.Emit` → `VulkanNativeHostSurfaceBridge.Emit` → 回调 `msg => ViewportNativeHostRoute.ReportVulkanBridge(DataContext as UiVm, msg)` 直接访问 Avalonia `DataContext`（仅 UI 线程可访问），Avalonia 抛异常未处理，进程闪退。
状态：修复完成，双项目 0W0E，全 .cs ≤100（VulkanNativeHost 95 / VulkanPresentLoop 96）；待用户真机验收「启动不闪退 + 首帧 Present 成功 + 蓝灰清屏 + Resize 单次重建」。

### 修复点
1. **日志回调线程安全入口**：`VulkanNativeHost.cs` 把原内联回调 `msg => ViewportNativeHostRoute.ReportVulkanBridge(DataContext as UiVm, msg)` 改为方法 `ReportVulkanMessage`；内部 `Dispatcher.UIThread.CheckAccess()` 判断——UI 线程直接调用 `ReportVulkanMessageOnUiThread`，非 UI 线程经 `Dispatcher.UIThread.Post(...)` 切回 UI 线程后再访问 `DataContext` / `UiVm` / 日志集合。新增 `using Avalonia.Threading;`。
2. **Present 泵日志防御**：`VulkanPresentLoop.Log` 由 `_log?.Invoke(m)` 改为 `try { _log?.Invoke(m); } catch { }`，后台线程日志回调异常被吞掉，绝不终止 Present 泵或炸进程（第二层保护）。

### 严禁（均未触碰）
- 不新增渲染能力：未改 RenderPass / Framebuffer / CommandBuffer / Present / Swapchain Resize 逻辑。
- 不修改日志 UX（LOG-UX 成果保持）。
- 不让 Render.Vulkan 引用 Avalonia：线程派发全部落在 Editor.UI 的 `VulkanNativeHost.cs`；`VulkanPresentLoop` 仅 `try/catch` 包裹 `Action<string> log` 调用，不引用任何 Avalonia 类型。
- Bridge 不膨胀（仍 83 行仅委托）。

### 关键实现细节
- 日志回调契约仍为 `Action<string> log`；契约不变，仅消费方（Editor.UI）保证线程安全。
- Emit 单出口不变：`VulkanBridgeLogFormatter.Emit` 仍先 `log?.Invoke` 后 `Console.WriteLine`；UI 集合更新经 Dispatcher 在 UI 线程发生，与控制台输出并行不悖。
- 控制台单出口、日志不重复、无种子日志：均保持。

### 验收
1. XuanYu.Render.Vulkan 0 warning / 0 error。
2. XuanYu.Editor.UI 0 warning / 0 error。
3. 所有 .cs 文件 ≤100 行（VulkanNativeHost 95 / VulkanPresentLoop 96）。
4. run.bat 启动不闪退，不再出现 "The calling thread cannot access this object"。
5. 控制台出现 "Present 泵已启动（独立线程）"。
6. 控制台出现 "首帧 Present 成功"。
7. 视口变成明显蓝灰色（0.25/0.45/0.70）。
8. Resize 后只重建一次 Swapchain。
9. 关闭时释放顺序正确（ClearFrame→Swapchain→Device→Surface→Instance）。
10. Vulkan 日志不重复，旧种子日志不出现。

### Commit
见交付报告（本 commit 哈希在回复中给出）。

## v0.2.6.2-fix Clear + Present 运行审计与 Resize 去重（2026-07-09）
- 原历史编号：VK4-D-R1

分支：fix/RZ-VK3-A-surface-contract
前提：VK4-D 初版实装后，用户真机发现（1）视口仍为黑色，（2）一次 Resize 触发两次 Swapchain 重建。
状态：修复完成，双项目 0W0E，全 .cs ≤100；待用户真机验收「单色清屏画面 + Resize 单次重建 + 首帧 Present 成功日志」。

### 修复点
1. **Resize 去重**：`VulkanNativeHostSurfaceBridge.Resize` 不再直接调用 `_swapchainOwner?.Recreate`，只转发 `_renderSession?.Resize(width, height)`；统一入口在 `VulkanRenderSession.Resize`：Stop pump → Swapchain recreate → Rebuild Framebuffers → Start pump。
2. **首帧 Present 可观测**：`VulkanPresentLoop` 成功完成第一次 `QueuePresent` 后，输出一次 `【VulkanClearFrame】首帧 Present 成功；imageIndex=...`，不每帧刷屏。
3. **Present 错误可见**：`AcquireNextImage` / `QueuePresent` 返回非成功/非 `SuboptimalKhr` 时，输出中文错误日志。
4. **修正 `SuboptimalKhr` 处理**：`AcquireNextImage` 返回 `SuboptimalKhr`（成功码）不再被误判为失败并退出 Present 泵；`ErrorOutOfDateKhr` 则 sleep 后继续，等下次 Resize 重建。
5. **clear 颜色更明显**：由 0.10/0.30/0.45 改为 0.25/0.45/0.70 蓝灰，便于肉眼区分黑屏与清屏。

### 严禁（均未触碰）
- 不做场景渲染 / 相机 / 网格 / 材质 / Gizmo / UI 叠加 / 持续动画。
- 不修改日志 UX（LOG-UX 成果保持）。
- 不把 RenderPass/CommandBuffer/PresentLoop 塞进 Bridge（Bridge 83 行仅委托）。
- 不让 Editor.UI 直接接触 Silk.NET.Vulkan 类型。

### 关键实现细节
- Present 泵独立后台 `Thread(IsBackground)`；Detach/Resize 先 `Stop()`（Join 2000ms）再释放/重建资源。
- 单 in-flight 帧 + 单 Fence；`_submitted` 守卫避免首帧 `WaitForFences` 空等。
- `KhrSwapchain` 方法名无 KHR 后缀：`AcquireNextImage` / `QueuePresent`。
- `using Semaphore = Silk.NET.Vulkan.Semaphore` 消除 `System.Threading.Semaphore` 歧义。

### 验收
1. XuanYu.Render.Vulkan 0 warning / 0 error。
2. XuanYu.Editor.UI 0 warning / 0 error。
3. 所有 .cs 文件 ≤100 行。
4. 编辑器启动不未响应。
5. 视口从黑色变为明显单色。
6. 控制台出现 `首帧 Present 成功`。
7. 一次 Resize 只出现一次 Swapchain 重建。
8. Resize 后单色画面恢复。
9. Detach 顺序正确。
10. Vulkan 日志不重复，旧种子日志不出现。

### Commit
见交付报告（本 commit 哈希在回复中给出）。

## v0.2.6.1-vk 最小 Clear + Present 单色清屏闭环（D1+D2+D3 同轮）(2026-07-09)
- 原历史编号：VK4-D

分支：fix/RZ-VK3-A-surface-contract
依据：docs/rz-vk4-d-plan.md（用户已认可并批准实装）
状态：初版实装完成，VK4-D-R1 修复后待验收「单色清屏画面」。

### 目标（最小单色清屏）
1. 黑屏 → 单色背景（clear 颜色 0.10/0.30/0.45/1.0 蓝）。✅ 实装
2. RenderPass + Framebuffer。✅ D1
3. CommandPool + CommandBuffer + 静态 clear 录制。✅ D2
4. Semaphore/Fence + AcquireNextImage → QueueSubmit → QueuePresent。✅ D3
5. Present 泵必须独立后台线程，不阻塞 UI 线程。✅ D3（Thread + IsBackground）
6. 引入薄 VulkanRenderSession 组合根，Bridge 只委托。✅ Session
7. Resize 只重建 Framebuffers（RenderPass/CP/CB/Sync 不动）。✅
8. OUT_OF_DATE/SUBOPTIMAL 当帧不强行 Present，交下次 Resize 重建。✅（loop continue）
9. Detach 顺序 ClearFrame → Swapchain → LogicalDevice → Surface → Instance。✅

### 严禁（均未触碰）
- 不做场景渲染 / 相机 / 网格 / 材质 / Gizmo / UI 叠加 / 持续动画。
- 不修改日志 UX（LOG-UX 成果保持）。
- 不把 RenderPass/CommandBuffer/PresentLoop 塞进 Bridge（Bridge 84 行仅委托）。
- 不让 Editor.UI 直接接触 Silk.NET.Vulkan 类型。

### 关键实现细节
- 每 Swapchain 图像一张 CommandBuffer，clear 命令录制一次；Resize 重建 Framebuffer 后重录。
- 单 in-flight 帧 + 单 Fence：`_submitted` 守卫避免首帧 WaitForFences 空等 1s。
- Acquire 用指针重载（`&idx`），仅 CommandBuffer[] 用 `fixed` 钉住；其余栈本地 `&` 直接取址。
- `KhrSwapchain` 方法名无 KHR 后缀（Silk.NET.Vulkan 2.22.0）：`AcquireNextImage` / `QueuePresent`。
- `SampleCountFlags.Count1Bit`、`ImageLayout.PresentSrcKhr`、`StructureType.PresentInfoKhr`、`ClearColorValue.Float32_0..3`。
- `using Semaphore = Silk.NET.Vulkan.Semaphore` 消除与 `System.Threading.Semaphore` 歧义。

### Commit
见交付报告（本 commit 哈希在回复中给出）。

## v0.2.5.1-vk Swapchain 生命周期规划（只规划不实装）(2026-07-08)
- 原历史编号：VK4-C-Plan

分支：fix/RZ-VK3-A-surface-contract
版本：VK4-C-Plan（仅文档，不写 Vulkan 实装代码）

### 背景
VK4-A / VK4-A-R1 / VK4-B / VK4-B-R1 全部完成，VK4-B 正式完全收口（Detach 顺序 `LogicalDevice → Surface → Instance` 已运行时验证）。LOG-UX-1 保留，LOG-UX-2 已回退删除。当前链路停在 LogicalDevice + Graphics/Present Queue，仍黑屏（Swapchain 未接）。

### 目标（只规划 Swapchain + Images + ImageViews）
1. VK4-C 只创建 `VkSwapchainKHR` + Swapchain Images + ImageViews。 ✅ 规划
2. 不创建 `RenderPass`。 ✅ 红线
3. 不创建 `Framebuffer`。 ✅ 红线
4. 不创建 `CommandPool` / `CommandBuffer`。 ✅ 红线
5. 不 `Clear`、不 `Present`。 ✅ 红线（仍黑屏为预期）
6. Resize 只重建 Swapchain + ImageViews。 ✅ 规划
7. Resize 不重建 Surface / Instance / LogicalDevice / Queue。 ✅ 红线
8. Dispose 顺序必须为 `ImageViews → Swapchain → LogicalDevice → Surface → Instance`。 ✅ 硬约束
9. Bridge 不再膨胀，Swapchain 进入独立 owner / attach step。 ✅ 约束（Bridge 已接近 100 行红线）
10. `VulkanDeviceOwner` 不增加职责。 ✅ 约束
11. 所有新增 .cs ≤100 行。 ✅ 约束
12. UI 不接触 `Silk.NET.Vulkan` 类型。 ✅ 约束
13. 不复制 `VulkanClearSession` 旧探针路径。 ✅ 约束

### 规划要点（详见 docs/rz-vk4-c-swapchain-plan.md）
- 新增 `XuanYu.Render.Vulkan/Swapchain/` 子目录（目标 3 文件）：`VulkanSwapchainCapabilities.cs`（查 caps/format/present mode/extent，输出纯数据）、`VulkanSwapchainOwner.cs`（建 Swapchain+Images+ImageViews，Dispose 先 ImageView 后 Swapchain）、`VulkanSwapchainLogFormatter.cs`（中文日志）。
- 新增 `Bridge/VulkanBridgeSwapchainAttachStep.cs`：在「选择 step → 设备 step」之后链式驱动 Swapchain 创建；前置失败只跳过、不崩。
- Attach 扩展：`... → LogicalDevice → Swapchain → ImageViews`；Detach 扩展：`ImageViews → Swapchain → LogicalDevice → Surface → Instance`。
- Resize：Bridge 现有 Resize 入口转发 `_swapchainOwner?.Recreate(newExtent)`，仅重建 Swapchain+ImageViews，跳过 0 尺寸 / 重复尺寸。

### 红线校验（本轮）
- 无代码改动；`git diff` 仅 `docs/rz-vk4-c-swapchain-plan.md`（新增）+ `changelog.md` + `file-tree.md`。
- 不构建（仅文档，依指令「如果动了代码则必须 build 0W0E」不适用）。

### 下一步
规划通过后开 `VK4-C`（Swapchain + ImageViews 实装）→ `VK4-C-R1`（Resize 重建 Swapchain 审计）→ `VK4-D`（ClearFrame 出画面）。**当前不进 VK4-C 实装。**

## v0.2.5.2-vk Swapchain + Images + ImageViews 实装（2026-07-08）
- 原历史编号：VK4-C

分支：fix/RZ-VK3-A-surface-contract
版本：VK4-C（Swapchain + Swapchain Images + ImageViews 实装，仍不出画面）｜状态：代码完成，待 VK4-C-R1 运行验证，未完全收口；VK4-D 暂缓。

### 背景
VK4-C-Plan 审计通过（只规划 Swapchain+Images+ImageViews，不建 RenderPass/Framebuffer/CommandPool/CommandBuffer、不 Clear/Present）。按用户拍板「开 VK4-C 实装，继续压边界」推进。当前链路停在 LogicalDevice + Graphics/Present Queue，仍黑屏。

### 目标（逐条对照规划）
1. 只创建 `VkSwapchainKHR` + Swapchain Images + ImageViews。 ✅
2. 不创建 `RenderPass` / `Framebuffer` / `CommandPool` / `CommandBuffer`。 ✅ 红线（grep 仅注释命中）
3. 不 `Clear`、不 `Present`。 ✅ 红线（仍黑屏为预期）
4. Resize 只重建 Swapchain + ImageViews。 ✅（`VulkanNativeHostSurfaceBridge.Resize` 转发 `_swapchainOwner?.Recreate(width,height)`）
5. Resize 不重建 Surface / Instance / LogicalDevice / Queue。 ✅
6. Dispose 顺序 `ImageViews → Swapchain → LogicalDevice → Surface → Instance`。 ✅（`VulkanNativeHostSurfaceBridge.Detach` 首行 `_swapchainOwner?.Dispose()`）
7. Bridge 不再膨胀：Swapchain 进独立 owner / attach step，Bridge 仅 98 行（接近 100 红线，未增 Swapchain 逻辑）。 ✅
8. `VulkanDeviceOwner` 不增加职责（仍仅 `CreateDevice` / `GetQueue` / `DisposeDevice`）。 ✅
9. 所有新增 .cs ≤100 行。 ✅（最大 Bridge 98）
10. UI 不接触 `Silk.NET.Vulkan` 类型（`XuanYu.Editor.UI` 零改动，仅随 Render.Vulkan 编译）。 ✅
11. 不复制 `VulkanClearSession` 旧探针路径。 ✅

### 新增文件（XuanYu.Render.Vulkan/Swapchain/，4 文件）
- `VulkanSwapchainCapabilities.cs`（80 行）：`Query` 查 Surface caps / formats / present modes / extent；`ChooseFormat` 优先 B8G8R8A8+SRGB、`ChoosePresentMode` 优先 MailboxKhr 否则 FifoKhr、`ChooseExtent` 处理 0/MaxValue；输出 `SwapchainCaps` record + `VulkanSwapchainCapabilitiesResult` record。
- `VulkanSwapchainBuilder.cs`（74 行）：`Build` 串 Query→CreateSwapchain→GetSwapchainImages→CreateImageViews；`CreateSwapchain` 用 `SwapchainCreateInfoKHR`（ColorAttachmentBit / OpaqueBitKhr / SpaceSrgbNonlinearKhr）；`CreateImagesAndViews` 循环建 `ImageViewCreateInfo`（ColorBit）并 `vk.CreateImageView`。
- `VulkanSwapchainOwner.cs`（77 行）：`Create(vk, instance, deviceOwner, surface, physicalDevice, width, height, log)` 经 `vk.TryGetDeviceExtension(instance, deviceOwner.LogicalDevice, out KhrSwapchain? khr)`；`Recreate(width,height)` 调 Builder 后 DestroyImagesAndViews 再赋值；`Dispose` 先 ImageView 后 Swapchain；不建 RenderPass 等。
- `VulkanSwapchainLogFormatter.cs`（13 行）：`Creating/Created(views)/Recreating/Recreated(w,h,views)/Disposed/Skipped/Failed` 中文格式器。

### 新增文件（XuanYu.Render.Vulkan/Bridge/，1 文件）
- `VulkanBridgeSwapchainAttachStep.cs`（32 行）：`Run(vk, instance, deviceOwner, surface, selection, width, height, log)` 在设备 step 后链式驱动 `VulkanSwapchainOwner.Create`；前置 null/Success 检查跳过。

### 改写文件
- `VulkanNativeHostSurfaceBridge.cs`（98 行）：新增 `using ...Swapchain`；字段 `_swapchainOwner`；`Attach` 串「选择→设备→Swapchain」；`Resize` 加 `_swapchainOwner?.Recreate(width,height)`；`Detach` 首行 `_swapchainOwner?.Dispose()`；`Emit` 改调 `VulkanBridgeLogFormatter.Emit(_log, message)`。
- `VulkanBridgeLogFormatter.cs`（35 行）：新增 `public static void Emit(Action<string>? log, string message) { log?.Invoke(message); Console.WriteLine(message); }`，原 Bridge 内联 Emit 逻辑迁出。

### 关键 API 坑（Silk.NET.Vulkan 2.22.0 真实成员名，经反射确认）
- `KhrSurface` 方法无 `KHR` 后缀：`GetPhysicalDeviceSurfaceCapabilities` / `GetPhysicalDeviceSurfaceFormats` / `GetPhysicalDeviceSurfacePresentModes`。
- 枚举成员均带 `_Khr` 或同义短名：`ImageUsageFlags.ColorAttachmentBit`、`ColorSpaceKHR.SpaceSrgbNonlinearKhr`、`PresentModeKHR.MailboxKhr` / `FifoKhr`、`CompositeAlphaFlagsKHR.OpaqueBitKhr`、`ImageAspectFlags.ColorBit`。
- `Vk.TryGetDeviceExtension<T>` 需 4 参数：`(Instance, Device, out T, string?)`。
- 弃用成员（`*_Khr` 旧名）改用非弃用短名，达成两项目 0W0E（0 警告 0 错误）。

### 验收
- `XuanYu.Render.Vulkan` 构建 0W0E；`XuanYu.Editor.UI` 构建 0W0E（零改动，仅随依赖编译）。
- 红线 grep：RenderPass / Framebuffer / CommandPool / CommandBuffer / Clear / Present 仅注释命中，无实装。
- 全部 .cs ≤100 行（最大 Bridge 98）。

### 下一步
- `VK4-C-R1`（Resize 重建 Swapchain 审计）：核对 `Recreate` 不重建 Surface/Instance/Device/Queue、`DestroyImagesAndViews` 顺序、异常路径资源不泄漏；严禁顺手推进 VK4-D。
- 用户真机运行时验证：启动→Swapchain 创建成功日志→Resize 重建→关闭 Detach 顺序（ImageViews→Swapchain→LogicalDevice→Surface→Instance）；仍黑屏为预期（ClearFrame 在 VK4-D）。

### Commit
见交付报告（本 commit 哈希在回复中给出）。

## v0.2.5.3-fix 启用 VK_KHR_swapchain + 0 尺寸跳过 + 格式暴露（2026-07-08）
- 原历史编号：VK4-C-Fix

分支：fix/RZ-VK3-A-surface-contract
版本：VK4-C 运行前置修正（非新渲染能力；仍不出画面；VK4-D 暂缓）

### 背景（审计发现）
用户审计 VK4-C 指出最大运行时风险：`VK_KHR_swapchain` 设备扩展可能未启用。静态核查 `VulkanDeviceOwner.Create` 确认 `DeviceCreateInfo` 未设置 `EnabledExtensionCount` / `PpEnabledExtensionNames` —— 编译过、扩展函数拿得到，但运行时 `CreateSwapchainKHR` 会失败。这是 VK4-C 必须补的运行缺口，非 VK4-D 问题。

### 修正（逐条）
1. `VulkanDeviceOwner.Create` 新增 `requiredDeviceExtension` 参数，创建 `DeviceCreateInfo` 时启用该设备扩展（`EnabledExtensionCount=1` + `PpEnabledExtensionNames` 指向 null 结尾的扩展名）。**扩展名由调用方传入**（当前 `VulkanSwapchainOwner.DeviceExtensionName = "VK_KHR_swapchain"`），DeviceOwner 不自带 swapchain 知识，守住「DeviceOwner 不增 Swapchain 职责」。DeviceOwner 96→99 行（≤100）。
2. 扩展名穿程：`VulkanSwapchainOwner.DeviceExtensionName` → `VulkanBridgeDeviceAttachStep.Run(..., requiredDeviceExtension)` → `VulkanDeviceOwner.Create`；`VulkanNativeHostSurfaceBridge.Attach` 调设备 step 时传入。`Bridge` 98 行不变（仅多一个实参）。
3. `VulkanSwapchainOwner.Recreate` 新增 0 尺寸跳过：`width<=0 || height<=0` 时记 `Skipped` 日志并 return，不重建、不崩溃（R1 #8）。初始 `Create` 不改（启动窗口必有有效尺寸；`ChooseExtent` 已夹取下限）。
4. `VulkanSwapchainOwner` 暴露只读信息：`Format` / `Extent` / `ImageViews`（ReadOnlySpan<ImageView>），供 VK4-D 建 RenderPass/Framebuffer 直接使用，免反查。Owner 77→86 行（≤100）。`VulkanSwapchainBuilder.Build` 返回元组追加 `Format` / `Extent`（74→74，未增行）。

### 红线校验
- 未建 RenderPass / Framebuffer / CommandPool / CommandBuffer、未 Clear / Present（仍黑屏为预期）。✅
- UI 零改动；`XuanYu.Editor.UI` 仅随依赖编译。✅
- 全 .cs ≤100（最大 DeviceOwner 99）。✅
- 两项目 0W0E。✅
- 仍非「完全收口」：上述为代码修正，运行时需 VK4-C-R1 真机验证（Swapchain 创建成功 / Resize 重建 / Detach 顺序）。

### 下一步
- `VK4-C-R1`：运行验证（见 `docs/rz-vk4-c-r1-audit-plan.md`），拿真机日志确认三项；严禁进 VK4-D。
- `VK4-D`：待 R1 通过后，才出画面（ClearFrame）。

### Commit
见交付报告（本 commit 哈希在回复中给出）。

## v0.2.5.4-fix Swapchain 重建（OldSwapchain）修复与二次运行验证（2026-07-08 晚）
- 原历史编号：VK4-C-R1
- 性质：VK4-C-R1 首次真机运行发现 Resize 重建运行时失败，仅修复 Swapchain 重建路径、不新增渲染能力；不进 VK4-D。
- 运行结果（用户真机，RTX 3050 4GB Laptop GPU）：
  - ✅ 第一项 首次 Swapchain 创建成功（`Swapchain 创建成功；ImageView 创建成功 3 张`）—— 证明 VK4-C-Fix 启用的 `VK_KHR_swapchain` 设备扩展已生效（编译过≠能建，现确证能建）。
  - ❌ 第二项 Resize 重建失败：两次 `CreateSwapchain 失败：ErrorNativeWindowInUseKhr`（713x549 与 713x188）；Resize 红线正确（日志 `不重建 Surface`）。
  - ⚠️ 第三项 Detach 顺序本次日志未含关闭事件，未验证。
- 根因：`VulkanSwapchainOwner.Recreate` 调 `Build` 建新 Swapchain 时旧 Swapchain 仍存在，且 `VulkanSwapchainBuilder.CreateSwapchain` 的 `SwapchainCreateInfoKHR` **未设置 `OldSwapchain`** → 驱动判窗口被旧 Swapchain 占用 → `VK_ERROR_NATIVE_WINDOW_IN_USE_KHR`。
- 修复：
  - `Build` 新增 `SwapchainKHR oldSwapchain = default` 参数并透传给 `CreateSwapchain`。
  - `CreateSwapchain` 设置 `info.OldSwapchain = oldSwapchain`（首次创建传 default=0，重建传 `_swapchain`）。
  - `Recreate` 调 `Build(..., _log, _swapchain)` 把当前 Swapchain 作为旧句柄传入；新建成功后再 `DestroyImagesAndViews` 退役旧 Swapchain（顺序：先 ImageView 后旧 Swapchain）。
- 红线校验：未新增 RenderPass/Framebuffer/CommandPool/CommandBuffer、未 Clear/Present；UI 零改动；行数 `VulkanSwapchainBuilder` 74→75、`VulkanSwapchainOwner` 86 不变，全 ≤100；双项目低内存构建 0W0E（Editor.UI 首次因 PID 8636 旧编辑器锁 bin 拷贝失败，taskkill 释放后重建通过）。
- 状态：**VK4-C 待二次 R1 验证（Resize 重建成功 + Detach 顺序正确）**，未完全收口；VK4-D 暂缓。
- 下一步：用户重跑编辑器，核对 Resize 后 `Swapchain 创建成功；ImageView 创建成功 N 张` + 关闭后 Detach 顺序 `ImageViews → Swapchain → LogicalDevice → Surface → Instance`；全过则 VK4-C 收口、开 VK4-D。
- 可视化：`docs/vk4-c-r1-swapchain-fix.svg`（修复前后 Swapchain 重建对比图：修复前未设 `OldSwapchain` → `ErrorNativeWindowInUseKhr`；修复后传当前 Swapchain 作旧句柄 → 创建成功后再退役旧 Swapchain）。

### Commit
见交付报告（本 commit 哈希在回复中给出）。

## v0.2.7.5-fix 日志系统三修：自动滚动根因修复 + 种子清理 + 控制台去重（2026-07-09）
- 原历史编号：LOG-UX-1-R4

分支：fix/RZ-VK3-A-surface-contract
版本：LOG-UX-1-R4（用户指令称 LOG-UX-1-R2，但 R2/R3 已被占用，顺延 R4）。仅改 UI shell + 低层 Vulkan Log 辅助（去 Console.WriteLine）；不碰 Vulkan 生命周期行为 / NativeHost / Swapchain 逻辑；不进 VK4-D。

### 一、自动滚动根因修复（前两次 R2/R3 仍失效）
- **根因（确证）**：R3 在 `TryHook`（`AttachedToVisualTree`/`DataContextChanged`）时 ListBox 模板尚未应用，`FindDescendantOfType<ScrollViewer>()` 返回 null 且不再重试 → `_logScroll` 永远为 null → `OnVmPropertyChanged` 每次 `if (_logScroll is null) return` 直接退出 → 滚动完全死。
- **修复**：
  - `LogList.TemplateApplied` 事件 + `Dispatcher.InvokeAsync(ResolveScrollViewer, Loaded)` 延迟重试，确保拿到内部 ScrollViewer 后才挂 `ScrollChanged` 并首次 `ScrollToTail`。
  - 新日志进入：`ResolveScrollViewer()` 兜底补解析 → `Dispatcher.InvokeAsync(ScrollToTail, Render)`，布局完成后 `ScrollToEnd()` 直接控 Offset 到底部。
  - 跟随态判定不变：`ScrollChanged` 仅当用户主动滚动（`|OffsetDelta.Y|>=0.5`）才重算 `_followTail`，Extent 增长不误判。上翻暂停、回底恢复。
- 代码：`Foot.axaml.cs` 91→98 行（注释精简，仍 <100）。

### 二、清理 21:32 示例/种子日志
- **现象**：日志面板混入 `编辑器布局已恢复`/`已打开项目：SampleProject`/`Vulkan Surface 生命周期已接入；Device / Swapchain 尚未接入`/`构建队列空闲`/`点击拾取未命中任何对象`/`资源导入队列为空`，且「Device / Swapchain 尚未接入」已过期（现已接入）。
- **来源**：`UiVm.Logging.InitLogs` 调 `_logBuffer.Seed(SampleLogEntries.All)` + 3 条 `_logBus.Info` 种子。
- **修复**：删除 `_logBuffer.Seed(...)` 与 3 条种子 `_logBus.Info`；空状态由 UI「暂无日志」占位呈现。`SampleLogEntries.cs` 数据类保留（无引用，无害）。
- 效果：启动后日志面板从真实 Vulkan 生命周期日志起，无假数据污染审计。

### 三、控制台 Vulkan 日志去重
- **现象**：`AttachConsole(-1)` 生效后终端每条 Vulkan 日志出现两遍。
- **根因（确认）**：低层 Vulkan `Log(log, m)` 辅助在 `log?.Invoke(m)` 之外**又各自 `Console.WriteLine(m)`**；而 `log` 就是 Bridge 的 `Emit` → `VulkanBridgeLogFormatter.Emit` 本身已 `Console.WriteLine` → 双写。同样问题在 `VulkanPhysicalDeviceSelector` 2 处内联、`VulkanBridge{Device,PhysicalDevice,Swapchain}AttachStep` 共 5 处。
- **修复（统一单出口）**：删除所有低层 `Console.WriteLine`，仅保留 `VulkanBridgeLogFormatter.Emit` 内的唯一 `Console.WriteLine` 作为控制台单出口。
- **未动**：`VulkanInstanceOwner`/`VulkanSurfaceOwner` 仅直接 `Console.WriteLine`（不走 `Emit`），终端已单现，不重复，保持。
- 效果：终端每条 Vulkan 生命周期日志仅一次；UI 面板仍正常一次。

### 红线校验
- `Foot.axaml.cs` 98 行 <100 ✅；`UiVm.Logging.cs` 100 行未动（仅删种子调用）✅；5 低层文件仅删 1 行 Console.WriteLine、AttachStep 删 5 处，全 ≤100 ✅。
- 不碰 Render.Vulkan 生命周期行为 / NativeHost Attach·Resize·Detach / Swapchain 创建·重建·释放 ✅。
- 双项目低内存构建 0W0E ✅。

### 下一步
- 用户重跑编辑器：① 日志面板自动滚到最新（不再卡在旧种子位置）；② 面板无 21:32 假日志；③ 终端每条 Vulkan 日志仅一次。
- 全过 → VK4-C 日志链路收口，开 VK4-D 出画面。

### Commit
见交付报告（本 commit 哈希在回复中给出）。

## v0.2.7.6-fix 止血：彻底禁用日志自动滚动（2026-07-09）
- 原历史编号：LOG-UX-1-R5A

分支：fix/RZ-VK3-A-surface-contract
版本：LOG-UX-1-R5A（用户称 R5 后仍「未响应」，指令止血而非继续叠补丁）。

### 现象
- 用户 run.bat 贴截图：编辑器窗口标题「**玄域编辑器（未响应）**」，终端日志已跑到 `【VulkanSwapchain】Swapchain 创建成功；ImageView 创建成功 3 张`。
- 结论：**Vulkan 主链路全过（Instance✅ Surface✅ PhysicalDevice✅ LogicalDevice✅ Queue✅ Swapchain✅ ImageView✅）**，未响应发生在 Editor.UI 层，系 UI 线程卡死。截图黑色大块为 Windows DWM 未响应残影，非代码 bug。

### 判断与决策
- 自动滚动状态机（TemplateApplied 解析 ScrollViewer + ScrollChanged 跟随 + Dispatcher 自动 ScrollToTail）在 Vulkan `Attach`（UI 线程同步执行 ~25 条日志）期间触发视觉树遍历 / Dispatcher 堆积 → UI 线程卡死。
- 用户明确：**不要在 `Foot.axaml.cs` 继续叠自动滚动补丁**。改做止血——禁用自动滚动，保留其余 LOG-UX 成果。

### 保留（不受影响）
- 控制台日志去重（R4 单出口）；种子日志清理（R4）；Ctrl+A/Ctrl+C 多行复制；详情换行；AttachConsole（-1）。

### 禁用 / 移除
- `Foot.axaml.cs` 全部自动滚动逻辑：ResolveScrollViewer / HookVm / OnVmPropertyChanged / OnScrollChanged / ScrollToTail / TemplateApplied 订阅 全部删除。
- `Foot.axaml.cs` 由 96 行精简至 **42 行**（仅保留 SelectionChanged / KeyDown 复制逻辑）。

### 红线校验
- `Foot.axaml.cs` 42 行 <100 ✅；不碰 Render.Vulkan / NativeHost / Swapchain 创建·Resize·释放 ✅；双项目低内存构建 0W0E ✅。

### 后续
- 自动滚动由 **LOG-UX-2** 重新设计：拆出 `Foot/LogListAutoScrollController.cs`，`Foot.axaml.cs` 只创建 controller + 交 ListBox + 通知新日志；controller 内部节流（已安排滚动则不重复安排，等布局完成只滚一次），避免 Dispatcher/ScrollChanged 套娃。
- 当前阶段：VK4-C Vulkan 链路通过；VK4-D 暂停；先稳定编辑器，再重开自动滚动设计。

### Commit
`8407657`（已推送 origin fix/RZ-VK3-A-surface-contract）。

---

## v0.2.7.8-rz 自动滚动重设计：独立控制器（2026-07-09）
- 原历史编号：LOG-UX-2

分支：fix/RZ-VK3-A-surface-contract
版本：LOG-UX-2（R5A 止血后，按「独立 controller + 节流 + 防重入 + 不碰 Vulkan」方案重做）。

### 背景
R5A 已禁用自动滚动、编辑器恢复稳定。本轮把自动滚动按用户给定方案重做，但**禁止再把状态机塞进 `Foot.axaml.cs`**。

### 设计
- 新增 `XuanYu.Editor.UI/Foot/LogListAutoScrollController.cs`（74 行）：独立控制器，职责只有「控制日志 ListBox 的自动滚动」。
- `Foot.axaml.cs`（64 行）只做接线：创建 controller、SelectionChanged 详情选中、Ctrl+A/Ctrl+C 复制、Unloaded 时 Dispose controller。

### 关键防卡死机制
1. **单次解析**：`Resolve()` 用 `_resolved` 守卫，仅 `TemplateApplied` 后 `FindDescendantOfType<ScrollViewer>()` 一次；模板未就绪则静默等待，**不每条日志遍历视觉树**。
2. **节流**：`OnLogItemsChanged` 用 `_pendingScroll` 标志，连续多条日志只排一次 `Dispatcher.UIThread.InvokeAsync(ScrollToTail, Render)`。
3. **防重入**：`_isProgrammaticScroll` 标志，程序滚动期间 `ScrollChanged` 直接 return，不重算跟随态，避开 ScrollChanged↔ScrollToEnd 套娃。
4. **不阻塞 UI 线程**：`OnVmPropertyChanged` 只调 `_autoScroll.OnLogItemsChanged()` 做布尔判定；Vulkan `Attach`（UI 线程同步 ~25 条日志）期间 `_scroll` 尚未解析 → 直接返回，**零视觉树遍历**。
5. **跟随态**：`_followTail` 用户上翻（>12px 容差）置 false 暂停；滚回底部恢复；用户在看历史时不强制拉回。

### 红线校验
- `Foot.axaml.cs` 64 行、`LogListAutoScrollController.cs` 74 行，均 ≤100 ✅。
- 不碰 `Render.Vulkan` / `NativeHost` / `Swapchain` 生命周期 / `UiVm.Logging.cs` ✅。
- 保留：控制台去重、种子清理、Ctrl+A/Ctrl+C、详情换行、AttachConsole ✅。
- 双项目低内存构建 **0W0E** ✅。

### 验收（待用户 run.bat 真机验证）
- 编辑器稳定启动，不再「未响应」；Vulkan 链路仍到 Swapchain+ImageView。
- 新日志自动滚到底；用户上翻不被强制拉回；滚回底部恢复跟随。
- Ctrl+A/Ctrl+C、详情换行、控制台 Vulkan 日志单次、无种子日志 均保持。

### Commit
见交付报告（本 commit 哈希在回复中给出）。

---

## v0.2.7.9-vk 正式收口 + VK4-D-Plan 启动（2026-07-09）
- 原历史编号：VK4-C / LOG-UX-2

分支：fix/RZ-VK3-A-surface-contract
文档：docs/rz-vk4-d-plan.md（新建）

### 收口确认（用户真机验证）
- **VK4-C 正式收口**：Instance→Surface→PhysicalDevice→LogicalDevice→Queue→Swapchain→ImageView 全链路通过；VK4-C-R1 Resize 重建（旧 Swapchain 句柄修复 `ErrorNativeWindowInUseKHR`）与 Detach 逆序释放（Swapchain→LogicalDevice→Surface→Instance）均运行时验证。
- **LOG-UX-2 正式收口**：独立 `Foot/LogListAutoScrollController.cs`（74 行）真机通过——新日志自动滚到底、上翻不禁、回底恢复；`Foot.axaml.cs`（64 行）只做接线。
- **控制台日志单出口去重 / 旧 21:32 种子假日志清理**：均保持无回归。
- LOG-UX-2 收口 Commit：`a7149f6`（已推送 origin）。

### 三问题最终归因（用户总结，作防回潮基线）
| 问题 | 根因 | 处理结果 |
|---|---|---|
| UI 未响应 | 自动滚动逻辑堆在 `Foot.axaml.cs`，视觉树查找 + Dispatcher + ScrollChanged 套娃 | ✅ 拆出 `LogListAutoScrollController` |
| 控制台日志重复 | 多处低层 `Console.WriteLine` 与统一 `Emit` 双出口 | ✅ 保留单出口 |
| 21:32 假日志 | 示例/种子日志混入真实运行日志 | ✅ 清理 |

### 下一阶段：VK4-D-Plan
- 文档 `docs/rz-vk4-d-plan.md` 已落定：目标 **最小 Clear + Present 单色清屏闭环**。
- VK4-D 首次真正涉及：RenderPass / Framebuffer / CommandPool / CommandBuffer / Semaphore / Fence / AcquireNextImage / QueueSubmit / QueuePresent。
- **红线（写死）**：只做最小清屏闭环；不做场景渲染 / 相机 / 网格 / 材质 / Gizmo / UI 叠加 / 持续动画。
- **边界**：Resize 只重建 Framebuffers（RenderPass/CP/CB/Sync 不动）；Detach 顺序 ClearFrame→Swapchain→Device→Surface→Instance；Present 泵独立线程，禁在 UI 线程。
- 为守住 `VulkanNativeHostSurfaceBridge` ≤100 红线与契约优先设计，VK4-D 实装时顺带引入薄组合根 `VulkanRenderSession`（原 VK4-E 范围），Bridge 委托给它。
- 当前阶段：VK4-A/B/C 完成；LOG-UX-1/2 收口；**进入 VK4-D-Plan**，不回头补日志功能。

## v0.2.7.4-fix 自动滚动修复 + WinExe 控制台输出（2026-07-09）
- 原历史编号：LOG-UX-1-R3

分支：fix/RZ-VK3-A-surface-contract
版本：LOG-UX-1-R3（仅 UI 改动 + Program.cs 一行 P/Invoke；不碰 Vulkan / Render.Vulkan / 日志数据模型）

### 性质
双修复：① R2 自动滚动未生效的根因修复；② WinExe 进程 Console.WriteLine 不显示在父终端的问题。

### 问题 1：R2 自动滚动不生效
- **现象**：用户真机验证 Foot 面板日志不自动滚到底。
- **根因**：R2 用 `LayoutUpdated` 事件触发 `ScrollToEnd`，但 Avalonia 的 `LayoutUpdated` 触发时机与 `PropertyChanged(LogItems)` 的时序不可靠——新日志写入 buffer → `RefreshLogBindings()` → `OnPropertyChanged(LogItems)` → 设置 `_pendingScroll=true`，但 `LayoutUpdated` 可能在设置前已触发过、或 ListBox 虚拟化延迟导致 LayoutUpdated 不再为本次变更触发。
- **修复**：改用 `Dispatcher.InvokeAsync(ScrollToTail, DispatcherPriority.Render)`——将滚动操作显式放入 dispatcher 队列的 Render 优先级，确保 Avalonia 布局完成后再执行，比事件驱动更可靠。
- **代码**：`Foot.axaml.cs` 重写（89→91 行，去掉了 R2 的自定义 `DispatcherTimerExt` 辅助类）。

### 问题 2：WinExe 控制台无输出
- **现象**：用户运行 `dotnet run` 后终端只显示 build 输出，所有 Vulkan 生命周期日志（Console.WriteLine）不出现。
- **根因**：`XuanYu.Editor.UI.csproj` 的 `<OutputType>WinExe</OutputType>`。Windows 上 WinExe 进程不继承父控制台句柄，`Console.WriteLine` 写入虚空。
- **影响范围**：Vulkan 代码已有 6 处 `Console.WriteLine`（BridgeLogFormatter.Emit / DeviceOwner.Log / SwapchainOwner.Log / Capabilities.Log / Builder.Log / Selector.Log），全部因 WinExe 无效。
- **修复**：`Program.cs:Main()` 首行调用 `AttachConsole(-1)`（ATTACH_PARENT_PROCESS），使 WinExe 进程继承 `dotnet run` 父终端。零改动 Vulkan 代码。
- **效果**：关闭编辑器后，终端窗口仍显示完整 Detach 释放顺序（ImageViews→Swapchain→LogicalDevice→Surface→Instance），直接解决 T6 审计问题。

### 红线校验
- `Foot.axaml.cs` 91 行 <100 ✅；`Program.cs` 28 行 ✅。
- UiVm.Logging.cs 保持 100 行未动 ✅。
- 不碰 Vulkan / Render.Vulkan / NativeHost / 日志数据模型 ✅。
- Editor.UI 构建 0W0E ✅。

### 下一步
- 用户重跑编辑器，验证两项：
  1. 启动后日志面板自动滚到最新（不再需手动拖到底）；
  2. 关闭编辑器后，`dotnet run` 终端显示 Detach 释放序列（AttachConsole 使 Console.WriteLine 生效）；
- 两项全过 → VK4-C 正式收口（T6 拿到证据），开 VK4-D 出画面。

### Commit
见交付报告（本 commit 哈希在回复中给出）。

## v0.2.7.3-fix 日志面板自动滚动到最新 (2026-07-09)
- 原历史编号：LOG-UX-1-R2

分支：fix/RZ-VK3-A-surface-contract
版本：LOG-UX-1-R2（仅 UI 改动，服务 VK4-C-R1 审计；不碰 Vulkan / Render.Vulkan / NativeHost 生命周期 / 日志数据模型）
注：用户指令称本轮为 LOG-UX-1-R1，但 changelog 中 LOG-UX-1-R1 已被 Ctrl+C 复制修复占用，按命名顺延为本节 R2。

### 性质
VK4-C-R1 二次运行已验证 Resize 重建 Swapchain 通过，但审计 Vulkan 生命周期时每次 Resize/Attach/Detach 都要手动把日志拖到底，影响可用性。本轮只补日志面板自动滚动，不新增渲染能力、不进 VK4-D。

### 目标（逐条对照）
1. 新日志进入且用户在底部时，自动滚动到最新日志 — ✅
2. 用户手动上翻历史时不强制拉回底部（暂停跟随）— ✅
3. 用户再滚到底部时恢复自动跟随 — ✅
4. `Ctrl+A` / `Ctrl+C` 多选复制不受影响 — ✅ 沿用 LOG-UX-1-R1 的 `LogList_KeyDown`
5. 多选日志不强制跳动、不破坏选择 — ✅ 滚动只改视图偏移，不触动 `SelectedItems`
6. 不修改 Vulkan 代码 — ✅ 仅 `XuanYu.Editor.UI`
7. 不修改 NativeHost 生命周期 — ✅
8. 不修改 Render.Vulkan 项目 — ✅ `git diff` 仅 `Foot.axaml.cs`
9. 不修改日志数据模型（EditorLogBuffer / EditorLogBus）— ✅
10. 所有 .cs ≤100 行 — ✅ `Foot.axaml.cs` 36→89
11. `XuanYu.Editor.UI` 构建 0W0E — ✅

### 实现
- `Foot.axaml.cs`：
  - 构造里 `AttachedToVisualTree` / `DataContextChanged` 双重挂接 `TryHook`（含重复订阅防护）。
  - `TryHook`：用 `LogList.FindDescendantOfType<ScrollViewer>()` 取内部 `ScrollViewer`；订阅其 `ScrollChanged` 与 `LogList.LayoutUpdated`；订阅 `UiVm.PropertyChanged` 仅在 `LogItems` 变化时置 `_pendingScroll = _followTail`。
  - 跟随判定 `LogScroll_OnScrollChanged`：仅当 `Math.Abs(e.OffsetDelta.Y) >= 0.5`（用户主动滚动）才重算 `_followTail`，`Offset.Y + Viewport.Height >= Extent.Height - 2.0` 视为在底部；Extent 增长（新日志）不误判，规避「用户本在底部却被误停跟随」的经典竞态。
  - 实际滚动放 `LogList_OnLayoutUpdated`：布局完成后再 `ScrollToEnd()`，确保新项已测量；`_pendingScroll` 一次性标志，防每帧空滚。
  - 首次附着若 `_followTail` 为真，立即 `_pendingScroll = true` 对齐到底部。
- 未触碰 `UiVm.Logging.cs`（已 100 行，遵守「不往里塞逻辑」红线）、`Foot.axaml`、`EditorLogBuffer/EditorLogBus`。

### 验收（用户重测）
- `run.bat` 启动编辑器 → 日志随新事件自动滚到最底；向上翻看历史时不再被拽回底部；滚回底部后恢复跟随。
- 关闭编辑器，确认底部自动跟到 Detach 释放顺序日志（T6 证据）：
  `【VulkanSwapchain】Swapchain 释放成功` → `【VulkanDevice】LogicalDevice 释放成功` → `【VulkanBridge】Surface 已释放` → `【VulkanBridge】Instance 已销毁` → `【VulkanBridge】分离完成：Surface + Instance 已释放`。
  （注：`VulkanSwapchainOwner.Dispose` 内部 `DestroyImagesAndViews` 先 ImageView 后 Swapchain，与 `Swapchain 释放成功` 单次日志合并，顺序正确；无需另开 Vulkan 改动轮。）

### 红线校验
- `git diff` 仅 `XuanYu.Editor.UI/Foot/Foot.axaml.cs`；`UiVm.Logging.cs` 保持 100 行未动、`Foot.axaml` 95 行未动；未改 `Render.Vulkan` / NativeHost。
- `Foot.axaml.cs` 36→89 行，<100。
- 可视化：`docs/log-ux-1-r2-autoscroll.svg`（跟随状态机 FOLLOW/PAUSED + 滚动时序 LogItems→LayoutUpdated→ScrollToEnd）。

### Commit
见交付报告（本 commit 哈希在回复中给出）。

## v0.2.7.7-rz 会话日志落盘（关闭后仍可审计 Detach 顺序）(2026-07-08)
- 原历史编号：LOG-UX-2

分支：fix/RZ-VK3-A-surface-contract
版本：LOG-UX-2（仅 Editor.UI 日志系统，不碰 Vulkan / NativeHost）

### 背景
VK4-B-R1 最后一项需验证关闭窗口时 Detach 释放顺序（LogicalDevice → Surface → Instance）。
但原日志只进 UI 内存 `EditorLogBuffer`，关闭窗口后面板消失无法复制。
方案 A（看控制台）经代码核查不成立：`EditorLogBus` 仅 `buffer.Add(...)`，无 `Console`/`File`/`Trace` 输出，控制台不会出现 Vulkan 生命周期日志。故按「A 不行就 B」决策树开 LOG-UX-2 落盘。

### 目标（逐条对照）
1. UI 日志照常显示，同时同步追加写入 `logs/editor-session-latest.log` — ✅
2. 不碰 Vulkan 代码 — ✅ 仅在 `EditorLogBus.Write` 加文件追加
3. 不碰 NativeHost 生命周期 — ✅
4. 所有 .cs ≤100 行 — ✅ `EditorLogBus.cs` 21→44
5. `XuanYu.Editor.UI` 0W0E — ✅
6. `logs/` 加入 `.gitignore` 避免入仓 — ✅

### 实现
- `EditorLogBus.cs`：新增 `_logDir`/`_logPath`（相对 `Environment.CurrentDirectory/logs`）；首次写时 `Directory.CreateDirectory` + 写会话头；之后每条日志 `File.AppendAllText`，格式与剪贴板一致（`时间\t级别\t来源\t分类\t消息\t详情`）。包 `try/catch`，落盘失败不阻塞 UI（诊断安全约定）。
- 每次启动重建 `editor-session-latest.log`（只保留最近一次会话），便于关闭后直接打开审计。

### 验收（用户重测）
- `run.bat` 启动编辑器 → 正常操作 → 关闭 → 打开 `logs/editor-session-latest.log`。
- grep `LogicalDevice 释放成功` / `Surface 已销毁` / `Instance 已销毁`，确认顺序为 Device → Surface → Instance。
- 至此 VK4-B-R1 最后一项（第⑪项）可从文件可靠审计，VK4-B 即可完全收口。

### 红线校验
- `git diff` 仅 `XuanYu.Editor.UI/Vm/Logging/EditorLogBus.cs` 与 `.gitignore`；未改 `XuanYu.Render.Vulkan` / `VulkanNativeHostSurfaceBridge` / NativeHost。

### 回退（2026-07-08 收尾）
LOG-UX-2 仅为临时调试手段，用于关闭窗口后从文件审计 Detach 顺序。VK4-B-R1 第⑪项已通过文件日志确认顺序为 `LogicalDevice 释放成功 → Surface 已释放 → Instance 已销毁`（Device→Surface→Instance），VK4-B 完全收口后，按用户要求删除该落盘功能：
- `EditorLogBus.cs` 还原为纯内存版（44→21 行，移除 `System.IO` 依赖与文件追加）。
- `.gitignore` 移除 `logs/` 条目。
- 磁盘 `logs/` 目录已删除。
- 不碰 Vulkan / NativeHost；`XuanYu.Editor.UI` 构建 0W0E。

## v0.2.7.2-fix Ctrl+C 复制无响应修复 (2026-07-08)
- 原历史编号：LOG-UX-1-R1

分支：fix/RZ-VK3-A-surface-contract
版本：LOG-UX-1-R1（对 LOG-UX-1 的缺陷修复，仅 Editor.UI）

### 根因
LOG-UX-1 在 `Foot.axaml` 用 `KeyUp` 事件 + `e.KeyModifiers.HasFlag(KeyModifiers.Control)` 判断快捷键。
Avalonia 中 `KeyUp` 的 `KeyModifiers` 反映「松开该键那一刻」的按键状态；用户按 `Ctrl+C` 后若先松开 Ctrl，则 `KeyUp(C)` 时 `KeyModifiers` 不再含 `Control`，`Ctrl+C` 分支条件不满足被跳过 → 表现为按了没反应。
原 `async void` + `await SetTextAsync` 也无异常兜底，剪贴板失败静默消失。

### 修复
1. `Foot.axaml`：`KeyUp="LogList_KeyUp"` → `KeyDown="LogList_KeyDown"`（按下 C 瞬间 Ctrl 必仍按着，检测稳定）。
2. `Foot.axaml.cs`：`LogList_KeyDown` 用 `KeyDown`；`Ctrl+A`→`SelectAll()`，`Ctrl+C`→`TopLevel.Clipboard.SetTextAsync(...)` 且包 `try/catch`，失败仅 `Debug.WriteLine` 不崩。
3. `UiVm.Logging.cs`：新增 `NotifyLogCopied()`（当前 96 行，未突破 100 红线）；复制成功后写一条「已复制 N 条日志到剪贴板」信息日志，提供可见反馈。
4. 为腾出空间，把 11 行 `RefreshLogBindings` 压成单行（行为不变）。

### 验收
- `XuanYu.Editor.UI` 构建 0W0E。
- `Foot.axaml.cs` 36 行 / `UiVm.Logging.cs` 96 行 / `Foot.axaml` 95 行，均 ≤100。
- 未碰 `Render.Vulkan` / NativeHost / Vulkan 链路。
- 用户重测：日志面板选中若干行 → `Ctrl+C` → 面板出现「已复制 N 条日志到剪贴板」→ 粘贴到记事本为纯文本表格。

## v0.2.7.1-fix 日志多选复制与详情换行修复 (2026-07-08)
- 原历史编号：LOG-UX-1

分支：fix/RZ-VK3-A-surface-contract
版本：LOG-UX-1（仅 UI 改动，不进入 VK4-C，不碰 Vulkan 链路）

### 背景与目标
VK4-B-R1 审计时需要把完整 Vulkan 生命周期日志贴回对话，但旧日志面板只能单行查看、详情被横向截断、无法整段复制。
用户拍板插一轮极小 `LOG-UX-1`：只修日志面板交互，服务 VK4-B-R1 审计，不修改 Vulkan 渲染链路 / NativeHost 生命周期 / Render.Vulkan。

### 目标（逐条对照）
1. 日志列表支持多选 — ✅ `SelectionMode` 改为 `Multiple`（Avalonia 12 已含 Shift 范围选择 + Ctrl 切换，等价于旧 `Extended`）。
2. Shift + 单击范围多选 — ✅ 由 Avalonia `Multiple` 原生支持。
3. Ctrl + 单击追加/取消选择 — ✅ 由 Avalonia `Multiple` 原生支持。
4. Ctrl + A 选择当前筛选结果中的全部日志 — ✅ `Foot.axaml.cs` 处理 `KeyUp` 调 `ListBox.SelectAll()`（= 当前 `ItemsSource` 即筛选结果）。
5. Ctrl + C 复制当前选中的日志 — ✅ `KeyUp` 调 `TopLevel.Clipboard.SetTextAsync(SelectedEntriesClipboardText)`。
6. 复制格式为纯文本表格：表头 `时间\t级别\t来源\t分类\t消息\t详情`，每行一条，详情不截断 — ✅ `EditorLogClipboardText.FromMany`。
7. 右侧「日志详情」的消息与详情自动换行、不横向截断 — ✅ `LogDetailPanel.axaml` 的 `detailBody` 样式补 `TextWrapping=Wrap` + `AcceptsReturn=True`。
8. 右侧详情文本可复制 — ✅ 详情用只读 `TextBox`，默认可选中复制。
9. 保留「复制详情」按钮 — ✅ 未改动其逻辑（单条 `EditorLogClipboardText.From`）。
10. 仅日志区域可复制/可选择 — ✅ 改动仅限 `Foot.axaml` / `LogDetailPanel.axaml`；项目树 / Inspector / 按钮 / 普通标签均为 `TextBlock`，未引入可选文本。
11. 不修改 Vulkan 代码 — ✅ 仅 `Editor.UI`。
12. 不修改 NativeHost 生命周期 — ✅。
13. 不修改 Render.Vulkan 项目 — ✅ `git diff` 仅 `XuanYu.Editor.UI`。
14. 所有 .cs 文件 ≤100 行 — ✅ `Foot.axaml.cs` 34 / `UiVm.Logging.cs` 100 / `EditorLogClipboardText.cs` 25。
15. `dotnet build XuanYu.Editor.UI` 0W0E — ✅。
16. 更新 changelog.md — ✅ 本节；无新增/移动文件，故 file-tree.md 不更新。

### 技术注记（Avalonia 12 陷阱）
- 本机 Avalonia 为 **12.0.4**，其 `Avalonia.Controls.SelectionMode` 枚举**没有 `Extended` 成员**（旧版才有）。
  旧 `Extended`（单击选一行 / Shift 范围 / Ctrl 切换）语义在 12.0.4 由 `SelectionMode.Multiple` 提供（含 Shift 范围 + Ctrl 切换）。
  XAML 中 `SelectionMode="Extended"` 会触发 AVLN3000（字符串无法转枚举）。须用 `SelectionMode="{x:Static av:SelectionMode.Multiple}"`，
  并在根元素加 `xmlns:av="using:Avalonia.Controls"`。
- `Multiple` 模式下「普通单击」是**切换该行**（而非「只选这一行」）；但详情面板仍按最后点击项显示，审计复制（Shift 范围 / Ctrl+A）不受影响。

### 改动文件
- `XuanYu.Editor.UI/Foot/Foot.axaml`（95 行）：ListBox 加 `x:Name="LogList"`、`SelectionMode=Multiple(x:Static)`、`SelectionChanged`/`KeyUp` 事件接线；根加 `av` 命名空间。
- `XuanYu.Editor.UI/Foot/Foot.axaml.cs`（34 行，+27→34）：`LogList_SelectionChanged` 把选中 `LogEntry[]` 推给 VM；`LogList_KeyUp` 处理 `Ctrl+A`(SelectAll) 与 `Ctrl+C`(写剪贴板)。
- `XuanYu.Editor.UI/Vm/UiVm.Logging.cs`（88→100 行）：新增 `_selectedEntries` 字段、`SetSelectedEntries(...)`、`HasSelectedEntries`、`SelectedEntriesClipboardText`（委托 `EditorLogClipboardText.FromMany`）。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogClipboardText.cs`（15→25 行）：新增 `FromMany(IEnumerable<LogEntry>)` 输出纯文本表格（表头 + 每行 \t 分隔）。
- `XuanYu.Editor.UI/Foot/LogDetailPanel.axaml`（63→64 行）：`detailBody` 样式补 `TextWrapping=Wrap` + `AcceptsReturn=True`，详情不再横向截断。

### 验收（需用户机运行）
- 单击日志行可切换选中；Shift+单击可范围多选；Ctrl+单击可追加/取消。
- Ctrl+A 选中当前筛选结果全部；Ctrl+C 复制到记事本为表格、每行一条、详情完整。
- 右侧详情「消息 / 详情」自动换行、可复制、不横向截断。
- Vulkan 日志仍正常显示，VK4-B 运行链路不受影响。

### 下一步
用户重跑编辑器，`Ctrl+A` + `Ctrl+C` 贴出完整 Vulkan 生命周期日志，据此核对 VK4-B-R1 最后一项（关闭时 `LogicalDevice → Surface → Instance` 释放顺序），收口 VK4-B。

## v0.2.4.3-vk 基于 VK4-A 选择结果创建 LogicalDevice + 队列 (2026-07-08)
- 原历史编号：VK4-B

分支：fix/RZ-VK3-A-surface-contract
提交：21f24026ff7b102c12b8346563c066b8a64449a7
版本：VK4-B

### 口径订正（重要）
本次截图验收机 GPU = `NVIDIA GeForce RTX 3050 4GB Laptop GPU`（备用机）；主力机为 RTX 3060。
VK4-B **不以具体显卡型号为准**，而以 **VK4-A 最终选择结果（`VulkanPhysicalDeviceSelection`）** 为准创建 LogicalDevice，
禁止硬编码 RTX 3050 / RTX 3060 或任何具体型号；在备用机上最终选择结果应为 RTX 3050 Laptop，在主力机上应为 RTX 3060。

### 目标
在 VK4-A 已选出的 PhysicalDevice 之上创建 `VkDevice`（LogicalDevice）与 Graphics / Present 队列。
只创建设备与队列，不建 Swapchain、不建 ImageView/RenderPass/CommandBuffer、不清屏、不 Present。
硬约束：必须复用 VK4-A 的最终选择结果，不得重新枚举 PhysicalDevice、不得自行选择其他设备（尤其不得选 D3D12 wrapper / Basic Render / iGPU）。

### 新增文件
- `XuanYu.Render.Vulkan/Device/VulkanPhysicalDeviceSelection.cs`（12 行）：将原内联在选择器末尾的「物理设备选择结果」记录抽出为独立文件，并补 `PhysicalDevice Handle` 字段（被选中的原生句柄），供 VK4-B 复用、禁止泄漏给 UI。
- `XuanYu.Render.Vulkan/Device/VulkanDeviceOwner.cs`（96 行）：基于 `VulkanPhysicalDeviceSelection` 创建 `VkDevice`，启用 Graphics/Present 队列族（同族合并），取 Graphics/Present `VkQueue`，`Dispose` 幂等释放 Device；输出中文日志（开始创建/物理设备名/队列族/创建成功/Queue 获取成功/释放成功）。
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgeDeviceAttachStep.cs`（29 行）：在 VK4-A 选择成功后调用 `VulkanDeviceOwner.Create`；选择失败则跳过（`sel` 为 null 或 `!Success` 时记日志返回 null）；异常仅记日志、不影响已附加的 Instance+Surface+已选中设备。

### 修改内容
- `XuanYu.Render.Vulkan/Device/VulkanPhysicalDeviceSelector.cs`（99→93 行）：移除末尾内联记录定义（已抽至独立文件）；`Select` 内捕获 `bestDevice = devices[i]` 并随结果返回 `Handle`；三处返回点补 `Handle` 实参。selector 仅负责枚举与选择，未触及 VK4-B 逻辑。
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgePhysicalDeviceAttachStep.cs`（23→24 行）：`Run` 返回类型由 `void` 改为 `VulkanPhysicalDeviceSelection?`，把选择结果交回 Bridge 以驱动设备创建。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.cs`（96→98 行）：`Attach` 在 Instance+Surface 就绪后先跑选择 step、再跑设备 step（链式 `_deviceOwner = VulkanBridgeDeviceAttachStep.Run(_vk, 选择step.Run(...), Emit)`）；`Detach` 逆序释放 `Device → Surface → Instance`；Resize 仍只记尺寸、不重建 Surface/Device/Queue；新增 `using XuanYu.Render.Vulkan.Device;` 与 `_deviceOwner` 字段。

### 未做内容（红线）
- 未建 `Swapchain` / `ImageView` / `RenderPass` / `CommandPool` / `CommandBuffer`、未清屏、未 `Present`、未取交换链图像。
- 未重新枚举 PhysicalDevice、未自行选择设备；选择结果直接复用 VK4-A 选定设备。
- UI（Editor.UI）未新增任何 `Silk.NET.Vulkan` 引用（仅历史探针债 `VulkanClearSession.*.cs` 与 csproj ProjectReference，未触碰）；未复制旧探针 `VulkanClearSession` 路径。
- 未顺手推进 VK4-C（Swapchain）；`Bridge/` 子目录 2 文件、`Device/` 子目录 5 文件，均未越 5-7 文件上限。

### 验收结果
| 项 | 结果 |
|---|---|
| VulkanDeviceOwner.cs 行数 | ✅ 96（≤100） |
| VulkanBridgeDeviceAttachStep.cs 行数 | ✅ 29（≤100，新增） |
| VulkanPhysicalDeviceSelection.cs 行数 | ✅ 12（≤100，新增） |
| VulkanPhysicalDeviceSelector.cs 行数 | ✅ 93（≤100） |
| VulkanBridgePhysicalDeviceAttachStep.cs 行数 | ✅ 24（≤100） |
| VulkanNativeHostSurfaceBridge.cs 行数 | ✅ 98（≤100） |
| Render.Vulkan 构建 | ✅ 0W0E |
| Editor.UI 构建（集成验证） | ✅ 0W0E |
| git grep 禁止项 | ✅ 无 Swapchain/ImageView/RenderPass/CommandBuffer/Clear/CreateSwapchain 新增实装（仅注释/日志）；`VkDevice`/`VkQueue` 仅出现在注释 |
| Editor.UI 新增 Silk.NET.Vulkan 引用 | ✅ 无（仍仅历史探针债） |

### 人工测试清单（需在用户机器运行编辑器）
1. 启动编辑器，确认无崩溃、NativeHost 正常附加。
2. 打开日志面板，确认出现 `【VulkanDevice】开始创建 LogicalDevice；物理设备：<最终选中设备名>`。
3. 确认日志含 `使用的 Graphics 队列族：N；Present 队列族：M`（应与 VK4-A 选择结果一致）。
4. 确认 `【VulkanDevice】LogicalDevice 创建成功` 与 `【VulkanDevice】Queue 获取成功（Graphics + Present）`。
5. 确认**仍黑屏**（无 Swapchain/ImageView/RenderPass，预期；真正出画面要等 VK4-D）。
6. 缩放窗口，确认 `尺寸变化已接收：不重建 Surface`，且**不重建 Device / Queue**（VK4-B 红线延续）。
7. 关闭编辑器，确认出现 `【VulkanDevice】LogicalDevice 释放成功`，且 Detach 顺序为 Device→Surface→Instance（无设备资源泄漏告警）。
8. 确认无 `Swapchain`/`ClearFrame`/`Present` 相关新增日志。

### 下一步
VK4-B 边界与行数红线均守住，可判 VK4-B 功能收口。但**必须先做 VK4-B-R1 生命周期审计**，重点核对 Detach 释放顺序 `LogicalDevice → Surface → Instance` 与异常路径资源不泄漏；严禁顺手推进 VK4-C（Swapchain）。

## v0.2.4.4-fix 生命周期审计与运行验证（静态审计已通过；运行时待用户机）(2026-07-08)
- 原历史编号：VK4-B-R1

分支：fix/RZ-VK3-A-surface-contract
关联提交：21f2402（VK4-B 代码） / 1f5da30（VK4-B 文档）
性质：仅审计与运行验证，不新增 Vulkan 能力；不进入 VK4-C；不新增 Swapchain 相关代码。

### 静态审计结果（已通过，无需用户机）
| 项 | 结果 |
|---|---|
| Detach 释放顺序 LogicalDevice → Surface → Instance | ✅ VulkanNativeHostSurfaceBridge.Detach 第 78–80 行已逆序释放 |
| Attach 异常路径逆序回滚 | ✅ catch 块（51–56 行）逆序释放 surface/instance/_vk；设备 step 失败仅返回 null 不抛 |
| 未重新枚举设备 / 基于 VK4-A 选择结果 | ✅ VulkanDeviceOwner.Create 复用 sel.Handle，不重枚举 |
| 未建 Swapchain/ImageView/RenderPass/CommandPool/CommandBuffer | ✅ 仅注释出现，grep 无实装 |
| 未清屏 / 未 Present | ✅ red-line B grep NONE_MATCH |
| UI(Editor.UI) 未新增 Silk.NET.Vulkan 引用 | ✅ 仅历史探针债 VulkanClearSession*.cs（4 文件）+ 生成 obj/bin |
| 命名约定（VulkanDevice 别名 / LogicalDevice 属性） | ✅ VulkanDeviceOwner 用别名，属性名 LogicalDevice，无 Device 作属性名 |
| 全 .cs ≤100 行 | ✅ 最大 Bridge 98 / DeviceOwner 96 / SurfaceOwner 75 |
| Render.Vulkan 构建 | ✅ 0W0E（3.9s） |
| Editor.UI 构建 | ✅ 0W0E（10.6s） |

### 行数明细（实际，订正用户口头「DeviceOwner 95」为 96）
- VulkanNativeHostSurfaceBridge.cs：98（余 2）
- VulkanDeviceOwner.cs：96（余 4）
- VulkanPhysicalDeviceSelector.cs：93
- VulkanBridgePhysicalDeviceAttachStep.cs：24
- VulkanBridgeDeviceAttachStep.cs：29
- VulkanPhysicalDeviceSelection.cs：12
- 其余（Instance/Surface 系列）：9–75

### 风险点（移交 VK4-C 必须遵守）
- **Bridge 98 行，仅余 2 行**：VK4-C 禁止再向 Bridge 塞 Swapchain 逻辑；Swapchain 必须进入独立 owner / attach step（与 VK4-B 的 DeviceOwner / BridgeDeviceAttachStep 同构）。
- **DeviceOwner 96 行，仅余 4 行**：VulkanDeviceOwner 只负责 CreateDevice / GetQueue / DisposeDevice；VK4-C 禁止顺手塞 Swapchain/CommandPool/RenderPass；补日志也须防越 100 行。
- **命名口径（用户明确）**：Silk.NET.Vulkan.Device 类型一律用 VulkanDevice 类型别名；业务 owner = VulkanDeviceOwner；业务属性 = LogicalDevice；禁止再用 Device 作属性名（避免与 XuanYu.Render.Vulkan.Device 命名空间混淆）。

### 待用户机运行验证（12 项，本环境无 GPU/窗口无法跑）
1. 启动编辑器无崩溃
2. VulkanBridge 附加成功：Instance + Surface 已创建
3. VulkanDevice 开始枚举物理设备
4. VulkanDevice 已选择物理设备：本机独显
5. VulkanDevice 开始创建 LogicalDevice
6. 日志显示 Graphics QueueFamily
7. 日志显示 Present QueueFamily
8. 日志显示 LogicalDevice 创建成功
9. 日志显示 Queue 获取成功
10. 缩放窗口：Resize 只记尺寸，不重建 Surface / Device / Queue
11. 关闭编辑器：释放顺序 LogicalDevice → Surface → Instance
12. 确认仍黑屏，无 Swapchain / Clear / Present 日志

> 运行验证通过前，VK4-B 不宣布完全收口；VK4-C 暂缓。

### 下一步
用户提供运行日志/截图后，核对 12 项；若全过则 VK4-B 完全收口，再议 VK4-C（Swapchain 独立 owner/step）。

### 日志路由补强（第⑪项证据闭合）(2026-07-08)
- **问题**：用户运行验证发现 `logs/editor-session-latest.log` 仅有 `【VulkanDevice】LogicalDevice 释放成功` 与合并行 `【VulkanBridge】分离完成：Surface + Instance 已释放`；缺 Surface / Instance 各自释放行，无法逐行核对顺序。
- **根因**：`VulkanSurfaceOwner.Dispose` / `VulkanInstanceOwner.Dispose` 用 `Console.WriteLine` 直写控制台，不经 `_log` 回调，故不入 UI 缓冲、不被 LOG-UX-2 落盘；Bridge 只发一条合并行。释放顺序在代码中本就正确（Device → Surface → Instance），只是文件证据缺独立行。
- **修复（仅日志，不改 Vulkan 语义与释放顺序）**：
  - `VulkanBridgeLogFormatter` 新增 `SurfaceDisposed()` / `InstanceDisposed()`（23 → 29 行）。
  - `VulkanNativeHostSurfaceBridge.Detach` 在 ②Surface、③Instance 释放后各补一条 `Emit`；文件现出现独立行：
    ```
    【VulkanDevice】LogicalDevice 释放成功
    【VulkanBridge】Surface 已释放
    【VulkanBridge】Instance 已销毁
    【VulkanBridge】分离完成：Surface + Instance 已释放
    ```
- **构建**：Render.Vulkan 0W0E；Bridge 由 98 → **100**（压红线边界，符合 ≤100）；格式化器 29 行。
- **关联提交**：c4c804a
- **结论**：第⑪项释放顺序证据闭合（Device → Surface → Instance 现可逐行核对）；VK4-B 可宣布完全收口。

## v0.2.4.2-fix 物理设备选择链路收口修正（压回 100 行红线）(2026-07-08)
- 原历史编号：VK4-A-R1

分支：fix/RZ-VK3-A-surface-contract
提交：fffb6d1a006306f3051ac8cabc2fa27a977301ec
版本：VK4-A-R1

### 目标
VK4-A 审计发现 `VulkanNativeHostSurfaceBridge.cs` 由 93 行涨到 110 行，违反“所有代码文件应 ≤100 行”红线
（“新增文件 ≤100 行”口径不够，被修改的旧文件涨行同样要处理）。本次仅做收口修正，不新增 Vulkan 能力、不改变 VK4-A 行为。

### 新增文件
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgePhysicalDeviceAttachStep.cs`（23 行）：在 Instance+Surface 就绪后调用 `VulkanPhysicalDeviceSelector.Select`，把选择结果与中文日志写入面板；选择异常仅记日志、不影响已附加的 Instance+Surface。

### 修改内容
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.cs`（110→96 行）：删除内联私有方法 `RunDeviceSelection()`，改在 `Attach` 末尾以带引用空值守卫的调用委托给 `VulkanBridgePhysicalDeviceAttachStep.Run(...)`；Bridge 只保留生命周期编排（Attach→Instance+Surface→run attach step / Resize 只记尺寸不重建 Surface / Detach 逆序释放）；`using XuanYu.Render.Vulkan.Device;` 换为 `using XuanYu.Render.Vulkan.Bridge;`。

### 未做内容（红线）
- 未创建 `VkDevice` / `LogicalDevice`、未取 `VkQueue`、未建 `Swapchain`、未建 `ImageView`、未清屏、未 `Present`。
- UI（Editor.UI）未新增任何 `Silk.NET.Vulkan` 引用；未复制旧探针 `VulkanClearSession`；未搬 `VulkanApiProbe`/`VulkanDeviceInfo` 旧代码。
- 未顺手推进 VK4-B（LogicalDevice + Queue）；目录 `Bridge/` 仅 1 文件，未越过 5-7 文件上限。

### 验收结果
| 项 | 结果 |
|---|---|
| VulkanNativeHostSurfaceBridge.cs 行数 | ✅ 96（≤100） |
| VulkanBridgePhysicalDeviceAttachStep.cs 行数 | ✅ 23（≤100，新增文件） |
| VulkanPhysicalDeviceSelector.cs 行数 | ✅ 99（≤100，未改） |
| Render.Vulkan 构建 | ✅ 0W0E |
| Editor.UI 构建（集成验证） | ✅ 0W0E |
| VK4-A 日志仍可见 | ✅ 候选设备/队列族/最终选择经面板输出（调用点不变） |
| git grep 禁止项 | ✅ 无 VkDevice/Queue/Swapchain/ClearFrame 新增实装（仅注释/日志） |

### 人工测试清单（需在用户机器运行编辑器）
1. 启动编辑器，确认无崩溃、NativeHost 正常附加。
2. 打开日志面板，确认仍出现 `【VulkanDevice】开始枚举物理设备；候选数量：N`（拆分后日志链路未断）。
3. 确认每个候选设备的 `候选设备[i]` 日志（名称/类型/API/队列族/呈现支持/可用性）。
4. 确认 RTX 3050 4GB Laptop（备用机；或本机最终选中的独显）被选为 `已选择物理设备`，原因 `优先独立显卡`。（口径订正：历史「RTX 3060」系误写；VK4 系列不以具体型号为准，而以 VK4-A 最终选择结果为准。）
5. 确认 `Surface 呈现支持：是` 且 `可用性：可用`。
6. 确认无 `VkDevice`/`Swapchain`/`ClearFrame` 相关新增日志（仍黑屏，预期）。
7. 缩放窗口，确认 `尺寸变化已接收：不重建 Surface`（VK3 契约不变）。
8. 关闭编辑器，确认 `分离完成：Surface + Instance 已释放`，无设备相关资源泄漏告警。

### 下一步
VK4-A 边界与行数红线均已守住，可判定 VK4-A 正式收口。下一步进入 VK4-B（创建 LogicalDevice + Queue），仍独立文件、独立 commit、独立红线校验；严禁把 B/C/D 混写。

## v0.2.4.1-vk 物理设备选择链路（仅选择，不创建设备） (2026-07-08)
- 原历史编号：VK4-A

分支：fix/RZ-VK3-A-surface-contract
提交：79eabd0c5f11c88ab78607041395353cf05156ae
版本：VK4-A

### 目标
在 VK3 已接入的 Instance + Surface 生命周期之后，新增 PhysicalDevice 选择链路。
只选设备、不渲染：枚举设备、检查 Graphics/Present 队列族与 Surface 呈现支持、
优先独显、输出中文日志、返回纯数据结果。严禁创建 LogicalDevice/Queue/Swapchain。
边界由审计压死：VK4-A 只做 PhysicalDevice 选择，不碰 Device/Queue/Swapchain/清屏。

### 新增文件
- `XuanYu.Render.Vulkan/Device/VulkanPhysicalDeviceInfo.cs`（12 行）：纯数据设备信息（名称/类型/API 版本/是否独显/是否可用），不持有任何 Vulkan 句柄。
- `XuanYu.Render.Vulkan/Device/VulkanQueueFamilySelection.cs`（14 行）：纯数据队列族选择（Graphics/Present 索引与可用性，`None` 静态默认值）。
- `XuanYu.Render.Vulkan/Device/VulkanPhysicalDeviceSelector.cs`（99 行，含结果 record `VulkanPhysicalDeviceSelection`）：`Select` 主入口枚举+选择+中文日志；`SelectQueueFamilies` 队列族与 Surface 支持检查；`TypeName` 类型中文化。

### 修改内容
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.cs`：VK4-A 时由 93→110 行（内联 `RunDeviceSelection()`）；**该 110 行状态已被 VK4-A-R1 修正**——选择逻辑迁出至 `Bridge/VulkanBridgePhysicalDeviceAttachStep.cs`，Bridge 回到 96 行，仅保留生命周期编排。此处记录原始 VK4-A 行为：`Attach` 在 Instance+Surface 就绪后触发选择、经 `Emit` 输出选择器日志、选择异常不影响已附加的 Instance+Surface、`Resize` 不重建 Surface。

### 未做内容（红线）
- 未创建 `VkDevice` / `LogicalDevice`、未取 `VkQueue`、未建 `Swapchain`、未建 `ImageView`、未清屏、未 `Present`。
- UI（Editor.UI）未新增任何 `Silk.NET.Vulkan` 引用；选择器结果仅经日志字符串间接可见，不把 `VkPhysicalDevice` 泄漏给上层。
- 未复制旧探针 `VulkanClearSession`；未搬 `VulkanApiProbe`/`VulkanDeviceInfo` 旧代码。
- 文件落在现有 `XuanYu.Render.Vulkan/Device/` 子目录（复用项目与 Silk.NET 引用，避免新建工程扩大改动面），3 个新文件均 ≤100 行。

### 验收结果
| 项 | 结果 |
|---|---|
| Render.Vulkan 构建 | ✅ 0W0E |
| Editor.UI 构建（集成验证） | ✅ 0W0E |
| 新增文件行数 | ✅ 均 ≤100（info 12 / queue 14 / selector 99） |
| 选择器边界 | ✅ 仅枚举+选择+日志，无 Device/Queue/Swapchain 实装 |
| UI 依赖 | ✅ Editor.UI 未新增 Silk.NET.Vulkan 引用 |
| git grep 禁止项 | ✅ 无 VkDevice/Queue/Swapchain 新增实装（仅注释/日志） |

### 人工测试清单（需在用户机器运行编辑器）
1. 启动编辑器，确认无崩溃、NativeHost 正常附加。
2. 打开日志面板，确认出现 `【VulkanDevice】开始枚举物理设备；候选数量：N`。
3. 确认每个候选设备的 `候选设备[i]` 日志（名称/类型/API/队列族/呈现支持/可用性）。
4. 确认 RTX 3050 4GB Laptop（备用机；或本机最终选中的独显）被选为 `已选择物理设备`，原因 `优先独立显卡`。（口径订正：历史「RTX 3060」系误写；VK4 系列不以具体型号为准，而以 VK4-A 最终选择结果为准。）
5. 确认 `Surface 呈现支持：是` 且 `可用性：可用`。
6. 确认无 `VkDevice`/`Swapchain`/`ClearFrame` 相关新增日志（仍黑屏，预期）。
7. 缩放窗口，确认 `尺寸变化已接收：不重建 Surface`（VK3 契约）。
8. 关闭编辑器，确认 `分离完成：Surface + Instance 已释放`，无设备相关资源泄漏告警。

### 下一步
VK4-A 收口后可进入 VK4-A-R1（审计 + 日志补强）。严禁顺手推进 VK4-B（LogicalDevice + Queue）；B 阶段单独开。

## v0.2.3.12-vk VK3 收口确认 + VK4 规划落地 (2026-07-08)
- 原历史编号：VK3-Closure + VK4-Plan

分支：fix/RZ-VK3-A-surface-contract
提交：49403707f152c9a60f88f7944ca1375b770cdc0a

### 目标
VK3 验收通过，收口确认；并落地 VK4 规划（只规划不实装）。不改任何 Vulkan 生命周期代码，不进入 VK4 实装。

### 新增文档
- `docs/rz-vk3-closure.md`  # VK3 收口确认：验收项表格、已完成阶段（VK3-A..VK3-C2-R1）、红线遵守确认、已知债务（UI 对 Render.Vulkan 工程级引用移交 VK4）、收口日期。结论：NativeHost HWND 生命周期已正式接入 Vulkan Instance + Surface；Swapchain 留 VK4。
- `docs/rz-vk4-plan.md`  # VK4 规划（不实装）：最小渲染闭环 PhysicalDevice→LogicalDevice→Queue→Swapchain→ClearFrame→RenderSession，五问规划、目标依赖方向、阶段分解 VK4-A..VK4-E、防回潮门禁（Resize 不重建 Surface、不搬探针、UI 不持 Vulkan、每步 5+100）。

### 同步
- `file-tree.md`  # 追加 VK3 收口 + VK4 规划文档小节，更新顶部摘要。

### 未做内容（红线）
- 未写任何 Vulkan 实装代码；未选 PhysicalDevice / 未创 LogicalDevice / 未建 Swapchain / 未碰 RenderFrame。
- 未扩大 UI 对 Vulkan 的直接认识。

## v0.2.3.11-fix VulkanBridge 日志面板可见性修复 (2026-07-08)
- 原历史编号：RZ-VK3-C2-R1

分支：fix/RZ-VK3-A-surface-contract
提交：2390c6314c75b30097e689fee60e6fdf05bfd31e

### 目标
不改 Vulkan 生命周期、不进入 VK4、不碰 Device/Queue/Swapchain，只把 `VulkanNativeHostSurfaceBridge` 的 Attach/Resize/Detach/Dispose 结果接入编辑器日志面板，让 VK3-C2 能在 UI 日志中验收。

### 修改内容
- `VulkanNativeHostSurfaceBridge.cs`（84→93 行）：新增 `Action<string>? _log` 日志回调（构造函数注入，默认 null 保留 Console.WriteLine 兜底）。`Attach/Resize/Detach` 经 `Emit` 同时 `_log?.Invoke` 与 `Console.WriteLine`。`Attach` 失败由“抛出不可见异常”改为“记录 `AttachFailed(原因)` 后吞掉异常”，避免编辑器崩溃且失败可见；回滚语义不变。
- `VulkanBridgeLogFormatter.cs`（20→23 行）：文案对齐验收串——`【VulkanBridge】附加成功：Instance + Surface 已创建（含窗口句柄）`、`【VulkanBridge】尺寸变化已接收：不重建 Surface`、`【VulkanBridge】分离完成：Surface + Instance 已释放`；新增 `AttachFailed(reason)`。
- `VulkanSurfaceBridgeProvider.cs`（12→13 行）：`Create()` 改为 `Create(Action<string> log)`，把日志回调接进具体桥接（组合根仍持有 `using Render.Vulkan`，保持 Editor.UI → Abstractions 方向）。
- `ViewportNativeHostRoute.cs`（12→15 行）：新增 `ReportVulkanBridge(UiVm?, string)` → `vm?.LogVulkanLifecycle(message, "")`，复用既有的 NativeHost→UiVm 日志面板路径。
- `VulkanNativeHost.cs`（82→83 行）：`OnAttachedToVisualTree` 中 `_bridge ??= VulkanSurfaceBridgeProvider.Create(msg => ViewportNativeHostRoute.ReportVulkanBridge(DataContext as UiVm, msg))`，把回调接到面板；其余生命周期钩子不变。
- `UiVm.Logging.cs`（line 45）：旧启动告警 `当前渲染后端尚未接入 Vulkan` 改为 `Vulkan Surface 生命周期已接入；Device / Swapchain 尚未接入`，级别 Warning→Info。
- `SampleLogEntries.cs`（line 13-15）：种子示例同步改为上述准确文案，级别 Warning→Info，避免面板出现互相矛盾的两行。

### 未做内容（红线）
- 未选 `PhysicalDevice`、未创 `LogicalDevice`、未取 `Queue`、未建 `Swapchain`、未碰 `RenderFrame`。
- `Resize` 不重建 Surface（桥 `Resize` 仅记中文日志）。
- 未搬 `VulkanClearSession` 探针到正式路径；旧探针未改动。
- 无新增文件，`file-tree.md` 未改（总数维持 105）。

### 验收结果
| 项 | 结果 |
|---|---|
| Render.Vulkan 构建 | ✅ 0W0E |
| Abstractions 构建 | ✅ 0W0E（未改动，仍零 Vulkan 代码引用） |
| Editor.UI 构建 | ✅ 0W0E（临时输出目录编译验证；in-place bin 复制因运行中的编辑器占用 XuanYu.Render.Vulkan.dll 而锁，代码本身 0W0E） |
| git grep 禁止项 | ✅ 7 文件无 PhysicalDevice/LogicalDevice/Queue/Swapchain 实装（仅注释） |
| 文件行数 | ✅ 均 ≤100（bridge 93 / formatter 23 / provider 13 / route 15 / nativehost 83 / logging 88 / sample 35） |
| 旧文案残留 | ✅ 已清除 |

### 下一步
关闭编辑器后 rebuild + run，即可在日志面板看到 `【VulkanBridge】附加成功 / 尺寸变化已接收 / 分离完成`；VK3 系列收尾，Device/Swapchain/RenderFrame 留待 VK4。

## v0.2.3.10-vk 组合根接线：桥接接入 NativeHost 生命周期 (2026-07-08)
- 原历史编号：RZ-VK3-C2

分支：fix/RZ-VK3-A-surface-contract
提交：a01855702866f5b243efa23a796831af1a1a6d7f

### 目标
把 `VulkanNativeHostSurfaceBridge` 接入现有 `VulkanNativeHost` 的 Attach/Resize/Detach 生命周期流，验证真实 HWND 能创建并释放 Instance+Surface。Resize 只记录尺寸不重建 Surface；仍不碰 Device/Queue/Swapchain/RenderFrame。

### 修改内容
- 新增 `VulkanSurfaceBridgeProvider.cs`（12 行，组合根）：`using XuanYu.Render.Vulkan` + `using XuanYu.Render.Abstractions`，`Create()` 返回 `INativeHostSurfaceBridge`。UI 宿主只认契约，具体类实例化隔离在组合根，保持 Editor.UI → Abstractions 依赖方向。
- `VulkanNativeHost.cs`（73→82 行）：
  - 新增 `INativeHostSurfaceBridge? _bridge` 字段（契约类型，不引入 Render.Vulkan 具体类）。
  - `Report` 返回 `NativeHostHandleSnapshot`；`OnAttachedToVisualTree` 用其构造 `NativeHostSurfaceHandle` 并 `_bridge ??= VulkanSurfaceBridgeProvider.Create(); _bridge.Attach(handle)`。
  - `OnSizeChanged` 合并回调内调 `_bridge?.Resize(snap.Width, snap.Height)`（经 250ms Coalescer 节流，高频路径不直写）。
  - `OnDetachedFromVisualTree` 调 `_bridge?.Detach()`。
  - `DestroyNativeControlCore` 调 `(_bridge as IDisposable)?.Dispose()` 并置空（Dispose 顺序：Surface→Instance→Vk，复用 C1-R2 所有权）。

### 未做内容（红线）
- 未选 `PhysicalDevice`、未创 `LogicalDevice`、未取 `Queue`、未建 `Swapchain`、未碰 `RenderFrame/CommandBuffer/RenderPass/Framebuffer`。
- `Resize` 不重建 Surface（桥 `Resize` 仅记中文日志）。
- 未搬 `VulkanClearSession` 探针到正式路径；旧探针未改动。
- `XuanYu.Editor.UI` 工程级对 `Render.Vulkan` 的引用未解耦（组合根 provider 已 `using Render.Vulkan`，口径不变）。

### 验收结果
| 项 | 结果 |
|---|---|
| Render.Vulkan 构建 | ✅ 0W0E |
| Abstractions 构建 | ✅ 0W0E（仍零 Silk.NET/Avalonia/Editor.Win/Vulkan 代码引用） |
| Editor.UI 构建 | ✅ 0W0E |
| git grep 禁止项 | ✅ 两 VK3-C2 文件无 PhysicalDevice/LogicalDevice/Queue/Swapchain 实装 |
| 文件行数 | ✅ VulkanNativeHost 82 / VulkanSurfaceBridgeProvider 12，均 ≤100 |
| Abstractions 纯净 | ✅ 仅解释性注释，无代码引用 |
| file-tree.md | 新增 1 文件，已更新（总数 104→105） |

### 下一步
VK3 系列 Instance/Surface 层已收口（VK3-A / B / C 全完成）。Device/Swapchain/RenderFrame 留待 VK4。

## v0.2.3.9-fix Vk 生命周期所有权收口 (2026-07-08)
- 原历史编号：RZ-VK3-C1-R2

分支：fix/RZ-VK3-A-surface-contract
提交：a176eb365dc42ade0d2c72cff9901ac9b9d740e0

### 目标
统一 `Vk.GetApi()` / `Vk.Dispose()` 的所有权，避免 VK3-C2 接真实 NativeHost 生命周期后出现重复 Dispose、提前 Dispose 或泄漏。不接 UI 组合根，不碰 Device/Swapchain。

### 修改内容
- `VulkanInstanceOwner.cs`（66→57 行）：
  - `Create` / `CreateWithResult` 改为接收 `Vk vk` 参数，移除内部 `Vk.GetApi()`。
  - `CreateWithResult` 失败路径不再 `vk.Dispose()`（所有权在调用方）。
  - `Dispose` 仅 `vk.DestroyInstance` 释放 Instance，**移除 `_vk.Dispose()`**——Vk 不再由本类释放。
- `VulkanSurfaceOwner.cs`（注释补强，75 行）：明确「Vk 由调用方（Bridge）统一持有与释放，本类只使用传入的 Vk，不持有也不释放 Vk」。
- `VulkanNativeHostSurfaceBridge.cs`（76→84 行）：
  - 新增 `Vk? _vk` 字段，由 `Attach` 统一 `Vk.GetApi()` 持有。
  - `Attach` 复用既有 `_vk`（避免重复 GetApi）；仅本轮新创建时才在失败回滚中 `vk.Dispose()`，复用则不释放，杜绝重复 Dispose。
  - 两个 Owner 均接收同一 `Vk` 实例。
  - `Dispose` 顺序固定：`Detach()`（Surface→Instance）→ `_vk?.Dispose()`（Vk 最后释放）。
  - `Attach` 失败回滚顺序与 Dispose 一致：Surface→Instance→（如本轮新创建）Vk。

### 未做内容（红线）
- 未接 UI 组合根；未选 `PhysicalDevice`、未创 `LogicalDevice`、未取 `Queue`、未建 `Swapchain`、未碰 `RenderFrame/CommandBuffer/RenderPass/Framebuffer`。
- 旧 `VulkanClearSession` / `VulkanApiProbe` 探针未改动，仍为历史债务，不纳入正式路径。
- `XuanYu.Editor.UI` 工程级对 `Render.Vulkan` 的引用未解耦（口径不变）。

### 验收结果
| 项 | 结果 |
|---|---|
| Render.Vulkan 构建 | ✅ 0W0E |
| Abstractions 构建 | ✅ 0W0E（仍零 Silk.NET/Avalonia/Editor.Win/Vulkan 代码引用） |
| Editor.UI 构建 | ✅ 0W0E |
| git grep 禁止项 | ✅ 三 VK3 文件仅注释提及，无 PhysicalDevice/LogicalDevice/Queue/Swapchain 实装 |
| 文件行数 | ✅ InstanceOwner 57 / SurfaceOwner 75 / Bridge 84，均 ≤100 |
| file-tree.md | 无新增文件，未改（总数维持 104） |

### 已知债务（已消解项）
- ~~`Vk.GetApi()` 所有权不统一~~ → 本轮已收口：Bridge 唯一所有者，Owner 仅使用。
- UI 工程级对 Render.Vulkan 的引用待后续解耦。

### 下一步
VK3-C2：把 `VulkanNativeHostSurfaceBridge` 挂到现有 NativeHost 生命周期流（组合根接线），仍不碰 Device/Swapchain，Resize 只传尺寸不重建 Surface；接线时复用已收口的 Vk 所有权。

## v0.2.3.8-fix Bridge 生命周期异常安全收口 (2026-07-08)
- 原历史编号：RZ-VK3-C1-R1

分支：fix/RZ-VK3-A-surface-contract
提交：733ccaef7f2a89477d32a01c6dc5dcce0879cb6d

### 目标
为 VK3-C1 的 `VulkanNativeHostSurfaceBridge` 补生命周期异常安全收口，避免半初始化状态、重复 Attach/Dispose 与误导日志。**不接 UI 组合根，不碰 Device/Swapchain**。

### 修改内容
- `VulkanNativeHostSurfaceBridge.cs`（46→76 行）：
  - `Attach` 开头检查 `_disposed`，已 Dispose 抛 `ObjectDisposedException`。
  - `Attach` 已附加判断改为 `_instanceOwner` 与 `_surfaceOwner` 双字段均非 null。
  - `Attach` 用临时变量 `instance`/`surface` 先创建，全成功后才落字段；任一失败进入 `catch`，先 `surface?.Dispose()` 再 `instance?.Dispose()`，并把两字段恢复为 null 后 `throw`——消除“有 Instance 无 Surface”的半初始化。
  - `Resize` 未附加时输出「收到尺寸变化但尚未 Attach，不处理 Surface」，仍不重建 Surface。
  - `Detach` 无资源时输出「跳过分离：尚未 Attach」，避免误判。
- `VulkanBridgeLogFormatter.cs`（14→20 行）：新增 `ResizedSkipped(int,int)` 与 `DetachedSkipped()` 两条跳过日志。

### 未做内容（红线）
- 未接 UI 组合根；未选 `PhysicalDevice`、未创 `LogicalDevice`、未取 `Queue`、未建 `Swapchain`、未碰 `RenderFrame/CommandBuffer/RenderPass/Framebuffer`。
- `Vk.GetApi()` 所有权统一问题（Bridge/InstanceOwner/SurfaceOwner 共用 Silk.NET 单例，避免在多处重复 Dispose）**未在本轮解决**，列为 VK3-C2 前需确认项，记于「已知债务」。

### 验收结果
| 项 | 结果 |
|---|---|
| Render.Vulkan 构建 | ✅ 0W0E |
| Abstractions 构建 | ✅ 0W0E（仍零 Silk.NET/Avalonia/Editor.Win/Vulkan 引用） |
| Editor.UI 构建 | ✅ 0W0E（未改其代码路径） |
| git grep 禁止项 | ✅ 无 PhysicalDevice/LogicalDevice/Queue/Swapchain 实装 |
| dotnet test | ⚠️ 仓库无独立测试项目（MSB1003），如实记录 |
| file-tree.md | 无新增文件，未改（总数维持 104） |

### 已知债务
- `Vk.GetApi()` 在 `VulkanNativeHostSurfaceBridge` 与两个 Owner 内共用同一 Silk.NET 单例，`VulkanInstanceOwner.Dispose` 会 `vk.Dispose()`；VK3-C2 接线前须确认不会多处获取/释放导致重复 Dispose 或泄漏。
- `VulkanClearSession` 仍是历史探针，不得搬进正式路径；C2 接线走新桥而非复用探针逻辑。
- UI 工程级对 Render.Vulkan 的引用待后续解耦。

### 下一步
VK3-C2：把 `VulkanNativeHostSurfaceBridge` 挂到现有 NativeHost 生命周期流（组合根接线），仍不碰 Device/Swapchain，Resize 只传尺寸不重建 Surface；接线前先确认 `Vk.GetApi()` 所有权。

## v0.2.3.7-vk NativeHost 生命周期桥接类 (2026-07-08)
- 原历史编号：RZ-VK3-C1

分支：fix/RZ-VK3-A-surface-contract
提交：2eb6cc930ae51eccb62546509df5925bd9eab146

### 目标
实现 `INativeHostSurfaceBridge` 的 Vulkan 桥接类，把已完成的 `VulkanInstanceOwner` 与 `VulkanSurfaceOwner` 串起来；**暂不接 UI 组合根**，只做桥本身。

### 修改内容
- 新增 `VulkanNativeHostSurfaceBridge.cs`（46 行，unsafe）：实现 `INativeHostSurfaceBridge` + `IDisposable`。
  - `Attach(handle)`：先 `VulkanInstanceOwner.Create()`，再 `VulkanSurfaceOwner.Create(Vk.GetApi(), instance, handle)`；幂等（已 Attach 则跳过）。
  - `Detach()`：先释放 Surface 再释放 Instance（顺序相反于创建）。
  - `Resize(w, h)`：仅 `Console.WriteLine` 中文日志，**不重建 Surface**（红线：Surface 仅绑定 Attach/Detach）。
  - `Dispose()`：幂等，转调 `Detach()`。
  - 暴露 `Instance` / `Surface` 只读属性供后续 VK3-C2 / VK4 取用。
- 新增 `VulkanBridgeLogFormatter.cs`（14 行）：纯中文生命周期日志格式器（Attached / Resized / Detached）。

### 未做内容（红线）
- 未接 UI 组合根（`VulkanNativeHostSurfaceBridge` 仅作为可独立实例存在的桥，未被任何 NativeHost 生命周期流引用）。
- 未选 `PhysicalDevice`、未创 `LogicalDevice`、未取 `Queue`、未建 `Swapchain`、未碰 `RenderFrame/CommandBuffer/RenderPass/Framebuffer`。
- 未把 Vulkan 实现放进 `XuanYu.Render.Abstractions`。
- `XuanYu.Editor.UI` 工程级仍因历史 Vulkan 探针（VulkanClearSession 等）保留对 `Render.Vulkan` 的引用，未完全解耦。

### 验收结果
| 项 | 结果 |
|---|---|
| Render.Vulkan 构建 | ✅ 0W0E |
| Abstractions 构建 | ✅ 0W0E（仍零 Silk.NET/Avalonia/Editor.Win/Vulkan 引用） |
| Editor.UI 构建 | ✅ 0W0E（未改其代码路径） |
| git grep 禁止项 | ✅ 无 PhysicalDevice/LogicalDevice/Queue/Swapchain/RenderFrame 实装 |
| dotnet test | ⚠️ 仓库无独立测试项目（MSB1003），如实记录 |
| file-tree.md / changelog.md | ✅ 已更新（总数 102→104） |

### 已知债务
- `VulkanClearSession` 仍是历史 Vulkan 探针，不得搬进正式路径；VK3-C2 接线应走新桥而非复用探针逻辑。
- UI 工程级对 Render.Vulkan 的引用待后续解耦。

### 下一步
VK3-C2：把 `VulkanNativeHostSurfaceBridge` 挂到现有 NativeHost 生命周期流（组合根接线），仍不碰 Device/Swapchain，Resize 只传尺寸不重建 Surface。

## v0.2.3.6-fix VulkanSurfaceOwner 健壮性收口 (2026-07-08)
- 原历史编号：RZ-VK3-B2-R1

分支：fix/RZ-VK3-A-surface-contract
提交：7a1299a9aa1d4f80b3dcd135ec5952a721ef4280
推送状态：已推送 origin

### 本轮目标
VK3-B2-R1：在正式接 VK3-C 组合根前，补强 VulkanSurfaceOwner 的失败诊断与入参校验。不进入 VK3-C，不接组合根，不碰 PhysicalDevice / LogicalDevice / Queue / Swapchain，未新增文件。

### 修改内容
- `VulkanSurfaceOwner.CreateWithResult`：将 `KhrWin32Surface.CreateWin32Surface` 的返回值保存为 `Result`，失败时把真实 `result.ToString()`（如 ErrorExtensionNotPresent / ErrorNativeWindowInUseKhr / ErrorOutOfHostMemory）写入 `VulkanSurfaceResult.ErrorMessage`，对齐 B1-R1 已落地的真实 VkResult 记录标准；不再只返回泛化的"CreateWin32Surface 失败"。
- `VulkanSurfaceOwner.CreateWithResult`：创建前校验 `handle.Hwnd != 0` 与 `handle.Hinstance != 0`，任一为 0 即返回失败结果（错误类型"无效句柄"，详情指明对应字段），避免 VK3-C 接入真实 NativeHost 生命周期后收到 0 句柄时错误难看。

### 未做内容
- 未选择 PhysicalDevice；未创建 LogicalDevice；未获取 Queue；未创建 Swapchain。
- 未碰 RenderFrame / CommandBuffer / RenderPass / Framebuffer。
- 未接组合根（INativeHostSurfaceBridge），未改动 Editor.UI。
- 未把任何 Vulkan 实现类型放进 XuanYu.Render.Abstractions。
- 未新增文件，file-tree.md 不变。

### 验收结果
- git diff 自审：无 PhysicalDevice / LogicalDevice / Queue / Swapchain 实装（仅注释提及）。
- `VulkanSurfaceOwner.cs` 由 69 → 74 行，仍 ≤100 行。
- XuanYu.Render.Abstractions 仍零 Silk.NET/Avalonia/Editor.Win/Vulkan 引用。
- XuanYu.Render.Vulkan / Abstractions / Editor.UI 构建均 0 warning / 0 error。
- 仓库无独立测试项目：`dotnet test` 退出 MSB1003，如实记录。

### 已知债务
- Editor.UI 仍因历史 Vulkan 探针保留对 Render.Vulkan 的工程级引用，不能宣称 UI 已完全解耦 Vulkan。
- VulkanSurfaceOwner 仍未接入任何使用方（含组合根），等待 VK3-C 接线。

### 下一步
VK3-B2-R1 收口后，可进入 VK3-C：经 INativeHostSurfaceBridge 由组合根把 VulkanInstanceOwner + VulkanSurfaceOwner 接线到 NativeHost Attach/Detach，仍不碰 Device / Swapchain。

## v0.2.3.5-vk Vulkan Surface 持有者 (2026-07-08)
- 原历史编号：RZ-VK3-B2

分支：fix/RZ-VK3-A-surface-contract
提交：9b41b28fc2a33f0953ccd00db7287353eb543be0
推送状态：已推送 origin

### 本轮目标
VK3-B2：在 XuanYu.Render.Vulkan 内新增 VulkanSurfaceOwner，仅负责创建与释放 VkSurfaceKHR（Win32），生命周期绑定 NativeHost Attach/Detach，不绑定 Resize。不碰 PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame。组合根接线（INativeHostSurfaceBridge）留给 VK3-C，本轮不接入 Editor.UI 正式路径。

### 修改内容
- 新增 `VulkanSurfaceOwner`（Render.Vulkan 内部 unsafe 类）：从 `Vk` + `Instance` + `NativeHostSurfaceHandle` 创建 `VkSurfaceKHR`；创建经 `KhrWin32Surface.CreateWin32Surface`，销毁经 `KhrSurface.DestroySurface`（双扩展分别取用，与既有 VulkanClearSession 模式一致）；`Dispose` 幂等（重复调用不炸）且释放后 `_surface = default`；通过 `VulkanSurfaceLogFormatter` 输出中文生命周期日志（创建成功含窗口句柄 / 释放含 Surface 句柄 / 失败含错误类型与详情）。
- 新增 `VulkanSurfaceLogFormatter`：纯中文生命周期日志格式器（创建成功含窗口句柄、释放含 Surface 句柄、失败含错误类型与详情）。
- 新增 `VulkanSurfaceResult`：极小创建结果类型，携带 Success / Owner / 错误类型与详情；`Create()` 抛异常，`CreateWithResult()` 返回结果，二者共用同一条创建链路。
- `XuanYu.Render.Vulkan.csproj` 补 `Silk.NET.Vulkan.Extensions.KHR` 包（提供 KhrWin32Surface / KhrSurface）与 `XuanYu.Render.Abstractions` 项目引用（取 `NativeHostSurfaceHandle`），对齐 Editor.UI 的 KHR 包版本 2.22.0。

### 范围口径（延续 VK3-B1 / B1-R1）
- "Editor.UI 改经 Abstractions 而非直接持有 Render.Vulkan" 仅限定为 NativeHost 生命周期链路；Editor.UI 工程级仍因历史 Vulkan 探针保留对 Render.Vulkan 的引用，本轮未改动 Editor.UI 正式路径。
- VulkanSurfaceOwner 仅消费 `NativeHostSurfaceHandle`（Abstractions 纯契约），不反向引用 Editor.UI。

### 未做内容
- 未选择 PhysicalDevice；未创建 LogicalDevice；未获取 Queue；未创建 Swapchain。
- 未碰 RenderFrame / CommandBuffer / RenderPass / Framebuffer。
- 未把 VulkanSurfaceOwner 接入任何使用方（组合根接线留给 VK3-C）。
- 未把任何 Vulkan 实现类型放进 XuanYu.Render.Abstractions。

### 验收结果
- git diff 自审：无 PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame 实装（仅注释提及）。
- 新增 3 文件，均 ≤100 行（Owner 69 / Result 9 / LogFormatter 16）。
- XuanYu.Render.Abstractions 仍零 Silk.NET/Avalonia/Editor.Win/Vulkan 引用。
- XuanYu.Render.Vulkan / Abstractions / Editor.UI 构建均 0 warning / 0 error。
- 仓库无独立测试项目：`dotnet test` 退出 MSB1003，如实记录。

### 已知债务
- Editor.UI 仍因历史 Vulkan 探针保留对 Render.Vulkan 的工程级引用，不能宣称 UI 已完全解耦 Vulkan。
- VulkanSurfaceOwner 仍未接入任何使用方（含组合根），等待 VK3-C 接线。

### 下一步
VK3-B2 收口后，可进入 VK3-C：经 INativeHostSurfaceBridge 由组合根把 VulkanInstanceOwner + VulkanSurfaceOwner 接线到 NativeHost Attach/Detach，仍不碰 Device / Swapchain。

## v0.2.3.4-fix VulkanInstanceOwner 行数与健壮性收口 (2026-07-08)
- 原历史编号：RZ-VK3-B1-R1

分支：fix/RZ-VK3-A-surface-contract
提交：fde25d2fe8140022a7273e133081fc8da23393d9
推送状态：已推送 origin

### 本轮目标
VK3-B1-R1：在进 VK3-B2 前，先把 VulkanInstanceOwner 从 98 行（贴 100 行红线）拆干净，避免 B2 接 VulkanSurfaceOwner 时顺手改 Owner 立刻破线。不新增 Surface，不进入 VK3-B2，不碰 Device / Swapchain / Queue。

### 修改内容
- `VulkanInstanceOwner` 由 98 行降到 66 行（<70）：移除内联的 ApplicationInfo / InstanceCreateInfo / 扩展指针构造，改调 `VulkanInstanceCreateInfoBuilder.BuildAndUse`。
- 新增 `VulkanInstanceExtensions`：仅存 Instance 启用的最小扩展名集合（VK_KHR_surface、VK_KHR_win32_surface，以 null 结尾字节序列）；明确禁止在此添加 Device / Swapchain / 其他扩展。
- 新增 `VulkanInstanceCreateInfoBuilder`：在 fixed 作用域内构造 InstanceCreateInfo 并交给回调，确保扩展名指针在创建调用期间有效；仅构造信息，不直接调用 Vulkan。
- `vk.CreateInstance` 失败时记录实际 `Result`（错误类型记为 `VkResult`），不再只写“创建 Vulkan Instance 失败”。
- `Dispose` 释放后 `_instance = default`，避免实例属性暴露已释放的旧句柄。

### 范围口径（延续 VK3-B1）
- "Editor.UI 改经 Abstractions 而非直接持有 Render.Vulkan" 仅限定为 NativeHost 生命周期链路；Editor.UI 工程级仍因历史 Vulkan 探针保留对 Render.Vulkan 的引用。本轮未改动 Editor.UI 正式路径。

### 未做内容
- 未新增 VulkanSurfaceOwner；未创建 VkSurfaceKHR；未调用 CreateWin32Surface。
- 未选择 PhysicalDevice；未创建 LogicalDevice；未获取 Queue；未创建 Swapchain。
- 未碰 RenderFrame / CommandBuffer / RenderPass / Framebuffer。
- 未把任何 Vulkan 实现类型放进 XuanYu.Render.Abstractions。

### 验收结果
- git diff 自审：无 Surface / Device / Swapchain / Queue 实装（仅注释提及）。
- 新增 2 文件 + 重构 1 文件，均 ≤100 行（Owner 66 / Builder 40 / Extensions 9）。
- XuanYu.Render.Abstractions 仍零 Silk.NET/Avalonia/Editor.Win/Vulkan 引用。
- XuanYu.Render.Vulkan / Abstractions / Editor.UI 构建均 0 warning / 0 error。
- 仓库无独立测试项目：`dotnet test` 退出 MSB1003，如实记录。

### 已知债务
- Editor.UI 仍因历史 Vulkan 探针保留对 Render.Vulkan 的工程级引用，不能宣称 UI 已完全解耦 Vulkan。
- VulkanInstanceOwner 仍未接入任何使用方，等待 VK3-B2 接线。

### 下一步
VK3-B1-R1 收口后，可进入 VK3-B2：VulkanSurfaceOwner 经 INativeHostSurfaceBridge 由组合根实现，仍不碰 Device / Swapchain。

## v0.2.3.3-vk Vulkan Instance 持有者 (2026-07-08)
- 原历史编号：RZ-VK3-B1

分支：fix/RZ-VK3-A-surface-contract
提交：aa56857
推送状态：已推送 origin

### 本轮目标
VK3-B1：在 XuanYu.Render.Vulkan 内新增 VulkanInstanceOwner，只负责创建与释放 Vulkan Instance，并确认启用 VK_KHR_surface 与 VK_KHR_win32_surface 扩展。不碰 Surface / Device / Swapchain / Queue。

### 修改内容
- 新增 `VulkanInstanceOwner`（Render.Vulkan 内部 unsafe 类）：创建 VkInstance 并启用 VK_KHR_surface、VK_KHR_win32_surface 两个扩展；`Dispose` 幂等（重复调用不炸）；通过 `VulkanInstanceLogFormatter` 输出中文生命周期日志（创建成功 / 释放 / 失败）。
- 新增 `VulkanInstanceLogFormatter`：纯中文生命周期日志格式器（创建成功含 API 版本与启用扩展清单、释放含实例句柄、失败含错误类型与详情）。
- 新增 `VulkanInstanceResult`：极小创建结果类型，携带 Success / Owner / ApiVersion / 错误类型与详情；`Create()` 抛异常，`CreateWithResult()` 返回结果，二者共用同一条创建链路。

### 范围口径（修正 VK3-A 表述）
- "Editor.UI 改经 Abstractions 而非直接持有 Render.Vulkan" 仅限定为：NativeHost 生命周期链路已改经 Abstractions；Editor.UI 工程级仍因历史 Vulkan 探针（`VulkanApiProbe` 等）保留对 Render.Vulkan 的引用。本轮未改动 Editor.UI 正式路径。

### 未做内容
- 未新增 VulkanSurfaceOwner；未创建 VkSurfaceKHR；未调用 CreateWin32Surface。
- 未选择 PhysicalDevice；未创建 LogicalDevice；未获取 Queue；未创建 Swapchain。
- 未碰 RenderFrame / CommandBuffer / RenderPass / Framebuffer。
- 未把任何 Vulkan 实现类型放进 XuanYu.Render.Abstractions。

### 验收结果
- git diff 自审：新增文件仅含 Instance 创建/释放与中文日志，无 Surface / Device / Swapchain / Queue 实装。
- XuanYu.Render.Abstractions 不引用 Silk.NET / Avalonia / Editor.Win / Render.Vulkan（仅注释提及，无 using/工程引用）；构建 0W0E。
- XuanYu.Render.Vulkan 构建 0 warning / 0 error。
- XuanYu.Editor.UI 构建 0 warning / 0 error（依赖方，本轮未改其代码路径）。
- 仓库无独立测试项目：根目录无 .sln / 测试 .csproj，`dotnet test` 因无项目/解决方案可运行而退出（MSB1003）；如实记录。

### 已知债务
- Editor.UI 仍因历史 Vulkan 探针保留对 Render.Vulkan 的工程级引用，不能宣称 UI 已完全解耦 Vulkan。
- VulkanInstanceOwner 当前未接入任何使用方（组合根 / 探针入口），仅作为 VK3-B1 交付物落地，等待 VK3-B2 接线。
- Instance 扩展写死为最小集合（仅 surface + win32_surface 两项），符合 VK3-B1 审计点 1。

### 下一步
VK3-B1 审计三点已过：扩展为最小集合 / Dispose 幂等 / 无 Surface(Swapchain) 实装。可进入 VK3-B2：VulkanSurfaceOwner 经 INativeHostSurfaceBridge 由组合根实现，仍不碰 Device / Swapchain。

## v0.2.3.2-fix Surface 契约层依赖收口 (2026-07-08)
- 原历史编号：RZ-VK3-A-R1

分支：fix/RZ-VK3-A-surface-contract
提交：37504b0
推送状态：已推送 origin

### 本轮目标
收口 VK3-A 契约层，避免 UI 继续通过 Render.Vulkan 获取 NativeHost 生命周期类型。

### 修改内容
- 将 `NativeHostHandleSnapshot` / `NativeHostLifecycleState` / `NativeHostLifecycleProbe` / `NativeHostLifecycleLogFormatter` 从 `XuanYu.Render.Vulkan` 迁入 `XuanYu.Render.Abstractions`（均为纯数据/枚举/探针/日志格式器，无 Silk.NET / Vulkan 依赖），删除 Render.Vulkan 内对应 4 个文件（git 识别为 rename）。
- UI 侧 5 个生命周期链路文件（`NativeHostSurfaceContract` / `NativeHostResizeCoalescer` / `ViewportNativeHostRoute` / `Vm/UiVm.NativeHostLifecycle` / `Viewport/Vulkan/VulkanNativeHost`）的 `using XuanYu.Render.Vulkan` 改为 `using XuanYu.Render.Abstractions`。
- 同步 `changelog.md` 与 `file-tree.md`。

### 未做内容
- 未创建 VulkanInstanceOwner / VulkanSurfaceOwner / VkSurfaceKHR。
- 未碰 Swapchain / Device / Queue / PhysicalDevice / LogicalDevice。
- 未迁移 `VulkanClearSession` 历史探针（留 Editor.UI，不进正式路径）。

### 验收结果
- `XuanYu.Render.Abstractions` 不引用 Silk.NET / Avalonia / Editor.Win / Render.Vulkan。
- 低内存模式构建 Editor.UI：0 warning / 0 error；Abstractions 0W0E。
- 仓库无独立测试项目，如实记录。

### 已知债务
- Editor.UI 仍因历史 Vulkan 探针（`VulkanApiProbe` 等）保留对 Render.Vulkan 的工程级引用，不能宣称 UI 已完全解耦。
- `VulkanClearSession` 探针仍留 Editor.UI（历史债），VK3-B 不得直接搬用。

### 下一步
VK3-B1：VulkanInstanceOwner。只做 Instance（启用 `VK_KHR_surface` + `VK_KHR_win32_surface`），不碰 Surface / Device / Swapchain。

## v0.2.3.1-vk Surface 契约层建立 (2026-07-07)
- 原历史编号：RZ-VK3-A

分支：fix/RZ-VK3-A-surface-contract
提交：2bfbe2e
推送状态：已推送 origin

### 本轮目标
建立 UI 与 Vulkan 之间的纯净交接契约层，为后续 SurfaceOwner 接线铺路。

### 修改内容
- 新增独立 `XuanYu.Render.Abstractions` 契约工程（net10.0，零 Silk.NET 引用）。
- 定义 `NativeHostSurfaceHandle`（HWND / Hinstance / 尺寸 / DPI）与 `INativeHostSurfaceBridge`（Attach / Resize / Detach 契约接口）。
- Editor.UI 侧新增 `NativeHostSurfaceContract`，把现有 `NativeHostHandleSnapshot` 映射为交接句柄（取 `Win32ViewportHost.ModuleHandle` 作 Hinstance），不引入任何 Vulkan 实现使用点。
- `XuanYu.Editor.UI.csproj` 加对 Abstractions 的工程引用。

### 未做内容
- 未创建 VulkanSurfaceOwner / VulkanInstanceOwner / VkSurfaceKHR / Swapchain / Device。
- UI 到 Render.Vulkan 的解耦收口见 RZ-VK3-A-R1。

### 验收结果
- 低内存模式构建 Editor.UI：0 warning / 0 error。
- 仓库无独立测试项目，如实记录。

### 已知债务
- VK3-A 仅新建 Abstractions 工程，UI 对 Render.Vulkan 的工程级引用与 7 处 using 当时未移除，故 VK3-A 只算「契约层雏形已建立，解耦未完成」。

### 下一步
RZ-VK3-A-R1 收口依赖。

## v0.2.2.7-vk VK3 Surface 生命周期规划 (2026-07-07)
- 原历史编号：RZ-VK3-Plan
- 仅规划正式 Vulkan Surface 生命周期，替代 `VulkanClearSession` 探针状态；本轮不写任何 Vulkan 实装代码。
- 明确：Surface 由 `XuanYu.Render.Vulkan` 内部 `VulkanSurfaceOwner` 创建/持有；NativeHost 只提供 HWND/尺寸与 Attach/Detach 生命周期，不直接管理 Vulkan；Editor.UI 不直接创建 Surface/Device/Swapchain；`VulkanClearSession` 仅作探针参考，不能直接搬进正式路径。
- VK3 只做 Surface，Swapchain 留给 VK4；阶段边界硬于技术规则，禁止 VK3 夹带 Swapchain。
- 产出 `docs/rz-vk3-surface-lifecycle-plan.md`。

## v0.2.2.6-rz Windows 兼容清单提交 (2026-07-07)
- 原历史编号：Fix-M1
- 单独提交 `XuanYu.Editor.UI/app.manifest` 中遗留的 Windows `supportedOS` 兼容清单块（10/11/8.1/8/7），仅声明系统兼容，无任何 Vulkan / 逻辑改动。
- 不碰 Vulkan / NativeHost / Resize / Surface / Swapchain / LogicalDevice；`.workbuddy/` 与 `qizheng-mvp-fixed/` 维持未跟踪，不纳入提交。
- 提交信息：`chore(editor): declare Windows compatibility manifest`。

## v0.2.2.5-fix NativeHost Resize 合并验证/收口 (2026-07-07)
- 原历史编号：RZ-VK2-R2
- 验证 RZ-VK2-R1 合并边界干净：NativeHostResizeCoalescer 只合并 UI 生命周期日志，未改变 Win32ViewportHost.Resize 调用时机，未牵连 VulkanClearSession.Resize / Surface / Swapchain / LogicalDevice。
- git diff 确认 VulkanClearSession.* 相对 HEAD 零改动；本回合文件均不引用它；无新增 Silk.NET.Vulkan 使用点。
- 确认工作树仅 app.manifest 为 tracked modified（非本轮任务），不混入提交。
- 新增 `docs/audit-RZ-VK2-R2-nativehost-resize-coalesce-verify.md`，回答四问：日志已转合并 / 无残留高频直写 / 未动 Surface/Swapchain/Device / Editor.UI 直接引用 Vulkan 债务仍在但未扩大。
- 验收：`dotnet restore` 通过；`dotnet build --no-restore` 通过，0 Warning / 0 Error；`dotnet test` 退出正常且仓库无独立测试项目。
- 提交信息：`test(editor): RZ-VK2-R2 verify native host resize coalescing`。

## v0.2.2.4-fix NativeHost 尺寸变化日志合并 (2026-07-07)
- 原历史编号：RZ-VK2-R1
- 修复 NativeHost 尺寸变化高频事件连续进入 `EditorLogBus` 的问题（`VulkanNativeHost.OnSizeChanged` 每次直写日志并 `RefreshLogBindings`，导致截图「重复 138 次」）。
- 新增 `NativeHostResizeSnapshot`（只保存尺寸数据）与 `NativeHostResizeCoalescer`（250ms debounce，连续 SizeChanged 只更新快照与合并计数，稳定后才生成一条低频合并日志）。
- `ViewportNativeHostRoute` 增加 `ReportMerged` 薄入口；`UiVm.NativeHostLifecycle` 增加 `LogNativeHostResizedMerged`（合并日志含最终宽度、高度、DPI、生命周期版本、合并次数；无效句柄只写一条低频失效日志）。
- `NativeHostLifecycleLogFormatter` 增加 `MergedMessage` 中文合并日志格式。
- `VulkanNativeHost` 的 `OnSizeChanged` 改为走 Coalescer；`OnDetachedFromVisualTree` / `DestroyNativeControlCore` 调用 `Cancel()` 安全停止 pending debounce，不补写日志。
- 中央视口文案 `Vulkan Clear Probe` 改为 `NativeHost Probe`（`Main.axaml`）与 `Vulkan Probe`（`VulkanViewport.axaml`）。
- 未创建 Surface / Swapchain / LogicalDevice，未接入真实渲染循环，未修改顶部/左侧/右侧/底部布局与输入链路。
- 验收：`dotnet restore` 通过；`dotnet build --no-restore` 通过，0 Warning / 0 Error；本轮新增/修改 `.cs` / `.axaml` 全部 ≤100 行；`dotnet test` 退出正常且仓库无独立测试项目。

## v0.2.2.1-rz 新人接手规则审计 (2026-07-07)
- 原历史编号：RZ-New-0
- 新增开发规范两份（经人工校正 5+100 / 依赖隔离 / 日志边界 / VK 阶段边界表述）：`docs/dev-rules.md`（硬规则执行手册 + 接手红线清单）、`docs/dev-rules-understanding.md`（事故来源与动机解释）。
- 新增 `docs/audit-RZ-New-0-onboarding.md`：按 10 项清单完成接手验收。实测确认 Editor.UI 仍直接引用 Silk.NET.Vulkan / XuanYu.Render.Vulkan（过渡期冲突）；VulkanClearSession 探针已创建 Instance/Surface/Device/Swapchain；NativeHost 高频 SizeChanged 直写 EditorLogBus 风险属实。
- 同步 file-tree.md。
- 验收：`dotnet restore` 通过；`dotnet build --no-restore` 通过，0 Warning / 0 Error；`dotnet test` 退出正常且仓库无独立测试项目。
- 提交信息：`docs(dev): 新增开发规范文档与 RZ-New-0 接手审计`。

## v0.2.2.3-vk NativeHost / HWND 生命周期收口 (2026-07-07)
- 原历史编号：RZ-VK2
- 新增 `XuanYu.Render.Vulkan` 内的 NativeHost 生命周期快照、状态、探针与中文日志格式化。
- `VulkanNativeHost` 收口为纯 HWND 生命周期宿主，只记录创建、附加、句柄可用、尺寸变化、移除、释放、失效，不再触碰 Vulkan 会话。
- 新增 `ViewportNativeHostRoute` 与 `UiVm.NativeHostLifecycle`，UI 仅通过薄入口把快照写入现有日志系统。
- 新增审计文档 `docs/audit-RZ-VK2-native-host-lifecycle.md`，记录 HWND 生命周期、验证结果与 RZ-VK3 接 Surface 的接点。
- 验收：`dotnet restore` 通过；`dotnet build --no-restore` 通过，0 Warning / 0 Error；`dotnet test` 退出正常且仓库无独立测试项目。

## v0.2.2.2-vk Vulkan 依赖接入与环境探针 (2026-07-07)
- 原历史编号：RZ-VK1
- 新增独立 `XuanYu.Render.Vulkan` 项目，接入 `Silk.NET.Vulkan`，只负责最小 Vulkan 环境探针。
- 探针完成 Vulkan API 入口创建、Instance 版本枚举、PhysicalDevice 枚举，并输出中文诊断日志。
- UI 只通过 `VulkanProbeRoute.Run(vm)` 这一薄入口触发探针，未修改布局、输入或日志面板结构。
- 未接入 Surface、Swapchain、LogicalDevice、CommandPool、CommandBuffer，也未进入真实渲染循环。
- 新增审计文档 `docs/audit-RZ-VK1-vulkan-probe.md`，记录本轮文件清单、验证范围和下一步建议。

## v0.2.1.14-rz Vulkan 接入前置审计 (2026-07-07)
- 原历史编号：RZ-Fix3-0
- 新增 `docs/vulkan-preflight-audit-RZ-Fix3-0.md`，收口当前中央视口、Avalonia NativeControlHost、Win32 子窗口、Vulkan Surface/Swapchain 生命周期和 fallback 策略。
- 确认当前工程已经存在 `Viewport/Vulkan` 预接入代码，实际状态已超过纯审计阶段，应在 RZ-Fix3-A 中收口为最小 Clear Probe，而不是继续扩大到完整 Renderer。
- 明确 Vulkan 只允许进入中央视口链路：`UiRoot` -> `Main` -> `VulkanViewport` -> `VulkanNativeHost` -> `VulkanClearSession`。
- 明确低频日志边界：只记录初始化、失败、Swapchain 重建、释放等生命周期摘要，禁止每帧 Acquire / Present / RenderFrame 日志。
- 明确 fallback UI 要求：Vulkan 初始化失败时中央视口显示占位提示，并引导查看底部日志详情，不能白屏或崩溃。
- 保持顶部工具栏、左侧项目树、右侧检查器、底部日志系统职责不变；本次不接 Gizmo、Picking、模型、相机、资源系统。
- 验收：`dotnet restore` 通过；`dotnet build --no-restore` 通过，0 Warning / 0 Error；`.cs` / `.axaml` 文件未发现超过 100 行。

## v0.2.1.13-rz — Vulkan 接入前置验证 (2026-07-06)
- 原历史编号：RZ-Fix3-A
- 中央视口从静态假网格切换为 `VulkanViewport` 宿主，保留顶部/底部视口状态提示
- 新增 `Viewport/Vulkan` 小模块，使用 Avalonia `NativeControlHost` 在中央区域创建 Win32 子窗口作为 Vulkan Surface 承载点
- 新增 Silk.NET Vulkan 依赖：`Silk.NET.Vulkan` 与 `Silk.NET.Vulkan.Extensions.KHR`
- 最小 Vulkan 生命周期已接入：Instance / Win32 Surface / PhysicalDevice / LogicalDevice / Swapchain 创建与释放
- Resize 时跳过 0 尺寸与重复尺寸，并在尺寸变化时重建 Swapchain；仅记录重建成功 / 失败摘要
- Vulkan 初始化失败时显示中央 fallback 占位提示，不影响编辑器其他 UI
- 低频日志只记录 Vulkan 初始化开始、成功 / 失败、Swapchain 重建、释放完成；不写每帧日志
- 本轮不接模型、Gizmo、Picking、相机、资源系统，不改顶部、左侧、右侧、底部日志结构
- 当前为 Vulkan Host / Surface / Swapchain 前置验证；逐帧 Clear + Present 留到 RZ-Fix3-A-R1 收口
- Build: 0 Warning, 0 Error；GUI 烟测启动 5 秒存活

## v0.2.1.12-rz — 右侧检查器 / 调试 / 偏好 / 模式页收口 (2026-07-06)
- 原历史编号：RZ-Fix2-D
- 右侧面板收口为四个职责明确的页签：检查器、调试、偏好、模式
- 检查器页改为当前选中对象 / 项目的属性查看区，使用紧凑键值布局显示名称、类型和路径
- 检查器页补明确空状态：未选择对象时提示从左侧项目树、层级页或视口选择对象
- 调试页收口为当前上下文快照，分组显示当前上下文、当前对象、工具状态和输入状态，不显示日志流
- 偏好页保留编辑器偏好占位，说明布局保存、主题、快捷键和编辑器偏好后续在此收口
- 模式页显示当前工作模式与当前工具说明，作为模式状态占位
- 图标继续全部使用 SVG / PathIcon 资源，不使用字符图标、emoji 或 Unicode 图标符号
- 不改中央视口、不接 Vulkan、不改日志系统、不改顶部工具栏、不改左侧项目树
- Build: 0 Warning, 0 Error

## v0.2.1.11-rz — 左侧项目树视觉与层级收口 (2026-07-06)
- 原历史编号：RZ-Fix2-C
- 左侧项目区收口为更稳定的编辑器侧栏：项目 / 层级 Tab、搜索框、项目树、选中态、Hover 和空状态统一整理
- 项目页保留静态示例结构：SampleProject、世界、MainWorld、TestWorld、资源、图标、材质、脚本、构建
- 项目树行高统一为约 28px，一级、二级、三级缩进分别保持 0、18px、36px
- 选中态使用浅蓝背景和半粗文字，Hover 使用轻量底色，不抢中央视口视觉
- 搜索框文案统一为“搜索项目树...”，本轮不接真实搜索逻辑
- 层级页改为明确空状态：暂无场景对象，提示打开世界或创建对象后显示层级
- 图标继续全部使用 SVG / PathIcon 资源，不使用字符图标、emoji 或 Unicode 图标符号
- 不接真实资源扫描、不做导入导出、不做右键菜单、不改中央视口、不接 Vulkan、不改日志系统
- Build: 0 Warning, 0 Error

## v0.2.1.10-fix — Splitter 默认布局与最小宽度修复 (2026-07-06)
- 原历史编号：RZ-Fix2-B-R1
- 修复 RZ-Fix2-B 后左右面板可能被 splitter 或窗口压窄的问题
- 主布局根容器增加最小宽度兜底，避免左侧、中央、右侧的最小可用宽度总和被整体压穿
- 左侧项目列继续默认 270px，并在列定义与面板上双层限制 200px 至 420px
- 右侧检查器列继续默认 340px，并在列定义与面板上双层限制 260px 至 480px
- `UiRoot` 增加轻量 clamp：监听 splitter 改动后的列宽，超出范围时回弹到合法宽度
- 明确底部日志默认收起：只显示摘要条；点击展开后显示日志列表与详情，拖拽只调整底部区域高度
- 不改中央视口绘制逻辑、不接 Vulkan、不扩展日志系统、不接 Probe
- Build: 0 Warning, 0 Error

## v0.2.1.9-rz — 主布局 Splitter 可拖拽收口 (2026-07-06)
- 原历史编号：RZ-Fix2-B
- 主布局改为可拖拽尺寸骨架：左侧项目区、中央视口、右侧检查器、底部日志区域通过轻量 splitter 调整空间
- 左侧项目区默认约 270px，限制为 200px 至 420px，避免项目树被压没或过度挤占中央视口
- 右侧检查器默认约 340px，限制为 260px 至 480px，为属性、调试、偏好等后续内容预留可调空间
- 底部日志区域增加横向 splitter，展开时跟随底部行高伸缩，收起时保留摘要条语义
- Splitter 视觉统一为 6px 轻量分隔条，Hover 时轻微高亮，默认不抢顶部和视口视觉
- 仅调整主布局容器，不改中央视口绘制逻辑、不接 Vulkan、不扩展日志系统、不接 Probe
- Build: 0 Warning, 0 Error

## v0.2.1.8-rz — 顶部菜单栏与工具栏收口 (2026-07-06)
- 原历史编号：RZ-Fix2-A
- RZ-Fix1 日志阶段判定完成并冻结：后续只维护，不继续扩展 Probe、文件日志或诊断包
- 撤回 ProbeScope / Trace / 高频摘要预研入口，Probe 系统延期到真实 bug 复现且普通日志不足时再做
- 顶部区域继续保持两行结构，改为主命令区与编辑工具区的分组式布局
- 第一行按“文件 / 编辑 / 运行”分组，右侧保留克制的状态显示
- 第二行按“选择 / 变换 / 视图 / 辅助”分组，右侧保留当前工具状态
- 不改中央视口、不接 Vulkan、不扩展日志系统、不接 Probe
- Build: 0 Warning, 0 Error

## v0.2.1.7-fix — 日志详情可读性与复制验收 (2026-07-06)
- 原历史编号：RZ-Fix1-G-R1
- 右侧日志详情区改为更紧凑的可读布局：顶部聚合显示时间、级别、来源和分类
- 消息与详情继续使用只读正文区域，便于选择/复制日志正文
- 重复次数、上下文 ID、操作链路 ID 改为键值行，空值继续显示“无”
- 保留“复制详情”按钮与结构化中文复制文本格式
- 保持详情只由点击日志行选择驱动，不使用 Hover / PointerMoved 刷新
- Build: 0 Warning, 0 Error

## v0.2.1.6-rz — 日志详情面板与复制单条日志 (2026-07-06)
- 原历史编号：RZ-Fix1-G
- 底部日志展开区改为左侧日志列表 + 右侧日志详情，点击日志行后通过 `SelectedLogEntry` 显示详情
- 新增 `LogDetailPanel`，显示时间、级别、来源、分类、消息、详情、重复次数、上下文 ID、操作链路 ID
- 未选择日志时显示明确空状态：“未选择日志，点击左侧日志行后显示详情”
- 新增 `EditorLogClipboardText`，集中生成结构化中文复制文本，复制逻辑不写入 XAML 或主 VM
- 新增“复制详情”按钮，使用 Avalonia 剪贴板接口复制单条日志详情
- 日志详情由点击选择驱动，不使用 Hover / PointerMoved 刷新详情
- 保持普通 UI 标签不可选，仅日志消息和详情正文使用只读文本框便于复制
- `docs/diagnostic-safety.md` 补充日志详情选择规则：禁止 hover 驱动详情刷新
- Build: 0 Warning, 0 Error

## v0.2.1.5-fix — 构建环境与低频日志总线验收收口 (2026-07-06)
- 原历史编号：RZ-Fix1-F-R1
- `NuGet.Config` 移除缺失的 `.nuget-local` 本地源，改为只保留 `nuget.org`，避免新克隆仓库因本地源不存在而无法 restore
- `run.bat` 改为稳定入口：先 restore，再 `--no-restore` build，最后启动当前 `XuanYu.Editor.UI`
- 审计低频日志总线：`SampleLogEntries` 仅在 `UiVm` 实例初始化时作为种子进入实例内 Buffer，过滤切换不会重复追加种子日志
- 确认摘要条来自 `EditorLogSummary.From(_logBuffer.All)` 计算错误数、警告数和最近事件
- 确认过滤按钮只返回过滤视图，不删除 `EditorLogBuffer` 原始日志
- 搜索确认 `PointerMoved / Hover / DragPreview / RenderFrame / Picking Hover / Splitter Drag` 未写入普通底部日志
- `docs/diagnostic-safety.md` 补充后台任务日志规则：后台构建、导入、加载、保存或渲染摘要未来接入时必须通过日志队列或 UI 调度合批刷新，不得直接修改 UI 绑定集合
- Build: 0 Warning, 0 Error

## v0.2.1.4-rz — 低频日志总线接入 (2026-07-06)
- 原历史编号：RZ-Fix1-F
- 新增 `Vm/Logging` 低频日志模块：`EditorLogBus`、`EditorLogBuffer`、`EditorLogSummary`、`EditorLogFilter`、`EditorLogFilterQuery`、`EditorLogRepeatKey`
- 底部日志从纯 `SampleLogEntries` 过渡为 Buffer 驱动；`SampleLogEntries` 仅作为初始化种子，运行中的按钮命令和工具切换会通过 `EditorLogBus` 写入
- `EditorLogBuffer` 最多保留最近 500 条日志，并对连续相同日志使用 `RepeatCount` 合并
- 摘要条改为从 Buffer 真实计算错误数、警告数和最近事件
- 过滤按钮接入真实过滤：全部 / 信息 / 警告 / 错误 / 构建 / 任务 / 输入 / 渲染
- 首批只接低频 UI 事件：编辑器布局恢复、项目打开、启动渲染提示、新建/打开/保存/运行/停止/构建命令、工具切换
- 明确不接 PointerMoved / Hover / Picking Hover / DragPreview / RenderFrame / Splitter Drag / Vulkan 初始化 / 中央视口渲染链路
- `docs/diagnostic-safety.md` 补充低频日志准入清单和禁止高频接入清单
- `file-tree.md` 同步当前真实文件数：102

## v0.2.1.3-fix — 日志显示语义与高频风险小修审计 (2026-07-06)
- 原历史编号：RZ-Fix1-E-R1
- 底部日志显示层中文化：内部枚举仍保留 `Editor / Layout` 等稳定标识，UI 显示为“编辑器 / 布局 / 项目 / 加载 / 渲染 / 后端 / 输入 / 捕获”等中文文本
- 重复折叠确认绑定到对应日志行末尾，示例行显示“点击拾取未命中任何对象  重复 6 次”，不再像面板级状态
- 示例拾取日志从“拾取结果为空”改为“点击拾取未命中任何对象”，明确它是低频点击事实日志，不代表 Hover / PointerMoved 逐条输出
- 搜索框界面文案从开发占位“搜索占位”改为用户可见的“搜索日志...”
- `docs/diagnostic-safety.md` 新增“底部普通日志准入”规则：PointerMoved / Hover / DragPreview / RenderFrame / Picking Hover / Splitter Drag 禁止逐条进入底部日志
- 截图复查右侧“调试”页：当前上下文、当前对象、工具/输入状态以快照方式显示，不作为第二个日志面板
- Build: 0 Warning, 0 Error

## v0.2.1.2-rz — 日志系统布局与调试快照职责收口 (2026-07-06)
- 原历史编号：RZ-Fix1-E
- `file-tree.md` 重建为当前工作区真实文件树，按 `rg --files` 统计 95 个文件，删除旧文档中已不存在的历史项目/目录记录
- 底部日志栏从滚动文本占位升级为全局事实日志视图：摘要条、级别/来源过滤入口、搜索占位、列式日志列表、空状态与重复折叠占位
- 明确底部日志只展示低频事实记录，示例覆盖 Editor / Project / Render / Build / Task / Input，不接真实日志后端、不接 Vulkan、不改中央视口
- 新增轻量日志模型：`LogEntry`、`EditorLogLevel`、`EditorLogSource`、`EditorLogCategory`，字段预留 Detail / ContextId / CorrelationId / RepeatCount
- `SampleLogEntries` 替代旧 `LogText`，避免 UI 内硬编码纯字符串日志，为后续 `EditorLogBuffer / EditorLogBus` 接入预留边界
- 右侧“调试”页收口为当前状态快照：当前上下文、当前对象、工具状态、输入状态；不追加滚动日志，不与底部日志抢职责
- 调试示例文案明确高频事件策略：PointerMoved / Hover / DragPreview 后续走摘要、覆盖快照或探针，不逐条进入普通日志 UI
- 所有新增和修改的 `XuanYu.Editor.UI` `.cs / .axaml` 文件均保持 ≤100 行
- Build: 0 Warning, 0 Error；截图复查底部日志与右侧调试职责清晰

## v0.2.1.1-rz — Avalonia 编辑器 UI 骨架收口与底部日志栏接入 (2026-07-05 20:47)
- 原历史编号：RZ-Fix1-D
- 新增轻量 `XuanYu.Editor.UI` 编辑器外壳：顶部工具区、左侧项目/层级、中央深色视口、右侧检查器、底部状态/日志栏
- 顶部改为两行紧凑工具栏：第一行主命令（新建/打开/保存/撤销/重做/运行/停止），第二行编辑工具（选择/移动/旋转/缩放/框选/聚焦/平移/环绕/吸附）
- 顶部命令和编辑工具全部改为集中管理的 SVG / PathData 图标，禁止字符、Unicode、emoji 占位
- 左侧面板收口为 `项目 / 层级` 两个页签；项目树和层级树均使用 SVG 图标，去掉重复的“工具”页
- 右侧检查器收口为对象摘要 + 基础信息，删除重复的“当前选择”文案
- 左右侧栏页签统一为轻量 Tab Bar：浅蓝激活态、蓝色底线、非激活灰蓝文字
- 中央视口加入编辑器感占位：网格、原点轴线、视图标签、方向提示、操作提示
- 底部栏升级为可展开日志面板：默认一行日志摘要，点击后展开 `日志 / 问题 / 构建 / 任务` 四页签，日志格式保持中文
- 新增 `XuanYu.Editor.UI/Icons/EditorIcons.axaml` 集中管理 UI 图标
- 新增 `XuanYu.Editor.UI/Vm/LogText.cs` 保存静态中文日志示例
- Build: 0 Warning, 0 Error

## v0.1.8.10-fix — 9.0X Native Viewport 鼠标捕获生命周期审计与修复 (2026-06-26)
- 原历史编号：9.0D-R2E
- 收口所有 Win32 鼠标捕获到 `NativeViewportMouseCapture`，禁止其他模块直接调用 `SetCapture` / `ReleaseCapture`
- 修复 `WM_MBUTTONUP` 此前只清内部状态、不调用 `ReleaseCapture()` 导致 Native Viewport 继续吞鼠标消息的问题
- `Release(nint ownerHwnd, string reason)` 以 `GetCapture() == ownerHwnd` 作为是否真实释放的最终依据，不再依赖内部 `_captured` 标志
- 新增 `WM_CANCELMODE` 兜底释放路径
- `WM_KILLFOCUS` / `WM_DESTROY` / `Dispose` 均兜底释放或清理捕获状态
- `WM_CAPTURECHANGED` 只同步内部状态，新捕获窗口句柄从 `lParam` 读取，不递归调用 `ReleaseCapture()`
- 增加中文 probe log：`Debug.WriteLine` + `EditorProbe` 双写，记录捕获开始/释放/来源/按钮/hwnd/Win32 当前捕获/释放原因
- 新增审计文档 `docs/audit-NativeViewportMouseCapture-lifecycle-9.0X.md`
- 新增回归验证脚本 `tools/mouse_capture_lifecycle_verify.ps1`：中键旋转 + MoveGizmo 拖动自动验证
- 项目窗口标题改为动态读取程序集版本，build 后自动显示当前版本号
- Build: 0 Warning, 0 Error / Tests: 697/698 passed（1 个预存：中文排序依赖 locale）
- commits: `8d6e7fd` `a48ecfd`

## v0.1.8.9-fix — Gizmo 拖动 Preview 高频路径复审 (2026-06-25 22:56)
- 原历史编号：9.0D-R2D
- 修复 `TransformPreview` 帧完成后仍可能调用 Diagnostics refresh 的路径：Preview 回调改为只记录“跳过 Diagnostics 刷新”
- 补齐中文 probe log：PointerMoved、Gizmo hit/drag、Preview transform、RenderScene preview、Redraw、PickSnapshot、Dispatcher、Inspector、Diagnostics、WorldState、日志面板、WorldHierarchy
- 保留 DebugDock 轻量化结果：Diagnostics/Performance/RenderScene 页不复活，仅提供 no-op 兼容方法，避免重建重型 Avalonia UI
- 新增 `docs/gizmo_drag_audit_2026-06-25.md`：完整调用链、频率分级和日志结论
- 复现日志：`docs/gizmo_drag_audit_probe.log` 共 355 行；Preview 拖动中 UI/WorldState/Diagnostics/Inspector 均为 0 次，PickSnapshot 跳过 20 次

## v0.1.8.8-fix — Gizmo 拖动高频路径探针审计 (2026-06-25 21:41)
- 原历史编号：9.0D-R2C
- 目标：确认 Move Gizmo Preview 高频路径未触发 Inspector / Diagnostics / PickSnapshot / WorldState / Avalonia 重布局
- 在 PointerMoved → Gizmo Hit/Drag → Preview Transform → RenderScene 写入 → Redraw 请求全链路植入中文 probe log
- 探针字段：阶段名、耗时 ms、UI 刷新、WorldState 写入、Diagnostics 刷新、Inspector 刷新
- 审计结论：
  - Preview（TransformPreview）帧：UI=否 / WorldState=否 / Diagnostics=否 / Inspector=否 / PickSnapshot=跳过
  - Commit（EntityTransformChanged）帧：WorldState=是 / Inspector=是 / Diagnostics=是 / PickSnapshot=执行
  - PointerMoved 高频路径未触发 Inspector/Diagnostics/PickSnapshot/WorldState，符合 9.0D-R2B 优化目标
- 新增 `tools/gizmo_drag_postmessage.ps1`：通过 PostMessage 向 Vulkan 视口子窗口发送鼠标事件，复现拖动并采集探针日志
- 新增 `tools/gizmo_drag_audit.ps1`：SendInput 方案（未能穿透 WinExe 输入队列，保留参考）
- 新增 `artifacts/gizmo_drag_audit_probe.log`：本次审计原始探针日志（299 行）
- 新增 `XuanYu.Engine.Editor.Windows/Shell/Diagnostics/GizmoDrag/` 探针实现
- `EditorProbe` 同时输出到终端与 `%APPDATA%/XuanYuEngine/editor_probe.log`，便于 WinExe 审计采信
- Build: 0 Warning, 0 Error / Tests: 693/694 passed（1 个预存 flaky：中文排序依赖 locale）

## v0.1.8.7-fix — 降低 Move Gizmo 拖动帧负载 (2026-06-25 00:18)
- 原历史编号：9.0D-R2B
- TransformPreview 不再每帧刷新 Inspector
- TransformPreview 帧不再刷新 Diagnostics / DebugDock
- TransformPreview 帧不再重建 PickSnapshot
- AxisDragAnchorBuilder 删除未使用的 DragPlane 构建路径
- Inspector 更新保留在 TransformCommit / TransformCancel 路径
- Trace 审计确认未接入 UI 日志
- Build: 0 Warning, 0 Error / Tests: 693/694 passed
- commit `26f2006`

## v0.1.8.6-rz — 诊断日志与 UI 调度安全规范 (2026-06-24)
- 原历史编号：9.0D-R3
- 新增 `docs/diagnostic-safety.md`：收录 9.0D 诊断回调导致 UI 卡死事故的根因与防护规范
- 覆盖启动期规则 / 高频路径规则 / 诊断 Sink 接口 / UI 日志异步投递 / 代码审查清单
- commit `e57d5c9`

## v0.1.8.5-rz — 选中实体自动显示并可拖 Move Gizmo (2026-06-24)
- 原历史编号：9.0D-R2
- **取消「按 G 才显示 Gizmo」的交互入口**，改为选中实体 + 相机有效即自动显示
- 改动 4 个点：
  - `MoveGizmoFrameSource.Build`：闸门从 `MoveToolActive` 改为 `selectedEntity.IsValid || MoveToolActive`
  - `MoveGizmoVisibility.ShouldShow`：同步去掉 `moveToolActive` 参数
  - `EditorTransformInputRoute.HandlePointerMoved`：Gizmo Hover 检测改为选中实体即启用
  - `EditorSceneToolInputRoute.HandlePressed`：去掉 `IsMoveToolActive`，选中实体即可拖动
- G 键保留为快捷移动入口，不再是 Gizmo 出现的必要条件
- build: 0 Error / test: 693/694 (1 pre-existing)
- commit `e66cbb4`

## v0.1.8.4-rz — Move Gizmo 轴约束求解器 (2026-06-24)
- 原历史编号：9.0D-R1
- **AxisDragAnchorBuilder 重写**：从 ScreenProjection 升级为 DragPlane 射线约束方案
- **Gram-Schmidt 法线构造**：轴约束平面包含目标轴并尽量面向摄像机，三级 fallback
- **双保险**：DragPlane 优先 + ScreenProjection 降级，Builder 必定返回有效锚点
- **TransformDragRoute 重构**：Move 方法提取公共 MoveDrag 辅助，Axis/Plane 共用
- **诊断追踪**：TransformDragRoute 添加 Trace 回调，输出 Begin/Move/Confirm 位置值与模式
- **死代码清理**：删除 AxisTranslationStart.cs（旧 ScreenProjection 实现）
- **新增测试**：AxisPlaneTranslationSolverTests（X/Y/Z 轴正交性 / 45°视角无倍率异常 / 射线平行失败路径 / Gram-Schmidt 数学正确性）
- 后续修复 4 轮（提交 7c88740 → ba85b92 → 4ac549e → c82bb41）
- build: 0 Error / test: 693/694 (1 pre-existing)
- 当前 Gizmo 可见时直接拖轴即可移动，不必按 G

## v0.1.8.3-rz — Inspector 与 Transform 同步 (2026-06-24)
- 原历史编号：9.0C
- WorldState 新增 `SetRotation()` / `SetScale()`
- 新增 TransformEdit 应用层（TransformInspectorSnapshot / TransformEditRequest / TransformEditResult / SelectedEntityTransformReader / SelectedEntityTransformApply）
- Inspector 支持显示并编辑 Position / RotationDegrees / Scale
- Apply 层统一校验 Scale > 0 / NaN / Infinity，非法值拒绝写入
- 旧 Position 编辑链路保持向后兼容
- Transform 修改后标记 Dirty + 请求视口刷新
- 预留 `SetSnapshot()` 入口供后续 Gizmo 同步 Inspector 使用
- 审计文档 `docs/audit-inspector-transform-9.0C-0.md`
- 新增测试 15 项（Reader 3 + Apply 11 + SaveLoad 1）
- build: 0 Error / test: 685/686 (1 pre-existing)
- commits: `69f056b` `d1cc14c` `d4cb881` `8fb4aaa`

## v0.1.8.2-rz — TransformComponent 补全：Position + Rotation + Scale (2026-06-24)
- 原历史编号：9.0B
- 新增 RotationComponent / ScaleComponent（Engine 层实体组件）
- TransformComponentDocument 增加 RotationDegrees / Scale（可空，兼容旧文件）
- WorldState 支持旋转/缩放存储与查询
- WorldDocumentValidator 增加 RotationDegrees 有限校验 / Scale 有限+正数校验
- 旧版只有 Position 的 world 文件兼容加载（缺 Rotation→补 0,0,0；缺 Scale→补 1,1,1）
- WorldStateDocumentConvert 单向/双向转换同步支持完整 Transform
- 新增/更新测试 20 项（Writer/Reader/Validator/RoundTrip）
- build: 0 Error / test: 670/671 (1 pre-existing)
- commits: `80230a2` `222f49a` `2043f4e` `39dc201`

## v0.1.8.1-rz — World 保存 / 加载 (2026-06-24)
- 原历史编号：9.0A
- 新增 WorldDocument / WorldEntityDocument / TransformComponentDocument / WorldVector3Document / WorldMetadataDocument 文档模型
- 新增 WorldDocumentReader / WorldDocumentWriter 支持 .world.json 读写
- 新增 WorldDocumentValidator（SchemaVersion / WorldId / EntityId / Transform Position 校验）
- 编辑器打开项目时自动加载 Content/Worlds/main.world.json
- 运行菜单新增「保存 World」入口
- 新增 WorldState ↔ WorldDocument 转换
- 新增保存 / 读取 / 校验 / RoundTrip 测试 23 项
- build: 0 Error / test: 661/662 (1 pre-existing, SampleProjectSmokeTests 受未跟踪 Content/ 目录影响)
- commits: `8bd920b` `825f3b3` `70099ce` `8f3d9a1`
- 新增 4 项命名回潮门禁测试（CodeFileBudgetTests +4 → 14 项）：
  - `NoNamespaceFluidWarfare` — 禁止生产代码出现 namespace FluidWarfare
  - `NoUsingFluidWarfare` — 禁止生产代码出现 using FluidWarfare
  - `NoXClassFluidWarfare` — 禁止 .axaml 出现 x:Class="FluidWarfare.*
  - `NoClrNamespaceFluidWarfare` — 禁止 .axaml 出现 clr-namespace:FluidWarfare.*
- 允许的例外：EditorSettingsPath.LegacyAppFolder / EditorSettingsPathMigration / 历史文档
- 确认最终残留仅限：R4 Legacy 迁移路径 / 历史记录 / LEGACY 文档 / naming 说明
- docs/naming-XuanYu-Engine.md 标记 RZ 完成
- build: 0 Error / test: 638/639 (1 flaky pre-existing)
- 架构门禁：14/14
- commit `fbf509b`

## v0.1.7.1-fix — Editor 启动 AccessViolation 修复 (2026-06-24 11:45)
- 原历史编号：8.8-RZ-Fix1
- **根因**：`EditorShellComposition.Build()` 中初始化顺序错误 — `ProjectBootstrapRoute` 在第 41 行创建时引用了 `ctx.HierarchyRoute`，但 `HierarchyRoute` 直到第 45 行才赋值，导致 `hierarchyRoute` 为 null
- **现象**：Editor 启动崩溃，退出码 -1073741819（0xC0000005），实际为 NullReferenceException
- **修复**：将 `ctx.HierarchyRoute = new(...)` 移到 `ctx.ProjectBootstrapRoute = new(...)` 之前
- **验收**：
  - build: 0 Error / 0 Warning ✅
  - test: 638/639 通过（1 个预存 flaky：中文排序依赖 locale）
  - Editor 启动：成功，不再崩溃
  - 架构门禁：14/14
- 附带修复：run.bat CRLF 行尾（Windows 批处理兼容性）
- commit `359e3ce`

## v0.1.7.4-rz — 应用图标入库 (2026-06-24 12:30)
- 原历史编号：8.8-RZ-Fix1d
- 将仓库根目录 `LOGO.png`（1254×1254）复制到 `Assets/Icons/logo.png` 作为应用图标
- `.csproj` 注册 `logo.png` 为 `AvaloniaResource`（同时补注新的 ViewportNavigation SVGs）
- `MainWindow.axaml` 设置 `Icon="/Assets/Icons/logo.png"`，标题栏显示玄域引擎 LOGO
- `file-tree.md` 同步记录
- build: 0 Error / 0 Warning ✅ / test: 638/639（1 flaky pre-existing）

## v0.1.7.3-rz — 视口导航按钮 SVG 图标资源入库 (2026-06-24 12:16)
- 原历史编号：8.8-RZ-Fix1c
- 新增 4 个 SVG 图标资源到 `Assets/Icons/ViewportNavigation/`：
  - `nav_pan.svg` — 四向箭头，表示平移视图
  - `nav_frame.svg` — 取景框角 + 中心点，表示聚焦/查看全部
  - `nav_projection_persp.svg` — 视锥图形，表示透视投影
  - `nav_projection_ortho.svg` — 网格方框，表示正交投影
- 所有 SVG 使用 `viewBox="0 0 30 30"` + `currentColor`，匹配按钮尺寸且支持主题色
- `file-tree.md` 同步记录新资源
- 路线规划：短期为资源预案，后续接 Avalonia Overlay 或 Vulkan 贴图渲染路径
- build: 0 Error / 0 Warning ✅ / test: 638/639（1 flaky pre-existing）/ 架构门禁 14/14

## v0.1.7.2-fix — Warning 全清理 (2026-06-24 12:05)
- 原历史编号：8.8-RZ-Fix1b
- **7 个 Warning 逐项处理**：
  - `VulkanScene3dFrameHandles.cs` — 去重 `using Silk.NET.Vulkan`
  - `VulkanScene3dSwapchainCreateResult.cs` — `Message` 改为 `string?`
  - `VulkanScene3dRendererProbeFrame.cs` — `r.Vk` 增加 null 安全检查
  - `EditorViewportInputRequest.cs` — `ToolPalette` 改为 `ViewportToolPalette?`
  - `EditorTransformInputRequest.cs` — `ToolPalette` 改为 `ViewportToolPalette?`
  - `EditorShellGroundPointerRoute.cs` — suppress CS9113（API 设计预留）
  - `EditorPickInputRoute.cs` — `applySelection` 改为 `Action<string?,...>`
  - `StatusBarPanel.SetCurrentSelection` — 改为 `string?`，null→"无"
  - `VulkanScene3dFrameResult.Failed` — 参数改为 `string?`
  - `VulkanScene3dSession.FailFrame` — 参数改为 `string?`
- **验收**：build 0 Error / 0 Warning ✅ / 架构门禁 14/14 ✅
- commit `e3f644f`

## v0.1.6.11-rz — 用户数据目录迁移 (2026-06-24 10:08)
- 原历史编号：8.8-R4
- 编辑器设置目录从 `%APPDATA%/FluidWarfare/` 迁移到 `%APPDATA%/XuanYuEngine/`
- 新增 `EditorSettingsPathMigration.cs`：旧→新目录迁移逻辑（幂等、不覆盖、不崩溃）
- `EditorSettingsPath.cs`：新增 `CurrentAppFolder = "XuanYuEngine"` / `LegacyAppFolder = "FluidWarfare"`
- `EditorSettingsPath.cs`：GetSettingsFilePath 首次调用时触发迁移
- 迁移策略：新目录存在→跳过 / 仅旧目录存在→复制 / 新旧都不存在→默认
- 不删除旧目录 / 不覆盖新目录已有文件 / 迁移失败不阻止编辑器启动
- 对应测试 5 项（EditorSettingsPathMigrationTests.cs）
- 生产 `Input/Settings/` 目录 5 文件（合规）
- build: 0 Error / test: 634/635 (1 flaky pre-existing)
- 架构门禁：10/10
- commit `644aff7`

## v0.1.6.10-rz — namespace 迁移全仓收口 (2026-06-24 09:54)
- 原历史编号：8.8-R3-Z
- 全仓 namespace FluidWarfare.* 清零确认 ✅
- AboutFluidWarfareWindow → AboutXuanYuEngineWindow（类名 + 文件名 + x:Class + 全部引用）
- 清理 14 处非 namespace 的 FluidWarfare 字符串（Vulkan 窗口标题 / Win32 类名 / 日志 / 测试路径等）
- 删除 docs/reports/namespace-migration-R3-plan.md（生命周期完成）
- 更新 docs/naming-XuanYu-Engine.md R3 状态、file-tree.md
- 残留说明：EditorSettingsPath.AppFolderName = "FluidWarfare" 保留到 R4
- build: 0 Error / test: 629/630 (1 flaky)
- commit `710dd88`

## v0.1.6.9-rz — Tests namespace 迁移 (2026-06-24 09:48)
- 原历史编号：8.8-R3-4
- 迁移 namespace `FluidWarfare.Tests.*` → `XuanYu.Engine.Tests.*`（73 文件）
- 全仓 namespace `FluidWarfare.*` 清零 ✅
- 剩余：EditorSettingsPath（R4）/ AboutFluidWarfareWindow（R3-Z）/ 历史记录
- build: 0 Error / test: 629/630
- commit `5c8966b`

## v0.1.6.8-rz — Editor.Windows 全仓 namespace + x:Class 成对迁移 (2026-06-24 09:42)
- 原历史编号：8.8-R3-3BC
- 合并 R3-3B + R3-3C 为原子提交（partial class 必须同 namespace）
- 244 纯 C# + 16 .axaml.cs + 16 .axaml x:Class + 7 clr-namespace
- GlobalUsings.cs: 43 条 Editor.Windows 全局 using（100 行门禁）
- 清零：namespace/x:Class/clr-namespace FluidWarfare.Editor.Windows 全部 ✅
- build: 0 Error / test: 629/630
- commit `775ba48`

## v0.1.6.7-rz — Render 层 namespace 迁移 (2026-06-24 09:10)
- 原历史编号：8.8-R3-2
- 迁移 Render/Render.Vulkan namespace：`FluidWarfare.Render.*` → `XuanYu.Engine.Render.*`
- Render：47 文件 namespace + 147 文件跨项目 using；Render.Vulkan：154 文件 namespace
- 修复 1 处完全限定类型引用；相机白名单文件 namespace 正确迁移
- Editor/Tests namespace 保持不动（R3-3/R3-4）
- x:Class/EditorSettingsPath 未改动
- build: 0 Error / test: 629/630 (1 flaky)
- commit `aa94a43`

## v0.1.6.6-rz — 底层模块 namespace 迁移 (2026-06-24)
- 原历史编号：8.8-R3-1
- 迁移 Core/Engine/Project/Bridge namespace：`FluidWarfare.*` → `XuanYu.Engine.*`
- 模块内 namespace 声明：36 文件；全仓 using 引用：209 文件；总计 185 文件改动
- 命名映射：`FluidWarfare.Core→XuanYu.Engine.Core`, `FluidWarfare.Engine→XuanYu.Engine`（注意无 `.Engine` 后缀）, `FluidWarfare.Project→XuanYu.Engine.Project`, `FluidWarfare.Bridge.ProjectEngine→XuanYu.Engine.Bridge.ProjectEngine`
- Render/Editor/Tests namespace 保持不动（R3-2/R3-3/R3-4）
- x:Class/EditorSettingsPath 未改动
- build: 0 Error / test: 629/630 (1 flaky)
- commit `6a90c9e`

## v0.1.6.5-rz — docs audit 文件清理 (2026-06-24)
- 原历史编号：8.8-R2C
- 删除 14 个临时 audit-* / whitelist-* / renderer-* 文件
- 旧 `docs/CHANGELOG.md`（179KB，表格密集）→ `changelog.md`（简洁格式，倒序）
- `file-tree.md` 中 31KB 的"未发布变更日志"区 → 指向 `changelog.md` 的引用
- build: 0 Error / test: 629/630 (1 flaky)
- commit `68ffde8`

## v0.1.6.4-rz — 旧占位目录清理 (2026-06-24)
- 原历史编号：8.8-R2B
- 删除 9 个仅含 `.gitkeep` 的空占位目录：`FluidWarfare.AI` / `Combat` / `Data` / `Ecs` / `Exporter` / `Runtime.Android` / `Runtime.Windows` / `Simulation` / `World`
- 删除审计确认：9 个文件全部为 `.gitkeep`，无误伤
- 未来需要时按命名规范重新声明（`XuanYu.Engine.*` / `XuanYu.SunWu.*` / `XuanYu.Tools.*`）
- build: 0 Error / test: 629/630 (1 flaky)
- commit `5bdda34`

## v0.1.6.3-rz — 工程外壳迁移 (2026-06-24)
- 原历史编号：8.8-R2
- `.sln` / 9 项目目录 / `.csproj` / `ProjectReference` 全部迁至 `XuanYu.Engine.*`
- 映射：`FluidWarfare.Core→XuanYu.Engine.Core`, `FluidWarfare.Engine→XuanYu.Engine`（无后缀）, `FluidWarfare.Editor.Windows→XuanYu.Engine.Editor.Windows`, 等
- 同步更新：`InternalsVisibleTo` / `app.manifest` / 测试路径常量 / PowerShell 脚本 / `.gitkeep`
- 故意保留：`namespace FluidWarfare.*`（R3）, `using FluidWarfare.*`（R3）, `x:Class`（R3）, `EditorSettingsPath.AppFolderName`（R4）
- build: 0 Error / test: 629/630 (1 flaky)
- commit `6ad57bd`

## v0.1.6.2-rz — 品牌换名：玄域引擎 (2026-06-24)
- 原历史编号：8.8-R0/R1
- 正式技术品牌从 FluidWarfare 迁移为"玄域引擎 / XuanYu Engine"
- 窗口标题：`FluidWarfare Editor` → `XuanYu Engine Editor`
- About 窗口：品牌名 / 版权 → XuanYu Engine / 玄域引擎贡献者
- 菜单：`关于 FluidWarfare` → `关于 玄域引擎`
- 示例项目描述：`FluidWarfare 示例项目` → `玄域引擎 示例项目`
- Vulkan app/engine name：8 文件 → `"XuanYu Engine"`
- 文档标题：CHANGELOG / AI 规则 / 代码宪法 / 命名规则 / file-tree / shaders
- namespace / .sln / .csproj / 程序集名 / 目录名未改动
- build: 0 Error / test: 629/630 (1 flaky)
- commit `71d6187`

## v0.1.6.1-rz — 架构防回潮门禁 (2026-06-24)
- 原历史编号：8.8-0
- `CodeFileBudgetTests.cs` 新增 5 个门禁测试：
  - `ProductionWhitelist_OnlyApproved` — 生产白名单精确锁死为 2 个相机文件
  - `GlobalUsings_Max100Lines` — `GlobalUsings.cs` ≤ 100 行
  - `EditorShellContext_Max95Lines` — `EditorShellContext.cs` ≤ 95 行
  - `EditorShell_NotInWhitelist` — EditorShell 不得回归白名单
  - `DirectoryWhitelist_RemainsZero` — 目录白名单保持 0
- build: 0 Error / test: 629/630 (1 flaky)
- commit `4c4d82c`

## v0.1.5.21-rz — EditorShell 组合根彻底薄化 (2026-06-23)
- 原历史编号：8.7.8-Z2
- `EditorShell.axaml.cs`：3,041→28 行，**从白名单移除**
- 95 个 using 移入 `GlobalUsings.cs`
- 新建 Composition 架构：
  - `EditorShellContext.cs` (88 行) — 上下文持有
  - `EditorShellComposition.cs` (59 行) — Build
  - `EditorShellCompositionRuntime.cs` (65 行) — 运行时
  - `EditorShellEventWiring.cs` (67 行) — 事件接线
  - `EditorShellLifecycle.cs` (29 行) — 生命周期
- 生产白名单：3→2（只剩两个相机算法文件）
- build: 0 Error / test: 624/625 (1 flaky)
- commit `913b66b`

## v0.1.5.20-rz — EditorShell 收口审计 (2026-06-23)
- 原历史编号：8.7.8H-5
- EditorShell 从 3,041 行压到 491 行（含 using，body ~396 行），累计削减 2,550 行
- 决策：Transform 管线暂缓（收益 ~30 行，风险影响全链路）
- 决策：EditorShell 白名单保留（组合根例外）
- 后续策略：只出不进，新增职责必须进 Route / 子模块
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.19-rz — EditorShell P2 中等风险清理 (2026-06-23)
- 原历史编号：8.7.8H-4B
- 提取日志委托 → `Shell/Diagnostics/Log/EditorShellLogRoute.cs` (18 行)
- 提取视口焦点 → `Shell/Viewport/EditorShellViewportFocusRoute.cs` (41 行)
- 提取 Scene3D 命令 → `Shell/Scene3D/EditorShellScene3dCommandRoute.cs` (19 行)
- EditorShell: 496→491 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.18-rz — EditorShell P1 低风险清理 (2026-06-23)
- 原历史编号：8.7.8H-4A
- 提取 Raw 输入处理 → `Shell/Input/Raw/EditorShellRawInputRoute.cs` (26 行)
- 提取视口帧命令 → `Shell/Viewport/EditorShellViewportFrameRoute.cs` (43 行)
- 提取视口尺寸工具 → `Shell/Viewport/EditorShellViewportSizeGuard.cs` (24 行)
- 删除空 `ExecuteTransformApply`（无调用者）
- EditorShell: 656→496 行（含 using，body ~403 行）
- build: 0 Error / test: 624/625 (1 flaky，白名单不删)

## v0.1.5.17-rz — EditorShell 第七刀：项目加载 + World Bootstrap (2026-06-23)
- 原历史编号：8.7.8H-2G
- 提取项目加载残留 → `Shell/Project/EditorShellProjectBootstrapRoute.cs` (46 行)
- EditorShell: 576→567 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.16-rz — EditorShell 第六刀：Startup Vulkan Probe (2026-06-23)
- 原历史编号：8.7.8H-2F
- 提取 Startup Vulkan Probe → `Shell/Startup/EditorShellStartupVulkanProbeRoute.cs` (46 行)
- EditorShell: 589→576 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.15-rz — EditorShell 第五刀：层级树 + 选择同步 (2026-06-23)
- 原历史编号：8.7.8H-2E
- 提取层级树 → `Shell/Hierarchy/EditorShellHierarchyRoute.cs` (37 行)
- 提取选择同步 → `Shell/Selection/EditorShellSelectionSyncRoute.cs` (51 行)
- EditorShell: 622→589 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.14-rz — EditorShell 第四刀：窗口菜单命令 (2026-06-23)
- 原历史编号：8.7.8H-2D
- 提取窗口命令 → `Shell/Commands/EditorShellWindowCommandsRoute.cs` (24 行)
- EditorShell: 629→622 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.13-rz — EditorShell 第三刀：Viewport 生命周期 + Vulkan Redraw (2026-06-23)
- 原历史编号：8.7.8H-2C
- 提取 Viewport 重绘 → `Shell/Viewport/EditorShellViewportRedrawRoute.cs` (83 行)
- EditorShell: 665→629 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.12-rz — EditorShell 第二刀：Transform 编辑 + Scrub (2026-06-23)
- 原历史编号：8.7.8H-2B
- 提取 Transform 路由 → `Shell/Transform/EditorShellTransformRoute.cs` (62 行)
- 提取 Scrub → `Shell/Transform/EditorShellScrubRoute.cs` (24 行)
- EditorShell: 725→665 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.11-rz — EditorShell 第一刀：Overlay 导航 + 地面指针 + Picking (2026-06-23)
- 原历史编号：8.7.8H-2A
- 提取 Overlay 导航 → `Shell/Navigation/EditorShellOverlayNavigationRoute.cs` (78 行)
- 提取地面指针 → `Shell/Picking/EditorShellGroundPointerRoute.cs` (63 行)
- 提取 Picking → `Shell/Input/Picking/EditorPickInputRoute.cs` (79 行)
- EditorShell: 969→725 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.10-rz — EditorPreferencesWindow SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8G-2
- `EditorPreferencesWindow.axaml.cs`：587→78 行
- 提取 Capture 逻辑 → `EditorPreferencesCapture.cs` (77 行)
- 提取 BindingList 管理 → `EditorPreferencesBindingList.cs` (81 行)
- 提取 DraftHandler → `EditorPreferencesDraftHandler.cs` (79 行)
- 提取 Helpers → `EditorPreferencesHelpers.cs` (30 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.9-rz — VulkanRenderContext SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8F-2
- `VulkanRenderContext.cs`：476→92 行
- 提取 Context Setup → `Context/VulkanRenderContextSetup.cs` (78 行)
- 提取 Device Selector → `Context/VulkanRenderContextSelector.cs` (32 行)
- 死代码锁定 Legacy
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.8-rz — VulkanClearProbe SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8E-2B
- `VulkanClearProbe.cs`：416→99 行
- 提取 ContextScope → `Clear/Probe/VulkanClearProbeContextScope.cs` (96 行)
- 提取 DeviceSelector → `Clear/Probe/VulkanClearProbeDeviceSelector.cs` (42 行)
- 提取 SurfaceQuery → `Clear/Probe/VulkanClearProbeSurfaceQuery.cs` (60 行)
- 提取 RenderTargetScope → `Clear/Probe/Render/VulkanClearProbeRenderTargetScope.cs` (98 行)
- 提取 RenderSubmitScope → `Clear/Probe/Render/VulkanClearProbeRenderSubmitScope.cs` (54 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.7-rz — Clear 目录容量整理 (2026-06-23)
- 原历史编号：8.7.8E-2A
- `Clear/Probe/` 目录 9→6 文件（容量达标）
- build: 0 Error / test: 624/625

## v0.1.5.6-rz — VulkanSwapchainProbe SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8D-2B
- `VulkanSwapchainProbe.cs`：301→78 行
- 提取 ContextScope → `Swapchain/Probe/VulkanSwapchainProbeContextScope.cs` (100 行)
- 提取 DeviceSelector → `Swapchain/Probe/VulkanSwapchainProbeDeviceSelector.cs` (46 行)
- 提取 SurfaceQuery → `Swapchain/Probe/VulkanSwapchainProbeSurfaceQuery.cs` (64 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.5-rz — Swapchain 目录容量整理 (2026-06-23)
- 原历史编号：8.7.8D-2A
- `Swapchain/` 子目录重建：Probe/ / Context/ / Image/ / Sync/
- 文件迁移确保 ≤5/目录
- build: 0 Error / test: 624/625

## v0.1.5.4-rz — GameProjectLoader SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8C-2
- `GameProjectLoader.cs`：392→82 行
- 提取 ManifestReader → `Loading/GameProjectManifestReader.cs` (89 行)
- 提取 FolderParser → `Loading/GameProjectFolderParser.cs` (100 行)
- 提取 ExtensionParser → `Loading/GameProjectExtensionParser.cs` (52 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.3-rz — VulkanDeviceProbe SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8B-4
- `VulkanDeviceProbe.cs`：288→77 行
- 提取 InstanceScope → `Device/VulkanDeviceInstanceScope.cs` (61 行)
- 提取 Selector → `Device/VulkanDeviceSelector.cs` (80 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.2-rz — VulkanSurfaceProbe SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8B-2
- `VulkanSurfaceProbe.cs`：203→66 行
- 提取 InstanceScope → `Surface/VulkanSurfaceInstanceScope.cs` (98 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.1-rz — WindowsViewportInputTranslator SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8A-2
- `WindowsViewportInputTranslator.cs`：284→54 行
- 拆为：`WindowsViewportModifierState.cs` (37) / `WindowsViewportRawInputTranslate.cs` (76) / `WindowsViewportGestureMatch.cs` (28)
- 白名单：1 项删除
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.4.11-rz — 全仓白名单债务审计与 8.7.8 路线图 (2026-06-23)
- 原历史编号：8.7.7F
- F-1：全仓盘点，49 项行白名单 + 8 项目录白名单
- F-2：3 项立即可清（InspectorPanel 84 行 / NativeHost 87 行 / SceneCameraPose 99 行）
- F-3：9 文件压缩 + 4 目录子目录化，13 项白名单删除
  - 文件压缩：WorldHierarchyTreeIndex (112→46) / ProjectContentNodeView (114→52) / WorldHierarchyTreeBuilder (126→49) / EditorInputActionCatalog (148→60) / VulkanInstanceProbe (122→43) / VulkanDebugMessengerScope (133→55) / VulkanValidationAvailabilityProbe (118→40) / GameContentFileScanner (130→38) / WorldState (121→48)
  - 目录子目录化：Validation 7→5, Camera 7→5, ProjectContentTree 6→5, Transform/Drag 6→5
- F-4A：DebugDockPanel SRP 提取 (145→53) / ViewportPlaceholderPanel SRP 提取 (189→46) / WorldHierarchy 目录 8→5
- F-4B：Panels/Viewport 11→5 文件 / Transform/Gizmo 8→5 文件
- F-4C：ViewportNavigation 9→5 文件（最后 1 个目录白名单删除）/ VulkanSceneRayBuilder SRP 167→40 / VulkanCameraMatrices SRP 189→21
- F-5：大债务登记到 8.7.8（13 项生产白名单分 A/B/C/D 四类）
- F-6：最终收口 — VulkanViewportHostPanel 158→43 / EditorInputBindingSnapshot 175→38；SceneNavigationCameraMotion(173) 与 SceneOrbitCameraMotion(202) 因相机算法放弃
- build: 0 Error / test: 625/625

## v0.1.4.10-rz — 全仓白名单深度清理 (2026-06-22)
- 原历史编号：8.7.7E
- E-1：VulkanScene3dRenderer SRP — 主文件 261→41 行，5 子模块全部 ≤100
- E-2A：Scene3D Session SRP — Session 主文件 371→46，CreateInstance/FrameFlow/Handles/FrameAcquire/Lifecycle
- E-2B：Swapchain SRP — 去重合并后 6 文件，全 ≤100
- E-2C：VulkanScene3dRenderer 去重式 SRP — 消除 3 个重复文件
- E-2D：Scene3D 白名单删除 + Overlay 目录 8→4
- 最终 9 文件白名单删除，ViewportNavigation 目录白名单清理
- build: 0 Error / test: 625/625

## v0.1.4.9-rz — 目录子目录化 + 文件重组 (2026-06-22)
- 原历史编号：8.7.7D
- D-1：Shell/Scene3D/ 11→5 文件（Scene3dFrameState/Scene3dDrawListBuilder/Scene3dPresentedState 迁入子目录）
- D-2：Viewport/Picking/ 重构 + Viewport/Transform/ 子目录重组
- build: 0 Error / test: 625/625

## v0.1.4.8-rz — NativeHost / ViewportPlaceholder / DebugDock SRP (2026-06-22)
- 原历史编号：8.7.7C
- C-1：NativeHost.axaml.cs 158→43 行（HWND 生命周期提取 / HostInfo 提取 / Input 提取）
- C-2：ViewportPlaceholderPanel.axaml.cs 189→46 行
- C-3：DebugDockPanel.axaml.cs 145→53 行
- 白名单 -3
- build: 0 Error / test: 625/625

## v0.1.4.7-rz — Project / World Tree Panels SRP (2026-06-22)
- 原历史编号：8.7.7B
- `ProjectContentTreePanel.axaml.cs`：128→63 行
- `WorldHierarchyTreePanel.axaml.cs`：229→95 行
- 新建 WorldHierarchyTreeItems.cs(14) / TreeExpansion.cs(43) / TreeSelection.cs(87)
- 白名单 -2
- build: 0 Error / test: 625/625

## v0.1.4.6-rz — InspectorPanel SRP 拆分 (2026-06-22)
- 原历史编号：8.7.7A
- `InspectorPanel.axaml.cs`：145→53 行
- 提取 TransformHeader.cs(31) / EntityIdRow.cs(26) / GroupSeparator.cs(16)
- 白名单 -1
- build: 0 Error / test: 625/625

## v0.1.4.5-rz — EditorShell Route 化重构 Phase 3：Composition (2026-06-21 ~ 22)
- 原历史编号：8.7.6
- 8.7.6.8C — Startup Bootstrap / Lifecycle / Vulkan Probe Route 化
- 8.7.6.8D-1 — Input Pipeline / Raw Viewport Events 提取
- 8.7.6.8D-2 — Transform / SceneTool Input Bridge
- 8.7.6.8D-3 — Ground Hover / Pick Bridge
- 8.7.6.8D-4 — Scene3D Manual Run / Session Commands
- 8.7.6.8D-5 — Panel Operation Apply
- 8.7.6.8E-1 — Transform / Ground Placement Apply 收口
- 8.7.6.8E-2 — Diagnostics / Refresh / Probe Residual 收口
- 8.7.6.8E-3 — Constructor / FindControls / Route Wiring
- 8.7.6.8E-3R/4 — Composition Cleanup + Final Stabilization（EditorShell 3,041→2,157 行）
- build: 0 Error / test: 625/625

## v0.1.4.4-rz — EditorShell Route 化 Phase 2：Selection & Gizmo (2026-06-20 ~ 21)
- 原历史编号：8.7.5
- 8.7.5.1-3 — Scene3D Frame Route 提取（FrameRoute/FrameState/DrawListBuilder/PresentedState）
- 8.7.5.4 — Viewport Pointer Pick Route
- 8.7.5.5A-C — Transform Interaction（State/Result/PointerRoute/KeyboardRoute/StartRequest）
- 8.7.5.5D — Transform Application（Preview/Commit/Cancel/Result）
- 8.7.5.5E — Gizmo（MoveGizmoDrawList/Element/HitTest/Interaction/Layout/VisualState/Snapshot）
- 8.7.5.6A-C — Camera Route / Focus / Navigation
- 8.7.5.6D-E — Frame Submit / Session Lifecycle
- 8.7.5.6F-G — Diagnostics / Vulkan Probe
- 8.7.5.7A-C — Selection Presenter / Route / State
- 8.7.5.8A — Project Bootstrap Route
- 8.7.5.8B — World Bootstrap Route (EntitySeed/RenderSeed/Input/Result)
- EditorShell 3,041→1,200+ 行（Route 化 Phase 1 后继续拆解）

## v0.1.3.9-vk — Scene3D 渲染与选择系统独立 (2026-06-18 ~ 20)
- 原历史编号：8.7.4
- Scene3D 渲染模块独立化（Scene3dFrameRun/Scene3dSessionLifecycle 等）
- 选择系统 Route 化（EditorSelectionRoute/State/Request/Result/Reason）
- 选择呈现（ViewportSelectionPresenter/WorldEntitySelectionPresenter）
- Picking 管线独立（ViewportPointerPickRoute）
- 多对象绘制与 Depth Buffer

## v0.1.3.8-vk — Vulkan 管线稳定化与 Swapchain 重构 (2026-06-17 ~ 18)
- 原历史编号：8.7.3
- Swapchain API 结果加固与生命周期规则收口
- Vulkan Clear 与 Swapchain Probe 重构
- Surface/Device/Instance 创建链路稳定化

## v0.1.4.3-rz — Transform 编辑基础 (2026-06-16 ~ 17)
- 原历史编号：8.7.2
- 单实体 Transform 编辑与地面放置
- 3D 地面拾取、世界坐标反馈与落点标记
- Gizmo 基础呈现（MoveGizmo）

## v0.1.4.2-rz — 视口与输入系统 (2026-06-15 ~ 16)
- 原历史编号：8.7.1
- 默认 3D 主视口、俯视矩阵修复
- Windows 原生视口子窗口宿主完善
- 输入管线路由化（RawInput→Transform→SceneTool）

## v0.1.4.1-rz — Shell Route 化 Phase 1 (2026-06-14 ~ 15)
- 原历史编号：8.7.0
- EditorShell 从 ~3,041 行开始 Route 化重构
- 第一批 Route 提取：Startup、Lifecycle、Log、PanelSwitch
- Route 装配与组合根（EditorShellComposition）

## v0.1.3.7-vk — 3D 地面拾取与 World Hierarchy (2026-06-12 ~ 14)
- 原历史编号：8.6
- 3D 地面拾取、世界坐标反馈与落点标记
- World Hierarchy 节点树与编辑器选择收口
- SVG 经典资源管理器式双树菜单
- 左侧双树页签、项目文件树与中文界面收口

## v0.1.3.6-vk — World Hierarchy 与选择系统 (2026-06-11 ~ 12)
- 原历史编号：8.5
- World Hierarchy 节点树（WorldHierarchyNode/TreeBuilder/Search）
- 编辑器选择 Route 化
- 项目内容树面板拆分

## v0.1.3.5-vk — 3D Picking 与单位选择 (2026-06-10 ~ 11)
- 原历史编号：8.4
- 3D Picking 管线（ScenePointerPicker/SceneRayGroundIntersection）
- 单位选择与高亮
- Picking 与选择 Route 化

## v0.1.3.4-vk — 持久 Scene3D 渲染会话与 RTS 相机 (2026-06-09 ~ 10)
- 原历史编号：8.3
- 持久 Scene3D 渲染会话（Session/Surface/Swapchain/Lifecycle）
- RTS 相机基础控制（ViewportNavigation）
- Overlay 渲染

## v0.1.3.3-vk — 多对象 3D 绘制与 Depth Buffer (2026-06-08 ~ 09)
- 原历史编号：8.2
- 多对象 3D 绘制（顶点缓冲/索引缓冲）
- 基础 Depth Buffer
- Ground Cursor 绘制

## v0.1.3.2-vk — Vulkan 3D 基础管线 (2026-06-06 ~ 08)
- 原历史编号：8.1
- Vulkan 3D 基础管线（ShaderModules/PipelineLayout/Pipelines/CommandRecorder）
- Scene3D 隔离（手动触发，不与 Editor 自动绑定）
- SPIR-V 手写编码废弃 → 标准 glslangValidator 工具链
- Validation Layer 开关接入
- Scene3D Renderer SRP 拆分

## v0.1.3.1-vk — RenderScene GPU 点位绘制 (2026-06-05 ~ 06)
- 原历史编号：8.0
- RenderScene 单对象 GPU 点位绘制
- Vulkan 战场视口填充与重绘修复
- 多对象点位绘制

## v0.1.2.2-vk — Vulkan 最小可见渲染闭环 (2026-06-04 ~ 05)
- 原历史编号：7.8
- 最小可见渲染闭环（CreateInstance→CreateSurface→CreateDevice→CreateSwapchain→Render→Present→Cleanup）
- Swapchain 扩展加载修复
- 底部调试终端与主视口收束

## v0.1.2.1-vk — Vulkan 基础集成 (2026-06-02 ~ 04)
- 原历史编号：7.0~7.7
- Vulkan 最小清屏（Clear Probe）
- Vulkan Instance 最小创建与释放
- Vulkan Device 最小选择与释放
- Vulkan Surface 宿主边界
- Windows 原生视口子窗口宿主
- Vulkan Surface 创建成功回归

## v0.1.1.5-rz — RenderScene 抽象 (2026-06-01 ~ 02)
- 原历史编号：6.0~6.1
- RenderScene 最小抽象
- 视口 RenderScene 调试显示

## v0.1.1.4-rz — World 实体与选择 (2026-05-31 ~ 06-01)
- 原历史编号：5.0~5.3
- 最小 World 实体
- 从项目内容生成占位实体
- 最小 World 实体列表面板
- World 实体选择与视口联动占位

## v0.1.1.3-rz — 项目系统 (2026-05-30 ~ 31)
- 原历史编号：4.3~4.4
- 项目内容文件入口声明与扩展名校验
- 项目校验报告

## v0.1.1.2-rz — 核心值对象与初始骨架 (2026-05-29 ~ 30)
- 原历史编号：2.x~4.x
- 解决方案骨架、项目宪章、架构说明、AI 开发规则、代码宪法、命名规则
- `EntityId` / `TimeStep` / `SimulationTime` / `Vector3d` / `YawRotation` / `EngineError` / `EngineResult`
- 初始项目内容文件入口声明
- 中文化补丁：明确人类可读文本默认使用中文

## v0.1.1.1-rz — 初始创建 (2026-05-28)
- 原历史编号：0.0.1-dev
- 创建初始解决方案骨架（`FluidWarfare.sln`）
- 创建顶层模块目录和资源目录规划
- 创建项目宪章、架构说明、AI 开发规则、代码宪法、命名规则、Phase 1 范围和旧仓库考古报告
- 创建 `.gitattributes`，固定 Markdown、解决方案、C# 和 JSON 文件使用 LF 行尾
- 创建 `FluidWarfare.Core` 纯 C# 类库项目
- 创建 `FluidWarfare.Tests` xUnit 测试项目
- 创建 `CoreSmokeTests` 最小冒烟测试
- `docs/MILESTONE1_PUBLIC_VALIDATION.md`：记录公开 GitHub Raw 验收命令
