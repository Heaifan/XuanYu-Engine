using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;

namespace XuanYu.Engine.Editor.Windows.Panels.Logging;

public sealed partial class LogPanel : UserControl
{
    private readonly List<string> _messages = [];
    private TextBox? _logTextBox;

    public LogPanel()
    {
        AvaloniaXamlLoader.Load(this);
        _logTextBox = this.FindControl<TextBox>("LogTextBox");
    }

    public void SetLogMessages(IEnumerable<string> messages)
    {
        _messages.Clear();

        foreach (var message in messages)
        {
            _messages.Add(message);
        }

        RefreshLogText();
    }

    public void AppendLogMessage(string message)
    {
        _messages.Add(message);
        RefreshLogText();
    }

    private void RefreshLogText()
    {
        _logTextBox ??= this.FindControl<TextBox>("LogTextBox");

        if (_logTextBox is null)
        {
            return;
        }

        var keepSelection = _logTextBox.IsFocused && _logTextBox.SelectionStart != _logTextBox.SelectionEnd;
        var start = _logTextBox.SelectionStart;
        var end = _logTextBox.SelectionEnd;
        var text = string.Join(Environment.NewLine, _messages);

        _logTextBox.Text = text;

        if (keepSelection)
        {
            var max = text.Length;
            _logTextBox.SelectionStart = Math.Clamp(start, 0, max);
            _logTextBox.SelectionEnd = Math.Clamp(end, 0, max);
            return;
        }

        _logTextBox.CaretIndex = text.Length;
    }

    private async void OnCopySelectionClicked(object? sender, RoutedEventArgs e)
    {
        _logTextBox ??= this.FindControl<TextBox>("LogTextBox");
        var text = _logTextBox?.SelectedText;
        if (string.IsNullOrEmpty(text))
        {
            text = _logTextBox?.Text;
        }

        await CopyToClipboard(text);
    }

    private async void OnCopyAllClicked(object? sender, RoutedEventArgs e)
    {
        _logTextBox ??= this.FindControl<TextBox>("LogTextBox");
        await CopyToClipboard(_logTextBox?.Text);
    }

    private async Task CopyToClipboard(string? text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is not null)
        {
            await clipboard.SetTextAsync(text);
        }
    }
}
