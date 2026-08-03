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
  - `Editor.App`（组合根 / 依赖装配）→ `Render.Vulkan`
  - `Editor.Win`（Windows 平台宿主，仅引用 `Core`）
  - `Render.Vulkan` → `Render.Abstractions`
  - `Editor` → `Core` / `World`（编辑器领域规则层，仅引用基础与 World；不得引用 `Editor.UI` / Avalonia / `Render.Vulkan` / `Silk.NET.Vulkan`）
  - `Editor.UI` → `Editor`（界面与输入适配消费编辑器规则，不得反向让 Editor 依赖 UI）
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
- 用户可见日志表格必须保持“时间 / 级别 / 来源 / 模块 / 消息 / 详情”顺序；普通日志不得把 `ARCH-*` / `WORLD-*` 阶段代号当作运行时模块名。
- 普通用户日志优先中文化；内部函数名、线程字段和英文调试 key 不得直接暴露到 UI 日志主文案，保留诊断时必须转为中文阶段与中文字段。

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
- 树节点、Inspector 字段和示例实体名必须走中文显示映射；禁止为中文化显示而重命名内部 C# 标识、实体 key 或公共 API。
- Project Tree 与 Hierarchy Tree 使用同一套分支线、缩进、行高、悬停、选中态和矢量路径图标，不得用 emoji 或纯字符替代正式图标。

## 6.1 人工 / 真机测试 IPO 门禁

- 人工测试、真机验收、截图验收必须使用“序号 / 路径 / 输入 I / 过程 P / 输出 O”。
- 路径必须写当前 UI 的真实中文路径和按钮名，例如“顶部工具栏 → 视图 → 查看全部”。
- 禁止用 `Frame Selected`、`Frame All`、`Orbit`、`Pan` 等内部英文名替代界面文字。
- 禁止“测试正常”式结论；输出 O 必须能由画面、状态、日志或数据明确判断。

## 7. 范围与结构纪律

- 一次只处理一个里程碑 / 小子任务；编辑前先列计划新增 / 修改文件。
- 结构性变更（增 / 删 / 改名 / 移动文件目录）**必须同步更新 `file-tree.md`**；无变更要显式说明"本次无需更新"。
- 不超出当前任务范围；每个阶段末尾带"禁止项确认"清单（可勾选）。
- 地基型架构必须采用“最小正确实现”，不得用“当前 Demo 小”“以后优化”让低扩展性路径进入正式主链。
- 空间查询、渲染数据流、状态所有权、生命周期、Undo 和存档等主链设计必须先通过长期扩展审计，再进入实现。
- 受控架构债务必须写明存在范围、禁止扩散范围、解决阶段和阻断条件。
- 用户确认的长期规则必须同轮写入治理文档；未经确认的推测性规则不得写入宪法。
- Git 开发分支必须跟随主里程碑切换：进入新的 `ARCH` / `WORLD` / `QZ` 主阶段时，从上一里程碑已验收且工作区干净的 HEAD 创建对应分支并推送。
- 禁止长期沿用语义过时的旧里程碑分支；禁止为换分支 rebase、force push、移动已有 commit 或删除旧分支。
- 凡有落库提交，最终回复必须直接展示本轮 SVG、给出仓库路径，并提供完整纯文本 SVG 源码或可访问源码；禁止只说“已生成 SVG”。

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

### 8.1 里程碑与轮次代号

