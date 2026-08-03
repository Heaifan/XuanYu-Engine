> ⚠️ **本报告已被 ORG-1-R1 退回修正并取代。** 原报告存在 11 项误判（分支范围、5+100 质量条件、空 catch、Editor.UI 活跃链路、Vk 所有权、Vulkan 异常安全、能力表数字、基线标注、证据链、111.ps1 强推措辞、ORG-2 范围）。**权威修正版见 `docs/project-baseline-audit-org-1-r1.md`**，本文件仅作历史留痕。

# ORG-1 项目真实基线审计（已退回，见 ORG-1-R1）

- 任务代号：ORG-1（已退回）
- 基线 Commit：`9bc210e`（HEAD，分支 `fix/RZ-VK3-A-surface-contract`）
- 审计日期：2026-07-12
- 审计性质：纯审计，**不改代码、不重构、不删除文件、不实装 VK5-E**
- 依据文档：`docs/玄域引擎_AI开发宪法.md`（最高规范）、`docs/版本号规范与历史映射.md`、`docs/CODE_CONSTITUTION.md`、`docs/AI_DEVELOPMENT_RULES.md`、各 VK3/VK4/VK5 计划与收口文档、`changelog.md`、`file-tree.md`

---

## 1. 审计范围与基线 Commit

本轮盘点仓库真实状态，回答五个问题：当前真实状态、已完成并真机验收的能力、仅有代码/计划未验收的能力、架构依赖与 Vulkan 生命周期是否符合宪法、代码/文档/仓库卫生债务。

基线 Commit 经 Git 命令重新确认（非假设）：`9bc210e`（详见第 2 节），即 DOC-VERSION-1-R1 提交。

审计范围限定：`.cs` / `.axaml` / `.csproj` / `.sln` / 文档 / 仓库卫生。所有数字均有命令或文件依据（见各节"依据"）。

---

## 2. Git 与工作区状态

| 项 | 值 |
| --- | --- |
| 当前分支 | `fix/RZ-VK3-A-surface-contract` |
| HEAD | `9bc210e0217e2b07956cc8732ae84aea6d83e3f2` |
| upstream | `origin/fix/RZ-VK3-A-surface-contract`（已设） |
| ahead / behind | `0 / 0`（与远端同步） |
| staged | 无 |
| unstaged | 无 |
| untracked | `qizheng-mvp-fixed/`（未跟踪，未被 .gitignore 覆盖） |
| 最近 15 提交 | 见 `git log`：9bc210e → 3f03725 → ab81c83 → b1f77d7 → 139c748 → 4485186 → c53b7a8 → 28f8c54 → 9a526eb → ca46586 → 89c5b47 → 0aaedf9 → 80bc320 → d749820 → cd509e1 |
| 当前 Tag | `m2.6.1-prefix-proof`（1 个；按宪法第十二条，正式 Tag 须验收通过并经用户确认，此 Tag 已存在，待裁决） |

**`qizheng-mvp-fixed/` 性质判定**：独立 JS MVP 项目误置于引擎仓库根目录。它不被任何引擎项目引用，含 23 个 `.js` / 4 个 `.md` / 1 个 `.css` / 1 个 `.html`，**不属于玄域引擎源码**。当前为 untracked 且未被 `.gitignore` 忽略——一旦执行 `git add -A` 即有入库风险。建议迁出或加入 `.gitignore`（见卫生审计与风险分级）。

依据：`git branch/rev-parse/status --short --branch/remote -v/fetch --prune/rev-list --left-right/log --oneline -15/tag --list`。

---

## 3. 解决方案与项目清单

