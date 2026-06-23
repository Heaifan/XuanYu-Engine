# 8.7.8 债务路线图

生成日期：2026-06-23
最后更新：2026-06-23（8.8-0：防回潮门禁已锁）

---

## 一句话现状

**8.7.8 全部完成。** EditorShell 白名单已移除，生产白名单锁死为 2 个相机算法文件。架构防回潮门禁（8.8-0）已通过测试锁定。

---

## 分类详情

### A 类 — 8.7.8 大件专项（2 个，必须单独拆）

这几个文件都超过 300 行，每个都需要一个专门的拆分阶段，不能顺手带掉。

| 文件 | 行数 | 为什么难拆 |
|------|------|-----------|
| ~~`EditorShell.axaml.cs`~~ | ~~491~~→**24** | **✅ 8.7.8-Z2 已从白名单移除**。当前为薄窗口入口，仅保留 InitializeComponent + 生命周期转发。所有业务逻辑已迁移到 Composition/Core/ + Composition/Wiring/ |
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

当前产线白名单预算：32（2 生产 + 30 测试；EditorShell 已移出白名单 ✅）
测试白名单：30 项（保留）
EditorShell 最终状态：**28 行（≤100 ✅），白名单已删除 ✅**

---

## 防回潮门禁（8.8-0）

```text
8.7.8-0 架构防回潮门禁 — 通过 ✅

新增测试：
├─ ProductionWhitelist_OnlyApproved     — 生产白名单精确锁死为 2 个相机文件
├─ GlobalUsings_Max100Lines             — GlobalUsings.cs ≤100 行
├─ EditorShellContext_Max95Lines        — EditorShellContext.cs ≤95 行
├─ EditorShell_NotInWhitelist           — EditorShell 不得回归白名单
└─ DirectoryWhitelist_RemainsZero       — 目录白名单保持 0

当前值：
├─ 生产白名单：2（SceneOrbitCameraMotion + SceneNavigationCameraMotion）
├─ 目录白名单：0
├─ GlobalUsings.cs：99 行
├─ EditorShellContext.cs：95 行
└─ EditorShell.axaml.cs：28 行
```
