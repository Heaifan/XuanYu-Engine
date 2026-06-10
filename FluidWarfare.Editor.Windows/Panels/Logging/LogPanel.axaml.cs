using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace FluidWarfare.Editor.Windows.Panels.Logging;

public sealed partial class LogPanel : UserControl
{
    private ItemsControl? _logList;

    public LogPanel()
    {
        AvaloniaXamlLoader.Load(this);
        _logList = this.FindControl<ItemsControl>("LogList");
    }

    public void SetLogMessages(IEnumerable<string> messages)
    {
        _logList ??= this.FindControl<ItemsControl>("LogList");

        if (_logList is not null)
        {
            _logList.ItemsSource = messages.ToArray();
        }
    }
}
