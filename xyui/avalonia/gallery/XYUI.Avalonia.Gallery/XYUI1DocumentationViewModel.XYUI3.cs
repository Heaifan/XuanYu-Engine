namespace XYUI.Avalonia.Gallery;

public sealed partial class XYUI1DocumentationViewModel
{
    public IReadOnlyList<XYUI1NavigationItem> XYUI3Items { get; private set; } = [];
    public string XYUI3CountText => $"{XYUI3Items.Count(x => x.Document is not null)}/{XYUI3Items.Count(x => x.Document is not null)}";
    bool _isX3 = true;
    public bool IsXYUI3Expanded { get => _isX3; set { if (_isX3 == value) return; _isX3 = value; PropertyChanged?.Invoke(this, new(nameof(IsXYUI3Expanded))); } }
    XYUI1NavigationItem? _selectedXYUI3;
    public XYUI1NavigationItem? SelectedXYUI3Item
    {
        get => _selectedXYUI3;
        set
        {
            if (value == _selectedXYUI3) return;
            _selectedXYUI3 = value; _selectedItem = null!; _selectedXYUI2 = null; _selectedFoundation = null;
            if (value is not null)
            {
                SelectedDocument = value.Document is null
                    ? new Views.XYUI3ModuleOverviewView { DataContext = this }
                    : (value.Document.HasLiveExamples
                        ? new Views.XYUI3ComponentDocumentView { DataContext = value.Document }
                        : new Views.XYUI1ComponentDocumentView { DataContext = value.Document });
            }
            PropertyChanged?.Invoke(this, new(nameof(SelectedXYUI3Item))); PropertyChanged?.Invoke(this, new(nameof(SelectedItem)));
            PropertyChanged?.Invoke(this, new(nameof(SelectedXYUI2Item))); PropertyChanged?.Invoke(this, new(nameof(SelectedFoundation)));
            PropertyChanged?.Invoke(this, new(nameof(SelectedDocument)));
        }
    }
    internal void BootstrapXYUI3()
    {
        var items = XYUI3DocumentationCatalog.Build().Select(x => new XYUI1NavigationItem(x.Id, x.ChineseName, x.EnglishName, x)).ToArray();
        var overview = new XYUI1NavigationItem("XYUI-3", "模块概览", "Navigation & Switching", null);
        var context = new XYUI1NavigationItem(XYUI3GalleryCatalog.ContextToolbarId, "Context Toolbar · 上下文工具栏", "A + A3 + A4", XYUI3DocumentationCatalog.ContextToolbarDocument());
        XYUI3Items = new[] { overview }.Concat(items).Append(context).ToArray();
        // 遵循 Catalog 契约：默认落点跟随当前清单末项（3.24）。
        SelectedXYUI3Item = context;
    }
    internal void SelectXYUI3(string id)
    {
        var item = XYUI3Items.FirstOrDefault(x => x.Id == id); if (item is not null) SelectedXYUI3Item = item;
    }
}
