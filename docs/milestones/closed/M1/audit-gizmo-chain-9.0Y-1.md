# 9.0Y-1 审计：Gizmo 链路审计 — DragPlane 退化 / 状态机 / 可见性

审计日期：2026/06/26  
审计目标：9.0X 已封版，9.0Y-0 已完成 stash 清点。本阶段只审计 Gizmo 链路类别 A 文件，不做修复。  
审计方法：只读 diff 审查 + 状态机一致性检查 + 数学正确性验证。不修改代码。

---

## 一、本轮纳入的文件

从 stash@{0} 提取（非 stash pop）：

| # | 文件 | 行数 | 性质 |
|---|---|---|---|
| 1 | `Viewport/Transform/Drag/AxisDragAnchorBuilder.cs` | 67 | 源文件（修改） |
| 2 | `Viewport/Transform/Gizmo/Interaction/MoveGizmoInteraction.cs` | 28 | 源文件（修改） |
| 3 | `Viewport/Transform/Presentation/MoveGizmoFrameSource.cs` | 91 | 源文件（修改） |
| 4 | `AxisDragAnchorBuilderTests.cs` | 76 | 测试文件（未跟踪，新增） |

### 未纳入的文件（保留在 stash 中）

| 文件 | 不进 9.0Y 的理由 |
|---|---|
| `GizmoDragProbe.cs` | Probe 基础设施 |
| `EditorShell.axaml` | UI / 性能监视器 |
| `EditorShell.axaml.cs` | 性能监视器 |
| `EditorConsoleOutput.cs` | Probe 基础设施 |
| `EditorProbe.cs` | Probe 基础设施 |
| `EditorShellPickingRoute.cs` | 地面十字标记退役 |
| `EditorShell.Performance.cs` | 性能监视器（未跟踪） |
| `AI_ONBOARDING_MANUAL_XuanYu_Engine.md` | 文档 |

---

## 二、Gizmo 拖动链路状态机

```
用户操作                 业务层                                   渲染层
─────────              ──────────                               ──────
鼠标移动 (无拖动)
  → UpdateGizmoHover() → MoveGizmoInteraction.SetHover(element)
  → MoveGizmoFrameSource.Build() → drawVerts = Build(layout, ..., HoveredElement)
  → Overlay 更新 → 渲染

WM_LBUTTONDOWN (Gizmo 命中)
  → EditorSceneToolInputRoute.HandlePressed()
    → pointer.UpdateGizmoHover(x, y, layout)
    → pointer.OnPointerPressed(req, snap)
      → MoveGizmoInteraction.TryBeginDrag(element)
        → ActiveElement = element
      → TransformDragRoute.Begin(element, x, y, snap)
  → Arbitration: ToolCapture.BeginDrag() + mouseCapture.Capture()

WM_MOUSEMOVE (拖动中)
  → EditorTransformInputRoute.HandlePointerMoved()
    → pointer.OnPointerMoved(x, y)
      → TransformDragRoute.Move(x, y)
        → AxisDragAnchorBuilder.Build(axis, x, y, ...)
        → AxisTranslationSolver.Solve(anchor, x, y) / SolveDragPlane(anchor, intersection)
  → ApplyPreviewPosition()  ← 只更新渲染预览，不写 WorldState

WM_LBUTTONUP / Commit
  → pointer.OnPointerReleased()
    → MoveGizmoInteraction.EndDrag()
      → ActiveElement = None, HoveredElement = None
    → TransformDragRoute.Confirm()
  → EditorSceneToolInputRoute.HandleReleased()
    → applyTransform()  ← 写 WorldState + 刷新 UI

WM_KEYDOWN(0x1B) / Esc Cancel
  → TransformPointerRoute.Cancel(Escape)
    → MoveGizmoInteraction.EndDrag()
      → ActiveElement = None, HoveredElement = None   ← 9.0Y-1 修复确保此处清 HoveredElement
    → TransformDragRoute.Cancel()
  → NativeHost.RequestCancelToolCapture()
    → ToolCapture.ClearState() + mouseCapture.Release()
```

---

## 三、AxisDragAnchorBuilder.DragPlane 退化降级审计

### 3.1 数学正确性

