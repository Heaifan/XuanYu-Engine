# 项目文件树 — XuanYu Engine

## ARCH-B-R4-R1 Win32 子窗口 Pointer 消息转发修复快照 (2026-07-17 21:56:17)
推进到 `v0.2.16.11-fix`，只修复 NativeControlHost 未收到 Win32 子窗口鼠标消息导致真实视口拖动无反应的问题；不修改 Vulkan、Picking、Gizmo、WorldState、Undo / Redo 或存档格式。
- `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs`  # Win32 子窗口创建与基础生命周期；窗口过程改为输入路由入口，仍不承载 Vulkan 资源逻辑。
- `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.Input.cs`  # Win32 子窗口鼠标消息转发；注册输入 Sink 并转发左键按下、移动、释放、捕获丢失和失焦。
- `XuanYu.Editor.UI/Viewport/Vulkan/NativePointerMessage.cs`  # Win32 Pointer 消息快照；保存消息类型、按钮状态和物理像素坐标。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`  # NativeHost 生命周期主体；创建子窗口后注册输入 Sink，销毁前清理 Sink。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs`  # 真实 Pointer 路由分部；接收 Win32 子窗口消息，把物理坐标转回逻辑坐标并提交既有交互事务。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题版本同步为 `玄域引擎编辑器 v0.2.16.11-fix`。
- `run.bat`  # 仓库根启动脚本；控制台标题同步为 `XuanYu Engine Editor v0.2.16.11-fix`。

## ARCH-B-R4 真实视口 Pointer 输入事务闭环快照 (2026-07-17 21:28:27)
推进到 `v0.2.16.10-rz`，只把真实视口 PointerPressed / PointerMoved / PointerReleased 接入既有交互事务 Owner；不修改 Vulkan、Picking、Gizmo、WorldState、Undo / Redo 或存档格式。
- `XuanYu.Editor.UI/EditorState/EditorInteractionPointerSnapshot.cs`  # 真实 Pointer 事务只读快照；记录 PointerId、逻辑起点、当前点、位移和 Preview 次数，不持有控件或 Vulkan 对象。
- `XuanYu.Editor.UI/EditorState/EditorInteractionSnapshot.cs`  # 交互捕获只读快照；新增 Pointer 快照字段，继续由 Owner 统一发布。
- `XuanYu.Editor.UI/EditorState/EditorInteractionCommand.cs`  # 交互事务命令；Begin / Preview / Commit 携带 Pointer 快照或 PointerId，用于 Owner 边界校验。
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.Interaction.cs`  # 交互事务唯一 Owner；新增 PointerId 校验，防止非当前指针 Preview / Commit。
- `XuanYu.Editor.UI/Vm/UiVm.InteractionPointer.cs`  # 真实视口 Pointer 意图转换；只允许移动工具启动真实拖动，并把逻辑像素坐标提交给 Owner。
- `XuanYu.Editor.UI/Vm/UiVm.Interaction.cs`  # 交互事务 VM 入口；补充窗口失焦和 PointerCaptureLost 的统一 Cancel 入口。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs`  # NativeHost 的 Avalonia Pointer 路由分部；接入按下、移动、释放和捕获丢失，不改 Attach / Resize / Present / Detach 生命周期。
- `XuanYu.Editor.UI/Win/UiWin.axaml.cs`  # 主窗口代码后置；窗口失焦时统一取消当前交互事务。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题版本同步为 `玄域引擎编辑器 v0.2.16.10-rz`。
- `run.bat`  # 仓库根启动脚本；控制台标题同步为 `XuanYu Engine Editor v0.2.16.10-rz`。

## ARCH-B-R3-R1 Escape 路由与宪法优先级修复快照 (2026-07-17 21:00:05)
推进到 `v0.2.16.9-fix`，只修复 R3 人工验收发现的 Escape 无响应和临时调试按钮裁切，并补充开发宪法优先级；不修改 Vulkan、Picking、Gizmo、WorldState、Undo / Redo 或存档格式。
- `XuanYu.Editor.UI/Win/UiWin.axaml.cs`  # 主窗口代码后置；新增窗口级 Escape 隧道路由，统一调用 VM 的交互 Cancel 入口，不负责事务状态所有权。
- `XuanYu.Editor.UI/Right/Right.axaml`  # 右侧调试页；临时事务探针按钮改为中文短文案，避免裁切，不作为正式用户功能。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题版本同步为 `玄域引擎编辑器 v0.2.16.9-fix`。
- `run.bat`  # 仓库根启动脚本；控制台标题同步为 `XuanYu Engine Editor v0.2.16.9-fix`。
- `docs/玄域引擎_AI开发宪法.md`  # 最高开发规范；新增宪法优先级条款，用户提示与宪法冲突时必须以宪法为准并说明原因。

## ARCH-B-R3 交互捕获事务边界快照 (2026-07-17 20:15:16)
推进到 `v0.2.16.8-rz`，只建设交互捕获 Owner 与 Preview / Commit / Cancel 边界；不开发真实 Gizmo、Picking、WorldState 写入、Undo / Redo、存档格式，不修改 Render.Vulkan / Swapchain / Present。
- `XuanYu.Editor.UI/EditorState/EditorInteractionSnapshot.cs`  # 交互捕获只读快照；记录 Revision、是否捕获、SessionId、OwnerTool、开始快照、最新 Preview 和阶段，不负责真实场景状态。
- `XuanYu.Editor.UI/EditorState/EditorInteractionCommand.cs`  # 交互 Begin / Preview / Commit / Cancel 命令；表达具体事务意图，不引入通用命令总线。
- `XuanYu.Editor.UI/EditorState/EditorInteractionChangedResult.cs`  # 交互状态变化结果；记录旧快照、新快照和变化类型，不反向修改 Owner。
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.Interaction.cs`  # Editor 交互捕获所有者；唯一管理捕获会话、Owner 校验、Preview 覆盖、Commit / Cancel 回 Idle。
- `XuanYu.Editor.UI/Vm/UiVm.Tool.cs`  # UI 工具切换分部；工具切换前取消当前交互，不直接持有正式工具状态。
- `XuanYu.Editor.UI/Vm/UiVm.Interaction.cs`  # UI 交互事务分部；提供调试命令和统一 Cancel 入口，不写正式场景状态。
- `XuanYu.Editor.UI/Right/Right.axaml`  # 右侧调试页；新增最小 Begin / Preview / Commit / Cancel 测试面板。
- `XuanYu.Editor.UI/Left/Left.axaml.cs` / `Win/UiWin.axaml.cs` / `Viewport/Vulkan/VulkanNativeHost.cs`  # Escape、窗口关闭、NativeHost Detach 汇聚到 VM Cancel；不改变渲染生命周期顺序。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题版本同步为 `玄域引擎编辑器 v0.2.16.8-rz`。
- `run.bat`  # 仓库根启动脚本；控制台标题同步为 `XuanYu Engine Editor v0.2.16.8-rz`。

## v0.2.16.7-fix Vulkan 既有 5+100 纯结构拆分快照 (2026-07-14 22:41:08)
在进入 ARCH-B-R3 前，先治理两个既有 5+100 超限文件；本轮只拆分职责，不改变 Attach、Resize、Present、自愈或释放逻辑。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`  # NativeHost 主生命周期与尺寸路径；保留创建、Attach、Resize、Detach、Dispose 编排，不负责后台日志线程派发实现。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Log.cs`  # NativeHost Vulkan 日志 UI 线程派发分部；只负责把后台 Present 泵日志切回 UI 线程并交给既有 Route，不改变日志内容或生命周期。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.cs`  # Swapchain 创建、重建、自愈与释放主体；不再承载只读访问器集中声明，不改变 Swapchain/ImageView 销毁顺序。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.Accessors.cs`  # Swapchain Owner 只读访问器与内部日志辅助；只暴露既有格式、Extent、ImageViews、Swapchain、KHR 和 ResourceGeneration。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题版本同步为 `玄域引擎编辑器 v0.2.16.7-fix`。
- `run.bat`  # 仓库根启动脚本；控制台标题同步为 `XuanYu Engine Editor v0.2.16.7-fix`。

## ARCH-B-R2 工具状态所有权快照 (2026-07-14 22:25:24)
推进到 `v0.2.16.6-rz`，只治理活动工具状态、工具捕获状态和状态栏语义边界；不开发真实 Gizmo、Picking、Transform Preview、场景存档，不修改 Vulkan、Resize、Present 或 Bridge 生命周期。
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.cs`  # Editor 状态所有者主分部；继续负责选择状态转换、线程门禁和选择快照，不直接依赖 Avalonia 控件、窗口或 Vulkan。
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.Tool.cs`  # Editor 工具状态所有者分部；唯一写入活动工具快照，重复提交同一工具返回 NoChange，不负责真实捕获输入或 Gizmo 行为。
- `XuanYu.Editor.UI/EditorState/EditorToolId.cs`  # 工具身份与捕获状态枚举；定义当前 UI 可切换的工具集合和最小捕获状态，不负责按钮文案或输入分发。
- `XuanYu.Editor.UI/EditorState/EditorToolText.cs`  # 工具中文文案与稳定工具身份之间的映射；不承担本地化系统或 UI 控件职责。
- `XuanYu.Editor.UI/EditorState/EditorToolSnapshot.cs`  # 工具只读快照；包含 Revision、活动工具和捕获状态，不泄漏可变 UI 字段。
- `XuanYu.Editor.UI/EditorState/EditorToolCommand.cs`  # 工具切换命令；只表达“请求切换到某工具”的具体意图，不引入通用命令总线。
- `XuanYu.Editor.UI/EditorState/EditorToolChangedResult.cs`  # 工具状态变化结果；记录旧 Revision、新 Revision 和新旧快照，不反向修改 Owner。
- `XuanYu.Editor.UI/Vm/UiVm.cs`  # UI ViewModel；工具名称、按钮高亮和 Footer 工具文本从 Owner 工具快照派生，按钮只提交切换意图，不再正式持有活动工具状态。
- `XuanYu.Editor.UI/Vm/UiVm.Selection.cs`  # 选择提交逻辑；选择对象后保持编辑器阶段为就绪，不再把“聚焦”写入通用状态，避免与聚焦工具混淆。
- `XuanYu.Editor.UI/Top/Top.axaml`  # 顶部工具栏；工具按钮高亮为单向快照显示，点击只调用切换工具命令。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题版本同步为 `玄域引擎编辑器 v0.2.16.6-rz`。
- `run.bat`  # 仓库根启动脚本；控制台标题同步为 `XuanYu Engine Editor v0.2.16.6-rz`。

