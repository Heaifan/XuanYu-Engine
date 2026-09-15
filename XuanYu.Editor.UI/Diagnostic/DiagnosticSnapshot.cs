using System.Globalization;
using Avalonia;

namespace XuanYu.Editor.UI;

public sealed record DiagnosticSnapshot(
    string? DebugId,
    string? AreaId,
    string? Version,
    string? SelectionType,
    string? EntityId,
    bool Visible,
    bool Enabled,
    Rect? Bounds,
    Size? Window,
    double? RenderScale,
    string? Theme)
{
    const string Missing = "N/A";

    public override string ToString()
    {
        var lines = new[]
        {
            "[XYengine Diagnostic]",
            $"DebugId: {Value(DebugId)}",
            $"AreaId: {Value(AreaId)}",
            $"Version: {Value(Version)}",
            $"SelectionType: {Value(SelectionType)}",
            $"EntityId: {Value(EntityId)}",
            $"Visible: {Visible}",
            $"Enabled: {Enabled}",
            $"Bounds: {BoundsText()}",
            $"Window: {WindowText()}",
            $"RenderScale: {Number(RenderScale)}",
            $"Theme: {Value(Theme)}",
        };
        return string.Join(Environment.NewLine + Environment.NewLine, lines);
    }

    string BoundsText() => Bounds is not { } value ? Missing :
        $"X={Number(value.X)} Y={Number(value.Y)} W={Number(value.Width)} H={Number(value.Height)}";
    string WindowText() => Window is not { } value ? Missing :
        $"{Number(value.Width)}x{Number(value.Height)}";
    static string Value(string? value) => string.IsNullOrEmpty(value) ? Missing : value;
    static string Number(double? value) => value?.ToString("0.##", CultureInfo.InvariantCulture) ?? Missing;
}
