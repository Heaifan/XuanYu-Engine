# 8.7.8H-3 — EditorShell 剩余代码重新审计

审计日期：2026-06-23
目标文件：`FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs`
目标行数：656 行（Read 计数；Measure-Object 报告 567 行）

---

## 1. 当前文件结构

| 区域 | 行号 | 行数 | 占比 |
|------|------|------|------|
| Using 指令 | 1-93 | 93 | 14% |
| 字段声明 | 98-193 | 96 | 15% |
| Constructor | 195-285 | 91 | 14% |
| SubscribePanelEvents | 287-326 | 40 | 6% |
| 生命周期事件 | 328-360 | 33 | 5% |
| 剩余方法 | 362-654 | 293 | 45% |
| 类尾 | 655-656 | 2 | 0% |

---

## 2. 字段分类

### A. UI 控件引用（7 个，不可移出）

```csharp
InspectorPanel? _inspectorPanel;
DebugDockPanel? _debugDockPanel;
StatusBarPanel? _statusBarPanel;
ViewportPlaceholderPanel? _viewportPlaceholderPanel;
VulkanViewportHostPanel? _vulkanViewportHostPanel;
ProjectWorldDockPanel? _dockPanel;
ViewportToolPalette? _viewportToolPalette;
```

这些必须留在 Shell 作为单一控件查找点。不可移出。

### B. 业务对象引用（3 个，不可移出）

```csharp
IReadOnlyList<GameContentFileInfo>? _contentFiles;
GameProjectInfo? _projectInfo;
WorldState? _worldState;
```

多处读取，Shell 级状态。_contentFiles 被 OnProjectContentSelected 读取，_projectInfo 被 RebuildAndShowHierarchy 读取，_worldState 被 10+ 方法读取。

### C. Route 装配字段（30 个，必须留 Shell）

从 `EditorShellRouteBuild.Build` 创建的 26+ Route 引用。所有字段通过 `_r = Build(...)` 赋值，再由 Shell 分发给各 H-x 子路由。这是组合装的钉板，不可移出。

如 `_selectionRoute`, `_cameraRoute`, `_lifecycle`, `_viewportInputRoute` 等。这些字段的构造期赋值（`= new()`）是无效的（立即被 Build 覆盖），只是 C# 语法要求。

**结论：不可移出，约 30 行。**

### D. Transform Application 层（3 个，可考虑移出但收益低）

```csharp
EntityTransformPreview? _previewApplier;
EntityTransformCommit? _commitApplier;
EntityTransformCancel? _cancelApplier;
```

由 InitTransformApplication 创建，被 ApplyPreviewPosition / CancelActiveTransform / CompleteGroundPlacement / BuildInputRequest 读取。若移出，需要将这些方法一起移出（约 50 行），属于 H-4 候选。

### E. Selection Presentation（3 个，Shell 层中转）

```csharp
WorldEntitySelectionPresenter _worldSelectionPresenter;
ProjectContentSelectionPresenter _contentSelectionPresenter;
ViewportSelectionPresenter _viewportSelectionPresenter;
```

由 Shell 创建，传递给 H-2 子路由或直接在 Shell 方法中调用。

- `_viewportSelectionPresenter`：只在 H-2G 路由构造时传入 → **可移至 ProjectBootstrapRoute**（减少 1 字段）
- `_worldSelectionPresenter`：在 ShowWorldEntitySelection 中使用 → 可作为回调参数移入选择路由

### F. 状态字段（4 + 1 个）

```csharp
bool _sessionActive;           // 多路由读取，不可移
int _renderSeq;                // 多路由读取，不可移
string _renderLastMode;        // 仅 RefreshDiagnostics 读取
bool _frameSelectedPending;    // 仅 ExecuteViewportFrameSelected 使用
EditorGroundPointerState _groundPointerState;   // 传递给子路由
EditorGroundPlacementState _groundPlacementState; // 传递给子路由 + HandleViewportEscape 读取
EditorWorldDirtyState _worldDirtyState;  // 传递给 BuildInputRequest
```

- `_renderLastMode`：仅 RefreshDiagnostics 读取 → **可随 RefreshDiagnostics 移出**
- `_frameSelectedPending`：随 ExecuteViewportFrameSelected 移出
- 其余状态字段：跨方法引用多，不宜移

### G. H-2 子路由（11 个，已提取完毕）

```
_overlayNavRoute, _groundPointerRoute, _pickingRoute,
_transformRoute, _scrubRoute,
_viewportRedrawRoute,
_windowCommandsRoute,
_hierarchyRoute, _selectionSyncRoute,
_startupProbeRoute,
_projectBootstrapRoute
```

这些是目标终点字段，不能再移。

---

## 3. 构造函数分析（91 行）

