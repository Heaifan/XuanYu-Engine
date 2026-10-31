using Avalonia.Controls;
using Avalonia.Interactivity;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class ContextToolBar : UserControl
{
    public ContextToolBar()
    {
        InitializeComponent();
        var board = new XYContextDropdownBoard("上下文", [new("point", "点"), new("line", "线"), new("area", "区域"), new("terrain", "地形")], new Dictionary<string, IReadOnlyList<XYContextAction>> { ["point"] = [new("marker", "点标记")], ["line"] = [new("road", "道路")], ["area"] = [new("region", "区域")], ["terrain"] = [] }, new HashSet<string> { "terrain" });
        ContextToolbarDiagnosticBridge.Attach(board);
        board.CategoryList.SelectionChanged += (_, category) =>
        {
            if (DataContext is not UiVm vm) return;
            if (category.Id == "terrain") vm.EnterTerrainContext();
            if (category.Id == "area") vm.EnterRegionContext();
        };
        DrawBoardHost.Children.Add(board); board.AttachTrigger(DrawSplitButton); board.ActionExecuted += OnDrawActionExecuted;
        DrawSplitButton.MainCommand = new RelayCommand(_ => RunMainDrawAction());
        DrawSplitButton.MenuCommand = new RelayCommand(_ => ToggleContextBoard(board));
    }
    async void OnDrawActionExecuted(object? sender, XYContextAction action) { if (DataContext is not UiVm vm) return; var tool = action.Id switch { "marker" => "地图标记", "road" => "道路", "region" => "区域面", _ => null }; if (tool is null) return; if (sender is XYContextDropdownBoard board) board.Close(); await vm.BeginContextDrawingAsync(tool); }
    void RunMainDrawAction() { var vm = DataContext as UiVm; if (vm?.IsDrawingTransactionActive == true) return; if (vm?.IsTerrainContext == true || vm?.LastDrawTool is null) OpenDrawingMenu(); else _ = vm.BeginLastDrawToolAsync(); }
    void ToggleContextBoard(XYContextDropdownBoard board) { var vm = DataContext as UiVm; if (vm?.CanOpenContextSelector != true) return; board.SelectCategory(vm.IsTerrainContext ? "terrain" : "area"); board.Toggle(DrawSplitButton); }
    void OpenDrawingMenu() => DrawSplitButton.MenuCommand?.Execute(null);
    void UndoDrawingVertex_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingTool) vm.UndoRoadDrawingVertex(); else vm.UndoRegionDrawingVertex(); } }
    void CompleteDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingTool) vm.CompleteRoadDrawing(); else vm.CompleteRegionDrawing(); } }
    void CancelDrawing_Click(object? s, RoutedEventArgs e) { if (DataContext is UiVm vm) { if (vm.IsRoadDrawingTool) vm.CancelRoadDrawing(); else vm.CancelRegionDrawing(); } }
}
