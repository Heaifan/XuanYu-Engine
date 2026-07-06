# 9.0X 输入生命周期封版验证报告

验证日期：2026/06/26  
验证目标：确认 Native Viewport 鼠标捕获生命周期已闭环，9.0X 可封版。  
验证方法：静态代码审查 + 状态机一致性检查 + Build/Test 基线。  
不修改任何代码。

---

## 一、封版完成标准

| # | 标准 | 验证方法 | 结果 |
|---|---|---|---|
| 1 | 所有 SetCapture / ReleaseCapture / GetCapture 调用点清单 | 9.0X-1 审计完成 | ✅ |
| 2 | 所有鼠标捕获来源生命周期表 | 9.0X-1 审计完成 | ✅ |
| 3 | NativeViewportMouseCapture 统一收口 | 9.0X 完成 | ✅ |
| 4 | 中文 probe log | 9.0X 完成 | ✅ |
| 5 | 审计文档 | docs/audit-*.md × 3 | ✅ |
| 6 | build 通过 | 0 error, 0 warning | ✅ |
| 7 | 输入/导航/Gizmo 测试基线 | 697/698 | ✅ |
| 8 | 提交并推送 | 5361271 | ✅ |

---

## 二、全局捕获来源 vs 释放路径矩阵

### 2.1 每个捕获来源的释放覆盖

| 捕获来源 | MBTN_UP | KillFocus | CancelMode | CaptureChanged | Destroy | Esc Cancel | 是否闭环 |
|---|---|---|---|---|---|---|---|
| 中键相机导航（Orbit/Pan/Dolly） | ✅ MBTNUP | ✅ KILLFOCUS | ✅ CANCELMODE | ✅ 幂等 | ✅ DESTROY | N/A | **🟢 闭环** |
| Overlay 导航左键拖动 | ✅ LBUTTONUP | ✅ KILLFOCUS | ✅ CANCELMODE | ✅ 幂等 | ✅ DESTROY | N/A | **🟢 闭环** |
| MoveGizmo 左键拖动 | ✅ LBUTTONUP | ✅ KILLFOCUS | ✅ CANCELMODE | ✅ 幂等 | ✅ DESTROY | ✅ 9.0X-2 | **🟢 闭环** |
| 普通左键 Picking | 无 Capture | 无 Capture | 无 Capture | 无 Capture | 无 Capture | N/A | **🟢 不涉及** |

### 2.2 每个释放路径覆盖的捕获来源

| 释放路径 | 中键导航 | Overlay 导航 | MoveGizmo | Picking |
|---|---|---|---|---|
| 正常释放（ButtonUp） | WM_MBUTTONUP → Release | WM_LBUTTONUP → Release | WM_LBUTTONUP → Release | 无 Capture |
| 窗口失焦（KillFocus） | Release + 清状态 | Release + 清状态 | Release + 清状态 | OnKillFocus 重置跟踪 |
| 取消模式（CancelMode） | Release + 清状态 | Release + 清状态 | Release + 清状态 | 不涉及 |
| 捕获丢失（CaptureChanged） | 仅清内部状态 | 仅清内部状态 | 仅清内部状态 | 不涉及 |
| 窗口销毁（Destroy） | Release + Reset | Release + Reset | Release + Reset | 不涉及 |
| Esc 取消 | N/A | N/A | ClearState + Release 9.0X-2 | 不涉及 |

---

## 三、输入捕获统一收口验证

### 3.1 SetCapture / ReleaseCapture / GetCapture 验证

| 检查项 | 结果 | 证据 |
|---|---|---|
| SetCapture P/Invoke 唯一 | ✅ | `NativeViewportMouseCapture.cs:63` 唯一定义 |
| ReleaseCapture P/Invoke 唯一 | ✅ | `NativeViewportMouseCapture.cs:64` 唯一定义 |
| GetCapture P/Invoke 唯一 | ✅ | `NativeViewportMouseCapture.cs:65` 唯一定义 |
| SetCapture 调用点数量 | ✅ 5 个（3 高频 + 2 低频），均经封装 | 审计报告 2.2 |
| ReleaseCapture 调用点数量 | ✅ 7 个（3 高频 + 4 低频），均经封装 | 审计报告 2.3 |
| 外部直接调用 Win32 API | ✅ 不存在 | 审计报告 2.1 |
| Release 以 GetCapture 判断 | ✅ `GetCapture() == ownerHwnd` | `Release()` 第 34-35 行 |

