using Avalonia.Controls;
using Avalonia.Input.Platform;

namespace XuanYu.Editor.UI;

public partial class LogDetailPanel : UserControl
{
    public LogDetailPanel()
    {
        InitializeComponent();
    }

    async void CopyDetail_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not LogPresenter presenter || string.IsNullOrWhiteSpace(presenter.SelectedLogClipboardText)) return;
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is not null) await clipboard.SetTextAsync(presenter.SelectedLogClipboardText);
    }
}
