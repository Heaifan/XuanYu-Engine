using System.Globalization;
using Avalonia;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed class XYText : XyuiTextComponent
{
    public XYText() : base("xyui-text") { }
    public override string CanonicalId => "XYUI-1-01";

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arranged = base.ArrangeOverride(finalSize);
        RenderTransform = TryGetSingleLineOffset(finalSize, out var offset)
            ? new TranslateTransform(0, offset) : null;
        return arranged;
    }

    bool TryGetSingleLineOffset(Size finalSize, out double offset)
    {
        offset = 0;
        if (string.IsNullOrEmpty(Text) || TextWrapping != TextWrapping.NoWrap || Text.Contains('\n')) return false;
        var typeface = new Typeface(FontFamily, FontStyle, FontWeight);
        var geometry = new FormattedText(Text, CultureInfo.CurrentUICulture, FlowDirection,
            typeface, FontSize, Brushes.Black).BuildGeometry(new Point(0, 0));
        if (geometry is null || finalSize.Height <= geometry.Bounds.Height) return false;
        offset = finalSize.Height / 2 - geometry.Bounds.Center.Y;
        return true;
    }
}
