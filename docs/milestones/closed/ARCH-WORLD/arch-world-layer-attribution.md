# ARCH-WORLD 物理分层归属审计（修正版）

> 版本：v0.2.19.1-rz ｜ 日期：2026-07-23 ｜ 阶段：ARCH-WORLD-R0 边界冻结（本文档轮）
> 来源：前序 Core 归属审计 + 项目负责人五点修正裁定。本文档为修正后落库版；原审计与本文冲突之处，以本文为准。

---

## 一、总结论

逻辑分层已经自然长出来，但物理程序集边界没有及时跟上。当前两大问题：

1. World / Editor 概念挤在 `XuanYu.Core`；
2. 两套 SpatialIndex 形成双轨空间真相（`SceneStateOwner._spatialIndex` 与 `WorldQuery._index`）。

治理窗口是现在：在继续 WORLD-A 功能扩张之前，先做一次“小步物理分层”。不做大爆炸式重构，按 ARCH-WORLD-R0 → R5 分轮推进，每轮独立验收。

执行顺序裁定：

```text
修正版归属审计落库（本轮）
→ ARCH-WORLD-R0 边界冻结（本轮，纯文档）
→ R1 建立 XuanYu.World 程序集
→ R2 收敛唯一 SpatialIndex
→ R3 Scene Truth 归位
→ R4 Editor 污染剥离
→ R5 Snapshot 边界整理
→ 恢复 WORLD-A 功能开发
```

---

## 二、五项架构裁定

### 裁定 1：`EntityId` 不加 Generation —— 稳定身份与临时句柄分离

实体身份长期稳定：同一实体跨 Region、休眠、Streaming 卸载、重新激活，`EntityId` 不变。三个概念分开：

- `EntityId` = 稳定身份，永不因存储槽变化而改变。当前实现 `XuanYu.Core/Identity/EntityId.cs`（int Value，单调递增）符合该裁定。
- `EntityHandle` = 未来需要时的运行时快速句柄（Index + Generation），用于存储槽复用后防 stale reference；属于临时句柄，不属于稳定身份。
- `EntityRevision` = 未来独立的状态版本修订号，不与 `EntityId` 混合。

现阶段只冻结三概念边界，不实现 Handle / Revision。

### 裁定 2：术语统一为 `EntityId`

正式代码与文档术语统一 `EntityId`，含义“世界范围内用于标识实体的稳定身份”。不再使用 `EntityKey` 第二套叫法（当前无证据表明二者表达不同事物）。存量变量命名（如 `SceneStateOwner.Lifecycle.cs` 中 `entityKey` 局部变量）在后续触碰该文件时顺手收敛，不单独扩面。

### 裁定 3：Viewport Picking 归 Editor，三层拆分

- `XuanYu.Core`：`Ray3d` / `Aabb3d` / 几何求交数学 / Vector / Matrix 基元。
- `XuanYu.World`：`SpatialIndex` / `WorldQuery`（Ray Query / Bounds Query / Nearest Query）。
- `XuanYu.Editor`：`ViewportPickingService`（ScreenPoint + Camera/ViewProjection → 生成 WorldRay → 调 WorldQuery → EntityId → Editor Selection）。

理由：Viewport Picking 知道 Viewport / ScreenPoint / 鼠标选择，已不是 Core 机制；游戏本身可复用 `WorldQuery.Ray(...)` 而完全不认识鼠标与编辑器。

### 裁定 4：空间索引归 World，纯几何数学留 Core

- 留 Core：`Vector3d` / `Ray3d` / `Aabb3d` / 几何求交 / 纯数学算法。
- 进 World：`ISpatialIndex` / `DynamicAabbTree` / `SpatialIndexOwner` / `WorldQuery` / `Region` / `Partition`。

即 `XuanYu.World.Spatial` 使用 Core 的 `Aabb3d` / `Ray3d` 实现 `DynamicAabbTree`；Core 不得拥有“实体的动态 AABB 树”。

