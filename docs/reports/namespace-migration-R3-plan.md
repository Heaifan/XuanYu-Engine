# R3 namespace 迁移计划

> **创建**：2026-06-24 | **类型**：迁移计划 | **生命周期**：R3-Z 完成后合并到 `changelog.md` 后删除
> **R3-1 状态**：✅ 已完成（2026-06-24）
> **R3-2 状态**：✅ 已完成（2026-06-24）
> **R3-3A 状态**：✅ 已完成（2026-06-24，Editor 项目 namespace 迁移，不碰 XAML）

---

## 当前状态

R0/R1（品牌层 ✅）+ R2（工程外壳 ✅）+ R2B（占位清理 ✅）+ R2C（docs 清理 ✅）已完成。  
C# namespace / using / x:Class 中的 `FluidWarfare` 残留为 **R3 唯一未动部分**。

---

## 1. namespace FluidWarfare.* 统计

| 项目 | .cs 文件数 | namespace 声明数 | 说明 |
|------|-----------|----------------|------|
| `Core` | 9 | 9 | 基础值对象、日志、数学、时间、结果、身份 |
| `Engine` | 8 | 8 | World 实体、组件、位置 |
| `Project` | 17 | 17 | 加载、内容、元数据、路径、验证、Transform |
| `Bridge.ProjectEngine` | 2 | 2 | 世界种子 |
| `Render` | 47 | ~30 | 相机、坐标、场景、选择（无 Vulkan） |
| `Render.Vulkan` | 154 | ~120 | Instance/Device/Surface/Clear/Swapchain/Scene3D 全层 |
| `Editor` | 60 | ~40 | 输入、绑定、Transform、层级树、选择、地面 |
| `Editor.Windows` | 260 | ~200 | UI 面板、Shell Route、Viewport、XAML code-behind |
| `Tests` | 73 | ~65 | 测试 namespace 随被测模块 |

**总计：~630 处 namespace 声明，分布在 ~630 个 .cs 文件。**

---

## 2. using FluidWarfare.* 统计

| 位置 | 数量 | 说明 |
|------|------|------|
| `GlobalUsings.cs` | **90 条** | EditorShell 组合根的全局 using |
| 各 .cs 文件中分散 | ~800+ 条 | 每个文件 ~1-3 条 |

using 随 namespace 迁移自动同步——修改 namespace 后，同文件的 using 引用其他模块时需同步更新。
**`GlobalUsings.cs` 的 90 条 using 必须在 R3-3 时一次性更新**。

---

## 3. x:Class="FluidWarfare.*" 清单

共 **16 处**，全部在 `Editor.Windows` 模块：

| # | 文件 | x:Class |
|---|------|---------|
| 1 | `App.axaml` | `FluidWarfare.Editor.Windows.App` |
| 2 | `MainWindow.axaml` | `FluidWarfare.Editor.Windows.MainWindow` |
| 3 | `Shell/EditorShell.axaml` | `FluidWarfare.Editor.Windows.Shell.EditorShell` |
| 4 | `About/AboutFluidWarfareWindow.axaml` | `FluidWarfare.Editor.Windows.About.AboutFluidWarfareWindow` |
| 5 | `Preferences/EditorPreferencesWindow.axaml` | `FluidWarfare.Editor.Windows.Preferences.EditorPreferencesWindow` |
| 6 | `Panels/DebugDock/DebugDockPanel.axaml` | `FluidWarfare.Editor.Windows.Panels.DebugDock.DebugDockPanel` |
| 7 | `Panels/HierarchyVisual/HierarchyNodeRow.axaml` | `FluidWarfare.Editor.Windows.Panels.HierarchyVisual.HierarchyNodeRow` |
| 8 | `Panels/Inspector/InspectorPanel.axaml` | `FluidWarfare.Editor.Windows.Panels.Inspector.InspectorPanel` |
| 9 | `Panels/LeftDock/ProjectWorldDockPanel.axaml` | `FluidWarfare.Editor.Windows.Panels.LeftDock.ProjectWorldDockPanel` |
| 10 | `Panels/Logging/LogPanel.axaml` | `FluidWarfare.Editor.Windows.Panels.Logging.LogPanel` |
| 11 | `Panels/ProjectContentTree/ProjectContentTreePanel.axaml` | `FluidWarfare.Editor.Windows.Panels.ProjectContentTree.ProjectContentTreePanel` |
| 12 | `Panels/Status/StatusBarPanel.axaml` | `FluidWarfare.Editor.Windows.Panels.Status.StatusBarPanel` |
| 13 | `Panels/Viewport/ViewportPlaceholderPanel.axaml` | `FluidWarfare.Editor.Windows.Panels.Viewport.ViewportPlaceholderPanel` |
| 14 | `Panels/Viewport/Tools/ViewportToolPalette.axaml` | `FluidWarfare.Editor.Windows.Panels.Viewport.Tools.ViewportToolPalette` |
| 15 | `Panels/Viewport/VulkanViewportHostPanel.axaml` | `FluidWarfare.Editor.Windows.Panels.Viewport.VulkanViewportHostPanel` |
| 16 | `Panels/WorldHierarchy/WorldHierarchyTreePanel.axaml` | `FluidWarfare.Editor.Windows.Panels.WorldHierarchy.WorldHierarchyTreePanel` |