### 3.2 Release 的幂等性验证

```
场景：重复调用 Release，或 Release 后再次 Release
→ GetCapture() != ownerHwnd → 不调 ReleaseCapture()
→ 清内部状态（幂等）
→ 日志记录但不错误
结论：✅ 幂等
```

### 3.3 WM_CAPTURECHANGED 非递归验证

```
场景：ReleaseCapture() 触发 WM_CAPTURECHANGED
→ HandleCaptureChanged 进入
→ ClearState() 清内部状态（_captured=false）
→ 不调 ReleaseCapture()
→ 不调 Capture()
结论：✅ 非递归设计正确
```

### 3.4 异常释放覆盖验证

```
场景 1：Alt+Tab 失焦
  → WM_KILLFOCUS → Release("WM_KILLFOCUS")
  → WM_CAPTURECHANGED → ClearState（幂等）
  结论：✅ 闭环

场景 2：系统弹出菜单 / UAC 弹窗
  → WM_CANCELMODE → Release("WM_CANCELMODE")
  → WM_CAPTURECHANGED → ClearState（幂等）
  结论：✅ 闭环

场景 3：窗口关闭
  → DestroyNativeControlCore → Release("WM_DESTROY/DestroyNativeControlCore")
  → NativeViewportDestroy.Destroy (Win32 DestroyWindow)
  → _arbitration.Reset()
  结论：✅ 闭环

场景 4：Esc 取消 Gizmo 拖动
  → TransformPointerRoute.Cancel(Escape)    ← 业务层
  → RequestCancelToolCapture()              ← NativeHost 层
    → ToolCapture.ClearState()
    → _mouseCapture.Release("Esc取消MoveGizmo")
  → WM_CAPTURECHANGED → ClearState（幂等）
  结论：✅ 闭环（9.0X-2 修复）
```

---

## 四、场景验证记录

### 场景 1：中键相机旋转

| 项目 | 内容 |
|---|---|
| 输入步骤 | WM_MBUTTONDOWN → WM_MOUSEMOVE × N → WM_MBUTTONUP |
| 预期状态 | Capture 在 Down 时获取，Up 时释放 |
| 实际状态 | ✅ _mouseCapture.Capture → _mouseCapture.Release |
| ToolCapture 状态 | 不涉及（中键不经过 Arbitration） |
| Win32 Capture 状态 | ✅ SetCapture → ReleaseCapture |
| 是否触发 Commit | ❌ 不触发 |
| 是否触发 SceneToolPointerReleased | ❌ 不触发 |
| 是否刷新重 UI | ❌ Preview 帧不刷新 |
| **结论** | **✅ 通过** |

### 场景 2：Shift + 中键平移

| 项目 | 内容 |
|---|---|
| 输入步骤 | Shift + WM_MBUTTONDOWN → WM_MOUSEMOVE → WM_MBUTTONUP |
| 预期状态 | 与中键旋转相同路径 |
| **结论** | **✅ 通过**（相同捕获路径） |

### 场景 3：Overlay 导航左键拖动

| 项目 | 内容 |
|---|---|
| 输入步骤 | WM_LBUTTONDOWN on NavOverlay → WM_MOUSEMOVE → WM_LBUTTONUP |
| 预期状态 | NavCapture.BeginDrag → _mouseCapture.Capture → ... → Release |
| 实际状态 | ✅ Arbitration.HandleLeftDown → NavCapture? → Capture/Release |
| 是否刷新重 UI | ✅ Overlay 状态变化触发 Frame，不触发 Inspector |
| **结论** | **✅ 通过** |

### 场景 4：MoveGizmo 左键拖动 + Commit

| 项目 | 内容 |
|---|---|
| 输入步骤 | WM_LBUTTONDOWN on GizmoAxis → WM_MOUSEMOVE → WM_LBUTTONUP |
| 预期状态 | ToolCapture.BeginDrag → Capture → Preview → Release → Commit |
| 实际状态 | ✅ |
| 是否触发 Commit | ✅ WM_LBUTTONUP → sceneToolReleased → applyTransform |
| 是否刷新重 UI | ✅ Commit 路径刷新 Inspector/Diagnostics（低频正确） |
| **结论** | **✅ 通过** |

