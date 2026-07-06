# 9.0X-2 审计：Esc 取消 MoveGizmo 后释放 Win32 Capture

审计日期：2026/06/26  
审计目标：修复 9.0X-1 发现的 P0 缺口——Esc 取消 Gizmo 拖动后，Win32 Capture 没有立即释放。  
范围限制：只修此 P0；不碰 Vulkan、UI 布局、Gizmo 几何、HitTest、Transform 数学逻辑。

---

## 一、修复前状态

### 1.1 Esc 取消链路（修复前）

```
Esc 键按下
  → RawKeyDown(0x1B)
  → EditorViewportInputRoute.HandleKeyDown
  → EditorTransformInputRoute.HandleKeyDown
  → TransformKeyboardRoute.HandleKeyDown
      → pointerRoute.Cancel(Escape)         ← 只清业务状态
          → _gizmo.EndDrag()                ← Gizmo 状态清除
          → _dragRoute.Cancel()             ← DragRoute 状态清除
          → 返回 Cancelled + InitialTransform
      ← 回到 HandleKeyDown
  → r.CancelTransform(kr)                   ← 视觉恢复（TransformApplyRoute.Cancel）
  → r.InfoLog("变换已取消")
```

**缺口：** `pointerRoute.Cancel(Escape)` 只清理业务层（Gizmo/DragRoute），**没有释放 Win32 Capture**。NativeHost 的 `ToolCapture` 状态残留，`_mouseCapture` 仍标记为已捕获。

### 1.2 修复前状态机问题

```
MoveGizmo 左键拖动中        Win32 Capture  ==== 持有中 ====
       ↓
用户按 Esc
       ↓
TransformPointerRoute.Cancel  →  业务层 Cancelled
       ↓                              ↓
Win32 Capture  ==== 仍持有 ====   Win32 以为 Viewport 还在捕获
       ↓
下一次 WM_LBUTTONUP 才释放       ← 窗口期状态残留
```

### 1.3 风险

- **窗口期状态残留：** Esc 后到下一次点击之间，Win32 Capture 仍被 Viewport HWND 持有，可能影响其他窗口消息分发。
- **行为异常：** 如果 Esc 后用户切换到其他应用，WM_CAPTURECHANGED 因 KILLFOCUS 才触发，但 KILLFOCUS 处理可能因内部状态已 Cancel 而记录异常日志。

---

## 二、修复方案

### 2.1 核心原则

```
业务取消归业务层，Win32 捕获闭环归 NativeHost 层。
```

- `TransformPointerRoute.Cancel` 继续只负责 Gizmo.EndDrag + DragRoute.Cancel
- NativeHost 层新增独立方法负责 ToolCapture 状态清理 + Win32 Capture 释放
- Shell 层在业务 Cancel 成功后，同步调用 NativeHost 的捕获释放方法

### 2.2 新增调用链路

```
Esc 键按下
  → TransformKeyboardRoute.HandleKeyDown
      → pointerRoute.Cancel(Escape)                  ← 业务层取消
  → EditorTransformInputRoute.HandleKeyDown
      → r.CancelTransform(kr)                        ← 视觉恢复
      → (新增) VulkanViewportHostPanel
          → WindowsVulkanViewportHostControl
              → RequestCancelToolCapture()
                  → if (ToolCapture.IsActive || DragCaptured)
                  → ToolCapture.ClearState()
                  → _mouseCapture.Release(hwnd, "Esc取消MoveGizmo")
                      → GetCapture() == hwnd ? ReleaseCapture() : 不调
                      → 中文日志记录
```

### 2.3 修改文件

| 文件 | 变更 |
|---|---|
| `WindowsVulkanViewportHostControl.cs` | 新增 `RequestCancelToolCapture()` 方法 |
| `VulkanViewportHostPanel.axaml.cs` | 新增 `RequestCancelToolCapture()` 转发 |
| `EditorShellCompositionRuntime.cs` | `CancelActiveTransform` 中增加 `RequestCancelToolCapture()` 调用 |

### 2.4 新增方法代码

**WindowsVulkanViewportHostControl.RequestCancelToolCapture():**
```csharp
public void RequestCancelToolCapture()
{
    if (_arbitration.ToolCapture.IsActive || _arbitration.ToolCapture.DragCaptured)
    {
        _arbitration.ToolCapture.ClearState();
        _mouseCapture.Release(_windowHandle, "Esc取消MoveGizmo");
    }
}
```

### 2.5 行为约束

