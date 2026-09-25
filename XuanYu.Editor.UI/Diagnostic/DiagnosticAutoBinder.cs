using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public static class DiagnosticAutoBinder
{
    public static void Bind(Control root)
    {
        var targets = root.GetVisualDescendants().OfType<Control>().Prepend(root).ToArray();
        var used = targets.Select(XYDiagnostic.GetDebugId).OfType<string>()
            .ToHashSet(StringComparer.Ordinal);
        foreach (var target in targets)
        {
            if (XYDiagnostic.GetDebugId(target) is not null || !IsCandidate(target)) continue;
            var token = Token(target); var scope = Scope(target) ?? "XYE.AUTO";
            var id = MakeId(scope, token, used); XYDiagnostic.SetDebugId(target, id); used.Add(id);
        }
    }

    static bool IsCandidate(Control target) => target is Button or ToggleButton ||
        target.GetType().Name.Contains("MenuItem", StringComparison.Ordinal) ||
        target.GetType().Name is "XYWorkspaceSwitcher" or "XYContextToolbar";

    static string? Scope(Control target)
    {
        for (var parent = target.GetVisualParent(); parent is not null; parent = parent.GetVisualParent())
            if (parent is Control control && XYDiagnostic.GetDebugId(control) is { } id) return id;
        return null;
    }

    static string MakeId(string scope, string token, HashSet<string> used)
    {
        var baseId = scope.Split('.').Length < 4 ? $"{scope}.{token}" : $"XYE.AUTO.{token}";
        var id = baseId; var suffix = 1;
        while (used.Contains(id)) id = $"{baseId}_{Suffix(++suffix)}";
        DiagnosticId.Validate(id); return id;
    }

    static string Suffix(int value) => new((char)('A' + value - 1), 1);

    static string Token(Control target)
    {
        var raw = string.IsNullOrWhiteSpace(target.Name) ? target.GetType().Name : target.Name!;
        var chars = raw.SelectMany((value, index) => index > 0 && char.IsUpper(value) && char.IsLower(raw[index - 1])
            ? new[] { '_', value } : new[] { value });
        var token = new string(chars.ToArray()).ToUpperInvariant();
        return token.Trim('_');
    }
}
