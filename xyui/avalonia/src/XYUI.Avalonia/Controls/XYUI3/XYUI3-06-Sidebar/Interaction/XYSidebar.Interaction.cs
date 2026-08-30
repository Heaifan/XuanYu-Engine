using Avalonia;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYSidebar
{
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) { base.OnPropertyChanged(change); if (change.Property == IsCollapsedProperty) Build(); }
    static XYNavigationItem Clone(XYNavigationItem item) => new() { Id = item.Id, Label = item.Label, Icon = item.Icon, IsSelected = item.IsSelected };
}
