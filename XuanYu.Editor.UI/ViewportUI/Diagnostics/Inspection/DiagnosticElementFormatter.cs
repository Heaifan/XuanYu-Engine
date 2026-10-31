using System.Globalization;

namespace XuanYu.Editor.UI;

public static class DiagnosticElementFormatter
{
    public static string Format(DiagnosticProbeResult result) => string.Join(Environment.NewLine + Environment.NewLine,
        "[XYengine Element Diagnostic]",
        $"DebugId: {result.DebugId}",
        $"ParentDebugId: {result.ParentDebugId}",
        $"ControlType: {result.ControlType}",
        $"Name: {result.Name}",
        $"Text: {result.Text}",
        $"RuntimeLocator: {result.RuntimeLocator}",
        $"Visible: {result.Visible}",
        $"Enabled: {result.Enabled}",
        $"Bounds: {Bounds(result.Bounds)}",
        $"ProbeMode: {result.ProbeMode}");

    static string Bounds(Avalonia.Rect? bounds) => bounds is not { } value ? "N/A" :
        $"X={Number(value.X)} Y={Number(value.Y)} W={Number(value.Width)} H={Number(value.Height)}";

    static string Number(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
