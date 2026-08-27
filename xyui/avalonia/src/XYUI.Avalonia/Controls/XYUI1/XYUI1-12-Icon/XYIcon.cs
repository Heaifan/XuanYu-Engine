using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public enum XyuiIconSize { Tiny, Small, Medium, Default = Medium, Large }

public sealed partial class XYIcon : Control
{
    public static readonly StyledProperty<XyuiVectorIcon> IconProperty = AvaloniaProperty.Register<XYIcon, XyuiVectorIcon>(nameof(Icon), XyuiVectorIcon.Info);
    public static readonly StyledProperty<XyuiIconSize> SizeProperty = AvaloniaProperty.Register<XYIcon, XyuiIconSize>(nameof(Size), XyuiIconSize.Medium);
    public static readonly StyledProperty<double> StrokeWidthProperty = AvaloniaProperty.Register<XYIcon, double>(nameof(StrokeWidth), 1.5d);
    public static readonly StyledProperty<IBrush?> StrokeProperty = AvaloniaProperty.Register<XYIcon, IBrush?>(nameof(Stroke));
    public static readonly StyledProperty<IBrush?> FillProperty = AvaloniaProperty.Register<XYIcon, IBrush?>(nameof(Fill));
    public static readonly StyledProperty<double> StrokeThicknessProperty = AvaloniaProperty.Register<XYIcon, double>(nameof(StrokeThickness), 1.5d);
    public static readonly StyledProperty<Stretch> StretchProperty = AvaloniaProperty.Register<XYIcon, Stretch>(nameof(Stretch), Stretch.Uniform);

    public XYIcon() { Classes.Add("xyui-1-component"); Classes.Add("xyui-icon"); Stretch = Stretch.Uniform; AttachedToVisualTree += (_, _) => ApplyIcon(Icon); ApplyIcon(Icon); ApplySize(Size); }
    public string CanonicalId => "XYUI-1-12";
    public XyuiVectorIcon Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public XyuiIconSize Size { get => GetValue(SizeProperty); set { SetValue(SizeProperty, value); ApplySize(value); } }
    public double StrokeWidth { get => GetValue(StrokeWidthProperty); set => SetValue(StrokeWidthProperty, value); }
    public IBrush? Stroke { get => GetValue(StrokeProperty); set => SetValue(StrokeProperty, value); }
    public IBrush? Fill { get => GetValue(FillProperty); set => SetValue(FillProperty, value); }
    public double StrokeThickness { get => GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }
    public Stretch Stretch { get => GetValue(StretchProperty); set => SetValue(StretchProperty, value); }
    public Geometry? IconGeometry { get; private set; }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconProperty) ApplyIcon(change.GetNewValue<XyuiVectorIcon>());
        if (change.Property == StrokeWidthProperty) StrokeThickness = change.GetNewValue<double>();
    }
    void ApplyIcon(XyuiVectorIcon value)
    {
        if (XyuiVectorIcons.IsPlatformReady) IconGeometry = XyuiVectorIcons.Create(value);
        InvalidateVisual();
    }
    void ApplySize(XyuiIconSize value)
    {
        foreach (var name in new[] { "tiny", "small", "medium", "large" }) Classes.Remove($"xyui-icon-{name}");
        var size = value == XyuiIconSize.Tiny ? ("tiny", 1d) : value == XyuiIconSize.Small ? ("small", 1.25d) : value == XyuiIconSize.Large ? ("large", 1.75d) : ("medium", 1.5d);
        Classes.Add($"xyui-icon-{size.Item1}"); SetValue(StrokeWidthProperty, size.Item2);
    }
}
