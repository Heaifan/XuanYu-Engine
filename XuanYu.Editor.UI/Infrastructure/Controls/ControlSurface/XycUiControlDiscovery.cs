using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;

namespace XuanYu.Editor.UI.XycControl;

internal sealed partial class XycUiRuntime
{
    Control[] Controls()
    {
        var top = FindTop();
        return new Control[] { top }.Concat(top.GetVisualDescendants().OfType<Control>()).ToArray();
    }

    static XycUiControlState Snapshot(Control control, (string? Id, string Source) identity,
        bool duplicate) => new(identity.Id, control.GetType().Name,
        control.IsEffectivelyVisible, control.IsEnabled, duplicate, identity.Source, "TOP",
        control is ToggleButton toggle ? toggle.IsChecked : null);
}
