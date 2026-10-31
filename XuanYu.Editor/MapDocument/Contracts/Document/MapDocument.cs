using System.Collections.Immutable;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：地图文档 DTO（.xymap v1 持久化模型）。表达地图文件数据，
// 不负责文件、UI、渲染与高度查询。领域权威在 XuanYu.World.Map（MapDefinition）。
// MAP-A-R2-D1：默认值按 R2 合同调整——默认 10000×10000 米、Flat 地表；
// 默认地图工厂见 World.MapDefaultDefinition（含默认图层与空区域集合）。
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
}
