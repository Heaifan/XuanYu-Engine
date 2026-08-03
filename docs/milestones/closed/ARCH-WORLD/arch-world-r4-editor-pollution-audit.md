# ARCH-WORLD-R4-R0A Editor 污染归属只读审计

版本：`v0.2.19.4-rz`｜分支：`refactor/ARCH-WORLD-layer-boundary`｜前置：`ARCH-WORLD-R3 CLOSED`（`e50d890`）+ changelog 哈希纠偏（`b82a240`）

> 本轮性质：**只读审计**，不修改任何生产代码、不移动类型、不新建项目/模块。仅输出归属结论与最小迁移计划草案，待用户裁定后实装。

## 一、审计范围与四组焦点

R4 目标：识别并定位"编辑器职责"对基础 `Scene`/`World`/`Core` 层的污染，确认依赖方向，产出最小迁移计划。四组焦点：

1. `DefaultEditorCamera.Create(0)` 隐藏兜底；
2. `TransformSession` 归属；
3. `Framing` / `Selection` / `Preview` 编辑器职责污染；
4. 依赖方向精确表。

## 二、焦点 1：`DefaultEditorCamera.Create(0)` 隐藏后门

- **位置**：`XuanYu.Core/Scene/SceneRenderSnapshot.cs:34` `CameraState => Camera ?? DefaultEditorCamera.Create(0)`。
- **`DefaultEditorCamera`**：静态类，`XuanYu.Core/Space/DefaultEditorCamera.cs:5-19`，纯只读相机合同（Position/Target/Up + `Create(revision)` 派生 `CameraState`）。
- **谁提供 `Camera`**：`SceneRenderSnapshot.Camera` 为可选 `CameraState?`。生产唯一生产者是 `UiVm.RenderSnapshot`（`UiVm.Scene.cs:22-28`），其构造快照时**始终传入 `_camera`**（`UiVm.Camera.cs:9` 初始化为 `DefaultEditorCamera.Create(1)`）。
- **是否触发**：生产路径中无任何代码直接读取 `SceneStateOwner.RenderSnapshot`（基础 World 投影，`SceneWorldProjection.cs:19-22` 不带 Camera）的 `.CameraState`；`UiVm` 仅从中取 `.Entity`/`.Entities` 并以自身 `_camera` 重建。故后门在真实编辑器中**永不触发，是死代码**。
- **掩盖缺陷分析**：后门仅在"构造 `SceneRenderSnapshot` 而不传 Camera"时静默生成默认相机——生产不触达，但会掩盖"`Camera` 从未被提供"的潜在缺陷（若未来新增生产路径漏传 Camera，将静默得到默认而非快速失败）。
- **处置选项**：① 显式可选（返回 `Camera`，由消费者处理 null）；② 快速失败（为 null 时抛异常）；③ Editor 侧必传（移除 `??`，要求 `UiVm` 始终提供——其已实现）。
- **移除影响（暂不删除）**：无生产路径受影响；仅破坏未传 Camera 的测试/静态夹具（`SceneRenderSnapshot.Empty`、`TestEntityAtOrigin`）或读取其 `.CameraState` 的测试。移除需同步调整这些测试。
- **归属裁定**：相机合同本身归 Editor（D4，R4）；DTO 内静默后门属 D2（R5）边界整理细节。R4 建议：显式要求 Editor 必传 `Camera`，移除 `??` 兜底，但拆除动作与 `SceneRenderSnapshot` 迁 `Render.Abstractions` 协同进行（R5），不在 R4 单独立项强删。

## 三、焦点 2：`TransformSession` 归属

