# ARCH-WORLD R5-R0A：Render 合同边界只读审计

> 分支：`refactor/ARCH-WORLD-R5-render-contract`（本审计轮新建，基线 `6ccfb660e4a5c958a7b33dd36398b1393bf775c8`）
> 版本：`v0.2.19.6-rz` ｜ 审计类型：**只读** ｜ 日期：2026-07-25
> 目标：在动手前用一轮只读审计，把 `SceneRenderSnapshot` / `ISceneRenderSnapshotSource` 与 Render 的真实边界查清楚，确定后续方案，不再围绕旧分支状态打转。

---

## 一、审计基线与范围

**基线**：远端 `origin/refactor/ARCH-WORLD-layer-boundary` tip = `6ccfb660e4a5c958a7b33dd36398b1393bf775c8`（R4 CLOSED 收口 commit）。新分支 `refactor/ARCH-WORLD-R5-render-contract` 直接由该提交创建，旧分支保留不删、不重写、不强推。

**核心问题（不是预设"把 Snapshot 迁入 Render.Abstractions"）**：

1. 世界事实由谁拥有？（World/Scene 权威层）
2. 编辑器附加了哪些状态？（Editor/UI 组合层）
3. Render 实际只需要哪些不可变字段？（Render 输入投影）
4. 是否需要提取最小 Render Projection？（由证据决定，不提前宣布）

**范围**：全仓静态核查（定义/创建/修改/消费/权威/生命周期/依赖/语义）+ 字段逐项分类 + 双 Source 定性 + `DefaultEditorCamera` 可达性 + Render 消费字段实测。**本轮零生产代码改动**，仅新增两份审计文档 + 版本/元信息 bump。

