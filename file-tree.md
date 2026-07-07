# 项目文件树 — XuanYu Engine

最近更新：RZ-Fix2-D 收口右侧检查器、调试、偏好与模式页职责；本轮未新增源文件。

统计口径：当前工作区真实文件快照，排除 `.git/`、`bin/`、`obj/` 生成目录。

当前文件总数：91

```
XuanYuEngine/
│
├── NuGet.Config  # NuGet 源配置。
├── changelog.md  # 项目变更记录，按时间倒序记录阶段性修改。
├── codex_log_xuanyu_handoff_20260705-2102.zip  # Codex 交接日志压缩包。
├── file-tree.md  # 当前文件树与文件职责说明。
├── run.bat  # Windows 启动脚本。
│
├── docs/
│   ├── AI_DEVELOPMENT_RULES.md  # AI 协作开发规则。
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
    │   └── Program.cs  # Avalonia 桌面启动入口。
    ├── Foot/
    │   ├── Foot.axaml  # 底部全局日志栏：摘要、过滤、搜索框、日志列表与右侧详情入口。
    │   ├── LogDetailPanel.axaml  # 日志详情面板：点击选中日志后显示详情并提供复制入口。
    │   ├── Foot.axaml.cs  # 底部栏代码后置。
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