## ARCH-B-R1-R3 跨树选择同步修复快照 (2026-07-14 21:15:02)
推进到 `v0.2.16.5-fix`，只修复项目树 / 层级树选中值回写与 Escape 清空链路；不改变树视觉，不新增正式状态种类，不开发 Picking / Gizmo / 存档，不修改 Vulkan、Resize、Present 或 Bridge 生命周期。
- `XuanYu.Editor.UI/Left/Left.axaml`  # 左侧项目树 / 层级树；`SelectedItem` 显式 `Mode=TwoWay`，确保视觉选中写回 `UiVm` 并进入同一个 Owner。
- `XuanYu.Editor.UI/Left/Left.axaml.cs`  # 左侧选择控件代码后置；Escape 同时清空项目树和层级树选中行，触发既有 Clear 路径。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题版本同步为 `玄域引擎编辑器 v0.2.16.5-fix`。
- `run.bat`  # 仓库根启动脚本；控制台标题同步为 `XuanYu Engine Editor v0.2.16.5-fix`。

## ARCH-B-R1-R2 树形视觉与 Inspector 元数据修复快照 (2026-07-14 20:47:57)
推进到 `v0.2.16.4-fix`，保留当前状态 Owner 链路，仅修复左侧树 UI 表达和 Inspector 元数据；不新增正式状态种类，不开发 Picking / Gizmo / 存档，不修改 Vulkan、Resize、Present 或 Bridge 生命周期。
- `XuanYu.Editor.UI/Vm/EditorTreeNode.cs`  # UI 专用树节点模型；描述当前样例项目树 / 层级树的 Key、标题、类型、路径、缩进和图标可见性；不负责场景模型、持久化或全局实体 ID。
- `XuanYu.Editor.UI/Vm/UiVm.Selection.cs`  # UI ViewModel 的选择提交与清空选择逻辑；把树节点元数据转为具体选择命令，并保持两个左侧列表缓存不争抢正式状态。
- `XuanYu.Editor.UI/Vm/UiText.cs`  # 静态 UI 样例数据；项目/层级从普通字符串列表升级为带语义的树节点列表，不负责真实场景数据源。
- `XuanYu.Editor.UI/Left/Left.axaml`  # 左侧项目树 / 层级树视觉；恢复缩进、节点图标、父子层级和真实选中行表达，不负责 Picking 或 Gizmo。
- `XuanYu.Editor.UI/Right/Right.axaml`  # Inspector 面板；路径字段绑定 `SelectionPath`，字号层级与左侧页签/树项收敛。
- `XuanYu.Editor.UI/EditorState/EditorSelectionCommand.cs` / `EditorSelectionSnapshot.cs` / `EditorStateOwner.cs`  # 选择命令和快照携带稳定 Key、标题、类型、路径，Owner 继续只负责选择状态转换与 NoChange 判定。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题版本同步为 `玄域引擎编辑器 v0.2.16.4-fix`。
- `run.bat`  # 仓库根启动脚本；控制台标题同步为 `XuanYu Engine Editor v0.2.16.4-fix`。

## ARCH-B-R1-R1 选择幂等与清空入口修复快照 (2026-07-14 20:26:21)
推进到 `v0.2.16.3-fix`，仅修复 ARCH-B-R1 收口缺口：重复选择 / 重复清空不再制造伪变化，左侧现有选择控件提供真实清空路径；不新增状态种类，不开发 Picking / Gizmo / 存档，不修改 Vulkan、Resize、Present 或 Bridge 生命周期。
- `XuanYu.Editor.UI/EditorState/EditorSelectionSnapshot.cs`  # 当前选择不可变快照；新增稳定 `SelectionKey` 用于幂等比较，不负责全局实体 ID 或持久化身份系统。
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.cs`  # Editor 状态所有者；`Select` / `Clear` 在状态未变化时返回 NoChange（`null`），不递增 Revision、不返回伪变化结果；仍不依赖 Avalonia、Vulkan、Silk、Window 或控件。
- `XuanYu.Editor.UI/Vm/UiVm.cs`  # UI ViewModel；项目/层级选择缓存统一提交给 Owner，null 选择进入清空命令，避免两个绑定缓存争抢正式选择状态。
- `XuanYu.Editor.UI/Left/Left.axaml`  # 左侧项目/层级区；改用现有 `ProjectItems` / `HierarchyItems` 绑定列表提供真实选择路径，不负责场景模型、Picking 或 Gizmo。
- `XuanYu.Editor.UI/Left/Left.axaml.cs`  # 左侧选择控件代码后置；Escape 清空当前列表选择，触发既有 null 绑定路径，不新增长期无用按钮。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题版本同步为 `玄域引擎编辑器 v0.2.16.3-fix`。
- `run.bat`  # 仓库根启动脚本；控制台标题同步为 `XuanYu Engine Editor v0.2.16.3-fix`。

## ARCH-B-R1 最小状态所有权闭环快照 (2026-07-14 19:35:40)
按 `v0.2.16.2-rz` 建立当前选择状态的最小 Editor State Owner。UI 只提交选择意图，正式状态由 Owner 持有，检查器从只读快照派生显示；本轮不开发 Picking / Gizmo / 存档，不修改 Vulkan、Resize、Present 或 Bridge 生命周期。
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.cs`  # Editor 状态所有者；当前只负责选择状态的唯一正式写入、校验、Revision 递增、快照生成和变更结果返回；不负责 Avalonia 控件、窗口、Vulkan、Silk、NativeHost、Swapchain 或渲染会话。
- `XuanYu.Editor.UI/EditorState/EditorSelectionSnapshot.cs`  # 当前选择状态的不可变快照；包含 `Revision`、是否有选择、标题和来源；不泄漏可变集合、ViewModel 或 Avalonia 属性。
- `XuanYu.Editor.UI/EditorState/EditorSelectionCommand.cs`  # ARCH-B-R1 的具体选择命令；只定义 `SelectEditorItemCommand` / `ClearEditorSelectionCommand`，不负责通用命令总线、字典 payload 或全局 EventBus。
- `XuanYu.Editor.UI/EditorState/EditorStateChangedResult.cs`  # 状态变化事实结果；记录旧 Revision、新 Revision、变化类型和新快照；不负责再次触发写入请求。
- `XuanYu.Editor.UI/Vm/UiVm.cs`  # UI ViewModel；项目树 / 层级树选择 setter 仅提交具体选择命令，检查器绑定从 Owner 快照读取；保留展示绑定、Footer 摘要和日志入口，不再作为选择状态正式 Owner。

## DOC-GIT-PUSH-1 Git 远端备份规则快照 (2026-07-13 23:41:18)
将“提交后必须 Push 当前工作分支到 GitHub”固化为长期协作规则，本轮不修改运行逻辑。
- `docs/玄域引擎_AI开发宪法.md`  # 最高开发规范；默认流程新增 Push 步骤，Git Push 章节改为每轮 Commit 后必须推送远端工作分支，同时保留 main 合并、PR、Tag、Release、强推、Rebase、重写历史的确认红线。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题版本同步为 `玄域引擎编辑器 v0.2.16.2-rz`。
- `run.bat`  # 仓库根启动脚本；控制台标题同步为 `XuanYu Engine Editor v0.2.16.2-rz`，启动项目仍为 `XuanYu.Editor.App`。

## ARCH-B-Plan 状态所有权审计快照 (2026-07-13 23:33:18)
ARCH-A 封版后进入 `v0.2.16.1-rz`，本轮只新增状态所有权规划文档并同步可见版本号，不修改运行逻辑。
- `docs/arch-b-plan.md`  # ARCH-B 规划文档：审计当前 `UiVm`、Viewport、层级树、Inspector、工具、日志与 NativeHost 写入路径；规划最小 Editor State Owner、只读快照、Preview / Commit / Cancel 边界和 R1-R4 分轮；不负责实现状态 Owner、不开发 Picking / Gizmo / 场景存档、不修改 Vulkan。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题版本同步为 `玄域引擎编辑器 v0.2.16.1-rz`，继续仅承担窗口壳定义。
- `run.bat`  # 仓库根启动脚本；控制台标题同步为 `XuanYu Engine Editor v0.2.16.1-rz`，启动项目仍为 `XuanYu.Editor.App`。

## ARCH-A-R4-R2 版本格式守卫快照 (2026-07-13 23:19:16)
修正无效开发期版本号 `v0.2.15.7-r1-rz`，将任务轮次保留在任务编号中，版本号推进为 `v0.2.15.8-fix`。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口标题同步合法开发期版本：`玄域引擎编辑器 v0.2.15.8-fix`。
- `run.bat`  # 控制台标题同步合法开发期版本，启动项目仍为 `XuanYu.Editor.App`。
- `scripts/arch-a-guard.ps1`  # ARCH-A 自动守卫脚本；新增开发期版本格式校验，当前允许 `rz` / `fix` / `vk` 类型，并继续校验标题、run.bat 与 changelog 顶部版本一致。

## ARCH-A-R4-R1 唯一启动入口守卫快照 (2026-07-13 23:04:38)
补齐 R4 总封版前的守卫缺口，确保解决方案内只有 `XuanYu.Editor.App` 是可执行启动入口。
- `XuanYu.Editor.Win/XuanYu.Editor.Win.csproj`  # 旧 WinForms 壳项目；移除 `OutputType=WinExe`，保留为非独立启动项目，避免与 `Editor.App` 形成双可执行入口。
- 已删除 `XuanYu.Editor.Win/Program.cs`  # 旧 WinForms 启动入口；删除后 `Editor.Win` 不再声明独立 `Main`。
- `scripts/arch-a-guard.ps1`  # ARCH-A 自动守卫脚本；新增 `OutputType` 检查，强制只有 `XuanYu.Editor.App` 可声明 `WinExe/Exe`。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口标题同步本轮版本：`玄域引擎编辑器 v0.2.15.7-r1-rz`。
- `run.bat`  # 控制台标题同步本轮版本，启动项目仍为 `XuanYu.Editor.App`。

## ARCH-A-R4 架构守卫与标题版本快照 (2026-07-13 22:53:06)
在 ARCH-A-R3 真机验收通过后，新增自动守卫并固化窗口标题版本号规则；本轮不修改 Vulkan 渲染主链。
- `scripts/arch-a-guard.ps1`  # ARCH-A 自动守卫脚本；检查 `Editor.UI` 禁止引用 `Render.Vulkan` / `Silk.NET.Vulkan`、`Render.Abstractions` 禁止引用 Vulkan/Silk/Avalonia、`Editor.App` 组装 UI 与 Vulkan、`run.bat` 启动 App、解决方案六项目、主窗口标题版本号和 5+100。
- `XuanYu.Editor.UI/Win/UiWin.axaml`  # 主窗口定义；标题显示产品名与当前开发版本号：`玄域引擎编辑器 v0.2.15.7-rz`。
- `run.bat`  # 仓库根启动脚本；继续唯一启动 `XuanYu.Editor.App`，控制台标题同步当前版本号。
- `docs/玄域引擎_AI开发宪法.md`  # 最高开发规范；补充窗口标题版本号必须随轮次更新，并强化 `changelog.md` 时间必须精确到秒。

