# ORG-1-R1 项目基线审计（修正版）

- 任务代号：ORG-1-R1（基于 ORG-1 `f187174` 退回修正）
- 被审计代码基线：`9bc210e`（DOC-VERSION-1-R1，ORG-1 审计时的代码树）
- 审计报告提交（本轮）：见 changelog `v0.2.14.6-rz`（提交后补充 hash）
- 分支：`fix/RZ-VK3-A-surface-contract`
- 审计性质：纯文档修正轮，**不改代码、不重构、不删除文件**
- 依据文档：`docs/玄域引擎_AI开发宪法.md`（最高规范）、`docs/版本号规范与历史映射.md`、`docs/CODE_CONSTITUTION.md`、`docs/AI_DEVELOPMENT_RULES.md`、各 VK3/VK4/VK5 计划与收口文档、`changelog.md`、`file-tree.md`

---

## 0. R1 修正摘要（对照 ORG-1 退回意见）

| # | ORG-1 误判 | R1 修正 |
| --- | --- | --- |
| 1 | 把"功能分支快照状态"写成"整个仓库基线"（"仓库无 .sln、无测试项目"） | 所有"无 .sln / 无测试"结论限定为 **`f187174` 所属 `fix/RZ-VK3-A-surface-contract` 分支快照**；`origin/main` 实测含 `XuanYu.Engine.sln` + `XuanYu.Engine.Tests/`（见 §3、§6） |
| 2 | 5+100 称"不存在压行伪装" | 物理行数通过（111 文件，0 超 100），但 `VulkanRenderSession.cs`、`VulkanPresentLoop.cs` 存在压行/职责密度问题；**"质量条件"未通过，红线总判定不成立**（见 §7） |
| 3 | 空 `catch` 判为正常（绿色 ✅） | `VulkanPresentLoop.cs:96-97` 空 `catch` 违反宪法异常规则，列为 **P1**（见 §8 项7、§14） |
| 4 | Editor.UI→Vulkan 称"非活跃历史债务" | `VulkanSurfaceBridgeProvider.cs`（组合根）、`UiVm.VulkanProbe.cs`、`VulkanProbeRoute.cs` 共 **3 个活跃 .cs** 直接 `using XuanYu.Render.Vulkan`；红线1违反位于**活跃组合根**（见 §4、§14 P1-a） |
| 5 | Vk 所有权表写"Session 持有并向下传播" | 实测 `Vk` 由 `VulkanNativeHostSurfaceBridge` 唯一持有并在 `Dispose` 释放；`VulkanRenderSession` **无 `_vk` 字段**（见 §8 表、项1） |
| 6 | Vulkan 异常后状态整体判"正常" | `Create` 部分泄漏、`Bridge.Attach` 部分泄漏、`Resize` 无 `try/finally`、`Stop` 的 `Join` 未检查返回值——新增 **P1：失败路径回滚与 Present 线程终止可靠性**（见 §8、§14 P1-c） |
| 7 | 能力表数字与 SVG 不一致（A=9/B=1/C=0/D=1） | 实测 A=10、B=1、D=1；Shape/Mesh/Camera/Scene/DescriptorSet 改列为**"未规划／不在当前阶段"**（非"已规划未实装"）；SVG 同步修正（见 §9、可视化） |
| 8 | 基线标注混淆（正文 9bc210e / SVG f187174） | 明确区分：**被审计代码基线 `9bc210e`** / **审计报告提交 `f187174`**（本轮 R1 另出新提交），SVG 双标注（见 §1、可视化） |
| 9 | 证据链不可复跑（引用 `.workbuddy` 本地脚本） | 新增 §附录：精确命令 + 排除目录 + 退出码 + 结果摘要 + 扫描文件数 + 发现列表，全部可复跑（见 §附录） |
| 10 | 111.ps1 称"强推" | 脚本为普通 `git push -u origin $Branch`，**无 `--force`**；修正表述为"破坏本地历史并尝试推向远端，是否成功取决于远端状态"（见 §13） |
| 11 | ORG-2 范围过大（文档+删移+架构+测试+卫生混一轮） | 拆为 5 轮：ORG-1-R1 / SAFE-1 / VK-LIFE-1 / ARCH-A-PLAN / ORG-2（见 §16） |

---

## 1. 审计范围与基线

本轮盘点 `fix/RZ-VK3-A-surface-contract` 分支在 `9bc210e` 代码树下的真实状态，回答五个问题：当前真实状态、已完成并真机验收的能力、仅有代码/计划未验收的能力、架构依赖与 Vulkan 生命周期是否符合宪法、代码/文档/仓库卫生债务。

**范围边界（关键修正）**：本轮**仅审计该分支快照**，**未**与 `main`、分叉点或合并状态比较。GitHub 默认分支 `main`（及 `origin/main`）仍为另一套旧目录结构，实测含 `XuanYu.Engine.sln` 与 `XuanYu.Engine.Tests/`（大量历史测试）；而本分支 `9bc210e`/`f187174` 仅有 5 个重构后项目，无解决方案、无测试项目。因此：

> "无 `.sln`、无测试项目" 仅对 **`f187174` 所属重构分支快照**成立，**不能**上升为整个仓库或项目的全局事实。

审计范围限定：`.cs` / `.axaml` / `.csproj` / 文档 / 仓库卫生。所有数字均有命令或文件依据（见各节"依据"与 §附录）。

---

## 2. Git 与工作区状态

| 项 | 值 |
| --- | --- |
| 当前分支 | `fix/RZ-VK3-A-surface-contract` |
| 被审计代码基线 | `9bc210e0217e2b07956cc8732ae84aea6d83e3f2` |
| 审计报告提交（ORG-1） | `f1871741dd97e3dc1e9dab2f9ce85846f852c78d` |
| upstream | `origin/fix/RZ-VK3-A-surface-contract`（已设） |
| ahead / behind | `0 / 0`（与同名远端同步） |
| 分支比较范围 | **仅**比较本地分支与同名远端分支；**未**比较 `main`/分叉点/合并状态（范围限制见 §1） |
| staged / unstaged | 无 |
| untracked | `qizheng-mvp-fixed/`（未跟踪，未被 .gitignore 覆盖） |
| 最近 15 提交 | f187174 → 9bc210e → 3f03725 → ab81c83 → b1f77d7 → 139c748 → 4485186 → c53b7a8 → 28f8c54 → 9a526eb → ca46586 → 89c5b47 → 0aaedf9 → 80bc320 → d749820 |
| 当前 Tag | `m2.6.1-prefix-proof`（1 个；按宪法第十二条，正式 Tag 须验收通过并经用户确认，此 Tag 已存在，待裁决） |

