using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

public partial class DiagnosticBadge : UserControl
{
    Control? _target;
    IDiagnosticClipboard _clipboard = new DiagnosticClipboard();

    public DiagnosticBadge() => InitializeComponent();

    public DiagnosticBadge(Control target, IDiagnosticClipboard? clipboard = null) : this()
    {
        _target = target;
        _clipboard = clipboard ?? _clipboard;
        var id = XYDiagnostic.GetDebugId(target) ?? "N/A";
        BadgeText.Text = id;
        ToolTip.SetTip(BadgeSurface, $"{id}；单击复制 ID，Shift+单击复制诊断快照");
    }

    async void BadgeSurface_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_target is null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        e.Handled = true;
        var text = e.KeyModifiers.HasFlag(KeyModifiers.Shift)
            ? DiagnosticSnapshotFactory.Capture(_target).ToString()
            : XYDiagnostic.GetDebugId(_target) ?? "N/A";
        await _clipboard.SetTextAsync(_target, text);
    }
}