- `R` 只表示 `Round`，即可独立定义目标、范围、自动门禁、真机验收和 `CLOSED` 结论的正式开发轮次。
- `D` 表示 Round 内正常开发步骤；涉及新 Schema、新依赖、新资产边界或重要架构决策时，`D0` 可用于审计、合同、依赖和边界冻结。
- `A` 只表示正式真机验收批次；自动测试、构建、架构守卫、静态检查和代码审计不得命名为 `A`。
- `F` 只用于正式验收 FAIL 后需要追加代码修复的批次；普通开发、编译错误、自动测试首次暴露问题和文档措辞修正不得命名为 `F`。
- `CLOSED` 是 Round 最终状态，不是层级；写作 `WORLD-C-R4：CLOSED`。
- 自 `WORLD-C-R4` 起禁止双 `R` 命名，例如 `WORLD-C-R4-R1`；尚未开始的计划改为 `WORLD-C-R4-D1` / `F1` / `A1`。
- 历史记录不追溯重命名；`WORLD-C-R3-R8` 等已落库名称作为审计事实保留。

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
- 运行期 `EntityId` 默认单调递增，Destroy 后不得立即复用旧 ID；禁止为避免简单计数而引入 GUID 或复杂分布式 ID。
- `GlobalWorld -> EntityRegistry -> Entity State` 是实体生命周期唯一事实源；UI、Renderer、Hierarchy、Inspector、Snapshot、Picking、Gizmo 只能读取、投影或缓存派生快照，不得拥有第二份正式实体真相。
- `EntityRegistry` 的最小正式入口为 `Create`、`Destroy`、`Get`、`TryGet`、`Exists`；销毁已不存在或非法实体必须稳定失败，不得污染 Registry。
- 1000 实体冒烟必须覆盖创建、查询、删除、重复删除、稳定 key、缺失 key 与无污染，并记录创建时间、查询时间和内存变化基线；基线只作观察，不作为未审计性能门槛。
- 在 Partition、Spatial Index、Organization、Terrain、Streaming、Gameplay 或 ECS 接入前，必须先确认它们消费 `GlobalWorld / EntityRegistry` 的事实，不得反向成为实体生命周期 Owner。
- `SceneStateOwner` 只能承担 Scene 投影、编辑会话、Snapshot 聚合和派生空间索引维护；Transform Commit、Undo、Redo、Destroy 必须写回或查询同一个 `GlobalWorld / EntityRegistry` 实体事实，不得维护第二份正式 Transform。
- `SceneStateOwner` 的派生空间索引自第 15 节起列为受控架构债务（与 `WorldQuery` 双轨并存）；ARCH-WORLD-R2 收敛前禁止新增消费者，收敛后该职责移除。

## 14. World Query 与 Spatial Index 门禁

- `SpatialIndex` 只能是从 `GlobalWorld` 实体位置 / Bounds 派生出的查询加速结构，不得成为 Entity、Position、Region 或 Activity 的事实源。
- 正式 `World Query` 路径禁止偷扫 `GlobalWorld.Entities` / Registry；O(N) 暴力扫描只允许作为自动测试 Oracle。
- `GlobalWorld` 正式 Position Commit 后，必须先更新 Partition 派生事实，再更新 Spatial Index 派生事实。
- `QueryRadius` / `QueryBounds` 返回 `EntityId` 集合；调用者必须回到 `GlobalWorld` 查询正式 Entity State。
- `Region` 解决管理、Activity、Streaming 边界；`Spatial Cell / Node` 解决查询加速；禁止把 `Region == Spatial Cell` 写成长期架构。
- 当前 R3 不上 Octree / BVH 大工程；允许最小可替换索引实现，但接口必须保留未来替换空间。

## 15. 物理分层与归属门禁

- 新增类型先判定归属层（Core / World / Editor / Render.Abstractions），禁止默认塞进 `XuanYu.Core`。
- `XuanYu.Core` 禁止新增 World / Scene / Viewport / Picking / Gizmo / Camera / History / Transform Session 类型。
- `EntityId` 禁止加入 Generation / Revision；实体身份术语只用 `EntityId`，不用 `EntityKey`。
- 空间索引唯一权威：禁止在 `GlobalWorld → SpatialIndexOwner → WorldQuery` 之外新增第二套空间查询索引。
- `SceneRenderSnapshot` 等 Snapshot 是派生表现边界 DTO，不得携带 Editor 相机创建后门；相机只能由 Editor/View 以 `CameraState` 传入。
- 归属迁移按 `ARCH-WORLD-R0 → R5` 序列执行（`docs/arch-world-layer-attribution.md`），每轮 build 0W0E + 全量测试 + arch-a-guard 通过后 commit；禁止一轮全搬、禁止跨轮夹带。
- R1 完成前禁止在 Core 继续扩张 World 概念新类型；确需新增必须先在 `docs/arch-world-layer-attribution.md` 归属总表登记裁定。

---

## 16. .NET 构建、测试与子进程生命周期（GOV-DOTNET-R1）

### 16.1 稳定验证模板（可直接复制）

```powershell
Set-Location 'E:\MyDoc\project-VSCode\XuanYuEngine'

dotnet build-server shutdown
$env:MSBUILDDISABLENODEREUSE = "1"

dotnet restore .\XuanYu.Engine.slnx

dotnet build .\XuanYu.Engine.slnx `
  --no-restore `
  -m:1 `
  -nr:false `
  -p:BuildInParallel=false `
  -p:UseSharedCompilation=false

dotnet test .\XuanYu.Core.Tests\XuanYu.Core.Tests.csproj `
  --no-build `
  --no-restore

dotnet test .\XuanYu.World.Tests\XuanYu.World.Tests.csproj `
  --no-build `
  --no-restore

