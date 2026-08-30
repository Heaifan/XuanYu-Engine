using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYMenuItem : Border
{
    bool _building;
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYMenuItem, string>(nameof(Label), "");
    public static readonly StyledProperty<string> ShortcutProperty = AvaloniaProperty.Register<XYMenuItem, string>(nameof(Shortcut), "");
    public static readonly StyledProperty<XyuiVectorIcon?> IconProperty = AvaloniaProperty.Register<XYMenuItem, XyuiVectorIcon?>(nameof(Icon));
    public static readonly StyledProperty<XyuiMenuCheckKind> CheckKindProperty = AvaloniaProperty.Register<XYMenuItem, XyuiMenuCheckKind>(nameof(CheckKind));
    public static readonly StyledProperty<bool> IsCheckedProperty = AvaloniaProperty.Register<XYMenuItem, bool>(nameof(IsChecked));
    public static readonly StyledProperty<bool> IsDestructiveProperty = AvaloniaProperty.Register<XYMenuItem, bool>(nameof(IsDestructive));
    public static readonly StyledProperty<bool> IsHoveredProperty = AvaloniaProperty.Register<XYMenuItem, bool>(nameof(IsHovered));
    public static readonly StyledProperty<bool> HasSubMenuProperty = AvaloniaProperty.Register<XYMenuItem, bool>(nameof(HasSubMenu));
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public string Shortcut { get => GetValue(ShortcutProperty); set => SetValue(ShortcutProperty, value); }
    public XyuiVectorIcon? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public XyuiMenuCheckKind CheckKind { get => GetValue(CheckKindProperty); set => SetValue(CheckKindProperty, value); }
    public bool IsChecked { get => GetValue(IsCheckedProperty); set => SetValue(IsCheckedProperty, value); }
    public bool IsDestructive { get => GetValue(IsDestructiveProperty); set => SetValue(IsDestructiveProperty, value); }
    public bool IsHovered { get => GetValue(IsHoveredProperty); set => SetValue(IsHoveredProperty, value); }
    public bool HasSubMenu { get => GetValue(HasSubMenuProperty); set => SetValue(HasSubMenuProperty, value); }
    public XYMenuItem() { Classes.Add("xyui-menu-item"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (!_building && change.Property != IsEnabledProperty && change.Property != ChildProperty) Build(); }
    void Build() { _building = true; UpdateClasses(); Child = new XYMenuItemVisual(this); _building = false; }
    void UpdateClasses() { Set("xyui-menu-hover", IsHovered); Set("xyui-menu-danger", IsDestructive); Set("xyui-menu-checked", IsChecked); }
    void Set(string name, bool value) { if (value && !Classes.Contains(name)) Classes.Add(name); if (!value) Classes.Remove(name); }
}
