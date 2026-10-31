using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input.Platform;

namespace XuanYu.Editor.UI;

public partial class InspectorReadOnlyValuePresenter : UserControl
{
    public InspectorReadOnlyValuePresenter() => InitializeComponent();

    async void CopyValue_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is InspectorPropertyRow row && TopLevel.GetTopLevel(this)?.Clipboard is { } clipboard)
            await clipboard.SetTextAsync(row.Value);
    }
}
