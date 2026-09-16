using Avalonia.Controls;
using Avalonia.Interactivity;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class ContextToolBar : UserControl
{
    public ContextToolBar()
    {
        InitializeComponent();
        DrawSplitButton.MainCommand = new RelayCommand(_ => RunMainDrawAction());
        DrawSplitButton.MenuCommand = new RelayCommand(_ => OpenDrawingMenu());
        BuildDrawingMenu();
    }
    void BuildDrawingMenu()
    {
        var models = new[]
        {
            new XYMenuItemModel("point", "点", Children: [new("marker", "点标记")]),
            new XYMenuItemModel("line", "线", Children: [new("road", "道路")]),
            new XYMenuItemModel("surface", "面", Children: [new("region", "区域")])
        };
        DrawMenu = XYMenu.FromModels(models);
        BindLeafCommands(DrawMenu);
        DrawMenuPopup.Child = DrawMenu;
    }
    void BindLeafCommands(XYMenu menu)
    {
        foreach (var item in menu.Items.OfType<XYMenuItem>())
        {
            if (item.SubMenu is { } submenu) BindLeafCommands(submenu.ChildMenu);
            else item.Command = new RelayCommand(_ => _ = BeginLeafAsync(item.Id));
        }
    }
    async Task BeginLeafAsync(string id)
    {
        var tool = id switch { "marker" => "地图标记", "road" => "道路", "region" => "区域面", _ => null };
        if (tool is not null && DataContext is UiVm vm) await vm.BeginContextDrawingAsync(tool);
    }
    void RunMainDrawAction() { if ((DataContext as UiVm)?.LastDrawTool is null) OpenDrawingMenu(); else _ = (DataContext as UiVm)?.BeginLastDrawToolAsync(); }
    void OpenDrawingMenu() { DrawMenuPopup.PlacementTarget = DrawSplitButton; DrawMenuPopup.IsOpen = true; DrawMenu.Open(); }
    void UndoDrawingVertex_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.UndoRoadDrawingVertex(); else vm.UndoRegionDrawingVertex(); } }
    void CompleteDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.CompleteRoadDrawing(); else vm.CompleteRegionDrawing(); } }
    void CancelDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.CancelRoadDrawing(); else vm.CancelRegionDrawing(); } }
}