- **位置**：`XuanYu.World/Transform/TransformSession.cs:9`，`using XuanYu.Core.Gizmo;`（D1 反向依赖 World→Core.Gizmo）+ Core.Scene / Core.Transform / World.Scene。
- **全部生产消费者**：仅 `XuanYu.Editor.UI/Vm/UiVm.MoveGizmo.cs:10`（`readonly TransformSession _transformSession = new();`）。确认**生产只有 Editor.UI 使用**。
- **测试消费者**：`WorldPartitionR1Tests`、`TransformSessionTests`、`WorldSceneSelectionReentryTests`、`WorldR1FinalSceneTests`、`TransformHistoryRedoIntegrationTests`、`TransformHistoryIntegrationTests`（均位于 `World.Tests`/`Core.Tests`）。
- **是否令 UI 成为核心权威**：**否**。`TransformSession` 是瞬态 UI 交互会话（Begin/Preview/Commit/Cancel）；`TryCommit`（:42-54）最终调用 `scene.CommitPositionWithResult` → `SceneStateOwner` → `GlobalWorld.UpdateTransform`，写入权始终在 World。它只是"UI 侧会话态"，迁 Editor 不会反转权威方向。
- **合适迁移目标**：当前无独立 `XuanYu.Editor` 非 UI 项目；唯一自然落点是其唯一消费者所在项目 `XuanYu.Editor.UI`。**不得为迁移擅自新建项目/模块**（需用户批准）。
- **归属裁定**：迁 Editor 层（D1，R4），解除 `World→Core.Gizmo` 反向依赖；落点 = `XuanYu.Editor.UI`（或待批准的新 `XuanYu.Editor`），并随迁其测试。

## 四、焦点 3：`Framing` / `Selection` / `Preview`

- **`EditorCameraFraming`**（`Core/Space/EditorCameraFraming.cs:5`）：静态纯函数，根据实体位置/视口比例/FOV 计算 `CameraState`，无状态、无 World 回渗。唯一生产消费者 `UiVm.Camera`（`FrameAll`/`FrameSelected`）。属编辑器构图职责，应迁 Editor（D4，R4）；迁移风险低（纯函数）。
- **`Selection`**（`UiVm.Selection`）：Editor 只读投影，反映 `World` 激活实体游标，**无写回 World**。非污染，维持现状。
- **`Preview`**（`TransformSession.Preview`）：拖拽瞬态会话态，仅流入 `SceneRenderSnapshot.RenderPosition` 供显示，**从不写入 World/空间索引**。属设计内的只读投影，随 `TransformSession` 一并迁 Editor，非独立污染。

## 五、焦点 4：依赖方向精确表

| 类型 | 当前项目 | 真实生产消费者 | 写入权 | 应保留/迁移位置 | 迁移风险 |
|---|---|---|---|---|---|
| `DefaultEditorCamera` | `Core.Space` | Core(`SceneRenderSnapshot`,`EditorCameraFraming`)、Editor.UI(`UiVm.Camera`) | 无（只读合同） | 迁 Editor（D4） | 低；纯静态 |
| `EditorCameraFraming` | `Core.Space` | 仅 Editor.UI(`UiVm.Camera`) | 无（纯计算） | 迁 Editor（D4） | 低；纯函数 |
| `SceneRenderSnapshot.Camera` 后门 | `Core.Scene` | 生产者=Editor.UI(`UiVm`) | 无（投影 DTO） | 删 `??`，Editor 必传（D2→R5 协同） | 中；需修测试 |
| `TransformSession` | `World.Transform` | 仅 Editor.UI(`UiVm.MoveGizmo`) | 无（Commit 经 SceneStateOwner→World） | 迁 Editor.UI（D1） | 中；解 World→Core.Gizmo，需挪测试 |
| `Selection` | `Editor.UI` | 仅 Editor.UI | 无（只读投影） | 维持现状 | 无 |
| `Preview` | `World.Transform`(会话态) | 仅 Editor.UI(经 TransformSession) | 无（仅进 RenderSnapshot 显示） | 随 TransformSession 迁 Editor | 无 |

## 六、结论与最小迁移计划草案（R4 待实施，本轮不写代码）

- **R4-M1（D4）**：`DefaultEditorCamera` / `EditorCameraFraming` 从 `Core.Space` 迁 `Editor` 层；相机合同不再驻 Core。
- **R4-M2（D1）**：`TransformSession` 从 `World.Transform` 迁 `Editor.UI`（或待批准新 `XuanYu.Editor`），解除 `World→Core.Gizmo` 反向依赖；随迁其测试。
- **R4-M3（D2→R5 协同）**：`SceneRenderSnapshot` 迁 `Render.Abstractions` 时移除 `Camera ?? DefaultEditorCamera.Create(0)`，显式要求 Editor 必传 `Camera`（本审计建议 R4 起草契约、R5 实施）。
- **不纳入 R4**：接口语义拆分（R5）、G1（已 CLOSED）、空间索引（R2 CLOSED）、创建/删除实体 UI、VK-LIFE-1、P1 零位移 Undo。
- **本轮未改任何生产代码、未移动类型、未新建项目；仅产出归属结论与计划草案。**