修复后的 `RequestCancelToolCapture` 保证：
- ✅ 只清理 ToolCapture 状态（不碰 NavCapture）
- ✅ 只释放 Win32 Capture（通过唯一出口 `_mouseCapture.Release`）
- ❌ 不触发 `SceneToolPointerReleased`
- ❌ 不触发 `applyTransform`
- ❌ 不走 Commit
- ❌ 不刷新 Inspector / Diagnostics / PickSnapshot
- ❌ 不修改 `_rawPointerDragCaptured`（只用于中键）

---

## 三、修复后状态机变化

### 3.1 MoveGizmo 左键拖动 + Esc 取消（修复后）

| 阶段 | 消息 | 修复前动作 | 修复后动作 |
|---|---|---|---|
| Pressed | WM_LBUTTONDOWN | ToolCapture.BeginDrag + mouseCapture.Capture | 不变 |
| DragPreview | WM_MOUSEMOVE | Gizmo Preview | 不变 |
| Esc Cancel | WM_KEYDOWN(0x1B) | 仅业务 Cancel | 业务 Cancel + **ToolCapture.ClearState + mouseCapture.Release** |
| → 后续 WM_LBUTTONUP | — | ToolCapture 仍 active → 走工具释放路径 | ToolCapture 已 cleared → 走 legacy 路径（仅 pick） |
| → CaptureChanged | 系统回调 | 内部状态同步 | 幂等（状态已清） |

### 3.2 修复后状态转换

```
  MoveGizmo 左键拖动中        Win32 Capture  ==== 持有中 ====
         ↓
    用户按 Esc
         ↓
  ┌──────────────────────────────────────┐
  │ TransformPointerRoute.Cancel         │  ← 业务层
  │   → Gizmo.EndDrag                    │
  │   → DragRoute.Cancel                 │
  │   → 返回 Cancelled + InitialTransform│
  └──────────────────────────────────────┘
         ↓
  ┌──────────────────────────────────────┐
  │ TransformApplyRoute.Cancel           │  ← 视觉恢复
  │   → 渲染恢复初始位置                  │
  └──────────────────────────────────────┘
         ↓
  ┌──────────────────────────────────────┐
  │ RequestCancelToolCapture()           │  ← NEW: NativeHost 层
  │   → ToolCapture.ClearState()         │
  │   → mouseCapture.Release(           │
  │       "Esc取消MoveGizmo")            │
  │     → ReleaseCapture()               │
  │     → WM_CAPTURECHANGED 同步内部状态   │
  └──────────────────────────────────────┘
         ↓
  Win32 Capture  ==== 已释放 ====
```

---

## 四、修复前后差异

| 项目 | 修复前 | 修复后 |
|---|---|---|
| Esc 取消后 Win32 Capture | 仍持有（等待下一次 WM_LBUTTONUP） | **立即释放** |
| ToolCapture 状态残留 | `IsActive=true` 残留 | `ClearState()` → 全 false |
| 后续 WM_LBUTTONUP 行为 | 误入工具释放路径 | 正确走 legacy 路径 |
| 日志记录 | 无 Esc 释放记录 | `[鼠标捕获] 阶段=释放 来源=... 原因=Esc取消MoveGizmo` |
| 对其他捕获来源的影响 | — | 无（只清 ToolCapture，不碰 NavCapture/中键） |

---

## 五、验收确认

### 5.1 验收项

| # | 验收项 | 验收方法 | 结果 |
|---|---|---|---|
| 1 | MoveGizmo 拖动中按 Esc 后 Win32 Capture 立即释放 | 代码审查 + 释放日志 | ✅ |
| 2 | Esc 后不依赖下一次 WM_LBUTTONUP 才释放 | 代码审查 | ✅ |
| 3 | Esc Cancel 不触发 Commit | 代码审查（不走 TransformPointerRoute.OnPointerReleased） | ✅ |
| 4 | Esc Cancel 不调用 SceneToolPointerReleased | 代码审查（RequestCancelToolCapture 不触发事件） | ✅ |
| 5 | Esc Cancel 后左键点击不会误入旧 ToolCapture | 代码审查（ToolCapture 已 ClearState） | ✅ |
| 6 | 中键导航不受影响 | 代码审查（不修改中键路径） | ✅ |
| 7 | Overlay 导航左键不受影响 | 代码审查（不修改 NavCapture 路径） | ✅ |
| 8 | 普通 Picking 不受影响 | 代码审查（legacy 路径不变） | ✅ |
| 9 | Build 0 error | dotnet build | ✅ |
| 10 | Tests 697/698 通过（1 个已有中文字符排序失败，与本次无关） | dotnet test | ✅ |

### 5.2 安全性分析

