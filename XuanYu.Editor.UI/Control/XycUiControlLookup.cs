using Avalonia.Automation;
using Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.Editor.UI.XycControl;

internal static class XycUiControlLookup
{
    public static Control ResolveUnique(IEnumerable<Control> controls, string id)
    {
        var matches = controls.Where(control => Identity(control).Id == id).Take(2).ToArray();
        if (matches.Length == 0) throw new XycUiCommandException("CONTROL_NOT_FOUND");
        if (matches.Length != 1) throw new XycUiCommandException("DUPLICATE_CONTROL_ID");
        return matches[0];
    }

    public static (string? Id, string Source) Identity(Control control)
    {
        if (XYDiagnostic.GetDebugId(control) is { Length: > 0 } debugId)
            return (debugId, "XYDiagnostic.DebugId");
        if (AutomationProperties.GetAutomationId(control) is { Length: > 0 } automationId)
            return (automationId, "AutomationId");
        return string.IsNullOrWhiteSpace(control.Name) ? (null, "NONE") : (control.Name, "Name");
    }
}
