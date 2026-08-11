using System.Collections.Immutable;

namespace XuanYu.Editor.MapDocument;

public sealed partial class MapDatasetRegistry
{
    public MapDocumentResult<string> UpdateLayerStates(IEnumerable<DatasetLayerState> states)
    {
        var candidate = CurrentManifest with
        {
            DatasetLayerStates = states.Select((item, index) => item with { Order = index }).ToImmutableArray()
        };
        var valid = MapManifestValidator.Validate(candidate);
        if (!valid.Succeeded)
            return MapDocumentResult<string>.Fail(valid.ErrorCode, valid.Message, valid.Stage, valid.Detail);
        CurrentManifest = candidate;
        return MapDocumentResult<string>.Ok(MapPath);
    }
}
