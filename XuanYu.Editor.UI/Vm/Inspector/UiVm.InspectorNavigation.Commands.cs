namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    string _selectedSearchKey = "";
    public string SelectedInspectorSearchKey => _selectedSearchKey;

    public void MoveInspectorSearch(int direction)
    {
        var rows = InspectorPropertySearch.Find(AllDescriptors(), InspectorSearchText);
        if (rows.Count == 0) return;
        var index = Math.Clamp(rows.ToList().FindIndex(row => row.Key == _selectedSearchKey) + direction, 0, rows.Count - 1);
        _selectedSearchKey = rows[index].Key;
        OnPropertyChanged(nameof(SelectedInspectorSearchKey));
    }

    public void ActivateInspectorSearchResult()
    {
        var rows = InspectorProperties;
        var row = rows.FirstOrDefault(item => item.Key == _selectedSearchKey) ?? rows.FirstOrDefault();
        if (row is null) return;
        SelectInspectorCategory(row.Descriptor.Category);
        _selectedSearchKey = row.Key;
    }

    void SelectInspectorCategory(string category)
    {
        if (InspectorCategories.Contains(category))
        {
            InspectorCategory = category;
            OnPropertyChanged(nameof(InspectorProperties)); OnPropertyChanged(nameof(IsInspectorBasicPage));
            OnPropertyChanged(nameof(IsInspectorPropertyListVisible)); OnPropertyChanged(nameof(IsInspectorRecentCategory));
            OnPropertyChanged(nameof(IsInspectorRecentEmpty)); OnPropertyChanged(nameof(IsInspectorNoResults));
            OnPropertyChanged(nameof(InspectorCategoryItems));
        }
    }

    public bool CommitInspectorProperty(string key, string text)
        => CommitInspectorProperty(CreateInspectorEditTarget(key), text);

    public bool CommitInspectorProperty(InspectorEditTarget target, string text)
    {
        var succeeded = target.PropertyKey switch
        {
            "Entity.Basic.Name" => CommitEntityName(target, text),
            "Road.Basic.Name" or "Region.Basic.Name" or "Marker.Basic.Name" => CommitFeatureName(target, text),
            _ => false
        };
        RecordInspectorCommit(target, succeeded);
        return succeeded;
    }

    bool CommitEntityName(InspectorEditTarget target, string text)
    {
        InspectorEntityNameText = text;
        return TryEntityKey(target.ObjectId, out var key) && RenameEntity(key, text);
    }
}
