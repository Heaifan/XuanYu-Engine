using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace XuanYu.Engine.Editor.Windows.Panels.Viewport.Tools;

public sealed partial class ViewportToolPalette : UserControl
{
    private ViewportEditorTool _activeTool = ViewportEditorTool.Select;

    public ViewportEditorTool ActiveTool => _activeTool;

    public event Action<ViewportEditorTool>? ToolChanged;

    public ViewportToolPalette()
    {
        InitializeComponent();
        UpdateVisualState();
    }

    public void SetActiveTool(ViewportEditorTool tool)
    {
        if (_activeTool == tool) return;
        _activeTool = tool;
        UpdateVisualState();
        ToolChanged?.Invoke(tool);
    }

    private void OnSelectClicked(object? sender, RoutedEventArgs e) => SetActiveTool(ViewportEditorTool.Select);

    private void OnMoveClicked(object? sender, RoutedEventArgs e) => SetActiveTool(ViewportEditorTool.Move);

    private void UpdateVisualState()
    {
        var selBg = _activeTool == ViewportEditorTool.Select
            ? ThemeBrush("BrushEditorButtonSelectedBackground")
            : ThemeBrush("BrushEditorButtonBackground");
        var moveBg = _activeTool == ViewportEditorTool.Move
            ? ThemeBrush("BrushEditorButtonSelectedBackground")
            : ThemeBrush("BrushEditorButtonBackground");

        SelectButton!.Background = selBg;
        MoveButton!.Background = moveBg;
    }

    private IBrush ThemeBrush(string key)
    {
        var value = this.FindResource(key);
        if (value is IBrush brush) return brush;

        return key == "BrushEditorButtonSelectedBackground"
            ? new SolidColorBrush(Color.Parse("#477EB8"))
            : new SolidColorBrush(Color.Parse("#394652"));
    }

}
