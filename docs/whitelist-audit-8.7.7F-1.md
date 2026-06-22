# 8.7.7F-1 — 全仓白名单债务盘点

生成日期：2026-06-23
基线预算：67 行 / 11 目录
当前占用：49 行 / 8 目录

---

## 1. 文件白名单 — 完整清单

### 1.1 立即可清（≤100 行，白名单是历史残留）

| 路径 | 当前行数 | 到期阶段 | 操作 |
|------|----------|----------|------|
| `Editor.Windows\Panels\Inspector\InspectorPanel.axaml.cs` | **84** | 8.7.7A | 删除白名单 |
| `Editor.Windows\Panels\Viewport\NativeHost\WindowsVulkanViewportHostControl.cs` | **87** | 8.7.7A | 删除白名单 |
| `Render\Camera\SceneCameraPose.cs` | **99** | 8.7.7E | 删除白名单 |

**小计：3 项 → 预算 67→64**

### 1.2 小修可清（101～150 行，轻度压缩即可合规）

| 路径 | 当前行数 | 超标 | 到期阶段 | 建议 |
|------|----------|------|----------|------|
| `Editor\WorldHierarchy\WorldHierarchyTreeBuilder.cs` | 126 | 26 | 8.7.8 | 压缩空白/合并短行 |
| `Editor\Input\Actions\EditorInputActionCatalog.cs` | 148 | 48 | 8.7.8 | 拆出常量或工具函数 |
| `Instance\VulkanInstanceProbe.cs` | 122 | 22 | 8.7.7C | 压缩或提取辅助方法 |
| `Validation\VulkanValidationAvailabilityProbe.cs` | 118 | 18 | 8.7.7C | 压缩 |
| `Validation\VulkanDebugMessengerScope.cs` | 133 | 33 | 8.7.7C | 压缩或拆前景逻辑 |
| `Project\Content\GameContentFileScanner.cs` | 130 | 30 | 8.7.8 | 压缩 |
| `Engine\World\WorldState.cs` | 121 | 21 | 8.7.8 | 压缩 |
| `Editor.Windows\Panels\WorldHierarchy\WorldHierarchyTreeIndex.cs` | 112 | 12 | 8.7.7D | 压缩 |
| `Editor.Windows\Panels\ProjectContentTree\ProjectContentNodeView.cs` | 114 | 14 | 8.7.7D | 压缩 |

**小计：9 项 → 全清则预算 64→55**

### 1.3 中等债务（151～300 行，需要 SRP 拆分或压缩）

| 路径 | 当前行数 | 超标 | 到期阶段 | 建议 |
|------|----------|------|----------|------|
| `Panels\DebugDock\DebugDockPanel.axaml.cs` | 145 | — | 8.7.7A | 实际≤150，归入上一档 |
| `Panels\Viewport\ViewportPlaceholderPanel.axaml.cs` | 189 | 89 | 8.7.7A | 提取子视图 |
| `Panels\Viewport\VulkanViewportHostPanel.axaml.cs` | 158 | 58 | 8.7.7A | 提取子视图 |
| `Panels\Viewport\Input\WindowsViewportInputTranslator.cs` | 284 | 184 | 8.7.7A | 按输入类型拆 |
| `Editor\Input\Runtime\EditorInputBindingSnapshot.cs` | 175 | 75 | 8.7.8 | 按绑定域拆 |
| `Camera\VulkanCameraMatrices.cs` | 189 | 89 | 8.7.7C | 按矩阵类型拆 |
| `Camera\VulkanSceneRayBuilder.cs` | 167 | 67 | 8.7.7C | 提取射线分类 |
| `Render\Camera\SceneOrbitCameraMotion.cs` | 202 | 102 | 8.7.7E | 按运动模式拆 |
| `Render\Camera\Navigation\SceneNavigationCameraMotion.cs` | 173 | 73 | 8.7.7E | 按导航模式拆 |
| `Surface\VulkanSurfaceProbe.cs` | 203 | 103 | 8.7.7C | 按查询/选择拆 |
| `Device\VulkanDeviceProbe.cs` | 288 | 188 | 8.7.7C | 按设备属性/队列/扩展拆 |
| `Swapchain\VulkanSwapchainProbe.cs` | 301 | 201 | 8.7.7C | 按创建/查询/选择拆 |
| `Editor\Input\Actions\EditorInputActionCatalog.cs` | 148 | — | 8.7.8 | 归入小修档 |

