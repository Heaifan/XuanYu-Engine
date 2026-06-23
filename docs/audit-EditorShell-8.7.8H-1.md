# 8.7.8H-1 — EditorShell 最终 Boss 拆分审计

审计日期：2026-06-23
目标文件：`FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs`
目标行数：969 行

---

## 1. 当前文件状态

| 维度 | 值 |
|------|-----|
| **行数** | 969 行 |
| **类型** | `sealed partial class EditorShell : UserControl` — Avalonia UI 控件 |
| **Shell/ 目录** | 18 个子目录（Composition/、Input/、Lifecycle/ 等），6+ 个直属 .cs |
| **白名单** | ✅ 在 `CodeFileBudgetTests` 中（行白名单） |

## 2. XAML 事件绑定

**此文件没有 XAML 声明事件绑定。** 所有事件都在 `SubscribePanelEvents()` 中通过 `+=` 代码绑定：

| # | 事件来源 | 事件 | 处理方法 |
|---|----------|------|----------|
| 1 | ViewportPlaceholder | ViewportFocused | `HandleViewportFocused` |
| 2 | DockPanel | EntitySelectionRequested | `OnHierarchyEntitySelected` |
| 3 | DockPanel | ContentSelectionRequested | `OnProjectContentSelected` |
| 4-12 | VulkanViewportHost | 9 个原生事件 | `HandleRawPointerButtonDown` 等 |
| 13-19 | Inspector | 7 个变换事件 | `HandleTransformDraftChanged` 等 |
| 20 | RunMenuButton | Click | `HandleScene3dRunRequested` |
| 21 | PreferencesMenuItem | Click | `HandlePreferencesClicked` |
| 22 | ShowInputBindingsMenuItem | Click | `HandleShowInputBindingsClicked` |
| 23 | AboutFluidWarfareMenuItem | Click | `HandleAboutFluidWarfareClicked` |

**XAML 风险为 0** — 所有事件通过代码绑定，方法名可以自由改。

## 3. 职责拆解

| # | 职责 | 方法/区域 | 行数 | 占比 |
|---|------|-----------|------|------|
| 1 | **Route 组合 + 构造函数** | `EditorShell` ctor + `EditorShellRouteSet` 装配 | 38 | 4% |
| 2 | **面板事件订阅** | `SubscribePanelEvents` | 33 | 3% |
| 3 | **Attach/Detach 生命周期** | `OnAttachedToVisualTree` / `OnDetachedFromVisualTree` + 请求构建 | 25 | 3% |
| 4 | **Vulkan 启动探测** | `RunStartupVulkanProbe` / `BuildStartupVulkanRequest` / `ApplyStartupVulkanResult` | 25 | 3% |
| 5 | **视口重绘调度** | `ScheduleVulkanViewportRedraw` / `RedrawVulkanViewportOnce` / `ApplyResizeRenderResult` | 50 | 5% |
| 6 | **原生输入转发** | 9 个 `HandleRaw*` 方法 → `BuildInputRequest` → `_viewportInputRoute` | 45 | 5% |
| 7 | **场景工具仲裁** | `HandleSceneToolPointerPressed` / `HandleSceneToolPointerReleased` | 10 | 1% |
| 8 | **Transform Application 层** | `InitTransformApplication` / `BuildTransformStartSnapshot` / `ApplyPreviewPosition` / `CancelActiveTransform` | 35 | 4% |
| 9 | **视口工具切换** | `HandleViewportToolChanged` + 滚轮转发 | 12 | 1% |
| 10 | **FrameSelected** | `ExecuteViewportFrameSelected` | 22 | 2% |
| 11 | **EditorViewportInputRequest 构建** | `BuildInputRequest` — 30+ 个参数的管家方法 | 12 | 1% |
| 12 | **Scene3dCommand 构建 + 执行** | `BuildScene3dCommandRequest` / `ApplyScene3dCommandResult` | 16 | 2% |
| 13 | **菜单命令** | `HandlePreferencesClicked` / `HandleScene3dRunRequested` / `ExecuteOpenPreferences` 等 | 30 | 3% |
| 14 | **Overlay 导航** | `GetPresentedNavigationLayout` + `HandleOverlayPointerPressed/Moved/Released/CaptureLost` | 80 | 8% |
| 15 | **选择路由** | `ApplyEntitySelection` / `SyncSceneSelection` / `ClearSelection` / `MapReason` | 35 | 4% |
| 16 | **地面指针移动** | `HandleViewportPointerMoved` + `HandleViewportPointerLeft` | 35 | 4% |
| 17 | **Picking** | `HandleViewportPick` | 20 | 2% |
| 18 | **地面标记控制** | `ShowGroundCursor` / `HideGroundCursor` | 18 | 2% |
| 19 | **Transform 编辑** | `HandleTransformApply` / `HandleTransformReset` / `HandleTransformDraftChanged` | 55 | 6% |
| 20 | **数值拖拽** | `HandleScrubValueChanged` / `HandleScrubCompleted` / `HandleScrubCancelled` | 22 | 2% |
| 21 | **GroundPlacement** | `HandleGroundPlacementToggle` / `CompleteGroundPlacement` | 20 | 2% |
| 22 | **层级树构建** | `RebuildAndShowHierarchy` / `BuildGroupLookup` | 15 | 2% |
| 23 | **诊断刷新 + 工具方法** | `RefreshDiagnostics` / `TryGetValidViewportSize` / `UpdateVulkanViewportHost` | 18 | 2% |
| — | 字段声明 | 42 个字段 | 42 | 4% |
| — | 其他/空行/注释 | — | ~260 | 27% |

