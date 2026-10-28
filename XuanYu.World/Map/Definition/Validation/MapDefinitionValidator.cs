using System.Collections.Immutable;

namespace XuanYu.World.Map;

// MAP-A-R2-D1-F1：地图聚合严格校验（领域权威层）。
// 覆盖：MapId/名称、尺寸范围、坐标系统（meter+Z-Up+零点）、地表白名单与参数域、
// 图层集合、区域集合。持久化 DTO（.xymap）另有 schema 层校验（Editor）。
public static class MapDefinitionValidator
{
    public const double MinSizeMeters = 100.0;
    public const double MaxSizeMeters = 1000000.0;

    public static MapValidationResult Validate(MapDefinition? map)
    {
        if (map is null)
            return MapValidationResult.Fail("NullMap", "地图聚合为空。");
        if (!map.MapId.IsValid)
            return MapValidationResult.Fail("InvalidMapId", "地图ID非法，必须是32位十六进制。");
        if (string.IsNullOrWhiteSpace(map.DisplayName))
            return MapValidationResult.Fail("InvalidMapName", "地图名称不能为空。");

        if (ValidateSize(map.SizeMeters) is { } sizeIssue) return sizeIssue;
        if (ValidateCoordinateSystem(map.CoordinateSystem) is { } coordIssue) return coordIssue;
        if (ValidateSurface(map.Surface) is { } surfaceIssue) return surfaceIssue;
        var layersResult = MapLayerValidator.Validate(map.Layers);
        if (!layersResult.Succeeded) return layersResult;
        var regionsResult = MapRegionValidator.Validate(map.Regions, map.Layers, map.SizeMeters);
        if (!regionsResult.Succeeded) return regionsResult;
        var roadsResult = MapRoadValidator.Validate(map.Roads, map.Layers, map.SizeMeters);
        if (!roadsResult.Succeeded) return roadsResult;
        var markersResult = MapMarkerValidator.Validate(map.Markers.IsDefault ? [] : map.Markers, map.Layers, map.SizeMeters);
        if (!markersResult.Succeeded) return markersResult;
        return MapValidationResult.Ok();
    }

    static MapValidationResult? ValidateSize(MapSize? size)
    {
        if (size is null) return Fail("InvalidSize", "地图尺寸缺失。");
        if (!Finite(size.Width) || !Finite(size.Depth))
            return Fail("InvalidSize", "地图尺寸必须为有限数字。");
        if (size.Width < MinSizeMeters || size.Width > MaxSizeMeters)
            return Fail("InvalidSize", $"地图宽度必须位于 {MinSizeMeters}～{MaxSizeMeters} 米之间。");
        if (size.Depth < MinSizeMeters || size.Depth > MaxSizeMeters)
            return Fail("InvalidSize", $"地图深度必须位于 {MinSizeMeters}～{MaxSizeMeters} 米之间。");
        return null;
    }

    static MapValidationResult? ValidateCoordinateSystem(MapCoordinateSystem? coord)
    {
        if (coord is null) return Fail("InvalidCoordinateSystem", "坐标系统缺失。");
        if (coord.Unit != "meter")
            return Fail("InvalidCoordinateSystem", "地图单位必须为 meter。");
        if (coord.UpAxis != "Z")
            return Fail("InvalidCoordinateSystem", "地图上轴必须为 Z（Z-Up）。");
        if (coord.Origin is null || !coord.Origin.IsZero)
            return Fail("InvalidCoordinateSystem", "地图原点必须为零点。");
        return null;
    }

    static MapValidationResult? ValidateSurface(MapSurfaceDefinition? surface)
    {
        if (surface is null) return Fail("InvalidSurface", "地表缺失。");
        if (!MapSurfaceKinds.IsKnown(surface.Kind))
            return Fail("UnknownSurfaceKind", $"未知地表类型：{surface.Kind}。");
        if (!Finite(surface.BaseHeightMeters) || !Finite(surface.AmplitudeMeters) || !Finite(surface.WavelengthMeters))
            return Fail("InvalidSurface", "地表参数必须为有限数字。");
        if (surface.AmplitudeMeters < 0)
            return Fail("InvalidSurface", "地表起伏幅度不得小于 0。");
        if (surface.WavelengthMeters <= 0)
            return Fail("InvalidSurface", "地表波长必须大于 0。");
        return null;
    }

    static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);

    static MapValidationResult Fail(string code, string message) =>
        MapValidationResult.Fail(code, message, "ValidateDefinition");
}
