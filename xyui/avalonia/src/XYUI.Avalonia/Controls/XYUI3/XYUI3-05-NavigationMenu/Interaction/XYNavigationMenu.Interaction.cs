namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationMenu
{
    public event EventHandler<XYNavigationItem>? SelectionChanged;
    void OnSelected(object? sender, EventArgs e)
    {
        if (sender is not XYNavigationItem selected) return;
        if (_state is not null) _state.Select(selected.Id); else SelectedId = selected.Id;
        foreach (var item in Groups.SelectMany(x => x.Items)) item.IsSelected = ReferenceEquals(item, selected);
        SelectionChanged?.Invoke(this, selected);
    }
}
