using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

public partial class DiagnosticBadge : UserControl
{
    Control? _target;
    IDiagnosticClipboard? _clipboard;
    public DiagnosticBadge() => InitializeComponent();

    public DiagnosticBadge(Control target, IDiagnosticClipboard? clipboard = null) : this()
    {
        _target = target; _clipboard = clipboard;
        var id = XYDiagnostic.GetDebugId(target) ?? "N/A";
        BadgeText.Text = id;
        ToolTip.SetTip(BadgeSurface, id);
        BadgeSurface.PointerPressed += OnPressed;
    }

    async void OnPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_target is null || _clipboard is null) return;
        var point = e.GetCurrentPoint(BadgeSurface);
        var text = point.Properties.IsLeftButtonPressed && e.KeyModifiers.HasFlag(KeyModifiers.Shift)
            ? DiagnosticSnapshotFactory.Capture(_target).ToString()
            : XYDiagnostic.GetDebugId(_target) ?? "N/A";
        await _clipboard.SetTextAsync(_target, text);
        e.Handled = true;
    }
}
