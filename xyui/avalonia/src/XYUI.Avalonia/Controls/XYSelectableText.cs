using Avalonia;

namespace XYUI.Avalonia.Controls;

public sealed class XYSelectableText : XyuiTextComponent
{
    public static readonly StyledProperty<bool> IsSelectableProperty = AvaloniaProperty.Register<XYSelectableText, bool>(nameof(IsSelectable), true);
    public XYSelectableText() : base("xyui-selectable-text") { }
    public override string CanonicalId => "XYUI-1-21";
    public bool IsSelectable { get => GetValue(IsSelectableProperty); set => SetValue(IsSelectableProperty, value); }
}
