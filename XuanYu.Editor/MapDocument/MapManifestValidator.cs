using System.Text.RegularExpressions;

namespace XuanYu.Editor.MapDocument;

// MAP-DOC-A-R2-C1：Manifest Dataset Descriptor 合同校验。
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
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var dataset in manifest.Datasets)
        {
            if (!IdPattern.IsMatch(dataset.Id) || dataset.Id.Contains('.'))
                return Fail("InvalidDatasetId", "Dataset ID 必须是小写字母、数字、短横线或下划线。", "datasets.id");
            if (!MapDatasetTypes.IsKnown(dataset.Type))
                return Fail("InvalidDatasetType", "Dataset type 不在允许的六类之内。", "datasets.type");
            if (dataset.Name is not null && string.IsNullOrWhiteSpace(dataset.Name))
                return Fail("InvalidDatasetName", "Dataset 名称不能为空。", "datasets.name");
            if (!MapDatasetPathPolicy.IsSafeSource(dataset.Source))
                return Fail("InvalidDatasetSource", "Dataset source 必须是 map 根目录下 data/ 内的安全相对路径。", "datasets.source");
            if (!ids.Add(dataset.Id))
                return Fail("DuplicateDatasetId", "Dataset ID 必须大小写不敏感唯一。", "datasets.id");
        }
        if (manifest.DatasetLayerStates.IsDefault || manifest.DatasetLayerStates.Length != manifest.Datasets.Length)
            return Fail("InvalidDatasetLayerState", "Dataset Layer State 必须与 Dataset 一一对应。", "dataset_layer_state");
        var stateIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var orders = new HashSet<int>();
        foreach (var state in manifest.DatasetLayerStates)
        {
            if (!ids.Contains(state.DatasetId) || !stateIds.Add(state.DatasetId) ||
                state.Order < 0 || !orders.Add(state.Order))
                return Fail("InvalidDatasetLayerState", "Dataset Layer State 引用或顺序无效。", "dataset_layer_state");
        }
        if (!orders.SetEquals(Enumerable.Range(0, manifest.DatasetLayerStates.Length)))
            return Fail("InvalidDatasetLayerState", "Dataset Layer State 顺序必须连续。", "dataset_layer_state");
        return MapDocumentResult<MapManifest>.Ok(manifest);
    }

    [GeneratedRegex("^[a-z0-9][a-z0-9_-]{0,127}$", RegexOptions.CultureInvariant)]
    private static partial Regex BuildIdPattern();

    static MapDocumentResult<MapManifest> Fail(string code, string message, string detail) =>
        MapDocumentResult<MapManifest>.Fail(code, message, "Validate", detail);
}