| 步骤 | 行数 | 性质 |
|------|------|------|
| XamlLoader.Load | 1 | 框架要求 |
| FindControls | 1 | 必需 |
| Panel 赋值 | 1 | 必需 |
| RouteBuild.Build | 1 | 必需 |
| Route 字段赋值 | 10 | 必需 |
| Diagnostics 上下文 | 3 | 可移但需要先移 DiagnosticsRoute |
| H-2 路由创建 | ~60 | 已提取完毕 |
| SubscribePanelEvents | 1 | 必需 |
| InitializeFeedback | 1 | 必需 |
| 启动加载/探测 | 3 | 已路由化 |

**结论：构造函数约 80 行为必需组合根代码，不可再移。**

---

## 4. SubscribePanelEvents 分析（40 行）

纯事件接线代码，将面板事件分配给 Shell 自身方法或 H-x 子路由：

- ViewportPlaceholder → 1 个事件（HandleViewportFocused）
- DockPanel → 2 个事件（EntitySelectionRequested, ContentSelectionRequested）
- VulkanViewportHost → 15 个事件（redraw, raw input, navigation, picking, pointer）
- Inspector → 7 个事件（transform, scrub）

**结论：不可移出。移出会导致 Shell 失去事件可见性和接线控制。**

---

## 5. 剩余方法逐项分析

### A. 生命周期（~30 行）⚠️ 必须保留

```csharp
OnAttachedToVisualTree / OnDetachedToVisualTree  // Avalonia 生命周期
BuildAttachRequest / ApplyAttachResult            // Attach 请求构建
BuildDetachRequest / ApplyDetachResult            // Detach 请求构建
```

这些是 Avalonia UserControl 生命周期钩子，必须留在 Shell 中。

### B. 日志委托（3 行）✅ 可移出

```csharp
AppendInfoLog / AppendWarningLog / AppendErrorLog
```

3 行 1 行式的 _feedback 转发。**建议移出到新文件 Shell/Feedback/EditorShellFeedbackRoute.cs**。移出后可减少 3 行，并消除 usings 依赖。

### C. 面板初始化（~5 行）⚠️ 必须保留

```csharp
InitializeFeedback()
```

### D. 聚焦处理（~8 行）✅ 可移出

```csharp
HandleViewportFocused
```

调用 _viewportFocusRoute.Focus + _panelApplyRoute.ShowViewportFocused + 日志。可移出到 Shell/Viewport/ 下新增的 EditorShellViewportFocusRoute.cs，约 -8 行。

### E. 实体选择转发（~5 行）⚠️ 薄层，保留

```csharp
OnHierarchyEntitySelected      // 1 行委托至 _selectionSyncRoute
OnProjectContentSelected       // 5 行调用 _contentSelectionPresenter + _panelApplyRoute
```

OnProjectContentSelected 可移出到 Shell/Project/ 下，约 -5 行。

### F. 实体选择展示（~8 行）✅ 可移出

```csharp
ShowWorldEntitySelection
```

调用 _worldSelectionPresenter + _panelApplyRoute。可移入 Shell/Selection/ 下已有的 EditorShellSelectionSyncRoute 或新建展示路由，约 -8 行。

### G. Scene3D 命令（~16 行）✅ 可移出

```csharp
HandleScene3dRunRequested         // 5 行
HandleRestartScene3d              // 4 行
ApplyScene3dCommandResult         // 7 行
```

这些是 Scene3D Session 的启动/重启/结果应用。可移出到 Shell/Scene3D/EditorShellScene3dCommandRoute.cs，约 -16 行。

但注意 ApplyScene3dCommandResult 被 StartupProbeRoute 的 Scene3dRestart 回调和 HandleScene3dRunRequested 引用。

### H. 输入管线初始化（~9 行）⚠️ 必须保留

```csharp
InitializeInputPipeline
```

涉及 EditorInputService.Instance + _viewportInputRoute.State.Translator 初始化。这是输入管线的连接点，不宜移。

### I. Raw 输入转发（~29 行）✅ 可移出

```csharp
HandleRawKeyDown / KeyUp / PointerButtonDown / PointerMoved / PointerButtonUp / InputFocusLost / MouseWheel
```

共 7 个方法，每个 1-3 行，都是转发到 _viewportInputRoute。这些是"体量大、单行薄"的胶水代码。如果合并到一个 Shell/Input/EditorShellRawInputRoute.cs，约 -29 行。

### J. 场景工具仲裁（~9 行）✅ 可随 Raw 输入移出

```csharp
HandleSceneToolPointerPressed / HandleSceneToolPointerReleased
```

### K. Transform 初始化 + 管线方法（~37 行）⚠️ 紧密耦合，可考虑移出

