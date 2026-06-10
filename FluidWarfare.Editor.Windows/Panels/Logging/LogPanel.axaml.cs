using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;

namespace FluidWarfare.Editor.Windows.Panels.Logging;

public sealed partial class LogPanel : UserControl
{
    private readonly ObservableCollection<string> _messages = [];
    private ItemsControl? _logList;

    public LogPanel()
    {
        AvaloniaXamlLoader.Load(this);
        _logList = this.FindControl<ItemsControl>("LogList");
        if (_logList is not null)
        {
            _logList.ItemsSource = _messages;
        }
    }

    public void SetLogMessages(IEnumerable<string> messages)
    {
        _logList ??= this.FindControl<ItemsControl>("LogList");

        if (_logList is not null)
        {
            _logList.ItemsSource = _messages;
        }

        _messages.Clear();

        foreach (var message in messages)
        {
            _messages.Add(message);
        }
    }

    public void AppendLogMessage(string message)
    {
        _messages.Add(message);
    }
}