### 裁定 5：`SceneStateOwner` 归 World/Scene；Snapshot 是边界 DTO；`DefaultEditorCamera` 归 Editor

- `SceneStateOwner` 持有 Scene Entity / Committed Transform / Scene World State，归 `XuanYu.World.Scene`。Editor 可以修改它，不等于它属于 Editor（文档不因 Word 能编辑它就属于 UI）。
- `SceneRenderSnapshot` / `ISceneRenderSnapshotSource` 是派生表现数据的边界 DTO，不是 World Truth；现阶段归 `XuanYu.Render.Abstractions`，不为美观新增 Presentation 项目。
- `DefaultEditorCamera` 归 Editor。当前越界证据：`XuanYu.Core/Scene/SceneRenderSnapshot.cs` 引用 `DefaultEditorCamera.Create()`——“没有相机就偷偷创建一台编辑器默认相机”的隐藏后门。目标形态：Editor/View 提供 `CameraState`，Snapshot 只接受 `CameraState`。

---

## 三、归属裁定总表（现状 → 目标）

| 现状（真实路径） | 目标层 | 迁移轮 |
|---|---|---|
| `Core/Identity/EntityId.cs` | 留 Core | — |
| `Core/Math/Vector3d.cs`、`YawRotation.cs` | 留 Core | — |
| `Core/Time/SimulationTime.cs`、`TimeStep.cs` | 留 Core | — |
| `Core/Results/EngineResult.cs`、`EngineError.cs` | 留 Core | — |
| `Core/Logging/*`、`Core/Diagnostics/CoreSelfTest.cs` | 留 Core（基础诊断契约，量须克制） | — |
| `Core/World/GlobalWorld*.cs`、`EntityRegistry.cs`、`RegionKey.cs`、`WorldPartition*.cs`、`WorldEntitySnapshot.cs`、`WorldEntityActivity.cs`、`WorldQuery.cs`、`GridWorldPartitionStrategy.cs`、`IWorldPartitionStrategy.cs` | `XuanYu.World` | R1 |
| `Core/Spatial/*`（`ISpatialIndex`、`DynamicAabbTree*`、`SpatialIndexOwner`、`SpatialQuery*`、`SpatialRaycast*`、`SpatialBounds`、`SpatialAabb`） | `XuanYu.World.Spatial`；其中纯几何求交数学（`RayAabbIntersection` 等）可留 `Core.Geometry`，粒度在 R1 判定 | R1 |
| `Core/Scene/SceneStateOwner*.cs`、`CommittedTransform.cs`、`SceneEntitySnapshot.cs`、`SceneWorldProjection.cs`、`SceneSpatialBoundsProjection.cs`、`SceneTransformCommitResult.cs` | `XuanYu.World.Scene`（行为完全不变） | R3 |
| `Core/Scene/SceneRenderSnapshot.cs`、`ISceneRenderSnapshotSource.cs` | `XuanYu.Render.Abstractions`（边界 DTO） | R5 |
| `Core/Space/CameraState.cs`、`ViewportState.cs`、`ViewProjectionState.cs`、`WorldRay.cs`、`WorldRayFactory.cs` | 跟随 Snapshot 边界定稿 | R5 |
| `Core/Space/DefaultEditorCamera.cs`、`EditorCameraFraming.cs` | `XuanYu.Editor` | R4 |
| `Core/Picking/ViewportPickingService.cs`、`ViewportPickingRequest.cs`、`ViewportPickingResult.cs` | `XuanYu.Editor` | R4 |
| `Core/Gizmo/MoveGizmo*.cs`、`ScreenPoint.cs` | `XuanYu.Editor` | R4 |
| `Core/Transform/TransformSession.cs`、`PreviewTransform.cs`、`TransformStartSnapshot.cs` | `XuanYu.Editor` | R4 |
| `Core/History/EditorHistoryOwner.cs`、`TransformHistoryEntry.cs` | `XuanYu.Editor` | R4 |