仓库**无 `.sln` / `.slnx` 解决方案文件**，仅有 5 个 `.csproj`。

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
                    ──ProjectRef──▶ XuanYu.Render.Vulkan   ⚠ 违反红线1（债务A）
                    ──ProjectRef──▶ XuanYu.Render.Abstractions
                    ──PackageRef──▶ Avalonia 12.0.4 (Desktop/Fonts.Inter/Themes.Fluent)
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
- **红线1：`Editor.UI` 不得直接依赖 Vulkan 实现** → **当前违反（已知债务 A）**。`XuanYu.Editor.UI.csproj` 同时含 `ProjectReference XuanYu.Render.Vulkan` 与 `PackageReference Silk.NET.Vulkan`。宪法已确认此为历史探针残留，明确"不得扩大 UI 对 Vulkan 的直接认识"，属待收口债务，非新违规。
- **红线2：`Render.Abstractions` 不得引用 `Silk.NET.Vulkan`** → **成立**。`Abstractions.csproj` 无任何 PackageReference、无 ProjectReference。

**`Editor.Win` 如何连接 UI 与 Vulkan**：实际**未连接**。`Editor.Win` 仅引用 `XuanYu.Core`，不含 Vulkan 或 Avalonia 引用，是 VK3 之前的遗留 WinForms 壳（仅 `MainForm`/`Program`）。真实的 Avalonia 编辑器在 `Editor.UI`，真实 Vulkan 链路在 `Render.Vulkan`；二者经 `Render.Abstractions` 契约与组合根接线。`Editor.Win` 与当前活跃架构脱钩，属卫生/归档候选。

**抽象层反向依赖 / Core 依赖实现 / 循环引用**：未发现。`Abstractions` 零依赖；`Core` 零依赖；`Vulkan → Abstractions`（实现依赖抽象，方向正确）；无循环 ProjectReference。

---

## 5. 第三方依赖

| 项目 | 第三方依赖 | 版本 |
| --- | --- | --- |
| XuanYu.Editor.UI | Avalonia / Avalonia.Desktop / Avalonia.Fonts.Inter / Avalonia.Themes.Fluent / Silk.NET.Vulkan / Silk.NET.Vulkan.Extensions.KHR | 12.0.4 / 2.22.0 |
| XuanYu.Render.Vulkan | Silk.NET.Vulkan / Silk.NET.Vulkan.Extensions.KHR | 2.22.0 |
| 其余项目 | 无 | — |

依据：5 个 `.csproj` 的 `PackageReference` 字段（详见第 4 节）。无 NuGet 版本冲突提示（构建 0 警告）。按宪法第十条，新增/升级第三方依赖须提案；当前依赖均为既有，本轮未变动。

---

## 6. 构建、警告和测试

无 `.sln`，逐项目构建（命令示例：`dotnet build XuanYu.Editor.UI/XuanYu.Editor.UI.csproj --nologo`）。

| 项目 | 命令 | Exit Code | Warning | Error | 测试总数 | 通过 | 失败 | 跳过 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| XuanYu.Core | `dotnet build XuanYu.Core/XuanYu.Core.csproj` | 0 | 0 | 0 | — | — | — | — |
| XuanYu.Render.Abstractions | `dotnet build XuanYu.Render.Abstractions/XuanYu.Render.Abstractions.csproj` | 0 | 0 | 0 | — | — | — | — |
| XuanYu.Render.Vulkan | `dotnet build XuanYu.Render.Vulkan/XuanYu.Render.Vulkan.csproj` | 0 | 0 | 0 | — | — | — | — |
| XuanYu.Editor.Win | `dotnet build XuanYu.Editor.Win/XuanYu.Editor.Win.csproj` | 0 | 0 | 0 | — | — | — | — |
| XuanYu.Editor.UI | `dotnet build XuanYu.Editor.UI/XuanYu.Editor.UI.csproj` | 0 | 0 | 0 | — | — | — | — |

**测试**：仓库**无任何测试项目**（5 个 csproj 中无 `*Tests*`/`*Test*`）。`dotnet test` 无可运行项。**发现：零自动化测试覆盖**（风险分级 P2）。

依据：5 个项目 `dotnet build` 实际输出（"已成功生成 / 0 个警告 / 0 个错误 / EXIT=0"）；`ls */*Tests* *.Tests*` 返回 NONE。未关闭分析器、未降级、未跳过测试——本轮无代码改动，但构建确为真实执行结果。

