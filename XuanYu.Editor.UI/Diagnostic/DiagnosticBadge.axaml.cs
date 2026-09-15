using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class DiagnosticBadge : UserControl
{
    public DiagnosticBadge() => InitializeComponent();

    public DiagnosticBadge(Control target, IDiagnosticClipboard? clipboard = null) : this()
    {
        var id = XYDiagnostic.GetDebugId(target) ?? "N/A";
        BadgeText.Text = id;
        ToolTip.SetTip(BadgeSurface, id);
    }
}
