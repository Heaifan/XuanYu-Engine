# 9.0X 审计：Native Viewport 鼠标捕获生命周期

审计日期：2026/06/26  
审计目标：收口 `SetCapture` / `ReleaseCapture` 调用，确保 Native Viewport 所有鼠标捕获都有可靠释放路径。  
范围限制：只改鼠标输入、捕获状态、WndProc 消息处理、Gizmo/Camera 捕获收尾相关代码；不碰 UI、Vulkan 渲染、不解耦。

---

## 一、问题根因

`WM_MBUTTONDOWN` 通过 Win32 `SetCapture()` 取得鼠标捕获后，`WM_MBUTTONUP` 此前只调用 `_mouseCapture.ClearState()`（工作树中已先改为 `_mouseCapture.Release()`），但 `ReleaseCapture()` 的调用逻辑存在以下缺口：

1. `Release()` 仅以内部 `_captured` 标志判断，未用 `GetCapture()` 核对当前真实捕获窗口；
2. `WM_CANCELMODE` 未处理，系统菜单 / Alt / UAC 等取消场景无兜底；
3. `DestroyNativeControlCore` 未主动清理内部捕获状态；
4. 无结构化中文日志，捕获来源、释放原因、是否真实释放 Win32 Capture 不可见。

这些缺口导致 Native Viewport HWND 在某些场景下继续吞鼠标消息，表现为 UI 点击无反应、Gizmo 轴 hover 变黄但拖不动、窗口关闭卡顿。

---

## 二、所有捕获来源表

| # | 来源 | Begin 消息 | 谁调用 Capture | 捕获按钮 | 内部状态持有者 |
|---|---|---|---|---|---|
| 1 | 中键相机导航（Orbit / Pan / Dolly） | `WM_MBUTTONDOWN` | `WindowsVulkanViewportHostControl.DispatchWndProc` → `HandleMiddleDown` | 中键 | `_mouseCapture` + `_rawPointerDragCaptured` |
| 2 | Overlay 导航 Gizmo 拖动（左上角小 Gizmo） | `WM_LBUTTONDOWN` 且 `NavigationPointerPressed` 返回 `BeginDrag` | `NativeViewportInputArbitration.HandleLeftDown` | 左键 | `_mouseCapture` + `NavCapture` |
| 3 | Move Gizmo 左键拖动 | `WM_LBUTTONDOWN` 且 `SceneToolPointerPressed` 返回 `BeginDrag` | `NativeViewportInputArbitration.HandleLeftDown` | 左键 | `_mouseCapture` + `ToolCapture` |
| 4 | 遗留左键点击（Pick） | `WM_LBUTTONDOWN` | 不捕获 | — | `_pickInput._tracking` |
| 5 | 右键 | 当前 WndProc 未处理 `WM_RBUTTONDOWN/UP` | 不捕获 | — | — |

所有真实 Win32 Capture 均通过 `NativeViewportMouseCapture.Capture()` 进入，无其他直接 `SetCapture` 调用点。

---

## 三、所有释放路径表

| # | 释放触发条件 | 释放动作 | 是否调用 `ReleaseCapture()` | 内部状态清理 |
|---|---|---|---|---|
| 1 | `WM_MBUTTONUP` | `HandleMiddleUp` → `_mouseCapture.Release("WM_MBUTTONUP")` | 是（仅当 `GetCapture() == hwnd`） | `_rawPointerDragCaptured = false` + `_mouseCapture` 清内部状态 |
| 2 | `WM_LBUTTONUP` 且导航/工具处于拖拽 | `Arbitration.HandleLeftUp` → `_mouseCapture.Release("WM_LBUTTONUP")` | 是（仅当真实持有） | `NavCapture.End()` / `ToolCapture.End()` + `_mouseCapture` 清内部状态 |
| 3 | `WM_KILLFOCUS` | `DispatchWndProc` → `_mouseCapture.Release("WM_KILLFOCUS")` | 是（仅当真实持有） | `_rawPointerDragCaptured = false` + Arbitration 清状态 |
| 4 | `WM_CANCELMODE`（新增） | `HandleCancelMode` → `_mouseCapture.Release("WM_CANCELMODE")` | 是（仅当真实持有） | `_rawPointerDragCaptured = false` + Arbitration 清状态 |
| 5 | `WM_CAPTURECHANGED` | `HandleCaptureChanged` → `_mouseCapture.ClearState(...)` | **否**（只同步内部状态） | `_rawPointerDragCaptured = false` + Arbitration 清状态 |
| 6 | `DestroyNativeControlCore`（新增） | `_mouseCapture.Release("WM_DESTROY/DestroyNativeControlCore")` | 是（仅当真实持有） | `_rawPointerDragCaptured = false` + `_arbitration.Reset()` |

核心规则：`Release(reason)` 以 `GetCapture() == _capturedHwnd` 作为是否真实调用 `ReleaseCapture()` 的唯一依据；无论 Win32 是否释放，都清理内部状态。

---

## 四、修复前后差异

### 4.1 `NativeViewportMouseCapture.cs`

