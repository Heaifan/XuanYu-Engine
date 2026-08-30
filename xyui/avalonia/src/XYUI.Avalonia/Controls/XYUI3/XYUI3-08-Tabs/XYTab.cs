using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYTab : Border
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYTab, string>(nameof(Label), "");
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<XYTab, bool>(nameof(IsSelected));
    public static readonly StyledProperty<bool> IsModifiedProperty = AvaloniaProperty.Register<XYTab, bool>(nameof(IsModified));
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public bool IsModified { get => GetValue(IsModifiedProperty); set => SetValue(IsModifiedProperty, value); }
    public event EventHandler? Selected;
    public XYTab() { Classes.Add("xyui-tab"); Build(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (change.Property != ChildProperty) Build(); }
    void Build() { Classes.Set("xyui-tab-selected", IsSelected); Child = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto,Auto"), Children = { new TextBlock { Text = Label, Classes = { "xyui-tab-label" } }, new Border { Classes = { "xyui-tab-modified" }, IsVisible = IsModified, [Grid.ColumnProperty] = 1 }, new Button { Content = "×", Classes = { "xyui-tab-close" }, IsVisible = IsSelected, [Grid.ColumnProperty] = 2 }, new Border { Classes = { "xyui-tab-accent" }, IsVisible = IsSelected } } }; PointerPressed += (_, _) => { IsSelected = true; Selected?.Invoke(this, EventArgs.Empty); }; }
}