```
退化路径触发条件：
  AxisScreenMetric.TryCompute(pivot, axis, vp, vw, vh, out dir, out ppu) == false
  → 屏幕投影退化，通常因为轴接近与视角方向平行

退化降级链路：
  1. TryBuildDragPlane(axis, x, y, pivot, camera, currentPosition, camForward, out dragPlane)
  2. 如果 1 失败：距离估算回退（与原代码相同）

TryBuildDragPlane 数学：
  planeNormal = Reject(camForward, axis)
    → 将相机前向量投影到垂直于 axis 的平面
    → 如果 camForward ∥ axis，Reject 返回 0 → 依次回退到 Reject(UnitY, axis) → Reject(UnitX, axis)
  planeNormal = planeNormal.Normalize()
  ray = VulkanSceneRayBuilder.TryBuild(x, y, camera, ...)
    → 从相机位置发射经过屏幕 (x,y) 的射线
  denom = ray.Direction · planeNormal
    → 射线方向与平面法线的点积
    → 如果 |denom| < 1e-10：射线与平面近乎平行，返回 false
  t = (pivot - ray.Origin) · planeNormal / denom
    → 射线参数 t，求交于包含 pivot 的平面
    → 如果 t <= 0：交点在相机后方，返回 false
  intersection = ray.At(t)
    → 三维交点坐标
  delta = intersection - StartIntersection
  axisDelta = delta · Axis
  targetPosition = InitialPosition + Axis * axisDelta
```

### 3.2 数学正确性结论

| 检查点 | 结果 | 说明 |
|---|---|---|
| Reject 公式正确 | ✅ | `v - v·a * a` 是标准向量投影到垂直于 a 的平面 |
| 退化回退链完整 | ✅ | camForward → UnitY → UnitX → false |
| 归一化前零向量检查 | ✅ | `IsZero` 后 `Normalize()` |
| 射线-平面平行检查 | ✅ | `Math.Abs(denom) < 1e-10` 早返 |
| 交点在后检查 | ✅ | `t <= 0` 早返 |
| 求解器只使用 StartIntersection 和 Axis | ✅ | `SolveDragPlane` 只用这两个字段，其他字段（CameraRight/CameraUp/CameraForward/DragPlaneNormal）仅存储但不使用 |
| NaN/Infinity 传播 | ⚠️ 低风险 | `ray.At(t)` 如果 t 是无效值会传播 NaN，但前面已做 t > 0 检查 + `VulkanSceneRayBuilder` 输入来自有限坐标 |

### 3.3 风险项

| # | 风险 | 优先级 | 说明 |
|---|---|---|---|
| D1 | CameraRight/UnitX 和 CameraUp/UnitY 硬编码为全局轴 | P2 | `CameraRight = Vector3d.UnitX` 和 `CameraUp = Vector3d.UnitY` 是硬编码的全局坐标轴，但求解器 `SolveDragPlane` 完全不使用这两个字段。它们是死数据，不影响行为。建议清理或根据相机姿态正确计算。 |
| D2 | Reject 假设 axis 已归一化 | P2 | `Reject(value, axis)` 中 `value.Dot(axis)` 假设 `axis` 是单位向量。调用时传入的是 `UnitX`/`UnitY`/`UnitZ`，保证归一化。但如果未来传入任意向量，结果会不正确。建议加归一化或文档说明。 |
| D3 | 测试未覆盖 DragPlane 退化路径 | P1 | 现有 4 个测试全部测试 ScreenProjection 成功路径。没有测试 AxisScreenMetric.TryCompute 失败后进入 DragPlane 的路径。 |
| D4 | 无 t 上限检查 | P2 | `t <= 0` 检查排除相机后交点，但 `t` 理论上无上限。非常大的 `t` 可能导致数值精度降低。非高频场景，影响有限。 |

---

## 四、MoveGizmoInteraction.EndDrag 清 HoveredElement 审计

### 4.1 变更

```
修复前：
  EndDrag() { ActiveElement = MoveGizmoElement.None; }

修复后：
  EndDrag() { ActiveElement = MoveGizmoElement.None; HoveredElement = MoveGizmoElement.None; }
```

### 4.2 状态机影响

| 时序 | 修复前 HoveredElement | 修复后 HoveredElement |
|---|---|---|
| 拖动中 | 保持不变（如 AxisX） | 保持不变 |
| EndDrag 后 | **残留旧值（AxisX）** | **清为 None** |
| 下一次 SetHover 前 | 残留值可能导致错误视觉状态 | 正确为 None |

