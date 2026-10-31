using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

public sealed partial class MapDatasetRegistry
{
    public MapDocumentResult<string> RenameDataset(string id, string name)
    {
        var trimmed = name.Trim();
        var descriptor = CurrentManifest.Datasets.FirstOrDefault(item =>
            string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase));
        if (descriptor is null)
            return MapDocumentResult<string>.Fail("NotFound", "Dataset 未注册。", "Validate");
        var candidate = CurrentManifest with
        {
            Datasets = CurrentManifest.Datasets.Select(item => item == descriptor
                ? item with { Name = trimmed } : item).ToImmutableArray()
        };
        var valid = MapManifestValidator.Validate(candidate);
        if (!valid.Succeeded)
            return MapDocumentResult<string>.Fail(valid.ErrorCode, valid.Message, valid.Stage, valid.Detail);
        CurrentManifest = candidate;
        return MapDocumentResult<string>.Ok(id);
    }
}
