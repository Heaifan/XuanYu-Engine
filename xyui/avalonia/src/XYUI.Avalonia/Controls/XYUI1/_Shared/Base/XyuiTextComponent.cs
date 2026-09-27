using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.Globalization;

namespace XYUI.Avalonia.Controls;

public abstract class XyuiTextComponent : TextBlock
{
    protected XyuiTextComponent(string className)
    {
        Classes.Add("xyui-1-component");
        Classes.Add(className);
    }

    public abstract string CanonicalId { get; }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arranged = base.ArrangeOverride(finalSize);
        RenderTransform = SingleLineTransform(finalSize);
        return arranged;
    }

    TranslateTransform? SingleLineTransform(Size finalSize)
    {
        if (string.IsNullOrEmpty(Text) || TextWrapping != TextWrapping.NoWrap || Text.Contains('\n')) return null;
        var typeface = new Typeface(FontFamily, FontStyle, FontWeight);
        var geometry = new FormattedText(Text, CultureInfo.CurrentUICulture, FlowDirection,
            typeface, FontSize, Brushes.Black).BuildGeometry(new Point(0, 0));
        if (geometry is null || finalSize.Height <= geometry.Bounds.Height) return null;
        return new TranslateTransform(0, finalSize.Height / 2 - geometry.Bounds.Center.Y);
    }
}

public abstract class XyuiTextSurface : Border
{
    protected readonly TextBlock TextPresenter = new();

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<XyuiTextSurface, string>(nameof(Text), "");

    protected XyuiTextSurface(string className)
    {
        Classes.Add("xyui-1-component");
        Classes.Add(className);
        TextPresenter.Classes.Add($"{className}-text");
        Child = TextPresenter;
    }

    public string Text { get => GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public abstract string CanonicalId { get; }

    protected virtual string FormatText(string value) => value;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty) TextPresenter.Text = FormatText(change.GetNewValue<string>());
    }
}