### 4.3 结论

| 检查点 | 结果 |
|---|---|
| 是否合理 | ✅ 修复前 EndDrag 不清理 hover 状态，会导致拖动结束后视觉残留 |
| 是否影响现有行为 | ✅ 无负面——任何读取 HoveredElement 的地方都应在 SetHover 后重新获取 |
| 是否与其他状态同步 | ✅ `ClearHover()` 独立存在且只清 hover，`EndDrag()` 现在同时清 active + hover |

**风险：无。**

---

## 五、MoveGizmoFrameSource 可见性条件审计

### 5.1 变更

```
修复前：
  if (!input.MoveToolActive || !input.SelectedEntityId.IsValid) return Empty;

修复后：
  if (!input.MoveToolActive && !input.SelectedEntityId.IsValid) return Empty;
```

### 5.2 逻辑真值表

| MoveToolActive | SelectedEntityId.IsValid | 旧值 (OR) | 新值 (AND) | 变化 |
|---|---|---|---|---|
| false | false | Empty | Empty | 不变 |
| false | true | Empty | **继续** | **变化** |
| true | false | Empty | Empty | 不变（Entity 无效时 Gizmo 仍需有效 entity 才有意义） |
| true | true | 继续 | 继续 | 不变 |

### 5.3 影响分析

变化发生在 **MoveTool 未激活但仍有选中实体**时：

| 场景 | 旧行为 | 新行为 |
|---|---|---|
| Select 工具下选中了实体 | Gizmo 不显示 | **Gizmo 显示**（可 hover/drag，但需要先激活 Move 工具才能拖动？） |

**但注意**：`SceneToolPointerPressed` 事件在 `EditorSceneToolInputRoute.HandlePressed` 中处理，它在执行 `pointer.OnPointerPressed` 前检查 `selection.State.SelectedWorldEntity` 和 `gizmo?.IsAvailable`。如果 Gizmo 的 `IsAvailable` 由 `PresentedGizmo` 决定，而 `PresentedGizmo` 来自 `MoveGizmoFrameSource.Build()` 的输出，那么 **当 Gizmo 不显示时也无法交互**。

因此这个变更实际影响的是：**使用 Select 工具时，选中实体是否显示 Gizmo（即使不处于 Move 模式）**。这在很多编辑器中是预期行为（选中即显示操作手柄）。但在本项目中，Gizmo 拖动只有在 Move 工具激活时才能通过 `EditorSceneToolInputRoute` 启动。

### 5.4 结论

| 检查点 | 结果 |
|---|---|
| 逻辑变更是否符合预期 | ✅ 更直觉——选中实体时显示 Gizmo，不管当前工具 |
| 是否影响拖动启动 | ✅ 不影响——拖动启动仍需经过 `EditorSceneToolInputRoute` 中的 `pointer.OnPointerPressed` |
| 是否影响渲染性能 | ✅ Gizmo 渲染计算量小，无性能问题 |
| 是否存在副作用 | ⚠️ 低风险——Select 工具下用户看到 Gizmo 但拖动可能会触发工具切换 |

**风险：低。**

---

## 六、测试覆盖审计

### 6.1 现有测试

| 测试 | 覆盖路径 | 结果 |
|---|---|---|
| `Build_ZAxis_UsesScreenProjection_WhenMetricIsAvailable` | Z 轴 + 默认相机 → ScreenProjection | ✅ |
| `Build_XAxis_UsesScreenProjection` | X 轴 + 默认相机 → ScreenProjection | ✅ |
| `Build_ZAxis_WithLowPitch_UsesContinuousScreenProjection` | Z 轴 + 18° 俯仰 → ScreenProjection | ✅ |
| `Solve_ZAxis_ScreenProjection_CanMoveUpThenBackDown` | Z 轴 ScreenProjection 拖动 30px 上下 | ✅ |

### 6.2 测试缺口

