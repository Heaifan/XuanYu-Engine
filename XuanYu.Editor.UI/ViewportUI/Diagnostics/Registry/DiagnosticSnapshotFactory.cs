using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public static partial class DiagnosticSnapshotFactory
{
    const string Missing = "N/A";

    public static DiagnosticSnapshot Capture(Control target)
    {
        var topLevel = TopLevel.GetTopLevel(target);
        var vm = target.DataContext as UiVm;
        var origin = topLevel is null ? null : target.TranslatePoint(default, topLevel);
        Rect? bounds = origin is { } point ? new Rect(point, target.Bounds.Size) : null;
        var selection = vm?.SelectedMapGeometry;
        return new DiagnosticSnapshot(
            XYDiagnostic.GetDebugId(target) ?? Missing,
            FindAreaId(target) ?? Missing,
            Version(vm) ?? Missing,
            vm?.InspectorIdentity.ToString() ?? Missing,
            vm is not null && selection is { } selected
                ? $"{vm.InspectorIdentity}:{selected.FeatureId}" : Missing,
            target.IsEffectivelyVisible,
            target.IsEffectivelyEnabled,
            bounds,
            topLevel?.ClientSize,
            topLevel?.RenderScaling,
            topLevel?.ActualThemeVariant.ToString() ?? Missing);
    }

    static string? FindAreaId(Control target)
    {
        for (Visual? current = target; current is not null; current = current.GetVisualParent())
            if (current is Control control && XYDiagnostic.GetAreaId(control) is { } id)
                return id;
        return null;
    }

    static string? Version(UiVm? vm) => vm is null ? null :
        VersionPattern().Match(vm.DocumentWindowTitle) is { Success: true } match ? match.Value : null;

    [GeneratedRegex(@"\bv\S+?-rz\b", RegexOptions.CultureInvariant)]
    private static partial Regex VersionPattern();
}
