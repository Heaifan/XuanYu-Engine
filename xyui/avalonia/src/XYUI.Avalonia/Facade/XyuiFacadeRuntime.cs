using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Facade;

internal static class XyuiFacadeRuntime
{
    internal static IBrush Brush(string name) =>
        XyuiColorTokens.TryFind(name, out var token) ? new SolidColorBrush(token.ToColor(false)) : Brushes.Transparent;

    internal static void Unsupported(string property, AvaloniaObject target, string supported) =>
        Trace.TraceWarning($"XY.{property} does not support {target.GetType().Name}. Supported targets: {supported}.");

    internal static void Foreground(AvaloniaObject target, string? name)
    {
        XyuiFacadeConflictDiagnostics.Check(target);
        if (name is null || !XyuiFacadeResolver.TryColor(name, out _)) return;
        var brush = Brush(name);
        if (target is XYIcon icon) { icon.Stroke = brush; icon.Fill = brush; }
        else if (target is TemplatedControl templated) templated.Foreground = brush;
        else if (target is TextBlock text) text.Foreground = brush;
        else Unsupported("Foreground", target, "TextBlock, TemplatedControl, XYIcon");
    }

    internal static void Surface(AvaloniaObject target, string? name)
    {
        XyuiFacadeConflictDiagnostics.Check(target);
        if (name is null || !XyuiFacadeResolver.TrySurface(name, out _)) return;
        var brush = Brush(name);
        if (target is global::Avalonia.Controls.Border border) border.Background = brush;
        else if (target is TemplatedControl templated) templated.Background = brush;
        else Unsupported("Surface", target, "Border, TemplatedControl");
    }

    internal static void Font(AvaloniaObject target, string? name)
    {
        XyuiFacadeConflictDiagnostics.Check(target);
        if (name is null || !XyuiFacadeResolver.TryFont(name, out var font)) return;
        if (target is TextBlock text) text.FontFamily = font;
        else if (target is TemplatedControl templated) templated.FontFamily = font;
        else Unsupported("Font", target, "TextBlock, TemplatedControl");
    }

    internal static void Typography(AvaloniaObject target, string? name)
    {
        XyuiFacadeConflictDiagnostics.Check(target);
        if (name is null || !XyuiFacadeResolver.TryTypography(name, out var role)) return;
        if (target is TextBlock text) { text.FontFamily = new FontFamily(role.Font); text.FontSize = role.Size; text.LineHeight = role.LineHeight; text.FontWeight = FontWeight(role.Weight); }
        else if (target is TemplatedControl control) { control.FontFamily = new FontFamily(role.Font); control.FontSize = role.Size; control.FontWeight = FontWeight(role.Weight); }
        else Unsupported("Typography", target, "TextBlock, TemplatedControl");
    }

    internal static void Padding(AvaloniaObject target, string? name)
    {
        XyuiFacadeConflictDiagnostics.Check(target);
        if (name is null || !XyuiFacadeResolver.TrySpace(name, out var value)) return;
        if (target is Border border) border.Padding = new Thickness(value);
        else if (target is TemplatedControl templated) templated.Padding = new Thickness(value);
        else Unsupported("Padding", target, "Border, TemplatedControl");
    }

    internal static void Gap(AvaloniaObject target, string? name)
    {
        XyuiFacadeConflictDiagnostics.Check(target);
        if (name is null || !XyuiFacadeResolver.TrySpace(name, out var value)) return;
        if (target is StackPanel panel) panel.Spacing = value;
        else Unsupported("Gap", target, "StackPanel");
    }

    internal static void Margin(AvaloniaObject target, string? name)
    {
        XyuiFacadeConflictDiagnostics.Check(target);
        if (name is null || !XyuiFacadeResolver.TrySpace(name, out var value)) return;
        if (target is Control control) control.Margin = new Thickness(value);
        else Unsupported("Margin", target, "Control");
    }

    internal static void Radius(AvaloniaObject target, string? name)
    {
        XyuiFacadeConflictDiagnostics.Check(target);
        if (name is null || !XyuiFacadeResolver.TryRadius(name, out var value)) return;
        if (target is Border border) border.CornerRadius = value;
        else if (target is TemplatedControl templated) templated.CornerRadius = value;
        else Unsupported("Radius", target, "Border, TemplatedControl");
    }

    internal static void Border(AvaloniaObject target, string? name)
    {
        XyuiFacadeConflictDiagnostics.Check(target);
        if (name is null || !XyuiFacadeResolver.TryBorder(name, out var value)) return;
        if (target is global::Avalonia.Controls.Border border) { border.BorderBrush = Brush(value.Brush.Replace("XY.Brush.", "XY.")); border.BorderThickness = new Thickness(value.Width); }
        else Unsupported("Border", target, "Border");
    }
    static global::Avalonia.Media.FontWeight FontWeight(int value) => value switch { 500 => global::Avalonia.Media.FontWeight.Medium, 600 => global::Avalonia.Media.FontWeight.SemiBold, 700 => global::Avalonia.Media.FontWeight.Bold, _ => global::Avalonia.Media.FontWeight.Normal };
}
