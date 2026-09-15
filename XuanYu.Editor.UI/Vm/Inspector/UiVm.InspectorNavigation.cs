using System.Windows.Input;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly InspectorRecentStore _inspectorRecent = new();
    InspectorObjectKind _navigationKind;
    string _inspectorSearchText = "";
    string _inspectorCategory = "基础";
    string _categoryBeforeSearch = "";

    public string InspectorSearchText
    {
        get => _inspectorSearchText;
        set
        {
            if (!Set(ref _inspectorSearchText, value)) return;
            if (value.Length > 0 && _categoryBeforeSearch == "") _categoryBeforeSearch = InspectorCategory;
            if (value.Length == 0 && _categoryBeforeSearch != "") { InspectorCategory = _categoryBeforeSearch; _categoryBeforeSearch = ""; }
            RefreshInspectorNavigation();
        }
    }
    public string InspectorCategory { get => _inspectorCategory; private set => Set(ref _inspectorCategory, value); }
    public bool IsInspectorSearchMode => _inspectorSearchText.Length > 0;
    public bool IsInspectorBasicPage => !IsInspectorSearchMode && InspectorCategory == "基础";
    public bool IsInspectorPropertyListVisible => IsInspectorSearchMode || InspectorCategory != "基础";
    public bool IsMapInspectorBasicPage => IsMapInspector && IsInspectorBasicPage;
    public bool IsMarkerInspectorBasicPage => IsMarkerInspector && IsInspectorBasicPage;
    public bool IsFeatureInspectorBasicPage => IsFeatureInspector && IsInspectorBasicPage;
    public bool IsEntityInspectorBasicPage => IsEntityInspector && IsInspectorBasicPage;
    public IReadOnlyList<string> InspectorCategories => CategoriesFor(InspectorIdentity);
    public IReadOnlyList<InspectorPropertyRow> InspectorProperties =>
        IsInspectorSearchMode ? SearchRows() : RowsFor(InspectorCategory);
    public ICommand SelectInspectorCategoryCommand => new RelayCommand(value => SelectCategory(value?.ToString()));

    public void RefreshInspectorNavigation()
    {
        var kind = InspectorIdentity;
        if (kind != _navigationKind)
        {
            _navigationKind = kind;
            InspectorCategory = CategoriesFor(kind).FirstOrDefault() ?? "基础";
        }
        OnPropertyChanged(nameof(InspectorCategories)); OnPropertyChanged(nameof(InspectorProperties));
        OnPropertyChanged(nameof(IsInspectorSearchMode)); OnPropertyChanged(nameof(IsInspectorBasicPage));
        OnPropertyChanged(nameof(IsInspectorPropertyListVisible)); OnPropertyChanged(nameof(IsMapInspectorBasicPage));
        OnPropertyChanged(nameof(IsMarkerInspectorBasicPage)); OnPropertyChanged(nameof(IsFeatureInspectorBasicPage));
        OnPropertyChanged(nameof(IsEntityInspectorBasicPage));
    }

    void SelectCategory(string? category)
    {
        if (string.IsNullOrWhiteSpace(category) || !InspectorCategories.Contains(category)) return;
        InspectorCategory = category;
        RaiseInspectorNavigationBindings();
    }

    void RaiseInspectorNavigationBindings()
    {
        OnPropertyChanged(nameof(InspectorProperties)); OnPropertyChanged(nameof(IsInspectorBasicPage));
        OnPropertyChanged(nameof(IsInspectorPropertyListVisible)); OnPropertyChanged(nameof(IsMapInspectorBasicPage));
        OnPropertyChanged(nameof(IsMarkerInspectorBasicPage)); OnPropertyChanged(nameof(IsFeatureInspectorBasicPage));
        OnPropertyChanged(nameof(IsEntityInspectorBasicPage));
    }

    IReadOnlyList<InspectorPropertyRow> SearchRows() => InspectorPropertySearch.Find(AllDescriptors(), _inspectorSearchText)
        .Select(Row).ToArray();

    IReadOnlyList<InspectorPropertyRow> RowsFor(string category)
    {
        if (category == "最近") return _inspectorRecent.KeysFor(InspectorIdentity)
            .Select(key => AllDescriptors().FirstOrDefault(item => item.Key == key)).OfType<InspectorPropertyDescriptor>()
            .Select(Row).ToArray();
        return AllDescriptors().Where(item => item.Category == category).Select(Row).ToArray();
    }

    IReadOnlyList<string> CategoriesFor(InspectorObjectKind kind)
    {
        if (kind == InspectorObjectKind.Empty) return [];
        var categories = AllDescriptors().Select(item => item.Category).Distinct().ToList();
        if (_inspectorRecent.KeysFor(kind).Count > 0) categories.Insert(0, "最近");
        return categories;
    }

    InspectorPropertyRow Row(InspectorPropertyDescriptor descriptor) =>
        new(descriptor, ValueFor(descriptor.Key), descriptor.Key == "Entity.Basic.Name");

    IReadOnlyList<InspectorPropertyDescriptor> AllDescriptors() => InspectorDescriptors.For(this, InspectorIdentity);
    string ValueFor(string key) => InspectorDescriptors.ValueFor(this, key);
    public void RecordInspectorCommit(string key, bool succeeded) { _inspectorRecent.RecordCommit(InspectorIdentity, key, succeeded); RefreshInspectorNavigation(); }
}
