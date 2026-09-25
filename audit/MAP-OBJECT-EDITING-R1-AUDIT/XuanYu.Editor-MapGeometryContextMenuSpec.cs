using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public readonly record struct MapGeometryContextMenuItem(string Id, string Label, bool IsEnabled);

public static class MapGeometryContextMenuSpec
{
    public static IReadOnlyList<MapGeometryContextMenuItem> Build(MapDefinition map, MapGeometryContextHit hit)
    {
        if (hit.Kind == MapGeometryContextKind.Empty) return [];
        return hit.Kind switch
        {
            MapGeometryContextKind.Vertex => Vertex(map, hit),
            MapGeometryContextKind.Edge => Edge(hit),
            MapGeometryContextKind.Face => Face(hit.Selection),
            MapGeometryContextKind.Marker => Marker(), _ => []
        };
    }

    static IReadOnlyList<MapGeometryContextMenuItem> Vertex(MapDefinition map, MapGeometryContextHit hit) =>
        hit.Selection.Kind == MapGeometryFeatureKind.Region
            ? [new("delete-vertex", "删除顶点", CanDeleteRegionVertex(map, hit))]
            : hit.Selection.Kind == MapGeometryFeatureKind.Road
            ? [new("delete-vertex", "删除顶点", CanDeleteRoadVertex(map, hit))]
            : [new("delete-marker", "删除标记", true)];

    static IReadOnlyList<MapGeometryContextMenuItem> Edge(MapGeometryContextHit hit) =>
        hit.Selection.Kind == MapGeometryFeatureKind.Region
            ? [new("add-vertex", "添加顶点", true), new("delete-region", "删除区域", true)]
            : [new("add-vertex", "添加顶点", true), new("edit-road", "编辑道路", true), new("delete-road", "删除道路", true)];

    static IReadOnlyList<MapGeometryContextMenuItem> Face(MapGeometrySelection selection) =>
        selection.Kind == MapGeometryFeatureKind.Region
            ? [new("edit-region", "编辑几何", true), new("delete-region", "删除区域", true)]
            : [new("edit-road", "编辑道路", true), new("delete-road", "删除道路", true)];

    static IReadOnlyList<MapGeometryContextMenuItem> Marker() =>
        [new("edit-marker", "编辑标记", true), new("delete-marker", "删除标记", true)];

    static bool CanDeleteRegionVertex(MapDefinition map, MapGeometryContextHit hit) =>
        map.Regions.FirstOrDefault(item => item.RegionId.ToString() == hit.Selection.FeatureId)?.Vertices.Length > 3;

    static bool CanDeleteRoadVertex(MapDefinition map, MapGeometryContextHit hit) =>
        map.Roads.FirstOrDefault(item => item.RoadId.ToString() == hit.Selection.FeatureId)?.Points.Length > 2;
}
