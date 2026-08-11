using System.Text.RegularExpressions;

namespace XuanYu.Editor.MapDocument;

// MAP-DOC-A-R1：Manifest 严格校验；Dataset/Asset 项目内容留给后续轮次。
public static partial class MapManifestValidator
{
    static readonly Regex IdPattern = BuildIdPattern();

    public static MapDocumentResult<MapManifest> Validate(MapManifest? manifest)
    {
        if (manifest is null) return Fail("MissingManifest", "地图 Manifest 为空。", "manifest");
        if (manifest.Format != MapManifest.CurrentFormat)
            return Fail("InvalidFormat", "地图 Manifest format 不受支持。", "format");
        if (manifest.Version != MapManifest.CurrentVersion)
            return Fail("UnsupportedVersion", "地图 Manifest version 不受支持。", "version");
        if (!IdPattern.IsMatch(manifest.Id))
            return Fail("InvalidId", "地图 ID 必须是小写字母、数字、短横线、下划线或点组成的稳定标识。", "id");
        if (string.IsNullOrWhiteSpace(manifest.Name))
            return Fail("InvalidName", "地图名称不能为空。", "name");
        if (manifest.CoordinateSystem is null)
            return Fail("InvalidCoordinateSystem", "coordinate_system 缺失。", "coordinate_system");
        if (manifest.CoordinateSystem.Type != "local_cartesian")
            return Fail("InvalidCoordinateSystem", "坐标系统类型必须为 local_cartesian。", "coordinate_system.type");
        if (manifest.CoordinateSystem.Unit != "meter")
            return Fail("InvalidCoordinateSystem", "坐标单位必须为 meter。", "coordinate_system.unit");
        if (manifest.Datasets.IsDefault)
            return Fail("InvalidDatasets", "datasets 必须是数组。", "datasets");
        if (manifest.Assets.IsDefault)
            return Fail("InvalidAssets", "assets 必须是数组。", "assets");
        return MapDocumentResult<MapManifest>.Ok(manifest);
    }

    [GeneratedRegex("^[a-z0-9][a-z0-9._-]{0,127}$", RegexOptions.CultureInvariant)]
    private static partial Regex BuildIdPattern();

    static MapDocumentResult<MapManifest> Fail(string code, string message, string detail) =>
        MapDocumentResult<MapManifest>.Fail(code, message, "Validate", detail);
}
