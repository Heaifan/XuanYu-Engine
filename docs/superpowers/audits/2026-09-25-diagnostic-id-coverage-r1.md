# Diagnostic ID Coverage Audit R1

审计基线：`64a8c2c55a27a42097928417a49bdd21c4799638`  
审计分支：`audit/diagnostic-id-coverage-r1`  
审计范围：XuanYu.Editor.UI 生产组合、Diagnostic 解析/注册/交互代码，以及相关 UiRuntime 测试；不修改生产注册、不补 DebugId、不改变行为。

## 结论

- 当前生产 UI 可达且可注册的唯一语义 DebugId：6 个：`XYE.MENU`、`XYE.CONTEXT_TOOLBAR`、`XYE.PROJECT_TREE`、`XYE.VIEWPORT`、`XYE.LAYER_DOCK`、`XYE.LOG_PANEL`。
- `XYE.VIEWPORT` 的 Avalonia `VulkanViewport` 与 Native `VulkanNativeHost` 是同一语义目标的两条路径；Native 路径不写入 `DiagnosticRegistry`，因此不构成注册重复。
- 检查器 ID（地图、标记、道路、区域及道路/区域四个分区）仍存在于旧面板文件/旧运行时测试中，但当前 `EditorRightTabs` 只组合 `InspectorPanel`，这些 ID 当前为 `LEGACY`，不是当前生产覆盖。
- 当前覆盖的 12 个目标面中有 8 个：Menu、Context Toolbar、Project Tree、Viewport、Layer Dock、Log Panel、Bottom、Native Viewport；未覆盖的关键面是 Top Toolbar、Inspector 容器、Popup、Dialog。

## 统计口径

| 指标 | 数值 | 口径 |
|---|---:|---|
| Total | 18 | 生产源码中可辨识的语义 DebugId 值：6 个当前生产值 + 12 个旧检查器值；不含 `XYE.AREA.*`、测试专用 `XYE.TEST.*` 和仅文档示例 |
| Unique | 18 | 按大小写敏感字符串/动态模板值去重后的语义值 |
| Duplicate | 0 | 当前注册表内相同 DebugId 的正向重复；`XYE.VIEWPORT` 的 Avalonia/Native 双路径按有意别名处理，不计重复 |
| Missing critical | 4 | Top Toolbar、Inspector 容器、Popup、Dialog；按用户要求的 12 个目标面统计 |
| Legacy | 12 | `XYE.INSPECTOR.MAP/MARKER/ROAD/REGION` 及道路/区域四分区 ID，定义在未进入当前生产组合的旧面板中 |
| Coverage | 8/12 = 66.7% | 目标面存在可解析 DebugId 或明确的 Native 特殊路径；Log Panel 与 Bottom 由同一 `Foot` 目标覆盖，但按目标面分别计数 |

## ID 明细

状态含义：`PASS` = 当前生产路径可定位且交互链存在；`MISSING` = 当前生产路径缺少语义 ID；`DUPLICATE` = 同一注册表出现重复值；`AMBIGUOUS` = 语义/路径无法唯一判定；`LEGACY` = 仅旧面板或旧组合仍有定义。

