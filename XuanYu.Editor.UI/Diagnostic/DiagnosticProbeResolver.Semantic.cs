using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public static partial class DiagnosticProbeResolver
{
    static bool IsTemplateInternal(Visual visual)
    {
        if (visual is Control control && control.Name?.StartsWith("PART_", StringComparison.Ordinal) == true)
            return true;
        return visual is ContentPresenter or ItemsPresenter or Border or Panel or Decorator or TextBlock;
    }
}
