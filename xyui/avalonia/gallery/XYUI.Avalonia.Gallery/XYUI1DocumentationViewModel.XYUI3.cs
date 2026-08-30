namespace XYUI.Avalonia.Gallery;

public sealed partial class XYUI1DocumentationViewModel
{
    public IReadOnlyList<XYUI1NavigationItem> XYUI3Items { get; private set; } = [];
    public string XYUI3CountText => "8/8";
    bool _isX3 = true;
    public bool IsXYUI3Expanded { get => _isX3; set { if (_isX3 == value) return; _isX3 = value; PropertyChanged?.Invoke(this, new(nameof(IsXYUI3Expanded))); } }
    XYUI1NavigationItem? _selectedXYUI3;
    public XYUI1NavigationItem? SelectedXYUI3Item
    {
        get => _selectedXYUI3;
        set
        {
            if (value == _selectedXYUI3) return;
            _selectedXYUI3 = value; _selectedItem = null!; _selectedXYUI2 = null;
            if (value?.Document is not null) SelectedDocument = new Views.XYUI1ComponentDocumentView { DataContext = value.Document };
            PropertyChanged?.Invoke(this, new(nameof(SelectedXYUI3Item))); PropertyChanged?.Invoke(this, new(nameof(SelectedDocument)));
        }
    }
    internal void BootstrapXYUI3()
    {
        XYUI3Items = XYUI3DocumentationCatalog.Build().Select(x => new XYUI1NavigationItem(x.Id, x.ChineseName, x.EnglishName, x)).ToArray();
        SelectedXYUI3Item = XYUI3Items.FirstOrDefault();
    }
    internal void SelectXYUI3(string id)
    {
        var item = XYUI3Items.FirstOrDefault(x => x.Id == id); if (item is not null) SelectedXYUI3Item = item;
    }
}