## ARCH-A-R3 UI Vulkan 直接依赖移除快照 (2026-07-13 22:38:14)
在 R2 Resize / 代际一致性真机验收通过后，移除 `Editor.UI` 的旧 Vulkan / Silk 直接依赖与 fallback 链路，UI 只保留抽象渲染桥入口。
- `XuanYu.Editor.UI/XuanYu.Editor.UI.csproj`  # UI 类库项目；移除 `Silk.NET.Vulkan`、`Silk.NET.Vulkan.Extensions.KHR` 和 `XuanYu.Render.Vulkan` 引用，仅保留 Avalonia、Core、Render.Abstractions。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Bridge.cs`  # NativeHost 桥接创建入口；仅接受 `INativeHostSurfaceBridgeFactory` 应用注入，缺失注入时明确失败，不再 fallback 到 Vulkan 实现。
- `XuanYu.Editor.UI/Bootstrap/App.axaml.cs`  # UI Application；不再运行旧 VulkanProbe，启动后只创建携带抽象 factory 的 `UiVm`。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml`  # 视口 UI 壳；fallback 文案改为后端中性表达，不承担后端探针职责。
- 已删除 `XuanYu.Editor.UI/VulkanProbeRoute.cs`、`XuanYu.Editor.UI/Vm/UiVm.VulkanProbe.cs`、`XuanYu.Editor.UI/Viewport/Vulkan/VulkanSurfaceBridgeProvider.cs`、`XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession*.cs`  # 历史探针、旧 fallback 与早期 ClearSession 死链；不再属于当前运行路径。

## ARCH-A-R2-R2 Swapchain 代际依赖修复快照 (2026-07-13 22:24:18)
修复日志栏 Resize 后 Swapchain 实际换代但 Framebuffer / CommandBuffer 因同 extent 被错误跳过重建，最终触发 `QueueSubmit ErrorDeviceLost` 的生命周期缺陷。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.cs`  # Swapchain 持有者；新增资源代际 `ResourceGeneration`，仅在 Swapchain / ImageView 集合实际换代时推进，供上层判断依赖资源是否必须重建。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.cs`  # Framebuffer / CommandBuffer 持有者；`RebuildFramebuffers` 支持强制重建，Swapchain 换代时即使 extent 相同也必须重建 FB 并重录 CB。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameLogFormatter.cs`  # ClearFrame / Resize 日志格式化；新增低频中文 Swapchain 代际与 Resize 跳过日志，不逐帧输出。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Resize.cs`  # UI 合并 Resize 入口；改为查询当前 Surface extent，若 Present 自愈已完成当前尺寸则在 Recreate 前快速跳过，否则按资源代际强制重建依赖资源。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Recover.cs`  # Present OutOfDate 自愈入口；Swapchain 换代后强制重建 FB / CB，并输出中文代际日志。

## ARCH-A-R2-R2 布局同步高度滞后修复快照 (2026-07-13 22:05:10)
修复日志详情栏展开/收起后 Win32 子窗口已到正确物理尺寸，但 Vulkan Swapchain / Framebuffer 仍可能沿用旧高度导致底部黑屏的问题。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.LayoutSync.cs`  # 日志栏展开/收起后的最终布局同步路径；先按逻辑尺寸乘 DPI 调整 Win32 子窗口并输出探针，再把逻辑尺寸交给 `NativeHostResizeCoalescer` 延后合并触发桥接 Resize，避免 Surface CurrentExtent 未稳定时重建 Swapchain。
- `XuanYu.Editor.UI/NativeHostResizeCoalescer.cs`  # 既有低频 Resize 合并器；本轮未改代码，但 LayoutSync 现在复用它承担延后触发桥接 Resize 的职责。

## ARCH-A-R2-R1 DPI 物理尺寸修复快照 (2026-07-13 21:42:14)
修复新 App 启动入口下 DPI 虚拟化与逻辑/物理尺寸混用导致的左上角绘制问题。
- `XuanYu.Editor.UI/app.manifest`  # App 可执行入口复用的 Windows manifest；声明 PerMonitorV2 DPI awareness，不负责渲染逻辑。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`  # NativeHost 控件；SizeChanged 时用物理像素调整 Win32 子窗口，逻辑尺寸仍交给渲染桥日志/请求路径。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Bridge.cs`  # Bridge 创建与工厂来源日志；优先使用 App 注入 factory，旧 provider 仅作 fallback。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Dpi.cs`  # 逻辑尺寸到物理像素尺寸换算工具；不触碰 Vulkan 对象。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.LayoutSync.cs`  # 日志栏展开/收起同步路径复用同一物理尺寸换算；R2-R2 起桥接 Resize 改为延后合并触发。
- `run.bat`  # 仓库根启动脚本；唯一启动 `XuanYu.Editor.App`，使用 UTF-8，透传退出码，失败时保留窗口。

## ARCH-A-R2 Avalonia 应用组装层快照 (2026-07-13 20:44:15)
新增真实应用启动与依赖组装入口，避免 UI 项目直接作为活动启动入口。
- `XuanYu.Editor.App/XuanYu.Editor.App.csproj`  # Avalonia WinExe 启动项目；引用 UI、Abstractions、Vulkan；不负责 Swapchain、Resize、Present 或 ViewModel 业务状态。
- `XuanYu.Editor.App/Program.cs`  # App 启动入口；AttachConsole 后通过 Avalonia `AppBuilder` 创建注入了抽象 factory 的 UI App；不创建窗口业务对象以外的渲染资源。
- `XuanYu.Editor.App/EditorCompositionRoot.cs`  # 应用组装根；创建 `VulkanNativeHostSurfaceBridgeFactory` 并以 `INativeHostSurfaceBridgeFactory` 暴露；不持有或释放 Bridge 实例。
- `XuanYu.Editor.UI/Bootstrap/App.axaml.cs`  # UI Application；可接收抽象 Bridge factory 并传给 `UiVm`；加载既有 App.axaml，不重复声明样式资源。
- `XuanYu.Editor.UI/Vm/UiVm.cs`  # UI 状态模型；保存可选 `INativeHostSurfaceBridgeFactory`，不认识 Vulkan 具体实现。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`  # NativeHost 控件；优先用 VM 注入的抽象 factory 创建 Bridge，旧 provider 仅作兼容 fallback；创建时输出低频中文日志标明工厂来源。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanSurfaceBridgeProvider.cs`  # 旧 UI 入口兼容 fallback；新 App 启动路径不应使用它直接创建 Vulkan Bridge。
- `XuanYu.Editor.UI/XuanYu.Editor.UI.csproj`  # UI 类库项目；不再作为活动 WinExe 启动入口，旧 Vulkan/Silk 引用暂留给 R3+ 清理。
- `XuanYu.Engine.slnx`  # 当前分支唯一权威解决方案入口；纳入 Core、Render.Abstractions、Render.Vulkan、Editor.UI、Editor.Win、Editor.App 六个项目供 restore/build/test 使用。当前分支不存在 `XuanYu.Engine.sln`。

## ARCH-A-R1 最小生命周期契约快照 (2026-07-13 20:30:44)
建立 NativeHost 渲染桥最小装配契约，Vulkan 侧开始适配；不修改 UI 旧调用链。
- `XuanYu.Render.Abstractions/INativeHostSurfaceBridge.cs`  # NativeHost Surface 生命周期桥契约；现在显式包含释放生命周期，不负责具体 Vulkan 创建。
- `XuanYu.Render.Abstractions/INativeHostSurfaceBridgeFactory.cs`  # NativeHost 渲染桥工厂契约；后续供组合根注入具体后端，不负责选择或持有 Vulkan 类型。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridgeFactory.cs`  # Vulkan 后端工厂适配；只创建 `VulkanNativeHostSurfaceBridge`，不改变 Attach/Resize/Detach 行为。

## ARCH-A-Plan 架构边界规划快照 (2026-07-13 20:27:01)
新增 ARCH-A 债务收口规划文档，只记录依赖边界和迁移顺序，不修改运行逻辑。
- `docs/arch-a-plan.md`  # ARCH-A 规划文档：审计 `Editor.UI` 直接依赖 `Render.Vulkan` / `Silk.NET.Vulkan` 的活跃文件与历史旧探针，约束 `v0.2.15.2-rz` 只建立最小生命周期契约并适配 Vulkan 实现；不负责删除旧 UI Vulkan 链路、不新增组合根项目、不改变渲染行为。

## VK-LIFE-1-R2 Fatal 状态发布快照 (2026-07-13)
收口 Present 后台线程 Fatal 状态跨线程发布契约。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs`  # `_failed` 改为 `int` 发布位，`IsFailed` / `FailureReason` 走 Volatile 读取。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Lifecycle.cs`  # `MarkFailed` 用 Interlocked.CompareExchange 抢占首个失败原因，再 Volatile 发布 failed，并保证 Fatal 日志只输出一次。

## VK-LIFE-1-R1 竞态补正快照 (2026-07-13)
补正 VK-LIFE-1 真机验收暴露的 Resize / Present 自愈竞态与状态传播问题。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Resize.cs`  # Resize 接管流程：等自愈锁、锁内复查尺寸、锁外停泵、仅实际重建时推进 generation。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Recover.cs`  # 自愈遇到 Resize 接管时让出；同尺寸自愈不推进 generation；连续失败标记 Session Failed。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.cs` / `.Lifecycle.cs`  # Present 致命错误经回调传播到 RenderSession Failed 状态。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.cs`  # Bridge 在后续 Resize 中识别 Failed Session，不再按正常附加状态继续。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameLogFormatter.cs`  # Session 释放与 ClearFrame 释放使用不同日志，避免重复“RenderPass + Framebuffer 释放成功”。

## VK-LIFE-1 生命周期安全快照 (2026-07-13)
加固 Vulkan 生命周期失败路径，并合并仓库收尾删除已批准的危险脚本。
- 删除 `111.ps1`  # 已批准删除的危险仓库设置脚本；不再作为 SAFE-1-R1 单独开轮。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.cs`  # Present 主循环：检查 Acquire/Wait/Reset/Submit/Present 结果，Stop 使用跨线程可见停止标志。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.Frame.cs`  # Present 单帧提交/Present 辅助：Fence、Submit、QueuePresent 结果检查。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.Lifecycle.cs`  # Present 生命周期：同步对象创建失败回滚、Join 超时阻断释放、日志回调受限兜底。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.cs`  # ClearFrame 创建入口：RenderPass/CommandPool/Framebuffer 创建失败显式失败。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.Commands.cs`  # CommandBuffer 分配与录制结果检查，保留固定三角形绘制路径。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.Lifecycle.cs`  # ClearFrame 资源释放与 Result 诊断辅助。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.Resources.cs`  # RenderPass / CommandPool 创建结果检查。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs`  # Create/Resize 失败逆序回滚；停泵失败时不继续释放底层资源。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Recover.cs`  # Present OutOfDate 自愈与失败计数。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Lifecycle.cs`  # Resize 失败释放与 TryDispose。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.cs`  # Attach 全成功后写字段；Attach/Resize 失败进入明确回滚或失效路径。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.Attach.cs`  # Attach 成功提交字段与失败逆序回滚。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.Lifecycle.cs`  # Bridge Dispose 与日志出口。

