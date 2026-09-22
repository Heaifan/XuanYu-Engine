using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public sealed record DiagnosticElementSnapshot(
    DiagnosticProbeResult Probe,
    DiagnosticXyuiIdentity Identity,
    string ComponentType,
    string VisualPath,
    string DebugId,
    string InstanceName,
    string Text,
    string ActualSize,
    string DesiredSize,
    string Position,
    string Margin,
    string Padding,
    string HorizontalAlignment,
    string VerticalAlignment,
    string Visible,
    string Enabled,
    string Focused,
    string PointerOver,
    string Pressed,
    string Selected,
    string Expanded,
    string PseudoClasses)
{
    public static DiagnosticElementSnapshot Capture(DiagnosticProbeResult result)
    {
        var target = result.SemanticTarget ?? result.DeepVisual;
        var control = target as Control;
        var top = control is null ? null : TopLevel.GetTopLevel(control);
        var position = control is not null && top is not null
            ? control.TranslatePoint(default, top)?.ToString() ?? "不可用" : "不可用";
        return new(result, DiagnosticXyuiResolver.Resolve(target), target.GetType().Name,
            Path(target), result.DebugId, result.Name, result.Text, Size(target.Bounds.Size),
            Size(control?.DesiredSize ?? target.Bounds.Size), position,
            control is null ? "不可用" : control.Margin.ToString(), GetPadding(control),
            control?.HorizontalAlignment.ToString() ?? "不可用", control?.VerticalAlignment.ToString() ?? "不可用",
            Bool(target.IsEffectivelyVisible), Bool(control?.IsEffectivelyEnabled), Bool(control?.IsFocused),
            Bool(control?.IsPointerOver), GetPressed(control), GetSelected(control), GetExpanded(control),
            "不可用");
    }

    static string Path(Visual target) => string.Join(Environment.NewLine + "> ", Chain(target).Reverse().Select(TypeOrName));
    static IEnumerable<Visual> Chain(Visual target) { for (Visual? v = target; v is not null; v = v.GetVisualParent()) yield return v; }
    static string TypeOrName(Visual v) => v is Control c && !string.IsNullOrWhiteSpace(c.Name) ? c.Name : v.GetType().Name;
    static string GetPadding(Control? control) => control is TemplatedControl t ? t.Padding.ToString() : "不可用";
    static string GetPressed(Control? c) => c is Button b ? Bool(b.IsPressed) : "不适用";
    static string GetSelected(Control? c) => c is ToggleButton b ? Bool(b.IsChecked) : "不适用";
    static string GetExpanded(Control? c) => c is Expander e ? Bool(e.IsExpanded) : "不适用";
    static string Bool(bool value) => value ? "是" : "否";
    static string Bool(bool? value) => value is null ? "不适用" : Bool(value.Value);
    static string Size(Size value) => $"{value.Width.ToString("0.##", CultureInfo.InvariantCulture)} × {value.Height.ToString("0.##", CultureInfo.InvariantCulture)}";
}
