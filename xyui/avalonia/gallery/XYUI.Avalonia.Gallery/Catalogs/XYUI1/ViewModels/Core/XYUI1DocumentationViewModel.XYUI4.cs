namespace XYUI.Avalonia.Gallery;

public sealed partial class XYUI1DocumentationViewModel
{
    public IReadOnlyList<XYUI1NavigationItem> XYUI4Items { get; private set; } = [];
    public string XYUI4CountText => $"{XYUI4Items.Count(x => x.Document is not null)}/{XYUI4Items.Count(x => x.Document is not null)}";
    bool _isX4;
    public bool IsXYUI4Expanded { get => _isX4; set { if (_isX4 == value) return; _isX4 = value; PropertyChanged?.Invoke(this, new(nameof(IsXYUI4Expanded))); } }
    XYUI1NavigationItem? _selectedXYUI4;

    public XYUI1NavigationItem? SelectedXYUI4Item
    {
        get => _selectedXYUI4;
        set
        {
            if (value == _selectedXYUI4) return;
            _selectedXYUI4 = value; _selectedItem = null!; _selectedXYUI2 = null; _selectedXYUI3 = null;
            if (value?.Document is not null) SelectedDocument = new Views.XYUI1ComponentDocumentView { DataContext = value.Document };
            PropertyChanged?.Invoke(this, new(nameof(SelectedXYUI4Item))); PropertyChanged?.Invoke(this, new(nameof(SelectedItem)));
            PropertyChanged?.Invoke(this, new(nameof(SelectedXYUI2Item))); PropertyChanged?.Invoke(this, new(nameof(SelectedXYUI3Item))); PropertyChanged?.Invoke(this, new(nameof(SelectedDocument)));
        }
    }

    internal void BootstrapXYUI4()
    {
        XYUI4Items = XYUI4DocumentationCatalog.Build().Select(x => new XYUI1NavigationItem(x.Id, x.ChineseName, x.EnglishName, x)).ToArray();
        PropertyChanged?.Invoke(this, new(nameof(XYUI4Items))); PropertyChanged?.Invoke(this, new(nameof(XYUI4CountText)));
    }

    internal void SelectXYUI4(string id)
    {
        IsXYUI1Expanded = false; IsXYUI2Expanded = false; IsXYUI3Expanded = false; IsXYUI4Expanded = true;
        var item = XYUI4Items.FirstOrDefault(x => x.Id == id); if (item is not null) SelectedXYUI4Item = item;
    }
}
