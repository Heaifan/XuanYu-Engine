using XuanYu.Editor.MapDocument;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool TryGetDatasetIdForLayer(MapLayerId layerId, out string datasetId)
    {
        var row = _datasetItems.FirstOrDefault(item =>
            item.Type == "区域" && MapDatasetLayerIdProjection.Project(item.Id) == layerId);
        datasetId = row?.Id ?? "";
        return row is not null;
    }

    public bool IsDatasetBackedRegionLayer(MapLayerId layerId) =>
        TryGetDatasetIdForLayer(layerId, out _);
}
