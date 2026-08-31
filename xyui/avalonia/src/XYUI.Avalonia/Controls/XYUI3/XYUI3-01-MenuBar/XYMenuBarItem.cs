using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenuBarItem : Border
{
    bool _building;
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYMenuBarItem, string>(nameof(Label), "");
    public static readonly StyledProperty<bool> IsActiveProperty = AvaloniaProperty.Register<XYMenuBarItem, bool>(nameof(IsActive));
    public static readonly StyledProperty<bool> IsHoveredProperty = AvaloniaProperty.Register<XYMenuBarItem, bool>(nameof(IsHovered));
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public bool IsActive { get => GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }
    public bool IsHovered { get => GetValue(IsHoveredProperty); set => SetValue(IsHoveredProperty, value); }
    public XYMenu? Menu { get; set; }
    public event EventHandler? Activated;
    public XYMenuBarItem() { Classes.Add("xyui-menu-bar-item"); Build(); InitializeInteraction(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (!_building && change.Property != ChildProperty) Build(); }
    void Build()
    {
        _building = true; Classes.Set("xyui-menu-active", IsActive); Classes.Set("xyui-menu-hover", IsHovered); Child = BuildVisual(); _building = false;
    }
}
