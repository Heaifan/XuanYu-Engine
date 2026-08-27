using Avalonia;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYIconLabel : XyuiVectorTextSurface
{
    public static readonly StyledProperty<XyuiVectorIcon> IconProperty = AvaloniaProperty.Register<XYIconLabel, XyuiVectorIcon>(nameof(Icon), XyuiVectorIcon.Info);
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYIconLabel, string>(nameof(Label), "");

    public XYIconLabel() : base("xyui-icon-label", XyuiVectorIcon.Info, XyuiVectorMarkPlacement.Inline) { AttachedToVisualTree += (_, _) => UpdateIcon(Icon); UpdateIcon(Icon); UpdateText(); }
    public override string CanonicalId => "XYUI-1-13";
    public XyuiVectorIcon Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change); if (change.Property == IconProperty) UpdateIcon(change.GetNewValue<XyuiVectorIcon>()); if (change.Property == LabelProperty) UpdateText();
    }
    void UpdateIcon(XyuiVectorIcon value) { if (XyuiVectorIcons.IsPlatformReady) VectorMark.Data = XyuiVectorIcons.Create(value); }
    void UpdateText() => Text = Label;
}