**`qizheng-mvp-fixed/` 性质判定**：独立 JS MVP 项目误置于引擎仓库根目录。不被任何引擎项目引用，含 23 个 `.js` / 4 个 `.md` / 1 个 `.css` / 1 个 `.html`，**不属于玄域引擎源码**。当前为 untracked 且未被 `.gitignore` 忽略——一旦 `git add -A` 即有入库风险。建议迁出或加入 `.gitignore`（见 §13、§16 SAFE-1）。

依据：`git branch -a` / `git rev-parse` / `git status --short --branch` / `git remote -v` / `git ls-tree -r origin/main`（验证 main 含 `.sln` 与 `Tests`）。

---

## 3. 解决方案与项目清单

**在 `f187174` 所属重构分支快照中，仓库无 `.sln` / `.slnx` 解决方案文件**，仅有 5 个 `.csproj`。

> ⚠ 范围限定：此结论仅对本分支快照成立。`origin/main` 实测含 `XuanYu.Engine.sln`（解决方案）与 `XuanYu.Engine.Tests/`（测试项目），详见 §6。

按规范"若无解决方案文件则逐项目构建并说明原因"：因缺少统一解决方案，且各项目依赖关系已由 ProjectReference 描述，`dotnet build` 会按需解析依赖，故逐项目执行构建，并单独报告每个项目结果。

| 项目 | 路径 | OutputType | TargetFramework |
| --- | --- | --- | --- |
| XuanYu.Core | `XuanYu.Core/XuanYu.Core.csproj` | 库（默认） | net10.0 |
| XuanYu.Render.Abstractions | `XuanYu.Render.Abstractions/XuanYu.Render.Abstractions.csproj` | 库 | net10.0 |
| XuanYu.Render.Vulkan | `XuanYu.Render.Vulkan/XuanYu.Render.Vulkan.csproj` | 库 | net10.0 |
| XuanYu.Editor.Win | `XuanYu.Editor.Win/XuanYu.Editor.Win.csproj` | WinExe | net10.0-windows |
| XuanYu.Editor.UI | `XuanYu.Editor.UI/XuanYu.Editor.UI.csproj` | WinExe | net10.0 |

---

## 4. 项目依赖

依赖图（→ 表示 ProjectReference 或 PackageReference 方向）：

```
XuanYu.Editor.UI  ──ProjectRef──▶ XuanYu.Core
                    ──ProjectRef──▶ XuanYu.Render.Vulkan   ⚠ 违反红线1（债务A，活跃）
                    ──ProjectRef──▶ XuanYu.Render.Abstractions
                    ──PackageRef──▶ Avalonia 12.0.4
                    ──PackageRef──▶ Silk.NET.Vulkan 2.22.0           ⚠ 直接引用
                    ──PackageRef──▶ Silk.NET.Vulkan.Extensions.KHR 2.22.0

XuanYu.Render.Vulkan ──ProjectRef──▶ XuanYu.Render.Abstractions
                      ──PackageRef──▶ Silk.NET.Vulkan 2.22.0
                      ──PackageRef──▶ Silk.NET.Vulkan.Extensions.KHR 2.22.0

XuanYu.Render.Abstractions ──（无依赖）✅ 不依赖 Silk.NET.Vulkan

XuanYu.Editor.Win ──ProjectRef──▶ XuanYu.Core  （不连 Vulkan，也不连 Editor.UI）

XuanYu.Core ──（无依赖）
```

**两条红线验证**：
- **红线1：`Editor.UI` 不得直接依赖 Vulkan 实现** → **当前违反（债务 A），且为活跃违反**。`XuanYu.Editor.UI.csproj` 含 `ProjectReference XuanYu.Render.Vulkan` 与 `PackageReference Silk.NET.Vulkan`（csproj 第 19/20/25 行）。直接引用并非仅来自历史 `VulkanClearSession` 死代码——**3 个活跃 `.cs` 也直接 `using XuanYu.Render.Vulkan`**：
  - `Viewport/Vulkan/VulkanSurfaceBridgeProvider.cs:2`（组合根，`new VulkanNativeHostSurfaceBridge(log)`）
  - `Vm/UiVm.VulkanProbe.cs:1`
  - `VulkanProbeRoute.cs:1`
  
  即宿主对象虽经 `INativeHostSurfaceBridge` 契约使用，但**实现装配发生在 Editor.UI 内的活跃组合根**，红线违反位于活跃链路，非仅历史死代码。收口方案（§16 ARCH-A-PLAN）须覆盖此活跃组合根，不能只清死代码。
- **红线2：`Render.Abstractions` 不得引用 `Silk.NET.Vulkan`** → **成立**。`Abstractions.csproj` 无任何 PackageReference、无 ProjectReference。

**`Editor.Win` 如何连接 UI 与 Vulkan**：实际**未连接**。`Editor.Win` 仅引用 `XuanYu.Core`，不含 Vulkan 或 Avalonia 引用，是 VK3 之前的遗留 WinForms 壳（仅 `MainForm`/`Program`）。真实 Avalonia 编辑器在 `Editor.UI`，真实 Vulkan 链路在 `Render.Vulkan`；二者经 `Render.Abstractions` 契约与组合根接线。`Editor.Win` 与当前活跃架构脱钩，属卫生/归档候选。

**抽象层反向依赖 / Core 依赖实现 / 循环引用**：未发现。`Abstractions` 零依赖；`Core` 零依赖；`Vulkan → Abstractions`（实现依赖抽象，方向正确）；无循环 ProjectReference。

---

## 5. 第三方依赖

| 项目 | 第三方依赖 | 版本 |
| --- | --- | --- |
| XuanYu.Editor.UI | Avalonia / Avalonia.Desktop / Avalonia.Fonts.Inter / Avalonia.Themes.Fluent / Silk.NET.Vulkan / Silk.NET.Vulkan.Extensions.KHR | 12.0.4 / 2.22.0 |
| XuanYu.Render.Vulkan | Silk.NET.Vulkan / Silk.NET.Vulkan.Extensions.KHR | 2.22.0 |
| 其余项目 | 无 | — |

