using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

static class ContextToolbarDiagnosticBridge
{
    static readonly string[] Ids =
    [
        "XYE.CONTEXT_MENU", "XYE.CONTEXT_MENU.POINT", "XYE.CONTEXT_MENU.LINE",
        "XYE.CONTEXT_MENU.AREA", "XYE.CONTEXT_ACTION.MARKER", "XYE.CONTEXT_ACTION.ROAD",
        "XYE.CONTEXT_ACTION.REGION"
    ];

    public static void Attach(XYContextDropdownBoard board)
    {
        var targets = Targets(board);
        for (var i = 0; i < targets.Count; i++) XYDiagnostic.SetDebugId(targets[i], Ids[i]);
        var syncing = false;
        EventHandler changed = (_, _) =>
        {
            if (syncing || !board.Popup.IsOpen || targets.All(IsRegistered)) return;
            syncing = true; Register(targets); syncing = false;
        };
        board.Popup.Opened += (_, _) =>
        {
            DiagnosticRegistry.Changed += changed;
            syncing = true; Register(targets); syncing = false;
        };
        board.Popup.Closed += (_, _) =>
        {
            DiagnosticRegistry.Changed -= changed;
            Unregister(targets);
        };
    }

    static void Register(IReadOnlyList<Control> targets)
    {
        foreach (var target in targets) DiagnosticRegistry.Unregister(target);
        foreach (var target in targets) DiagnosticRegistry.Register(target);
    }

    static void Unregister(IReadOnlyList<Control> targets)
    {
        foreach (var target in targets) DiagnosticRegistry.Unregister(target);
    }

    static bool IsRegistered(Control target) =>
        XYDiagnostic.GetDebugId(target) is { } id && DiagnosticRegistry.Targets.TryGetValue(id, out var current) && ReferenceEquals(current, target);

    static IReadOnlyList<Control> Targets(XYContextDropdownBoard board)
    {
        var categories = board.Menu.Items.OfType<XYMenuItem>().ToArray();
        var actions = board.SubMenus.SelectMany(x => x.ChildMenu.Items.OfType<XYMenuItem>()).ToArray();
        return [board.RootMenuSurface, categories[0], categories[1], categories[2], actions[0], actions[1], actions[2]];
    }
}
