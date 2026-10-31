using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class Left : UserControl
{
    public Left()
    {
        InitializeComponent();
        SelectContentTab("project");
    }

    void ContentTabs_SelectionChanged(object? sender, XYTab tab) => SelectContentTab(tab.Id);

    void SelectContentTab(string id)
    {
        var project = id != "file";
        ContentTabs.SelectedTabId = project ? "project" : "file";
        ProjectWorkspace.IsVisible = project;
        FileWorkspace.IsVisible = !project;
    }
}