## SAFE-1 仓库安全快照 (2026-07-13)
收口 ORG-1 指定的两项仓库安全问题。
- `111.ps1`  # SAFE-1 后曾为禁用占位脚本；VK-LIFE-1 中已按批准删除。
- `.gitignore`  # SAFE-1 新增 `qizheng-mvp-fixed/` 忽略规则，防止误提交根目录下的独立 MVP 项目。
- `qizheng-mvp-fixed/`  # 本地未跟踪独立 MVP 项目；仍留在工作区，但已被根 `.gitignore` 覆盖，不属于玄域引擎源码。

## 文档补充快照 (2026-07-12)
新增《玄域引擎 AI 开发宪法》总治理文档（从用户 Downloads 移入 docs/）。
- `docs/玄域引擎_AI开发宪法.md`  # 826 行总治理文档：统辖执行流程、绝对红线（5+100 / 分层边界 / 敏感信息禁入库 / 聊天记录禁入库 / 禁止掩盖失败）、AI 自主权限、计划与范围控制、Bug 排查、日志、测试验证、异常处理、存档兼容、依赖配置、生成文件、Git 规范、删移重命名、注释与 TODO、警告静态检查、版本规范、文档同步、收口报告、重大事项请示。本文件为后续所有开发/审计/修复/规划/Codex 协作的统一最高执行标准，原 `AI_DEVELOPMENT_RULES.md` / `CODE_CONSTITUTION.md` 内容已由其统辖（保留作历史参考，不删除）。
- 文档任务按"文档同步规范"校验：格式 / 链接 / 内容一致性 / 文件引用 / 事实准确性——纯文档改动，无代码变更，无编译警告新增。

## RZ-VK5-E-Plan 快照 (2026-07-11)
清理 VulkanClearSession 死代码（债务 B）规划轮：经审计确认 VulkanClearSession（Editor.UI 4 个 partial 文件）为 VK3-A 前早期探针，已被 VulkanRenderSession 正式链路取代，全仓无任何 .cs 外部引用或 TryCreate 调用方，属确定无引用死代码。
- `docs/rz-vk5-e-plan.md`（新）：9 项规划（死代码确认 / 文件+调用方+替代链路+删除影响 / 正式链路由 VulkanRenderSession 承担 / 只删无引用死代码 / 不改三角形·Resize·PresentLoop·Pipeline / 不新增 / 全 .cs ≤100 / 双项目 0W0E / 实装步骤 + 风险回滚）+ 红线。
- 实装（待确认）：`git rm` 4 个 VulkanClearSession.*.cs（XuanYu.Editor.UI/Viewport/Vulkan/），低内存构建验证 0W0E，更新 changelog/file-tree，独立 commit + push。
- 红线守住：只删死代码；不碰 VulkanRenderSession 链路 / UI / NativeHost / LOG-UX；不扩大 Editor.UI→Render.Vulkan 引用；双项目 0W0E；全 .cs ≤100。

## RZ-VK5-C-Plan 快照 (2026-07-11)
VK5-C（viewport/scissor 与 Resize 关系）规划轮：经源码取证确认 viewport/scissor 已使用动态状态、Resize 后 CommandBuffer 必然重录且取最新 Swapchain extent、GraphicsPipeline 不随 Resize 重建——三项诉求均满足，VK5-C 无需改代码，改为「验证收口轮」。
- `docs/rz-vk5-c-plan.md`（新）：8 问逐答（带文件/行号源码证据）+ 验证收口方案 + 真机 run-list。
- 本轮零代码改动（无 .cs/.axaml/.csproj 变更）；双项目 0W0E 维持；红线守住（不进 VK5-E、不新增渲染能力、不改三角形/shader/UI/NativeHost、不清 VulkanClearSession）。
- 进度指针：VK5-C 验证收口后推进 VK5-E（清 VulkanClearSession 死代码 = 债务 B）。

## RZ-VK5-D-R3 实装快照 (2026-07-10)
Resize 同尺寸快速跳过 Present 泵停启——消除"自愈成功后又停泵重启"造成的视觉停顿。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` 98→100：Resize 在 Stop 泵前新增短路——目标尺寸==当前 Swapchain.Extent 时打 `Resize 快速跳过` 中文日志并直接 return，不 Stop/Start 泵、不重建 Swapchain/Framebuffer、不重录 CommandBuffer；保留 R2 同尺寸去重与自愈机制。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameLogFormatter.cs` 21→23：新增 `ResizeFastSkipped(uint generation, int w, int h)`（中文：尺寸已由自愈恢复）。
- 验收：Render.Vulkan 0W0E；全 .cs ≤100；功能行为不变（三角形/自愈/关闭释放顺序保持）；红线守住（不进 VK5-C/E、不新增渲染能力、不改三角形/shader/UI/NativeHost、不清 VulkanClearSession）。

## RZ-VK5-D-R2 实装快照 (2026-07-10)
Resize/Present 重复重建去重 + 追踪 gen 修正（用户选方案1：去重+修追踪）。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.cs` 96→98：+`generation` 参数；`Recreate`/`TryRecreateToCurrent` 同尺寸跳过重建（Skipped 日志）。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.cs` 95→96：+`generation` 参数；`RebuildFramebuffers` 同尺寸跳过帧缓冲重建。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` 97→98：Resize/Recover 透传 `_generation`；`RecoverFromOutOfDate` 顶部用真实 gen 打 `Present.OutOfDate` 日志。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.cs` 100→99：移除硬编码 `gen=0` 的 OutOfDate 日志与 Diagnostic using（日志改由 RecoverFromOutOfDate 承担）。
- 验收：Render.Vulkan 0W0E；全 .cs ≤100；行为零变化（去重仅减少冗余重建）；不改红线。

## RZ-VK5-D-R1 实装快照 (2026-07-10)
Resize / Present 慢半拍全链路诊断——不修、先诊断，加 T+elapsedMs 追踪定位慢在哪一段。
- **新增** `XuanYu.Render.Vulkan/Diagnostic/VulkanResizeTracer.cs`（48 行）：共享 Stopwatch 诊断工具，StartTrace()/ElapsedMs()/Stage()/HealStage()/DuplicateWarning()。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` 97→97：+`VulkanResizeTracer.StartTrace()` + Resize/自愈阶段日志；LogProbe 内联消除。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.cs` 100→96：Recreate/TryRecreateToCurrent 加 T+ 阶段日志；属性行合并压缩。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.cs` 100→95：RebuildFramebuffers 加 FB 创建完成追踪；for 循环压缩单行。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.cs` 100→100：OutOfDate(Acquire/QueuePresent) 加来源追踪；注释精简。
- 验收：双项目 0W0E；全 .cs ≤100（新增 48 行 tracer + 4 文件改）；行为零变化。

## RZ-VK5-D 实装快照 (2026-07-10)
VK5-B 封版后职责边界收口：只在 `VulkanClearFrameOwner.cs` 内部整理，不新增渲染能力、不重命名、不改对外行为。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.cs` 99→100：抽出 `RecordDraw(CommandBuffer cb)`（BindPipeline+SetViewport+SetScissor+CmdDraw(3)）从 `RecordOne` 的清屏中拆出；类头注释显式列出 VK5-D 职责边界（帧缓冲管理｜命令录制｜绘制｜管线注入）；`SetPipeline`/`RebuildFramebuffers` 两重录入口仍各自直调 `RecordCommandBuffers`。靠局部变量取地址 + 精简注释 + 去空行守住 ≤100。
- 零改动：`SwapchainOwner`/`PipelineOwner`/`PresentLoop`/`RenderSession` 经审计确认已是干净 SRP，本轮未动。
- 验收：双项目 `dotnet build` **0W0E**；全仓 `.cs` 无超 100 行；功能行为零变化（清屏+三角形、Resize 自愈、关闭释放顺序同 VK5-B 封版）。

## RZ-VK5-B 实装快照 (2026-07-10)
在 VK5-A Pipeline 基础上画出蓝灰背景上的第一个固定三角形（gl_VertexIndex + CmdDraw，不建 VertexBuffer）。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.Vert.cs` 18→33：顶点着色器改用 `gl_VertexIndex` 生成 3 顶点（`glslangValidator -V` 重编译）。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.Frag.cs` 18：片元输出固定琥珀色（重编译）。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.cs` 96→97：新增 `public Silk.NET.Vulkan.Pipeline Pipeline => _pipeline` 供注入。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.cs` 95→99（VK5-D 再 99→100）：+`_pipeline` 字段与 `SetPipeline(Pipeline)`（注入后重录含 Draw）；`RecordOne` 在 RenderPass 内插入 `CmdBindPipeline`+`CmdSetViewport`+`CmdSetScissor`+`CmdDraw(3,1,0,0)`；VK5-D 把绘制抽出为 `RecordDraw`，`RecordOne` 变为 `清屏 → RecordDraw → 结束`；Resize 重建后重录自然带 Draw。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` 97→98：`Create` 把 Pipeline 创建提前到 `loop.Start()` 前并 `clear.SetPipeline(...)`（泵启动前注入，无竞态）；PresentLoop 未改。
- 验收：双项目 `dotnet build` **0W0E**；全改动 `.cs` ≤100（最大 99）；蓝灰背景上出现琥珀色固定三角形；Resize 后三角形仍显示且 Present 自愈保留。

## RZ-VK5-A-R2 实装快照 (2026-07-10)
修复 Resize 后 Present 泵停在 Swapchain OutOfDate 的问题（受控自愈）。不 Draw、不画三角形、不进 VK5-B。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.cs` 99→100：OutOfDate 不再永久 break，改经 `onOutOfDate` 回调请求 RenderSession 统一自愈；无回调时退回 continue；移除 `_outOfDateLogged` 与 `OutOfDatePaused` 永久暂停分支。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` 63→97：新增 `RecoverFromOutOfDate`（OutOfDate 统一自愈入口，返回 true=继续 Present / false=放弃暂停）经 lambda 绑定（规避静态方法组）；`_rebuildLock` 防 Resize 线程与 PresentLoop 线程并发重建；`_generation` 标记重建代次；连续自愈上限 5 次（超上限输出中文错误日志并暂停）；`Resize` 走统一入口（Stop 期间重建）；`Create` 新增 `NativeHostSurfaceHandle?` 形参供探针取 DPI；`VulkanBridgeRenderSessionAttachStep.Run` 透传 handle。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.cs` 88→100：新增 `TryRecreateToCurrent(out Extent2D)`——按 Surface 当前 `CurrentExtent` 重建（Windows `ChooseExtent` 直接返回它，忽略传入尺寸），0/uint.MaxValue 尺寸跳过。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameLogFormatter.cs` 17→21：移除 `OutOfDatePaused`；新增 `OutOfDateProbe`（来源/旧 extent/新 Surface CurrentExtent/DPI/逻辑尺寸/generation）、`OutOfDateRecovered`、`OutOfDateRecoverFailed`。
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgeRenderSessionAttachStep.cs` 15→16：`Run` 透传 `NativeHostSurfaceHandle? handle`。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.cs` 83：Attach 把 `handle` 透传给 RenderSession step。
- 验收：双项目 `dotnet build` **0W0E**；全改动 `.cs` ≤100（最大 100）；Resize 后 Present 自愈恢复（不再永久停在"Swapchain 已过期"）；释放顺序不变（PresentLoop→GraphicsPipeline→ClearFrame→Swapchain→LogicalDevice→Surface→Instance）。
- 红线守住：不 Draw / 不画三角形 / 不建 VertexBuffer·DescriptorSet / 不接 Scene·Camera·Mesh·Material·Gizmo / 不改 UI overlay / 不扩大 Editor.UI→Render.Vulkan 引用（handle 走 Abstractions 契约）/ 不清 VulkanClearSession / 不无限重建（守护上限）/ PresentLoop 线程不 join 自身 Stop/Dispose。

