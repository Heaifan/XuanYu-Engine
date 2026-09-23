using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public static class DiagnosticXyuiResolver
{
    static readonly IReadOnlyDictionary<string, (string Number, string Id)> Types =
        new Dictionary<string, (string, string)>(StringComparer.Ordinal)
        {
            ["XYText"] = ("XYUI1", "XYUI-1-01"),
            ["XYButton"] = ("XYUI2", "XYUI-2-01"),
            ["XYIconButton"] = ("XYUI2", "XYUI-2-02"),
            ["XYToggleButton"] = ("XYUI2", "XYUI-2-03"),
            ["XYSplitButton"] = ("XYUI2", "XYUI-2-04"),
            ["XYTextField"] = ("XYUI2", "XYUI-2-09"),
            ["XYComboBox"] = ("XYUI2", "XYUI-2-12"),
            ["XYContextToolbar"] = ("XYUI3", "XYUI-3-17"),
            ["XYMenu"] = ("XYUI3", "XYUI-3-02"),
            ["XYSubMenu"] = ("XYUI3", "XYUI-3-04")
        };

    public static DiagnosticXyuiIdentity Resolve(Visual visual)
    {
        var type = visual.GetType().Name;
        if (Types.TryGetValue(type, out var mapped))
            return new(mapped.Number, type, "XYUI", mapped.Id);
        return DiagnosticXyuiIdentity.None(Source(visual));
    }

    public static bool IsMapped(Visual visual) => Types.ContainsKey(visual.GetType().Name);

    static string Source(Visual visual)
    {
        var ns = visual.GetType().Namespace ?? string.Empty;
        if (ns.StartsWith("Avalonia", StringComparison.Ordinal)) return "Avalonia 原生";
        if (ns.StartsWith("XuanYu", StringComparison.Ordinal)) return "XYEngine 自定义";
        if (ns.StartsWith("XYUI", StringComparison.Ordinal) || ns.Contains("XYUI", StringComparison.Ordinal)) return "XYUI";
        return "未知";
    }
}