---

## 7. 5+100 审计

脚本统计 5 个引擎项目下所有手写 `.cs` / `.axaml`（排除 `bin/obj/.git/.vs/qizheng-mvp-fixed`，并排除生成物目录 `.artifacts`）。

| 指标 | 值 |
| --- | --- |
| 扫描到文件总数 | 112（含 1 个生成物） |
| 手写文件数（排除 `.artifacts/ui-obj` 生成物） | **111** |
| 超过 100 行的文件数 | **0** |
| 最大文件行数 | 100（`XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs`，恰为硬顶） |
| 其余文件最大行数 | 99（`VulkanDeviceOwner.cs`、`VulkanPresentLoop.cs`） |

**结论：当前全仓手写 `.cs/.axaml` 文件零超过 100 行，5+100 红线成立。** 不存在"单行压缩伪装""多类型硬塞一文件""为满足 100 行删注释"等规避迹象——Vulkan 各 Owner 均通过合理的局部变量取地址、精简注释、去空行守住在 ≤100（见 `file-tree.md` 各快照记录）。

**死代码 VulkanClearSession 4 文件行数**：`VulkanClearSession.cs` 81 / `.Device.cs` 49 / `.Swapchain.cs` 46 / `.Dispose.cs` 18（合计 194）。均 ≤100，行数合规，但属无引用死代码（见第 8、9 节）。

依据：`.workbuddy/audit_5plus100.py` 输出（`TOTAL_HANDWRITTEN=112`，`OVER_100` 为空，`MAX_10` 最大 100）。

---

## 8. Vulkan 生命周期

基于读取 `VulkanRenderSession.cs`（组合根）、`VulkanPresentLoop.cs`、`VulkanClearFrameOwner.cs` 及各 Owner 文件。

