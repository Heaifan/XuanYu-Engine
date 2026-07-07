# 玄域引擎 · 开发硬规则（执行手册）

> 本文件只放**硬规则**，不含解释与动机。解释见同目录 `dev-rules-understanding.md`。
> 新 agent / AI 接手任何任务前，先过第 0 节红线清单，再按 1–9 节逐条自检。

---

## 0. 接手红线清单（每轮任务开始前必过）

1. 本轮任务名是什么（属于哪个里程碑 / 阶段）？
2. 本轮允许改哪些文件（先列清单）？
3. 本轮禁止做什么（先列禁止项）？
4. 是否涉及高频链路（PointerMoved / Drag / RenderFrame / Hover / Picking）？
5. 是否涉及 Vulkan / NativeHost / Surface / Swapchain 生命周期？
6. 是否涉及结构性变更（增 / 删 / 改名 / 移动文件目录）→ 需要同步 `file-tree.md` 吗？
7. 按范围纪律，是否需要新增 audit 文档（审计 → 修复 → 封版验证）？
8. 是否所有 `.cs` / `.axaml` ≤ 100 行（>100 必须有原因 / 债务登记 / 拆分计划）？
9. 是否 `dotnet build` 0 warning / 0 error？
10. 提交前 `git status` 是否干净（无随手 stash pop、无越界残留）？

---

## 1. 5+100 形态门禁

- `.cs` / `.axaml` 单文件 ≤ 100 行是**默认硬门禁**。
- > 100 行只能是**历史债务**或**明确审计过的临时例外**；禁止把"复杂"作为自然突破 100 行的理由。
- 超过 100 行必须先说明原因、列入债务或立即拆分；不得用 `partial class` 掩盖职责过大，不得为压行数把多职责挤一行。
- 单一职责优先于行数：**单职责 > 单文件 ≤100 行 > 目录规模美观**。
- 目录规模（<7 文件 / >10 拆二级）作为**建议与预警**，不与该硬门禁放在同一优先级；不得为减少目录文件数而把职责塞回大文件。

## 2. 依赖方向硬隔离

- `Core` 不依赖任何上层（UI / Runtime / Vulkan / Windows / Android / Avalonia）。
- `Editor.UI` **不得直接引用 `Silk.NET.Vulkan`**；**不得持有 `Vk` / `Instance` / `Device` / `Surface` / `Swapchain` 等 Vulkan 对象**；**不得直接控制 Vulkan 生命周期**。
- `Editor.UI` 允许：通过 `Render.Abstractions` 调用抽象能力；通过薄 `Route` 发起诊断请求；显示来自后端的探针结果；记录低频生命周期日志。
- Vulkan 只写在 `Render.Vulkan`；Avalonia 只在编辑器；不引入 Unity / Unreal / Godot / MonoGame。
- 目标依赖方向：
  - `Editor.UI` → `Render.Abstractions`
  - `Editor.Win`（平台宿主 / 组合根）→ `Render.Vulkan`
  - `Render.Vulkan` → `Render.Abstractions`
- **过渡期现实**：当前 `XuanYu.Editor.UI` 已含 `VulkanProbeRoute.cs` / `ViewportNativeHostRoute.cs`，属 VK1 最小探针的暂时接受；VK2 / VK3 前应改为经 `Abstractions` / 平台组合根装配，消除 UI 直接认识 Vulkan 实现。

## 3. 高频链路纪律（输入 / Gizmo / Viewport / Vulkan Resize 共同规则）

- `PointerMove`（高频）：只更新 `PreviewTransform`、请求重绘、更新轻量内存状态。
- `MouseUp`：Commit（写 `WorldState`、刷面板、写一条总结日志、提交 Undo）。
- `Esc`：Cancel（清 Preview、重绘一次、写取消摘要；不写 `WorldState`、不提交 Undo）。
- `PointerMove` **禁止**：写 `WorldState` / 刷 Inspector / Diagnostics / 写普通日志面板 / 提交 Undo / 重建 Swapchain / 堆积 `RenderRequest`。