**风险**：每个 `x:Class` 对应一对 `.axaml` + `.axaml.cs` 文件。  
修改时必须同步更新：

- `.axaml` 中 `x:Class` 属性
- `.axaml.cs` 中 `namespace` 声明
- 类型引用（构造调用、菜单绑定等）
- Avalonia 编译生成的 `: axaml` 分部类

**强制**：x:Class + namespace + 类型名必须在同一提交中修改，不允许拆分。

---

## 4. FluidWarfare 类型名 / 文件名残留

| 位置 | 当前名 | 新名 | 类型 |
|------|--------|------|------|
| `Editor.Windows/About/` | `AboutFluidWarfareWindow` | `AboutXuanYuEngineWindow` | 类名 + 文件名 + x:Class |
| `Editor.Windows/About/` | `AboutFluidWarfareWindow.axaml` | `AboutXuanYuEngineWindow.axaml` | XAML 文件名 |
| `Editor.Windows/About/` | `AboutFluidWarfareWindow.axaml.cs` | `AboutXuanYuEngineWindow.axaml.cs` | code-behind 文件名 |

依赖此类型的位置：`Shell/EditorShell.axaml` 中的 `x:Name="AboutFluidWarfareMenuItem"` 等。

---

## 5. Avalonia 高风险点清单

| 风险 | 影响 | 等级 | 缓解措施 |
|------|------|------|----------|
| x:Class + namespace 不同步 | build 崩溃 | 🔴 | 同一提交同步改 |
| XAML 编译生成类路径断裂 | build 崩溃 | 🔴 | 同上 |
| `clr-namespace:FluidWarfare.*` xmlns 引用 | 绑定断裂 | 🟡 | 扫描 .axaml 中的 xmlns |
| `AboutFluidWarfareWindow` 构造调用 | 编译错误 | 🟢 | 只有 1 处引用 |
| `GlobalUsings.cs` 90 条 using | 跨文件影响 | 🟡 | R3-3 一次性更新 |

**策略**：R3-3（Editor.Windows）是整个迁移中唯一高风险阶段。  
建议在独立分支上完成全部 x:Class + namespace + 类型名更改后，一次 build 验证通过再合入。

---

## 6. R3 映射表

| 旧 namespace | 新 namespace | 模块 | 阶段 |
|-------------|-------------|------|------|
| `FluidWarfare.Core.*` | `XuanYu.Engine.Core.*` | Core | R3-1 |
| `FluidWarfare.Engine.*` | `XuanYu.Engine.*` | Engine | R3-1 |
| `FluidWarfare.Project.*` | `XuanYu.Engine.Project.*` | Project | R3-1 |
| `FluidWarfare.Bridge.ProjectEngine.*` | `XuanYu.Engine.Bridge.ProjectEngine.*` | Bridge | R3-1 |
| `FluidWarfare.Render.*` | `XuanYu.Engine.Render.*` | Render | R3-2 |
| `FluidWarfare.Render.Vulkan.*` | `XuanYu.Engine.Render.Vulkan.*` | Render.Vulkan | R3-2 |
| `FluidWarfare.Editor.*` | `XuanYu.Engine.Editor.*` | Editor | R3-3 |
| `FluidWarfare.Editor.Windows.*` | `XuanYu.Engine.Editor.Windows.*` | Editor.Windows | R3-3 |
| `FluidWarfare.Tests.*` | `XuanYu.Engine.Tests.*` | Tests | R3-4 |

**特殊规则**：`FluidWarfare.Engine.*` → `XuanYu.Engine.*`（注意：没有 `.Engine` 后缀）。

---

## 7. 不属于 R3 的保留项

| 残留 | 保留阶段 | 原因 |
|------|----------|------|
| `EditorSettingsPath.AppFolderName = "FluidWarfare"` | **R4** | 用户 %APPDATA% 目录，需迁移逻辑 |
| `changelog.md` 中历史记录 | **永久** | 变更记录，保留原貌 |
| `docs/naming-XuanYu-Engine.md` 迁移说明 | **永久** | 命名规范文档 |
| 其他 `docs/` 长期文档中的引用 | **永久** | 项目宪法类文档 |
| GitHub 仓库名 `Heaifan/FluidWarfare` | **外部** | 远程仓库重命名 |
| 本地根目录 `fluidwarfare/` 文件夹名 | **外部** | 磁盘上文件夹名 |

---

## 8. R3 分阶段迁移建议

### ✅ R3-1 — 底层模块（Core / Engine / Project / Bridge）

```
文件数：~36
namespace 更改：~36 处
using 更改：~209 处（跨全仓引用）
XAML 风险：无
风险等级：🟢 低 — 已完成 ✅
```

