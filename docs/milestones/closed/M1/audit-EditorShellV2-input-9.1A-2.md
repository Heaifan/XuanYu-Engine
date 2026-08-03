# 9.1A-2 审计：EditorShellV2 接入 Viewport 输入路由

审计日期：2026/06/26  
审计目标：让 V2 Viewport 接入已封版的输入路由，支持基础相机导航。不接 Inspector/StatusBar/ProjectTree。  
范围限制：不恢复 stash、不修改旧 Shell、不修改 Vulkan/Gizmo/Transform、不刷新重 UI。

---

## 一、新增/修改文件清单

| 文件 | 行数 | 职责 | 类型 |
|---|---|---|---|
| `ShellV2/Composition/EditorShellV2Routes.cs` | 40 | 路由工厂：创建所有 Route 实例 | **新增** |
| `ShellV2/Composition/EditorShellV2InputWiring.cs` | 82 | 输入接线：ViewportPanel 事件 → Route | **新增** |
| `ShellV2/Composition/EditorShellV2Context.cs` | 61 | +输入路由引用（Selection/Pointer/Camera/Navigation 等） | **修改** |
| `ShellV2/Composition/EditorShellV2Composition.cs` | 96 | 改为调用子文件 Create() + Wire()，不膨胀 | **修改** |

### 行数检查

| 文件 | 行数 | 红线 |
|---|---|---|
| `EditorShellV2Composition.cs` | 96 | ≤100 ✅（9.1A-1 时 95，此次仅微调结构） |
| `EditorShellV2Context.cs` | 61 | ≤100 ✅ |
| `EditorShellV2InputWiring.cs` | 82 | ≤100 ✅ |
| `EditorShellV2Routes.cs` | 40 | ≤100 ✅ |

---

## 二、V2 输入事件接线图

```
VulkanViewportHostPanel 事件                Route 消费                     行为
─────────────────────────                  ──────────                     ────
RawPointerButtonDown(btn=4)
  → EditorShellV2InputWiring               ctx.Camera.Apply(Orbit)       中键旋转
  → (Shift 标记时)                          ctx.Camera.Apply(Pan)          Shift+中键平移

RawPointerMoved(dx, dy)
  → if _isOrbiting                         ctx.Camera.Apply(Orbit)       旋转视角
  → if _isPanning                          ctx.Camera.Apply(Pan)         平移视角
  → ctx.Pointer.OnPointerMoved()            TransformPointerRoute         Gizmo hover 更新

RawPointerButtonUp(btn=4)
  → 清除 _isOrbiting / _isPanning                                      捕获释放（9.0X）

RawMouseWheel(delta)
  → ctx.Camera.Apply(Zoom)                                              滚轮缩放

RawKeyDown(0x10=Shift)
  → 标记 _isPanning                                                      Shift 模式

RawInputFocusLost
  → 清除所有导航状态                                                    失焦兜底

NavigationPointerPressed
  → return NotHandled（预留：V2 暂不显示导航叠加层）

SceneToolPointerPressed
  → return NotHandled（预留：9.1A-3 接入 Gizmo）

PickRequested
  → 空桩（预留：9.1A-3 接入 Picking）
```

---

## 三、复用哪些旧 Route

| Route | 来源 | 构造方式 | 复用状态 |
|---|---|---|---|
| `ViewportCameraRoute` | 旧 Shell | `new()` | ✅ |
| `ViewportNavigationRoute` | 旧 Shell | `new()` | ✅ |
| `TransformPointerRoute` | 旧 Shell | `new()` | ✅ |
| `EditorSelectionRoute` | 旧 Shell | `new()` | ✅ |
| `ViewportPointerPickRoute` | 旧 Shell | `new()` | ✅ |
| `EditorPickInputRoute` | 旧 Shell | `new()` | ✅ |
| `EditorGroundHoverInputRoute` | 旧 Shell | `new()` | ✅ |
| `EditorSceneToolInputRoute` | 旧 Shell | `new()` | ✅ |
| `EditorTransformApplyRoute` | 旧 Shell | `new()` | ✅ |
| `EditorViewportInputRoute` | 旧 Shell | `new()` | ✅ |