## 视口 UI 收口快照 (2026-07-09)
移除视口内部 overlay，只留纯 Vulkan 视口。
- `XuanYu.Editor.UI/Main/Main.axaml` 20→6：移除 Grid 顶部（透视 / NativeHost Probe）与底部（左键选择 / 中键环绕 / 右键平移 / 工具：选择）两组 pill，内容简化为 `<local:VulkanViewport/>`；`x:DataType` 移除（绑定已删）。`VulkanViewport` 交互逻辑未动。
- 同轮：`RZ-VK5-A-R1` 关闭释放顺序静态验证通过（无代码改动）。

## RZ-VK5-A 实装快照 (2026-07-09)
在 VK4-D Clear+Present 闭环上新增最小 Graphics Pipeline 创建/释放能力（不 Draw、不画三角形）。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.Vert.cs` 25：内嵌顶点 SPIR-V `uint[]`（glslangValidator 本地编译，passthrough，entry main）。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.Frag.cs` 18：内嵌片元 SPIR-V `uint[]`（输出固定色，entry main）。
- `XuanYu.Render.Vulkan/Pipeline/VulkanShaderModuleOwner.cs` 29：`unsafe` 助手，用 `uint[]` 建/销 vert+frag 两个 ShaderModule。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.cs` 96：建空 PipelineLayout + 绑 RenderPass 的 GraphicsPipeline（动态 viewport/scissor、空 vertex input、TriangleList）；建 Pipeline 后立即释放 ShaderModule（短生命周期）；Dispose 释放 Pipeline→Layout。
- `XuanYu.Render.Vulkan/Pipeline/VulkanPipelineLogFormatter.cs` 13：中文日志格式器（经 `Action<string> log`，日志单出口）。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.cs` 93→94：+1 只读 getter `RenderPass => _renderPass`。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` 59→63：+pipeline 字段；`Create` 中 ClearFrame 之后建 Pipeline；`Dispose` 中最先释放 Pipeline。

## RZ-VK5-A-Plan 文档快照 (2026-07-09)
本轮仅新增规划文档，不改任何代码（`.cs` / `.axaml` / `.csproj` 均未动）。
- `docs/rz-vk5-a-plan.md`  # RZ-VK5-A 规划：在 VK4-D Clear+Present 闭环上接入 ShaderModule + PipelineLayout + GraphicsPipeline 最小方案（只规划不实装）。10 项输出：当前 Vulkan 文件职责 / VK5-A 新增(4 文件 `Pipeline/`)+修改(ClearFrameOwner +1 RenderPass getter、RenderSession +pipeline 接线)清单 / ShaderModule·PipelineLayout·GraphicsPipeline 创建释放顺序 / RenderPass·Swapchain·Framebuffer·Pipeline 依赖（RenderPass 构造时建一次、Resize 不重建→Pipeline Resize 稳定）/ ≤100 拆分 / 禁止事项 / 验收 / 风险与回滚；3 决策点（内嵌 SPIR-V byte[]、动态 viewport-scissor、ShaderModule 持有到会话结束）。关键结论：PresentLoop 提交 ClearFrameOwner 录好的 CommandBuffer，VK5-A/B 加 BindPipeline+Draw 零改动 PresentLoop。

## VK4-Closure + VK5-Plan 文档快照 (2026-07-09)
本轮仅新增文档，不改任何代码（`.cs` / `.axaml` / `.csproj` 均未动）。
- `docs/rz-vk4-closure.md`  # VK4 阶段正式收口确认：VK4-A/B/C/D + VIEWPORT-RESIZE-R2 逐项收口表、已验证清单、跨阶段长期硬规则、已知债务、下一阶段指向 VK5。
- `docs/rz-vk5-plan.md`  # VK5 最小几何渲染规划（只规划不实装）：VK5-A Shader+Pipeline / VK5-B 固定三角形（gl_VertexIndex，暂不建 VertexBuffer）/ VK5-C Resize 兼容 / VK5-D 渲染命令边界；12 条红线、资源创建/释放顺序、文件结构、SVG 规划图。

## VIEWPORT-RESIZE-R2 快照 (2026-07-09)
VIEWPORT-RESIZE-R2（Editor.UI 侧）已收口：修复 R1 的 DPI 错配——`SyncFinalSize` 把 Avalonia 逻辑尺寸 ×DPI 换算成物理像素再喂 `Win32ViewportHost.Resize`，`_bridge.Resize` 仍收逻辑尺寸；探针补「目标物理」字段。全部 ≤100 行，双项目 0W0E。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs` 98：`partial`；`OnSizeChanged`/`Coalescer` 路径原样保留（拖动窗口仍走它）。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.LayoutSync.cs` 38→49：`SyncFinalSize` 先 `physicalW=max(1,round(logicalW*GetDpiScale()))`、`physicalH=max(1,round(logicalH*GetDpiScale()))`；`Win32ViewportHost.Resize(_hwnd, physicalW, physicalH)`（物理像素）；`_bridge.Resize(logicalW, logicalH)`（逻辑）；`GetClientSize` 取实际物理供探针。
- `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs` 67：`Resize` 裸 `SetWindowPos` 不乘 DPI（收物理像素）；`GetClientSize(hwnd)` 取子窗口物理像素。
- `XuanYu.Editor.UI/ViewportNativeHostRoute.cs` 18：`ReportProbe(vm, open, logicalW, logicalH, dpi, targetW, targetH, clientW, clientH)`。
- `XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs` 38：`LogNativeHostProbe(... dpi, targetW, targetH, clientW, clientH)` 输出 目标物理 + 子窗口实际。
VK4-D-R3（Render.Vulkan 侧）已收口：修改 6 个 Render.Vulkan 文件，全部 ≤100 行，双项目 0W0E。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.cs` 99：OutOfDate 优雅降级（`ErrorOutOfDateKhr` 仅记一次 `OutOfDatePaused()` 后 break，不刷屏）；`Start()` 重置 `_outOfDateLogged`；`Stop()` 局部捕获线程防 NRE。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` 59：Resize 日志顺序收口（Rebuilt 在 Start 前，用 `_swapchainOwner.Extent` 物理像素）。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainCapabilities.cs` 81：能力日志打印请求逻辑尺寸 + Surface CurrentExtent（物理像素）+ 选择 extent（物理像素）。
VK4-D-R3（Render.Vulkan 侧）已收口：修改 6 个 Render.Vulkan 文件，全部 ≤100 行，双项目 0W0E。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.cs` 92→99：OutOfDate 优雅降级（`ErrorOutOfDateKhr` 仅记一次 `OutOfDatePaused()` 后 break，不刷屏）；`Start()` 重置 `_outOfDateLogged`；`Stop()` 局部捕获线程防 NRE。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` 59：Resize 日志顺序收口（Rebuilt 在 Start 前，用 `_swapchainOwner.Extent` 物理像素）。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainCapabilities.cs` 80→81：能力日志打印请求逻辑尺寸 + Surface CurrentExtent（物理像素）+ 选择 extent（物理像素）。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainLogFormatter.cs` 13→15：`Created`/`Recreated` 收 `Extent2D` 打印物理像素。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameLogFormatter.cs` 14→17：`Rebuilt(Extent2D, uint)` 打印物理像素；新增 `OutOfDatePaused()`。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.cs` 88：`Created`/`Recreated` 改传实际 `_extent`。

最近更新：VK3 已收口（验收通过，2026-07-08）：新增 docs/rz-vk3-closure.md（收口确认）+ docs/rz-vk4-plan.md（VK4 规划，只规划不实装）；VK4 目标为最小渲染闭环（PhysicalDevice→LogicalDevice→Queue→Swapchain→ClearFrame→RenderSession），红线：Resize 不重建 Surface、不搬探针、UI 不持 Vulkan、每步 5+100。VK3 既有链路（VK3-A..VK3-C2-R1）不变。VK4-A 已实装（2026-07-08）：在 Instance+Surface 之后新增 PhysicalDevice 选择链路（XuanYu.Render.Vulkan/Device/ 下 3 个新文件，桥 Attach 后调用，仅枚举/选择/中文日志，未创建设备/队列/Swapchain）。VK4-A-R1 已收口（2026-07-08）：VK4-A 后 VulkanNativeHostSurfaceBridge.cs 由 93→110 行越过 100 行红线，已将内联选择逻辑迁出至 XuanYu.Render.Vulkan/Bridge/VulkanBridgePhysicalDeviceAttachStep.cs（23 行），Bridge 压回 96 行，仅保留生命周期编排；行为不变。VK4-B 已实装（2026-07-08）：基于 VK4-A 选择结果新增 LogicalDevice + Graphics/Present 队列（`Device/VulkanPhysicalDeviceSelection.cs` 抽为独立文件并补 PhysicalDevice 句柄、`Device/VulkanDeviceOwner.cs` 创建 VkDevice+队列、`Bridge/VulkanBridgeDeviceAttachStep.cs` 在 Instance+Surface 就绪后链式驱动创建设备）；未建 Swapchain/ImageView/RenderPass/CommandBuffer、未清屏/Present、UI 未接触 Silk.NET.Vulkan 类型；Bridge Detach 逆序释放 Device→Surface→Instance。VK4-C 已实装（2026-07-08）：在 VK4-B 的 LogicalDevice+Queue 之后新增 Swapchain + Swapchain Images + ImageViews 链路（`XuanYu.Render.Vulkan/Swapchain/` 下 4 文件：VulkanSwapchainCapabilities 80 / VulkanSwapchainBuilder 74 / VulkanSwapchainOwner 86 / VulkanSwapchainLogFormatter 13；`Bridge/VulkanBridgeSwapchainAttachStep.cs` 32 在设备 step 后链式驱动；均 ≤100 行）；Bridge 改写 98 行（接近 100 红线不再膨胀，仅编排生命周期，Swapchain 逻辑全在独立 owner/step）；Resize 只重建 Swapchain+ImageViews（不重建 Surface/Instance/Device/Queue）；Dispose 顺序 ImageViews→Swapchain→LogicalDevice→Surface→Instance；红线守住：未建 RenderPass/Framebuffer/CommandPool/CommandBuffer、未 Clear/Present（仍黑屏为预期）、UI 零改动不接触 Silk.NET.Vulkan、未复制 VulkanClearSession 旧探针；两项目 0W0E。VK4-C 已补运行前置修正（2026-07-08）：`VulkanDeviceOwner` 创建设备时启用 `VK_KHR_swapchain` 设备扩展（扩展名由 `VulkanSwapchainOwner.DeviceExtensionName` 传入，DeviceOwner 96→99 行）、`VulkanSwapchainOwner.Recreate` 加 0 尺寸跳过（77→86 行）、暴露 `Format`/`Extent`/`ImageViews` 只读供 VK4-D；均 ≤100，仍不出画面。状态：VK4-C 代码完成，待 VK4-C-R1 真机运行验证，未完全收口；VK4-D 已实装（D1+D2+D3+VK4-D-R1 修复），VK4-D-R2（2026-07-09）修复 Present 泵后台线程日志回调线程派发导致的闪退——VulkanNativeHost 新增 ReportVulkanMessage / ReportVulkanMessageOnUiThread 经 Dispatcher.UIThread 切回 UI 线程访问 DataContext / UiVm；VulkanPresentLoop.Log 加 try/catch 防御；双项目 0W0E，VulkanNativeHost 95 / VulkanPresentLoop 96 行；VK4-D 仍待真机验收。

