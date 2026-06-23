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

### B 类 — 8.7.8 中风险专项（3 个，有风险但比 A 类轻）

| 文件 | 行数 | 注意事项 |
|------|------|---------|
| `WindowsViewportInputTranslator.cs` | 284 | 原始输入处理，NativeHost 相关 |
| `VulkanDeviceProbe.cs` | 288 | Device 诊断，Vulkan 核心 |
| `VulkanSurfaceProbe.cs` | 203 | Surface 诊断，Vulkan 核心 |

### C 类 — 可选 F-6 清尾（4 个，可尝试在本阶段收掉）

| 文件 | 行数 | 建议 |
|------|------|------|
| `EditorInputBindingSnapshot.cs` | 175 | 数据快照类，风险低 |
| `HostInfo/VulkanViewportHostPanel.axaml.cs` | 158 | UI 面板，已有 partial 模式可参考 |
| `Orbit/SceneOrbitCameraMotion.cs` | 202 | 相机运动，注意不改数学 |
| `Navigation/SceneNavigationCameraMotion.cs` | 173 | 导航相机，注意不改行为 |

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