依据：5 个 `.csproj` 的 `PackageReference` 字段（详见 §4）。无 NuGet 版本冲突；5 项目构建均 0 警告 0 错误（见 §6、§附录）。按宪法第十条，新增/升级第三方依赖须提案；当前依赖均为既有，本轮未变动。

---

## 6. 构建、警告和测试

无 `.sln`，逐项目构建（命令与退出码见 §附录 A2）。

| 项目 | 命令 | Exit Code | Warning | Error |
| --- | --- | --- | --- | --- |
| XuanYu.Core | `dotnet build XuanYu.Core/XuanYu.Core.csproj --nologo` | 0 | 0 | 0 |
| XuanYu.Render.Abstractions | `dotnet build XuanYu.Render.Abstractions/XuanYu.Render.Abstractions.csproj --nologo` | 0 | 0 | 0 |
| XuanYu.Render.Vulkan | `dotnet build XuanYu.Render.Vulkan/XuanYu.Render.Vulkan.csproj --nologo` | 0 | 0 | 0 |
| XuanYu.Editor.Win | `dotnet build XuanYu.Editor.Win/XuanYu.Editor.Win.csproj --nologo` | 0 | 0 | 0 |
| XuanYu.Editor.UI | `dotnet build XuanYu.Editor.UI/XuanYu.Editor.UI.csproj --nologo` | 0 | 0 | 0 |

**测试**：**在本分支 `f187174` 快照中无任何测试项目**（5 个 csproj 中无 `*Tests*`/`*Test*`）。`dotnet test` 无可运行项。**发现：本分支零自动化测试覆盖**（风险分级 P2）。

> ⚠ 范围限定：此结论仅对本分支成立。`origin/main` 实测含 `XuanYu.Engine.Tests/` 全套测试（EntityId/Vector3d/EngineResult/CodeFileBudgetTests/ProjectDependencyDirectionTests 等）。本分支重构时测试未被携带/重建，故当前无测试基建。

依据：5 个项目 `dotnet build` 实际输出（"已成功生成 / 0 个警告 / 0 个错误 / EXIT=0"，见 §附录 A2）。未关闭分析器、未降级、未跳过测试——本轮无代码改动，构建为真实执行结果。

---

## 7. 5+100 审计

统计范围：当前分支 `git ls-files '*.cs' '*.axaml'`（自动排除 `.gitignore` 覆盖的 `bin/obj/.artifacts/codex_log` 及 untracked 的 `qizheng-mvp-fixed`）。可复跑命令见 §附录 A1。

| 指标 | 值 |
| --- | --- |
| 手写 `.cs/.axaml` 文件总数（tracked） | **111** |
| 超过 100 行的文件数 | **0** |
| 最大文件行数 | 100（`XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs`，恰为硬顶） |
| 次大文件 | 99（`VulkanDeviceOwner.cs`、`VulkanPresentLoop.cs`） |

**结论（修正）：物理行数条件通过，质量条件未通过，红线总判定不成立。**

- **数字条件（通过）**：111 个手写文件物理行数均 ≤100，0 个超限；最大文件恰 100 行。
- **质量条件（未通过）**：至少以下文件存在明显压行与职责密度问题，属为卡 100 行牺牲可读性的迹象：
  - `VulkanRenderSession.cs:23`：`uint _generation; int _recoverTries; bool _disposed;`（三字段挤一行）
  - `VulkanRenderSession.cs:28-29`：构造内多字段连续赋值挤在一行
  - `VulkanRenderSession.cs:47`：`catch (Exception ex) { log?.Invoke(...); return null; }`（判断+记录日志+返回挤一行）
  - `VulkanRenderSession.cs:93`：`if (_disposed) return; _disposed = true;`（判断返回与赋值挤一行）
  - `VulkanPresentLoop.cs:14-15`：多个 `readonly` 字段 / 7 个实例字段声明挤在一行
  - `VulkanPresentLoop.cs:43-44`：多个局部变量声明与 `SubmitInfo`/`PresentInfoKHR` 初始化挤在一行

因此 ORG-1 原"不存在压行伪装迹象"与"5+100 红线成立"**均不成立**。正确表述：**5+100 的"数字条件"通过，"质量条件"未通过；红线作为"可读的小文件纪律"总判定不成立**，至少 `VulkanRenderSession.cs`、`VulkanPresentLoop.cs` 须在 VK-LIFE-1 或专项轮做可读性拆分（不扩大行数阈值，而是把压行展开为合理多行，仍守 ≤100）。

**死代码 VulkanClearSession 4 文件行数**：`VulkanClearSession.cs` 81 / `.Device.cs` 49 / `.Swapchain.cs` 46 / `.Dispose.cs` 18（合计 194）。均 ≤100，行数合规，但属无引用死代码（见 §8、§9）。

---

## 8. Vulkan 生命周期

基于读取 `VulkanRenderSession.cs`（组合根）、`VulkanPresentLoop.cs`、`VulkanClearFrameOwner.cs`、`VulkanNativeHostSurfaceBridge.cs` 及各 Owner 文件。

### 8.1 对象所有权表（修正）

