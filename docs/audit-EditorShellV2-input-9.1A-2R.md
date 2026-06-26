# 9.1A-2R 审计：修正 EditorShellV2 输入状态机

审计日期：2026/06/26  
审计目标：修正 9.1A-2 中 Shift 状态误当 Pan 状态的缺口，移除 static 输入状态，修复高频路径触发诊断的问题。  
范围限制：不恢复 stash、不修改旧 Shell、不修改 Vulkan/Gizmo/Transform、不接其他面板。

---

## 一、9.1A-2 的 3 个问题

| # | 问题 | 严重度 | 修复方式 |
|---|---|---|---|
| 1 | **Shift 被当成 Pan**：`_isPanning = !_isOrbiting` 使只按 Shift 移动鼠标就触发平移 | **P0** | 拆分修饰键与导航动作 |
| 2 | **static 输入状态**：`_isOrbiting`/`_isPanning`/`_lastX`/`_lastY` 是 static，多实例冲突 | **P1** | 使用实例状态机 |
| 3 | **FrameScheduler 对相机导航触发 ProbeValidation**：高频路径不应触发诊断 | **P2** | 排除 CameraPan/CameraZoom |

---

## 二、修复后的状态机

### 2.1 `EditorShellV2InputState`（新增）

```
Shift 按下 → IsShiftDown = true
Shift 释放 → IsShiftDown = false

中键按下 → IsMiddleDown = true
          → IsOrbiting  = IsMiddleDown && !IsShiftDown
          → IsPanning   = IsMiddleDown &&  IsShiftDown
中键释放 → IsMiddleDown = false
          → IsOrbiting = false, IsPanning = false

失焦     → IsShiftDown = false, IsMiddleDown = false
```

### 2.2 关键时序

```
时序 1：只按 Shift
  RawKeyDown(Shift) → IsShiftDown=true, IsOrbiting=false, IsPanning=false
  RawPointerMoved    → IsOrbiting=false, IsPanning=false → 不导航 ✅

时序 2：中键旋转
  RawPointerButtonDown(中键) → IsMiddleDown=true, IsShiftDown=false → IsOrbiting=true
  RawPointerMoved             → dx,dy → ApplyOrbit ✅

时序 3：Shift + 中键平移
  RawKeyDown(Shift)          → IsShiftDown=true
  RawPointerButtonDown(中键)  → IsMiddleDown=true, IsShiftDown=true → IsPanning=true
  RawPointerMoved             → dx,dy → ApplyPan ✅

时序 4：中键释放后松开 Shift
  RawPointerButtonUp(中键)    → IsMiddleDown=false → IsPanning=false
  RawKeyUp(Shift)             → IsShiftDown=false

时序 5：失焦
  RawInputFocusLost          → IsShiftDown=false, IsMiddleDown=false → 全清 ✅
```

### 2.3 FrameScheduler 修复

```
修复前：仅排除 TransformPreview
修复后：排除 TransformPreview + CameraPan + CameraZoom
        相机旋转（Orbit）返回 CameraPan → 也被排除
        相机缩放（Zoom）返回 CameraZoom → 也被排除
```

---

## 三、修改/新增文件清单

| 文件 | 行数 | 变更 |
|---|---|---|
| `ShellV2/Composition/Input/EditorShellV2InputState.cs` | 18 | **新增**：实例状态机 |
| `ShellV2/Composition/EditorShellV2InputWiring.cs` | 75 | **修改**：使用实例状态，移除 static |
| `ShellV2/Composition/EditorShellV2Composition.cs` | 98 | **修改**：FrameScheduler 排除高频路径 |
| `Tests/Editor/ShellV2/EditorShellV2InputStateTests.cs` | 138 | **新增**：11 个状态机测试 |
| `docs/audit-EditorShellV2-input-9.1A-2R.md` | 新文件 | 审计文档 |

### 行数检查

