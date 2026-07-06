# 9.0Y-0 审计：Gizmo 链路封版前的工作树与 stash 清点

审计日期：2026/06/26  
审计目标：9.0X 已封版。进入 9.0Y 前清点 stash 和工作树，分类所有存量变更，确定哪些可以进入 9.0Y。  
审计方法：只读 diff 审查，不修改代码，不恢复 stash。

---

## 一、当前工作树状态

```bash
git status --short
# ?? XuanYu.Engine.Editor.Windows/Shell/EditorShell.Performance.cs
# ?? XuanYu.Engine.Tests/Editor/Transform/Translation/Axis/AxisDragAnchorBuilderTests.cs
# ?? docs/AI_ONBOARDING_MANUAL_XuanYu_Engine.md
```

未跟踪文件 3 个，无已修改但未提交的文件。工作树本身干净（排除 stash 内容）。

---

## 二、stash 内容总览

只有一个 stash：

```
stash@{0}: 9.0X-2R: 暂存非 9.0X-2 的工作树变更
           （性能监视器/Gizmo/Probe等），待 9.0X-3 验证后恢复
```

包含 9 个文件，+110 / -54：

| # | 文件 | +/− | 分类 |
|---|---|---|---|
| 1 | `Shell/Diagnostics/GizmoDrag/GizmoDragProbe.cs` | +4 | PROBE |
| 2 | `Shell/EditorShell.axaml` | +73/−30 | UI 布局 / 性能监视器 |
| 3 | `Shell/EditorShell.axaml.cs` | +3 | 性能监视器 |
| 4 | `Shell/Feedback/EditorConsoleOutput.cs` | +1 | PROBE |
| 5 | `Shell/Feedback/EditorProbe.cs` | +5 | PROBE |
| 6 | `Shell/Picking/EditorShellPickingRoute.cs` | −10 | 功能移除 |
| 7 | `Viewport/Transform/Drag/AxisDragAnchorBuilder.cs` | +61/−12 | **GIZMO 数学** |
| 8 | `Viewport/Transform/Gizmo/Interaction/MoveGizmoInteraction.cs` | +1 | **GIZMO 行为** |
| 9 | `Viewport/Transform/Presentation/MoveGizmoFrameSource.cs` | +1/−1 | **GIZMO 逻辑** |

---

## 三、未跟踪文件详情

| 文件 | 行数 | 分类 | 说明 |
|---|---|---|---|
| `EditorShell.Performance.cs` | 49 | 性能监视器 | `DispatcherTimer` 驱动，500ms 间隔读取 FPS/CPU%/FrameIndex 并更新 `TopPerfText` |
| `AxisDragAnchorBuilderTests.cs` | 76 | **GIZMO 测试** | 4 个测试用例：Z轴/X轴 ScreenProjection、低仰角连续投影、上下拖动可逆 |
| `AI_ONBOARDING_MANUAL_XuanYu_Engine.md` | 746 | 文档 | 项目 AI 入场手册 |

---

## 四、逐项分类与处置建议

### 类别 A：建议进入 9.0Y（Gizmo 链路直接相关）

| 文件 | 改动摘要 | 进入 9.0Y 建议 |
|---|---|---|
| `AxisDragAnchorBuilder.cs` (+61/−12) | **DragPlane 退化降级**：当 `AxisScreenMetric.TryCompute` 失败时，不再走旧的简单距离估算，而是通过 `TryBuildDragPlane` 用射线-平面求交构造 DragPlane anchor。新增 `Reject` 向量投影辅助方法。新增 `StartIntersection`、`DragPlaneNormal`、`CameraForward/Right/Up` 字段到返回的 anchor。 | **✅ 建议进入 9.0Y** — 这是 9.0Y 应当审查和验证的核心 Gizmo 数学变更 |
| `MoveGizmoInteraction.cs` (+1) | `EndDrag()` 中增加 `HoveredElement = MoveGizmoElement.None`，确保结束拖动时同时清除 hover 状态。 | **✅ 建议进入 9.0Y** — Gizmo 状态机修复 |
| `MoveGizmoFrameSource.cs` (+1/−1) | Gizmo 可见性条件从 `!MoveToolActive || !SelectedEntityId.IsValid`（OR）改为 `!MoveToolActive && !SelectedEntityId.IsValid`（AND）。— 当 MoveTool 未激活但 Entity 有效时，不再隐藏 Gizmo。 | **✅ 建议进入 9.0Y** — Gizmo 渲染逻辑修正 |
| `AxisDragAnchorBuilderTests.cs` (新, 76行) | DragPlane 退化路径的测试：Z轴/X轴 ScreenProjection、低仰角连续投影、上下拖动可逆。 | **✅ 建议进入 9.0Y** — 随 AxisDragAnchorBuilder 一同验证 |

### 类别 B：建议延后或独立处理（不属 Gizmo 链路）

| 文件 | 改动摘要 | 处置建议 |
|---|---|---|
| `EditorShellPickingRoute.cs` (−10) | 地面点击十字标记已退役，`ShowGroundCursor` / `HideGroundCursor` 方法体清空。 | **⏸ 建议作为独立变更单独提交**。与 Gizmo 链路无关，不应混入 9.0Y。可在 9.0Y 后单独处理。 |

### 类别 C：建议本次不进 9.0Y（性能监视器/UI/Probe 基础设施）

