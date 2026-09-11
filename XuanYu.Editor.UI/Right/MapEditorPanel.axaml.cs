using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class MapEditorPanel : UserControl
{
    public event Action<string>? TabChanged;
    public string SelectedTabId => MapPager.SelectedId ?? "base";

    public MapEditorPanel()
    {
        InitializeComponent();
        SelectTab(MapPager.SelectedId ?? "base");
    }

    void MapPager_SelectionChanged(object? sender, XYPagerPage page) => TabChanged?.Invoke(page.Id);

    public void SelectTab(string id)
    {
        id = id is "environment" or "display" or "data" or "advanced" ? id : "base";
        if (SelectedTabId != id) MapPager.Select(id);
    }
}