## 七、R4-R1 实施结果（2026-07-25，v0.2.19.4-rz）

用户裁定：批准新增最小 `XuanYu.Editor` 程序集，R4 不再只是"移动两个文件"，而是建立长期稳定的编辑器领域边界。R4-R1 实装已完成：

- **新增 `XuanYu.Editor`**（`XuanYu.Editor.csproj`，net10.0，TargetFramework/Nullable/ImplicitUsings 沿用规范）：仅引用 `XuanYu.Core` 与 `XuanYu.World`；不引用 Avalonia / `XuanYu.Editor.UI` / `XuanYu.Render.Vulkan` / Silk.NET / 第三方包。加入 `XuanYu.Engine.slnx`；`XuanYu.Editor.UI` 新增对 Editor 的引用。
- **R4-R1A 迁移 `EditorCameraFraming`**：`Core/Space/EditorCameraFraming.cs` → `Editor/Camera/EditorCameraFraming.cs`（namespace `XuanYu.Editor.Camera`）；保留 `using XuanYu.Core.Space;`（消费 `DefaultEditorCamera`/`CameraState`，留 R5）+ `using XuanYu.Core.Math;`。行为完全不变（Frame All/Selected、空集合、大坐标、相机方向/Z-up）。
- **R4-R1B 迁移 `TransformSession`**：`World/Transform/TransformSession.cs` → `Editor/Transform/TransformSession.cs`（namespace `XuanYu.Editor.Transform`）；usings 不变（Core.Gizmo/Core.Math/Core.Scene/Core.Transform/World.Scene）。写入链 `UiVm → TransformSession → SceneStateOwner → GlobalWorld` 不变；不拥有实体永久位置，不自行改空间索引/Region。
- **测试策略**：未新建 `XuanYu.Editor.Tests`；现有回归测试暂留 `XuanYu.World.Tests` / `XuanYu.Core.Tests`，仅更新 `ProjectReference` + namespace（`using XuanYu.Editor.Camera` / `using XuanYu.Editor.Transform`）；迁移回归用例（Frame Selected/All、空集合、Begin→Preview→Commit、Escape Cancel、延迟 MouseUp、WM_CANCELMODE、跨 Region、Undo/Redo）继续运行。
- **架构守卫**：新增 `scripts/arch-a-guard-editor.ps1`（5+100 拆分），主脚本新增 Editor 入 `$projects` 并 dot-source；规则：Core/World ✕ Editor、Editor ✕ Editor.UI/Avalonia/Vulkan/Silk、Editor.csproj 仅 Core+World、Editor.UI 允许 Editor、Solution 含 Editor。
- **文档同步**：`arch-world-layer-attribution.md`（Editor 层 + 依赖禁区 + R4-R1 状态）、`玄域引擎_AI开发宪法.md`§26（Editor 边界红线）、`dev-rules.md`§2（Editor 依赖约束）、`changelog.md`（v0.2.19.4-rz + 补录 R4-R0A 条目）、`file-tree.md`、新增 `docs/arch-world-r4-editor-boundary.svg`。
- **R4 其余迁移**（Gizmo / History / ViewportPicking / DefaultEditorCamera / ScreenPoint）与 **R5**（DefaultEditorCamera.Create(0) 后门、SceneRenderSnapshot、ISceneRenderSnapshotSource 拆分）按原计划边界，不在 R4-R1 范围。
- **验证（提交前）**：`dotnet build` 10 项目 0W0E；`dotnet test` Core.Tests + World.Tests 共 168 passed / 0 failed；`arch-a-guard.ps1` EXIT=0；`git diff --check` 通过；SVG XML 通过；5+100 通过。状态：**R4-R1 代码完成、自动测试全绿、架构守卫通过；待用户真机验收（R4-R2 收口清单）后正式 CLOSED**。

## 八、R4 正式关闭（2026-07-25，v0.2.19.5-fix）

经 R4-R1 实装（`v0.2.19.4-rz`）+ FIX1（`v0.2.19.5-fix`）与用户真机验收，ARCH-WORLD R4 裁定 **CLOSED**：

