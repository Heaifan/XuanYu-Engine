using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public static partial class DiagnosticProbeResolver
{
    static (Visual Control, int Rank) Candidate(Visual visual)
    {
        if (HasDebugId(visual) && visual is Control debug && IsInteractive(debug)) return (visual, 0);
        if (DiagnosticXyuiResolver.IsMapped(visual)) return (visual, 1);
        if (visual is Control control && !string.IsNullOrEmpty(NameValue(control)) && IsInteractive(control)) return (visual, 2);
        if (visual is Control known && IsInteractive(known)) return (visual, 3);
        if (visual is Control custom && custom.GetType().Namespace?.StartsWith("XuanYu", StringComparison.Ordinal) == true) return (visual, 3);
        if (visual is Control named && !string.IsNullOrEmpty(NameValue(named))) return (visual, 4);
        if (IsTemplateInternal(visual)) return (visual, 6);
        return (visual, 5);
    }

    static bool IsInteractive(Control control) => control is Button or ToggleButton or MenuItem or TextBox or
        ComboBox or TreeViewItem or TabItem || control.GetType().Namespace?.StartsWith("XYUI", StringComparison.Ordinal) == true;

    static bool IsTemplateInternal(Visual visual)
    {
        if (visual is Control control && control.Name?.StartsWith("PART_", StringComparison.Ordinal) == true)
            return true;
        return visual is ContentPresenter or ItemsPresenter or Border or Panel or Decorator or TextBlock;
    }
}