---

## 四、双轨空间索引：本次审计最严重问题

现状（真实代码证据）：

- `XuanYu.Core/Scene/SceneStateOwner.cs:11`：`readonly SpatialIndexOwner _spatialIndex = new();`——Scene / Picking 派生索引。
- `XuanYu.Core/World/WorldQuery.cs:9`：`SpatialIndexOwner _index = new();`——World 正式派生索引。

同一个世界存在两套“谁在哪里”的答案，直接违反唯一权威事实原则。可能导致：Picking 命中旧位置而 Render 显示新位置；或 Scene 有实体而 WorldQuery 无实体。

裁定（收敛方向）：

```text
GlobalWorld（World Truth）
    → 唯一 SpatialIndexOwner
        → WorldQuery
            → Picking / Query / Streaming 共用
```

`SceneStateOwner` 不再拥有第二套空间索引。该收敛即 ARCH-WORLD-R2，必须在 WORLD-A 功能继续扩张前完成；与 changelog `v0.2.18.19-rz` 遗留项“R3-R2 优先让 Picking 接 WorldQuery”一致，R2 将其正式化。收敛前旧索引列为受控架构债务，禁止新增消费者。

---

## 五、治理序列 ARCH-WORLD-R0 → R5

| 轮次 | 内容 | 边界 |
|---|---|---|
| R0（本轮） | 边界冻结：本文档 + 宪法第二十六条 + dev-rules 第 15 节 + file-tree / changelog 同步 | 只落文档，不改任何代码归属 |
| R1 | 建立 `XuanYu.World` 程序集；迁 GlobalWorld / EntityRegistry / RegionKey / WorldPartition* / WorldEntitySnapshot / WorldEntityActivity / WorldQuery / GridWorldPartitionStrategy 与 Spatial Index 实现 | 不碰 Editor |
| R2 | 收敛唯一空间真相：唯一权威 SpatialIndex；Picking 接 WorldQuery；Scene 旧索引退场 | 触碰 Picking 主链，需真机验收 |
| R3 | Scene Truth 归位：迁 CommittedTransform / SceneEntitySnapshot / SceneWorldProjection / SceneStateOwner | 行为完全不变 |
| R4（进行中） | Editor 污染剥离：R4-R1 已建 `XuanYu.Editor` 并迁入 `EditorCameraFraming` + `TransformSession`；其余 Gizmo / History / ViewportPicking / DefaultEditorCamera / ScreenPoint 留待后续 R4 子轮 | — |
| R5 | Snapshot 边界整理：SceneRenderSnapshot / ISceneRenderSnapshotSource / CameraState / ViewProjectionState；保证 World 不依赖 Editor、Render 不依赖 World 实现、Editor 不依赖 Vulkan 实现 | — |

每轮独立：修改 → 验证（build 0W0E + 全量测试 + arch-a-guard + 涉及面真机）→ commit → push。禁止一轮全搬，禁止跨轮夹带。

---

## 六、依赖方向冻结

允许：

```text
兵无常势（游戏） → XuanYu.WarCore（未来）
XuanYu.WarCore   → XuanYu.World / XuanYu.Core
XuanYu.World     → XuanYu.Core
XuanYu.Editor    → XuanYu.World / XuanYu.Core（本轮仅引用 Core+World；Render.Abstractions 待 R5 Snapshot 边界再评估）
XuanYu.Render.Vulkan → XuanYu.Render.Abstractions
```

禁止：

```text
Core    → World / WarCore / Editor / Vulkan
World   → WarCore / Editor / Vulkan
WarCore → Editor / Vulkan
Editor  → Editor.UI / Avalonia / Vulkan / Silk.NET
```

附注：若 R5 采用"Snapshot 契约归 Render.Abstractions"，允许 `World → Render.Abstractions` 单向引用边界 DTO；`Render.Abstractions` 任何情况下不得引用 World 实现（红线 2 已禁 Silk.NET / Avalonia / UI，R5 补齐 World 方向）。

