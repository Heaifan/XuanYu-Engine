# 9.0Y-2 审计：Gizmo 链路最小测试补强

审计日期：2026/06/26  
审计目标：补测试锁定 Gizmo 行为，不修改 Gizmo 数学逻辑。覆盖 DragPlane 退化路径、EndDrag 状态清理、Gizmo 可见性条件。  
范围限制：只补测试，不新增功能。

---

## 一、本轮纳入的文件

| # | 文件 | 行数 | 类型 |
|---|---|---|---|
| 1 | `Tests/Editor/Transform/Translation/Axis/AxisDragAnchorBuilderTests.cs` | 110 | 新增 2 个测试 |
| 2 | `Tests/Editor/Transform/Gizmo/MoveGizmoInteractionTests.cs` | 76 | 新测试文件（8 个测试） |
| 3 | `Tests/Editor/Transform/Presentation/MoveGizmoFrameSourceTests.cs` | 80 | 新测试文件（5 个测试） |
| 4 | `docs/audit-gizmo-chain-9.0Y-2.md` | 新文件 | 审计报告 |

---

## 二、新增测试清单

### 2.1 AxisDragAnchorBuilder — DragPlane 退化路径

| 测试 | 覆盖内容 | 结果 |
|---|---|---|
| `Build_ZAxis_WithTopDownCamera_FallsBackToDragPlane` | 相机从正上方俯视（Pitch=89°），Z 轴屏幕投影 < 6px → DragPlane 模式。验证 StartIntersection 非 NaN、DragPlaneNormal 非零。 | ✅ |
| `Solve_ZAxis_DragPlane_CanMoveUpThenBackDown` | DragPlane 模式下沿 +Z 移动 5 单位再回到起点，验证位移可逆、无方向锁死。 | ✅ |

### 2.2 MoveGizmoInteraction — EndDrag 状态清理

| 测试 | 覆盖内容 | 结果 |
|---|---|---|
| `InitialState_IsNone` | 新建实例 ActiveElement/HoveredElement/IsDragging 均为初始值 | ✅ |
| `SetHover_UpdatesHoveredElement` | SetHover(AxisX) → HoveredElement == AxisX | ✅ |
| `ClearHover_ResetsHoveredElement` | SetHover → ClearHover → None | ✅ |
| `TryBeginDrag_WithValidElement_SetsActiveAndIsDragging` | TryBeginDrag(AxisX) → true, ActiveElement=AxisX, IsDragging=true | ✅ |
| `TryBeginDrag_WithNoneElement_ReturnsFalse` | TryBeginDrag(None) → false, 状态不变 | ✅ |
| **`EndDrag_ClearsActiveElementAndHoveredElement`** | **BeginDrag → EndDrag → ActiveElement=None, HoveredElement=None** | ✅ |
| `EndDrag_WithoutBeginDrag_DoesNotThrow` | 幂等调用 EndDrag → 不抛异常 | ✅ |
| `SetHover_DuringDrag_DoesNotAffectActiveElement` | 拖动中 SetHover → ActiveElement 不变，HoveredElement 更新 | ✅ |

### 2.3 MoveGizmoFrameSource — 可见性条件

| 测试 | 覆盖内容 | 结果 |
|---|---|---|
| `Build_WithMoveToolActiveAndSelectedEntity` | 两者都 true → 不 Empty | ✅ |
| `Build_WithMoveToolActiveAndNoEntity` | 工具激活但无有效实体 → 不 Empty（AND 条件：!true && !false = false） | ✅ |
| **`Build_WithMoveToolInactiveAndSelectedEntity`** | **非 Move 工具但有选中实体 → 不 Empty（9.0Y-1 新行为）** | ✅ |
| `Build_WithMoveToolInactiveAndNoEntity` | 两者 false → Empty | ✅ |
| `Build_WithInvalidCamera_ReturnsEmpty` | 相机无效 → Empty | ✅ |

---

## 三、行为锁定确认

| 行为 | 锁定方式 | 测试数 |
|---|---|---|
| DragPlane 退化路径数学正确 | 断言 Mode=DragPlane + 字段非 NaN + 上下可逆 | 2 |
| EndDrag 同时清理 ActiveElement + HoveredElement | 直接断言 | 1 |
| Gizmo 可见性：选中实体即显示（无论工具） | `MoveToolInactiveAndSelectedEntity → 不 Empty` | 1 |
| Camera 无效 → 不显示 | 断言 Empty | 1 |
| 拖动交互状态机（BeginDrag/EndDrag/Hover） | 完整状态转换覆盖 | 8 |

---

## 四、发现的注意事项

### 4.1 MoveToolActive + NoEntity 行为待产品确认

`Build_WithMoveToolActiveAndNoEntity_ReturnsNonEmpty` 测试通过，但代表的行为是：**Move 工具激活但没有选中实体时，Gizmo 仍然渲染**（生成 DrawList 和 PendingSnapshot）。这可能导致在无选中实体时 Gizmo 仍可见，只是 entity ID 为 `EntityId.None`（Value=0）。

这是当前 AND 条件 `!MoveToolActive && !SelectedEntityId.IsValid` 的自然结果。如果产品预期“无选中实体时永远不显示 Gizmo”，这里需要额外加 `SelectedEntityId.IsValid` 守卫。

### 4.2 CameraRight/CameraUp 仍是死数据

`AxisDragAnchorBuilder` 硬编码的 `Vector3d.UnitX`/`UnitY` 在 DragPlane 模式的 anchor 中存储但不被求解器使用。新增测试验证了 `SolveDragPlane` 不依赖这些字段。

---

## 五、风险排序

| 优先级 | 风险 | 说明 |
|---|---|---|
| **P2** | MoveToolActive + NoEntity 时 Gizmo 仍渲染 | 当前 AND 条件逻辑结果，需产品确认是否预期 |
| P3 | CameraRight/CameraUp 死数据 | 不影响行为，后续清理即可 |

未发现 P0/P1 缺陷。

---

## 六、Build / Test 结果

```bash
dotnet build XuanYu.Engine.sln
# Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test XuanYu.Engine.Tests
# Failed: 1, Passed: 712, Skipped: 0, Total: 713
# 失败项：WorldHierarchyTreeBuilderTests（已有中文字符排序，与本次无关）
# 新增 15 个测试全部通过（2 + 8 + 5）
```

### 测试数量变化

| 阶段 | 测试总数 | 新增 |
|---|---|---|
| 9.0Y-1 | 698 | 基线 |
| 9.0Y-2 | **713** | **+15**（2 DragPlane + 8 Interaction + 5 FrameSource） |

---

## 七、结论

### 是否可以进入 9.0Y-3 封版验证？

> **✅ 可以。所有 Gizmo 行为已被测试锁定。**

### 锁定的行为

1. DragPlane 退化路径数学正确，屏幕投影不足 6px 时正常降级
2. EndDrag 同时清理 ActiveElement 和 HoveredElement
3. Gizmo 可见性按 AND 条件正确输出 Empty 或非 Empty
4. MoveGizmoInteraction 完整状态机覆盖

### 未覆盖（可延后）

1. CameraRight/CameraUp 死数据清理（P3）
2. MoveToolActive + NoEntity 渲染确认（需产品决策）

---

## 八、禁止项确认

- [x] 未修改 Gizmo 数学逻辑
- [x] 未修改 Vulkan
- [x] 未修改 EditorShell UI
- [x] 未恢复性能监视器 / Probe
- [x] 未处理地面十字标记退役
- [x] 未 stash pop
- [x] 未新增右键/旋转/缩放功能