**核查对象**（证据文件，均在 `6ccfb66` 工作树）：
- `XuanYu.Core/Scene/SceneRenderSnapshot.cs`、`ISceneRenderSnapshotSource.cs`、`SceneEntitySnapshot.cs`
- `XuanYu.Core/Space/DefaultEditorCamera.cs`、`CameraState.cs`、`ViewProjectionState.cs`、`ViewportState.cs`
- `XuanYu.World/Scene/SceneStateOwner.cs`、`SceneStateOwner.Lifecycle.cs`、`SceneWorldProjection.cs`
- `XuanYu.Editor.UI/Vm/UiVm.Scene.cs`、`UiVm.Camera.cs`
- `XuanYu.Editor/Camera/EditorCameraFraming.cs`
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.Draw.cs`、`.Scene.cs`、`VulkanNativeHostSurfaceBridge.cs`、`.Scene.cs`
- `XuanYu.Render.Abstractions/*`、`*.csproj`

---

## 二、类型归属矩阵

| 类型 | 定义位置 | 项目 | 创建者 | 主要修改者 | 消费者 | 数据权威 | 生命周期 | 当前依赖 | 含 World/Editor/Render 语义 | 建议归属 |
|---|---|---|---|---|---|---|---|---|---|---|
| `SceneRenderSnapshot` | `Core/Scene` | Core | `SceneWorldProjection`(base) / `UiVm`(composed) | Core+Editor.UI | `VulkanClearFrameOwner`(Render)、`UiVm`(自读)、测试 | 无（派生 DTO） | 每帧重算 | Core；被 Render.Vulkan、Editor.UI 引用 | **含 Editor**（IsSelected/Preview/ShowMoveGizmo） | 保留为"组合快照"；Render 改消费最小投影 |
| `SceneEntitySnapshot` | `Core/Scene` | Core | `SceneWorldProjection.ToSceneEntity` | Core | `SceneRenderSnapshot`、`VulkanClearFrameOwner` | World（EntityId/Name/Type/Transform） | 随快照 | Core | World 事实 | 保留 Core/Scene（纯 DTO） |
| `ISceneRenderSnapshotSource` | `Core/Scene` | Core | — | Core | `VulkanNativeHostSurfaceBridge`、测试 | 接口契约 | 长驻 | Core；被 Render.Abstractions 引用（工厂参数） | 语义过宽（同返回 Snapshot） | R5 拆分为 base/composed 两接口 或 改用最小投影 |
| `SceneStateOwner` | `World/Scene` | World | `new()` 自举实体 | World | `UiVm`(读 `.RenderSnapshot`) | World（GlobalWorld） | 长驻 | World→Core | 基础 World/Scene 投影 | 保留为"基础投影生产者" |
| `SceneWorldProjection` | `World/Scene` | World | — | World | `SceneStateOwner` | World | 静态纯函数 | World→Core | World→Snapshot 投影 | 保留 |
| `UiVm` | `Editor.UI/Vm` | Editor.UI | `new()` | Editor.UI | `VulkanNativeHostSurfaceBridge`(注入)、`UiVm` 自读 | **组合**（读 SceneStateOwner + Editor 状态） | 长驻 | Editor.UI→Core/World/Editor/Render.Abstractions | Editor 组合源（装饰 base） | 保留为"活动组合源" |
| `EditorCameraFraming` | `Editor/Camera` | Editor | — | Editor | `UiVm.Camera` | Editor（构图规则） | 静态纯函数 | Editor→Core | Editor 相机 | 保留 Editor |
| `DefaultEditorCamera` | `Core/Space` | Core | — | Core | `SceneRenderSnapshot.CameraState` 后门、`EditorCameraFraming` | Core（硬编码默认） | 静态 | Core | **后门默认值** | 移除 `Create(0)` 兜底 |
| `CameraState` | `Core/Space` | Core | `DefaultEditorCamera`/`EditorCameraFraming` | Core | `SceneRenderSnapshot`、`ViewProjectionState`、`VulkanClearFrameOwner` | 中性值类型 | 不可变值 | Core | 中性（消费方决定语义） | 保留 Core/Space；进最小投影 |
| `ViewProjectionState` | `Core/Space` | Core | `Create(Camera,Viewport)` | Core | `VulkanClearFrameOwner.Draw` | 派生（Camera+Viewport） | 瞬时 | Core | 派生数学 | 保留 Core/Space |
| `ViewportState` | `Core/Space` | Core | `VulkanClearFrameOwner.Draw`(由 swapchain 尺寸) | Core | `ViewProjectionState` | Render 宿主尺寸 | 瞬时 | Core | Render 输入 | 保留 Core/Space |
| `VulkanClearFrameOwner` | `Render.Vulkan/Render` | Render.Vulkan | — | Render.Vulkan | 自身 Draw | 无（消费快照） | 帧级 | Render.Vulkan→Core(+Core.Scene) | Render 消费者 | 改消费最小投影 |
| `VulkanNativeHostSurfaceBridge` | `Render.Vulkan` | Render.Vulkan | `new(...,sceneSource)` | Render.Vulkan | 宿主 | 无 | 长驻 | Render.Vulkan→Core(+Core.Scene) | Render 桥接 | 改依赖最小投影接口 |
| `Render.Abstractions` 契约 | `Render.Abstractions` | Render.Abstractions | — | — | `VulkanNativeHostSurfaceBridgeFactory` 等 | 无 | 长驻 | **Render.Abstractions→Core** | 当前耦合 `ISceneRenderSnapshotSource` | 改定义并自持最小投影类型 |

**依赖方向实测（csproj）**：`Core`→∅；`World`→Core；`Editor`→Core+World；`Render.Abstractions`→Core；`Render.Vulkan`→Core+Render.Abstractions；`Editor.UI`→Core+World+Editor+Render.Abstractions。冻结分层成立，**但 `Render.Abstractions → Core.Scene` 是当前真实耦合点**（工厂参数 `ISceneRenderSnapshotSource? sceneSource`），即 Render 直接依赖了带 Editor 语义的 Snapshot 类型——R5 的核心治理对象。

---

## 三、SceneRenderSnapshot 字段逐项分类

类型 `readonly record struct SceneRenderSnapshot`（`Core/Scene/SceneRenderSnapshot.cs:8`）：

| 字段 | 类型 | 生产者（composed 路径 / base 路径） | Render 是否消费 | 分类 | 说明 |
|---|---|---|---|---|---|
| `Entity` | `SceneEntitySnapshot` | UiVm 取 `scene.Entity`（=SceneStateOwner 基础投影的 Entity） | 间接（经由 `Entities`） | **A. World 事实** | 实体身份+Name+Type+Transform，权威来自 World |
| `IsSelected` | `bool` | UiVm：`HasSelection && SelectionKey==entity.Key`（默认 `false`） | **否**（全仓 `.cs` 无消费，仅历史 changelog:369） | **B. Editor 组合状态** | 纯编辑器选中高亮；Render 不读 → 不应进入 Render 投影 |
| `PreviewTransform` | `PreviewTransform?` | UiVm：`_transformSession.Preview`（拖拽预览） | 间接（`RenderPosition`/`PositionFor` 读取其 Position） | **B. Editor 组合状态** | 编辑器拖拽预览；Render 仅消费其位置覆盖 |
| `ShowMoveGizmo` | `bool` | UiVm：`EditorTransformCapturePolicy.ShouldShowMoveGizmo(tool,selected)` | **是**（`VulkanClearFrameOwner.Draw.cs:31` `if(_sceneSnapshot.ShowMoveGizmo) DrawActiveGizmo`） | **B. Editor 组合状态（被 Render 读取）** | 由工具+选中派生；Render 据其决定画不画 Gizmo |
| `RenderEntities` | `IReadOnlyList<SceneEntitySnapshot>?` | UiVm 传 `scene.Entities`（=SceneStateOwner 全实体投影） | **是**（`Draw.cs:24` 遍历 `_sceneSnapshot.Entities`） | **A. World 事实** | 全实体快照列表，权威来自 World |
| `Camera` | `CameraState?` | UiVm 传 `_camera`（Editor 相机，初值 `DefaultEditorCamera.Create(1)`，由 `EditorCameraFraming` 更新） | **是**（经 `CameraState` 属性→`Draw.cs:52`） | **C. Render 输入投影** | 编辑器相机参数，喂给 View-Projection；base 路径为 `null` → 触发后门 |

**派生属性**：
- `CameraState => Camera ?? DefaultEditorCamera.Create(0)`（`SceneRenderSnapshot.cs:34`）：**D. 临时兼容/兜底**——当 `Camera==null` 静默返回默认编辑器相机，掩盖"缺相机"的生产缺失（见第五节）。
- `RenderPosition => PreviewTransform?.Position ?? Entity.Transform.Position`（`SceneRenderSnapshot.cs:36`）：World 事实 + Editor 预览位置覆盖。
- `Entities => RenderEntities ?? (HasEntity ? [Entity] : [])`（`SceneRenderSnapshot.cs:31`）：派生列表。

**分类结论**：6 个主字段中，**A 类（World 事实）2 个**（Entity、RenderEntities），**B 类（Editor 状态）3 个**（IsSelected、PreviewTransform、ShowMoveGizmo，其中 ShowMoveGizmo 被 Render 读），**C 类（Render 输入）1 个**（Camera）。快照本质是一个"World 事实 + Editor 状态 + Render 相机"的**组合 DTO**，而非纯 Render 合同——这正是它不该整体归属 `Render.Abstractions` 的证据。

---

## 四、双 Snapshot Source 真实关系

`ISceneRenderSnapshotSource` 共有 **两个实现**（全仓搜索确认，无第三实现）：

1. **`SceneStateOwner`**（`World/Scene/SceneStateOwner.cs:9` 实现接口）
   - `RefreshSnapshot()` → `SceneWorldProjection.ToRenderSnapshot(active, Entities)`（`SceneStateOwner.cs:74`）。
   - `ToRenderSnapshot` 产：`new SceneRenderSnapshot(ToSceneEntity(entity), isSelected:false, RenderEntities: list)`（`SceneWorldProjection.cs:19`）——**只填 Entity + RenderEntities，IsSelected=false、PreviewTransform=null、ShowMoveGizmo 默认 false、Camera=null**。即"基础 World/Scene 投影"。
2. **`UiVm`**（`Editor.UI/Vm/UiVm.cs:10` 实现接口）
   - `RenderSnapshot` getter（`UiVm.Scene.cs:13`）：`var scene = _sceneState.RenderSnapshot;` 取基础投影 → `selected = HasSelection && ...` → `showMove = ShouldShowMoveGizmo(...)` → `new SceneRenderSnapshot(entity, selected, _transformSession.Preview, showMove, scene.Entities, _camera)`。即"读 base + 装饰 Editor 状态 + 注入 Editor 相机"。

**关系判定 = 选项 2：基础快照生产者 与 组合装饰器**（非选项 4 并列权威源）。
- `UiVm` 显式读取 `SceneStateOwner.RenderSnapshot` 作为上游（`UiVm.Scene.cs:17`），再装饰——是**装饰器**而非独立权威。
- `SceneStateOwner` 数据是 World 真相的只读投影，不写回、不与 `UiVm` 并列生成"完整权威快照"。
- 与 R3 裁定一致："UiVm=唯一活动组合源，SceneStateOwner=基础投影，非第二真相"。**不存在第二事实源，无 R0A 架构风险登记项**。

**但遗留问题（R5 要解）**：两个实现返回**同一类型** `SceneRenderSnapshot`，接口语义过宽（R3 已转交 R5）。Render 拿到任一 `ISceneRenderSnapshotSource` 无法区分"基础"还是"组合"，只能全盘接收带 Editor 字段的快照。R5 应让 Render 只依赖**最小投影**（选项 B），从根上消解接口过宽。

---

## 五、DefaultEditorCamera 可达性与风险

`DefaultEditorCamera.Create(0)` 仅在 `SceneRenderSnapshot.Camera == null` 时由 `CameraState` 属性触发（`SceneRenderSnapshot.cs:34`）。

**可达性**：
- 活动路径（`UiVm` 组合快照）：`_camera` 初值即 `DefaultEditorCamera.Create(1)`（`UiVm.Camera.cs:10`），并由 `EditorCameraFraming.FrameAll/FrameSelected` 持续更新 → `Camera` 恒非 null → **后门在活动路径不触发**（与 R4 审计"生产为死代码、UiVm 恒传 Camera"一致）。
- 可达路径：① `SceneStateOwner` 基础投影 `Camera=null`；② 任何 camera-less 快照被传给 Render。一旦 Render 接收 camera-less 快照，`CameraState` 静默返回 `(4,-5,3)` 默认相机。

**风险定性（高）**：
- 它不是"合法默认值"，而是**掩盖快照生产缺失的兜底**——若某生产者漏设相机，渲染不会报错，而是用固定默认相机，bug 被隐藏。
- `DefaultEditorCamera`（`Core/Space/DefaultEditorCamera.cs`）本身是 Core 层硬编码常量（position/target/up/fov/near/far），属"渲染器不该依赖的编辑器概念残留"。
- **R5 处置**：移除 `?? DefaultEditorCamera.Create(0)`；最小 Render Projection 必须携带**显式 `CameraState`**，缺失即构建/运行期明确失败，而非静默默认。

---

## 六、Render 实际消费字段（实测）

`VulkanClearFrameOwner.Draw.cs`（`RecordDraw`）实测消费：

| 消费点 | 来源字段 | 语义 | 是否 Editor 语义 |
|---|---|---|---|
| `var entities = _sceneSnapshot.Entities` (L24) | `RenderEntities`(A) | 全实体列表 | World |
| `_sceneSnapshot.PositionFor(entities[i])` (L27) | `Entity.Transform` / `PreviewTransform`(A+B) | 实体渲染位置（含预览覆盖） | World + Editor 预览 |
| `if (_sceneSnapshot.ShowMoveGizmo) DrawActiveGizmo` (L31) | `ShowMoveGizmo`(B) | 是否画 Gizmo | Editor |
| `_sceneSnapshot.RenderPosition` (L37, gizmo) | `PreviewTransform?`/`Entity.Transform`(A+B) | Gizmo 位置 | World + Editor 预览 |
| `var source = _sceneSnapshot.CameraState` (L52) | `Camera`(C) → 后门 | 相机参数→View-Projection | Editor 相机 |
| `_extent` (swapchain) | 视口尺寸（**非快照字段**） | Viewport | Render 宿主 |

**Render 不消费**：`IsSelected`（全仓 `.cs` 零消费，仅历史）；`Entity` 单数（Render 用 `Entities` 列表）。

**Render 真实最小输入集合** = `{ 实体渲染位置(+预览) , Gizmo 可见标志 , 相机参数 , 视口尺寸 }`。
→ 这正是"最小 Render Projection"的种子：World 事实（位置）+ Editor 状态（Gizmo 可见）+ Render 输入（相机/视口），三者解耦后各自来自权威层。

---

## 七、A / B / C 三套方案比较

### A. 保持现状（仅修正文档/命名）
- 适用条件：Snapshot 已最小、稳定、无 Editor 污染、Render 输入即其全部。
- 现状不符：含 `IsSelected`（Render 不消费）、`ShowMoveGizmo`/`PreviewTransform`（Editor 状态）、`Camera ?? Create(0)` 后门；Render 仅消费子集。
- **结论：不适用。**

### B. 提取最小 Render Projection（推荐）
- 保留：World 权威事实（Entity/Transform，在 World/Scene）+ Editor 组合状态（选中/预览/Gizmo，在 Editor/UI）。
- 由 Editor 或专门适配层，从组合快照中**只抽取 Render 真正消费的不可变字段**，组合成最小投影，供 Render 消费。
- Render 改依赖 `Render.Abstractions` 自持的投影类型，**不再依赖 `Core.Scene` 的 `SceneRenderSnapshot`**。
- 与证据一致：Render 实测只吃位置/Gizmo/相机/视口。
- **结论：由代码证据支持，推荐。**

### C. 整体迁移 Snapshot 到 Render.Abstractions
- 适用条件：所有字段都是稳定渲染合同，无 World/Editor 权威语义。
- 现状不符：含 World 权威（Entity/Transform）、Editor 状态（IsSelected/Preview/ShowMoveGizmo）。
- **结论：默认不选择。**

---

## 八、推荐方案与证据

**推荐方案 B：提取最小 Render Projection。**

证据链（均来自 `6ccfb66` 工作树实测）：
1. `SceneRenderSnapshot` 是组合 DTO，6 字段跨 A/B/C 三类（第三节），非纯 Render 合同 → 否决 C。
2. Render 实测仅消费 `Entities`/`PositionFor`/`ShowMoveGizmo`/`CameraState`（第六节），`IsSelected` 零消费 → Snapshot 对 Render 而言"过重且含杂质" → A 不适用。
3. 双 Source 为 base+组合装饰器（第四节），无第二事实源；接口过宽是 R5 要解的耦合，B 从 Render 侧切断对 `Core.Scene` 的依赖，根解法。
4. `Camera ?? DefaultEditorCamera.Create(0)` 是掩盖缺相机的兜底（第五节），B 的投影要求显式相机，自然消除后门。
5. 依赖实测 `Render.Abstractions → Core.Scene`（`INativeHostSurfaceBridgeFactory.Create(ISceneRenderSnapshotSource?)`）是当前唯一让 Render 触碰 Editor 语义类型的耦合点；B 让 `Render.Abstractions` 自持投影类型，恢复"Render 不依赖 World/Editor/Core.Scene 语义"的洁净。

---

## 九、R5-R1 最小实装边界（供下一轮，本轮不实施）

**新增类型（位于 `XuanYu.Render.Abstractions`，自持，不反向依赖 Core.Scene）**：
- `RenderSceneProjection`（immutable `readonly record struct`）：
  - `CameraState Camera`（**必填**，无后门）；
  - `IReadOnlyList<RenderEntityPlacement> Entities`，其中 `RenderEntityPlacement { EntityId Key; Vector3d RenderPosition; }`（World 位置 + 预览覆盖已在此合并，Render 不再见 PreviewTransform）；
  - `bool GizmoVisible`（由 `ShowMoveGizmo` 映射）；
  - 视口尺寸**不进投影**（由 Render 宿主 swapchain 提供，保持帧级注入）。

**适配器（位于 Editor/UI，组合层职责）**：
- 新增 `ISceneRenderProjectionSource` 或 `SceneRenderProjectionAdapter`：输入 `SceneRenderSnapshot`（组合快照），输出 `RenderSceneProjection`，**只抽取** Render 消费字段；`IsSelected` 等纯 UI 状态不进入投影。

**Render 侧改造（Render.Vulkan）**：
- `VulkanNativeHostSurfaceBridge` / `VulkanClearFrameOwner` 改收 `RenderSceneProjection`（或 `ISceneRenderProjectionSource`），删除对 `SceneRenderSnapshot`/`ISceneRenderSnapshotSource` 的引用；`Draw.cs` 直接读投影字段。
- `Render.Abstractions` 工厂参数由 `ISceneRenderSnapshotSource?` 改为 `ISceneRenderProjectionSource?`，解除对 `Core.Scene` 的引用。

**红线（R5-R1 仍守）**：
- 不搬动 World 权威（Entity/Transform 仍在 World/Scene）；
- 不重做 Editor 状态语义（IsSelected/Preview/ShowMoveGizmo 仍由 Editor/UI 维护，仅不进投影）；
- 不改 Vulkan 视觉行为，仅换输入类型；
- 测试数量不减（≥168），5+100 不新增违例。

---

## 十、停止线与非目标

**R0A 停止线（本轮禁止）**：移动 DTO；修改项目引用；拆分接口；新增公共抽象（仅审计，R5-R1 才加）；修改 Vulkan；修改 Snapshot 生产逻辑；处理日志风暴；顺手整理 Core 目录；因"不顺眼"批量改命名。

**R0A 允许**：搜索/读代码；执行 build/test/架构守卫（仅版本/文档轮，生产 .cs 零改动，结果继承 R4 CLOSED 基线）；更新版本字符串；新增审计文档与 SVG；更新 changelog/file-tree；commit 并 push。

**非目标**：本审计不裁定 R5-R1 的具体代码落点细节（仅给边界）；不重新讨论 R3/R4 已 CLOSED 的结论；不扩大 Editor 边界（仍守 R4 用户裁定：两类型、不引入实体 UI/Snapshot 重构/DefaultEditorCamera 重做/Undo 重构/Gizmo 重做）。

**后续节奏**：R5-R0A（本轮，只读定方案）→ R5-R1（最小合同实装）→ R5-R2（迁生产者/消费者）→ R5-R3（清旧合同与兜底）→ R5-R4（真机回归/守卫/收口）。证据支持 B，R5-R1 直接进入"最小 Render Projection"实装，不再额外开计划轮。