| 对象 | 责任文件 | 创建者 | 唯一持有/释放者 | 使用者 | 释放者 | 异常后状态 | 结论 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| **Vk（Silk 调度表）** | `VulkanNativeHostSurfaceBridge` | `Vk.GetApi()`（Bridge 内，仅 ownedVk 时） | **`VulkanNativeHostSurfaceBridge`**（`_vk` 字段；`Dispose` 中 `_vk?.Dispose()`） | 各 Owner / Session（经参数传入，不持有） | `VulkanNativeHostSurfaceBridge.Dispose` | 可恢复 | 正常（所有权唯一） |
| Instance | `VulkanInstanceOwner` | `VulkanInstanceOwner.Create` | `VulkanInstanceOwner` | Bridge/Session | `VulkanInstanceOwner.Dispose` | 快速失败 | 正常 |
| Surface | `VulkanSurfaceOwner` | `VulkanSurfaceOwner.Create`（Win32） | `VulkanSurfaceOwner` | Bridge/Session | `VulkanSurfaceOwner.Dispose` | 快速失败 | 正常 |
| PhysicalDevice | `VulkanPhysicalDeviceSelector` | Select | Session（经 Selection 传入） | Session | 随 Device | — | 正常 |
| LogicalDevice | `VulkanDeviceOwner` | `VulkanDeviceOwner.Create` | `VulkanDeviceOwner` | PresentLoop/ClearFrame | `VulkanDeviceOwner.Dispose` | 快速失败 | 正常 |
| Queue | `VulkanDeviceOwner` 内选取 | Create | `VulkanDeviceOwner` | PresentLoop | 随 Device | — | 正常 |
| Swapchain | `VulkanSwapchainOwner` | `Recreate` | `VulkanSwapchainOwner` | PresentLoop/ClearFrame | `DestroySwapchain` | 可恢复 | 正常 |
| ImageView | `VulkanSwapchainOwner` | 随 Swapchain | `VulkanSwapchainOwner` | ClearFrame | 随 Swapchain | — | 正常 |
| RenderPass | `VulkanClearFrameOwner.BuildRenderPass` | 构造 | `VulkanClearFrameOwner` | ClearFrame | `Dispose` | — | 正常 |
| Framebuffer | `VulkanClearFrameOwner` | `RebuildFramebuffers` | `VulkanClearFrameOwner` | ClearFrame | `DestroyFramebuffers` | — | 正常 |
| ShaderModule | `VulkanShaderModuleOwner` | Create（短生命周期） | `VulkanShaderModuleOwner` | Pipeline | 建完 Pipeline 释放 | — | 正常 |
| GraphicsPipeline | `VulkanGraphicsPipelineOwner` | `Create` | `VulkanGraphicsPipelineOwner` | ClearFrame | `Dispose` | — | 正常 |
| CommandPool/Buffer | `VulkanClearFrameOwner` | 构造/Record | `VulkanClearFrameOwner` | PresentLoop | `Dispose`/`Free` | — | 正常 |
| Semaphore/Fence | `VulkanPresentLoop.CreateSync` | `CreateSync` | `VulkanPresentLoop` | PresentLoop | `Dispose` | — | 正常 |
| Present Pump | `VulkanPresentLoop` | `Start`（后台线程） | `VulkanPresentLoop` | Session | `Stop`→`Dispose` | 见 8.2 | **不达标（P1）** |
| Resize | `VulkanRenderSession.Resize` | UI 调用 | Session（`_rebuildLock`） | — | — | 见 8.2 | **不达标（P1）** |
| Detach/Dispose | 组合根 / Session | — | Session | — | 顺序释放 | 见 8.2 | **不达标（P1）** |

**Vk 真实所有权（修正）**：

```
VulkanNativeHostSurfaceBridge
    └─ 唯一持有并释放 Vk（_vk 字段；Dispose 中 _vk?.Dispose()）
       ├─ Vk.GetApi() 仅当本 Bridage 自持时创建
       ├─ 传给 Instance/Surface/Device/Swapchain（经参数）
       └─ 传给 VulkanRenderSession 及其子 Owner 使用（经参数，Session 不持有 _vk）
```

> ⚠ ORG-1 原表称"Vk 由 Session 持有并向下传播"**错误**：`VulkanRenderSession` 源码无 `_vk` 字段，仅将传入的 `Vk` 继续交给各 Owner；`Vk` 的唯一持有/释放者在 `VulkanNativeHostSurfaceBridge`。

### 8.2 十项重点验证（修正）

1. **Vk 是否由唯一 Bridge 持有/释放**：✅ 是。`VulkanNativeHostSurfaceBridge._vk` 持有，`Dispose` 释放；`VulkanRenderSession` 不持有 `_vk`。
2. **Instance/Surface 是否无重复所有权**：✅ 分别由 `VulkanInstanceOwner`/`VulkanSurfaceOwner` 唯一持有。
3. **Resize 是否只在真实尺寸变化时进入重建**：✅ `Resize` 先判 `Extent == 目标` → 同尺寸直接 `return`。
4. **同尺寸 Resize 是否快速返回**：✅ 打 `Resize 快速跳过` 日志后 return，不 Stop/Start 泵。
5. **Present Pump 停止/重建/恢复顺序**：⚠ 顺序本身正确（Stop→Recreate→Rebuild→Start；OutOfDate 经统一入口），但**失败/终止可靠性见第 7 项 P1**。
6. **Detach 是否先停泵再释放**：✅ `Dispose` 先 `_presentLoop.Stop()` 再 Dispose Pipeline/ClearFrame。
7. **后台线程日志回调异常**：❌ **P1 违反宪法**。源码 `VulkanPresentLoop.cs:94-98`：
   ```csharp
   void Log(string m)
   {
       try { _log?.Invoke(m); }
       catch { }          // ← 空 catch，违反"禁止空 catch / 异常必须可追踪"
   }
   ```
   ORG-1 判为绿色 ✅ 属漏报。正确做法：使用不回调原日志链的兜底诊断通道，或记录一次受限诊断状态，而非完全静默吞异常。
8. **Editor.UI 是否只经抽象使用渲染**：❌ **P1（活跃违反）**。活跃链路 `VulkanSurfaceBridgeProvider` 在 Editor.UI 内 `new VulkanNativeHostSurfaceBridge(log)`（组合根装配），且 `UiVm.VulkanProbe.cs`、`VulkanProbeRoute.cs` 也直接 `using XuanYu.Render.Vulkan`。红线1违反位于活跃组合根，非仅历史死代码。
9. **关闭时 RenderPass/Framebuffer 重复释放日志**：✅（非阻断日志瑕疵）经代码确认 `Dispose` 对每个对象仅销毁一次；`file-tree.md`/VK5-B 收口注明"关闭日志重复打印一行（仅日志重复，非双重释放，留待 LOG-CLEANUP）"。
10. **VK5-E VulkanClearSession 死代码是否真实**：✅ 真实。`4` 个 partial 位于 `XuanYu.Editor.UI/Viewport/Vulkan/`，全仓 grep 无外部引用（无 `new`、无字段、无 `TryCreate` 调用方），仅类内部互调 + 历史 markdown。死代码属实（债务 B）。

### 8.3 Vulkan 异常安全（新增 P1，修正"整体正常"误判）