**Q: 如果 Cancel 时 ToolCapture 已经不 active（例如已被 WM_KILLFOCUS 清理），调用 RequestCancelToolCapture 会怎样？**

A: 方法是幂等的——`if (_arbitration.ToolCapture.IsActive || _arbitration.ToolCapture.DragCaptured)` 条件不满足，直接跳过。`_mouseCapture.Release` 在多处调用也是安全的（它以 `GetCapture() == ownerHwnd` 做最终判断）。

**Q: 释放后 WM_CAPTURECHANGED 是否会递归触发清理？**

A: `ReleaseCapture()` 会触发 `WM_CAPTURECHANGED` 同步进入 WndProc → `HandleCaptureChanged` → `ClearState`，但 ToolCapture 已清理、`_mouseCapture` 已清理，全部幂等。设计上 `WM_CAPTURECHANGED` 不调 `ReleaseCapture`，无递归风险。

**Q: 如果 ToolCapture 刚好处在 BeginDrag 但还没 EnterDrag 的阶段？**

A: `RequestCancelToolCapture` 用 `IsActive || DragCaptured` 做判断，覆盖两种状态。`ClearState()` 把两者都清零。

---

## 六、Build / Test 结果

```bash
dotnet build XuanYu.Engine.sln
# 结果：成功（0 错误，0 警告）

dotnet test XuanYu.Engine.Tests --no-build
# 结果：697 passed / 1 failed
# 失败项：WorldHierarchyTreeBuilderTests.Build_MultipleEntities_OrdersByGroupThenDisplayName（中文字符串排序受 Culture 影响，与 9.0X-2 无关）
```

---

## 七、变更清单

### 9.0X-2 核心修复（仅 3 个源文件 + 1 个文档）

| 文件 | 行数变化 | 变更类型 |
|---|---|---|
| `Panels/Viewport/NativeHost/WindowsVulkanViewportHostControl.cs` | +7 行 | 新增 `RequestCancelToolCapture()` 方法 |
| `Panels/Viewport/HostInfo/Panel/VulkanViewportHostPanel.axaml.cs` | +1 行 | 新增 `RequestCancelToolCapture()` 转发 |
| `Shell/Composition/Core/EditorShellCompositionRuntime.cs` | +4 行 | `CancelActiveTransform` 增加捕获释放调用 |
| `docs/audit-input-lifecycle-9.0X-2.md` | 新文件 | 审计文档 |

### 提交边界说明

首次提交 `44e5b86` 因 `git add .` 混入了不属于 9.0X-2 的存量变更（性能监视器、.vscode 配置、Gizmo 改动等），共 17 个文件。经 `9.0X-2R` 收口后，已重新整理为只包含上述核心修复文件的干净提交。

混入内容已在当前工作树中保留，不属 9.0X-2 范围，将随各自任务独立提交。

### 行数检查

- `WindowsVulkanViewportHostControl.cs`：修复前 93 行 → 修复后 101 行（+8，≤150 ✅）
- `VulkanViewportHostPanel.axaml.cs`：修复前 42 行 → 修复后 43 行（+1，≤100 ✅）
- `EditorShellCompositionRuntime.cs`：修复前 77 行 → 修复后 80 行（+3，≤100 ✅）

---

## 八、提交信息

```
fix(NativeHost): 9.0X-2 Esc 取消 MoveGizmo 后释放 Win32 Capture

9.0X-1 审计发现 P0 缺口：Esc 取消 Gizmo 拖动后业务层已 Cancel，
但 Win32 Capture 未释放，ToolCapture 状态残留。

修复方案（业务取消归业务层，Win32 闭环归 NativeHost 层）：
- WindowsVulkanViewportHostControl.RequestCancelToolCapture():
  清理 ToolCapture 状态 + 释放 Win32 Capture（唯一出口）
- VulkanViewportHostPanel: 转发方法
- CancelActiveTransform: 业务 Cancel 后同步调用捕获释放

不触发 SceneToolPointerReleased / Commit / Inspector 刷新。
幂等设计：以 GetCapture()==ownerHwnd 为最终依据。
```

---

## 九、禁止项确认

- [x] 未修改 Vulkan
- [x] 9.0X-2 核心修复未修改 UI 布局（注：工作树中的性能监视器 UI 变更属其他任务，不属本提交）
- [x] 未修改 Gizmo 几何 / HitTest / Transform 数学逻辑
- [x] 未混入 9.0Y / 9.1A / 9.1B
- [x] 未做右键功能
- [x] 未在 `TransformPointerRoute.Cancel` 内直接调用 Win32 ReleaseCapture
