using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed class XYTooltip : ContentControl
{
    public static readonly StyledProperty<int> ShowDelayProperty = AvaloniaProperty.Register<XYTooltip, int>(nameof(ShowDelay), 400);
    public static readonly StyledProperty<bool> ViewportAvoidanceProperty = AvaloniaProperty.Register<XYTooltip, bool>(nameof(ViewportAvoidance), true);
    public static readonly StyledProperty<bool> AutoFlipProperty = AvaloniaProperty.Register<XYTooltip, bool>(nameof(AutoFlip), true);
    public static readonly StyledProperty<bool> PointerCaptureProperty = AvaloniaProperty.Register<XYTooltip, bool>(nameof(PointerCapture), false);
    public static readonly StyledProperty<bool> InteractiveContentProperty = AvaloniaProperty.Register<XYTooltip, bool>(nameof(InteractiveContent), false);
    public XYTooltip() { Classes.Add("xyui-tooltip"); MaxWidth = 280; }
    public string CanonicalId => "XYUI-1-19";
    public int ShowDelay { get => GetValue(ShowDelayProperty); set => SetValue(ShowDelayProperty, value); }
    public bool ViewportAvoidance { get => GetValue(ViewportAvoidanceProperty); set => SetValue(ViewportAvoidanceProperty, value); }
    public bool AutoFlip { get => GetValue(AutoFlipProperty); set => SetValue(AutoFlipProperty, value); }
    public bool PointerCapture { get => GetValue(PointerCaptureProperty); set => SetValue(PointerCaptureProperty, value); }
    public bool InteractiveContent { get => GetValue(InteractiveContentProperty); set => SetValue(InteractiveContentProperty, value); }
}
