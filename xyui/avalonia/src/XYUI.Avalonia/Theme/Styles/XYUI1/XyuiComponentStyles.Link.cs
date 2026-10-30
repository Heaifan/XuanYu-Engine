using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Interaction;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void LinkStates(Styles styles)
    {
        var baseStyle = new Style(x => x.OfType<XYLink>().Class("xyui-link"));
        baseStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent));
        baseStyle.Setters.Add(new Setter(Button.BorderBrushProperty, Brushes.Transparent));
        baseStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(0)));
        baseStyle.Setters.Add(new Setter(Button.PaddingProperty, new Thickness(2, 0)));
        baseStyle.Setters.Add(new Setter(Button.MinHeightProperty, 0d));
        baseStyle.Setters.Add(new Setter(Button.MinWidthProperty, 0d));
        baseStyle.Setters.Add(new Setter(Button.CursorProperty, new Cursor(StandardCursorType.Hand)));
        baseStyle.Setters.Add(new Setter(Button.CornerRadiusProperty, new CornerRadius(2)));
        styles.Add(baseStyle);

        // 彻底杜绝 FluentTheme 在 pointerover / pressed 时注入按钮底色
        var hoverBg = new Style(x => x.OfType<XYLink>().Class("xyui-link").Class(":pointerover"));
        hoverBg.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent));
        styles.Add(hoverBg);

        var pressedBg = new Style(x => x.OfType<XYLink>().Class("xyui-link").Class(":pressed"));
        pressedBg.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Transparent));
        styles.Add(pressedBg);

        styles.Add(XyuiInteractionState.Build("xyui-link", XyuiInteractionState.Hover, Button.ForegroundProperty, "XY.Brush.Accent.Strong"));
        styles.Add(XyuiInteractionState.Build("xyui-link", XyuiInteractionState.Pressed, Button.ForegroundProperty, "XY.Brush.Accent.Strong"));
        styles.Add(XyuiInteractionState.Build("xyui-link", XyuiInteractionState.Disabled, Button.ForegroundProperty, "XY.Brush.State.Disabled.Text"));
    }
}