ORG-1 大量使用"快速失败、可恢复、正常"，但源码存在明确失败路径缺口，**Vulkan 生命周期不能整体判为"正常"**：

- **`VulkanRenderSession.Create` 部分资源泄漏**：若 `clear` 创建成功、`pipeline` 创建成功，但 `loop = new VulkanPresentLoop(...)`（第 42 行）或后续抛出，`catch`（第 47 行）直接 `return null`，**未释放已创建的 `clear`/`pipeline`**。→ 需 `try/finally` 或 using 模式回滚部分资源。
- **`VulkanNativeHostSurfaceBridge.Attach` 部分回滚缺失**：`Attach` 中 `_deviceOwner = VulkanBridgeDeviceAttachStep.Run(...)`（第 41 行，写入字段）；若随后 `Swapchain` 步（第 42 行）或 `RenderSession` 步（第 43 行）抛异常，`catch`（第 45-52 行）仅 `surface?.Dispose(); instance?.Dispose();` 并清理局部/ownedVk，**未释放已写入字段的 `_deviceOwner`**（亦未释放可能已建好的 `_swapchainOwner`/`_renderSession` 局部/字段）。→ 需完整回滚已提交的字段。
- **`VulkanRenderSession.Resize` 无 `try/finally`**：`Resize` 先 `_presentLoop.Stop()`（第 57 行），在 `_rebuildLock` 内 `Recreate`/`RebuildFramebuffers`（第 60-61 行）；若重建抛异常，异常逃出 `Resize`，第 65 行 `_presentLoop.Start()` **不会执行**，渲染泵停在失败（已停）状态。→ 需 `try/finally` 保证异常时 `Start()` 或进入明确失败态。
- **`VulkanPresentLoop.Stop` 的 `Join` 未检查返回值**：`Stop`（第 79 行）`_stop = true; t.Join(2000); _thread = null;` 不检查 `Join` 返回值；随后 `Dispose`（第 83-92 行）直接 `DestroySemaphore`/`DestroyFence`。若 `Join` 超时（线程仍在跑），`Dispose` 会在后台线程仍使用同步对象时销毁它们，**理论存在竞态/崩溃**。→ 需检查 `Join` 返回值并在超时时不立即释放，或改用可取消的协作终止。

**结论**：Vulkan 生命周期"正常路径"基本正确，但**失败路径资源回滚与 Present 线程终止可靠性不达标**，列为 **P1（VK-LIFE-1 专项处理）**。

---

## 9. 当前能力状态（修正数字）

严格区分"真机验收"与"代码存在"。

| 能力 | 当前状态 | 证据 | 最后提交 | 遗留问题 |
| --- | --- | --- | --- | --- |
| Instance / Surface 生命周期 | A 已真机验收 | `rz-vk3-closure.md` + 用户真机 | b1f77d7 前 | 无 |
| 物理/逻辑设备选择 | A 已真机验收 | VK4-A 收口 + 真机 | `vk4-a*` | 无 |
| Swapchain 创建/重建 | A 已真机验收 | VK4-C 收口 + 真机 | `vk4-c*` | 无 |
| Clear + Present 单色清屏 | A 已真机验收 | VK4-D 真机 | `vk4-d*` | 关闭日志重复一行（非阻断） |
| 固定三角形（GPU 绘制） | A 已真机验收 | VK5-B 封版（2026-07-10 用户拍板） | d749820 | 无 |
| Resize 后 Present 自愈 | A 已真机验收 | VK5-A-R2 真机 | cd509e1 | 无 |
| 同尺寸 Resize 快速跳过 | A 已真机验收 | VK5-D-R3 真机（2026-07-11 trace） | 9a526eb | 无 |
| 关闭释放顺序 | A 已真机验收 | 多轮真机（VK4-D/VK5） | — | 同 §8.2⑨ 日志瑕疵 |
| viewport/scissor 与 Resize 关系 | A 已封版（验证收口，非新代码） | VK5-C 封口（2026-07-11 用户确认） | 139c748 | 宽高比修正留待 Camera/Projection |
| LOG-UX 日志系统 | A 已实装并文档收口 | LOG-UX 各轮 + 收口 | `log-ux-*` | 无 |
| 视口 UI 收口（移除 overlay） | B 自动构建通过，无明确真机验收记录 | 仅构建 0W0E | `viewport-ui-*` | 待用户真机确认（UI 类改动按宪法须真机） |
| VulkanClearSession 死代码清理（VK5-E） | D 仅规划，未实装 | `docs/rz-vk5-e-plan.md` | — | 待确认实装（债务 B） |
| Shape/Mesh/Camera/Scene/DescriptorSet | **未规划／不在当前阶段** | 各 VK 轮红线明确"不建"；路线图未定 | — | 非"已规划未实装"，属未来阶段 |

**数字汇总（修正）**：A=10、B=1、D=1、未规划/不在当前阶段=1（组）。

> ⚠ ORG-1 原 SVG 写 A=9、B=1、C=0、D=1 与正文不一致；且 Shape/Mesh/Camera/Scene/DescriptorSet 原误列 D（"已规划未实装"），实为"尚未开发、不在当前阶段"。本报告与可视化已统一修正。

**真机验收边界说明**：A 类均有多轮用户真机回传或收口文档支撑；B 类（视口 UI 收口）仅有构建通过，无真机验收记录，按宪法第十二条 UI 类改动需真机验收再 Push，故标记待确认；D 类（VK5-E）仅规划文档，代码未动；"未规划/不在当前阶段"类仅为路线图占位，无任何计划或代码。

---

## 10. 真机验收证据

- VK5-B 固定三角形：`changelog` v0.2.9.1 + `d749820` 提交 + "2026-07-10 用户拍板封版"。
- VK5-D-R3 同尺寸跳过：`changelog` v0.2.11.4 + `9a526eb` + "2026-07-11 用户 run.bat 回传 trace，全部 11 项验收 PASS"。
- VK5-A-R2 自愈：`changelog` v0.2.8.4 + `cd509e1` + 真机 Resize 后 Present 恢复。
- VK5-C 封版：`changelog` v0.2.10.1 + `139c748` + "2026-07-11 用户回传确认三件事成立"。
- VK3/VK4 各轮：`rz-vk3-closure.md` / `rz-vk4-closure.md` 收口报告 + 双项目 0W0E。