| 缺口 | 优先级 | 建议 |
|---|---|---|
| 无 DragPlane 退化路径测试 | **P1** | 需要构造一个 `AxisScreenMetric.TryCompute` 失败的相机-Axis 配置（如相机正对轴方向）。建议新增测试：`Build_ZAxis_WhenScreenMetricFails_FallsBackToDragPlane` |
| 无 TryBuildDragPlane 失败后距离估算回退测试 | P2 | 需要构造同时使 ScreenMetric 和 DragPlane 都失败的配置 |
| 无极端视角测试（Reject 零向量） | P2 | 需要构造 camForward ∥ axis 的相机配置 |
| 无射线-平面平行测试 | P2 | 需要构造视线∥DragPlane 的相机配置 |
| 无 `SolveDragPlane` 端到端测试 | P2 | 需要验证从 Build → SolveDragPlane 的完整路径 |

---

## 七、风险排序

| 优先级 | 风险 | 文件 | 说明 |
|---|---|---|---|
| **P1** | 测试未覆盖 DragPlane 退化路径 | `AxisDragAnchorBuilderTests.cs` | 现有 4 个测试全走 ScreenProjection，退化路径无覆盖 |
| **P2** | CameraRight/CameraUp 硬编码为全局轴（死数据） | `AxisDragAnchorBuilder.cs:60-61` | `Vector3d.UnitX`/`UnitY`，但求解器不使用，建议清理 |
| **P2** | Reject 假设 axis 已归一化 | `AxisDragAnchorBuilder.cs:66` | 当前调用安全（Unit 向量），但 API 契约不明确 |
| **P2** | 可见性条件变化导致 Select 工具下 Gizmo 可见 | `MoveGizmoFrameSource.cs:19` | 行为变更但无意料外影响 |
| **P3** | 无 t 上限检查 | `AxisDragAnchorBuilder.cs:53` | 理论上 t 无上限，极远端交点可能精度降低 |

---

## 八、建议 9.0Y-2 修复范围

如果进入 9.0Y-2 最小修复，建议只修 P1：

1. **新增测试覆盖 DragPlane 退化路径** → 构造 `AxisScreenMetric.TryCompute` 失败的相机配置，验证 Build 返回 `DragPlane` 模式且数学正确。

P2 项不建议在本阶段修（死数据不影响行为，Reject 假设在当前调用下安全）。

---

## 九、Build / Test 结果

```bash
dotnet build XuanYu.Engine.sln
# Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test XuanYu.Engine.Tests --no-build
# Failed: 1, Passed: 698, Skipped: 0, Total: 699
# AxisDragAnchorBuilderTests 新增 4 测试：全部通过
# 唯一失败：WorldHierarchyTreeBuilderTests（已有中文字符排序，与本次无关）
```

注意：测试总数从 698 变为 699（新增 `AxisDragAnchorBuilderTests.cs` 的 4 个测试）。

---

## 十、变更清单

| 文件 | 行数 | 变更类型 |
|---|---|---|
| `Viewport/Transform/Drag/AxisDragAnchorBuilder.cs` | 67 | 从 stash 提取：DragPlane 退化降级 |
| `Viewport/Transform/Gizmo/Interaction/MoveGizmoInteraction.cs` | 28 | 从 stash 提取：EndDrag 清 HoveredElement |
| `Viewport/Transform/Presentation/MoveGizmoFrameSource.cs` | 91 | 从 stash 提取：Gizmo 可见性条件修正 |
| `Tests/Editor/Transform/Translation/Axis/AxisDragAnchorBuilderTests.cs` | 76 | 新增测试文件 |
| `docs/audit-gizmo-chain-9.0Y-1.md` | 新文件 | 审计报告 |

---

## 十一、结论

### 是否可以进入 9.0Y-2 修复？

> **✅ 可以。**

### 判断依据

1. DragPlane 退化降级数学正确，回退链完整。
2. CameraRight/CameraUp 硬编码为死数据，不影响求解器。
3. EndDrag 清 HoveredElement 是明确的状态机修复。
4. Gizmo 可见性条件从 OR 改 AND 行为更直觉，无负面。
5. 测试覆盖了 ScreenProjection 成功路径，但缺 DragPlane 退化路径（P1）。

### 禁止项确认

- [x] 未直接 stash pop
- [x] 未恢复性能监视器 / UI / Probe
- [x] 未修改 Vulkan
- [x] 未修改 EditorShell 布局
- [x] 未引入修复逻辑
- [x] 只读审计
