# TERRAIN-HGT-WAVE1-CONVERGENCE-R1

## 唯一收口链

```text
HGT / NASADEM_HGT
  -> HgtTerrainElevationTileReader
  -> TerrainElevationTile
  -> ITerrainElevationQuery
  -> TerrainWorld.FromElevationTile
  -> TerrainWorldProjection
  -> TerrainRenderResource
```

`TerrainElevationTileRuntimeAdapter` 是 Tile 到 Runtime 的入口适配器，
只委托 `TerrainWorld` 的正式投影链，不维护第二套降采样或方向转换算法。

## 冻结事实

- Tile 身份由 `TileId` 表达，不写死台湾或 `n23e121`。
- HGT 样本为有符号 Big-endian Int16，单位为 meter。
- NASADEM HGT 的垂直基准为 `TerrainVerticalDatum.Egm96Geoid`。
- `TerrainGeoBounds` 顺序为 South、West、North、East。
- Tile Query 默认 Bilinear，也允许 Nearest。
- HGT NoData 使用 `short.MinValue`，查询遇到 NoData 返回 Invalid。
- Tile 样本存储复用 `TerrainSourceRaster`，不建立第二份样本容器。
- `XuanYu.World` 不引用 Avalonia 或 XYUI。

## 责任边界

- `IHgtReader`：只定义 Stream 到 Tile 的读取契约。
- `HgtTerrainElevationTileReader`：当前唯一 HGT Reader；保留现有 ZIP 便捷入口供已有 UI 候选代码调用。
- `TerrainElevationTile`：唯一 Tile 数据模型和经纬度 Query 实现。
- `TerrainWorldFactory.FromElevationTile`：把 Tile 接入现有 TerrainWorld。
- `TerrainWorldProjection`：唯一 TerrainWorld 到 Render Heightfield 的投影实现。
- `TerrainElevationTileRuntimeAdapter`：薄适配入口，不复制投影逻辑。

## 本轮未处理

- 不改 Editor.UI / XYUI。
- 不新增最终 Import UI 行为。
- 不实现多 Tile、Streaming、Terrain Mesh 或真机验收。
- 现有 `TerrainGeographicBounds` 保留给旧 ESRI ASCII Grid 链，后续不得在新 HGT 代码中继续使用。

## 验证重点

- Stream Reader 产出 NASADEM 元数据。
- Big-endian 解码和 HGT 南北方向成立。
- Bilinear / NoData Query 契约成立。
- 3601×3601 Tile 可保留完整 World 数据并投影为 513×513 Runtime Snapshot。