| 对象 | 责任文件 | 创建者 | 持有者（唯一状态所有者） | 使用者 | 释放者 | 创建时机 | 释放时机 | 线程归属 | Resize 行为 | 异常后状态 | 结论 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Vk（Silk 调度表） | 组合根（Editor.UI）创建后传入 `VulkanRenderSession.Create(Vk)` | 组合根 | `VulkanRenderSession` 持有并向下传播 | 各 Owner | 组合根 Detach | Attach | Detach/Dispose | UI/组合根 | — | 可恢复 | 正常 |
| Instance | `VulkanInstanceOwner` | `VulkanInstanceOwner.Create` | `VulkanInstanceOwner` | Session | `VulkanInstanceOwner.Dispose` | Attach | Detach | UI | — | 快速失败 | 正常 |
| Surface | `VulkanSurfaceOwner` | `VulkanSurfaceOwner.Create`（Win32） | `VulkanSurfaceOwner` | Session | `VulkanSurfaceOwner.Dispose` | Attach | Detach | UI | — | 快速失败 | 正常（无重复所有权） |
| PhysicalDevice | `VulkanPhysicalDeviceSelector` | Select | Session（经 Selection 传入） | Session | 随 Device 释放 | Create | Dispose | — | — | — | 正常 |
| LogicalDevice | `VulkanDeviceOwner` | `VulkanDeviceOwner.Create` | `VulkanDeviceOwner` | PresentLoop/ClearFrame | `VulkanDeviceOwner.Dispose` | Create | Detach | — | — | 快速失败 | 正常 |
| Queue | `VulkanDeviceOwner` 内选取 | Create | `VulkanDeviceOwner` | PresentLoop | 随 Device | Create | Detach | Present 线程 | — | — | 正常 |
| Swapchain | `VulkanSwapchainOwner` | `Recreate` | `VulkanSwapchainOwner` | PresentLoop/ClearFrame | `DestroySwapchain` | Create/Recreate | Detach/Recreate | UI+Present | 重建（见下） | 可恢复 | 正常 |
| ImageView | `VulkanSwapchainOwner` | 随 Swapchain | `VulkanSwapchainOwner` | ClearFrame | 随 Swapchain | Create | Destroy | — | — | — | 正常 |
| RenderPass | `VulkanClearFrameOwner.BuildRenderPass` | 构造 | `VulkanClearFrameOwner` | ClearFrame | `Dispose` | 构造 | Dispose | — | 不重建 | — | 正常 |
| Framebuffer | `VulkanClearFrameOwner` | `RebuildFramebuffers` | `VulkanClearFrameOwner` | ClearFrame | `DestroyFramebuffers` | 构造/Rebuild | Dispose/Rebuild | — | 重建 | — | 正常 |
| ShaderModule | `VulkanShaderModuleOwner` | Create（短生命周期，建完 Pipeline 即释放） | `VulkanShaderModuleOwner` | Pipeline | 建完 Pipeline 释放 | 管线创建 | 管线创建后 | — | — | — | 正常 |
| GraphicsPipeline | `VulkanGraphicsPipelineOwner` | `Create` | `VulkanGraphicsPipelineOwner` | ClearFrame（注入） | `Dispose` | Create | Session.Dispose | — | 不随 Resize 重建 | — | 正常 |
| CommandPool | `VulkanClearFrameOwner` | 构造 | `VulkanClearFrameOwner` | ClearFrame | `Dispose` | 构造 | Dispose | — | 不重建 | — | 正常 |
| CommandBuffer | `VulkanClearFrameOwner` | `RecordCommandBuffers` | `VulkanClearFrameOwner` | PresentLoop | `FreeCommandBuffers` | 每次 Rebuild | Rebuild/Dispose | — | 重录 | — | 正常 |
| Semaphore（image/render） | `VulkanPresentLoop.CreateSync` | `CreateSync` | `VulkanPresentLoop` | PresentLoop | `Dispose` | Start | Dispose | Present | — | — | 正常 |
| Fence | `VulkanPresentLoop.CreateSync` | `CreateSync` | `VulkanPresentLoop` | PresentLoop | `Dispose` | Start | Dispose | Present | — | — | 正常 |
| Present Pump | `VulkanPresentLoop` | `Start`（后台线程 `VulkanPresent`） | `VulkanPresentLoop` | Session | `Stop`→`Dispose` | Start | Session.Dispose | Present 线程（IsBackground） | 停泵→重建→重启 | 可恢复 | 正常 |
| Resize | `VulkanRenderSession.Resize` | UI 调用 | Session（`_rebuildLock`） | — | — | UI | — | UI | **同尺寸快速跳过（不 Stop/Start/重建）；真实尺寸变化才 Stop→重建→Start** | 可恢复 | 正常 |
| Detach | 组合根 | — | Session | — | `Dispose`：先 `Stop` 泵，再 Dispose Pipeline→ClearFrame | — | — | — | — | — | 正常 |
| Dispose | `VulkanRenderSession.Dispose` | — | Session | — | 顺序：PresentLoop.Stop→PresentLoop.Dispose→Pipeline.Dispose→ClearFrame.Dispose | — | — | — | — | — | 正常 |