```csharp
InitTransformApplication        // 14 行
BuildTransformStartSnapshot     // 15 行
ApplyPreviewPosition            // 4 行
CancelActiveTransform           // 4 行
```

这些由 ApplyScene3dCommandResult 和 BuildInputRequest 引用。InitTransformApplication 创建 _previewApplier/_commitApplier/_cancelApplier 实例。移出后需要在路由中管理这些实例的生命周期。**可移出但需要小心生命周期**。约 -37 行。

### L. 视口工具（~10 行）✅ 可移出

```csharp
HandleViewportToolChanged       // 6 行
HandleRawMouseWheel             // 4 行（但已计入 Raw 输入）
```

HandleViewportToolChanged 可移入 Shell/Viewport/ 路由，约 -6 行。

### M. Frame Selected（~22 行）✅ 可移出

```csharp
ExecuteViewportFrameSelected
```

调用 _cameraRoute + ScheduleScene3dFrame + 状态栏。可移出到 Shell/Viewport/ 下的聚焦路由，约 -22 行。

### N. BuildInputRequest（~10 行）⚠️ 保留

```csharp
BuildInputRequest
```

30+ 参数的管家方法，几乎所有 Shell 方法都引用它。不可移。

### O. BuildScene3dCommandRequest（~4 行）⚠️ 保留（或随 Scene3D 命令移出）

### P. 空操作/未使用（~9 行）✅ 可删除

```csharp
ExecuteTransformApply           // 4 行（空方法！）
ExecuteCancelCurrentTool        // 4 行（委托至 HandleViewportEscape）
ExecuteTransformResetDraft      // 5 行（委托至 _transformRoute）
```

这些是键盘命令命中方法。`ExecuteTransformApply` 是空方法体。可删除 -9 行。

### Q. Overlay / Picking / Transform 薄转发（~30 行）⚠️ 保留

```csharp
HandleViewportEscape            // 9 行
ScheduleScene3dFrame            // 5 行
HandleViewportPick              // 3 行
ApplyEntityTransform            // 2 行
CompleteGroundPlacement         // 6 行
RefreshDiagnostics              // 1 行
UpdateVulkanViewportHost        // 1 行
```

这些是 H-2 提取后剩余的薄转发壳。保留（已尽可能薄）。

### R. TryGetValidViewportSize（~20 行）✅ 可移出

```csharp
static TryGetValidViewportSize
```

纯工具方法，与 Shell 无状态交互。可移入 Shell/Viewport/ 工具类，约 -20 行。

---

## 6. 可继续拆分清单

| 优先级 | 职责 | 建议目标 | 预计减少 | 难度 |
|--------|------|----------|----------|------|
| P1 | Raw 输入转发（7 个 HandleRaw* + 场景工具） | Shell/Input/EditorShellRawInputRoute.cs | -38 | 低 |
| P1 | ExecuteViewportFrameSelected | Shell/Viewport/EditorShellViewportFocusRoute.cs | -22 | 低 |
| P1 | 空删除（ExecuteTransformApply 等） | 直接删除或短路 | -9 | 极低 |
| P2 | HandleViewportFocused | Shell/Viewport/EditorShellViewportFocusRoute.cs | -8 | 低 |
| P2 | Scene3D 命令（HandleScene3dRunRequested + Restart + ApplyResult） | Shell/Scene3D/EditorShellScene3dCommandRoute.cs | -16 | 中 |
| P2 | ShowWorldEntitySelection | 移入 Shell/Selection 已有路由 | -8 | 低 |
| P2 | Transform 管线（InitTransformApplication + BuildTransformStartSnapshot + ApplyPreviewPosition + CancelActiveTransform） | Shell/Transform/Pipeline/ | -37 | 高 |
| P2 | TryGetValidViewportSize | Shell/Viewport/ 工具类 | -20 | 低 |
| P3 | OnProjectContentSelected | Shell/Project/EditorShellProjectLoadRoute.cs | -5 | 低 |
| P3 | AppendInfoLog / Warn / Error | Shell/Feedback/EditorShellFeedbackRoute.cs | -3 | 低 |
| P3 | RefreshDiagnostics + _renderLastMode | 随 Diagnostics 移出 | -4 | 中 |

---

## 7. 高风险保留清单

| 代码 | 风险原因 |
|------|----------|
| RouteBuild.RouteSet 装配（30 个字段，~10 行赋值） | 顺序依赖，牵一发动全身 |
| BuildInputRequest | 30+ 参数管家方法，10+ Shell 回调 |
| 构造函数中的 DiagnosticsRoute.SetContext | 涉及 10+ Route 引用 |
| SubscribePanelEvents | 事件连线，Shell 总控 |
| InitTransformApplication | 创建 Transform Application 层实例，生命周期敏感 |
| ApplyScene3dCommandResult | 被多路径引用（Scene3dRun + StartupProbe 回调） |
| ScheduleScene3dFrame | 被 ~15 处引用，Shell 级调度节点 |
| CompleteGroundPlacement | 跨 H-2 路由调用链（PickingRoute + GroundPlacementRoute + TransformRoute） |

