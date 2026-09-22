using Avalonia.Controls;
using Avalonia.Interactivity;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class ContextToolBar : UserControl
{
    public ContextToolBar()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => SyncDiagnosticHosts();
        DrawMenuPopup.Opened += (_, _) => DrawMenuPopupHost.SetPopupOpen(true);
        DrawMenuPopup.Closed += (_, _) => ResetDrawingMenu();
        DrawSubMenuPopup.Opened += (_, _) => DrawSubMenuPopupHost.SetPopupOpen(true);
        DrawSubMenuPopup.Closed += (_, _) => DrawChildMenu.Close();
        DrawSubMenuPopup.Closed += (_, _) => DrawSubMenuPopupHost.SetPopupOpen(false);
        DrawMenuPopup.Closed += (_, _) => DrawMenuPopupHost.SetPopupOpen(false);
        DrawSplitButton.MainCommand = new RelayCommand(_ => RunMainDrawAction());
        DrawSplitButton.MenuCommand = new RelayCommand(_ => OpenDrawingMenu());
        BuildDrawingMenu();
    }
    void SyncDiagnosticHosts()
    {
        DrawMenuPopupHost.DataContext = DataContext; DrawSubMenuPopupHost.DataContext = DataContext;
        var enabled = (DataContext as UiVm)?.IsDiagnosticMode == true;
        DrawMenuPopupHost.SetDiagnosticEnabled(enabled); DrawSubMenuPopupHost.SetDiagnosticEnabled(enabled);
    }
    void BuildDrawingMenu()
    {
        var menu = new XYMenu();
        menu.Items = [Category("点", "点标记", "地图标记"), Category("线", "道路", "道路"), Category("面", "区域", "区域面")];
        DrawMenu.Items = menu.Items;
    }
    XYMenuItem Category(string label, string display, string value)
    {
        var item = new XYMenuItem { Label = label, HasSubMenu = true };
        item.SubMenuRequested += (_, _) => OpenDrawingSubMenu(item, label, display, value);
        return item;
    }
    void OpenDrawingSubMenu(Control anchor, string label, string display, string value)
    {
        SyncDiagnosticHosts();
        var item = new XYMenuItem { Label = display };
        item.Invoked += (_, _) => { DrawSubMenuPopup.IsOpen = false; _ = (DataContext as UiVm)?.BeginContextDrawingAsync(value); };
        DrawChildMenu.Items = [item];
        DrawSubMenuPopup.PlacementTarget = anchor;
        DrawSubMenuPopup.IsOpen = true;
        DrawChildMenu.Open();
    }
    void RunMainDrawAction() { if ((DataContext as UiVm)?.LastDrawTool is null) OpenDrawingMenu(); else _ = (DataContext as UiVm)?.BeginLastDrawToolAsync(); }
    void OpenDrawingMenu() { SyncDiagnosticHosts(); ResetDrawingMenu(); DrawMenuPopup.PlacementTarget = DrawSplitButton; DrawMenuPopup.IsOpen = true; DrawMenu.Open(); }
    void ResetDrawingMenu() { DrawSubMenuPopup.IsOpen = false; DrawChildMenu.Close(); DrawMenu.Close(); }
    void UndoDrawingVertex_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.UndoRoadDrawingVertex(); else vm.UndoRegionDrawingVertex(); } }
    void CompleteDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.CompleteRoadDrawing(); else vm.CompleteRegionDrawing(); } }
    void CancelDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingDraftActive) vm.CancelRoadDrawing(); else vm.CancelRegionDrawing(); } }
}
