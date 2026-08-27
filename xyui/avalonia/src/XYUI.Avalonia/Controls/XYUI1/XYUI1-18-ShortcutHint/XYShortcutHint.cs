using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Controls;

public enum XyuiShortcutCombinationMode { SeparateKeycaps }

public sealed class XYShortcutHint : Border
{
    public static readonly StyledProperty<string> ShortcutProperty = AvaloniaProperty.Register<XYShortcutHint, string>(nameof(Shortcut), "");
    public static readonly StyledProperty<XyuiShortcutCombinationMode> CombinationModeProperty = AvaloniaProperty.Register<XYShortcutHint, XyuiShortcutCombinationMode>(nameof(CombinationMode), XyuiShortcutCombinationMode.SeparateKeycaps);
    readonly StackPanel _keys = new() { Orientation = global::Avalonia.Layout.Orientation.Horizontal, Spacing = 4 };
    public XYShortcutHint() { Classes.Add("xyui-1-component"); Classes.Add("xyui-shortcut-hint"); Child = _keys; Rebuild(Shortcut); }
    public string CanonicalId => "XYUI-1-18";
    public string Shortcut { get => GetValue(ShortcutProperty); set => SetValue(ShortcutProperty, value); }
    public XyuiShortcutCombinationMode CombinationMode { get => GetValue(CombinationModeProperty); set => SetValue(CombinationModeProperty, value); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change); if (change.Property == ShortcutProperty) Rebuild(change.GetNewValue<string>());
    }

    void Rebuild(string value)
    {
        _keys.Children.Clear();
        foreach (var part in value.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (_keys.Children.Count > 0) _keys.Children.Add(new TextBlock { Text = "+", Classes = { "xyui-shortcut-separator" } });
            _keys.Children.Add(new Border { Child = new TextBlock { Text = part, FontFamily = new FontFamily(XyuiTypographyTokens.FontMono), Classes = { "xyui-shortcut-keycap-text" } }, Classes = { "xyui-shortcut-keycap" } });
        }
    }
}
