# 8.8-R3-0 — namespace / x:Class / 类型名迁移审计

创建时间：2026-06-24
阶段性质：**审计阶段，不修改代码**

---

## 1. 当前状态

R0/R1（品牌层）+ R2（工程外壳）+ R2B（占位清理）已完成。  
C# namespace / using / x:Class / 类型名中的 `FluidWarfare` 残留尚未迁移。

---

## 2. namespace FluidWarfare.* 统计

总出现次数：**629 处**，分布在 9 个项目。

| 项目 | .cs 文件数 | namespace 种类 | 说明 |
|------|-----------|---------------|------|
| `Core` | 9 | 5 | 基础值对象、日志、数学 |
| `Engine` | 8 | 3 | World 组件 |
| `Project` | 17 | 5 | 加载、内容、元数据 |
| `Bridge.ProjectEngine` | 2 | 1 | 世界种子 |
| `Render` | 47 | 10 | 相机、坐标、选择 |
| `Render.Vulkan` | 154 | 28 | Vulkan 全层（最大模块） |
| `Editor` | 60 | 12 | 输入、变换、层级树 |
| `Editor.Windows` | 260 | 58 | UI（含 XAML，最高风险） |
| `Tests` | 73 | 30 | 测试（namespace 依附于被测模块） |

---

## 3. using FluidWarfare.* 统计

总出现次数：**约 900+ 处**（含 GlobalUsings.cs 中的 90 个全局 using）。  
using 随 namespace 同步迁移，不需要单独逐一修改——修改 namespace 后，同文件的 using 会自动匹配或通过 IDE 全局替换。

**关键点**：`GlobalUsings.cs`（90 个全局 using）必须在 Editor.Windows 模块迁移时同步更新。它会影响 EditorShell 组合根下的所有文件。

---

## 4. x:Class="FluidWarfare.*" 统计

总出现次数：**16 处**，分布在 16 个 `.axaml` 文件。

| 序号 | x:Class | 文件 |
|------|---------|------|
| 1 | `FluidWarfare.Editor.Windows.App` | `App.axaml` |
| 2 | `FluidWarfare.Editor.Windows.MainWindow` | `MainWindow.axaml` |
| 3 | `FluidWarfare.Editor.Windows.Shell.EditorShell` | `Shell/EditorShell.axaml` |
| 4 | `FluidWarfare.Editor.Windows.About.AboutFluidWarfareWindow` | `About/AboutFluidWarfareWindow.axaml` |
| 5 | `FluidWarfare.Editor.Windows.Preferences.EditorPreferencesWindow` | `Preferences/EditorPreferencesWindow.axaml` |
| 6 | `FluidWarfare.Editor.Windows.Panels.DebugDock.DebugDockPanel` | `Panels/DebugDock/DebugDockPanel.axaml` |
| 7 | `FluidWarfare.Editor.Windows.Panels.HierarchyVisual.HierarchyNodeRow` | `Panels/HierarchyVisual/HierarchyNodeRow.axaml` |
| 8 | `FluidWarfare.Editor.Windows.Panels.Inspector.InspectorPanel` | `Panels/Inspector/InspectorPanel.axaml` |
| 9 | `FluidWarfare.Editor.Windows.Panels.LeftDock.ProjectWorldDockPanel` | `Panels/LeftDock/ProjectWorldDockPanel.axaml` |
| 10 | `FluidWarfare.Editor.Windows.Panels.Logging.LogPanel` | `Panels/Logging/LogPanel.axaml` |
| 11 | `FluidWarfare.Editor.Windows.Panels.ProjectContentTree.ProjectContentTreePanel` | `Panels/ProjectContentTree/ProjectContentTreePanel.axaml` |
| 12 | `FluidWarfare.Editor.Windows.Panels.Status.StatusBarPanel` | `Panels/Status/StatusBarPanel.axaml` |
| 13 | `FluidWarfare.Editor.Windows.Panels.Viewport.ViewportPlaceholderPanel` | `Panels/Viewport/ViewportPlaceholderPanel.axaml` |
| 14 | `FluidWarfare.Editor.Windows.Panels.Viewport.Tools.ViewportToolPalette` | `Panels/Viewport/Tools/ViewportToolPalette.axaml` |
| 15 | `FluidWarfare.Editor.Windows.Panels.Viewport.NativeHost.VulkanViewportHostPanel` | `Panels/Viewport/HostInfo/Panel/VulkanViewportHostPanel.axaml` |
| 16 | `FluidWarfare.Editor.Windows.Panels.WorldHierarchy.WorldHierarchyTreePanel` | `Panels/WorldHierarchy/WorldHierarchyTreePanel.axaml` |