---

## 8. 推荐后续阶段

### H-4A：P1 低风险清理（~80 行预估）

| 提取 | 行数 |
|------|------|
| 删除空方法 ExecuteTransformApply | -4 |
| 提取 Raw 输入转发 + 场景工具 → Shell/Input/EditorShellRawInputRoute.cs | -38 |
| 提取 ExecuteViewportFrameSelected → Shell/Viewport/EditorShellViewportFocusRoute.cs | -22 |
| 提取 TryGetValidViewportSize → Shell/Viewport/ViewportUtility.cs | -20 |

**H-4A 后 Shell 预计：~487 行**

### H-4B：P2 中等风险提取（~70 行预估）

| 提取 | 行数 |
|------|------|
| HandleViewportFocused → Shell/Viewport/EditorShellViewportFocusRoute.cs | -8 |
| ShowWorldEntitySelection → Shell/Selection/EditorShellSelectionSyncRoute.cs | -8 |
| Scene3D 命令（RunRequested + Restart + ApplyResult + BuildScene3dCommandRequest）→ Shell/Scene3D/EditorShellScene3dCommandRoute.cs | -20 |
| OnProjectContentSelected → Shell/Project/EditorShellProjectLoadRoute.cs | -5 |
| AppendInfoLog/Warn/Error → Shell/Feedback/EditorShellFeedbackRoute.cs | -3 |
| RefreshDiagnostics + UpdateVulkanViewportHost → Shell/Diagnostics/EditorShellDiagnosticsRoute.cs | -2 |

**H-4B 后 Shell 预计：~417 行**

### H-4C：P3 高风险提取（~50 行预估，可能放弃）

| 提取 | 行数 |
|------|------|
| Transform 管线（InitTransformApplication + BuildTransformStartSnapshot + ApplyPreviewPosition + CancelActiveTransform）→ Shell/Transform/Pipeline/ | -37 |
| 残余 | -13 |

**H-4C 后 Shell 预计：~367 行**

---

## 9. EditorShell 能否最终 ≤100 行？

**不能。**

即使完成 H-4A + H-4B + H-4C（共 ~200 行提取），Shell 仍然保留约 **367 行**，远超 100 行红线。

不可削减的核心代码包括：

| 类别 | 预估行数 |
|------|----------|
| 构造函数（面板查找 + RouteBuild + 路由创建 + Subscribe + 初始化） | ~90 |
| SubscribePanelEvents | ~40 |
| 字段声明（UI 控件 + Route 引用 + 状态） | ~80 |
| 生命周期（Attach/Detach） | ~33 |
| BuildInputRequest | ~10 |
| ScheduleScene3dFrame + CompleteGroundPlacement + 残留薄层 | ~40 |
| Overlay/Picking/Transform 薄转发 | ~30 |
| 面板操作方法（ShowWorldEntitySelection 等 H-4B 后残留） | ~10 |
| 跨路由回调链闭包 | ~34 |
| **合计** | **~367** |

**结论：EditorShell 作为组合根 + 事件总线 + 编排核心，其最小合理边界约为 350-400 行。100 行红线在此文件上不现实。**

白名单必须保持，直到项目结束或 EditorShell 重构策略从"拆分"转为"重写"。

---

## 10. 下一步明确建议

### 建议：执行 H-4A

H-4A 是收益最高、风险最低的残余拆分：

1. **删除 ExecuteTransformApply** — 空方法体，无调用者 ✅
2. **提取 Raw 输入转发** — 纯机械的 7:1 转发，约 -38 行
3. **提取 ExecuteViewportFrameSelected** — 逻辑独立，约 -22 行
4. **提取 TryGetValidViewportSize** — 纯静态工具，约 -20 行

H-4A 后 EditorShell 预计降至 **~487 行**。

H-4B / H-4C 可后续评估收益与风险。H-4A 本身已完成约 80 行提取。

---

## 11. 结论

| 维度 | 结论 |
|------|------|
| **当前行数** | 656 行（工具计数 567） |
| **剩余职责** | ~10 类 |
| **最大风险** | Scene3D 命令 + Transform 管线生命周期 |
| **建议继续拆** | **是** — H-4A（P1，~80 行，低风险） |
| **Shell 能否 ≤100** | **不能** — 最小合理边界 ~350-400 行 |
| **白名单** | **必须保留** — Shell 不可能 ≤100 行 |
| **下一刀推荐** | **H-4A** — Raw 输入 + Frame Selected + 空删除 + 工具方法 |
