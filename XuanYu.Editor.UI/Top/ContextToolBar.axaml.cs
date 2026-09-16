using Avalonia.Controls;
using Avalonia.Interactivity;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class ContextToolBar : UserControl
{
    public ContextToolBar()
    {
        InitializeComponent();
        DrawSplitButton.MenuCommand = new RelayCommand(_ => OpenDrawingMenu());
        BuildDrawingMenu();
    }
    void BuildDrawingMenu()
    {
        var menu = new XYMenu();
        menu.Items = [Category("点", "Marker", "地图标记"), Category("线", "道路", "道路"), Category("面", "区域", "区域面")];
        DrawMenu.Items = menu.Items;
    }
    XYMenuItem Category(string label, string childLabel, string value)
    {
        var item = new XYMenuItem { Label = label, HasSubMenu = true };
        item.Invoked += (_, _) => OpenDrawingSubMenu(childLabel, value, item);
        return item;
    }
    void OpenDrawingSubMenu(string label, string value, Control trigger)
    {
        var item = new XYMenuItem { Label = label };
        item.Invoked += (_, _) => _ = (DataContext as UiVm)?.BeginContextDrawingAsync(value);
        DrawSubMenu.Items = [item]; DrawSubMenuPopup.PlacementTarget = trigger;
        DrawSubMenuPopup.IsOpen = true; DrawSubMenu.Open();
    }
    void OpenDrawingMenu() { DrawMenuPopup.PlacementTarget = DrawSplitButton; DrawMenuPopup.IsOpen = true; DrawMenu.Open(); }
    void UndoDrawingVertex_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.UndoRoadDrawingVertex(); else vm.UndoRegionDrawingVertex(); } }
    void CompleteDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.CompleteRoadDrawing(); else vm.CompleteRegionDrawing(); } }
    void CancelDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.CancelRoadDrawing(); else vm.CancelRegionDrawing(); } }
    void MapEditorBreadcrumb_Invoked(object? s, System.EventArgs e) => (DataContext as UiVm)?.SwitchWorkspaceCommand.Execute("MapEditor");
}