**风险说明**：每个 x:Class 对应一个 `.axaml.cs` code-behind 文件。修改 x:Class 时，必须同步修改：

- `.axaml` 中的 `x:Class` 属性
- `.axaml.cs` 中的 `namespace` 声明 + 类名（如适用）
- 任何代码中对这些类的引用（`new AboutFluidWarfareWindow()` 等）
- Avalonia 编译生成的 `: axaml` 分部类绑定

---

## 5. FluidWarfare 类型名 / 文件名残留

| 当前名 | 类型 | 未来名 | 阶段 |
|--------|------|--------|------|
| `AboutFluidWarfareWindow` | 类名 + 文件名 | `AboutXuanYuEngineWindow` | R3-3 |
| `AboutFluidWarfareWindow.axaml` | XAML 文件 | `AboutXuanYuEngineWindow.axaml` | R3-3 |
| `AboutFluidWarfareWindow.axaml.cs` | code-behind | `AboutXuanYuEngineWindow.axaml.cs` | R3-3 |

文件名迁移要求在 R3-3 中同步处理。建议在 Editor.Windows 模块迁移时一并完成。

---

## 6. R3 命名映射表

| 旧 namespace | 新 namespace | 模块 |
|-------------|-------------|------|
| `FluidWarfare.Core.*` | `XuanYu.Engine.Core.*` | Core |
| `FluidWarfare.Engine.*` | `XuanYu.Engine.*` | Engine |
| `FluidWarfare.Project.*` | `XuanYu.Engine.Project.*` | Project |
| `FluidWarfare.Bridge.ProjectEngine.*` | `XuanYu.Engine.Bridge.ProjectEngine.*` | Bridge |
| `FluidWarfare.Render.*` | `XuanYu.Engine.Render.*` | Render |
| `FluidWarfare.Render.Vulkan.*` | `XuanYu.Engine.Render.Vulkan.*` | Render.Vulkan |
| `FluidWarfare.Editor.*` | `XuanYu.Engine.Editor.*` | Editor |
| `FluidWarfare.Editor.Windows.*` | `XuanYu.Engine.Editor.Windows.*` | Editor.Windows |
| `FluidWarfare.Tests.*` | `XuanYu.Engine.Tests.*` | Tests |

**规则**：新 namespace = 新项目目录名 + 原有子命名空间。  
例如：`FluidWarfare.Render.Vulkan.Clear.Probe` → `XuanYu.Engine.Render.Vulkan.Clear.Probe`

---

## 7. Avalonia 风险点

| 风险项 | 影响范围 | 风险等级 |
|--------|----------|----------|
| `x:Class` + `namespace` 不同步 | build 崩溃 | 🔴 高 |
| XAML 编译生成类路径断裂 | build 崩溃 | 🔴 高 |
| `xmlns:xxx="clr-namespace:FluidWarfare.*"` | 绑定断裂 | 🟡 中 |
| `AboutFluidWarfareWindow` 构造引用 | 编译错误 | 🟢 低（只有 1 处） |
| `GlobalUsings.cs` 中的 90 个 `using FluidWarfare.*` | 跨文件影响 | 🟡 中 |

**关键策略**：Editor.Windows 模块必须在单个原子提交中完成 namespace + x:Class 同步更改。  
如果 x:Class 改了但 namespace 没改（或反序），build 中间状态不可编译。

**推荐的 Editor.Windows 迁移步骤**：

1. 修改所有 `.axaml.cs` 的 `namespace` 声明
2. 同步修改所有 `.axaml` 的 `x:Class` 属性
3. 同步修改所有 `xmlns:xxx="clr-namespace:..."` 引用
4. 修改 `GlobalUsings.cs` 中的 90 个全局 using
5. 重命名 `AboutFluidWarfareWindow` 类/文件
6. 修改所有对该类型的引用（菜单创建等）
7. 立即 build 验证
8. 提交

**禁止**：将 namespace 和 x:Class 分拆到两个提交。

---

## 8. 不属于 R3 的残留

