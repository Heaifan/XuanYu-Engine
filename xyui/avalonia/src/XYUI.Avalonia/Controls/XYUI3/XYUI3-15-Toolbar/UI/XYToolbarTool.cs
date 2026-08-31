using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed class XYToolbarTool : ContentControl
{
    bool _showLabel = true;
    public string ToolId { get; set; } = Guid.NewGuid().ToString("N");
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYToolbarTool, string>(nameof(Label), "");
    public static readonly StyledProperty<XyuiVectorIcon?> IconProperty = AvaloniaProperty.Register<XYToolbarTool, XyuiVectorIcon?>(nameof(Icon));
    public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<XYToolbarTool, bool>(nameof(IsSelected));
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public XyuiVectorIcon? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public bool ShowLabel { get => _showLabel; internal set { if (_showLabel == value) return; _showLabel = value; Sync(); } }
    public event EventHandler? SelectionRequested;
    public XYIconButton Button { get; }
    public XYToolbarTool() { Classes.Add("xyui-toolbar-tool"); Button = new XYIconButton(); Button.Click += (_, _) => SelectionRequested?.Invoke(this, EventArgs.Empty); Content = Button; Sync(); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e) { base.OnPropertyChanged(e); if (e.Property is var p && (p == LabelProperty || p == IconProperty || p == IsSelectedProperty)) Sync(); }
    void Sync() { Button.IsSelected = IsSelected; Button.Content = Icon is { } i ? (Control)(ShowLabel ? new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, Children = { new XYIcon { Icon = i, Size = XyuiIconSize.Small }, new TextBlock { Text = Label, VerticalAlignment = VerticalAlignment.Center } } } : new XYIcon { Icon = i, Size = XyuiIconSize.Small }) : new TextBlock { Text = Label }; }
}