| DebugId | 中文名称 | 组件类型 | 代码位置 | 区域 | 注册方式 | 唯一性 | Hover | Click Track | Copy AI | Popup/Native 特殊路径 | 状态 |
|---|---|---|---|---|---|---|---|---|---|---|---|
| `XYE.MENU` | 文件菜单模块 | `FileModule` | `XuanYu.Editor.UI/Top/Top.axaml:15` | Top / Menu | XAML attached property；`UiRoot.RefreshDiagnosticRegistry()` 扫描可视树后注册 | 当前注册表唯一 | `DiagnosticProbeResolver` 语义解析；XYUI 控件映射优先 | 探针点击锁定目标；普通 Pointer 仍由原控件处理 | `DiagnosticFloatingCard`/`DiagnosticBadge` 提供复制诊断文本 | 区域边界使用 Badge Popup；普通探针使用 floating tool window | PASS |
| `XYE.CONTEXT_TOOLBAR` | 上下文工具栏 | `ContextToolBar` | `XuanYu.Editor.UI/Top/Top.axaml:29` | Top / Context Toolbar | 同上，静态 attached property | 当前注册表唯一 | 同上；XYContextToolbar 映射为 `XYUI3` | 同上 | 同上 | 无独立 Native 路径 | PASS |
| `XYE.PROJECT_TREE` | 项目树 | `ProjectWorkspace` | `XuanYu.Editor.UI/Left/ProjectWorkspace.axaml:6` | Left / Project Tree | 同上，静态 attached property | 当前注册表唯一 | 父级语义 ID 可作为 `ParentDebugId`；子项使用语义/命名解析 | 探针点击不替代树项命令；诊断快捷键可锁定 | 同上 | 区域边界 Badge Popup；无 Native 路径 | PASS |
| `XYE.VIEWPORT` | 渲染视口 | `VulkanViewport` / `VulkanNativeHost` | `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml:6`；`DiagnosticNativeViewportTarget.cs:8-12` | Center / Viewport | Avalonia 根控件静态注册；Native Move 通过 `DiagnosticNativeViewportTarget.Create()` 生成固定目标，不进入 Registry | 注册表唯一；双路径有意汇聚 | Avalonia PointerMoved 解析；Native Move 旁路覆盖 Avalonia 结果 | Native 目标仅更新诊断卡，不接管 Camera/Picking/Gizmo；退出后恢复 Avalonia | Tool window 使用 `CopyToolText`；无 Native Popup | Native pointer 事件、坐标映射、离开清理；Viewport 目标不画普通边框高亮 | PASS |
| `XYE.LAYER_DOCK` | 图层停靠区 | `EditorLayerDock` | `XuanYu.Editor.UI/Right/EditorLayerDock.axaml:6` | Right / Layer Dock | 同上，静态 attached property；仅编辑模式可见 | 当前注册表唯一 | 通用语义解析 | 诊断点击不吞掉图层选择/拖拽命令 | 同上 | 区域边界 Badge Popup；无 Native 路径 | PASS |
| `XYE.LOG_PANEL` | 日志面板 | `Foot` | `XuanYu.Editor.UI/Foot/Foot.axaml:1` | Bottom / Log Panel | 同上，静态 attached property | 当前注册表唯一 | 通用语义解析；日志内部控件无独立 DebugId | 诊断探针与日志筛选/展开命令并存 | 同上 | `SourceFilterPopup` 会被 Popup 根追踪；面板本身无 Native 路径 | PASS |
| `XYE.INSPECTOR` | 检查器容器 | `InspectorPanel`（当前） | `XuanYu.Editor.UI/Right/EditorRightTabs.axaml:25-27`；当前 `InspectorPanel.axaml` 无 DebugId | Right / Inspector | 无当前生产 attached property 或运行时注册 | 无 | 可解析内部控件，但无法稳定反向归属到 Inspector 语义 ID | 可探针点击内部控件；无 Inspector 容器语义锁定 | 只能复制内部目标的 `N/A`/局部快照 | Inspector 内部 Popup/Dialog 没有统一语义根 | MISSING |
| `XYE.INSPECTOR.MAP` | 地图检查器 | `MapEditorPanel` | `XuanYu.Editor.UI/Right/MapEditorPanel.axaml:6-8` | Right / Inspector | 静态 attached property，但文件为旧面板；当前组合未引用 | 当前生产注册不存在 | 旧面板可解析 | 旧面板可探针 | 旧面板可复制 | 非当前生产路径 | LEGACY |
| `XYE.INSPECTOR.MARKER` | 地图标记检查器 | `MarkerInspectorPanel` | `XuanYu.Editor.UI/Right/MarkerInspectorPanel.axaml:6-8` | Right / Inspector | 静态 attached property，但文件为旧面板；当前组合未引用 | 当前生产注册不存在 | 旧面板可解析 | 旧面板可探针 | 旧面板可复制 | 非当前生产路径 | LEGACY |
| `XYE.INSPECTOR.ROAD` | 道路检查器 | `FeatureInspectorPanel` | `FeatureDiagnosticIds.cs:19-26`；旧面板动态绑定 `FeatureInspectorPanel.axaml:8-14` | Right / Inspector | 动态 Binding；仅旧 Feature 面板实例化时注册 | 当前生产注册不存在 | 旧面板可解析 | 旧面板可探针 | 旧面板可复制 | 非当前生产路径 | LEGACY |
| `XYE.INSPECTOR.ROAD.BASIC` | 道路·基础 | `XYToggleButton` | `FeatureInspectorPanel.axaml:8`；`UiVm.DiagnosticIdentity.cs:6` | Right / Inspector | 动态 Binding | 旧面板实例内唯一；当前生产不可达 | 旧面板可解析 | 旧面板可探针 | 旧面板可复制 | 非当前生产路径 | LEGACY |
| `XYE.INSPECTOR.ROAD.GEOMETRY` | 道路·几何 | `XYToggleButton` | `FeatureInspectorPanel.axaml:10`；`UiVm.DiagnosticIdentity.cs:7` | Right / Inspector | 动态 Binding | 旧面板实例内唯一；当前生产不可达 | 同上 | 同上 | 同上 | 非当前生产路径 | LEGACY |
| `XYE.INSPECTOR.ROAD.STATE` | 道路·状态 | `XYToggleButton` | `FeatureInspectorPanel.axaml:12`；`UiVm.DiagnosticIdentity.cs:8` | Right / Inspector | 动态 Binding | 旧面板实例内唯一；当前生产不可达 | 同上 | 同上 | 同上 | 非当前生产路径 | LEGACY |
| `XYE.INSPECTOR.ROAD.RELATIONS` | 道路·关联 | `XYToggleButton` | `FeatureInspectorPanel.axaml:14`；`UiVm.DiagnosticIdentity.cs:9` | Right / Inspector | 动态 Binding | 旧面板实例内唯一；当前生产不可达 | 同上 | 同上 | 同上 | 非当前生产路径 | LEGACY |
| `XYE.INSPECTOR.REGION` | 区域检查器 | `FeatureInspectorPanel` | `FeatureDiagnosticIds.cs:19-26`；旧面板动态绑定 `FeatureInspectorPanel.axaml:8-14` | Right / Inspector | 动态 Binding；仅旧 Feature 面板实例化时注册 | 当前生产注册不存在 | 旧面板可解析 | 旧面板可探针 | 旧面板可复制 | 非当前生产路径 | LEGACY |
| `XYE.INSPECTOR.REGION.BASIC` | 区域·基础 | `XYToggleButton` | `FeatureInspectorPanel.axaml:8`；`UiVm.DiagnosticIdentity.cs:6` | Right / Inspector | 动态 Binding | 旧面板实例内唯一；当前生产不可达 | 旧面板可解析 | 旧面板可探针 | 旧面板可复制 | 非当前生产路径 | LEGACY |
| `XYE.INSPECTOR.REGION.GEOMETRY` | 区域·几何 | `XYToggleButton` | `FeatureInspectorPanel.axaml:10`；`UiVm.DiagnosticIdentity.cs:7` | Right / Inspector | 动态 Binding | 旧面板实例内唯一；当前生产不可达 | 同上 | 同上 | 同上 | 非当前生产路径 | LEGACY |
| `XYE.INSPECTOR.REGION.STATE` | 区域·状态 | `XYToggleButton` | `FeatureInspectorPanel.axaml:12`；`UiVm.DiagnosticIdentity.cs:8` | Right / Inspector | 动态 Binding | 旧面板实例内唯一；当前生产不可达 | 同上 | 同上 | 同上 | 非当前生产路径 | LEGACY |
| `XYE.INSPECTOR.REGION.RELATIONS` | 区域·关联 | `XYToggleButton` | `FeatureInspectorPanel.axaml:14`；`UiVm.DiagnosticIdentity.cs:9` | Right / Inspector | 动态 Binding | 旧面板实例内唯一；当前生产不可达 | 同上 | 同上 | 同上 | 非当前生产路径 | LEGACY |

