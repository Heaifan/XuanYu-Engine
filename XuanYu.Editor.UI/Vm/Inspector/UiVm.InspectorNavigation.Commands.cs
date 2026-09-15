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

    public bool CommitInspectorProperty(string key, string text) => key switch
    {
        "Entity.Basic.Name" => CommitEntityName(text),
        _ => false
    };

    bool CommitEntityName(string text)
    {
        InspectorEntityNameText = text;
        return CommitInspectorEntityName();
    }
}
