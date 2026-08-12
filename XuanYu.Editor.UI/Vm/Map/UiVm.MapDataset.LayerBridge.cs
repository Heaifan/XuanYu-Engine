using XuanYu.Editor.MapDocument;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool TryGetDatasetTypeForLayer(MapLayerId layerId, out string type)
    {
        if (TryGetDatasetIdForLayer(layerId, out var id) && _datasetItems.FirstOrDefault(item => item.Id == id) is { } row) { type = row.Type; return true; }
        type = ""; return false;
    }
    public bool TryGetDatasetIdForLayer(MapLayerId layerId, out string datasetId)
    {
        var row = _datasetItems.FirstOrDefault(item =>
            item.Type is "区域" or "道路" && MapDatasetLayerIdProjection.Project(item.Id) == layerId);
        datasetId = row?.Id ?? "";
        return row is not null;
    }

    public bool IsDatasetBackedRegionLayer(MapLayerId layerId) =>
        TryGetDatasetIdForLayer(layerId, out _);
}
