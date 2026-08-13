# MAP-DATA-A-R2-F3-E1 · Region/Road Mapping & Gap Report

状态：`CLOSED`（历史只读调查；Road Generic Snap 缺口已由 F3-E 实现并通过真机验收）。基线：`5822bb3`。

## Capability Mapping

| 能力 | Region | Road | 事实依据 |
|---|---|---|---|
| GeometryKind | Polygon | Polyline | `MapRegion.Vertices` 闭合语义；`MapRoad.Points` 开放语义 |
| Selectable | YES | YES | `MapGeometryHitTester.TryHitFeature` 与 UiVm 选择路径 |
| VertexEditable | YES | YES | `TryBeginMapGeometryVertexPointer` + `MapEditSession.EditRegionVertices/EditRoadPoints` |
| Snappable | YES | NO（待 E5） | Region 进入 `RegionSnapPipeline`；Road 当前仅自由拖动 |
| SnapTarget Vertex | YES | NO（待 E5） | Region Local Query → Candidate；Road 尚未进入候选源 |
| SnapTarget Segment | YES | NO（待 E5） | Region Edge Resolver；Road Segment 尚未进入候选源 |
| Preview | YES | YES | 共享 `MapGeometryPreview` / `PreviewMapGeometryPointer` |
| Commit | YES | YES | 共享 `MapEditSession.CommitMapChange`，领域入口不同 |
| Esc Cancel | YES | YES | 共享 `CancelMapGeometryPointer` |
| Undo/Redo | YES | YES | 共享 `MapHistoryEntry` 与 `MapEditSession.Undo/Redo` |
| Closed Geometry | YES | NO | Polygon 隐式尾点→首点；Polyline 不闭合 |

## E1-Q1 · Geometry Source / Identity

正式链路为：

```text
Manifest Dataset Descriptor (DatasetId, Type, Source)
  ↓ MapDatasetRegistry load
MapDatasetDocument.Features (JSON Feature)
  ↓ MapDatasetFeatureBinding + Region/Road Dataset Codec
MapDefinition.Regions / MapDefinition.Roads
  ↓
MapRegion.Vertices / MapRoad.Points
```

`MapRegion.RegionId` / `MapRoad.RoadId` 拥有 Feature Identity；`MapDatasetDescriptor.Id` 拥有 Dataset Identity；`MapDatasetLayerIdProjection.Project(DatasetId)` 生成 LayerId，把 Feature 归属映射到 Dataset。`MapEditSession.CurrentMap` 是运行时唯一地图内容真源；`MapDatasetRegistry.BuildFeatureSaveCandidates` 按 LayerId 将修改后的 Feature 写回对应 Dataset，未创建第二份 Geometry Source。

## E1-Q2 · Edit Lifecycle / History

- A（已共享）：`MapGeometryDrag`、`MapGeometryPreview`、`PreviewMapGeometryPointer`、`CommitMapGeometryPointer`、`CancelMapGeometryPointer`、`MapEditSession.CommitMapChange`、`MapHistoryEntry`、Undo/Redo。
- B（结构类似，需 Adapter）：Region/Road 的顶点读取、ID 解析、Commit 入口和显示几何分支。
- C（Feature-specific）：Region Polygon 合法性/空间索引同步与闭合边界；Road Polyline 相邻重复节点校验与开放结构；Region Snap Solver。

## E1-Q3 · Picking

屏幕空间半径语义已共享于 `MapGeometryHitTester.TryHitVertex`（10 DIP）；Feature Picking 与 Vertex Picking 由同一类编排，但内部仍有 Region inside-polygon 与 Road segment-distance 分支。当前不存在 Road 专属 HitTester；这不是完全形状无关的算法，E2 不得假设无需 Adapter。

## E1-Q4 · Snap Candidate Source

Region 已有 `MapEditSession.QueryLocalRegions` → `RegionSpatialIndex` → `RegionSnapQuery.BuildCandidates` → Vertex/Segment Resolver；查询只覆盖局部 Region 候选。Road 当前没有局部空间索引，也没有 Road Vertex/Segment Candidate Query。

结论：`GAP-01` Road lacks local snap candidate query。E1 不补索引，不扫描全部 Road；E3/E5 必须先确定统一局部候选源。

## E1-Q5 · History Generality

历史容器已是 Map 级通用 `MapHistoryEntry(Before, After, Reason)`，可覆盖 Region/Road 的整张 `MapDefinition` 状态；但没有独立的 `GeometryEdit(DatasetId, FeatureId, Before, After)` 记录。`GAP-02`：E2 如需细粒度 Geometry Edit 诊断，必须在不破坏 Map History 合同的前提下评估，不能直接替换现有历史模型。

## 全量扫描审计

当前 F3-C Region Candidate 正式路径使用 `QueryLocalRegions`；没有发现为 Generic Snap 新增的 PointerMove 全量 Road 扫描。现有 `MapGeometryHitTester` 的 Feature/Vertex Picking 是编辑选择路径，不是已实现的 Generic Snap 候选路径；E3/E4 必须保持这个边界。
