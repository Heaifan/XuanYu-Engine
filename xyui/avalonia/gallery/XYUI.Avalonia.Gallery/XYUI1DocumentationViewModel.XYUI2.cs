namespace XYUI.Avalonia.Gallery;

// XYUI-2 区块：Batch 01 导航、选中路由与默认落点（复用既有文档视图与记录类型）。
public sealed partial class XYUI1DocumentationViewModel
{
    public IReadOnlyList<XYUI1NavigationItem> XYUI2Items { get; private set; } = [];
    XYUI1NavigationItem? _selectedXYUI2;

    public XYUI1NavigationItem? SelectedXYUI2Item
    {
        get => _selectedXYUI2;
        set
        {
            if (value == _selectedXYUI2) return;
            _selectedXYUI2 = value;
            _selectedItem = null!;
            SelectedDocument = value?.Document is null
                ? new Views.XYUI2ModuleOverviewView { DataContext = this }
                : new Views.XYUI1ComponentDocumentView { DataContext = value.Document };
            PropertyChanged?.Invoke(this, new(nameof(SelectedXYUI2Item)));
            PropertyChanged?.Invoke(this, new(nameof(SelectedItem)));
            PropertyChanged?.Invoke(this, new(nameof(SelectedDocument)));
        }
    }

    internal void BootstrapXYUI2()
    {
        var items = XYUI2DocumentationCatalog.Build()
            .Select(x => new XYUI1NavigationItem(x.Id, x.ChineseName, x.EnglishName, x)).ToArray();
        var overview = new XYUI1NavigationItem("XYUI-2", "模块概览", "Buttons & Inputs", null);
        XYUI2Items = new[] { overview }.Concat(items).ToArray();
        SelectedXYUI2Item = items.FirstOrDefault();
    }

    internal void SelectXYUI2(string id)
    {
        var item = XYUI2Items.FirstOrDefault(x => x.Id == id);
        if (item is not null) SelectedXYUI2Item = item;
    }
}
