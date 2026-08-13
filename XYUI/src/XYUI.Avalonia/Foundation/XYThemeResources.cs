using Avalonia.Controls;

namespace XYUI.Avalonia.Foundation;

public static class XYThemeResources
{
    public static ResourceDictionary CreateLight()
    {
        return new ResourceDictionary
        {
            ["XY.Theme.Variant"] = XYThemeVariant.Light
        };
    }

    public static ResourceDictionary CreateDark()
    {
        var resources = new ResourceDictionary
        {
            ["XY.Theme.Variant"] = XYThemeVariant.Dark
        };
        resources["XY.Color.Surface"] = global::Avalonia.Media.Color.Parse("#17212B");
        resources["XY.Color.SurfaceRaised"] = global::Avalonia.Media.Color.Parse("#24313D");
        resources["XY.Color.TextPrimary"] = global::Avalonia.Media.Color.Parse("#F4F7FA");
        return resources;
    }
}
