# 9.1A-1 审计：EditorShellV2 最小骨架

审计日期：2026/06/26  
审计目标：创建 EditorShellV2 最小可运行骨架，只接 Viewport。旧 EditorShell 保持可运行。  
范围限制：不恢复 stash、不修改旧 Shell、不修改 Vulkan/Gizmo/Transform、不接入其他面板。

---

## 一、新增文件清单

| 文件 | 行数 | 职责 |
|---|---|---|
| `ShellV2/EditorShellV2.axaml` | 38 | V2 布局：顶部菜单栏 + 中央 Viewport（覆盖层工具条预留） |
| `ShellV2/EditorShellV2.axaml.cs` | 26 | V2 生命周期：组合根构建、Attach/Detach 转发 |
| `ShellV2/Composition/EditorShellV2Context.cs` | 33 | V2 最小上下文：只持 Viewport 控件 + 核心路由引用 |
| `ShellV2/Composition/EditorShellV2Composition.cs` | 96 | V2 组合根：创建路由集、事件接线、NativeHost 驱动渲染循环 |
| `MainWindowV2.axaml` | 13 | V2 测试窗口：加载 EditorShellV2，不修改旧 MainWindow |
| `MainWindowV2.axaml.cs` | 13 | V2 窗口标题版本号 |

**未修改的旧 Shell 文件：**
- `Shell/EditorShell.axaml` — 未改动
- `Shell/EditorShell.axaml.cs` — 未改动
- `MainWindow.axaml` — 未改动
- `MainWindow.axaml.cs` — 未改动

---

## 二、每个文件职责

### EditorShellV2.axaml

```
Grid (2 行: 44px / *)
├── Row 0: 菜单栏（精简：仅文件/运行/设置）
├── Row 1: Viewport（全区域）
│   └── VulkanViewportHostPanel（x:Name=VulkanViewportHostPanelV2）
│   └── ToolOverlayBorder（浮动覆盖层，工具条预留，当前隐藏）
```

ViewportToolPalette 作为覆盖层浮在左上角，不占用 Viewport 水平空间。

### EditorShellV2.axaml.cs

- 构造函数：`_ctx = EditorShellV2Composition.Build(this)`
- `OnAttachedToVisualTree`：不执行额外操作（渲染由 NativeHostInfoChanged 驱动）
- `OnDetachedFromVisualTree`：停止渲染计时器

### EditorShellV2Context.cs

只持有 V2 需要的引用：

| 引用 | 用途 |
|---|---|
| `VulkanViewportHostPanel? ViewportPanel` | 视口控件 |
| `VulkanViewportProbeRoute` | Vulkan 设备探测 |
| `EditorStartupVulkanRoute` | 启动探测 |
| `Scene3dSessionLifecycle` | 3D Session 生命周期 |
| `ViewportRenderSceneStore` | 渲染场景状态 |
| `ViewportCameraRoute` | 相机状态 |
| `Scene3dResizeRenderRoute` | 渲染帧调度 |
| `EditorDiagnosticsRefreshRoute` | 诊断/Probe 刷新 |
| `DispatcherTimer?` | 渲染循环定时器 |

**不引用：** InspectorPanel / DebugDockPanel / ProjectWorldDockPanel / StatusBarPanel / EditorShellLogRoute / EditorShellTransformRoute / EditorShellScrubRoute 等旧 Shell 组件。

### EditorShellV2Composition.cs

创建最小路由集 + 事件接线：

```
NativeHostInfoChanged 事件
  ├── 首次: RunStartupProbe → StartupVulkan.RunConstructProbes → ScheduleRender
  └── 后续: ScheduleRender → DispatcherTimer → RenderOnce → Scene3dResizeRenderRoute.RenderOnce
```

空事件桩预留：RawPointerButtonDown/Moved/Up、RawKeyDown/Up、RawMouseWheel、RawInputFocusLost。

### MainWindowV2.axaml / MainWindowV2.axaml.cs

- 仅加载 `EditorShellV2`
- 旧 `MainWindow.axaml` 完全不受影响
- 切换方式：临时修改 `App.axaml.cs` 中 `desktop.MainWindow = new MainWindowV2()`

---

## 三、旧 Shell 是否仍可运行

> **✅ 旧 Shell 可运行。默认路径不受影响。**

| 项目 | 状态 |
|---|---|
| `MainWindow.axaml` 加载旧 `EditorShell` | ✅ 未修改 |
| `App.axaml.cs` 默认加载 `MainWindow` | ✅ 未修改 |
| `Shell/EditorShell.axaml` 布局 | ✅ 未修改 |
| `Shell/Composition/` 全部路由 | ✅ 未修改 |
| `VulkanViewportHostPanel` / NativeHost | ✅ 未修改 |