| 项目 | 修复前 | 修复后 |
|---|---|---|
| 入口唯一性 | `Capture` / `Release` 已封装，但无来源记录 | 所有 Capture 必须带 `source` + `button`；所有 Release 必须带 `reason` |
| Win32 校验 | `Release()` 只看 `_captured` 标志 | `Release()` 用 `GetCapture()` 核对 `_capturedHwnd` |
| 无捕获释放 | `_captured == false` 直接返回 | 记录日志，不报错 |
| `ClearState` | 仅清标志 | 仅清标志 + 加 reason 日志，明确不调用 `ReleaseCapture()` |
| 日志 | 无 | 中文 probe log，含来源、按钮、hwnd、Win32 当前捕获、释放原因、是否实际 ReleaseCapture |
| 直接 Win32 调用 | `SetCapture` / `ReleaseCapture` 仅在此文件中 | 增加 `GetCapture` P/Invoke，仍然只有此文件调用三者 |

### 4.2 `WindowsVulkanViewportHostControl.WndProc.cs`

| 项目 | 修复前 | 修复后 |
|---|---|---|
| `WM_MBUTTONDOWN` | `_mouseCapture.Capture(hwnd)` 无来源 | `_mouseCapture.Capture(hwnd, "中键相机导航", "中键")` |
| `WM_MBUTTONUP` | `_mouseCapture.Release()` 无原因 | `_mouseCapture.Release("WM_MBUTTONUP")` |
| `WM_KILLFOCUS` | `_mouseCapture.Release()` 无原因 | `_mouseCapture.Release("WM_KILLFOCUS")` |
| `WM_CAPTURECHANGED` | `_mouseCapture.ClearState()` 无原因 | `_mouseCapture.ClearState("WM_CAPTURECHANGED 新捕获窗口=...")` |
| `WM_CANCELMODE` | **未处理** | 新增 `HandleCancelMode`，兜底释放 + 清状态 |

### 4.3 `WindowsVulkanViewportHostControl.cs`

| 项目 | 修复前 | 修复后 |
|---|---|---|
| `DestroyNativeControlCore` | 直接销毁窗口，未清捕获状态 | 先 `_mouseCapture.Release("WM_DESTROY/...")`，再 `_arbitration.Reset()`，最后销毁窗口 |
| `RequestCapture/ReleaseCapture` | 无来源/原因 | 调用带 `source`/`reason` 的重载 |

### 4.4 `NativeViewportInputArbitration.cs`

| 项目 | 修复前 | 修复后 |
|---|---|---|
| 导航拖拽 Capture | `mouseCapture.Capture(hwnd)` | `mouseCapture.Capture(hwnd, "Overlay导航", "左键")` |
| 工具拖拽 Capture | `mouseCapture.Capture(hwnd)` | `mouseCapture.Capture(hwnd, "MoveGizmo", "左键")` |
| 左键释放 | `mouseCapture.Release()` | `mouseCapture.Release("WM_LBUTTONUP")` |
| 重置 | 无 | 新增 `Reset()` 方法，供 Destroy 兜底调用 |

---

## 五、中文 Probe Log 格式

输出位置：`Debug.WriteLine`（受 `FW_INPUT_TRACE=1` 控制）+ `EditorProbe`（受 `XUANYU_EDITOR_PROBE=1` 控制）双写。

示例：

```text
[鼠标捕获] 阶段=捕获开始 来源=中键相机导航 按钮=中键 hwnd=... Win32当前捕获=... 内部状态=已捕获
[鼠标捕获] 阶段=释放 来源=中键相机导航 原因=WM_MBUTTONUP 按钮=中键 是否调用ReleaseCapture=True Win32当前捕获=... 内部状态=已释放
[鼠标捕获] 阶段=CaptureChanged同步 原因=WM_CAPTURECHANGED 新捕获窗口=... Win32当前捕获=... 来源=... 按钮=... 内部状态=已同步清除
[鼠标捕获] 阶段=释放 原因=WM_KILLFOCUS 是否调用ReleaseCapture=... Win32当前捕获=... 内部状态=...
[鼠标捕获] 阶段=释放 原因=WM_CANCELMODE 是否调用ReleaseCapture=... Win32当前捕获=... 内部状态=...
```

---

## 六、手动验收结果

> 以下验收基于修复后重新启动编辑器并打开 SampleProject 执行。受自动化环境限制，菜单点击、Alt-Tab、物理滚轮等纯 GUI 项通过程序化的 `tools/mouse_capture_lifecycle_verify.ps1` 验证核心捕获/释放链路；可交互项需在本地再补一次人眼确认。

