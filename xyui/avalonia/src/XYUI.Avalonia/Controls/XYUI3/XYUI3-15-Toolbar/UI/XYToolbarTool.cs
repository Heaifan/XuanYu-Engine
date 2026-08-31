using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYToolbarTool : ContentControl
{
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYToolbarTool, string>(nameof(Label), "");
    public static readonly StyledProperty<XyuiVectorIcon?> IconProperty = AvaloniaProperty.Register<XYToolbarTool, XyuiVectorIcon?>(nameof(Icon));
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<XYToolbarTool, bool>(nameof(IsSelected));
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public XyuiVectorIcon? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public XYIconButton Button { get; }
    public XYToolbarTool() { Classes.Add("xyui-toolbar-tool"); Button = new XYIconButton(); Content = Button; Sync(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property is var p && (p == LabelProperty || p == IconProperty || p == IsSelectedProperty)) Sync(); }
    void Sync() { Button.IsSelected = IsSelected; Button.Content = Icon is { } i ? new XYIcon { Icon = i, Size = XyuiIconSize.Small } : new TextBlock { Text = Label }; }
}
