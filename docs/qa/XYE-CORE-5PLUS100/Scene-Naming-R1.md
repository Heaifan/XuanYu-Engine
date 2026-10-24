# XYE Core Scene｜命名精简 R1

日期：2026-10-24。基线：`f9a0ef9553391a2ceca73b362c9a483b3acb7b7f`。
范围：仅五份 `XuanYu.Core.Tests/Scene` 的独立测试源码。保持 `test_` 前缀、职责子目录与每夹≤5/每文件≤100行的硬约束。
原则：模块/子目录已给出上下文，测试文件名与方法名不重复堆叠 `Scene`、`Transform`、`Snapshot` 等词。测试函数以“短动词+关键语义”命名，保留 Assert 和测试数据完全不变。

## 一、文件名和测试类名

| 原测试文件 | 精简后文件 | 原测试类 → 新测试类 |
| --- | --- | --- |
| `Render/test_ISceneRenderSnapshotSource.cs` | `Render/test_RenderSource.cs` | `SceneRenderSnapshotSourceTests` → `RenderSourceTests` |
| `Render/test_SceneRenderSnapshot.cs` | `Render/test_RenderSnapshot.cs` | `SceneRenderSnapshotTests` → `RenderSnapshotTests` |
| `State/test_SceneStateContracts.cs` | `State/test_State.cs` | `SceneStateContractsTests` → `StateTests` |
| `Transform/test_CommittedTransform.cs` | `Transform/test_Committed.cs` | `CommittedTransformTests` → `CommittedTests` |
| `Transform/test_PreviewTransform.cs` | `Transform/test_Preview.cs` | `PreviewTransformTests` → `PreviewTests` |

新文件名前加上目录，分别是 `Scene/Render/test_RenderSource.cs` 等。旧文件已删除，不保留重复版本。

## 二、全部方法命名映射（19个，中文释义）

### Render / test_RenderSource.cs
- `PropertyAndEventPublishTheSameSnapshot` → `PublishesSnapshot`（快照属性与事件同步发布）。

### Render / test_RenderSnapshot.cs
- `EmptyHasNoEntitiesAndIdentityTransform` → `EmptyIsNeutral`（空快照保持空实体、单位变换）。
- `TestEntityAtOriginHasOneEntity` → `OriginHasEntity`（原点测试实体）。
- `RenderEntitiesOverridePrimaryFallback` → `ExplicitEntitiesWin`（显式实体集合优先）。
- `PreviewAppliesOnlyToPrimaryId` → `PreviewTargetsPrimary`（仅主实体应用预览）。
- `InvalidPrimaryIdNeverReceivesPreview` → `InvalidIdIgnoresPreview`（无效ID不得应用预览）。
- `ExplicitCameraOverridesDefault` → `ExplicitCameraWins`（显式相机优先）。

### State / test_State.cs
- `EntityValidityUsesIdOnly` → `ValidatesEntityId`（实体有效性取决于ID）。
- `TransformCommitRecordsBeforeAfterAndChangedFlag` → `RetainsCommitData`（保留提交前后和变更标志）。
- `CommitResultDoesNotCalculateChanged` → `ChangedIsDeclared`（Changed由调用方声明）。

### Transform / test_Committed.cs
- `IdentityHasZeroPositionRotationAndUnitScale` → `IdentityDefaults`（单位变换默认值）。
- `PositionConstructorUsesIdentityRotationAndScale` → `PositionDefaults`（仅给位置时，旋转/缩放取默认值）。
- `WithMethodsModifyOnlyTheirRequestedComponent` → `WithMethodsAreIsolated`（With函数只修改指定分量）。
- `RejectsNonFiniteTransformComponents` → `RejectsNonFinite`（拒绝NaN/无穷大）。
- `RejectsScaleBelowMinimum` → `RejectsSmallScale`（拒绝小于最小阈值的缩放）。
- `AcceptsMinimumScale` → `AcceptsMinScale`（允许缩放临界值）。

### Transform / test_Preview.cs
- `PositionConstructorPreservesCommittedTransformRules` → `PositionUsesDefaults`（位置构造使用默认旋转/缩放）。
- `TransformConstructorKeepsAllComponents` → `KeepsTransform`（保持完整变换分量）。
- `StartSnapshotExposesEntityAndStartingTransform` → `RetainsStartState`（保留操作开始状态）。

## 三、公共API保留范围与门禁

- 七份 `XuanYu.Core/Scene` 生产源码和公共类型/方法/属性/命名空间均完全不变；包括较长的 `ISceneRenderSnapshotSource` 和 `SceneTransformCommitResult`。这些名称有跨模块引用，不在只能静态编辑时单方面缩短，否则生产声明、文件名和调用方可能失配。
- 每份生产文件名仍匹配其公开类型，有利于AI和IDE直接从符号跳到对应文件；不制造“缩短文件名但类型名不变”的检索歧义。
- 只修改测试类、测试方法、测试文件路径；断言/数据/被测类型/辅助方法逻辑不变。
- 5份测试文件行数依次35、96、46、80、41；文件夹 Render 2、State 1、Transform 2。
- 静态目录/文件命名核查：PASS；编译、xUnit、Windows/Vulkan：NOT RUN；集成验收：NOT PASS。
