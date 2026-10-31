using Avalonia;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public sealed class XYDiagnostic
{
    private XYDiagnostic() { }

    public static readonly AttachedProperty<string?> DebugIdProperty =
        AvaloniaProperty.RegisterAttached<XYDiagnostic, Control, string?>("DebugId");

    public static readonly AttachedProperty<string?> AreaIdProperty =
        AvaloniaProperty.RegisterAttached<XYDiagnostic, Control, string?>("AreaId");

    public static string? GetDebugId(Control target) => target.GetValue(DebugIdProperty);
    public static void SetDebugId(Control target, string? value) => target.SetValue(DebugIdProperty, value);
    public static string? GetAreaId(Control target) => target.GetValue(AreaIdProperty);
    public static void SetAreaId(Control target, string? value) => target.SetValue(AreaIdProperty, value);
}
