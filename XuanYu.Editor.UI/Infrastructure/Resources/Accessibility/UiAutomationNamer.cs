using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;

namespace XuanYu.Editor.UI;

public static class UiAutomationNamer
{
    public static int Apply(Control root)
    {
        var count = 0;
        foreach (var item in root.GetLogicalDescendants().OfType<Control>().Prepend(root))
        {
            if (!string.IsNullOrWhiteSpace(AutomationProperties.GetName(item))) continue;
            var name = CandidateName(item);
            if (string.IsNullOrWhiteSpace(name)) continue;
            AutomationProperties.SetName(item, name);
            count++;
        }
        return count;
    }

    static string CandidateName(Control c) => c switch
    {
        MenuItem m => TextOf(m.Header),
        TabItem t => TextOf(t.Header),
        TextBox t => t.PlaceholderText ?? t.Name ?? "",
        ToggleButton b => TextOf(b.Content) ?? TextOf(ToolTip.GetTip(b)),
        Button b => TextOf(b.Content) ?? TextOf(ToolTip.GetTip(b)),
        _ => ""
    } ?? "";

    static string? TextOf(object? value) => value switch
    {
        null => null,
        string s => Clean(s),
        TextBlock text => Clean(text.Text),
        _ => null
    };

    static string? Clean(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        var trimmed = text.Trim();
        return trimmed.Contains("ARCH-", StringComparison.Ordinal) ? null : trimmed;
    }
}