### 关键发现

**此文件没有任何 XAML 事件绑定。** 所有事件通过 `+=` 在 `SubscribePanelEvents()` 中代码绑定。这意味着拆分时方法名可以自由改名，不需要担心 XAML 引用断掉。

## 4. 字段归属分析

| 字段 | 当前归属 | 拆分建议 |
|------|----------|----------|
| `_selectionRoute`, `_worldState`, `_contentFiles`, `_projectInfo` | Shell | **SelectionRoute** — 保持 |
| `_lifecycle`, `_sessionActive`, `_renderSeq`, `_renderLastMode` | Shell | **Scene3dSessionContext** — 移出 |
| `_cameraRoute`, `_navigationRoute`, `_viewportFocusRoute` | Shell | **NavigationRoute** — 保持（已 Route） |
| `_viewportInputRoute`, `_groundHoverRoute`, `_pickInputRoute` | Shell | **InputRoute** — 保持（已 Route）|
| `_viewportPickRoute`, `_viewportToolPalette` | Shell | 保持 |
| `_pointerRoute`, `_renderSceneStore` | Shell | 保持（已 Route）|
| `_previewApplier`, `_commitApplier`, `_cancelApplier` | Shell | **TransformLifecycleScope** — 移出 |
| `_probeRoute`, `_feedback`, `_runMenu`, `_startupVulkanRoute` | Shell | 保持（已 Route）|
| `_diagnosticsRoute`, `_resizeRenderRoute`, `_windowRoute` | Shell | 保持（已 Route）|
| `_scene3dCommandRoute`, `_panelApplyRoute`, `_transformApplyRoute` | Shell | 保持（已 Route）|
| `_groundPlacementRoute`, `_groundPlacementState`, `_groundPointerState` | Shell | 保持 |
| `_worldDirtyState`, `_frameSelectedPending` | Shell | 保持 |
| `_viewportResizeRenderTimer` | Shell | **VulkanViewportLifecycle** — 移出 |
| `_startupRoute`, `_attachRoute`, `_detachRoute` | Shell | 保持（已 Route）|
| `_viewportPlaceholderPanel`, `_vulkanViewportHostPanel`, `_dockPanel` | Shell | **PanelRefs** — 移出 |
| `_inspectorPanel`, `_debugDockPanel`, `_statusBarPanel` | Shell | **PanelRefs** — 移出 |
| `_worldSelectionPresenter`, `_contentSelectionPresenter`, `_viewportSelectionPresenter` | Shell | **SelectionPresenterScope** — 移出 |
| `_navigationRoute` | Shell | 保持（已 Route）|

## 5. 可立即拆分与高风险清单

### ✅ 可立即拆分（低风险，纯提取）

| 方法 | 提取目标 | 原因 |
|------|----------|------|
| `Overlay 导航` (6 方法, 80 行) | `EditorShellNavigationHandler` | 与 Shell 其他职责无交叉，仅依赖 Route |
| `地面指针` (2 方法, 35 行) | `EditorShellGroundPointerHandler` | 逻辑独立，仅依赖 Route |
| `Transform 编辑` (4 方法, 55 行) | `EditorShellTransformHandler` | 逻辑独立 |
| `Picking` + 地面标记 (4 方法, 38 行) | `EditorShellPickHandler` | 逻辑独立 |
| `数值拖拽` (3 方法, 22 行) | `EditorShellScrubHandler` | 逻辑独立 |
| `层级树` (2 方法, 15 行) | 留在 Shell 或移入 PanelApply | 非常简单 |

