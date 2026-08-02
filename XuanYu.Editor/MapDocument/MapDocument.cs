using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：地图文档（内存模型）。表达地图数据，不负责文件、UI、渲染与高度查询。
// LayerReferences 用 ImmutableArray 保证 record 结构相等（Round-trip 断言可靠）。
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
        double width = 2000.0,
        double depth = 2000.0,
        MapSurfaceDefinition? surface = null,
        MapEnvironmentDefinition? environment = null) =>
        new(
            CurrentSchemaVersion,
            MapId.New(),
            name,
            new MapSize(width, depth),
            MapCoordinateSystem.ZUpMeter,
            surface ?? MapSurfaceDefinition.DefaultGentleHills,
            environment ?? MapEnvironmentDefinition.DefaultClearDay,
            ImmutableArray<string>.Empty);
}
