using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenuBarItem : Border
{
    bool _building;
    XYMenu? _menu;

    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYMenuBarItem, string>(nameof(Label), "");
    public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<XYMenuBarItem, bool>(nameof(IsActive));
    public static readonly StyledProperty<bool> IsHoveredProperty = AvaloniaProperty.Register<XYMenuBarItem, bool>(nameof(IsHovered));
    public static readonly StyledProperty<bool> ShowChevronProperty = AvaloniaProperty.Register<XYMenuBarItem, bool>(nameof(ShowChevron));
    public static readonly StyledProperty<XyuiVectorIcon?> IconProperty = AvaloniaProperty.Register<XYMenuBarItem, XyuiVectorIcon?>(nameof(Icon));
    public static readonly StyledProperty<XyuiIconSize> IconSizeProperty = AvaloniaProperty.Register<XYMenuBarItem, XyuiIconSize>(nameof(IconSize), XyuiIconSize.Small);

    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public string Header { get => Label; set => Label = value; }
    public bool IsActive { get => GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }
    public bool IsHovered { get => GetValue(IsHoveredProperty); set => SetValue(IsHoveredProperty, value); }
    public bool ShowChevron { get => GetValue(ShowChevronProperty); set => SetValue(ShowChevronProperty, value); }
    public XyuiVectorIcon? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public XyuiIconSize IconSize { get => GetValue(IconSizeProperty); set => SetValue(IconSizeProperty, value); }

    [Content]
    public XYMenu? Menu
    {
        get => _menu;
        set
        {
            if (_menu is not null) LogicalChildren.Remove(_menu);
            _menu = value;
            if (_menu is not null) LogicalChildren.Add(_menu);
        }
    }

    public event EventHandler? Activated;

    public XYMenuBarItem() { Classes.Add("xyui-menu-bar-item"); Build(); InitializeInteraction(); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (!_building && change.Property != ChildProperty) Build();
    }

    void Build()
    {
        _building = true;
        Classes.Set("xyui-menu-active", IsActive);
        Classes.Set("xyui-menu-hover", IsHovered);
        Child = BuildVisual();
        _building = false;
    }
}
