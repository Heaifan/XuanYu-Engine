namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    string _datasetNameText = "";

    public string DatasetNameText
    {
        get => _datasetNameText;
        set => Set(ref _datasetNameText, value);
    }

    public async Task RenameSelectedDatasetAsync()
    {
        if (_datasetRegistry is null || DatasetSelectedId is not { } id) return;
        var result = _datasetRegistry.RenameDataset(id, DatasetNameText);
        if (!result.Succeeded) { FooterMessage = result.Message; return; }
        _mapManifestOwner.Modify(_datasetRegistry.CurrentManifest);
        await RefreshDatasetProjectionAsync();
        DatasetNameText = SelectedDataset?.Name ?? "";
        FooterMessage = "数据集名称已应用。";
        RaiseMapDocumentChanged();
    }
}
