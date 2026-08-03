using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：地图文档严格校验。返回结构化结果，不抛出来源不明的异常。
public static class MapDocumentValidator
{
    public const double MinSizeMeters = 100.0;
    public const double MaxSizeMeters = 1000000.0;

    readonly record struct Issue(string Code, string Message, string Detail);
    public static MapDocumentResult<MapDocument> Validate(MapDocument? doc)
    {
        if (doc is null) return Fail("BrokenDocument", "地图文档为空。", "Validate", "");
        if (doc.SchemaVersion != MapDocument.CurrentSchemaVersion)
            return Fail("UnsupportedSchema", "地图文件版本不受支持。", "Validate", "schemaVersion");
        if (!doc.MapId.IsValid)
            return Fail("InvalidMapId", "地图ID非法，必须是32位十六进制。", "Validate", "mapId");
        if (string.IsNullOrWhiteSpace(doc.Name))
            return Fail("InvalidMapName", "地图名称不能为空。", "Validate", "name");

        var issue = ValidateSize(doc.SizeMeters)
            ?? ValidateCoordinateSystem(doc.CoordinateSystem)
            ?? ValidateSurface(doc.Surface)
            ?? ValidateEnvironment(doc.Environment)
            ?? ValidateLayers(doc.LayerReferences);
        if (issue is { } found) return Fail(found);
        return MapDocumentResult<MapDocument>.Ok(doc);
    }

    static Issue? ValidateSize(MapSize? size)
    {
        if (size is null) return new Issue("InvalidSize", "sizeMeters 缺失。", "sizeMeters");
        if (!Finite(size.Width) || !Finite(size.Depth))
            return new Issue("InvalidSize", "地图尺寸必须为有限数字。", "sizeMeters");
        if (size.Width < MinSizeMeters || size.Width > MaxSizeMeters)
            return new Issue("InvalidSize", $"sizeMeters.width 必须位于 {MinSizeMeters}～{MaxSizeMeters} 米之间。", "sizeMeters.width");
        if (size.Depth < MinSizeMeters || size.Depth > MaxSizeMeters)
            return new Issue("InvalidSize", $"sizeMeters.depth 必须位于 {MinSizeMeters}～{MaxSizeMeters} 米之间。", "sizeMeters.depth");
        return null;
    }
    static Issue? ValidateCoordinateSystem(MapCoordinateSystem? coord)
    {
        if (coord is null) return new Issue("InvalidCoordinateSystem", "coordinateSystem 缺失。", "coordinateSystem");
        if (coord.Unit != "meter")
            return new Issue("InvalidCoordinateSystem", "地图单位必须为 meter。", "coordinateSystem.unit");
        if (coord.UpAxis != "Z")
            return new Issue("InvalidCoordinateSystem", "地图上轴必须为 Z（Z-Up）。", "coordinateSystem.upAxis");
        if (coord.Origin is null || !coord.Origin.IsZero)
            return new Issue("InvalidCoordinateSystem", "地图原点必须为零点。", "coordinateSystem.origin");
        return null;
    }

    static Issue? ValidateSurface(MapSurfaceDefinition? surface)
    {
        if (surface is null) return new Issue("InvalidSurface", "surface 缺失。", "surface");
        if (!MapSurfaceKinds.IsKnown(surface.Kind))
            return new Issue("UnknownSurfaceKind", $"未知地表类型：{surface.Kind}。", "surface.kind");
        if (!Finite(surface.BaseHeightMeters) || !Finite(surface.AmplitudeMeters) || !Finite(surface.WavelengthMeters))
            return new Issue("InvalidSurface", "地表参数必须为有限数字。", "surface");
        if (surface.AmplitudeMeters < 0)
            return new Issue("InvalidSurface", "地表起伏幅度不得小于 0。", "surface.amplitudeMeters");
        if (surface.WavelengthMeters <= 0)
            return new Issue("InvalidSurface", "地表波长必须大于 0。", "surface.wavelengthMeters");
        return null;
    }
    static Issue? ValidateEnvironment(MapEnvironmentDefinition? env)
    {
        if (env is null) return new Issue("InvalidEnvironment", "environment 缺失。", "environment");
        if (!MapSkyPresets.IsKnown(env.SkyPreset))
            return new Issue("UnknownSkyPreset", $"未知天空预设：{env.SkyPreset}。", "environment.skyPreset");
        var dir = env.SunDirection;
        if (dir is null || !Finite(dir.X) || !Finite(dir.Y) || !Finite(dir.Z))
            return new Issue("InvalidEnvironment", "太阳方向分量必须为有限数字。", "environment.sunDirection");
        if (dir.IsZero)
            return new Issue("InvalidEnvironment", "太阳方向不得为零向量。", "environment.sunDirection");
        if (!Finite(env.SunIntensity) || env.SunIntensity < 0)
            return new Issue("InvalidEnvironment", "太阳强度必须为非负有限数字。", "environment.sunIntensity");
        if (!Finite(env.AmbientIntensity) || env.AmbientIntensity < 0)
            return new Issue("InvalidEnvironment", "环境光强度必须为非负有限数字。", "environment.ambientIntensity");
        return null;
    }

    static Issue? ValidateLayers(ImmutableArray<string> layers)
    {
        if (layers.IsDefault)
            return new Issue("InvalidLayerReferences", "图层引用字段缺失。", "layerReferences");
        if (layers.Length != 0)
            return new Issue("NonEmptyLayerReferences", "R1 图层引用必须为空数组。", "layerReferences");
        return null;
    }
    static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
    static Issue NewIssue(string code, string message, string detail) => new(code, message, detail);

    static MapDocumentResult<MapDocument> Fail(Issue issue) =>
        MapDocumentResult<MapDocument>.Fail(issue.Code, issue.Message, "Validate", issue.Detail);

    static MapDocumentResult<MapDocument> Fail(string code, string message, string stage, string detail) =>
        MapDocumentResult<MapDocument>.Fail(code, message, stage, detail);
}
