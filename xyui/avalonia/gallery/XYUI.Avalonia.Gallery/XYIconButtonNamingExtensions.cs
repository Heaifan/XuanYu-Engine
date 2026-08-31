using Avalonia.Automation;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

internal static class XYIconButtonNamingExtensions
{
    public static XYIconButton Named(this XYIconButton button, string name)
    {
        AutomationProperties.SetName(button, name);
        return button;
    }
}