操作：
1. Core：`FluidWarfare.Core.*` → `XuanYu.Engine.Core.*`（9 文件 namespace + 134 文件 using）
2. Engine：`FluidWarfare.Engine.*` → `XuanYu.Engine.*`（8 文件 namespace + 37 文件 using，注意无后缀）
3. Project：`FluidWarfare.Project.*` → `XuanYu.Engine.Project.*`（17 文件 namespace + 35 文件 using）
4. Bridge：`FluidWarfare.Bridge.ProjectEngine.*` → `XuanYu.Engine.Bridge.ProjectEngine.*`（2 文件 namespace + 3 文件 using）
5. build: 0 Error / test: 629/630 (1 flaky)
6. R3-1 范围内旧 namespace/using 全部清零 ✅

### ✅ R3-2 — Render 层（Render + Render.Vulkan）

```
文件数：~201
namespace 更改：~201 处
using 更改：~147 处（跨全仓引用）
XAML 风险：无
风险等级：🟢 低 — 已完成 ✅
```

操作：
1. Render：47 文件 namespace + 147 文件跨项目 using
2. Render.Vulkan：154 文件 namespace（using 已被 Render 覆盖）
3. 修复 1 处完全限定类型引用 `FluidWarfare.Render.Camera.SceneCameraPose`
4. 相机白名单文件 namespace 正确迁移：`XuanYu.Engine.Render.Camera.*`
5. build: 0 Error / test: 629/630 (1 flaky)
6. R3-2 范围内旧 namespace/using 全部清零 ✅

### R3-3 — Editor 层（Editor + Editor.Windows）⚠️ 高风险（拆三段）

#### ✅ R3-3A — Editor 项目 namespace（已完成）

```
60 文件 namespace + 57 文件跨项目 using
build: 0 Error / test: 629/630
Editor namespace 清零 ✅
Editor.Windows namespace 保持不动 ✅
```

#### ⬜ R3-3B — Editor.Windows 非 XAML C# namespace（待做）

#### ⬜ R3-3C — Avalonia XAML / x:Class / 类型名迁移（高风险）

```
文件数：~320
namespace 更改：~240 处
x:Class 更改：16 处
类型名更改：1 处（AboutFluidWarfareWindow）
XAML 风险：🔴
风险等级：🔴 高
```

步骤（**必须同一提交完成 2-7**）：
1. Editor（非 UI）：60 文件，纯 C# namespace 迁移
2. Editor.Windows code-behind：~260 文件，namespace 迁移
3. 全部 16 个 `.axaml` 的 `x:Class` 同步更新
4. 全部 `.axaml` 中 `clr-namespace:` 引用更新
5. `AboutFluidWarfareWindow` → `AboutXuanYuEngineWindow` 重命名（类 + 文件 + x:Class）
6. `GlobalUsings.cs` 中 90 条 using 更新
7. 更新 `EditorShell.axaml` 中的菜单引用（`AboutFluidWarfareMenuItem`）
8. build 验证
9. 提交

**禁止**：将 namespace 和 x:Class 分拆到两个提交。

### R3-4 — Tests

```
文件数：~73
namespace 更改：~65 处
XAML 风险：无
风险等级：🟢 低
```

步骤：
1. 73 文件 namespace 迁移
2. 更新 `CodeFileBudgetTests.cs` 中 `StartsWith("FluidWarfare.Tests"` 等路径前缀
3. 更新 `ProjectDependencyDirectionTests.cs` 中依赖映射
4. build + test

### R3-Z — 全仓残留审计

```
验收项：
- namespace FluidWarfare.* = 0
- using FluidWarfare.* = 0（除 R4 保留）
- x:Class FluidWarfare.* = 0
- EditorSettingsPath 仍保留 = 1（R4）
- changelog.md 历史记录保留原貌
```

---

## 9. 执行顺序

```
R3-1 ── build+test ──→ R3-2 ── build+test ──→ R3-3 ── build+test ──→ R3-4 ── build+test ──→ R3-Z
🟢低风险      🟢低风险       🔴高风险         🟢低风险
```

每个阶段独立提交。不允许跨阶段合并。  
R3-3 必须等 R3-1/2 完成后执行（Editor 依赖底层模块）。  
R3-4 最后执行（Tests 依赖所有模块）。

---

## 附录：命令速查

```bash
# 统计 namespace
grep -rn "namespace FluidWarfare\\." --include="*.cs" | grep -v "/bin/" | grep -v "/obj/" | wc -l

# 统计 using
grep -rn "using FluidWarfare\\." --include="*.cs" | grep -v "/bin/" | grep -v "/obj/" | wc -l

# 列出 x:Class
grep -rn 'x:Class="FluidWarfare\\.' --include="*.axaml"

# 列出类型名
grep -rn "class.*FluidWarfare\\|record.*FluidWarfare" --include="*.cs" | grep -v "/bin/" | grep -v "/obj/"
```