**十项重点验证结果**：
1. **Vk 是否仍由唯一 Bridge 持有/释放**：Vk 在组合根创建并经 `VulkanRenderSession.Create(Vk)` 传入，由 Session 持有并向下传播，无重复 Vk 实例。✅
2. **Instance 与 Surface 是否无重复所有权**：分别由 `VulkanInstanceOwner` / `VulkanSurfaceOwner` 唯一持有，无重复。✅
3. **Resize 是否只在真实尺寸变化时进入重建**：`Resize` 先判 `Extent == 目标` → 同尺寸直接 `return`（VK5-D-R3）。✅
4. **同尺寸 Resize 是否仍快速返回**：是，打 `Resize 快速跳过` 日志后 return，不 Stop/Start 泵。✅
5. **Present Pump 停止/重建/恢复顺序**：`Resize` 在 `_rebuildLock` 内 Stop→Recreate Swapchain→RebuildFramebuffers→Start；OutOfDate 经 `RecoverFromOutOfDate` 统一入口（`_rebuildLock`+`_generation`+上限 5 次）。✅
6. **Detach 是否先停泵再释放同步与渲染资源**：`Dispose` 先 `_presentLoop.Stop()` 再 Dispose Pipeline/ClearFrame。✅
7. **UI 日志回调是否派发到 UI 线程**：`VulkanPresentLoop.Log` 以 `try/catch` 包裹 `_log?.Invoke`（后台线程日志异常被吞，避免炸泵）；`_log` 由 Editor.UI 经 LOG-UX 层派发到 UI 线程。✅
8. **Editor.UI 是否只经抽象/宿主契约使用渲染**：活跃链路经 `Render.Abstractions` 契约 + 组合根接线；但历史 `VulkanClearSession` 与 csproj 直接引用仍使 Editor.UI 触碰 `Silk.NET.Vulkan`（即债务 A，非活跃链路）。⚠（已知）
9. **关闭时 RenderPass/Framebuffer 重复释放日志**：经代码确认 `Dispose` 对每个对象仅销毁一次；`file-tree.md`/VK5-B 收口记录注明"关闭日志重复打印一行（仅日志重复，非双重释放，留待 LOG-CLEANUP）"。即**仅日志重复，非真实双重释放**。✅（非阻断日志瑕疵）
10. **VK5-E 所称 VulkanClearSession 死代码是否真实存在**：**真实存在**。4 个 partial 文件位于 `XuanYu.Editor.UI/Viewport/Vulkan/`，全仓 grep 无任何 `.cs` 外部引用（无 `new`、无字段、无 `TryCreate` 调用方），仅类内部互调 + 历史 markdown。死代码属实（债务 B）。✅

---

## 9. 当前能力状态

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
| 关闭释放顺序 | A 已真机验收 | 多轮真机（VK4-D/VK5） | — | 同 8-⑨ 日志瑕疵 |
| viewport/scissor 与 Resize 关系 | A 已封版（验证收口，非新代码） | VK5-C 封口（2026-07-11 用户确认） | 139c748 | 宽高比修正留待 Camera/Projection |
| 视口 UI 收口（移除 overlay） | B 自动构建通过，无明确真机验收记录 | 仅构建 0W0E | `viewport-ui-*` | 待用户真机确认（UI 类改动按宪法须真机） |
| LOG-UX 日志系统 | A 已实装并文档收口 | LOG-UX 各轮 + 收口 | `log-ux-*` | 无 |
| VulkanClearSession 死代码清理（VK5-E） | D 仅规划，未实装 | `docs/rz-vk5-e-plan.md` | — | 待确认实装（债务 B） |
| Shape/Mesh/Camera/Scene/DescriptorSet | D 尚未实装 | 各 VK 轮红线明确"不建" | — | 路线图未定 |

