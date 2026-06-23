# 8.7.8-Z1 — 全仓最终验收与白名单债务总审计

审计日期：2026-06-23
审计类型：全仓最终验收（不改代码）

---

## 1. 8.7.8 总成果摘要

8.7.8 专项拆解完成 **8 个 Boss 文件 + 1 个组合根收口**：

| 阶段 | 目标 | 原始 | 当前 | 减少 |
|------|------|------|------|------|
| 8.7.8A-2 | WindowsViewportInputTranslator | 284 | 54（门面）+ 3 子组件 | ✅ |
| 8.7.8B-2 | VulkanSurfaceProbe | 203 | 66（门面）+ 98 InstanceScope | ✅ |
| 8.7.8B-4 | VulkanDeviceProbe | 288 | 77（门面）+ 61 InstanceScope + 80 Selector | ✅ |
| 8.7.8C-2 | GameProjectLoader | 392 | 82（门面）+ 89 ManifestReader + 100 FolderParser + 52 ExtensionParser | ✅ |
| 8.7.8D-2B | VulkanSwapchainProbe | 301 | 78（门面）+ 100 ContextScope + 46 DeviceSelector + 64 SurfaceQuery | ✅ |
| 8.7.8E-2B | VulkanClearProbe | 416 | 99（门面）+ 96 ContextScope + 42 DeviceSelector + 60 SurfaceQuery + 98 RenderTargetScope + 54 RenderSubmitScope | ✅ |
| 8.7.8F-2 | VulkanRenderContext | 476 | 92（门面）+ 78 Setup + 32 Selector + 30 Legacy | ✅ |
| 8.7.8G-2 | EditorPreferencesWindow | 587 | 78（门面）+ 77 Capture + 81 BindingList + 79 DraftHandler + 46 FormatText + 50 KeyMapper | ✅ |
| **8.7.8H** | **EditorShell** | **3,041** | **491 行（含 using 95）** | **✅ 组合根例外** |

---

## 2. 架构测试结果

| 测试 | 结果 |
|------|------|
| `ProductionFiles_Max100Lines` | ✅ 通过 |
| `TestFiles_Max180Lines` | ✅ 通过 |
| `AllDirectories_Max5CsFiles` | ✅ 通过 — 无目录超过 5 文件 |
| `NoForbiddenNames_FileAndType` | ✅ 通过 — 无 Manager/Helper/Utils/Processor/Factory/Creator |
| `WhitelistBudget_NotExceeded` | ✅ 通过 — 33/49 行 + 0/3 目录 |

---

## 3. 当前白名单清单

### 生产白名单（3 项）

| 文件 | 行数 | 类型 | 说明 |
|------|------|------|------|
| `EditorShell.axaml.cs` | 491 | **组合根例外** | H-5 收口审计确认，只出不进 |
| `SceneOrbitCameraMotion.cs` | 178 | 相机算法 | 不可硬拆，放弃拆分 |
| `SceneNavigationCameraMotion.cs` | 152 | 相机算法 | 不可硬拆，放弃拆分 |

### 测试白名单（30 项）

测试白名单允许保留，政策不强制拆分。30 项均已在 `CodeFileBudgetTests.cs` 登记。

### 目录白名单（0 项）

全部目录直属 .cs 均 ≤5 ✅

---

## 4. 合规扫描结果

| 检查项 | 结果 |
|--------|------|
| 生产文件 >100 行但未登记 | **无** ✅ |
| 测试文件 >180 行但未登记 | **无** ✅ |
| 目录直属 .cs >5 但未登记 | **无** ✅ |
| 含禁用命名文件/类型 | **无** ✅ |
| 行白名单预算超限 | **33/49** ✅（余 16 松弛）|
| 目录白名单预算超限 | **0/3** ✅ |

---

## 5. EditorShell 组合根例外说明

EditorShell 当前 491 行（含 using 95，body ~396），作为 8.7.8H 收口产物，从 3,041 行压缩了 **2,550 行（84%）**。

**保留理由：**

1. **组合根本质** — 必须承担控件查找、Route 创建、事件接线职责（~220 行）
2. **回调连接总站** — 持有 11 个 H-x 子路由 + 26 个 Build Route 引用
3. **Transform 管线不可硬拆** — ~37 行的管线创建/快照逻辑，收益低、风险高
4. **BuildInputRequest** — 30+ 参数的管家方法，Shell 唯一入口
5. **继续压到 ≤100** 需架构颠覆性变革，非小刀可完成

**后续规则：**
- ✅ 只出不进
- ❌ 不得新增业务逻辑
- ✅ 新增职责必须进 Route / 子模块

---

## 6. 文档同步检查

| 文档 | 状态 |
|------|------|
| `docs/whitelist-debt-roadmap-8.7.8.md` | ✅ 已更新 — EditorShell 标注为组合根例外 |
| `docs/CHANGELOG.md` | ✅ 已更新 — H-2A→H-5 全部记录 |
| `file-tree.md` | ✅ 已同步 — Shell 目录树含 15 子目录 |
| `docs/audit-EditorShell-closeout-8.7.8H-5.md` | ✅ 已创建 — 收口审计 |
| `docs/audit-EditorShell-remaining-8.7.8H-3.md` | ✅ 已存档 — 中期审计 |

---

## 7. 8.7.8 收口结论

| 维度 | 结论 |
|------|------|
| **Boss 文件治理** | ✅ **全部完成** — 8 个 Boss 全部拆完 |
| **目录治理** | ✅ **全部合规** — 无目录超过 5 文件 |
| **白名单治理** | ✅ **全部登记** — 3 生产 + 30 测试，无非登记违规 |
| **EditorShell 组合根** | ✅ **收口登记** — 只出不进 |
| **是否可以正式收口** | ✅ **可以** |
| **下一阶段建议** | **8.7.9 或 8.8**（见下节） |

---

## 8. 下一阶段建议

### 建议方向：8.7.9 — 白名单持续维护 + 小修小补

收口后不立即进入新的大拆分阶段，而是：

1. 持续监控白名单预算（当前 33/49，余量充足）
2. 相机算法文件（SceneOrbitCameraMotion 178 行 / SceneNavigationCameraMotion 152 行）如后期有重构计划可附带处理
3. 测试文件白名单保留，政策不变
4. 不引入新的 100+ 行生产文件

### 如果需要新功能：建议进入 8.8

8.8 应关注：
1. 新功能开发（非重构）
2. 如果在 Shell 中新增职责，必须新建 Route 文件，严禁塞入 EditorShell
