using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYTab : Border
{
    bool _pointerHooked;
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYTab, string>(nameof(Label), "");
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<XYTab, bool>(nameof(IsSelected));
    public static readonly StyledProperty<bool> IsModifiedProperty = AvaloniaProperty.Register<XYTab, bool>(nameof(IsModified));
    public static readonly StyledProperty<bool> IsClosableProperty = AvaloniaProperty.Register<XYTab, bool>(nameof(IsClosable), true);
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public bool IsModified { get => GetValue(IsModifiedProperty); set => SetValue(IsModifiedProperty, value); }
    public bool IsClosable { get => GetValue(IsClosableProperty); set => SetValue(IsClosableProperty, value); }
    public event EventHandler? SelectionRequested;
    public event EventHandler? CloseRequested;
    public XYTab() { Classes.Add("xyui-tab"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (change.Property != ChildProperty) Build(); }
    void Build()
    {
        Classes.Set("xyui-tab-selected", IsSelected);
        var close = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Clear, Size = XyuiIconSize.Tiny }, Classes = { "xyui-tab-close" }, IsHitTestVisible = IsClosable && IsSelected, Opacity = IsClosable && IsSelected ? 1 : 0 };
        close.Click += (_, _) => RequestClose();
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto,18,Auto") };
        grid.Children.Add(new TextBlock { Text = Label, Classes = { "xyui-tab-label" } });
        grid.Children.Add(new Border { Classes = { "xyui-tab-modified" }, IsVisible = IsModified, [Grid.ColumnProperty] = 1 });
        grid.Children.Add(close);
        Grid.SetColumn(close, 2);
        var divider = new Border { Classes = { "xyui-tab-divider" }, Height = 22, Width = 1, VerticalAlignment = VerticalAlignment.Center };
        grid.Children.Add(divider); Grid.SetColumn(divider, 3);
        var accent = new Border { Classes = { "xyui-tab-accent" }, IsVisible = IsSelected, IsHitTestVisible = false };
        grid.Children.Add(accent); Grid.SetColumnSpan(accent, 4);
        Child = grid;
        if (!_pointerHooked) { PointerPressed += OnPointerPressed; _pointerHooked = true; }
    }
}