**真机验收边界说明**：A 类均有多轮用户真机回传或收口文档支撑；B 类（视口 UI 收口）仅有构建通过，无真机验收记录，按宪法第十二条 UI 类改动需真机验收再 Push，故标记待确认；D 类（VK5-E）仅规划文档，代码未动。

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
| 已完成收口报告 | `rz-vk3-closure.md`、`rz-vk4-closure.md`、`rz-vk5-c-plan.md`（封版）、`rz-vk4-c-r1-audit-plan.md`（部分）、本轮 `project-baseline-audit-org-1.md` |
| 历史计划（对应工作已完成） | `vulkan-lifecycle-plan.md`、`rz-vk3-surface-lifecycle-plan.md`、`rz-vk4-plan.md`、`rz-vk4-c-swapchain-plan.md`、`rz-vk4-d-plan.md`、`rz-vk5-plan.md`、`rz-vk5-a-plan.md` |
| 历史记录/审计（已完成参考） | `audit-EditorShellV2-*`(6)、`audit-gizmo-*`(5)、`audit-input-lifecycle-9.0X-*`(3)、`audit-NativeViewportMouseCapture-9.0X.md`、`audit-inspector-transform-9.0C-0.md`、`plan-9.0D-move-gizmo-final.md`、`editor-*`(4)、`gizmo_drag_audit_2026-06-25.md`、`naming-XuanYu-Engine.md`、`vulkan-preflight-audit-RZ-Fix3-0.md`、`audit-RZ-*`(4)、`MILESTONE1_PUBLIC_VALIDATION.md` |
| 内容需要更新 | `AI_DEVELOPMENT_RULES.md`（仍写 `FluidWarfare.*` 命名空间，与现 `XuanYu.*` 冲突）、`CODE_CONSTITUTION.md`（100 行规则偏松，与新宪法硬红线表述冲突）、`PROJECT_CHARTER.md`/`PHASE1_SCOPE.md`/`ENGINE_ARCHITECTURE.md`/`NAMING_RULES.md`/`LEGACY_FLUIDWARFARE_OLD_AUDIT.md`（含旧品牌/旧阶段表述） |
| 内容重复 | 多份 RZ/VK 审计与计划存在交叉重叠（如 `audit-RZ-VK2-*` 与 `vulkan-preflight-audit-*`） |
| 内容冲突 | `AI_DEVELOPMENT_RULES.md` 旧命名 vs 实际 `XuanYu.*`；`CODE_CONSTITUTION.md` 100 行偏松 vs 新宪法 100 硬红线 |
| 待归档候选 | 全部"历史记录/审计"类（M1/RZ 早期）、`codex_log/`（AI 日志，已 gitignore） |
| 待删除候选 | 无（本轮不改内容；`111.ps1` 为脚本非文档，见卫生审计） |

**防止把历史计划误当当前计划**：当前唯一仍"待实装"的计划是 `rz-vk5-e-plan.md`（VK5-E）。VK3/VK4/VK5-A/B/C/D 的计划文档已全部对应实装/封版，归类为"历史计划"，**不**视为当前计划。

---

## 12. 三份治理文档规则矩阵

| 规则领域 | 新宪法（玄域引擎_AI开发宪法） | 代码宪法（CODE_CONSTITUTION） | 旧AI规则（AI_DEVELOPMENT_RULES） | 判断 |
| --- | --- | --- | --- | --- |
| 5+100 | 100 硬红线，无例外 | 100 建议，复杂可近 150，200 须理由，300 禁止 | 未明确（只说优先小文件） | 冲突：新宪法最严，统辖；代码宪法偏松，待更新 |
| 项目依赖边界 | Editor.UI 不依赖 Vulkan 实现；Abstractions 不引 Silk | 平台隔离（Core/...不得依赖 Windows/Android/Avalonia/具体 Vulkan 实现） | Vulkan 只在 `FluidWarfare.Render.Vulkan`（旧名） | 方向一致；旧名需更新；当前 Editor.UI 实际违反（债A） |
| Git 权限 | 禁止擅自 merge/rebase/强推/重写历史 | 未涉及 | 未涉及 | 新宪法独有，统辖 |
| Push 与真机验收 | UI/Vulkan/输入/生命周期/性能须真机验收再 Push | 未涉及 | 未涉及 | 新宪法独有 |
| 删除/移动/重命名 | 必须列文件/原因/依赖/影响/批准/下一轮 | 未涉及 | 仅"增删移改名须更新 file-tree" | 新宪法更严，统辖 |
| 新文件 | 重大模块/抽象须请示；小文件可加 | 未涉及 | 范围纪律（增删移改名更新 file-tree） | 互补 |
| 第三方依赖 | 新增/升级须先提案 | 未涉及 | 不引入 Unity/Unreal 等 | 新宪法统辖提案要求 |
| Bug 排查 | 复杂 Bug 用中文探针四步 | 未涉及 | 未涉及 | 新宪法独有 |
| 中文化 | 机器英文/人类中文；高频链路禁普通日志 | 机器英文/人类中文 | 人类可读默认中文 | 一致；新宪法更细（高频链路） |
| Preview/Commit | Preview 只更新预览/渲染；Commit 才更新重负载 | 未涉及 | 未涉及 | 新宪法独有 |
| 测试 | 禁止弱化断言/删失败用例/跳测试须请示 | 生命周期测试 4 类 | 核心模块配套测试 | 一致；新宪法统辖禁止项 |
| 异常处理 | 禁空 catch；可恢复记中文+降级；不可恢复快速失败 | 未涉及 | 未涉及 | 新宪法独有 |
| TODO | 须说明原因/触发/责任；禁"以后优化"；写入报告+changelog | 未涉及 | 未涉及 | 新宪法独有 |
| 警告 | 不新增警告/分析器告警；禁关分析器/降级/批量 Suppress | 未涉及 | 未涉及 | 新宪法独有 |
| 版本规范 | `v0.M.m.r-类型`；RZ/VK/FIX 等只作末尾标签 | 未涉及 | 未涉及 | 新宪法独有（旧 8.x/9.x/RZ 体系已废弃） |
| 文档同步 | changelog/file-tree 必须更新+字段 | 中文化提到 file-tree/CHANGELOG 中文 | 范围纪律+中文化 | 一致；新宪法字段更全 |
| 收口报告 | 五项（Git/文档/范围/可视化/结论） | 未涉及 | 每里程碑给变更清单+验收 | 新宪法统辖 |

