using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using XYUI.Avalonia.Vector;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Controls;

public enum XyuiIconSize { Tiny, Small, Medium, Default = Medium, Large }

public sealed class XYIcon : VectorPath
{
    public static readonly StyledProperty<XyuiVectorIcon> IconProperty = AvaloniaProperty.Register<XYIcon, XyuiVectorIcon>(nameof(Icon), XyuiVectorIcon.Info);
    public static readonly StyledProperty<XyuiIconSize> SizeProperty = AvaloniaProperty.Register<XYIcon, XyuiIconSize>(nameof(Size), XyuiIconSize.Medium);
    public static readonly StyledProperty<double> StrokeWidthProperty = AvaloniaProperty.Register<XYIcon, double>(nameof(StrokeWidth), 1.5d);

    public XYIcon() { Classes.Add("xyui-1-component"); Classes.Add("xyui-icon"); Stretch = Stretch.Uniform; AttachedToVisualTree += (_, _) => ApplyIcon(Icon); ApplyIcon(Icon); ApplySize(Size); }
    public string CanonicalId => "XYUI-1-12";
    public XyuiVectorIcon Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public XyuiIconSize Size { get => GetValue(SizeProperty); set { SetValue(SizeProperty, value); ApplySize(value); } }
    public double StrokeWidth => GetValue(StrokeWidthProperty);
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change); if (change.Property == IconProperty) ApplyIcon(change.GetNewValue<XyuiVectorIcon>());
    }
    void ApplyIcon(XyuiVectorIcon value) { if (XyuiVectorIcons.IsPlatformReady) Data = XyuiVectorIcons.Create(value); }
    void ApplySize(XyuiIconSize value)
    {
        foreach (var name in new[] { "tiny", "small", "medium", "large" }) Classes.Remove($"xyui-icon-{name}");
        var size = value == XyuiIconSize.Tiny ? ("tiny", 1d) : value == XyuiIconSize.Small ? ("small", 1.25d) : value == XyuiIconSize.Large ? ("large", 1.75d) : ("medium", 1.5d);
        Classes.Add($"xyui-icon-{size.Item1}"); SetValue(StrokeWidthProperty, size.Item2);
    }
}
