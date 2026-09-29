using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public enum XyuiInsertionMode { Before, Into, After }

public sealed class XYInsertionIndicator : Control
{
    public static readonly StyledProperty<Rect> TargetRectProperty = AvaloniaProperty.Register<XYInsertionIndicator, Rect>(nameof(TargetRect));
    public static readonly StyledProperty<Rect> GapRectProperty = AvaloniaProperty.Register<XYInsertionIndicator, Rect>(nameof(GapRect));
    public static readonly StyledProperty<XyuiInsertionMode> ModeProperty = AvaloniaProperty.Register<XYInsertionIndicator, XyuiInsertionMode>(nameof(Mode));
    public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<XYInsertionIndicator, bool>(nameof(IsActive));
    public static readonly StyledProperty<bool> ShowGapPreviewProperty = AvaloniaProperty.Register<XYInsertionIndicator, bool>(nameof(ShowGapPreview));
    public static readonly StyledProperty<int> IndentLevelProperty = AvaloniaProperty.Register<XYInsertionIndicator, int>(nameof(IndentLevel));
    public static readonly StyledProperty<double> IndentPerLevelProperty = AvaloniaProperty.Register<XYInsertionIndicator, double>(nameof(IndentPerLevel), 16);
    public static readonly StyledProperty<double> AnchorSizeProperty = AvaloniaProperty.Register<XYInsertionIndicator, double>(nameof(AnchorSize), 7);
    public static readonly StyledProperty<double> LineWidthProperty = AvaloniaProperty.Register<XYInsertionIndicator, double>(nameof(LineWidth), 2);
    public static readonly StyledProperty<IBrush?> LineBrushProperty = AvaloniaProperty.Register<XYInsertionIndicator, IBrush?>(nameof(LineBrush));
    public static readonly StyledProperty<IBrush?> GapBrushProperty = AvaloniaProperty.Register<XYInsertionIndicator, IBrush?>(nameof(GapBrush));
    public static readonly StyledProperty<IBrush?> GapBorderBrushProperty = AvaloniaProperty.Register<XYInsertionIndicator, IBrush?>(nameof(GapBorderBrush));
    public static readonly StyledProperty<string?> GapTextProperty = AvaloniaProperty.Register<XYInsertionIndicator, string?>(nameof(GapText));

    static XYInsertionIndicator() => AffectsRender<XYInsertionIndicator>(TargetRectProperty, GapRectProperty, ModeProperty, IsActiveProperty, ShowGapPreviewProperty, IndentLevelProperty, IndentPerLevelProperty, AnchorSizeProperty, LineWidthProperty, LineBrushProperty, GapBrushProperty, GapBorderBrushProperty);

    public XYInsertionIndicator() { Classes.Add("xyui-4-component"); Classes.Add("xyui-insertion-indicator"); IsHitTestVisible = false; }
    public string CanonicalId => "XYUI-4-4.13";
    public Rect TargetRect { get => GetValue(TargetRectProperty); set => SetValue(TargetRectProperty, value); }
    public Rect GapRect { get => GetValue(GapRectProperty); set => SetValue(GapRectProperty, value); }
    public XyuiInsertionMode Mode { get => GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    public bool IsActive { get => GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }
    public bool ShowGapPreview { get => GetValue(ShowGapPreviewProperty); set => SetValue(ShowGapPreviewProperty, value); }
    public int IndentLevel { get => GetValue(IndentLevelProperty); set => SetValue(IndentLevelProperty, Math.Max(0, value)); }
    public double IndentPerLevel { get => GetValue(IndentPerLevelProperty); set => SetValue(IndentPerLevelProperty, Math.Max(0, value)); }
    public double AnchorSize { get => GetValue(AnchorSizeProperty); set => SetValue(AnchorSizeProperty, Math.Clamp(value, 6, 8)); }
    public double LineWidth { get => GetValue(LineWidthProperty); set => SetValue(LineWidthProperty, Math.Clamp(value, 2, 2.5)); }
    public IBrush? LineBrush { get => GetValue(LineBrushProperty); set => SetValue(LineBrushProperty, value); }
    public IBrush? GapBrush { get => GetValue(GapBrushProperty); set => SetValue(GapBrushProperty, value); }
    public IBrush? GapBorderBrush { get => GetValue(GapBorderBrushProperty); set => SetValue(GapBorderBrushProperty, value); }
    public string? GapText { get => GetValue(GapTextProperty); set => SetValue(GapTextProperty, value); }
    public double AnchorOffset => IndentLevel * IndentPerLevel;

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (!IsActive) return;
        if (ShowGapPreview && GapRect.Width > 0 && GapRect.Height > 0 && GapBorderBrush is not null)
            context.DrawRectangle(GapBrush, new Pen(GapBorderBrush, 1, new DashStyle([4, 4], 0)), GapRect);
        if (LineBrush is null || TargetRect.Width <= 0 || TargetRect.Height <= 0) return;
        var anchor = new Point(TargetRect.Left + AnchorOffset, TargetRect.Center.Y);
        var pen = new Pen(LineBrush, LineWidth);
        if (Mode == XyuiInsertionMode.Into)
        {
            context.DrawLine(pen, new Point(anchor.X, TargetRect.Top + AnchorSize), new Point(anchor.X, TargetRect.Bottom - AnchorSize));
            anchor = new Point(anchor.X, TargetRect.Center.Y);
        }
        else
        {
            var y = Mode == XyuiInsertionMode.Before ? TargetRect.Top : TargetRect.Bottom;
            anchor = new Point(anchor.X, y);
            context.DrawLine(pen, anchor, new Point(TargetRect.Right, y));
        }
        context.DrawEllipse(LineBrush, null, anchor, AnchorSize / 2, AnchorSize / 2);
    }
}