- **边界建立完成**：`XuanYu.Editor` 程序集（仅引 Core+World，禁 Avalonia/Vulkan/Editor.UI/Silk）已落地；R4-M1 `EditorCameraFraming`（Core.Space→Editor.Camera）与 R4-M2 `TransformSession`（World.Transform→Editor.Transform）归位，依赖方向 `Core ← World ← Editor ← Editor.UI` 成立；架构守卫 `arch-a-guard-editor.ps1` 长期守护。
- **World 仍为唯一空间权威**：写入链 `UiVm → TransformSession → SceneStateOwner → GlobalWorld → Region/SpatialIndex/Snapshot` 不变；Editor 不持有实体永久位置，`Preview` 仅进 `RenderSnapshot` 显示、从不回写 World。
- **FIX1 恢复全仓 5+100**：`8e80098f` 纯 partial 物理拆分 4 个超限 `.cs`（`UiVm.Selection` 102→88 + `SelectionProjection` 61→75；`WorldPartitionR1Tests` 108→93 + `.Activity`；`WorldPartitionTests` 101→89 + `.PartitionStrategy`；`WorldSpatialQueryTests` 103→95 + `.Geometry`），行为零变化；真实版本源 `UiWin.axaml` / `run.bat` / `changelog` 三处一致 `v0.2.19.5-fix`。
- **自动门禁核验**：R4-R2 文档收口轮（commit `6635e989`）仅改三份文档、无生产代码变化，故未重新执行 `dotnet build`/`dotnet test`（沙箱禁跑且非必要）；本轮**当场复证**的静态门禁 = 三架构守卫 `arch-a-guard*.ps1` EXIT=0、SVG 47/47、全仓 5+100 = 0、`git diff --check` 通过、版本源三处一致（`run.bat`/`UiWin.axaml`/`changelog`=`v0.2.19.5-fix`）、远端引用=`6635e989`。`10 项目 0W0E` 与 `168 passed / 0 failed / 0 skipped` 为**继承证据**：来自前序提交 `8e80098f` 的自动验证（自该提交后无生产逻辑改动），并由用户本轮 `v0.2.19.5-fix` 真机六项回归（含完整构建 + Vulkan 释放链）作为外部补充证据佐证无生产回归。
- **真机验收通过**：`v0.2.19.4-rz` 完整 11 项（Frame/Undo/Picking/Resize/Vulkan 释放等）PASS；`v0.2.19.5-fix` 简化回归六项（实体命中/空白取消、Move Commit、跨 Region、Undo/Redo、Escape Cancel、Resize/Swapchain 恢复、Vulkan 关闭释放链）全 PASS，FIX1 未发现运行回归。
- **非阻断后续项（不塞入 R4）**：高频 `PublishSceneRenderSnapshot` / 命令缓冲重录日志风暴——功能无失败，属诊断/性能噪声（P1 非阻断）；独立登记为「日志限流 / Snapshot 发布合并」待办，留后续轮处理，不在 R4 收口轮修复。
- **下一阶段**：进入 **R5-R0A 只读审计**。核心问题不是"是否把 `SceneRenderSnapshot` 照搬进 `Render.Abstractions`"（渲染器使用某对象≠该对象归渲染层所有），而是分离三类概念：① **世界事实快照**（实体是谁/在哪/状态，属 World/场景权威层）；② **编辑器组合状态**（选中/Gizmo/Editor 相机/辅助显示开关，属 Editor）；③ **渲染输入投影**（Render 真正消费的不可变、只读、帧级数据合同，才可能属 `Render.Abstractions`）。真实审计问题：是否从 `SceneRenderSnapshot` 提取一个最小 Render Projection，而非整体搬入 Render.Abstractions。交付五部分：① 类型归属矩阵（当前项目/命名空间/生产者/消费者/数据权威/生命周期/建议归属/是否迁移）；② `SceneRenderSnapshot` 字段逐项分类（World 事实/Editor 状态/Render 投影/临时兼容/应删或拆）；③ 双 `ISceneRenderSnapshotSource` 真实关系（同名接口 / 单接口双实现 / 基础生产者+组合装饰器 / 双并列权威源——若都能独立生成完整快照则存在第二事实源）；④ 三套迁移选项（A 保持现状仅修正命名 / B 拆出最小 Render Projection / C 整体迁移 Snapshot，默认不直选 C）；⑤ 明确停止线（R5-R0A 只读阶段不得：移动 DTO、修改项目引用、拆分接口、改 Vulkan、顺手处理日志风暴、因"不顺眼"批量整理）。

> **ARCH-WORLD R4 = CLOSED**（2026-07-25，v0.2.19.5-fix）。