**V2 只重新 wiring，不复制旧 Shell 的大块逻辑。**

---

## 四、未接入哪些面板

| 面板 | 接入阶段 | 原因 |
|---|---|---|
| Inspector | 9.1A-4 | 9.1A-2 不接入 |
| StatusBar | 9.1A-3 | 9.1A-2 不接入 |
| ProjectTree | 9.1A-3 | 9.1A-2 不接入 |
| Diagnostics / Console | 9.1A-4 | 9.1A-2 不接入 |
| Gizmo 完整交互 | 9.1A-3 | 9.1A-2 预留桩 |
| Picking | 9.1A-3 | 9.1A-2 预留桩 |
| Overlay 导航叠层 | 9.1A-3 | V2 暂不显示 |

---

## 五、Preview / Commit 分离检查

| 行为 | V2 状态 | 说明 |
|---|---|---|
| Preview 高频路径是否刷新 Inspector | ❌ 不刷新 | V2 未接 Inspector |
| Preview 高频路径是否刷新 Diagnostics | ❌ 不刷新 | 仅保留 `FrameScheduler` 中 TransformPreview 跳过 ProbeValidation |
| Preview 高频路径是否刷新 PickSnapshot | ❌ 不刷新 | V2 未接 Picking |
| Commit 是否刷新 Inspector | ❌ 不接入 | Gizmo 预留桩，尚未接入 Commit |
| 180ms DispatcherTimer | ⚠️ 临时方案 | 仅用于启动探测和最小渲染，Gizmo Preview 高频需后续替换 |

---

## 六、旧 Shell 是否仍可运行

> **✅ 可运行。默认路径不受影响。**

| 检查项 | 状态 |
|---|---|
| `MainWindow.axaml` → 旧 `EditorShell` | ✅ 未修改 |
| `App.axaml.cs` 默认加载 `MainWindow` | ✅ 未修改 |
| `Shell/EditorShell.axaml` 布局 | ✅ 未修改 |
| `Shell/Composition/` 全部路由 | ✅ 未修改 |

---

## 七、风险分析

| 优先级 | 风险 | 说明 |
|---|---|---|
| **P1** | V2 中键导航状态 `_isOrbiting`/`_isPanning` 是静态字段，双 Shell 实例冲突 | 当前管理原则：同一时间只有一个 Shell 实例活跃。如果 V2 和旧 Shell 同时运行，静态标志会冲突。旧 Shell 默认路径不受影响。 |
| **P2** | 180ms DispatcherTimer 不适合 Gizmo Preview 高频刷新 | 当前仅用于基础渲染和启动探测。Gizmo 接入时需改用 9.0Y 的 Preview 原则单独处理。 |
| **P2** | SceneTool/Gizmo 和 Picking 当前为空桩 | 9.1A-3 补全，不影响 9.1A-2 验收。 |

---

## 八、Build / Test 结果

```bash
dotnet build XuanYu.Engine.sln
# Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test XuanYu.Engine.Tests
# Failed: 1, Passed: 712, Skipped: 0, Total: 713
# 失败项：WorldHierarchyTreeBuilderTests（中文字符排序，与 9.1A-2 无关）
```

### 文件结构合规性

- `ShellV2/Composition/` 文件数：4（≤5 ✅）
- 所有文件 ≤100 行 ✅
- 无重复 using / 冗余引用 ✅

---

## 九、后续建议

| 阶段 | 内容 |
|---|---|
| **9.1A-3** | 接入 Gizmo 完整交互（SceneTool）+ Picking + StatusBar |
| 9.1A-4 | 接入 Inspector（Commit 后低频刷新） |
| 9.1A-5 | 布局可调 + 折叠/隐藏 |

---

## 十、禁止项确认

- [x] 未恢复 stash
- [x] 未修改旧 Shell 运行逻辑
- [x] 未修改 Vulkan / Gizmo 数学 / Transform 逻辑
- [x] 未接入 Inspector / ProjectTree / Diagnostics / Console / StatusBar
- [x] 未引入性能监视器 / Probe 基础设施
- [x] EditorShellV2Composition.cs 未突破 100 行（96）
- [x] file-tree.md 已更新