统计口径：当前工作区真实文件快照，排除 `.git/`、`bin/`、`obj/` 生成目录。

当前文件总数：124

```

## RZ-VK3-A-R1 / RZ-VK3-A Surface 契约层快照 (2026-07-08)

新增独立契约工程 `XuanYu.Render.Abstractions`（net10.0，零 Silk.NET / Avalonia / Editor.Win / Render.Vulkan 引用），承载 UI 与 Vulkan 之间的纯窗口交接通道。

### XuanYu.Render.Abstractions/
- `XuanYu.Render.Abstractions.csproj`  # 纯契约工程，无包引用。
- `NativeHostSurfaceHandle.cs`  # HWND / Hinstance / 尺寸 / DPI 交接句柄（VK3-A）。
- `INativeHostSurfaceBridge.cs`  # Attach / Resize / Detach 交接契约接口（VK3-A）。
- `NativeHostLifecycleState.cs`  # 生命周期状态枚举（VK3-A-R1 从 Render.Vulkan 迁入）。
- `NativeHostHandleSnapshot.cs`  # 生命周期快照记录（VK3-A-R1 从 Render.Vulkan 迁入）。
- `NativeHostLifecycleProbe.cs`  # 生命周期探针（VK3-A-R1 从 Render.Vulkan 迁入）。
- `NativeHostLifecycleLogFormatter.cs`  # 中文生命周期日志格式器（VK3-A-R1 从 Render.Vulkan 迁入）。

### XuanYu.Render.Vulkan/（已移除 4 个生命周期类型）
- 删除 `NativeHostLifecycleState.cs` / `NativeHostHandleSnapshot.cs` / `NativeHostLifecycleProbe.cs` / `NativeHostLifecycleLogFormatter.cs`（迁入 Abstractions）。
- 保留 `VulkanApiProbe.cs` / `VulkanProbeResult.cs` / `VulkanProbeLogFormatter.cs` / `VulkanDeviceInfo.cs`（Vulkan 环境探针，仍属 Render.Vulkan）。
- 新增 `VulkanInstanceOwner.cs` / `VulkanInstanceLogFormatter.cs` / `VulkanInstanceResult.cs`（VK3-B1：Vulkan Instance 持有者；VK3-B1-R1 将 VulkanInstanceOwner 由 98 行拆至 66 行，抽出 `VulkanInstanceExtensions.cs` 最小扩展名集合与 `VulkanInstanceCreateInfoBuilder.cs` InstanceCreateInfo 构造；仍仅创建/释放 Instance 并启用 VK_KHR_surface + VK_KHR_win32_surface；Dispose 幂等且释放后清空句柄；中文生命周期日志；禁止 Surface / PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame）。
- 新增 `VulkanInstanceExtensions.cs` / `VulkanInstanceCreateInfoBuilder.cs`（VK3-B1-R1：前者存 Instance 启用的最小扩展名集合，后者在 fixed 作用域内构造 InstanceCreateInfo 并交给回调，均不直接调用 Vulkan）。
- 新增 `VulkanSurfaceOwner.cs` / `VulkanSurfaceResult.cs` / `VulkanSurfaceLogFormatter.cs`（VK3-B2：Vulkan Surface 持有者，仅创建/释放 VkSurfaceKHR-Win32，生命周期绑定 NativeHost Attach/Detach，不绑定 Resize；创建经 KhrWin32Surface.CreateWin32Surface，销毁经 KhrSurface.DestroySurface；取 NativeHostSurfaceHandle 的 Hwnd/Hinstance；Dispose 幂等且释放后清空句柄；中文生命周期日志；禁止 PhysicalDevice/LogicalDevice/Queue/Swapchain/RenderFrame）。
- 新增 `VulkanNativeHostSurfaceBridge.cs` / `VulkanBridgeLogFormatter.cs`（VK3-C1：前者实现 INativeHostSurfaceBridge，Attach 经 VulkanInstanceOwner.Create + VulkanSurfaceOwner.Create 串起 Instance+Surface，Detach/Dispose 先释放 Surface 再释放 Instance，Resize 只记日志不重建 Surface；后者为纯中文生命周期日志格式器；均不接 UI 组合根、不碰 PhysicalDevice/LogicalDevice/Queue/Swapchain/RenderFrame）。
- 新增 `Device/VulkanPhysicalDeviceInfo.cs` / `Device/VulkanQueueFamilySelection.cs` / `Device/VulkanPhysicalDeviceSelector.cs`（VK4-A：PhysicalDevice 选择链路，落在 `XuanYu.Render.Vulkan/Device/` 子目录；仅枚举设备、检查 Graphics/Present 队列族与 Surface 呈现支持、优先独显、返回纯数据结果（`VulkanPhysicalDeviceInfo` / `VulkanQueueFamilySelection` / `VulkanPhysicalDeviceSelection`）、输出中文日志；`VulkanNativeHostSurfaceBridge.Attach` 在 Instance+Surface 就绪后调用选择 step；红线：未创建 LogicalDevice/Queue/Swapchain/ImageView、未清屏/Present、未让 UI 引用 Vulkan 类型、未复制旧探针 `VulkanClearSession`、新增文件均 ≤100 行）。
- VK4-B 抽出 `Device/VulkanPhysicalDeviceSelection.cs`（12 行，原内联在选择器末尾的记录独立成文件，并补 `PhysicalDevice Handle` 字段供 VK4-B 复用、不泄漏 UI）；新增 `Device/VulkanDeviceOwner.cs`（99 行，基于 `VulkanPhysicalDeviceSelection` 创建 VkDevice 与 Graphics/Present 队列、Dispose 幂等释放；输出中文日志；VK4-C 修正启用 VK_KHR_swapchain 设备扩展）；`VulkanPhysicalDeviceSelector.cs` 由 99→93 行（移除内联记录、捕获 `bestDevice` 随结果返回句柄），仅负责枚举与选择。
- 新增 `Bridge/VulkanBridgePhysicalDeviceAttachStep.cs`（VK4-A-R1：在 Instance+Surface 就绪后调用 `VulkanPhysicalDeviceSelector.Select`、把选择结果（现返回 `VulkanPhysicalDeviceSelection?`）与中文日志写入面板的薄委托层；选择异常仅记日志、不影响已附加的 Instance+Surface；`VulkanNativeHostSurfaceBridge` 不再内联选择逻辑，仅保留生命周期编排，已压回 96 行；目录 `Bridge/` 现 2 文件，未越过 5-7 文件上限）。
- VK4-B 新增 `Bridge/VulkanBridgeDeviceAttachStep.cs`（29 行，在 VK4-A 选择成功后基于 `VulkanPhysicalDeviceSelection` 调用 `VulkanDeviceOwner.Create` 创建 LogicalDevice；选择失败（`sel` 为 null 或 `!Success`）则跳过、异常仅记日志，不影响已附加的 Instance+Surface+已选中设备）。`VulkanNativeHostSurfaceBridge.Attach` 现链式执行「选择 step → 设备 step」，Detach 逆序释放 Device→Surface→Instance。
- VK4-C 新增 `Swapchain/` 子目录（4 文件）：`VulkanSwapchainCapabilities.cs`（80 行，查 Surface caps/formats/present modes/extent 并输出纯数据 `SwapchainCaps`/`VulkanSwapchainCapabilitiesResult`）、`VulkanSwapchainBuilder.cs`（75 行，串 Query→CreateSwapchain→GetSwapchainImages→CreateImageViews）、`VulkanSwapchainOwner.cs`（86 行，经 `vk.TryGetDeviceExtension(instance, deviceOwner.LogicalDevice, out KhrSwapchain? khr)` 创建 Swapchain+Images+ImageViews，`Recreate(width,height)` 只重建（含 0 尺寸跳过）、`Dispose` 先 ImageView 后 Swapchain，暴露 `Format`/`Extent`/`ImageViews` 只读供 VK4-D）、`VulkanSwapchainLogFormatter.cs`（13 行，中文生命周期日志）。均不建 RenderPass/Framebuffer/CommandPool/CommandBuffer、不 Clear/Present。
- VK4-C 新增 `Bridge/VulkanBridgeSwapchainAttachStep.cs`（32 行，在设备 step 后链式驱动 `VulkanSwapchainOwner.Create`；前置 null/Success 检查跳过、异常仅记日志）。`VulkanNativeHostSurfaceBridge.cs`（VK4-C 改写 98 行，接近 100 红线）`Attach` 串「选择→设备→Swapchain」、`Resize` 转发 `_swapchainOwner?.Recreate(width,height)`、`Detach` 首行 `_swapchainOwner?.Dispose()`、`Emit` 迁入 `VulkanBridgeLogFormatter.Emit`；`VulkanBridgeLogFormatter.cs`（VK4-C 改写 35 行，新增 `Emit(Action<string>?, string)` 统一日志出口）。
- VK4-D 实装（D1+D2+D3 同轮）+ VK4-D-R1 审计修复：新增 `Render/` 子目录——`VulkanClearFrameLogFormatter.cs`（14 行，中文清屏日志 + 首帧 Present 成功日志）、`VulkanClearFrameOwner.cs`（93 行：RenderPass+CommandPool+CommandBuffer[] 每 Swapchain 图像一张静态 clear 录制+Framebuffer[]，clear 颜色 0.25/0.45/0.70 明显蓝灰，Resize 只重建 Framebuffer+重录）、`VulkanPresentLoop.cs`（92 行：独立后台线程跑 Acquire→Submit→Present 单帧闭环、单 in-flight 帧+单栅栏、`_submitted` 守卫避免首帧 WaitForFences 空等、把 `SuboptimalKhr` 当成功码处理、Acquire/Present 失败记录中文错误、首帧 Present 成功只打印一次、Detach/Resize 先 Stop 再重建；`using Semaphore = Silk.NET.Vulkan.Semaphore` 消歧义）；`Session/VulkanRenderSession.cs`（59 行薄组合根，只装配 ClearFrame+PresentLoop，Resize 统一负责 Stop→Swapchain 重建→Framebuffer 重建→Start）；`Bridge/VulkanBridgeRenderSessionAttachStep.cs`（15 行，Bridge 仅委托）。`VulkanSwapchainOwner.cs` 补 `Swapchain`/`Khr` 只读 getter（88 行）。`VulkanNativeHostSurfaceBridge.cs` 压回 83 行：Attach 经独立 step 创建 `_renderSession`、Resize 仅转发 `_renderSession?.Resize`（不再二次重建 Swapchain）、Detach 首行 `_renderSession?.Dispose()`（顺序 ClearFrame→Swapchain→Device→Surface→Instance）。全 .cs ≤100；双项目 0W0E。

### XuanYu.Editor.UI/（依赖收口）
- `NativeHostSurfaceContract.cs`  # 仅 `using XuanYu.Render.Abstractions`（VK3-A）。
- `NativeHostResizeCoalescer.cs` / `ViewportNativeHostRoute.cs` / `Vm/UiVm.NativeHostLifecycle.cs` / `Viewport/Vulkan/VulkanNativeHost.cs`  # 生命周期链路改用 `using XuanYu.Render.Abstractions`（VK3-A-R1）；VK4-D-R2 新增 ReportVulkanMessage / ReportVulkanMessageOnUiThread，把 Vulkan 日志回调经 Dispatcher.UIThread 切回 UI 线程（后台 Present 泵线程安全）。
- `VulkanProbeRoute.cs` / `Vm/UiVm.VulkanProbe.cs`  # 仍 `using XuanYu.Render.Vulkan`（Vulkan 探针类型），故 Editor.UI.csproj 保留对 Render.Vulkan 的工程级引用。
- `Viewport/Vulkan/VulkanSurfaceBridgeProvider.cs`  # VK3-C2 组合根：装配 INativeHostSurfaceBridge 具体实现，UI 宿主只认 Abstractions 契约（故 Editor.UI.csproj 仍引用 Render.Vulkan）。


XuanYuEngine/
│
├── NuGet.Config  # NuGet 源配置。
├── changelog.md  # 项目变更记录，按时间倒序记录阶段性修改。
├── codex_log_xuanyu_handoff_20260705-2102.zip  # Codex 交接日志压缩包。
├── file-tree.md  # 当前文件树与文件职责说明。
├── run.bat  # Windows 启动脚本。
│
├── docs/
│   ├── 玄域引擎_AI开发宪法.md  # 玄域引擎 AI 开发宪法（总治理文档，统辖执行流程 / 绝对红线 / 自主权限 / 计划范围 / Bug 排查 / 日志 / 测试 / 异常 / 存档 / 依赖 / 生成文件 / Git / 删移重命名 / 注释 / 警告 / 版本 / 文档同步 / 收口报告 / 重大事项请示）。
│   ├── 版本号规范与历史映射.md  # DOC-VERSION-1 配套说明：版本号格式 v0.M.m.r-类型、类型标签暂用三类（非封闭白名单）、M1/M2 里程碑边界为项目整理确认、146 行历史编号映射索引。changelog.md 仅保留纯日志条目。
│   ├── project-baseline-audit-org-1.md  # ORG-1 项目真实基线审计（2026-07-12，已退回，见 R1）：16 节纯审计文档。
│   ├── project-baseline-audit-org-1-r1.md  # ORG-1-R1 修正版（2026-07-12）：修正 ORG-1 的 11 项误判——分支范围限定 / 5+100 质量条件不通过 / 空 catch P1 / Editor.UI 活跃组合根违反 / Vk 所有权（Bridge 持有）/ Vulkan 失败路径回滚 P1 / 能力表数字 A=10 B=1 D=1+未规划类 / 基线双标注 / 可复跑证据附录 / 111.ps1 非强推 / ORG-2 拆为 5 轮。仅审计不改代码。
│   ├── AI_DEVELOPMENT_RULES.md  # AI 协作开发规则（已被 AI 开发宪法统辖，保留作历史参考）。
│   ├── CODE_CONSTITUTION.md  # 代码宪法与结构约束。
│   ├── ENGINE_ARCHITECTURE.md  # 引擎架构说明。
│   ├── LEGACY_FLUIDWARFARE_OLD_AUDIT.md  # 旧 FluidWarfare 项目审计记录。
│   ├── MILESTONE1_PUBLIC_VALIDATION.md  # 里程碑 1 公开验证说明。
│   ├── NAMING_RULES.md  # 命名规则。
│   ├── PHASE1_SCOPE.md  # Phase 1 范围定义。
│   ├── PROJECT_CHARTER.md  # 项目章程。
│   ├── audit-EditorShellV2-9.1A-1.md  # EditorShellV2 9.1A 第一轮审计。
│   ├── audit-EditorShellV2-freeze-9.1A-Freeze.md  # EditorShellV2 冻结问题审计。
│   ├── audit-EditorShellV2-input-9.1A-2.md  # EditorShellV2 输入链路审计。
│   ├── audit-EditorShellV2-input-9.1A-2R.md  # EditorShellV2 输入链路复审。
│   ├── audit-EditorShellV2-picking-gizmo-9.1A-3.md  # EditorShellV2 Picking / Gizmo 审计。
│   ├── audit-EditorShellV2-picking-gizmo-9.1A-3R.md  # EditorShellV2 Picking / Gizmo 复审。
│   ├── audit-EditorShellV2-plan-9.1A-0.md  # EditorShellV2 9.1A 审计计划。
│   ├── audit-NativeViewportMouseCapture-lifecycle-9.0X.md  # Native Viewport 鼠标捕获生命周期审计。
│   ├── audit-gizmo-chain-9.0Y-1.md  # Gizmo 链路审计 9.0Y-1。
│   ├── audit-gizmo-chain-9.0Y-2.md  # Gizmo 链路审计 9.0Y-2。
│   ├── audit-gizmo-chain-9.0Y-3.md  # Gizmo 链路审计 9.0Y-3。
│   ├── audit-gizmo-stash-9.0Y-0.md  # Gizmo 暂存状态审计。
│   ├── audit-input-lifecycle-9.0X-1.md  # 输入生命周期审计 9.0X-1。
│   ├── audit-input-lifecycle-9.0X-2.md  # 输入生命周期审计 9.0X-2。
│   ├── audit-input-lifecycle-9.0X-3.md  # 输入生命周期审计 9.0X-3。
│   ├── audit-inspector-transform-9.0C-0.md  # Inspector / Transform 同步审计。
│   ├── diagnostic-safety.md  # 诊断日志、底部日志准入与 UI 调度安全规范。
│   ├── editor-top-area-target-9.1B.md  # 顶部区域目标说明。
│   ├── editor-top-svg-icons-9.1C-R.md  # 顶部 SVG 图标细修说明。
│   ├── editor-top-svg-icons-9.1C.md  # 顶部 SVG 图标替换说明。
│   ├── editor-ui-terms-9.1B.md  # 编辑器 UI 术语说明。
│   ├── gizmo_drag_audit_2026-06-25.md  # Gizmo 拖动审计报告。
│   ├── gizmo_drag_audit_probe.log  # Gizmo 拖动审计探针日志。
│   ├── naming-XuanYu-Engine.md  # XuanYu Engine 命名迁移说明。
│   └── plan-9.0D-move-gizmo-final.md  # Move Gizmo 最终验收计划。
│
├── codex_log/
│   ├── README.md  # Codex 日志目录说明。
│   ├── raw_sessions/
│   │   ├── rollout-2026-06-24T21-20-22-019ef9c9-df07-7dc1-b8e3-ff99f4382fdc.jsonl  # Codex 原始会话日志。
│   │   ├── rollout-2026-06-25T11-37-38-019efcda-b9b4-7201-876a-09bbd3d169f3.jsonl  # Codex 原始会话日志。
│   │   ├── rollout-2026-06-25T22-42-25-019eff3b-4db5-74a1-aadb-0f30d30ddfac.jsonl  # Codex 原始会话日志。
│   │   ├── rollout-2026-06-27T19-35-50-019f08dd-3fff-7b92-a37b-ac0ace4f39a1.jsonl  # Codex 原始会话日志。
│   │   ├── rollout-2026-07-03T11-56-19-019f261e-b69a-7790-9a25-bc9afd0ffbca.jsonl  # Codex 原始会话日志。
│   │   ├── rollout-2026-07-05T11-39-54-019f305c-6407-7f50-95be-51b18d0219a5.jsonl  # Codex 原始会话日志。
│   │   └── rollout-2026-07-05T20-05-18-019f322b-2ff6-7f70-8f17-388a2e5a7e1d.jsonl  # Codex 原始会话日志。
│   └── rollout_summaries/
│       ├── 2026-06-24T13-20-17-hWSo-xuan_yu_engine_log_copy_and_gizmo_hit_test_fix.md  # 日志复制与 Gizmo 命中修复摘要。
│       ├── 2026-06-25T03-37-33-v23F-move_gizmo_cadence_debug_probe_ui_io_fix.md  # Move Gizmo 高频探针与 UI IO 修复摘要。
│       ├── 2026-06-25T14-42-17-Dgjq-gizmo_drag_audit_and_middle_button_capture_fix.md  # Gizmo 拖动审计与中键捕获修复摘要。
│       ├── 2026-06-27T11-35-45-ffSF-editor_move_flow_blue_marker_debug.md  # 编辑器移动流程与蓝色标记调试摘要。
│       └── 2026-07-03T03-56-14-ks8q-xuanyueditor_ui_short_names_layout_skeleton.md  # XuanYu.Editor.UI 短命名布局骨架摘要。
│
├── XuanYu.Core/
│   ├── XuanYu.Core.csproj  # 核心类库项目文件。
│   ├── Diagnostics/
│   │   └── CoreSelfTest.cs  # Core 自检入口。
│   ├── Identity/
│   │   └── EntityId.cs  # 实体 ID 值对象。
│   ├── Logging/
│   │   ├── EngineLogEntry.cs  # 引擎日志条目。
│   │   └── EngineLogLevel.cs  # 引擎日志等级。
│   ├── Math/
│   │   ├── Vector3d.cs  # 三维向量值对象。
│   │   └── YawRotation.cs  # Yaw 旋转值对象。
│   ├── Results/
│   │   ├── EngineError.cs  # 引擎错误值对象。
│   │   └── EngineResult.cs  # 引擎结果类型。
│   └── Time/
│       ├── SimulationTime.cs  # 模拟时间值对象。
│       └── TimeStep.cs  # 时间步长值对象。
│
├── XuanYu.Editor.Win/
│   ├── XuanYu.Editor.Win.csproj  # WinForms 编辑器项目文件。
│   ├── MainForm.cs  # WinForms 主窗口。
│   └── Program.cs  # WinForms 启动入口。
│
└── XuanYu.Editor.UI/
    ├── XuanYu.Editor.UI.csproj  # Avalonia 编辑器 UI 外壳项目。
    ├── RelayCommand.cs  # ICommand 简易实现。
    ├── Ui.axaml  # 全局 UI 样式资源。
    ├── app.manifest  # Windows 应用清单。
    ├── Bootstrap/
    │   ├── App.axaml  # Avalonia 应用资源入口。
    │   ├── App.axaml.cs  # 应用启动与主窗口挂载。
    │   └── Program.cs  # Avalonia 桌面启动入口（含 AttachConsole(-1) 继承父控制台使 Console.WriteLine 可见）。
    ├── Foot/
    │   ├── Foot.axaml  # 底部全局日志栏：摘要、过滤、搜索框、日志列表与右侧详情入口。
    │   ├── LogDetailPanel.axaml  # 日志详情面板：点击选中日志后显示详情并提供复制入口。
    │   ├── Foot.axaml.cs  # 底部栏代码后置。64 行：LOG-UX-2 只做接线——创建 LogListAutoScrollController(LogList)、SelectionChanged 详情选中、Ctrl+A/C 多选复制。自动滚动状态机已拆入 LogListAutoScrollController.cs。
    │   ├── LogListAutoScrollController.cs  # 74 行：LOG-UX-2 独立自动滚动控制器。单次解析 ScrollViewer（TemplateApplied 后 FindDescendantOfType 一次，不每条日志遍历）；节流（_pendingScroll 连续多条只滚一次）；防重入（_isProgrammaticScroll 区分程序/用户滚动）；_followTail 用户上翻暂停、回底恢复（容差 12px）。不碰 Vulkan/UiVm.Logging。
    │   └── LogDetailPanel.axaml.cs  # 日志详情复制按钮与剪贴板桥接。
    ├── Icons/
    │   └── EditorIcons.axaml  # SVG / PathData 图标集中资源。
    ├── Left/
    │   ├── Left.axaml  # 左侧项目 / 层级页签。
    │   └── Left.axaml.cs  # 左侧面板代码后置。
    ├── Main/
    │   ├── Main.axaml  # 中央深色视口占位。
    │   └── Main.axaml.cs  # 中央视口代码后置。
    ├── Right/
    │   ├── Right.axaml  # 右侧检查器 / 调试 / 偏好 / 模式页签，调试页只显示状态快照。
    │   └── Right.axaml.cs  # 右侧面板代码后置。
    ├── Root/
    │   ├── UiRoot.axaml  # 主布局：顶部、左侧、视口、右侧、底部。
    │   └── UiRoot.axaml.cs  # 主布局代码后置。
    ├── Top/
    │   ├── Top.axaml  # 顶部两行工具区：主命令分组、编辑工具分组、状态与当前工具。
    │   └── Top.axaml.cs  # 顶部工具栏代码后置。
    ├── Vm/
    │   ├── DebugText.cs  # 右侧调试页状态快照示例数据。
    │   ├── EditorLogCategory.cs  # 编辑器日志分类枚举。
    │   ├── EditorLogLevel.cs  # 编辑器日志等级枚举。
    │   ├── EditorLogSource.cs  # 编辑器日志来源枚举。
    │   ├── LogEntry.cs  # 编辑器日志条目模型。
    │   ├── SampleLogEntries.cs  # 底部日志栏示例数据。
    │   ├── UiText.cs  # 静态中文 UI 文案与检查器示例数据。
    │   ├── UiVm.Logging.cs  # UI ViewModel 的日志总线、Buffer、过滤绑定与低频日志入口。
    │   ├── UiVm.cs  # UI ViewModel：工具选择、状态、日志展开、检查器/日志/调试绑定。
    │   └── Logging/
    │       ├── EditorLogBuffer.cs  # 编辑器内存日志缓冲区，最多保留最近 500 条并合并连续重复。
    │       ├── EditorLogBus.cs  # 编辑器低频日志入口：Info / Warning / Error。
    │       ├── EditorLogClipboardText.cs  # 单条日志详情的结构化中文复制文本格式化。
    │       ├── EditorLogFilter.cs  # 底部日志过滤枚举与中文按钮映射。
    │       ├── EditorLogFilterQuery.cs  # 日志过滤匹配规则。
    │       ├── EditorLogRepeatKey.cs  # 重复日志折叠键。
    │       └── EditorLogSummary.cs  # 日志摘要计算：错误数、警告数、最近事件。
    └── Win/
        ├── UiWin.axaml  # Avalonia 主窗口壳。
        └── UiWin.axaml.cs  # 主窗口代码后置。
```
# 项目文件树 — XuanYu Engine