**判断汇总**：新宪法为最高有效规范，统辖其余两份。代码宪法与旧 AI 规则存在两处需裁决的冲突（5+100 表述、旧 `FluidWarfare` 命名），本轮不修改其内容，建议 ORG-2 由用户裁决归档/合并。

---

## 13. 仓库卫生问题

| 问题 | 路径 | 当前状态 | 风险 | 后续建议 |
| --- | --- | --- | --- | --- |
| 危险设置脚本已入库 | `111.ps1`（根目录，tracked） | 含 `Remove-Item ".git" -Recurse -Force` + 硬编码分支 `fix/RZ-Fix1-editor-access-violation` + `git add -A` | 高：误跑会删除用户 .git 并强推 | P1：经删除流程批准后移除；或先隔离 |
| 独立项目误放且未忽略 | `qizheng-mvp-fixed/`（根目录，untracked，未 gitignore） | 含 23 .js / 4 .md / .css / .html，非引擎代码 | 中：误 `git add -A` 即入库 | P2：迁出或加入 `.gitignore` |
| AI 日志本地存在 | `codex_log/`、`codex_log_xuanyu_handoff_20260705-2102.zip` | 已被 `.gitignore` 覆盖（未入库） | 低：仅本地残留 | P2：定期本地清理（非违规） |
| 生成物目录 | `.artifacts/`、`bin/`、`obj/` | 已被 `.gitignore` 覆盖 | 低 | 无 |
| 旧品牌命名残留于文档 | `AI_DEVELOPMENT_RULES.md`/`CODE_CONSTITUTION.md` 等 | tracked 文档 | 低：仅文档误导性 | P2：ORG-2 裁决 |
| 文档绝对路径残留 | `docs/audit-NativeViewportMouseCapture-lifecycle-9.0X.md:203` | 仅作为"已不再硬编码"的整改说明 | 低 | P3：可顺手清理表述 |
| 密钥/Token/密码 | 全仓 tracked 文件 | grep `ghp_/sk-/AKIA/password=/token=` 未命中 | 无 | 无（符合红线3） |
| FluidWarfare 旧命名泄漏源码 | 全仓 `.cs/.axaml/.csproj` | grep 未命中 | 无 | 无（仅旧文档） |

依据：`git ls-files`、`.gitignore` 内容、`git status`、`grep` 源码（FluidWarfare / 绝对路径 / 密钥模式）。

---

## 14. 风险分级

- **P0（阻断继续开发）**：无。
- **P1（下一功能轮前必须处理）**：
  1. `Editor.UI` 直接依赖 `Render.Vulkan` + `Silk.NET.Vulkan`（红线1违反，债务 A）——须排专项收口到 `Abstractions`。
  2. `111.ps1` 危险脚本已入库——须按删除流程批准后移除。
