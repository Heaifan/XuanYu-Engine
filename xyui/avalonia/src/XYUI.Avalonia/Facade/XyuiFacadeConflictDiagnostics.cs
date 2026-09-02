using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace XYUI.Avalonia.Facade;

internal static class XyuiFacadeConflictDiagnostics
{
    internal static void Check(AvaloniaObject target)
    {
        if (target is TextBlock text && text.IsSet(TextBlock.ForegroundProperty)) Warn("Foreground", target);
        if (target is TemplatedControl control && control.IsSet(TemplatedControl.ForegroundProperty)) Warn("Foreground", target);
        if (target is Border border && border.IsSet(Border.BackgroundProperty)) Warn("Surface", target);
        if (target is TemplatedControl surface && surface.IsSet(TemplatedControl.BackgroundProperty)) Warn("Surface", target);
        if (target is TextBlock font && font.IsSet(TextBlock.FontFamilyProperty)) Warn("Font/Typography", target);
        if (target is TemplatedControl type && type.IsSet(TemplatedControl.FontFamilyProperty)) Warn("Font/Typography", target);
        if (target is Border padding && padding.IsSet(Border.PaddingProperty)) Warn("Padding", target);
        if (target is TemplatedControl inset && inset.IsSet(TemplatedControl.PaddingProperty)) Warn("Padding", target);
        if (target is Control margin && margin.IsSet(Control.MarginProperty)) Warn("Margin", target);
        if (target is Border radius && radius.IsSet(Border.CornerRadiusProperty)) Warn("Radius", target);
        if (target is TemplatedControl shape && shape.IsSet(TemplatedControl.CornerRadiusProperty)) Warn("Radius", target);
        if (target is Border edge && (edge.IsSet(Border.BorderBrushProperty) || edge.IsSet(Border.BorderThicknessProperty))) Warn("Border", target);
    }

    static void Warn(string facade, AvaloniaObject target) =>
        Trace.TraceWarning($"XY.{facade} and a native property are both explicitly set on {target.GetType().Name}; native value may be overwritten.");
}