| 文件 | 行数 | 红线 |
|---|---|---|
| `EditorShellV2Composition.cs` | 98 | ≤100 ✅（未涨超） |
| `EditorShellV2InputWiring.cs` | 75 | ≤100 ✅（从 82 降到 75） |
| `EditorShellV2InputState.cs` | 18 | ≤100 ✅ |
| `EditorShellV2InputStateTests.cs` | 138 | ≤180（测试上限）✅ |

---

## 四、新增测试清单

| 测试 | 覆盖内容 | 结果 |
|---|---|---|
| `InitialState_AllFalse` | 新建状态全部 false | ✅ |
| `ShiftDown_SetsShift_DoesNotTriggerOrbitOrPan` | 只按 Shift → 不产生导航 | ✅ |
| `MiddleDown_WithoutShift_TriggersOrbit` | 中键 → Orbit | ✅ |
| `MiddleDown_WithShiftDown_TriggersPan` | Shift+中键 → Pan | ✅ |
| `MiddleUp_ClearsOrbitAndPan` | 中键释放 → 清导航 | ✅ |
| `MiddleUp_WithShift_KeepsShiftDown` | 中键释放后 Shift 仍保留 | ✅ |
| `ShiftUp_ClearsShift` | Shift 释放后中键仍按 → 切回 Orbit | ✅ |
| `FocusLost_ClearsAll` | 失焦 → 全清 | ✅ |
| `ShiftOnly_Move_DoesNotTriggerPan` | 只按 Shift 移动 → 不 Pan | ✅ |
| `MiddleDown_RecordsLastPosition` | 中键按下记录位置 | ✅ |
| `Move_UpdatesLastPosition` | 移动更新位置 | ✅ |

所有测试 100% 通过。

---

## 五、验收确认

| 验收项 | 结果 |
|---|---|
| 只按 Shift 移动鼠标不会平移 | ✅ `EditorShellV2InputStateTests.ShiftOnly_Move_DoesNotTriggerPan` |
| 中键拖动可旋转 | ✅ `IsOrbiting = IsMiddleDown && !IsShiftDown` |
| Shift+中键拖动可平移 | ✅ `IsPanning = IsMiddleDown && IsShiftDown` |
| 中键释放不残留导航 | ✅ `OnMiddleUp()` → `IsMiddleDown = false` |
| Shift 释放不残留 Pan | ✅ `OnShiftUp()` → `IsShiftDown = false` |
| 失焦不残留状态 | ✅ `OnFocusLost()` → 全清 |
| 输入状态非 static | ✅ 实例状态机，每个 `Wire(ctx)` 创建新实例 |
| 高频路径不触发诊断 | ✅ FrameScheduler 排除 CameraPan/CameraZoom |
| 不接 Inspector/StatusBar/Picking/Gizmo | ✅ 未接入 |
| Build 0 error | ✅ |
| Tests 结果 | ✅ 723/724（仅已有排序失败） |

---

## 六、Build / Test 结果

```bash
dotnet build XuanYu.Engine.sln
# Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test XuanYu.Engine.Tests
# Failed: 1, Passed: 723, Skipped: 0, Total: 724
# 失败项：WorldHierarchyTreeBuilderTests（已有，与本次无关）
# 新增 11 个状态机测试全部通过
```

### 测试数量变化

| 阶段 | 总数 | 新增 |
|---|---|---|
| 9.1A-2 | 713 | 基线 |
| **9.1A-2R** | **724** | **+11**（输入状态机） |

---

## 七、结论

> **✅ 9.1A-2 输入状态缺口已闭环。可进入 9.1A-3。**

### 下一步建议

| 阶段 | 内容 |
|---|---|
| **9.1A-3** | 接入 Gizmo 完整交互（SceneTool）+ Picking + 轻量状态栏 |

---

## 八、禁止项确认

- [x] 未恢复 stash
- [x] 未修改旧 Shell
- [x] 未修改 Vulkan / Gizmo 数学 / Transform 逻辑
- [x] 未接 Inspector / StatusBar / Picking / Gizmo
- [x] 所有生产文件 ≤100 行
- [x] file-tree.md 已更新