## 4. 日志边界

- 日志总线（`EditorLogBus`）只接收**低频事实**（启动 / 打开 / 保存 / 工具切换 / 构建 / 低频生命周期）。
- 高频事件**先在源头合并**，再决定是否上报；折叠 / 节流 / Buffer 不能替代源头合并。
- 节流（每 100ms 最多一次 UI 刷新）只是低频日志 UI 刷新的**保护层**，**不是**允许 `PointerMoved` / `RenderFrame` / `DragPreview` 进入 `EditorLogBus` 的理由。
- 诊断不得阻塞输入 / 渲染 / UI 线程；禁同步 `Dispatcher.Invoke`、`Task.Wait/Result`，禁高频路径写文件 / 写 UI 日志。高频路径只走非阻塞 `DiagnosticSink.TryWrite` / `Debug.WriteLine` / `RingBuffer`。
- 启动期（Build / 构造 / 窗口未 Loaded / 项目未打开前）禁止把 UI 日志回调注入纯逻辑层。

## 5. Vulkan 阶段边界（比技术规则更硬）

- **VK1**：只探针，不 Surface。
- **VK2**：只 NativeHost / HWND 生命周期，不 Surface。
- **VK3**：只 Surface 生命周期，不 Swapchain 扩张。
- **VK4**：Swapchain 最小闭环。
- 防回潮：**VK2 不得夹带 Surface；VK3 不得夹带 Swapchain；VK4 不得夹带真实渲染循环。**
- 通用生命周期：资源创建即带销毁；`Swapchain` 唯一入口；Resize 先标记 dirty，下一帧 / 稳定后统一重建，跳过 0 尺寸 / 重复尺寸；所有 `VkResult` 必须保存并分类处理；UI 线程禁 `ulong.MaxValue` 无限等待。

## 6. 中文化铁律

- **英文保留**：命名空间、类名、方法名、文件名、测试方法名、`EngineError.Code`、程序内部枚举名。
- **中文保留**：异常 message、日志 message、编辑器提示、导出提示、验收结果、`file-tree.md`、`CHANGELOG.md`、开发任务说明。
- 示例：`Core.InvalidArgument`（码·英文）/ `参数无效。`（信息·中文）。

## 7. 范围与结构纪律

- 一次只处理一个里程碑 / 小子任务；编辑前先列计划新增 / 修改文件。
- 结构性变更（增 / 删 / 改名 / 移动文件目录）**必须同步更新 `file-tree.md`**；无变更要显式说明"本次无需更新"。
- 不超出当前任务范围；每个阶段末尾带"禁止项确认"清单（可勾选）。

## 8. 命名与品牌

- 总品牌：**玄域引擎 / XuanYu Engine**；战争子标识 **孙武引擎 / SunWu**（暂不引入核心命名空间）；游戏名 **兵无常势**（暂不改）。
- `FluidWarfare` 仅作历史代号语境，不得作当前正式品牌名；命名空间目标 `XuanYu.Engine.*`，禁旧前缀。
- 数据 / 资源文件可用领域前缀：`cfg_/dat_/scn_/rpl_/log_/mesh_/tex_/mat_/shd_/spv_/loc_`。
- 明确命名职责（`ScenarioEntityLoader`），禁 `cls_/fuc_/var_/obj_/str_/int_` 前缀，禁 `BingWuChangShiEngine/Bwc.*`。

## 9. 构建 / 测试 / 审计门禁

- `dotnet build` **0 warning / 0 error**。
- `CodeFileBudgetTests` 锁文件行数、生产白名单、`GlobalUsings ≤ 100` 等。
- 每阶段：**审计文档（现状 + 风险排序）→ 修复 → 封版验证报告**（末尾带禁止项确认 + Build / Test 结果 + 变更清单）。
- 冻结纪律：`ShellV2` 冻结后不回滚 / 不继续 / 不扩大；重启前必须人工确认路线。
- stash 纪律：不随手 `git stash pop`；先分类（A 进当前 / B 延后 / C 不进），只提取需要的文件独立提交。
