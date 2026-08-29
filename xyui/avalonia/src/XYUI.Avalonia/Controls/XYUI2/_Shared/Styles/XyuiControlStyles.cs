using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    public static Styles Create()
    {
        var styles = new Styles();
        AddButtonFamily(styles);
        AddInputFamily(styles);
        SearchField(styles); PasswordField(styles);
        Slider(styles);
        AddChoiceControls(styles);
        ComboBox(styles);
        Select(styles);
        TextArea(styles);
        return styles;
    }

    static void State(Styles styles, Type type, string cls, string state,
        AvaloniaProperty property, string resource)
    {
        var style = new Style(x => x.OfType(type).Class(cls).Class(state));
        Set(style, property, resource);
        styles.Add(style);
    }

    static void Set(Style style, AvaloniaProperty property, string resource) =>
        style.Setters.Add(new Setter(property, new DynamicResourceExtension(resource)));
}
