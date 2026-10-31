using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenuItem : Border
{
    bool _building;
    XYSubMenu? _subMenu;
    public static readonly StyledProperty<string> IdProperty = AvaloniaProperty.Register<XYMenuItem, string>(nameof(Id), "");
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYMenuItem, string>(nameof(Label), "");
    public static readonly StyledProperty<string> ShortcutProperty = AvaloniaProperty.Register<XYMenuItem, string>(nameof(Shortcut), "");
    public static readonly StyledProperty<XyuiVectorIcon?> IconProperty = AvaloniaProperty.Register<XYMenuItem, XyuiVectorIcon?>(nameof(Icon));
    public static readonly StyledProperty<XyuiMenuCheckKind> CheckKindProperty = AvaloniaProperty.Register<XYMenuItem, XyuiMenuCheckKind>(nameof(CheckKind));
    public static readonly StyledProperty<bool> IsCheckedProperty = AvaloniaProperty.Register<XYMenuItem, bool>(nameof(IsChecked), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<XYMenuItem, bool>(nameof(IsSelected));
    public static readonly StyledProperty<bool> IsDestructiveProperty = AvaloniaProperty.Register<XYMenuItem, bool>(nameof(IsDestructive));
    public static readonly StyledProperty<bool> IsHoveredProperty = AvaloniaProperty.Register<XYMenuItem, bool>(nameof(IsHovered));
    public static readonly StyledProperty<bool> HasSubMenuProperty = AvaloniaProperty.Register<XYMenuItem, bool>(nameof(HasSubMenu));
    public static readonly StyledProperty<object?> CommandProperty = AvaloniaProperty.Register<XYMenuItem, object?>(nameof(Command));
    public static readonly StyledProperty<object?> CommandParameterProperty = AvaloniaProperty.Register<XYMenuItem, object?>(nameof(CommandParameter));
    public string Id { get => GetValue(IdProperty); set => SetValue(IdProperty, value); }
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public string Header { get => Label; set => Label = value; }
    public string Shortcut { get => GetValue(ShortcutProperty); set => SetValue(ShortcutProperty, value); }
    public XyuiVectorIcon? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public XyuiMenuCheckKind CheckKind { get => GetValue(CheckKindProperty); set => SetValue(CheckKindProperty, value); }
    public string? ToggleType
    {
        get => CheckKind switch { XyuiMenuCheckKind.Radio => "Radio", XyuiMenuCheckKind.Check => "CheckBox", _ => "None" };
        set { if (string.Equals(value, "Radio", StringComparison.OrdinalIgnoreCase)) CheckKind = XyuiMenuCheckKind.Radio; else if (string.Equals(value, "CheckBox", StringComparison.OrdinalIgnoreCase)) CheckKind = XyuiMenuCheckKind.Check; else CheckKind = XyuiMenuCheckKind.None; }
    }
    public bool IsChecked { get => GetValue(IsCheckedProperty); set => SetValue(IsCheckedProperty, value); }
    public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public bool IsDestructive { get => GetValue(IsDestructiveProperty); set => SetValue(IsDestructiveProperty, value); }
    public bool IsHovered { get => GetValue(IsHoveredProperty); set => SetValue(IsHoveredProperty, value); }
    public bool HasSubMenu { get => GetValue(HasSubMenuProperty); set => SetValue(HasSubMenuProperty, value); }
    public object? Command { get => GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
    public object? CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }
    public XYSubMenu? SubMenu { get => _subMenu; set { _subMenu = value; HasSubMenu = value is not null; } }
    public XYMenuItem() { Classes.Add("xyui-menu-item"); FocusAdorner = null; Build(); InitializeInteraction(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == CommandProperty) OnCommandChanged(change.OldValue as ICommand, change.NewValue as ICommand);
        else if (change.Property == CommandParameterProperty) UpdateCanExecute();
        else if (!_building && change.Property != IsEnabledProperty && change.Property != ChildProperty)
        {
            if (change.Property == CheckKindProperty || change.Property == IconProperty || change.Property == HasSubMenuProperty || change.Property == LabelProperty || change.Property == ShortcutProperty) Build();
            else { UpdateClasses(); (Child as XYMenuItemVisual)?.Refresh(this); }
        }
    }
    void Build() { _building = true; UpdateClasses(); Child = new XYMenuItemVisual(this); _building = false; }
    internal void RebuildVisual() => Build();
    void UpdateClasses() { Set("xyui-menu-hover", IsHovered); Set("xyui-menu-danger", IsDestructive); Set("xyui-menu-checked", IsChecked); Set("xyui-menu-selected", IsSelected); }
    void Set(string name, bool value) { if (value && !Classes.Contains(name)) Classes.Add(name); if (!value) Classes.Remove(name); }
}