dotnet build-server shutdown
```

### 16.2 规则要点

- 这是一套**稳定默认模板**，不代表所有测试项目必须每轮全部运行；具体测试范围仍按本轮改动决定，但所有被选中的测试必须**串行**。
- 验证起止必须 `dotnet build-server shutdown`；必须设置 `MSBUILDDISABLENODEREUSE=1`。
- 解决方案只完整构建一次，后续测试一律 `--no-build --no-restore`。
- 单命令 ≤ 5 分钟，连续 2 分钟无输出立即停；环境失败（`Access is denied` / obj·bin 写锁）优先判环境阻断，不得伪装代码失败。
- 沙箱禁止进程管理时（LOLBin 拦 `tasklist` / `Stop-Process`），停止执行、请用户手动清进程或重启，不得绕过。
- 同一失败命令仅环境真实变化后重试一次；changelog 只记本轮真实退出码。

---

## 17. 开发轮流程与关键路径治理（GOV-FLOW-R1）

> 因 WORLD-B-R4 收口期暴露“目标不冻结即扩围、调查无限考古、快速验证滞后、文档对账拖慢落库、自动测试通过即误判 CLOSED”而设立（v0.2.20.13-fix）。与 §16 生命周期治理互补；冲突时以 §16 + 本章共同约束为准。详细条款见 `玄域引擎_AI开发宪法.md` 第十九条《任务冻结》、第二十条《范围控制》、第三十四条《串行验证》。

### 17.1 稳定流程模板（每轮开工前冻结）

每轮开始必须先把以下五项写进计划，禁止边做边扩：

```text
主要目标   ：最多 3 项
允许范围   ：文件清单
验收门禁   ：明确可判定的通过条件
停止条件   ：达到后必须停止修改
禁止事项   ：明确不做什么
```

固定状态推进（禁止在状态间来回反复）：

```text
调查 → 实装 → 针对性快速验证 → 正式完整门禁 → 最小文档同步 → commit + push → 等待用户验收
```

进入 `commit + push` 后禁止继续加功能或重开全仓库审计。

### 17.2 规则要点

- **冻结目标**：新问题只在“会导致错误实现 / 破坏架构边界 / 造成审计失真 / 使本轮功能不成立”时方可扩围；普通措辞、历史漏登记、非阻断视觉偏好不得拖慢主线。
- **根因链闭合即停**：调查只回答“问题产生点 → 合同传递 → 失败表现 → 测试遗漏”，闭合后停止搜索，禁止无目标全仓库考古。
- **快速验证前置**：改合同→编译相关项目；改 Vulkan/Shader→编译渲染项目；改交互→跑相关测试；最后统一一次正式门禁。快速验证不能替代正式门禁，但必须尽早暴露漏 `using`、签名不一致、合同错误。
- **正式门禁只跑一次有效链**：完整 build → 相关测试 → 架构守卫，全部串行（模板见 §16.1）。仅改 changelog、版本号或准确描述时，不得无意义重跑完整构建。
- **并行纪律**：只读调查 / 跨文件只读检查 / 独立证据收集可并行；两个 `dotnet`、同一文件多写、Git staging/commit/push、改同一状态源的任务必须串行。同一文件禁止并行编辑。
- **文档最小同步**：只同步本轮真实版本号、changelog、file-tree 的职责变化、本轮新确认的长期治理规则；禁止顺手全面清理历史文档。
- **完成即落库**：代码 + 针对性测试 + 正式门禁 + 必要文档齐备后，立即 `git diff --check → commit → push → 远端 tip 复核 → 工作区 clean`，不得等用户催促。
- **汇报收敛**：仅在有真实阻断 / 实装完成 / 门禁结束 / 落库完成时汇报；禁止每条小命令后长篇预告。
- **停止条件**：代码与测试全绿、必要文档完成、commit+push 完成、远端 tip=本地 HEAD、工作区 clean，达到即停止修改。
- **验收前禁启后续**：自动测试通过 ≠ 功能 CLOSED；当前轮需真机验收的，未获用户明确真机通过不得关闭阶段、不得规划下一阶段。宪法案例：**WORLD-B-R4 真机未过前禁止启动 F5**。
- **速度与真实并重**：严格治理不可沦为无限审计或无限对账；不跳过真实门禁、不伪造结果、不扩围、不延迟已具备条件的落库、不以“继续核对”无限拖延。
- **落库即等待**：commit+push 且远端复核通过后，进入等待验收状态，不得主动开始下一阶段、不得在未验收前启动后续功能（如 F5）、不得顺手扩展/重构/补账历史文档。