## 六之二、R4-R1 实施状态（2026-07-25，v0.2.19.4-rz）

- 新增程序集 `XuanYu.Editor`（net10.0，仅引用 `XuanYu.Core` 与 `XuanYu.World`），确立编辑器领域边界；首批生产类型 `EditorCameraFraming`（`Camera/`）、`TransformSession`（`Transform/`），均由原归属经纯文件迁移，行为不变。
- 依赖方向落地：Core / World 不得引用 Editor（生产红线）；Editor 不得引用 Editor.UI / Avalonia / Vulkan / Silk.NET；Editor.UI 允许引用 Editor；Editor 最终写入经 `SceneStateOwner` → `GlobalWorld`（World 写入权不变）。
- R4 其余迁移（Gizmo / History / ViewportPicking / DefaultEditorCamera / ScreenPoint）与 R5（Snapshot 边界）保持原计划边界，不在 R4-R1 范围内。

---

## 七、Core 目标形态（极度克制）

```text
XuanYu.Core
├─ Identity/     EntityId
├─ Math/         Vector3d、Aabb3d、Ray3d、基础数学
├─ Time/         SimulationTime
├─ Results/      EngineResult
└─ Diagnostics/  极少量真正全局基础诊断契约
```

以及经证明确需共享的少数基础机制。打开 `XuanYu.Core` 应几乎闻不到“玄域编辑器”或“兵无常势”的味道——这是目标，不是缺陷。

---

## 八、WarCore 不现在创建

维持原判断：本轮只把 Core / World 地基分干净。WarCore 在“奇正相生·一个士兵闭环”正式启动时再建。第一块砖候选：`MilitaryIdentity` / `FactionId` / `MinimalOrganization`，验证 `EntityId #10001` 在 World（它在哪里）与 WarCore（它是谁、属于谁）的两侧投影。不提前造 `IWarPlugin` / `ICombatProvider` / `IFrontlineFactory` 之类的空接口。

---

## 九、对原审计的五点修正记录

1. 原审计建议 `EntityId = Id + Generation` → 修正：`EntityId` 保持纯稳定身份；Generation 属于未来 `EntityHandle`；Revision 独立。
2. 原审计建议 `ViewportPickingService` 可留 Core（解耦 Gizmo 即可）→ 修正：明确归 Editor；Core 只留 Ray/AABB 数学，World 留空间查询。
3. 原审计把 `Spatial.*`（`ISpatialIndex` / `DynamicAabbTree`）判归 Core → 修正：归 World；纯空间数学才留 Core。
4. 原审计把 `SceneStateOwner` / `SceneRenderSnapshot` 整体偏向 Editor / Presentation → 修正：`SceneStateOwner` 归 World/Scene；`SceneRenderSnapshot` 为边界 DTO 归 Render.Abstractions；`DefaultEditorCamera` 归 Editor。
5. 原审计建议物理迁移推迟到 WORLD-B → 修正：在继续 WORLD-A 功能扩张前即按 R0 → R5 小步治理，不等 WORLD-B。

---

## 十、当前进度估计（落库时快照）

| 项目 | 当前估计 | 依据 |
|---|---:|---|
| ARCH-WORLD 分层清晰度 | 约 90% | Core / World / WarCore / Editor / Render 职责与依赖方向基本可冻结 |
| WORLD-A | 约 18% | 功能暂停扩张，先物理分层与单一空间权威治理 |
| Vulkan / 引擎地基 | 约 92% | 本轮不推翻 Vulkan 生命周期成果 |
| 奇正相生可试装准备度 | 约 36% | WarCore 未启动，World 分层完成后进入一个士兵闭环更稳 |
| 总体完成度 | 约 24% | 必要的架构边界定型，不是重新造引擎 |

---

关联：`docs/玄域引擎_AI开发宪法.md` 第二十六条；`docs/dev-rules.md` 第 15 节；`docs/arch-world-layer-attribution.svg`。
