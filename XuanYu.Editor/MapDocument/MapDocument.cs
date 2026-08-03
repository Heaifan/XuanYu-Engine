using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：地图文档（内存模型）。表达地图数据，不负责文件、UI、渲染与高度查询。
// LayerReferences 用 ImmutableArray 保证 record 结构相等（Round-trip 断言可靠）。
// MAP-A-R2-D1：默认值按 R2 合同调整——默认 10000×10000 米、Flat 地表；
// Layers/Regions 领域模型已独立建立（MapLayer/MapRegion），挂载与持久化在 D4/D6。
public sealed record MapDocument(
    int SchemaVersion,
    MapId MapId,
    string Name,
    MapSize SizeMeters,
    MapCoordinateSystem CoordinateSystem,
    MapSurfaceDefinition Surface,
    MapEnvironmentDefinition Environment,
    ImmutableArray<string> LayerReferences)
{
    public const int CurrentSchemaVersion = 1;

    public static MapDocument CreateNew(
        string name,
        double width = 10000.0,
        double depth = 10000.0,
        MapSurfaceDefinition? surface = null,
        MapEnvironmentDefinition? environment = null) =>
        new(
            CurrentSchemaVersion,
            MapId.New(),
            name,
            new MapSize(width, depth),
            MapCoordinateSystem.ZUpMeter,
            surface ?? MapSurfaceDefinition.DefaultFlat,
            environment ?? MapEnvironmentDefinition.DefaultClearDay,
            ImmutableArray<string>.Empty);

    // MAP-A-R2-D1：默认地图工厂。R2 默认地图：10 km × 10 km、平面地表、中心原点。
    public static MapDocument CreateDefault() => CreateNew("未命名地图");
}
