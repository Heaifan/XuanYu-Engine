using Avalonia.Controls;
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

        _logTextBox.Text = string.Join(Environment.NewLine, _messages);
        _logTextBox.CaretIndex = _logTextBox.Text?.Length ?? 0;
    }
}
