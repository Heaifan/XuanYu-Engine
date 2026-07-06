# 9.1A-3 审计：EditorShellV2 接入 Picking + MoveGizmo 输入路由

审计日期：2026/06/26  
审计目标：补齐 V2 Viewport 的 Picking 与 MoveGizmo 输入路由。不接 Inspector/StatusBar/ProjectTree。  
范围限制：不恢复 stash、不修改旧 Shell、不修改 Vulkan/Gizmo/Transform、不刷新重 UI。

---

## 一、新增/修改文件清单

| 文件 | 行数 | 职责 | 类型 |
|---|---|---|---|
| `ShellV2/Composition/Input/EditorShellV2PickingWiring.cs` | 42 | Picking 接线：ViewportPickRoute → Selection | **新增** |
| `ShellV2/Composition/Input/EditorShellV2SceneToolWiring.cs` | 75 | SceneTool 接线：Gizmo press/drag/release | **新增** |
| `ShellV2/Composition/EditorShellV2InputWiring.cs` | 71 | 迁移 stub → 专用文件，保留导航+键盘 | **修改** |
| `ShellV2/Composition/EditorShellV2Composition.cs` | 99 | +PickingWiring.Wire + SceneToolWiring.Wire | **修改** |

### 行数检查

| 文件 | 行数 | 红线 |
|---|---|---|
| `EditorShellV2Composition.cs` | 99 | ≤100 ✅（未突破） |
| `EditorShellV2InputWiring.cs` | 71 | ≤100 ✅ |
| `EditorShellV2PickingWiring.cs` | 42 | ≤100 ✅ |
| `EditorShellV2SceneToolWiring.cs` | 75 | ≤100 ✅ |
| `EditorShellV2InputState.cs` | 18 | ≤100 ✅ |

---

## 二、V2 Picking 接线图

```
PickRequested(x, y)
  → 检查 Session Active + Camera Snapshot
  → ViewportPointerPickRoute.Pick(req)
  → if ViewportPickKind.Entity
      → ctx.Selection.State.Select(WorldEntityInfo(entityId, name, null))
  → else
      → ctx.Selection.State.Select(null)
  → 不刷新 Inspector
  → 不刷新 Diagnostics
  → 不刷新 PickSnapshot
```

**不依赖 `EditorShellPickingRoute` 或 `EditorPickInputRoute`**（避免 12 参数构造函数耦合）。直接使用 `ViewportPointerPickRoute` + `EditorSelectionRoute.State.Select()`。

---

## 三、V2 MoveGizmo 接线图

```
场景工具按下（鼠标左键在 Gizmo 轴上）
  → SceneToolPointerPressed(x, y)
  → EditorSceneToolInputRoute.HandlePressed(x, y, pointer, selection, lifecycle, ...)
  → 内部调用:
      1. UpdateGizmoHover → MoveGizmoHitTest
      2. OnPointerPressed → _gizmo.TryBeginDrag + _dragRoute.Begin
  → return BeginDrag → NativeHost 仲裁层捕获鼠标（9.0X 链路）
  → 无需 V2 手动 SetCapture

鼠标移动（拖动中）
  → RawPointerMoved → ctx.Pointer.OnPointerMoved(x, y) [已由 InputWiring 处理]
  → _dragRoute.Move → AxisDragAnchorBuilder.Build → AxisTranslationSolver.Solve
  → Preview 刷新（仅 Viewport，不刷 Inspector/Diagnostics）

鼠标松开
  → SceneToolPointerReleased(x, y)
  → OnPointerReleased → _gizmo.EndDrag + _dragRoute.Confirm
  → schedule(TransformPreview) — 仅 Viewport 刷新

Esc 取消
  → RawKeyDown(0x1B) → (暂由旧 Shell 处理，V2 键盘 Esc 待补)
  → 9.0X 捕获释放路径（WM_CAPTURECHANGED / WM_KILLFOCUS 兜底）
```

---

## 四、重用哪些旧 Route