| 残留 | 保留到 | 原因 |
|------|--------|------|
| `EditorSettingsPath.AppFolderName = "FluidWarfare"` | **R4** | 用户 %APPDATA% 目录名，需要迁移逻辑 |
| CHANGELOG 中 130+ 处 `FluidWarfare` 历史记录 | **永久** | 历史记录，保留原貌 |
| `docs/naming-XuanYu-Engine.md` 中 25 处 `FluidWarfare` | **永久** | 迁移说明文档 |
| 其他 docs/*.md 中 `FluidWarfare` 历史引用 | **永久** | 历史记录或迁移说明 |
| GitHub 仓库名 `Heaifan/FluidWarfare` | **外部** | 远程仓库重命名，非代码迁移 |
| 本地根目录 `fluidwarfare/` | **外部** | 本地文件夹名，不影响 build |

---

## 9. R3 分阶段迁移建议

| 阶段 | 模块 | 文件数 | namespace 数 | XAML？ | 风险 |
|------|------|--------|-------------|--------|------|
| **R3-1** | Core + Engine + Project + Bridge | ~36 | 14 | 无 | 🟢 低 |
| **R3-2** | Render + Render.Vulkan | ~201 | 38 | 无 | 🟢 低 |
| **R3-3** | Editor + Editor.Windows | ~320 | 70 | **16 个 x:Class** | 🔴 **高** |
| **R3-4** | Tests | ~73 | 30 | 无 | 🟢 低 |
| **R3-Z** | 全仓残留审计 | — | — | — | 验收 |

**R3-3 是唯一高风险的阶段**，原因：

1. XAML 的 `x:Class` 和 code-behind `namespace` 必须严格同步
2. `clr-namespace:FluidWarfare.Editor.Windows.Shell` 等 xmlns 引用需要同步更新
3. `GlobalUsings.cs` 中的 90 个全局 using 必须一次性更新
4. `AboutFluidWarfareWindow` 类名+文件名+构造引用需要同步变更

建议 R3-3 的前几步先在 **独立工作树或分支** 上验证，确保 build 通过后再合并。

---

## 10. 各阶段具体操作步骤

### R3-1：底层模块（Core / Engine / Project / Bridge）

```
1. Core：修改 9 个 .cs 文件的 namespace 声明
   FluidWarfare.Core.* → XuanYu.Engine.Core.*
2. Engine：修改 8 个 .cs 文件的 namespace 声明
   FluidWarfare.Engine.* → XuanYu.Engine.*
3. Project：修改 17 个 .cs 文件的 namespace + using
4. Bridge：修改 2 个 .cs 文件
5. build + test
```

**注意**：FluidWarfare.Engine → XuanYu.Engine（无 `.Engine` 后缀），
与其他模块的 `XuanYu.Engine.XXX` 模式不同。

### R3-2：Render 层

```
1. Render：47 个 .cs 文件，namespace 映射 + using 同步
2. Render.Vulkan：154 个 .cs 文件，同上
3. 更新 GlobalUsings.cs 中属于 Render 层的 using（约 30 条）
4. build + test
```

### R3-3：Editor 层（高风险）

```
1. Editor（非 UI）：60 个 .cs 文件，namespace 迁移
2. GlobalUsings.cs：90 个 using 全部更新
3. Editor.Windows code-behind：~260 个 .cs 文件 namespace 迁移
4. 所有 16 个 .axaml x:Class 同步更新
5. 所有 .axaml 中 clr-namespace 引用更新
6. AboutFluidWarfareWindow → AboutXuanYuEngineWindow 重命名
7. 更新菜单引用（EditorShell.axaml 中的 AboutFluidWarfareMenuItem）
8. build + test
```

**强制要求**：步骤 2-7 必须在同一个提交中完成。不要拆分。

### R3-4：Tests

```
1. 73 个 .cs 文件 namespace 迁移
2. 测试中的 using 随 namespace 自动同步
3. 更新 CodeFileBudgetTests.cs 中白名单前缀检查（不影响 build）
4. 更新 ProjectDependencyDirectionTests.cs 中的依赖映射（不影响 build）
5. build + test
```

### R3-Z：残留审计

```
1. 确认所有 namespace 已迁移到 XuanYu.Engine.*
2. 确认所有 using 已同步
3. 确认所有 x:Class 已迁移
4. 确认 GlobalUsings.cs 无 FluidWarfare 引用
5. 确认 EditorSettingsPath 仍保留（R4）
6. 确认 CHANGELOG 历史记录保留原貌
7. 输出最终残留清单
```

---

## 11. 执行顺序图

```
R3-1 (Core/Engine/Project/Bridge) ──────┐
                                         ├──→ build+test → R3-2
                                        OK
                                           ↓
R3-2 (Render/Render.Vulkan) ────────────┐
                                         ├──→ build+test → R3-3
                                        OK
                                           ↓
R3-3 (Editor + Editor.Windows + XAML) ──┐
                                         ├──→ build+test → R3-4
                                        OK
                                           ↓
R3-4 (Tests) ──────────────────────────┐
                                        ├──→ build+test
                                       OK
                                          ↓
R3-Z (残留审计) ────────────────────────→ 验收通过
```

每个阶段独立提交。不允许跨阶段合并提交。  
R3-2 必须在 R3-1 之后执行（因为 Render 依赖 Core）。  
R3-3 必须在 R3-1/2 之后执行（Editor 依赖所有底层模块）。  
R3-4 最后（Tests 依赖所有模块）。
