using Avalonia.Controls;
using Avalonia.Interactivity;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class ContextToolBar : UserControl
{
    XYSubMenu? _drawSubMenu;
    public ContextToolBar()
    {
        InitializeComponent();
        DrawSplitButton.MainCommand = new RelayCommand(_ => RunMainDrawAction());
        DrawSplitButton.MenuCommand = new RelayCommand(_ => OpenDrawingMenu());
        BuildDrawingMenu();
    }
    void BuildDrawingMenu()
    {
        var menu = new XYMenu();
        menu.Items = [Category("点", "地图标记"), Category("线", "道路"), Category("面", "区域面")];
        DrawMenu.Items = menu.Items;
    }
    XYMenuItem Category(string label, string value)
    {
        var item = new XYMenuItem { Label = label, HasSubMenu = true };
        item.SubMenuRequested += (_, _) => OpenDrawingSubMenu(label, value);
        return item;
    }
    void OpenDrawingSubMenu(string label, string value)
    {
        var item = new XYMenuItem { Label = label };
        item.Invoked += (_, _) => _ = (DataContext as UiVm)?.BeginContextDrawingAsync(value);
        var parent = new XYMenu(new XYMenuItem { Label = label, HasSubMenu = true });
        var child = new XYMenu(item);
        _drawSubMenu?.Close();
        _drawSubMenu = new XYSubMenu { ParentMenu = parent, ChildMenu = child };
        _drawSubMenu.Close(); DrawMenuPopup.Child = _drawSubMenu;
        _drawSubMenu.Open(); child.Open();
    }
    void RunMainDrawAction() { if ((DataContext as UiVm)?.LastDrawTool is null) OpenDrawingMenu(); else _ = (DataContext as UiVm)?.BeginLastDrawToolAsync(); }
    void OpenDrawingMenu() { _drawSubMenu?.Close(); DrawMenuPopup.Child = DrawMenu; DrawMenuPopup.PlacementTarget = DrawSplitButton; DrawMenuPopup.IsOpen = true; DrawMenu.Open(); }
    void UndoDrawingVertex_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.UndoRoadDrawingVertex(); else vm.UndoRegionDrawingVertex(); } }
    void CompleteDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.CompleteRoadDrawing(); else vm.CompleteRegionDrawing(); } }
    void CancelDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.CancelRoadDrawing(); else vm.CancelRegionDrawing(); } }
}