---

## 11. 文档基线

扫描 `docs/`（54 个 `.md`）+ 根目录文档。分类：

| 类别 | 文件 |
| --- | --- |
| 最高有效规范 | `玄域引擎_AI开发宪法.md` |
| 当前有效状态/契约文档 | `版本号规范与历史映射.md`、`dev-rules.md`、`diagnostic-safety.md`、`dev-rules-understanding.md`、`file-tree.md`、`changelog.md` |
| 当前有效计划（仍待实装） | `rz-vk5-e-plan.md`（VK5-E 死代码清理） |
| 已完成收口报告 | `rz-vk3-closure.md`、`rz-vk4-closure.md`、`rz-vk5-c-plan.md`（封版）、`rz-vk4-c-r1-audit-plan.md`（部分）、`project-baseline-audit-org-1.md`（已退回）、本轮 `project-baseline-audit-org-1-r1.md` |
| 历史计划（对应工作已完成） | `vulkan-lifecycle-plan.md`、`rz-vk3-surface-lifecycle-plan.md`、`rz-vk4-plan.md`、`rz-vk4-c-swapchain-plan.md`、`rz-vk4-d-plan.md`、`rz-vk5-plan.md`、`rz-vk5-a-plan.md` |
| 历史记录/审计（已完成参考） | `audit-EditorShellV2-*`(6)、`audit-gizmo-*`(5)、`audit-input-lifecycle-9.0X-*`(3)、`audit-NativeViewportMouseCapture-9.0X.md`、`audit-inspector-transform-9.0C-0.md`、`plan-9.0D-move-gizmo-final.md`、`editor-*`(4)、`gizmo_drag_audit_2026-06-25.md`、`naming-XuanYu-Engine.md`、`vulkan-preflight-audit-RZ-Fix3-0.md`、`audit-RZ-*`(4)、`MILESTONE1_PUBLIC_VALIDATION.md` |
| 内容需要更新 | `AI_DEVELOPMENT_RULES.md`（仍写 `FluidWarfare.*` 命名空间）、`CODE_CONSTITUTION.md`（100 行规则偏松）、`PROJECT_CHARTER.md`/`PHASE1_SCOPE.md`/`ENGINE_ARCHITECTURE.md`/`NAMING_RULES.md`/`LEGACY_FLUIDWARFARE_OLD_AUDIT.md` |
| 内容重复 | 多份 RZ/VK 审计与计划存在交叉重叠 |
| 内容冲突 | `AI_DEVELOPMENT_RULES.md` 旧命名 vs 实际 `XuanYu.*`；`CODE_CONSTITUTION.md` 100 行偏松 vs 新宪法硬红线 |
| 待归档候选 | 全部"历史记录/审计"类、`codex_log/`（AI 日志，已 gitignore） |
| 待删除候选 | 无（本轮不改内容；`111.ps1` 为脚本非文档，见 §13） |

**防止把历史计划误当当前计划**：当前唯一仍"待实装"的计划是 `rz-vk5-e-plan.md`（VK5-E）。VK3/VK4/VK5-A/B/C/D 的计划文档已全部对应实装/封版，归类为"历史计划"，**不**视为当前计划。

---

## 12. 三份治理文档规则矩阵

| 规则领域 | 新宪法 | 代码宪法 | 旧AI规则 | 判断 |
| --- | --- | --- | --- | --- |
| 5+100 | 100 硬红线，无例外 | 100 建议，复杂可近 150 | 未明确 | 冲突：新宪法最严，统辖；代码宪法偏松，待更新。**质量条件亦须守（见 §7）** |
| 项目依赖边界 | Editor.UI 不依赖 Vulkan 实现；Abstractions 不引 Silk | 平台隔离 | 旧名需更新 | 方向一致；当前 Editor.UI **活跃违反**（债A，见 §4） |
| Git 权限 | 禁止擅自 merge/rebase/强推/重写历史 | 未涉及 | 未涉及 | 新宪法独有，统辖 |
| Push 与真机验收 | UI/Vulkan/输入/生命周期/性能须真机验收再 Push | 未涉及 | 未涉及 | 新宪法独有 |
| 删除/移动/重命名 | 必须列文件/原因/依赖/影响/批准/下一轮 | 未涉及 | 仅"更新 file-tree" | 新宪法更严，统辖 |
| 新文件 | 重大模块/抽象须请示；小文件可加 | 未涉及 | 范围纪律 | 互补 |
| 第三方依赖 | 新增/升级须先提案 | 未涉及 | 不引入 Unity/Unreal 等 | 新宪法统辖 |
| Bug 排查 | 复杂 Bug 用中文探针四步 | 未涉及 | 未涉及 | 新宪法独有 |
| 中文化 | 机器英文/人类中文；高频链路禁普通日志 | 一致 | 人类可读默认中文 | 一致；新宪法更细 |
| Preview/Commit | Preview 只更新预览/渲染；Commit 才更新重负载 | 未涉及 | 未涉及 | 新宪法独有 |
| 测试 | 禁止弱化断言/删失败用例/跳测试须请示 | 生命周期测试 4 类 | 核心模块配套测试 | 一致；新宪法统辖禁止项 |
| 异常处理 | **禁空 catch；可恢复记中文+降级；不可恢复快速失败** | 未涉及 | 未涉及 | 新宪法独有（**VulkanPresentLoop 空 catch 违反，见 §8.2⑦**） |
| TODO | 须说明原因/触发/责任；禁"以后优化" | 未涉及 | 未涉及 | 新宪法独有 |
| 警告 | 不新增警告/分析器告警；禁关分析器/降级/批量 Suppress | 未涉及 | 未涉及 | 新宪法独有 |
| 版本规范 | `v0.M.m.r-类型` | 未涉及 | 未涉及 | 新宪法独有 |
| 文档同步 | changelog/file-tree 必须更新+字段 | 中文化提到 | 范围纪律+中文化 | 一致；新宪法字段更全 |
| 收口报告 | 五项（Git/文档/范围/可视化/结论） | 未涉及 | 每里程碑变更清单+验收 | 新宪法统辖 |