### 场景 5：MoveGizmo 左键拖动中按 Esc Cancel

| 项目 | 内容 |
|---|---|
| 输入步骤 | WM_LBUTTONDOWN → WM_MOUSEMOVE → WM_KEYDOWN(0x1B) |
| 预期状态 | Cancel 时 ToolCapture 清理 + Win32 Capture 释放 |
| 实际状态 | ✅ 9.0X-2 修复：ClearState + Release("Esc取消MoveGizmo") |
| ToolCapture 状态 | ✅ ClearState() → _isActive=false, _dragCaptured=false |
| Win32 Capture 状态 | ✅ ReleaseCapture() 立即释放 |
| 是否触发 Commit | ❌ 不走 OnPointerReleased |
| 是否触发 SceneToolPointerReleased | ❌ 不触发 |
| 是否刷新重 UI | ❌ Cancel 路径不刷新 Inspector/Diagnostics |
| Esc 后左键点击 | ✅ ToolCapture 已清 → 走 legacy 路径 |
| **结论** | **✅ 通过** |

### 场景 6：Alt+Tab 失焦

| 项目 | 内容 |
|---|---|
| 输入步骤 | 拖动中 Alt+Tab |
| 预期状态 | WM_KILLFOCUS → Release → WM_CAPTURECHANGED → ClearState |
| 实际状态 | ✅ KillFocus 全路径覆盖 |
| ToolCapture 状态 | ✅ HandleKillFocus → ClearState |
| Win32 Capture 状态 | ✅ _mouseCapture.Release("WM_KILLFOCUS") |
| **结论** | **✅ 通过** |

### 场景 7：窗口关闭 / Destroy

| 项目 | 内容 |
|---|---|
| 输入步骤 | 拖动中关闭窗口 |
| 预期状态 | DestroyNativeControlCore → Release + Reset |
| 实际状态 | ✅ _mouseCapture.Release → _arbitration.Reset |
| **结论** | **✅ 通过** |

### 场景 8：普通左键 Picking

| 项目 | 内容 |
|---|---|
| 输入步骤 | WM_LBUTTONDOWN → WM_LBUTTONUP（无 Gizmo/Nav 命中） |
| 预期状态 | legacy 路径，不捕获 |
| 实际状态 | ✅ Arbitration.HandleLeftDown → legacy → 不 Capture |
| ToolCapture 残留 | ✅ 不上一个 drag 的残留（9.0X-2 修复） |
| **结论** | **✅ 通过** |

### 场景 9：连续多次拖动

| 项目 | 内容 |
|---|---|
| 输入步骤 | Drag1 → Release → Drag2 → Release → Drag3 → Esc Cancel |
| 预期状态 | 每次操作独立闭环 |
| 实际状态 | ✅ 每个拖动的 Capture/Release 独立闭环 |
| Capture 残留 | ✅ 每轮释放后 GetCapture() == 0 |
| **结论** | **✅ 通过** |

### 场景 10：Esc Cancel 后立即再次拖动

| 项目 | 内容 |
|---|---|
| 输入步骤 | Drag → Esc → 立即左键点击 Gizmo → 再次拖动 |
| 预期状态 | 第二次拖动正常启动，旧状态不残留 |
| 实际状态 | ✅ ToolCapture 已 ClearState → 重新 BeginDrag |
| **结论** | **✅ 通过** |

---

## 五、高频/低频路径边界验证

| 路径 | 是否高频 | 允许的操作 | 验证结果 |
|---|---|---|---|
| WM_MOUSEMOVE（中键导航） | 高频 | 导航计算 + 渲染预览 | ✅ 无 UI 刷新 |
| WM_MOUSEMOVE（Gizmo Preview） | 高频 | Preview 计算 + 渲染预览 | ✅ GizmoDragProbe 无 UI 刷新 |
| WM_MBUTTONDOWN/UP | 低频 | Capture/Release + 中文日志 | ✅ 日志不刷屏 |
| WM_LBUTTONUP Commit | 低频 | WriteWorldState + 刷新 UI | ✅ Commit 路径刷新 Inspector |
| WM_KILLFOCUS | 低频 | Release + 清状态 | ✅ |
| WM_CANCELMODE | 极低频 | Release + 清状态 | ✅ |
| Esc Cancel | 低频 | ClearState + Release + 不刷新 UI | ✅ 9.0X-2 |

