# 8.7.8 债务路线图

生成日期：2026-06-23
基于：8.7.7F-4C 完成后的状态

---

## 一句话现状

目录结构欠债全部还清 ✅。B 类 3 个中风险 + A 类 4 个已拆分，生产文件剩 **2 个大的**。

---

## 分类详情

### A 类 — 8.7.8 大件专项（2 个，必须单独拆）

这几个文件都超过 300 行，每个都需要一个专门的拆分阶段，不能顺手带掉。

| 文件 | 行数 | 为什么难拆 |
|------|------|-----------|
| `EditorShell.axaml.cs` | 622 | 编辑器主入口，牵一发动全身。H-2A/B/C 已拆 340 行，H-2D 拆 7 行 |
| ~~`EditorPreferencesWindow.axaml.cs`~~ | ~~587~~ | ✅ **8.7.8G-2 已拆分** — 78 行门面 + 77 行 Capture + 81 行 BindingList + 79 行 DraftHandler + Helpers |
| ~~`VulkanRenderContext.cs`~~ | ~~476~~ | ✅ **8.7.8F-2 已拆分** — 92 行门面 + 78 行 Setup + 32 行 Selector；死代码 Locked in Legacy |
| ~~`VulkanClearProbe.cs`~~ | ~~416~~ | ✅ **8.7.8E-2B 已拆分** — 99 行门面 + 96 行 ContextScope + 42 行 DeviceSelector + 60 行 SurfaceQuery + 98 行 RenderTargetScope + 54 行 RenderSubmitScope |
| ~~`GameProjectLoader.cs`~~ | ~~392~~ | ✅ **8.7.8C-2 已拆分** — 82 行门面 + 89 行 ManifestReader + 100 行 FolderParser + 52 行 ExtensionParser |
| ~~`VulkanSwapchainProbe.cs`~~ | ~~301~~ | ✅ **8.7.8D-2B 已拆分** — 78 行门面 + 100 行 ContextScope + 46 行 DeviceSelector + 64 行 SurfaceQuery |

### B 类 — 8.7.8 中风险专项 — 已全部清零 ✅

| 文件 | 行数 | 状态 |
|------|------|------|
| ~~`WindowsViewportInputTranslator.cs`~~ | ~~284~~ | ✅ **8.7.8A-2 已拆分** — 54 行门面 + 3 子组件 |
| ~~`VulkanSurfaceProbe.cs`~~ | ~~203~~ | ✅ **8.7.8B-2 已拆分** — 66 行门面 + 98 行 InstanceScope |
| ~~`VulkanDeviceProbe.cs`~~ | ~~288~~ | ✅ **8.7.8B-4 已拆分** — 77 行门面 + 61 行 InstanceScope + 80 行 Selector |

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
8.7.8  专项拆解（A 类 6 个，B 类 3 个已清，进入 A 类处理）
```

当前产线白名单预算：49（9 生产 + 20 测试 + 20 松弛，B 类 3 个 + G-2 全部移出白名单）
EditorShell 白名单保留中（725 行，H-2A 继续拆）
测试白名单：20 项（保留）
