# XYE-EDITOR-R1-L — Editor 四级目录与 SRP 收口

状态：目录修复完成；工程验证未运行；整体 5+100/SRP 判定为 PARTIAL。

## 基线与边界

- 工作树 C# 源文件：258 个；Authoring 92、MapEditing 72、MapDocument 47、Assets 31、SceneDocument 16。
- 本轮纯路径迁移：13 个文件；C# 内容 SHA-256 与 R1-K 清单一致。
- file-tree.md：修复 24 条 K3 旧路径，并同步本轮 13 个最终位置；258 个 Editor C# 路径与工作树一一对应。
- Restore / Build / Test / Clean / Rebuild：均未执行。
- 完整唯一源目标与 SHA-256：见 [editor-migration-map.csv](editor-migration-map.csv)。``R1LPreviousPath`` 标记本轮实际移动的源位置。

## 完整目录容量树

计数：直接 C# 文件 / 直接其他文件 / 直接子目录 / 合计直接项。生成目录 bin、obj、artifacts 不纳入源码树。项目根 .csproj 是否纳入直接项守卫仍待裁定。

- `XuanYu.Editor/` — C# 0 / 其他文件 1 / 子目录 5 / 直接项 6
  - `XuanYu.Editor/Assets/` — C# 0 / 其他文件 0 / 子目录 5 / 直接项 5
  - `XuanYu.Editor/Authoring/` — C# 0 / 其他文件 0 / 子目录 5 / 直接项 5
  - `XuanYu.Editor/MapDocument/` — C# 0 / 其他文件 0 / 子目录 5 / 直接项 5
  - `XuanYu.Editor/MapEditing/` — C# 0 / 其他文件 0 / 子目录 5 / 直接项 5
  - `XuanYu.Editor/SceneDocument/` — C# 0 / 其他文件 0 / 子目录 5 / 直接项 5
    - `XuanYu.Editor/Assets/Catalog/` — C# 1 / 其他文件 0 / 子目录 0 / 直接项 1
    - `XuanYu.Editor/Assets/Gltf/` — C# 0 / 其他文件 0 / 子目录 4 / 直接项 4
    - `XuanYu.Editor/Assets/Hosting/` — C# 0 / 其他文件 0 / 子目录 4 / 直接项 4
    - `XuanYu.Editor/Assets/Identity/` — C# 1 / 其他文件 0 / 子目录 0 / 直接项 1
    - `XuanYu.Editor/Assets/StaticModels/` — C# 0 / 其他文件 0 / 子目录 3 / 直接项 3
    - `XuanYu.Editor/Authoring/CoordinationFlow/` — C# 0 / 其他文件 0 / 子目录 5 / 直接项 5
    - `XuanYu.Editor/Authoring/CoordinationState/` — C# 0 / 其他文件 0 / 子目录 5 / 直接项 5
    - `XuanYu.Editor/Authoring/InputContracts/` — C# 0 / 其他文件 0 / 子目录 4 / 直接项 4
    - `XuanYu.Editor/Authoring/InputRuntime/` — C# 0 / 其他文件 0 / 子目录 5 / 直接项 5
    - `XuanYu.Editor/Authoring/ViewportInteraction/` — C# 0 / 其他文件 0 / 子目录 5 / 直接项 5
    - `XuanYu.Editor/MapDocument/Contracts/` — C# 0 / 其他文件 0 / 子目录 4 / 直接项 4
    - `XuanYu.Editor/MapDocument/Mapping/` — C# 0 / 其他文件 0 / 子目录 3 / 直接项 3
    - `XuanYu.Editor/MapDocument/Owners/` — C# 0 / 其他文件 0 / 子目录 2 / 直接项 2
    - `XuanYu.Editor/MapDocument/Persistence/` — C# 0 / 其他文件 0 / 子目录 4 / 直接项 4
    - `XuanYu.Editor/MapDocument/Registry/` — C# 1 / 其他文件 0 / 子目录 4 / 直接项 5
    - `XuanYu.Editor/MapEditing/Geometry/` — C# 0 / 其他文件 0 / 子目录 3 / 直接项 3
    - `XuanYu.Editor/MapEditing/RegionSnap/` — C# 0 / 其他文件 0 / 子目录 4 / 直接项 4
    - `XuanYu.Editor/MapEditing/RegionSpatial/` — C# 0 / 其他文件 0 / 子目录 3 / 直接项 3
    - `XuanYu.Editor/MapEditing/Sessions/` — C# 0 / 其他文件 0 / 子目录 5 / 直接项 5
    - `XuanYu.Editor/MapEditing/ViewportInteraction/` — C# 0 / 其他文件 0 / 子目录 4 / 直接项 4
    - `XuanYu.Editor/SceneDocument/Contracts/` — C# 0 / 其他文件 0 / 子目录 3 / 直接项 3
    - `XuanYu.Editor/SceneDocument/Mapping/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
    - `XuanYu.Editor/SceneDocument/Persistence/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
    - `XuanYu.Editor/SceneDocument/Transactions/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
    - `XuanYu.Editor/SceneDocument/Validation/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/Assets/Gltf/Containers/` — C# 1 / 其他文件 0 / 子目录 0 / 直接项 1
      - `XuanYu.Editor/Assets/Gltf/Importers/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/Assets/Gltf/Reading/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/Assets/Gltf/Transforms/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/Assets/Hosting/Contracts/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/Assets/Hosting/Planning/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/Assets/Hosting/Policies/` — C# 1 / 其他文件 0 / 子目录 0 / 直接项 1
      - `XuanYu.Editor/Assets/Hosting/Transactions/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/Assets/StaticModels/Authoring/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/Assets/StaticModels/Data/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/Assets/StaticModels/Import/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/Authoring/CoordinationFlow/CompositionRoot/` — C# 1 / 其他文件 0 / 子目录 0 / 直接项 1
      - `XuanYu.Editor/Authoring/CoordinationFlow/EditorModes/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/Authoring/CoordinationFlow/History/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/Authoring/CoordinationFlow/WorkspaceContracts/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/Authoring/CoordinationFlow/WorkspaceManagement/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/Authoring/CoordinationState/Commands/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/Authoring/CoordinationState/Owners/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/Authoring/CoordinationState/Snapshots/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/Authoring/CoordinationState/StateContracts/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/Authoring/CoordinationState/StateLayerPolicies/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/Authoring/InputContracts/ConsumerContracts/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/Authoring/InputContracts/Events/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/Authoring/InputContracts/Ports/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/Authoring/InputContracts/State/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/Authoring/InputRuntime/DispatchConsumers/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/Authoring/InputRuntime/LifecycleComposition/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/Authoring/InputRuntime/MapBackend/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/Authoring/InputRuntime/MapConsumers/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/Authoring/InputRuntime/RouterCore/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/Authoring/ViewportInteraction/CameraDiagnostics/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/Authoring/ViewportInteraction/CameraFraming/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/Authoring/ViewportInteraction/CameraNavigation/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/Authoring/ViewportInteraction/CameraSpace/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/Authoring/ViewportInteraction/Transform/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/MapDocument/Contracts/Dataset/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/MapDocument/Contracts/Document/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/MapDocument/Contracts/Manifest/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/MapDocument/Contracts/Validation/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/MapDocument/Mapping/Dataset/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/MapDocument/Mapping/Document/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/MapDocument/Mapping/Manifest/` — C# 1 / 其他文件 0 / 子目录 0 / 直接项 1
      - `XuanYu.Editor/MapDocument/Owners/LegacyDocument/` — C# 1 / 其他文件 0 / 子目录 0 / 直接项 1
      - `XuanYu.Editor/MapDocument/Owners/ManifestBaseline/` — C# 1 / 其他文件 0 / 子目录 0 / 直接项 1
      - `XuanYu.Editor/MapDocument/Persistence/DatasetCodecs/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/MapDocument/Persistence/Serialization/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/MapDocument/Persistence/Storage/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/MapDocument/Persistence/Working/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/MapDocument/Registry/Commands/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/MapDocument/Registry/Identity/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/MapDocument/Registry/Queries/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/MapDocument/Registry/Transactions/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/MapEditing/Geometry/Features/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/MapEditing/Geometry/Snap/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/MapEditing/Geometry/Spatial/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/MapEditing/RegionSnap/EdgeContracts/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/MapEditing/RegionSnap/EdgeResolution/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/MapEditing/RegionSnap/Pipeline/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/MapEditing/RegionSnap/VertexSnap/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/MapEditing/RegionSpatial/Core/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/MapEditing/RegionSpatial/Queries/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/MapEditing/RegionSpatial/Tree/` — C# 2 / 其他文件 0 / 子目录 0 / 直接项 2
      - `XuanYu.Editor/MapEditing/Sessions/Authority/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/MapEditing/Sessions/Commands/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/MapEditing/Sessions/Contracts/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/MapEditing/Sessions/Features/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/MapEditing/Sessions/Operations/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/MapEditing/ViewportInteraction/Context/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/MapEditing/ViewportInteraction/Drafts/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/MapEditing/ViewportInteraction/Ground/` — C# 5 / 其他文件 0 / 子目录 0 / 直接项 5
      - `XuanYu.Editor/MapEditing/ViewportInteraction/HitTesting/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/SceneDocument/Contracts/Models/` — C# 4 / 其他文件 0 / 子目录 0 / 直接项 4
      - `XuanYu.Editor/SceneDocument/Contracts/Results/` — C# 3 / 其他文件 0 / 子目录 0 / 直接项 3
      - `XuanYu.Editor/SceneDocument/Contracts/Schema/` — C# 1 / 其他文件 0 / 子目录 0 / 直接项 1

## SRP 与所有权裁定

- **Editor Coordination：目录归属清晰。** CompositionRoot 仅承载组合工厂；EditorModes 持有 Manage/Edit 模式；EditorState Owners 持有协调状态；Workspace Management 持有当前工作区；History 保留编辑器操作历史。Workspace 定义与模式标识归入 WorkspaceContracts。
- **Editor Interaction：目录归属清晰。** Camera、Input Contracts、Input Runtime、Transform 分开；按键分发在 RouterCore，取消原因在 InputContracts/State，取消 partial 与生命周期协同在 LifecycleComposition。六个 ViewportInputRouter partial 限于 InputRuntime 两个职责叶目录，完整类型重命名仍是 B 类。
- **MapEditing：结构与职责边界可辨。** MapEditSession 是地图内容、选区、编辑历史与 Undo/Redo 的会话权威；Commands、Features、Operations、Authority 按职责分组。Drafts 持有临时绘制状态，Snap/Spatial 是计算与查询边界。本轮不改行为。
- **MapDocument：目录边界清楚，状态 SRP 未通过。** MapDatasetRegistry.CurrentManifest 与 MapManifestOwner.CurrentManifest/IsDirty 同时可变；Editor.UI 明确从 Registry 状态调用 Owner.Modify 同步，存在重复权威与同步过期风险。Registry 保存事务还受既有 K-DATA-001/R1-E 非原子风险约束。列为 C 类，不在本轮改行为。
- **Assets：职责目录清楚。** Hosting、Gltf、StaticModels、Catalog、Identity 边界可辨；SceneAssetHostingTransaction 的 partial 均留在 Transactions。
- **SceneDocument：职责目录清楚。** Contracts、Mapping、Persistence、Transactions、Validation 分离；Session 管理路径、标识、名称与保存修订元数据，不在本轮改事务行为。
- **容量不代表 SRP。** 物理目录行数、直接 C# 数和子目录数合规；MapDocument 的重复 Manifest 状态所有权仍阻止整体 SRP PASS。

## 本轮实际移动

| 原路径（R1-L 前） | 最终路径 | SHA-256 |
|---|---|---|
| `XuanYu.Editor/Authoring/CoordinationFlow/EditorLifecycle/EditorRuntimeComposition.cs` | `XuanYu.Editor/Authoring/CoordinationFlow/CompositionRoot/EditorRuntimeComposition.cs` | `2A19A3426890A65EAB6F25BFDF9A567B50661EB4882520BF01C8CF296ADD9627` |
| `XuanYu.Editor/Authoring/CoordinationFlow/EditorLifecycle/EditorModeId.cs` | `XuanYu.Editor/Authoring/CoordinationFlow/EditorModes/EditorModeId.cs` | `34E4EBAF3E5237AD8823D0B8E6EA93934930F7FE3BE6AC209FC0317951EED8D1` |
| `XuanYu.Editor/Authoring/CoordinationFlow/EditorLifecycle/EditorModeManager.cs` | `XuanYu.Editor/Authoring/CoordinationFlow/EditorModes/EditorModeManager.cs` | `11677B1E74E342B0B17C79EC849DEAEA9788DABC62B3BB1547F20AFE7214E844` |
| `XuanYu.Editor/Authoring/CoordinationFlow/EditorLifecycle/EditorModeTransition.cs` | `XuanYu.Editor/Authoring/CoordinationFlow/EditorModes/EditorModeTransition.cs` | `6B9763200017F5F45599EFB74B903F56E5BCB37887E67D65CFFE1F01837CFCAD` |
| `XuanYu.Editor/Authoring/CoordinationFlow/WorkspaceRegionMode/RegionAuthoringMode.cs` | `XuanYu.Editor/Authoring/CoordinationFlow/WorkspaceContracts/RegionAuthoringMode.cs` | `28A378DC285A937FB06BBB86A47A841EDBD95BF70D385904741C34F807B8C12C` |
| `XuanYu.Editor/Authoring/CoordinationState/IdentityResults/EditorInteractionChangedResult.cs` | `XuanYu.Editor/Authoring/CoordinationState/StateContracts/EditorInteractionChangedResult.cs` | `500BC45983447F99028B26CE49E2451C3A2B51502E5FAD1E763C4A740DC812D3` |
| `XuanYu.Editor/Authoring/CoordinationState/IdentityResults/EditorStateChangedResult.cs` | `XuanYu.Editor/Authoring/CoordinationState/StateContracts/EditorStateChangedResult.cs` | `A969B70C38D655273D129F0BFAA1A70DD2B42CB5E134A5C48D02921DACFC88AD` |
| `XuanYu.Editor/Authoring/CoordinationState/IdentityResults/EditorToolChangedResult.cs` | `XuanYu.Editor/Authoring/CoordinationState/StateContracts/EditorToolChangedResult.cs` | `512729AD8EF5DC0B5A99368C3A340D6D53D6540F6A8FF633AB9CC025D0A4707D` |
| `XuanYu.Editor/Authoring/CoordinationState/IdentityResults/EditorToolId.cs` | `XuanYu.Editor/Authoring/CoordinationState/StateContracts/EditorToolId.cs` | `2691CDAA96C6FDAFAEF674DCC666C66FAE88DBC680EA0C343EA97E6C829B50CE` |
| `XuanYu.Editor/Authoring/CoordinationState/IdentityResults/EditorToolText.cs` | `XuanYu.Editor/Authoring/CoordinationState/StateContracts/EditorToolText.cs` | `D97321DBC351072C49E5DBB6BB20AA9485B99171876933CADEA452BFC79950DD` |
| `XuanYu.Editor/Authoring/InputRuntime/LifecycleComposition/ViewportCancellationReason.cs` | `XuanYu.Editor/Authoring/InputContracts/State/ViewportCancellationReason.cs` | `51AD01735313057D224B31EB308C3A86591315C6666D8746849BB3D2BEA450C0` |
| `XuanYu.Editor/Authoring/InputRuntime/RouterCore/ViewportInputRouter.Cancellation.cs` | `XuanYu.Editor/Authoring/InputRuntime/LifecycleComposition/ViewportInputRouter.Cancellation.cs` | `0E3A647D63D55874DE572616B8D89D965F5C88C5A219F9270EB38DBBB5B9D4F9` |
| `XuanYu.Editor/Authoring/InputContracts/ViewportInputRouterKeyboard/ViewportInputRouter.Keyboard.cs` | `XuanYu.Editor/Authoring/InputRuntime/RouterCore/ViewportInputRouter.Keyboard.cs` | `2990D64055797513180C90A97E710F825027DA6945A9F5CA3C4A578F5B1938A6` |

## 验收状态

- 源码目录容量与深度：PASS（五模块目录；最大四级，按项目根计一级；所有目录直接 C# 文件和子目录均 ≤5）。
- 行数：PASS（258 个手写 C# 文件均 ≤100 行；最长 93 行）。
- SRP：PARTIAL（MapDocument Manifest 状态权威与保存事务风险需 C 类整改；Router partial 仍分两个同属 InputRuntime 的职责叶目录）。
- Editor 项目根：PARTIAL（五个二级职责目录 + XuanYu.Editor.csproj = 6 个直接项；守卫是否计入项目文件未改规则）。
- 工程集成：NOT RUN；待验证唯一文件债务仍为 250，不重复计数。
