using Avalonia;

namespace XYUI.Avalonia.Controls;

public sealed class XYShortcutHint : XyuiTextSurface
{
    public static readonly StyledProperty<string> ShortcutProperty = AvaloniaProperty.Register<XYShortcutHint, string>(nameof(Shortcut), "");
    public XYShortcutHint() : base("xyui-shortcut-hint") { }
    public override string CanonicalId => "XYUI-1-18";
    public string Shortcut { get => GetValue(ShortcutProperty); set => SetValue(ShortcutProperty, value); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change); if (change.Property == ShortcutProperty) Text = change.GetNewValue<string>();
    }
}
