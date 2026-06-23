# 8.7.8 债务路线图

生成日期：2026-06-23
基于：8.7.7F-4C 完成后的状态

---

## 一句话现状

目录结构欠债全部还清 ✅。生产文件还剩 **13 个大的**，总行数约 **4500 行**。这些不适合在 8.7.7 继续拆，目标改为：分出轻重缓急，登记到 8.7.8 处理。

---

## 分类详情

### A 类 — 8.7.8 大件专项（6 个，必须单独拆）

这几个文件都超过 300 行，每个都需要一个专门的拆分阶段，不能顺手带掉。

| 文件 | 行数 | 为什么难拆 |
|------|------|-----------|
| `EditorShell.axaml.cs` | 969 | 编辑器主入口，牵一发动全身 |
| `EditorPreferencesWindow.axaml.cs` | 587 | 设置窗口，业务耦合重 |
| `VulkanRenderContext.cs` | 476 | 渲染上下文核心，调用链深 |
| `VulkanClearProbe.cs` | 416 | 清屏诊断，涉及多管道 |
| `GameProjectLoader.cs` | 392 | 项目加载，流程长 |
| `VulkanSwapchainProbe.cs` | 301 | Swapchain 诊断探针 |

### B 类 — 8.7.8 中风险专项（1 个，有风险但比 A 类轻）

| 文件 | 行数 | 注意事项 |
|------|------|---------|
| ~~`WindowsViewportInputTranslator.cs`~~ | ~~284~~ | ✅ **8.7.8A-2 已拆分** — 54 行门面 + 3 子组件 |
| `VulkanDeviceProbe.cs` | 288 | Device 诊断，Vulkan 核心 |
| ~~`VulkanSurfaceProbe.cs`~~ | ~~203~~ | ✅ **8.7.8B-2 已拆分** — 66 行门面 + 98 行 InstanceScope |

### C 类 — F-6 已清 ✅

| 文件 | 操作 | 当前行数 |
|------|------|----------|
| `EditorInputBindingSnapshot.cs` | Build 提取 → `.Build.cs` | 38 ✅ |
| `HostInfo/Panel/VulkanViewportHostPanel.axaml.cs` | SRP partial 提取 | 43 ✅ |
| `Orbit/SceneOrbitCameraMotion.cs` | 放弃（相机算法，不可硬拆） | 202 |
| `Navigation/SceneNavigationCameraMotion.cs` | 放弃（同上） | 173 |

### D 类 — 测试白名单（保留，不强制拆）

20 个测试文件，占线白名单但政策允许保留。继续保持在白名单中，不做处理。

---

## 后续计划

```
8.7.7F-6 最终收口：更新白名单预算、CHANGELOG、file-tree，关闭 8.7.7
8.7.8  专项拆解（A 类 6 个 + B 类 3 个，分多轮完成）
```

当前产线白名单预算：51（13 生产 + 20 测试 + 18 松弛）
测试白名单：20 项（保留）