## 目标面反向检查

| 目标面 | 反向检查结果 | 证据/说明 |
|---|---|---|
| Menu | PASS | `Top.axaml:15` 有 `XYE.MENU` |
| Top Toolbar | MISSING | `Top` 仅有 Menu 与 Context Toolbar 子目标，没有 Top Toolbar 容器 DebugId |
| Context Toolbar | PASS | `Top.axaml:29` 有 `XYE.CONTEXT_TOOLBAR` |
| Project Tree | PASS | `ProjectWorkspace.axaml:6` |
| Viewport | PASS | `VulkanViewport.axaml:6`；Native 旁路固定生成 `XYE.VIEWPORT` |
| Inspector | MISSING | 当前组合为 `InspectorPanel`，其源码没有 `XYDiagnostic.DebugId`；旧 Inspector 面板未组合 |
| Layer Dock | PASS | `EditorLayerDock.axaml:6` |
| Log Panel | PASS | `Foot.axaml:1` |
| Bottom | PASS | Root Bottom 区域承载 `Foot`，`UiRoot.axaml:74` + `Foot.axaml:1` |
| Popup | MISSING | Popup 根仅通过 `AttachProbePopups()` 追踪，未发现稳定语义 DebugId；区域边界 Popup 是诊断载体，不是业务 Popup ID |
| Dialog | MISSING | `Win` 下 Dialog/Confirmation Window 未发现 DebugId；当前 Registry 只扫描 `UiRoot` 可视后代 |
| Native Viewport | PASS | Native Enter/Move/Exit 事件转换到固定 `XYE.VIEWPORT`，并在退出时清理覆盖状态 |