**小计：12 项**（部分可归入上档或下档）

### 1.4 大债务（>300 行，需要独立阶段）

| 路径 | 当前行数 | 超标 | 到期阶段 | 建议 |
|------|----------|------|----------|------|
| `Clear\VulkanClearProbe.cs` | 416 | 316 | 8.7.7C | 按 Clear 管道阶段拆分 |
| `Context\VulkanRenderContext.cs` | 476 | 376 | 8.7.7C | 全场景 VRenderContext SRP 拆分 |
| `Preferences\EditorPreferencesWindow.axaml.cs` | 587 | 487 | 8.7.7D | 按首选项页签拆 |
| `Shell\EditorShell.axaml.cs` | 969 | 869 | 8.7.6.8 | 已有 Route 拆解，继续收口 |

**小计：4 项 → 需要独立阶段，不在 F 处理**

### 1.5 测试文件（白名单允许保留，不强制拆）

| 路径 | 当前行数 | 红线 | 建议 |
|------|----------|------|------|
| 20 个测试 .cs 文件 | 均≤180 | 180 行软上限 | 保留，不强制拆 |

**小计：20 项 — 不处理**

---

## 2. 目录白名单 — 完整清单

| 路径 | 文件数 | 红线 | 超标 | 建议 | 优先级 |
|------|--------|------|------|------|--------|
| `Render.Vulkan\Validation` | 7 | ≤5 | 2 | 拆子目录：Availability/Callback/Scope | F-3 |
| `Render\Camera` | 7 | ≤5 | 2 | 拆子目录：Motion/Pose/Navigation | F-3 |
| `Render\ViewportNavigation` | 9 | ≤5 | 4 | 拆子目录：Layout/Command/Response | F-3 |
| `Editor.Windows\Panels\Viewport` | 8 | ≤5 | 3 | 拆子目录：Input/NativeHost/Render | F-3 |
| `Editor.Windows\Panels\WorldHierarchy` | 8 | ≤5 | 3 | 拆入子目录或减 file 数 | F-4 |
| `Editor.Windows\Panels\ProjectContentTree` | 6 | ≤5 | 1 | 移 1 文件到子目录 | F-3 |
| `Editor.Windows\Viewport\Transform\Gizmo` | 8 | ≤5 | 3 | 拆子目录：HitTest/DrawList/State | F-4 |
| `Editor.Windows\Viewport\Transform\Drag` | 6 | ≤5 | 1 | 移 1 文件到子目录 | F-3 |

---

## 3. 汇总与分层

### F-2 — 立即可清（3 项）
删除白名单条目，不修改代码：
- InspectorPanel.axaml.cs ✅
- WindowsVulkanViewportHostControl.cs ✅
- SceneCameraPose.cs ✅

### F-3 — 小型文件/目录债务（≤150 行 / 6～7 文件）
9 文件 + 5 目录：轻度压缩或移动即可。

### F-4 — 中型债务（151～300 行 / 8 文件目录）
12 文件 + 3 目录：需要 SRP 拆分或子目录化。

### F-5 — 大债务 → 转入 8.7.8（>300 行）
4 文件：EditorShell(970)、ClearProbe(416)、RenderContext(476)、EditorPreferences(587)

### F-6 — 最终收口
- CodeFileBudgetTests 白名单预算更新
- CHANGELOG.md / file-tree.md 同步
- git commit + push

---

## 4. F 阶段执行建议

```
F-1  盘点              ✅ 当前文档
F-2  立即可清            3 文件：删除白名单，不碰代码
F-3  小修                9 文件 + 5 目录：轻度压缩/移动
F-4  中等                12 文件 + 3 目录：SRP 拆分 + 子目录化
F-5  大债务→8.7.8       4 文件：登记但不强拆
F-6  收口                白名单预算 + 文档 + git
```

核心原则：
- **能删就删**：已经合规的条目优先删除
- **能移就移**：目录超标的先移文件再拆
- **小拆不解 God**：150 行以内的拆完，300+ 的登记到 8.7.8
- **白名单预算只在成功删除后降**：每删一项减对应预算