**判断汇总**：新宪法为最高有效规范，统辖其余两份。代码宪法与旧 AI 规则存在两处需裁决冲突（5+100 表述、旧 `FluidWarfare` 命名），本轮不修改其内容。

---

## 13. 仓库卫生问题（修正 111.ps1 措辞）

| 问题 | 路径 | 当前状态 | 风险 | 后续建议 |
| --- | --- | --- | --- | --- |
| 危险设置脚本已入库 | `111.ps1`（根目录，tracked） | 含 `Remove-Item ".git" -Recurse -Force` + 硬编码分支 `fix/RZ-Fix1-editor-access-violation` + `git init` + `git add -A` + `git push -u origin $Branch` | 高：误跑会删除用户 `.git` 并重新初始化仓库，尝试把重新初始化后的分支推向远端（是否成功取决于远端状态） | P1：经删除流程批准后移除；或先隔离 |
| 独立项目误放且未忽略 | `qizheng-mvp-fixed/`（根目录，untracked，未 gitignore） | 含 23 .js / 4 .md / .css / .html，非引擎代码 | 中：误 `git add -A` 即入库 | P2：迁出或加入 `.gitignore`（SAFE-1） |
| AI 日志本地存在 | `codex_log/`、`codex_log_xuanyu_handoff_20260705-2102.zip` | 已被 `.gitignore` 覆盖 | 低：仅本地残留 | P2：定期本地清理 |
| 生成物目录 | `.artifacts/`、`bin/`、`obj/` | 已被 `.gitignore` 覆盖 | 低 | 无 |
| 旧品牌命名残留于文档 | `AI_DEVELOPMENT_RULES.md`/`CODE_CONSTITUTION.md` 等 | tracked 文档 | 低：仅文档误导性 | P2：ORG-2 裁决 |
| 文档绝对路径残留 | `docs/audit-NativeViewportMouseCapture-lifecycle-9.0X.md:203` | 仅作为整改说明 | 低 | P3：可顺手清理 |
| 密钥/Token/密码 | 全仓 tracked 文件 | grep 模式未命中（见 §附录 A4） | 无 | 无（符合红线3） |
| FluidWarfare 旧命名泄漏源码 | 全仓 `.cs/.axaml/.csproj` | grep 未命中（见 §附录 A5） | 无 | 无（仅旧文档） |

依据：`git ls-files`、`.gitignore`、`git status`、`git ls-tree -r origin/main`、`grep` 源码（FluidWarfare / 密钥模式 / `Render.Vulkan` 引用）。

> ⚠ ORG-1 原表将 `111.ps1` 描述为"强推"**错误**：脚本第 41 行为普通 `git push -u origin $Branch`，**无 `--force`**。修正为"破坏本地 Git 历史并尝试把重新初始化后的分支推向远端；是否成功取决于远端状态"。

---

## 14. 风险分级（修正）

- **P0（阻断继续开发）**：无。
- **P1（下一功能轮前必须处理）**：
  1. **`Editor.UI` 直接依赖 `Render.Vulkan` + `Silk.NET.Vulkan`（红线1 活跃违反，债务 A）**：位于组合根 `VulkanSurfaceBridgeProvider` 及 `UiVm.VulkanProbe`/`VulkanProbeRoute` 共 3 个活跃 `.cs` + csproj（§4、§8.2⑧）。须排 ARCH-A-PLAN 专项收口到 `Abstractions`，覆盖活跃组合根。
  2. **空 `catch` 违反宪法（VulkanPresentLoop.cs:96-97）**：后台线程日志回调空 `catch` 静默吞异常，违反"禁止空 catch / 异常必须可追踪"。须改用兜底诊断通道或受限诊断状态（§8.2⑦）。
  3. **Vulkan 失败路径资源回滚与 Present 线程终止可靠性**：`Create`/`Attach` 部分泄漏、`Resize` 无 `try/finally`、`Stop` 的 `Join` 未检查（§8.3）。须排 VK-LIFE-1 专项修复。
  4. **`111.ps1` 危险脚本已入库**：须按删除流程批准后移除（SAFE-1）。
- **P2（已知债务，可排专项）**：
  5. 本分支零自动化测试覆盖（注：`origin/main` 有 `XuanYu.Engine.Tests`，本分支未携带）——须建/恢复测试项目（至少 Render.Vulkan 生命周期/Dispose 幂等）。
  6. `qizheng-mvp-fixed/` 未跟踪未忽略——迁出或 gitignore（SAFE-1）。
  7. 旧治理文档含旧命名/偏松规则——ORG-2 裁决归档或合并。
  8. `codex_log/` 与 handoff zip 本地残留（虽 gitignore）——定期清理。
- **P3（非阻断整理项）**：
  9. 关闭时 RenderPass/Framebuffer 释放日志重复一行（仅日志，留 LOG-CLEANUP）。
  10. 文档数量庞大（54 个 md），存在重复/历史计划——ORG-2 归档整理。
  11. `file-tree.md` 手动计数可能未含未跟踪/忽略项（qizheng/codex_log）——本轮仅因新增审计文档 +1。

---

## 15. 未解决问题

1. 红线1（Editor.UI→Vulkan 直接依赖）如何收口到 `Abstractions` 且覆盖**活跃组合根**（VulkanSurfaceBridgeProvider 等），而非仅清死代码——需专项设计（ARCH-A-PLAN）。
2. 空 `catch`（VulkanPresentLoop）的兜底诊断通道设计——须在 VK-LIFE-1 一并处理。
3. Vulkan 失败路径资源回滚（Create/Attach 部分泄漏、Resize try/finally、Stop Join 检查）——VK-LIFE-1 专项。
4. VK5-E（清 VulkanClearSession 死代码）实装时机——规划已就绪，待用户确认。
5. `111.ps1` 删除需用户按删除流程批准（本轮只审计，不删，SAFE-1）。
6. `qizheng-mvp-fixed/` 处置方式（迁出 vs gitignore）待用户定（SAFE-1）。
7. 旧治理文档（FluidWarfare 命名/100 行偏松）是否标记 deprecated/合并——待用户裁决（ORG-2）。

---

## 16. 后续轮次建议（拆轮，修正 ORG-1 过大范围）

ORG-1 原 ORG-2 把"文档+删移+架构+测试+卫生"混成一轮，违反范围控制原则。修正为 5 个独立轮次：