---

## 四、V2 能否加载 Viewport

> **✅ V2 可独立构造。Viewport 渲染由 NativeHostInfoChanged 驱动启动探测和渲染循环。**

当前 V2 已搭建的链路：
1. NativeHost 创建 → `NativeHostInfoChanged` → 启动 Vulkan Probe → `Scene3dResizeRenderRoute.RenderOnce`
2. 渲染循环：`DispatcherTimer` (180ms) 驱动 `RenderOnce`
3. 覆盖层工具条位置预留（当前隐藏）

---

## 五、哪些面板暂未接入

| 面板 | 接入阶段 | 原因 |
|---|---|---|
| Inspector | 9.1A-2 | 编辑核心，第二阶段 |
| StatusBar | 9.1A-2 | 简单状态栏，第二阶段 |
| ProjectTree | 9.1A-3 | 内容浏览，第三阶段 |
| Diagnostics / Console | 9.1A-4 | 底部调试页签，第四阶段 |
| 布局可调（Splitter） | 9.1A-5 | 面板交互优化，第五阶段 |

---

## 六、输入路由复用情况

| 输入路由 | 复用情况 | 说明 |
|---|---|---|
| `EditorViewportInputRoute` | **未接入** | 9.1A-2 接入 |
| `EditorSceneToolInputRoute` | **未接入** | 9.1A-2 接入 |
| `EditorShellRawInputRoute` | **未接入** | 9.1A-2 接入 |
| `EditorShellOverlayNavigationRoute` | **未接入** | 9.1A-2 接入 |
| `EditorShellPickingRoute` | **未接入** | 9.1A-2 接入 |
| `ViewportNavigationRoute` | **未接入** | 9.1A-2 接入 |
| `TransformPointerRoute` | **未接入** | 9.1A-2 接入 |

**当前 V2 只做什么：** 显示 Viewport 并渲染 3D 场景。相机导航（中键旋转等）和 Gizmo 交互在 9.1A-2 接入。

---

## 七、风险分析

| 优先级 | 风险 | 说明 |
|---|---|---|
| **P0** | V2 的 `FindControl("VulkanViewportHostPanelV2")` 可能因 axaml 命名错误返回 null | **已验证**：`x:Name="VulkanViewportHostPanelV2"` 存在于 axaml，类型匹配 |
| **P1** | `Scene3dResizeRenderRoute.RenderOnce` 在没有 Session 时静默失败 | 当前 RenderOnce 内部有 Session 空值判断，不影响构建 |
| **P2** | V2 渲染循环与旧 Shell 同时运行时可能冲突 | 管理原则：同一时间只有一个 Shell 实例活跃。当前默认加载旧 Shell，V2 需手动切换 |

---

## 八、Build / Test 结果

```bash
dotnet build XuanYu.Engine.sln
# Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test XuanYu.Engine.Tests
# Failed: 1, Passed: 712, Skipped: 0, Total: 713
# 失败项：WorldHierarchyTreeBuilderTests（中文字符排序，与 9.1A-1 无关）
```

### 行数检查

| 文件 | 行数 | 红线 |
|---|---|---|
| `EditorShellV2.axaml` | 38 | ≤100 ✅ |
| `EditorShellV2.axaml.cs` | 26 | ≤100 ✅ |
| `EditorShellV2Context.cs` | 33 | ≤100 ✅ |
| `EditorShellV2Composition.cs` | 96 | ≤100 ✅ |
| `MainWindowV2.axaml` | 13 | ≤100 ✅ |
| `MainWindowV2.axaml.cs` | 13 | ≤100 ✅ |

---

## 九、后续建议

| 阶段 | 内容 |
|---|---|
| **9.1A-2** | 接入输入路由（RawInput/Navigation/Picking/Gizmo）和 Inspector + StatusBar |
| 9.1A-3 | 接入 ProjectTree |
| 9.1A-4 | 接入 Diagnostics + Console（页签系统） |
| 9.1A-5 | 布局可调（Splitter）+ 折叠/隐藏 |

---

## 十、禁止项确认

- [x] 未恢复 stash
- [x] 未修改旧 Shell 运行逻辑
- [x] 未修改 Vulkan / Gizmo / Transform
- [x] 未接入 Inspector / ProjectTree / Diagnostics / Console
- [x] 未引入性能监视器 / Probe 基础设施
- [x] 所有新文件 ≤100 行
- [x] file-tree.md 已更新