### ⚠️ 不建议立即拆的高风险清单

| 方法 | 风险原因 |
|------|----------|
| `构造函数 + SubscribePanelEvents + Route 装配` | 引擎初始化顺序敏感，拆错改不了 |
| `Attach/Detach` 生命周期 | 涉及 VisualTree 生命周期，与 Avalonia 绑定 |
| `Vulkan 启动探测 + 重绘调度` | 涉及 Probe → Session → Resize 全链路，顺序敏感 |
| `原生输入转发` (HandleRaw*) | 已经非常薄（每个方法 1-3 行），再拆就是为拆而拆 |

## 6. 推荐拆分阶段

### 阶段 H-2A：提取 Overlay 导航 + 地面指针 + Picking（~140 行）

| 新文件 | 行数 | 职责 |
|--------|------|------|
| `Shell/Navigation/EditorShellNavigationHandler.cs` | ≤100 | Overlay 导航 6 方法 + `GetPresentedNavigationLayout` |
| `Shell/Picking/EditorShellGroundPointerHandler.cs` | ≤100 | 地面指针移动/离开 + Picking + 地面标记 |

### 阶段 H-2B：提取 Transform 编辑 + 数值拖拽 + Scrub（~80 行）

| 新文件 | 行数 | 职责 |
|--------|------|------|
| `Shell/Transform/EditorShellTransformHandler.cs` | ≤100 | `HandleTransformApply/Reset/DraftChanged` + `ApplyEntityTransform` + `CompleteGroundPlacement` |
| `Shell/Transform/EditorShellScrubHandler.cs` | ≤50 | `HandleScrubValueChanged/Completed/Cancelled` |

### 阶段 H-2C：提取 Viewport 生命周期 + 窗口命令（~70 行）

| 新文件 | 行数 | 职责 |
|--------|------|------|
| `Shell/Viewport/EditorShellViewportHandler.cs` | ≤100 | `ScheduleVulkanViewportRedraw` + `RedrawVulkanViewportOnce` + `ApplyResizeRenderResult` |
| `Shell/Viewport/EditorShellWindowCommands.cs` | ≤50 | 菜单命令 + `ApplyWindowResult` |

### 预计效果

```
阶段    提取行数    Shell 剩余    备注
H-2A       140        829        只拆低风险
H-2B        80        749        继续减
H-2C        70        679        仍 > 100，还需 3-4 轮
...          -        ≤100       可能需要 ~6 轮
```

**100 行红线 vs 现实：**
EditorShell 是组合根 + 事件总线，部分代码（构造函数 + 事件订阅 + Route 装配 + Attach/Detach）**必须保留**在 Shell 中作为编排核心。这部分 ~120 行。加上字段声明 ~30 行。剩余 routing 代码约 20 行。**Shell 最终能减到 ~150-200 行**，但 ≤100 行可能不现实 — 不像其他文件那样可以将逻辑完全提取到独立类。

## 7. 结论

| 维度 | 结论 |
|------|------|
| **是否可以拆** | ✅ **可以拆，分多轮进行** |
| **最大风险** | **Route 装配顺序** — 构造函数中的 `EditorShellRouteBuild.Build` 涉及 20+ Route 的创建与注入 |
| **最大优势** | **无 XAML 事件绑定** — 所有事件代码绑定，方法名可自由改 |
| **推荐第 1 刀** | **H-2A** — 提取 Overlay 导航 + 地面指针 + Picking（~140 行，3 个文件）|
| **Shell 最终行数** | 预计 ~200 行（组合根 + 编排核心），≤100 行可能不现实 |
| **是否需要先整理目录** | **是** — Shell/ 根目录已经有多文件，建议将 Handler 类放入子目录 |
| **下一轮建议** | H-2A：执行第 1 刀拆分 |

### 一句话总结

EditorShell 是**组合根 + 事件总线 + 编排核心**，这与 God 类不同。它的"大"来自于它必须知道所有子系统的存在来协调它们。这 969 行里大约 **400 行可以安全提取**到 Handler/Presenter 类中，剩下的 ~200 行是合法的组合根代码。