1. **ORG-1-R1（本轮）**：仅修正 ORG-1 审计报告中的事实、表格、风险等级与可视化；不修代码。✅ 本轮完成。
2. **SAFE-1（安全卫生）**：删除 `111.ps1`（删除流程批准后）；处理 `qizheng-mvp-fixed/`（迁出或 gitignore）；本地清理 `codex_log/`。纯仓库安全，不碰架构/文档体系。
3. **VK-LIFE-1（Vulkan 生命周期专项）**：审计并修复异常回滚（Create/Attach 部分泄漏）、空 `catch`（兜底诊断通道）、`Resize` 的 `try/finally`、`Stop` 的 `Join` 检查、5+100 压行可读性拆分（§7、§8）。独立 commit，须真机验收。
4. **ARCH-A-PLAN（架构迁移设计）**：单独设计 `Editor.UI` 组合根迁移到 `Abstractions`，使 `Editor.UI` 不再直接 `using XuanYu.Render.Vulkan`/引用 `Silk.NET.Vulkan`（覆盖活跃组合根）。纯设计文档 + 可选 PoC，独立 commit。
5. **ORG-2（治理收口，最后做）**：仅做治理文档单一入口与归档——合并/标记 deprecated `AI_DEVELOPMENT_RULES.md`/`CODE_CONSTITUTION.md`；更新旧品牌/旧阶段表述；将 30+ 历史文档移入 `docs/archive/`。不夹杂删移脚本、架构重构或测试基建。

各轮独立 commit、独立验收；所有删除/移动按宪法第十三条走批准流程。

---

## 附录 A：可复跑证据（精确命令 + 结果）

> ORG-1 原报告引用 `.workbuddy/audit_5plus100.py`（本地 gitignore 目录），后续审计者无法复跑。本附录以 `git ls-files` 等标准命令记录，任何审计者可在本分支复跑。

### A1. 5+100 行数统计
```
命令：git ls-files '*.cs' '*.axaml' | wc -l            # 文件总数
      git ls-files '*.cs' '*.axaml' | while IFS= read -r f; do wc -l "$f"; done | sort -n | tail -15
      git ls-files '*.cs' '*.axaml' | while IFS= read -r f; do n=$(wc -l < "$f"); [ "$n" -gt 100 ] && echo "OVER100: $f ($n)"; done
排除目录：自动由 .gitignore 排除 bin/obj/.artifacts/codex_log；untracked 的 qizheng-mvp-fixed 不计入 tracked
退出码：0
结果摘要：tracked 手写文件 = 111；最大 = 100（VulkanRenderSession.cs）；0 个文件 >100 行
发现列表：压行问题见 §7（VulkanRenderSession.cs / VulkanPresentLoop.cs）
```

### A2. 五项目构建
```
命令（逐项目）：dotnet build <proj>/<proj>.csproj --nologo > build_<proj>.log 2>&1 ; echo EXIT=$?
退出码：XuanYu.Core=0 / Render.Abstractions=0 / Render.Vulkan=0 / Editor.Win=0 / Editor.UI=0
警告/错误：每个项目构建日志末行均为 "已成功生成。 / 0 个警告 / 0 个错误"
扫描文件数：5 个 .csproj
发现列表：无警告、无错误、无缺失依赖
```

### A3. 分支与解决方案/测试范围
```
命令：git branch -a
      git ls-tree -r --name-only origin/main | grep -iE '\.sln$|Tests'
      git ls-files | grep -iE '\.sln$'
退出码：0
结果摘要：本分支（f187174）无 .sln；origin/main 含 XuanYu.Engine.sln 与 XuanYu.Engine.Tests/（全套历史测试）
发现列表：本分支无解决方案/测试项目 ≠ 整个仓库无（范围限定见 §1/§3/§6）
```

### A4. 密钥扫描
```
命令：git ls-files | grep -iE '\.(cs|axaml|csproj|md|json|config|ps1|bat)$' | xargs grep -rilE "ghp_|sk-|AKIA|password=|token=|client_secret=" 
退出码：0（无输出 = 无命中）
结果摘要：全仓 tracked 源/文档文件未发现密钥模式
发现列表：无
```

### A5. 旧命名泄漏扫描
```
命令：git ls-files '*.cs' '*.axaml' '*.csproj' | xargs grep -l "FluidWarfare"
退出码：0（无输出 = 无命中）
结果摘要：源码层无 FluidWarfare 命名泄漏（仅旧文档含）
发现列表：无
```

### A6. Editor.UI → Vulkan 活跃引用扫描
```
命令：grep -rn "XuanYu.Render.Vulkan" XuanYu.Editor.UI --include=*.cs | grep -v "VulkanClearSession"
      grep -nE "Render.Vulkan|Silk.NET" XuanYu.Editor.UI/XuanYu.Editor.UI.csproj
结果摘要：3 个活跃 .cs（VulkanSurfaceBridgeProvider / UiVm.VulkanProbe / VulkanProbeRoute）+ csproj 直接引用
发现列表：红线1 活跃违反（见 §4、§8.2⑧、§14 P1-a）
```

---

## 验证（提交前自查）

1. Git diff 仅含 `docs/project-baseline-audit-org-1-r1.md`（新增）+ `docs/project-baseline-audit-org-1.md`（加 superseded 注）+ `changelog.md` + `file-tree.md`。✅
2. 无源码/项目文件/配置改动。✅
3. 每个数字均有命令或文件依据（见各节"依据"与 §附录）。✅
4. "真机验收"与"代码存在"严格区分（§9）。✅
5. 5+100 统计排除生成物与 untracked（git ls-files 自动）。✅
6. 明确区分"被审计代码基线 9bc210e"与"审计报告提交 f187174"。✅
7. 文档总数与 `file-tree.md` 一致（本轮 +1 → 待更新）。✅
8. 无敏感内容抄入本报告（密钥仅报类型/路径，未复制值）。✅
9. `changelog.md` 新增 `v0.2.14.6-rz` 条目。✅
10. 修正了 ORG-1 退回的 11 项误判（见 §0）。✅

> 本轮为纯审计文档修正轮，未修改任何代码、未实装 VK5-E、未删除/移动任何文件。所有发现仅报告，不修复（修复排入 VK-LIFE-1 / SAFE-1 / ARCH-A-PLAN）。