最近更新：RZ-Fix3-A 新增中央 Vulkan Host 前置验证，接入 NativeControlHost、Win32 Surface、Device 与 Swapchain 生命周期；本轮新增 `XuanYu.Editor.UI/Viewport/Vulkan/`。

统计口径：当前工作区真实文件快照，排除 `.git/`、`bin/`、`obj/`、`artifacts/` 生成目录。

当前文件总数：99

新增 Vulkan 视口文件：
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.Device.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.Swapchain.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.Dispose.cs`

## RZ-VK2 / RZ-VK1 current snapshot (2026-07-07)
Current file count: 117
New Vulkan / NativeHost files:
- XuanYu.Render.Vulkan/XuanYu.Render.Vulkan.csproj
- XuanYu.Render.Vulkan/VulkanApiProbe.cs
- XuanYu.Render.Vulkan/VulkanDeviceInfo.cs
- XuanYu.Render.Vulkan/VulkanProbeLogFormatter.cs
- XuanYu.Render.Vulkan/VulkanProbeResult.cs
- XuanYu.Render.Vulkan/NativeHostHandleSnapshot.cs
- XuanYu.Render.Vulkan/NativeHostLifecycleState.cs
- XuanYu.Render.Vulkan/NativeHostLifecycleProbe.cs
- XuanYu.Render.Vulkan/NativeHostLifecycleLogFormatter.cs
- XuanYu.Editor.UI/VulkanProbeRoute.cs
- XuanYu.Editor.UI/ViewportNativeHostRoute.cs
- XuanYu.Editor.UI/Vm/UiVm.VulkanProbe.cs
- XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs
- docs/audit-RZ-VK1-vulkan-probe.md
- docs/audit-RZ-VK2-native-host-lifecycle.md

## 开发规范理解文档补充 (2026-07-07)
新增开发规范两份（由 AI 接手理解验收提炼，经人工校正 5+100 / 依赖隔离 / 日志边界 / VK 阶段边界表述）：
- `docs/dev-rules.md`  # 开发硬规则执行手册：接手红线清单 + 5+100 + 依赖方向硬隔离 + 高频链路纪律 + 日志边界 + VK 阶段边界 + 中文化 + 范围结构 + 命名品牌 + 构建测试审计门禁。
- `docs/dev-rules-understanding.md`  # 开发规范「为什么这样规定」：事故来源、设计动机、历史坑、ShellV2 冻结 / Gizmo 卡顿 / Vulkan 生命周期教训、常见误读速查。

## RZ-VK2-R1 NativeHost Resize 日志合并 (2026-07-07)
结构性新增（已同步）：
- `XuanYu.Editor.UI/NativeHostResizeSnapshot.cs`  # 尺寸变化快照，只保存尺寸数据。
- `XuanYu.Editor.UI/NativeHostResizeCoalescer.cs`  # 250ms debounce，合并连续尺寸变化，稳定后生成一条低频合并日志；Detach/Dispose 安全停止 pending。
- `docs/audit-RZ-VK2-R1-nativehost-resize-coalesce.md`  # RZ-VK2-R1 审计与验收文档。
- `docs/audit-RZ-VK2-R2-nativehost-resize-coalesce-verify.md`  # RZ-VK2-R2 验证/收口轮：确认合并边界干净、未牵连 Vulkan 生命周期。
- `docs/rz-vk3-surface-lifecycle-plan.md`  # RZ-VK3-Plan：正式 VK3 Surface 生命周期规划（只规划不写实装）。
修改（职责收口，未改布局/输入）：
- `XuanYu.Editor.UI/ViewportNativeHostRoute.cs`  # 增加 ReportMerged 薄入口。
- `XuanYu.Render.Vulkan/NativeHostLifecycleLogFormatter.cs`  # 增加 MergedMessage 中文合并日志格式。
- `XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs`  # 增加 LogNativeHostResizedMerged。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`  # OnSizeChanged 走 Coalescer；Detach/Dispose 调 Cancel。
- `XuanYu.Editor.UI/Main/Main.axaml` 与 `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml`  # 视口文案 Vulkan Clear Probe 改为 NativeHost Probe / Vulkan Probe。