## 交互链证据

- 注册：`DiagnosticRegistry.Register()` 对 ID 做校验并以 Ordinal 字典 `TryAdd`，重复时抛出异常；`UiRoot.RefreshDiagnosticRegistry()` 在 Loaded/Attached/InspectorIdentity 变化时清空并重新扫描。
- Hover：`DiagnosticOverlayHost.OnProbePointerMoved()` 记录指针、过滤诊断覆盖层，并调用 `ProbeHover()`；Native Viewport override 存在时忽略 Avalonia 结果。
- Click Track：`ProbeClick()` 在存在 DeepVisual 控件时锁定当前探针并复制 AI 诊断报告；`DiagnosticBadge.OnPressed()` 支持普通 ID 复制与 Shift+click 快照复制；Ctrl+Shift+C 走同一 ProbeClick 链。
- Copy AI：`DiagnosticOverlayHost.ToolWindow` 将 `CopyToolText` 传入卡片；`DiagnosticBadge` 的 Shift 路径使用 `DiagnosticSnapshotFactory`。本审计未宣称本轮运行测试通过，仅记录源码与已有测试断言覆盖。
- Popup：区域边界由 `DiagnosticRegistry.Targets` 创建非焦点、OverlayLayer Popup；业务 Popup 打开/关闭时由 `AttachProbePopup()` / `ClearProbeForRoot()` 维护探针根。
- Native：`DiagnosticNativeViewportTarget.Create()` 固定产生 `XYE.VIEWPORT`、`VulkanViewport`；Native Exit 清理 override，使后续 Avalonia probe 恢复。

## FOLLOW-UP（仅缺失项）

1. 为当前生产 `InspectorPanel` 建立一个稳定的 Inspector 容器语义 ID，并重新定义地图/标记/道路/区域分支是否需要独立 ID；不要直接恢复已判定为 LEGACY 的旧面板组合。
2. 为 `Top` 建立明确的 Top Toolbar 容器 ID，保留现有 `XYE.MENU` 与 `XYE.CONTEXT_TOOLBAR` 子目标。
3. 为需要被诊断的业务 Popup 建立稳定 Popup 语义根；不要把诊断 Badge Popup 本身当作业务目标。
4. 为 Dialog/Confirmation Window 建立跨 Window 可发现的注册边界；当前 `UiRoot` 局部扫描无法覆盖独立 Window。

## 证据索引

- 生产声明与 ID 工厂：`XuanYu.Editor.UI/Diagnostic/FeatureDiagnosticIds.cs:5-26`。
- 注册唯一性与扫描：`XuanYu.Editor.UI/Diagnostic/DiagnosticRegistry.cs:6-22`、`XuanYu.Editor.UI/Root/UiRoot.axaml.cs:47-52`。
- 语义 Hover/路径解析：`XuanYu.Editor.UI/Diagnostic/DiagnosticProbeResolver.cs:12-83`、`DiagnosticProbeResolver.Semantic.cs:11-31`。
- 交互/复制：`XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.Interaction.cs:15-72`、`DiagnosticBadge.axaml.cs:13-31`、`DiagnosticOverlayHost.ToolWindow.cs:14-62`。
- Popup：`XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.Sync.cs:8-46`、`DiagnosticOverlayHost.PopupProbe.cs:14-45`。
- Native Viewport：`DiagnosticNativeViewportTarget.cs:6-12`、`DiagnosticOverlayHost.NativeViewportProbe.cs:27-79`。
- 运行时注册测试证据：`XuanYu.World.Tests/UiRuntime/DiagnosticRegistrationRuntimeTests.cs:19-64`；交互测试证据：`DiagnosticOverlayRuntimeTests.cs:47-79`；Native 所有权测试证据：`DiagnosticNativeTargetOwnershipTests.cs:24-65`。

## 本轮验证

- `git diff --check`：通过。
- 目标 Diagnostic 测试未能启动：当前环境没有安装 .NET SDK（`dotnet test ... --no-restore` 返回 `No .NET SDKs were found`）。因此本文只把仓库中已有测试断言作为源码证据，不宣称本轮测试通过。
