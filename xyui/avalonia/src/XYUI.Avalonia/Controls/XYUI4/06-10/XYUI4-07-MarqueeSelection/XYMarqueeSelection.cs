using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed class XYMarqueeSelection : Control
{
    public static readonly StyledProperty<IBrush?> BorderBrushProperty =
        AvaloniaProperty.Register<XYMarqueeSelection, IBrush?>(nameof(BorderBrush));
    public static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<XYMarqueeSelection, IBrush?>(nameof(Fill));
    public static readonly StyledProperty<bool> IsCrossingProperty =
        AvaloniaProperty.Register<XYMarqueeSelection, bool>(nameof(IsCrossing));

    static XYMarqueeSelection() => AffectsRender<XYMarqueeSelection>(BorderBrushProperty, FillProperty, IsCrossingProperty);

    public XYMarqueeSelection()
    {
        Classes.Add("xyui-4-component");
        Classes.Add("xyui-marquee-selection");
        ApplyCrossingClass();
        IsHitTestVisible = false;
    }

    public string CanonicalId => "XYUI-4-4.07";
    public IBrush? BorderBrush { get => GetValue(BorderBrushProperty); set => SetValue(BorderBrushProperty, value); }
    public IBrush? Fill { get => GetValue(FillProperty); set => SetValue(FillProperty, value); }
    public bool IsCrossing { get => GetValue(IsCrossingProperty); set => SetValue(IsCrossingProperty, value); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsCrossingProperty) ApplyCrossingClass();
    }

    void ApplyCrossingClass() => Classes.Set("xyui-marquee-selection-crossing", IsCrossing);

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Bounds.Width <= 0 || Bounds.Height <= 0 || BorderBrush is null) return;
        var pen = new Pen(BorderBrush, 1.25, IsCrossing ? DashStyle.Dash : null);
        context.DrawRectangle(Fill, pen, new Rect(0.625, 0.625, Math.Max(0, Bounds.Width - 1.25), Math.Max(0, Bounds.Height - 1.25)));
    }
}
