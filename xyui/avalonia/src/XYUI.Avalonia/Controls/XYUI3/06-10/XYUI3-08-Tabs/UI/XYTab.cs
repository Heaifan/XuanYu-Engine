using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTab : Border
{
    bool _pointerHooked;
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYTab, string>(nameof(Label), "");
    public static readonly StyledProperty<string> IdProperty = AvaloniaProperty.Register<XYTab, string>(nameof(Id), "");
    public static readonly StyledProperty<XyuiVectorIcon?> IconProperty = AvaloniaProperty.Register<XYTab, XyuiVectorIcon?>(nameof(Icon));
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<XYTab, bool>(nameof(IsSelected));
    public static readonly StyledProperty<bool> IsModifiedProperty = AvaloniaProperty.Register<XYTab, bool>(nameof(IsModified));
    public static readonly StyledProperty<bool> IsClosableProperty = AvaloniaProperty.Register<XYTab, bool>(nameof(IsClosable), true);
    public static readonly StyledProperty<bool> ShowSelectedAccentProperty = AvaloniaProperty.Register<XYTab, bool>(nameof(ShowSelectedAccent), true);
    public string Id { get => GetValue(IdProperty); set => SetValue(IdProperty, value); }
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public XyuiVectorIcon? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public bool IsModified { get => GetValue(IsModifiedProperty); set => SetValue(IsModifiedProperty, value); }
    public bool IsClosable { get => GetValue(IsClosableProperty); set => SetValue(IsClosableProperty, value); }
    public bool ShowSelectedAccent { get => GetValue(ShowSelectedAccentProperty); set => SetValue(ShowSelectedAccentProperty, value); }
    public event EventHandler? SelectionRequested;
    public event EventHandler? CloseRequested;
    public XYTab() { Classes.Add("xyui-tab"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (ReferenceEquals(change.Property, IdProperty) || ReferenceEquals(change.Property, LabelProperty) || ReferenceEquals(change.Property, IconProperty) || ReferenceEquals(change.Property, IsEnabledProperty) || ReferenceEquals(change.Property, IsSelectedProperty) ||
            ReferenceEquals(change.Property, IsModifiedProperty) || ReferenceEquals(change.Property, IsClosableProperty) ||
            ReferenceEquals(change.Property, ShowSelectedAccentProperty)) Build();
    }
    void Build()
    {
        Classes.Set("xyui-tab-selected", IsSelected);
        var close = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Clear, Size = XyuiIconSize.Tiny }, Classes = { "xyui-tab-close" }, IsVisible = IsClosable, IsHitTestVisible = IsClosable && IsSelected, Opacity = IsClosable && IsSelected ? 1 : 0 };
        close.AddHandler(InputElement.PointerPressedEvent, OnClosePointerPressed, RoutingStrategies.Tunnel); close.KeyDown += OnCloseKeyDown;
        var iconTrack = Icon is null ? 0 : XyuiComponentTokens.TabIconTrackWidth;
        var modifiedTrack = IsModified ? XyuiComponentTokens.TabModifiedTrackWidth : 0;
        var closeTrack = IsClosable ? XyuiComponentTokens.TabCloseHitTargetSize : 0;
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions($"{iconTrack},*,{modifiedTrack},{closeTrack}") };
        grid.Children.Add(new XYIcon { Icon = Icon ?? XyuiVectorIcon.Info, Size = XyuiIconSize.Tiny, IsVisible = Icon is not null, Classes = { "xyui-tab-icon" } });
        grid.Children.Add(new TextBlock { Text = Label, Classes = { "xyui-tab-label" }, VerticalAlignment = VerticalAlignment.Center, [Grid.ColumnProperty] = 1 });
        grid.Children.Add(new Border { Classes = { "xyui-tab-modified" }, IsVisible = IsModified, VerticalAlignment = VerticalAlignment.Center, [Grid.ColumnProperty] = 2 });
        grid.Children.Add(close); Grid.SetColumn(close, 3);
        var accent = new Border { Classes = { "xyui-tab-accent" }, IsVisible = IsSelected && ShowSelectedAccent, IsHitTestVisible = false };
        grid.Children.Add(accent); Grid.SetColumnSpan(accent, 4);
        Child = grid;
        if (!_pointerHooked) { PointerPressed += OnPointerPressed; _pointerHooked = true; }
    }
}
