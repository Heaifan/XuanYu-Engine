using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Styling;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiComponentStyles
{
    static void AddXYUI4(Styles styles) { Spinner(styles); LoadingIndicator(styles); ProgressBar(styles); }

    static void Spinner(Styles styles)
    {
        var root = new Style(x => x.OfType<XYSpinner>().Class("xyui-spinner"));
        Brush(root, XYSpinner.TrackProperty, "XY.Brush.Accent.Soft");
        Brush(root, XYSpinner.ArcProperty, "XY.Brush.Accent.Default"); styles.Add(root);
    }

    static void LoadingIndicator(Styles styles)
    {
        var root = new Style(x => x.OfType<XYLoadingIndicator>().Class("xyui-loading-indicator"));
        root.Setters.Add(new Setter(Border.BackgroundProperty, Brushes.Transparent));
        root.Setters.Add(new Setter(Border.BorderThicknessProperty, new Thickness(0))); styles.Add(root);
        var label = new Style(x => x.OfType<TextBlock>().Class("xyui-loading-indicator-text"));
        label.Setters.Add(new Setter(TextBlock.FontSizeProperty, XyuiTypographyTokens.FontSizeBody));
        label.Setters.Add(new Setter(TextBlock.ForegroundProperty, new DynamicResourceExtension("XY.Brush.Text.Secondary"))); styles.Add(label);
        var secondary = new Style(x => x.OfType<TextBlock>().Class("xyui-loading-indicator-secondary"));
        secondary.Setters.Add(new Setter(TextBlock.FontSizeProperty, XyuiTypographyTokens.FontSizeCaption));
        secondary.Setters.Add(new Setter(TextBlock.ForegroundProperty, new DynamicResourceExtension("XY.Brush.Text.Secondary"))); styles.Add(secondary);
    }

    static void ProgressBar(Styles styles)
    {
        var root = new Style(x => x.OfType<XYProgressBar>().Class("xyui-progress-bar"));
        Brush(root, XYProgressBar.TrackProperty, "XY.Brush.Accent.Soft");
        Brush(root, XYProgressBar.FillProperty, "XY.Brush.Accent.Default"); styles.Add(root);
    }
}
