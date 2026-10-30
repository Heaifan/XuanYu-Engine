using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYIconLabel : XyuiTextSurface
{
    public static readonly StyledProperty<XyuiVectorIcon> IconProperty = AvaloniaProperty.Register<XYIconLabel, XyuiVectorIcon>(nameof(Icon), XyuiVectorIcon.Info);
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYIconLabel, string>(nameof(Label), "");
    public XYIcon IconPart { get; }

    public XYIconLabel() : base("xyui-icon-label")
    {
        IconPart = new XYIcon { Size = XyuiIconSize.Small, Classes = { "xyui-icon-label-icon" }, VerticalAlignment = VerticalAlignment.Center };
        var content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = XyuiSpatialTokens.Space1, VerticalAlignment = VerticalAlignment.Center };
        Child = null;
        content.Children.Add(IconPart); content.Children.Add(TextPresenter);
        TextPresenter.VerticalAlignment = VerticalAlignment.Center;
        Child = content;
        UpdateIcon(Icon); UpdateText();
    }
    public override string CanonicalId => "XYUI-1-13";
    public XyuiVectorIcon Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change); if (change.Property == IconProperty) UpdateIcon(change.GetNewValue<XyuiVectorIcon>()); if (change.Property == LabelProperty) UpdateText();
    }
    void UpdateIcon(XyuiVectorIcon value) => IconPart.Icon = value;
    void UpdateText() => Text = Label;
}
