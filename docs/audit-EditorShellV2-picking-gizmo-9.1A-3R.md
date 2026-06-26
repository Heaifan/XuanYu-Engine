# 9.1A-3R 审计：补齐 MoveGizmo drag preview 与 Esc Cancel

审计日期：2026/06/26  
审计目标：修复 9.1A-3 两个缺口：① Gizmo drag preview 未接入 RawPointerMoved ② Esc Cancel 未接入 V2 键盘路由。  
范围限制：不恢复 stash、不修改旧 Shell、不修改 Vulkan/Gizmo/Transform、不接面板。

---

## 一、9.1A-3 的两个缺口

| # | 问题 | 严重度 | 根因 | 修复 |
|---|---|---|---|---|
| 1 | **drag preview 未接**：RawPointerMoved 不调用 ctx.Pointer.OnPointerMoved | **P0** | 9.1A-3 只接了 press/release，漏了 drag 实时预览 | `RawPointerMoved` 中 `IsDragActive` 优先 → `OnPointerMoved` → `Schedule(TransformPreview)` |
| 2 | **Esc Cancel 未接**：V2 RawKeyDown 不处理 0x1B，仅依赖 KillFocus 异常兜底 | **P1** | 9.1A-3 键盘路由预留未补 | `RawKeyDown(0x1B)` → `pointer.Cancel(Escape)` + `panel.RequestCancelToolCapture()` |

---

## 二、修复后的接线

### 2.1 RawPointerMoved — 优先级链

```
RawPointerMoved(x, y)
  ├── if ctx.Pointer.IsDragActive                    ← 优先级 1：Gizmo drag preview
  │     → ctx.Pointer.OnPointerMoved(x, y)
  │     → if Previewed → schedule(TransformPreview)
  │     → return（不走 Orbit/Pan）
  │
  └── else（相机导航）
        → s.OnMove(x, y)
        → if IsOrbiting → ApplyOrbit
        → if IsPanning  → ApplyPan
```

Gizmo drag active 时相机导航不执行，避免拖动中误触相机旋转。

### 2.2 Esc Cancel — V2 键盘路由

```
RawKeyDown(0x1B)
  └── if ctx.Pointer.IsDragActive
        → ctx.Pointer.Cancel(TransformInteractionReason.Escape)
        → schedule(TransformPreview)         ← 视觉恢复
        → panel.RequestCancelToolCapture()   ← 9.0X 路径：ToolCapture.ClearState + Win32 ReleaseCapture
        → return（handled）
```

不触发 Commit / SceneToolPointerReleased / Inspector 刷新。

---

## 三、修改文件

| 文件 | 行数 | 变更 |
|---|---|---|
| `EditorShellV2InputWiring.cs` | 87 | RawPointerMoved 增加 Gizmo preview 优先级 + RawKeyDown 增加 Esc Cancel |
| `docs/audit-EditorShellV2-picking-gizmo-9.1A-3R.md` | 新 | 审计文档 |

### 行数检查

| 文件 | 行数 | 红线 |
|---|---|---|
| `EditorShellV2InputWiring.cs` | 87 | ≤100 ✅（从 71→87，+16行） |
| `EditorShellV2Composition.cs` | 99 | ≤100 ✅（未涨） |

---

## 四、验收确认

| 验收项 | 结果 | 证据 |
|---|---|---|
| Gizmo drag active 时 RawPointerMoved 进入 OnPointerMoved | ✅ 代码第 29-33 行：`if (ctx.Pointer.IsDragActive)` 优先分支 | `InputWiring.cs:29-33` |
| Preview 只刷新 Viewport | ✅ `schedule(TransformPreview)` 不触发诊断 | `InputWiring.cs:32` |
| Gizmo drag 时不走 Orbit/Pan | ✅ `IsDragActive` 分支 `return` | `InputWiring.cs:34` |
| Esc Cancel 主动可用 | ✅ `RawKeyDown(0x1B)` + `pointer.Cancel(Escape)` | `InputWiring.cs:45-49` |
| Esc Cancel 不触发 Commit | ✅ 不调 OnPointerReleased | ✅ |
| Esc Cancel 释放 Win32 Capture | ✅ `panel.RequestCancelToolCapture()` | `InputWiring.cs:48` |
| 中键 Orbit/Shift+中键 Pan 不受影响 | ✅ `IsDragActive` 非 true 时才进入导航分支 | ✅ |
| Build | ✅ 0 error, 0 warning | ✅ |
| Tests | ✅ 723/724（仅已有排序失败） | ✅ |

---

## 五、Build / Test 结果

```bash
dotnet build XuanYu.Engine.sln
# Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test XuanYu.Engine.Tests
# Failed: 1, Passed: 723, Skipped: 0, Total: 724
# 失败项：WorldHierarchyTreeBuilderTests（已有，与本次无关）
```

---

## 六、结论

> **✅ 两个缺口已闭环。9.1A-3 可验收。可进入 9.1A-4。**

### 下一步建议

| 阶段 | 内容 |
|---|---|
| **9.1A-4** | 轻量 StatusBar + WorldState 引用 + 键盘路由完善 |

---

## 七、禁止项确认

- [x] 未恢复 stash
- [x] 未修改旧 Shell
- [x] 未修改 Vulkan / Gizmo 数学 / Transform 逻辑
- [x] 未接 Inspector / StatusBar / ProjectTree / Diagnostics / Console
- [x] Preview 不刷新重 UI
- [x] `EditorShellV2Composition.cs` 未突破 100 行（99）
