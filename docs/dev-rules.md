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
11. 是否涉及地基型架构决策？若涉及，是否已写清最终规模、复杂度增长、替换成本、扩散风险和最小正确实现？
12. 当前阶段 Entry Gate / Exit Gate 是否明确？入口条件不满足时，是否停止临时主链实现？

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
- 地基型架构必须采用“最小正确实现”，不得用“当前 Demo 小”“以后优化”让低扩展性路径进入正式主链。
- 空间查询、渲染数据流、状态所有权、生命周期、Undo 和存档等主链设计必须先通过长期扩展审计，再进入实现。
- 受控架构债务必须写明存在范围、禁止扩散范围、解决阶段和阻断条件。
- 用户确认的长期规则必须同轮写入治理文档；未经确认的推测性规则不得写入宪法。

## 10. 空间查询与 Picking 主链

- Picking 正式主路径禁止 `GetAllEntities` 后逐个遍历全场景。
- 禁止每次点击临时重建整个空间索引。
- 禁止 `PointerMoved` 默认持续 Picking。
- 禁止 Picking 直接读取 `Render.Vulkan` 或 `Vk*` 数据。
- 空间索引不得持有 Vulkan、Avalonia、Win32 HWND 或 UI 控件对象。
- 正式路径必须是：场景事实 → 增量维护空间查询索引 → 视口 / 相机事实 → 世界射线 → 索引裁剪候选 → Ray-AABB → 最近有效命中。
- 当前没有正式 Camera / View / Projection 契约时，禁止使用 Clip Space 临时破解；必须先建立渲染后端无关的视口 / 相机变换契约。
- 在 `Pointer -> WorldRay -> Spatial Query` 进入正式主链前，Render 必须消费同一套 `CameraState / ViewportState / ViewProjectionState`；禁止让 Vulkan 画面和 Picking 长期各自生活在两套空间事实里。

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

## 11. 坐标契约门禁

- World Space：右手系、`+Z` Up、XY 水平，`X × Y = Z`；正旋转使用右手规则。
- 禁止把世界 `+X` 或 `+Y` 写成所有系统共享的 Forward；Camera / Object Local / Asset / Geographic Forward 分别定义。
- 默认编辑相机、Picking、Gizmo 与 Vulkan Render 必须消费同一 `CameraState / ViewportState / ViewProjectionState`。
- 正高度 Vulkan Viewport 的 Y 差异只允许在 `Render.Vulkan` 组装渲染矩阵时转换 Core Projection 的副本；Picking 不得读取该副本或加入 Vulkan 翻轴。
- 修改 Camera / Projection / Screen-NDC / Gizmo 时，必须覆盖 Basis、World-Clip-World Round Trip、Center Ray、XYZ 分量与 Resize/DPI 测试。
- 发现 `-X`、`-Y`、`Swap(Y,Z)` 等视觉补丁时必须追溯根因，禁止用第二个补丁抵消第一个错误。

## 12. 工具状态与历史门禁

- `ActiveTool` 只表示持续编辑工具：选择、框选、移动、旋转、缩放。
- `Snap` 是 Toggle，撤销、重做、聚焦是 Command；它们不得覆盖或伪装成 `ActiveTool`。
- 顶部工具高亮、右上角工具文本、底部工具文本必须从同一个工具快照派生。
- 视口中显示的 Gizmo 必须对应当前 `ActiveTool` 的真实可操作能力；不可用工具不得显示其他工具的 Gizmo。
- 交互 Session 创建时，`SessionTool` 必须来自捕获开始瞬间的唯一 `ActiveTool` 快照，不得来自旧缓存、默认 TransformMode 或 UI 文案猜测。
- 未实现真实能力的工具不得偷偷退化执行已实现工具；例如 Rotate / Scale 未实现时不得进入 Move Session。
- Undo / Redo 必须恢复已提交的 Before / After Snapshot，不得重新模拟输入 Delta。
- Undo / Redo 不得产生新 History；新 Commit 后必须清空旧 Redo 分支。

## 13. Global World 与 Entity Registry 事实源

- 正式实体身份只能使用既有 `EntityId`；禁止为同一实体发明第二套长期 ID、UI ID、Render ID 或 Registry Key。
- `GlobalWorld -> EntityRegistry -> Entity State` 是实体生命周期唯一事实源；UI、Renderer、Hierarchy、Inspector、Snapshot、Picking、Gizmo 只能读取、投影或缓存派生快照，不得拥有第二份正式实体真相。
- `EntityRegistry` 的最小正式入口为 `Create`、`Destroy`、`Get`、`TryGet`、`Exists`；销毁已不存在或非法实体必须稳定失败，不得污染 Registry。
- 1000 实体冒烟必须覆盖创建、查询、删除、重复删除、稳定 key、缺失 key 与无污染，并记录创建时间、查询时间和内存变化基线；基线只作观察，不作为未审计性能门槛。
- 在 Partition、Spatial Index、Organization、Terrain、Streaming、Gameplay 或 ECS 接入前，必须先确认它们消费 `GlobalWorld / EntityRegistry` 的事实，不得反向成为实体生命周期 Owner。
- `SceneStateOwner` 只能承担 Scene 投影、编辑会话、Snapshot 聚合和派生空间索引维护；Transform Commit、Undo、Redo、Destroy 必须写回或查询同一个 `GlobalWorld / EntityRegistry` 实体事实，不得维护第二份正式 Transform。
