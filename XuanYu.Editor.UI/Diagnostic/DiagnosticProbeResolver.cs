using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;
public static partial class DiagnosticProbeResolver
{
    const string Missing = "N/A";

    public static DiagnosticProbeResult Resolve(Visual hit, bool deepVisual = false)
    {
        var chain = Chain(hit).ToArray();
        var semantic = deepVisual ? null : chain
            .Where(x => ReferenceEquals(x, hit) || !HasDebugId(x))
            .Select(Candidate).OrderBy(x => x.Rank).First().Control as Control;
        var selected = deepVisual ? hit : semantic ?? hit;
        var parentId = ParentDebugId(selected);
        if (parentId == Missing && !ReferenceEquals(selected, hit)) parentId = ParentDebugId(hit);
        return new DiagnosticProbeResult(hit, semantic, Value(DebugId(selected)), parentId,
            selected.GetType().Name, Name(selected), Text(selected), Locator(selected, hit),
            selected.IsEffectivelyVisible, selected is Control control && control.IsEffectivelyEnabled,
            selected.Bounds, deepVisual ? DiagnosticProbeMode.DeepVisual : DiagnosticProbeMode.Semantic);
    }

    static IEnumerable<Visual> Chain(Visual hit)
    {
        for (Visual? current = hit; current is not null; current = current.GetVisualParent())
            yield return current;
    }

    static (Visual Control, int Rank) Candidate(Visual visual)
    {
        if (IsTemplateInternal(visual)) return (visual, 4);
        if (DiagnosticXyuiResolver.IsMapped(visual)) return (visual, 0);
        if (HasDebugId(visual)) return (visual, 1);
        if (visual is Control control && !string.IsNullOrEmpty(NameValue(control)) && IsInteractive(control))
            return (visual, 2);
        if (visual is Control known && IsInteractive(known)) return (visual, 3);
        if (visual is Control named && !string.IsNullOrEmpty(NameValue(named))) return (visual, 4);
        return (visual, 5);
    }

    static bool IsInteractive(Control control) => control is Button or ToggleButton or TextBox or
        ComboBox or TreeViewItem or TabItem || control.GetType().Namespace?.StartsWith("XYUI", StringComparison.Ordinal) == true;

    static bool HasDebugId(Visual visual) => LocalDebugId(visual) is not null;
    static string? DebugId(Visual visual) => LocalDebugId(visual);

    static string? LocalDebugId(Visual visual) => visual is Control control
        ? XYDiagnostic.GetDebugId(control)
        : null;

    static string ParentDebugId(Visual selected) => Chain(selected).Skip(1).Select(DebugId).FirstOrDefault(x => x is not null) ?? Missing;

    static string Name(Visual visual) => visual is Control control && NameValue(control) is { Length: > 0 } name ? name : Missing;

    static string? NameValue(Control control) => control.Name;

    static string Text(Visual visual) => visual switch
    {
        TextBlock text => Value(text.Text),
        TextBox text => Value(text.Text),
        ContentControl content => content.Content?.ToString() ?? Missing,
        ContentPresenter presenter => presenter.Content?.ToString() ?? Missing,
        _ => Missing
    };

    static string Locator(Visual selected, Visual fallback)
    {
        if (!ReferenceEquals(selected, fallback) && HasDebugId(selected)) return Locator(fallback, fallback);
        var chain = Chain(selected).ToArray();
        var root = chain.FirstOrDefault(HasDebugId);
        if (root is null && !ReferenceEquals(selected, fallback)) return Locator(fallback, fallback);
        if (root is null) return Missing;
        var rootIndex = Array.FindIndex(chain, item => ReferenceEquals(item, root));
        if (rootIndex < 0) return Missing;
        var parts = chain.Take(rootIndex).Reverse()
            .Where(item => HasName(item) || !HasNamedDescendant(chain, item))
            .Select(PathPart).ToArray();
        return string.Join("/", new[] { DebugId(root) ?? Missing }.Concat(parts));
    }

    static bool HasNamedDescendant(Visual[] chain, Visual item) =>
        chain.TakeWhile(x => !ReferenceEquals(x, item)).Any(x => x is Control control && NameValue(control) is not null);

    static bool HasName(Visual visual) => visual is Control control && NameValue(control) is not null;

    static string PathPart(Visual visual)
    {
        if (visual is Control control && !string.IsNullOrEmpty(NameValue(control))) return NameValue(control)!;
        var parent = visual.GetVisualParent();
        if (parent is null) return visual.GetType().Name + "[0]";
        var index = parent.GetVisualChildren().OfType<Visual>().Where(x => x.GetType() == visual.GetType()).TakeWhile(x => !ReferenceEquals(x, visual)).Count();
        return $"{visual.GetType().Name}[{index}]";
    }

    static string Value(string? value) => string.IsNullOrEmpty(value) ? Missing : value;
}