| 文件 | 改动摘要 | 不进 9.0Y 的理由 |
|---|---|---|
| `EditorShell.axaml` (+73/−30) | 顶部菜单栏从纯 `StackPanel` 改为 `Grid` 两列布局，右侧新增 `TopPerfText` 显示区域（含边框样式），用于展示 FPS/CPU%/FrameIndex。 | **❌ 不建议进 9.0Y** — UI 布局变更，与 Gizmo 链路无关。如果在 Gizmo 封版期间引入，会污染调试时对渲染卡顿的归因 |
| `EditorShell.axaml.cs` (+3) | `InitPerformanceMonitor()` / `StartPerformanceMonitor()` / `StopPerformanceMonitor()` 三个钩子。 | **❌ 不建议进 9.0Y** — 与 UI 布局变更同上 |
| `EditorShell.Performance.cs` (新, 49行) | 完整的 500ms 定时性能监视器，读 `Process.TotalProcessorTime` 和 `FrameIndex` 计算 FPS/CPU%。 | **❌ 不建议进 9.0Y** — 本身有价值，但应该在 9.0Y 后作为独立特性提交。如果在 Gizmo 封版期间引入，其 DispatcherTimer 和 Process 轮询可能干扰 Gizmo 高频路径 |
| `GizmoDragProbe.cs` (+4) | 增加 `s_enabled` 检查，在 probe 未启用时跳过所有内部逻辑（`BeginFrame` / `EndFrame` / `Log` 都加早返）。 | **❌ 不建议进 9.0Y** — Probe 基础设施优化。应与 EditorProbe.cs 的变更一起作为独立提交 |
| `EditorProbe.cs` (+5) | 增加 `s_enabled` 静态标志（从 `XUANYU_EDITOR_PROBE` 环境变量读取），`Write()` 方法早返，暴露 `IsEnabled` 属性。 | **❌ 不建议进 9.0Y** — 同上 |
| `EditorConsoleOutput.cs` (+1) | `AttachParentConsole` 增加 probe 开关保护。 | **❌ 不建议进 9.0Y** — 同上 |

---

## 五、9.0Y 入口建议范围

基于以上分类，9.0Y 的入口建议只包含：

```
9.0Y 核心范围（Gizmo 链路封版）:
  ├── AxisDragAnchorBuilder.cs       ← DragPlane 退化降级
  ├── MoveGizmoInteraction.cs        ← EndDrag 清 HoveredElement
  ├── MoveGizmoFrameSource.cs        ← Gizmo 可见性条件修正
  └── AxisDragAnchorBuilderTests.cs  ← 配套测试

9.0Y 不应混入:
  ├── EditorShell.axaml / .axaml.cs / .Performance.cs   ← 性能监视器 UI
  ├── GizmoDragProbe.cs / EditorProbe.cs / EditorConsoleOutput.cs  ← Probe 基础设施
  ├── EditorShellPickingRoute.cs    ← 地面十字标记移除
  └── docs/AI_ONBOARDING_MANUAL.md  ← 文档
```

---

## 六、关于 stash 恢复策略

**不建议直接 `git stash pop`。**

理由：

1. stash 包含 9 个文件，其中只有 4 个（类别 A）应进入 9.0Y
2. 直接 pop 会一次性恢复所有变更，重新制造"不知道是谁导致的"问题
3. 建议手动应用（`git checkout stash@{0} -- <file>`）只提取类别 A 的文件，然后独立提交

操作示例：

```bash
# 只提取 Gizmo 相关变更到工作树
git checkout stash@{0} -- XuanYu.Engine.Editor.Windows/Viewport/Transform/Drag/AxisDragAnchorBuilder.cs
git checkout stash@{0} -- XuanYu.Engine.Editor.Windows/Viewport/Transform/Gizmo/Interaction/MoveGizmoInteraction.cs
git checkout stash@{0} -- XuanYu.Engine.Editor.Windows/Viewport/Transform/Presentation/MoveGizmoFrameSource.cs

# Gizmo 测试文件在未跟踪中，可直接测试
```

---

## 七、结论

| 项目 | 结论 |
|---|---|
| 当前工作树是否干净 | ✅ 无已修改未提交文件 |
| stash 中有哪些内容 | 9 个文件：4 Gizmo + 1 功能移除 + 3 Probe + 1 性能监视器 UI |
| 建议进入 9.0Y | `AxisDragAnchorBuilder.cs`、`MoveGizmoInteraction.cs`、`MoveGizmoFrameSource.cs`、`AxisDragAnchorBuilderTests.cs` |
| 不应进入 9.0Y | 性能监视器全套、Probe 全套、地面十字标记移除 |
| 下一步 9.0Y-1 建议范围 | Gizmo 链路审计：DragPlane 退化路径、EndDrag/HoveredElement 状态机、Gizmo 可见性逻辑 |

---

## 八、文件变更

| 文件 | 行数 | 变更类型 |
|---|---|---|
| `docs/audit-gizmo-stash-9.0Y-0.md` | 新文件 | stash 清点报告 |

未修改任何源文件。

---

## 九、禁止项确认

- [x] 没有直接恢复 stash
- [x] 没有源代码改动
- [x] 没有 UI / Vulkan / Gizmo 数学改动
- [x] 9.0Y 的入口边界已明确
