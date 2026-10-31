using XuanYu.Editor.MapDocument;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    string _datasetNameText = "";

    public string DatasetNameText
    {
        get => _datasetNameText;
        set => Set(ref _datasetNameText, value);
    }

    public async Task<bool> RenameSelectedDatasetAsync(string? requestedName = null)
    {
        if (requestedName is not null) DatasetNameText = requestedName;
        if (_datasetRegistry is null || DatasetSelectedId is not { } id) return false;
        var result = _datasetRegistry.RenameDataset(id, DatasetNameText);
        if (!result.Succeeded) { FooterMessage = result.Message; return false; }
        _mapManifestOwner.Modify(_datasetRegistry.CurrentManifest);
        var runtime = MapDatasetRuntimeProjection.Apply(MapSession.CurrentMap, _datasetRegistry.CurrentManifest);
        if (!runtime.Succeeded || runtime.Value is null) { FooterMessage = runtime.Message; return false; }
        var applied = MapSession.ApplyRuntimeLayerProjection(runtime.Value);
        if (!applied.IsSuccess) { FooterMessage = applied.Error!.Value.Message; return false; }
        await RefreshDatasetProjectionAsync();
        DatasetNameText = SelectedDataset?.Name ?? "";
        FooterMessage = "数据集名称已应用。";
        RaiseMapDocumentChanged();
        return true;
    }
}
