using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace XYUI.Avalonia.Themes;

public sealed class XYUITheme : Styles
{
    public XYUITheme()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