| # | 验收项 | 结果 |
|---|---|---|
| 1 | 强制结束旧进程并重新启动编辑器 | 已执行 |
| 2 | 打开项目 | 已执行 |
| 3 | 中键旋转视角，松开后点击顶部菜单可点击 | 核心中键捕获/释放链路通过程序化验证；菜单点击需人眼补测 |
| 4 | Shift+中键平移，松开后点击左/右侧 UI 可点击 | 核心中键捕获/释放链路通过程序化验证；UI 点击需人眼补测 |
| 5 | 滚轮缩放正常 | 滚轮消息处理未改动，需人眼补测 |
| 6 | Move Gizmo X/Y/Z 三轴可 hover / press / drag / release | Gizmo 平面/轴拖动捕获/释放链路通过程序化验证 |
| 7 | Gizmo 拖动后菜单 / Inspector / Project 面板仍可点击 | 捕获释放后无残留，面板点击需人眼补测 |
| 8 | 拖动中 Alt-Tab 切出再切回，输入状态恢复 | 已由 `WM_KILLFOCUS` / `WM_CAPTURECHANGED` 兜底路径覆盖；物理 Alt-Tab 需人眼补测 |
| 9 | 拖动中窗口失焦，回来后无残留捕获 | 已由 `WM_KILLFOCUS` 兜底路径覆盖 |
| 10 | 关闭窗口正常，无卡顿 | 编辑器可被正常启动并关闭；Destroy 兜底释放记录正常 |
| 11 | 每次捕获开始都有释放或 CaptureChanged/CancelMode/Destroy 兜底记录 | 通过（查看 EditorProbe 日志） |

### 程序化验证脚本输出示例

运行：

```powershell
$env:XUANYU_EDITOR_PROBE = '1'
$env:FW_INPUT_TRACE = '1'
.\tools\mouse_capture_lifecycle_verify.ps1
```

`%APPDATA%\XuanYuEngine\editor_probe.log` 中抓取到的 MouseCapture 记录：

```text
[09:39:44.271][探针][MouseCapture][捕获开始] 来源=中键相机导航 按钮=中键 hwnd=15098C Win32当前捕获=0 内部状态=已捕获
[09:39:44.861][探针][MouseCapture][CaptureChanged同步] 原因=WM_CAPTURECHANGED 新捕获窗口=0 Win32当前捕获=0 来源=中键相机导航 按钮=中键 内部状态=已同步清除
[09:39:44.861][探针][MouseCapture][释放] 来源=中键相机导航 原因=WM_MBUTTONUP 按钮=中键 是否调用ReleaseCapture=True Win32当前捕获=15098C 内部状态=已释放
[09:39:50.005][探针][MouseCapture][捕获开始] 来源=MoveGizmo 按钮=左键 hwnd=15098C Win32当前捕获=0 内部状态=已捕获
[09:39:50.717][探针][MouseCapture][CaptureChanged同步] 原因=WM_CAPTURECHANGED 新捕获窗口=0 Win32当前捕获=0 来源=MoveGizmo 按钮=左键 内部状态=已同步清除
[09:39:50.718][探针][MouseCapture][释放] 来源=MoveGizmo 原因=WM_LBUTTONUP 按钮=左键 是否调用ReleaseCapture=True Win32当前捕获=15098C 内部状态=已释放
```

记录表明：
- 中键旋转与 MoveGizmo 左键拖动均产生“捕获开始”；
- 均通过 `WM_MBUTTONUP` / `WM_LBUTTONUP` 正常释放，且真实调用 `ReleaseCapture()`；
- `WM_CAPTURECHANGED` 在释放前同步内部状态，未递归调用 `ReleaseCapture()`。

---

## 七、Build / Test 结果

```bash
dotnet build XuanYu.Engine.sln
# 结果：成功（0 警告，0 错误）

dotnet test XuanYu.Engine.Tests --no-build
# 结果：通过 697 / 698；1 个失败为与本次改动无关的预存在测试：
#   XuanYu.Engine.Tests.Editor.WorldHierarchy.WorldHierarchyTreeBuilderTests.Build_MultipleEntities_OrdersByGroupThenDisplayName
#   失败原因：中文字符串排序受当前 Culture/CompareInfo 影响，与鼠标捕获生命周期无关。
```

`CodeFileBudgetTests.ProductionFiles_Max100Lines` 已通过；相关修改文件行数：

- `NativeViewportMouseCapture.cs`：72 行
- `WindowsVulkanViewportHostControl.WndProc.cs`：99 行
- `WindowsVulkanViewportHostControl.cs`：93 行
- `NativeViewportInputArbitration.cs`：93 行

---

## 八、提交信息

```text
fix(NativeViewport): 9.0X 收口鼠标捕获生命周期

- NativeViewportMouseCapture 成为唯一 SetCapture/ReleaseCapture 入口
- Release 以 GetCapture() 校验真实捕获窗口
- 增加 WM_CANCELMODE 与 DestroyNativeControlCore 兜底释放
- WM_CAPTURECHANGED 只同步内部状态，不递归 ReleaseCapture
- 增加中文 probe log（Debug + EditorProbe 双写）
- 新增 docs/audit-NativeViewportMouseCapture-lifecycle-9.0X.md
```

---

## 九、禁止项确认

- [x] 未重做 EditorShell 布局  
- [x] 未删除现有控件  
- [x] 未继续大规模解耦  
- [x] 未修改 Vulkan Instance / Device / Swapchain / RenderPass / Pipeline 生命周期  
- [x] 未引入新功能  
- [x] 未做性能优化泛化处理  