| Route | 构造 | V2 使用方式 |
|---|---|---|
| `ViewportPointerPickRoute` | `new()` | 直接调用 `Pick(req)` |
| `EditorSelectionRoute` | `new()` | 调用 `State.Select()` 更新选择 |
| `EditorSceneToolInputRoute` | `new()` | 调用 `HandlePressed/Released` |
| `TransformPointerRoute` | `new()` | `OnPointerPressed/Moved/Released` |
| `Scene3dSessionLifecycle` | `new()` | Snapshot/PresentedGizmo 读取 |
| `EditorSelectionRoute` | `new()` | 选中实体状态 |

**V2 不使用的旧 Shell 路由：**
- `EditorShellPickingRoute`（12 参构造，含 Inspector/Diagnostics 依赖）
- `EditorPickInputRoute.Pick()`（含 refreshDiagnostics 调用）
- `EditorShellSelectionSyncRoute`（Shell 级选择同步）

---

## 五、Preview / Commit 分离确认

| 行为 | V2 状态 |
|---|---|
| Gizmo drag Preview 刷新 Inspector | ❌ 不刷新 |
| Gizmo drag Preview 刷新 Diagnostics | ❌ 不刷新 |
| Gizmo drag Preview 刷新 PickSnapshot | ❌ 不刷新 |
| Gizmo drag Preview 仅 Viewport 渲染 | ✅ `schedule(TransformPreview)` |
| Commit 后写 WorldState | ❌ V2 暂不写 WorldState |
| Commit 后仅更新渲染预览 | ✅ `schedule(TransformPreview)` |
| Esc Cancel 路径 | ✅ 依赖 9.0X CaptureChanged/KillFocus 兜底 |

---

## 六、旧 Shell 是否仍可运行

> **✅ 可运行。默认路径不受影响。**

- `MainWindow.axaml` → 旧 `EditorShell`：未修改
- `App.axaml.cs` → 默认 `MainWindow`：未修改
- `Shell/EditorShell.axaml` → 布局未修改
- `Shell/Composition/` → 路由未修改

---

## 七、风险分析

| 优先级 | 风险 | 说明 |
|---|---|---|
| **P1** | V2 无 WorldState → Gizmo 初始位置为 `Vector3d.Zero` | `BuildTransformSnapshot` 使用零位置。不影响 Gizmo 渲染位置（来自 FrameSource），但拖动偏移从零开始计算。待 9.1A-4/5 接入 WorldState 后修复。 |
| **P2** | V2 键盘 Esc 尚未直接 wiring | V2 的 RawKeyDown 未处理 Esc（0x1B）。当前依赖 9.0X WM_KILLFOCUS/WM_CAPTURECHANGED 兜底释放。9.1A-4 补全键盘路由。 |
| **P2** | Navigation overlay 仍为 NotHandled | V2 没有 Overlay 导航 UI，中键旋转已通过 RawPointer 链路工作。 |

---

## 八、Build / Test 结果

```bash
dotnet build XuanYu.Engine.sln
# Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test XuanYu.Engine.Tests
# Failed: 1, Passed: 723, Skipped: 0, Total: 724
# 失败项：WorldHierarchyTreeBuilderTests（已有，与本次无关）
```

### 文件结构

```
ShellV2/Composition/
├── EditorShellV2Context.cs              (61)
├── EditorShellV2Composition.cs          (99 ✅ 未突破)
├── EditorShellV2Routes.cs               (40)
├── EditorShellV2InputWiring.cs          (71)
└── Input/
    ├── EditorShellV2InputState.cs       (18)
    ├── EditorShellV2PickingWiring.cs    (42 ✅ 新增)
    └── EditorShellV2SceneToolWiring.cs  (75 ✅ 新增)
```

`Input/` 目录 3 个文件（≤5 ✅）。

---

## 九、下一步建议

| 阶段 | 内容 |
|---|---|
| **9.1A-4** | 轻量 StatusBar + 键盘 Esc 路由 + WorldState 引用 |
| 9.1A-5 | Inspector（Commit/Selection 后低频刷新） |

---

## 十、禁止项确认

- [x] 未恢复 stash
- [x] 未修改旧 Shell
- [x] 未修改 Vulkan / Gizmo 数学 / Transform 逻辑
- [x] 未接 Inspector / StatusBar / ProjectTree / Diagnostics / Console
- [x] Preview 不刷新重 UI
- [x] `EditorShellV2Composition.cs` 99 行，未突破 100
- [x] `Input/` 目录 ≤5 文件（3）
- [x] file-tree.md 已更新
