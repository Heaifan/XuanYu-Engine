using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

public sealed partial class MapDatasetRegistry
{
    public async Task<MapDocumentResult<string>> UnregisterAsync(string id)
    {
        var descriptor = CurrentManifest.Datasets.FirstOrDefault(
            item => string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase));
        if (descriptor is null) return MapDocumentResult<string>.Fail("NotFound", "Dataset 未注册。", "Validate");
        if (CurrentManifest.DatasetLayerStates.Any(item =>
                string.Equals(item.DatasetId, id, StringComparison.OrdinalIgnoreCase) && item.IsLocked))
            return MapDocumentResult<string>.Fail("DatasetLocked", "已锁定的数据集不能解除注册。", "Validate");
        var candidate = CurrentManifest with
        {
            Datasets = CurrentManifest.Datasets.Where(item => item != descriptor).ToImmutableArray(),
            DatasetLayerStates = CurrentManifest.DatasetLayerStates.Where(item =>
                    !string.Equals(item.DatasetId, id, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Order).Select((item, index) => item with { Order = index }).ToImmutableArray()
        };
        var valid = MapManifestValidator.Validate(candidate);
        if (!valid.Succeeded) return MapDocumentResult<string>.Fail(valid.ErrorCode, valid.Message, valid.Stage, valid.Detail);
        var saved = await _manifestStorage.SaveAsync(MapPath, candidate);
        if (!saved.Succeeded) return saved;
        CurrentManifest = candidate;
        return MapDocumentResult<string>.Ok(id);
    }
}