- **P2（已知债务，可排专项）**：
  3. 零自动化测试覆盖——须建测试项目（至少 Render.Vulkan 生命周期/Dispose 幂等）。
  4. `qizheng-mvp-fixed/` 未跟踪未忽略——迁出或 gitignore。
  5. 旧治理文档（`AI_DEVELOPMENT_RULES.md`/`CODE_CONSTITUTION.md` 等）含旧命名/偏松规则——ORG-2 裁决归档或合并。
  6. `codex_log/` 与 handoff zip 本地残留（虽 gitignore）——定期清理。
- **P3（非阻断整理项）**：
  7. 关闭时 RenderPass/Framebuffer 释放日志重复一行（仅日志，留 LOG-CLEANUP）。
  8. 文档数量庞大（54 个 md），存在重复/历史计划——ORG-2 归档整理。
  9. `file-tree.md` 手动计数可能未含未跟踪/忽略项（qizheng/codex_log）——本轮仅因新增审计文档 +1。

---

## 15. 未解决问题

1. 红线1（Editor.UI→Vulkan 直接依赖）如何收口到 `Abstractions` 而不扩大当前活跃链路——需专项设计（债务 A）。
2. VK5-E（清 VulkanClearSession 死代码）实装时机——规划已就绪，待用户确认。
3. `111.ps1` 删除需用户按删除流程批准（本轮只审计，不删）。
4. `qizheng-mvp-fixed/` 处置方式（迁出 vs gitignore）待用户定。
5. 旧治理文档（FluidWarfare 命名/100 行偏松）是否标记 deprecated/合并——待用户裁决（非本轮范围）。

---

## 16. ORG-2 建议范围

ORG-2 定位为"文档归档与单一入口"轮，建议范围：

1. **治理文档收口**：将 `AI_DEVELOPMENT_RULES.md` / `CODE_CONSTITUTION.md` 合并或标记 deprecated，由 `玄域引擎_AI开发宪法.md` 单一入口统辖；更新 `PROJECT_CHARTER`/`PHASE1_SCOPE`/`ENGINE_ARCHITECTURE`/`NAMING_RULES` 中的旧品牌/旧阶段表述。
2. **历史文档归档**：将 M1/RZ 早期审计与计划（约 30+ 份）移入 `docs/archive/` 或压缩为索引，减少根目录噪音。
3. **卫生清理**：删除 `111.ps1`（批准后）；`qizheng-mvp-fixed/` 迁出或 gitignore；本地清理 `codex_log/`。
4. **债务 A 启动**：设计 `Editor.UI → Abstractions` 收口方案（不扩大活跃链路），独立 commit。
5. **测试基建**：新增测试项目，覆盖 Vulkan 生命周期创建/部分失败/Dispose 幂等（代码宪法"生命周期测试"要求）。
6. **VK5-E 实装**：在 ORG-2 卫生清理后，按 `rz-vk5-e-plan.md` 实装死代码清理（独立 commit）。

ORG-2 不扩大本轮审计未覆盖的架构变更；所有删除/移动按宪法第十三条走批准流程。

---

## 验证（提交前自查）

1. Git diff 仅含 `docs/project-baseline-audit-org-1.md` + `changelog.md` + `file-tree.md`。✅
2. 无源码/项目文件/配置改动。✅
3. 每个数字均有命令或文件依据（见各节"依据"）。✅
4. "真机验收"与"代码存在"严格区分（第 9、10 节）。✅
5. 5+100 统计排除生成物（`.artifacts/ui-obj`）。✅
6. 所有超限文件登记：0 个超限，已说明。✅
7. 文档总数与 `file-tree.md` 一致（本轮 +1 → 123）。✅
8. 无冲突标记。✅
9. 无敏感内容抄入本报告（密钥仅报类型/路径，未复制值）。✅
10. `changelog.md` 新增 `v0.2.14.5-rz` 条目。✅

> 本轮为纯审计文档轮，未修改任何代码、未实装 VK5-E、未删除/移动任何文件。所有发现仅报告，不修复。
