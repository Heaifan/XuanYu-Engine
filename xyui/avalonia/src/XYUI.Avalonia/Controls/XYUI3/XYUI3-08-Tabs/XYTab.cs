using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYTab : Border
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
    public event EventHandler? Selected;
    public event EventHandler? CloseRequested;
    public XYTab() { Classes.Add("xyui-tab"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (change.Property is not ChildProperty) Build(); }
    void Build()
    {
        Classes.Set("xyui-tab-selected", IsSelected);
        var close = new Button { Content = "×", Classes = { "xyui-tab-close" }, IsVisible = IsClosable && IsSelected };
        close.Click += (_, _) => CloseRequested?.Invoke(this, EventArgs.Empty);
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto,Auto") };
        grid.Children.Add(new TextBlock { Text = Label, Classes = { "xyui-tab-label" } });
        grid.Children.Add(new Border { Classes = { "xyui-tab-modified" }, IsVisible = IsModified, [Grid.ColumnProperty] = 1 });
        grid.Children.Add(close);
        Grid.SetColumn(close, 2);
        grid.Children.Add(new Border { Classes = { "xyui-tab-accent" }, IsVisible = IsSelected });
        Child = grid;
        if (!_pointerHooked) { PointerPressed += (_, _) => { IsSelected = true; Selected?.Invoke(this, EventArgs.Empty); }; _pointerHooked = true; }
    }
}
