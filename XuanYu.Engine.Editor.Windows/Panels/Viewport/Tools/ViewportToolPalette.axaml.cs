using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using AM = Avalonia.Media;

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
            ? new SolidColorBrush(AM.Color.Parse("#4A7FEF"))
            : new SolidColorBrush(AM.Color.Parse("#353B44"));
        var moveBg = _activeTool == ViewportEditorTool.Move
            ? new SolidColorBrush(AM.Color.Parse("#4A7FEF"))
            : new SolidColorBrush(AM.Color.Parse("#353B44"));

        SelectButton!.Background = selBg;
        MoveButton!.Background = moveBg;
    }
}