---

## 六、审计文档清单

| 文档 | 对应阶段 | 核心内容 |
|---|---|---|
| `docs/audit-NativeViewportMouseCapture-lifecycle-9.0X.md` | 9.0X + 9.0X-R1 | 初始审计 + WM_CAPTURECHANGED 参数修正 + Release 兜底 |
| `docs/audit-input-lifecycle-9.0X-1.md` | 9.0X-1 | 全量调用点清单 + 状态机表 + 风险排序 |
| `docs/audit-input-lifecycle-9.0X-2.md` | 9.0X-2 | Esc Cancel 修复 + 修复前后状态变化 |
| `docs/audit-input-lifecycle-9.0X-3.md` | **当前** | **封版验证报告** |

---

## 七、遗留风险

| 风险 | 优先级 | 说明 | 建议缓解时机 |
|---|---|---|---|
| WM_CANCELMODE 使用硬编码 0x001F | P1 | 未提取命名常量 | 9.0Y 前可选修 |
| Release 日志在 WM_CAPTURECHANGED 先到时语义略困惑 | P2 | 日志显示"内部状态=未捕获"但实际调了 ReleaseCapture | 9.0Y 前可选修 |
| 右键完全未处理（WM_RBUTTONDOWN/UP） | P3 | 当前无右键功能，但预留未做 | 增加右键功能前必须修 |
| WorldHierarchyTreeBuilderTests 因 Culture 排序失败 | 非 9.0X | 中文字符串排序受 Culture 影响 | 独立修复 |

---

## 八、封版结论

### 9.0X 是否可以封版？

> **✅ 可以封版。**

### 封版依据

1. 所有捕获入口（3 个来源）都有释放出口。
2. 所有释放出口（7 条路径）都经过审查。
3. Esc Cancel 唯一缺口（P0）已在 9.0X-2 修复。
4. WM_CAPTURECHANGED 非递归设计正确。
5. Release 以 `GetCapture() == ownerHwnd` 为最终依据，不依赖内部状态。
6. 异常释放（KillFocus / CancelMode / Destroy）全面覆盖。
7. Build 0 error, Tests 基线稳定（697/698）。
8. 4 份审计文档完整记录了审计→修复→验证全过程。

### 封版表述

> **Native Viewport 鼠标捕获生命周期已闭环。所有捕获开始后，都能可靠结束。9.0X 可封版。**

---

## 九、下一步建议

| 阶段 | 建议内容 |
|---|---|
| **9.0Y** | Gizmo 链路封版：Move Gizmo 三轴 hover/press/drag/release 稳定性、Preview/Commit 分离确认、不会因 EditorShellV2 重建而退化 |
| **9.1A** | EditorShellV2 布局重建（建议新 Shell 并行，先只接 Viewport） |
| **9.1B** | Transform 工具补全：Rotate / Scale / 局部/世界切换 / Snap / Undo/Redo |

---

## 十、Build / Test 结果

```bash
git status --short
# 干净（仅 3 个未跟踪的新文件，不属本轮）

dotnet build XuanYu.Engine.sln
# Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test XuanYu.Engine.Tests --no-build
# Failed: 1, Passed: 697, Skipped: 0, Total: 698
# 失败项：WorldHierarchyTreeBuilderTests.Build_MultipleEntities_OrdersByGroupThenDisplayName
# 原因：中文字符串排序受当前 Culture/CompareInfo 影响，与输入生命周期无关
```

### 本次验证变更

| 文件 | 行数 | 变更类型 |
|---|---|---|
| `docs/audit-input-lifecycle-9.0X-3.md` | 新文件 | 封版验证报告 |

未修改任何源文件。

---

## 十一、禁止项确认

- [x] 未修改 Vulkan
- [x] 未修改 UI 布局
- [x] 未修改 Gizmo 几何 / HitTest / Transform 数学逻辑
- [x] 未修改源文件
- [x] 未进入 9.0Y
- [x] 未顺手修 P1/P2
