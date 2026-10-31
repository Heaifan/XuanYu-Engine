using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public enum XyuiDropState { None, Valid, Invalid, Conditional }

public sealed class XYDropIndicator : Control
{
    public static readonly StyledProperty<Rect> TargetRectProperty = AvaloniaProperty.Register<XYDropIndicator, Rect>(nameof(TargetRect));
    public static readonly StyledProperty<XyuiDropState> StateProperty = AvaloniaProperty.Register<XYDropIndicator, XyuiDropState>(nameof(State));
    public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<XYDropIndicator, bool>(nameof(IsActive));
    public static readonly StyledProperty<double> BorderWidthProperty = AvaloniaProperty.Register<XYDropIndicator, double>(nameof(BorderWidth), 1.5);
    public static readonly StyledProperty<IBrush?> TargetBorderBrushProperty = AvaloniaProperty.Register<XYDropIndicator, IBrush?>(nameof(TargetBorderBrush));
    public static readonly StyledProperty<IBrush?> ValidBrushProperty = AvaloniaProperty.Register<XYDropIndicator, IBrush?>(nameof(ValidBrush));
    public static readonly StyledProperty<IBrush?> InvalidBrushProperty = AvaloniaProperty.Register<XYDropIndicator, IBrush?>(nameof(InvalidBrush));
    public static readonly StyledProperty<IBrush?> ConditionalBrushProperty = AvaloniaProperty.Register<XYDropIndicator, IBrush?>(nameof(ConditionalBrush));
    public static readonly StyledProperty<IBrush?> ContainerTintBrushProperty = AvaloniaProperty.Register<XYDropIndicator, IBrush?>(nameof(ContainerTintBrush));
    public static readonly StyledProperty<double> ContainerTintOpacityProperty = AvaloniaProperty.Register<XYDropIndicator, double>(nameof(ContainerTintOpacity), .08);
    public static readonly StyledProperty<string?> ActionHintProperty = AvaloniaProperty.Register<XYDropIndicator, string?>(nameof(ActionHint));

    static XYDropIndicator() => AffectsRender<XYDropIndicator>(TargetRectProperty, StateProperty, IsActiveProperty, BorderWidthProperty, TargetBorderBrushProperty, ValidBrushProperty, InvalidBrushProperty, ConditionalBrushProperty, ContainerTintBrushProperty, ContainerTintOpacityProperty);

    public XYDropIndicator() { Classes.Add("xyui-4-component"); Classes.Add("xyui-drop-indicator"); IsHitTestVisible = false; }
    public string CanonicalId => "XYUI-4-4.12";
    public string ConditionalTokenGap => "GAP-004";
    public Rect TargetRect { get => GetValue(TargetRectProperty); set => SetValue(TargetRectProperty, value); }
    public XyuiDropState State { get => GetValue(StateProperty); set => SetValue(StateProperty, value); }
    public bool IsActive { get => GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }
    public double BorderWidth { get => GetValue(BorderWidthProperty); set => SetValue(BorderWidthProperty, Math.Clamp(value, 1.5, 2)); }
    public IBrush? TargetBorderBrush { get => GetValue(TargetBorderBrushProperty); set => SetValue(TargetBorderBrushProperty, value); }
    public IBrush? ValidBrush { get => GetValue(ValidBrushProperty); set => SetValue(ValidBrushProperty, value); }
    public IBrush? InvalidBrush { get => GetValue(InvalidBrushProperty); set => SetValue(InvalidBrushProperty, value); }
    public IBrush? ConditionalBrush { get => GetValue(ConditionalBrushProperty); set => SetValue(ConditionalBrushProperty, value); }
    public IBrush? ContainerTintBrush { get => GetValue(ContainerTintBrushProperty); set => SetValue(ContainerTintBrushProperty, value); }
    public double ContainerTintOpacity { get => GetValue(ContainerTintOpacityProperty); set => SetValue(ContainerTintOpacityProperty, Math.Clamp(value, 0, .2)); }
    public string? ActionHint { get => GetValue(ActionHintProperty); set => SetValue(ActionHintProperty, value); }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (!IsActive || State == XyuiDropState.None || TargetRect.Width <= 0 || TargetRect.Height <= 0) return;
        var semantic = State switch { XyuiDropState.Valid => ValidBrush, XyuiDropState.Invalid => InvalidBrush, XyuiDropState.Conditional => ConditionalBrush, _ => null };
        var border = semantic ?? TargetBorderBrush;
        if (ContainerTintBrush is not null) using (context.PushOpacity(ContainerTintOpacity)) context.DrawRectangle(ContainerTintBrush, null, TargetRect);
        if (border is null) return;
        context.DrawRectangle(null, new Pen(border, BorderWidth), TargetRect);
        DrawStateMark(context, semantic ?? border, TargetRect.Center);
    }

    static void DrawStateMark(DrawingContext context, IBrush brush, Point center)
    {
        var pen = new Pen(brush, 2);
        context.DrawEllipse(null, pen, center, 6, 6);
        context.DrawLine(pen, new Point(center.X - 3, center.Y), new Point(center.X + 3, center.Y));
    }
}