## RZ-New-0 接手验收审计 (2026-07-07)
- `docs/audit-RZ-New-0-onboarding.md`  # RZ-New-0 接手验收审计：10 项清单、真实状态（含 Editor.UI 直接引用 Vulkan 过渡债务与探针已超 VK3 的发现）。
- `docs/dev-rules.md` / `docs/dev-rules-understanding.md`  # 开发规范执行手册与解释（登记见「开发规范理解文档补充」一节）。

## VK3 收口 + VK4 规划文档 (2026-07-08)
- `docs/rz-vk3-closure.md`  # VK3 收口确认：验收项表格、已完成阶段（VK3-A..VK3-C2-R1）、红线遵守确认、已知债务（UI 对 Render.Vulkan 工程级引用移交 VK4）、收口日期。VK3 结论：NativeHost HWND 生命周期已正式接入 Vulkan Instance + Surface；Swapchain 留 VK4。
- `docs/rz-vk4-plan.md`  # VK4 规划（只规划不实装）：最小渲染闭环 PhysicalDevice→LogicalDevice→Queue→Swapchain→ClearFrame→RenderSession 五问规划、目标依赖方向、阶段分解 VK4-A..VK4-E、防回潮门禁（Resize 不重建 Surface、不搬探针、UI 不持 Vulkan、每步 5+100）。
- `docs/rz-vk4-c-swapchain-plan.md`  # VK4-C-Plan（2026-07-08，只规划不实装）：Swapchain + Images + ImageViews 生命周期规划；边界（不建 RenderPass/Framebuffer/CommandPool/CommandBuffer、不 Clear/Present）、Resize 只重建 Swapchain+ImageViews、Dispose 顺序 ImageViews→Swapchain→LogicalDevice→Surface→Instance、独立 owner/attach step 文件结构、命名与 100 行红线。
- `docs/rz-vk4-c-r1-audit-plan.md`  # VK4-C-R1 审计与运行验证计划（2026-07-08）：只审计不新增能力、不进 VK4-D；静态审计结论 + 运行验证清单（14 项）+ Codex 指令。
- `docs/vk4-c-r1-swapchain-fix.svg`  # VK4-C-R1 Swapchain 重建修复对比图：修复前未设 OldSwapchain→失败 / 修复后传旧句柄→成功（透明背景、规范箭头，符合 Visualizer 约束）。
- `docs/log-ux-1-r2-autoscroll.svg`  # LOG-UX-1-R2 日志自动滚动可视化：跟随状态机（FOLLOW/PAUSED）+ 滚动时序（LogItems→LayoutUpdated→ScrollToEnd，规避 Extent 增长误判竞态）。
