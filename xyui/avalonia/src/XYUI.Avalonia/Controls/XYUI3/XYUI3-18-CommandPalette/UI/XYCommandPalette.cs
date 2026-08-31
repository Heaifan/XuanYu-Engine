using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed record XYPaletteCommand(string Label, string? Prefix = null);

public sealed class XYCommandPalette : Border
{
    readonly StackPanel _results = new(); readonly TextBlock _feedback = new(); int _selected;
    public XYSearchField SearchBox { get; } = new() { Placeholder = "输入命令或搜索...", Height = 34 };
    public IReadOnlyList<XYPaletteCommand> Commands { get; }
    public IReadOnlyList<XYPaletteCommand> FilteredCommands { get; private set; } = [];
    public string? ExecutedText { get; private set; }
    public event EventHandler<XYPaletteCommand>? ExecuteRequested;
    public XYCommandPalette(params XYPaletteCommand[] commands)
    {
        Commands = commands; Classes.Add("xyui-command-palette"); SearchBox.TextChanged += (_, _) => Refresh(); SearchBox.KeyDown += OnKeyDown; Child = Build(); Refresh();
    }
    Control Build() { var panel = new StackPanel { Spacing = 8 }; panel.Children.Add(SearchBox); panel.Children.Add(_results); _feedback.Classes.Add("xyui-palette-feedback"); panel.Children.Add(_feedback); return panel; }
    void Refresh() { var query = SearchBox.Text?.Trim() ?? ""; FilteredCommands = Commands.Where(x => string.IsNullOrEmpty(query) || x.Label.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Prefix == query).ToArray(); _selected = Math.Clamp(_selected, 0, Math.Max(0, FilteredCommands.Count - 1)); _results.Children.Clear(); foreach (var item in FilteredCommands) { var button = new Button { Content = item.Label, Height = 30, HorizontalContentAlignment = HorizontalAlignment.Left, Classes = { "xyui-palette-result" } }; button.Click += (_, _) => Execute(item); _results.Children.Add(button); } }
    void OnKeyDown(object? sender, KeyEventArgs e) { if (e.Key == Key.Down) _selected = Math.Min(_selected + 1, FilteredCommands.Count - 1); else if (e.Key == Key.Up) _selected = Math.Max(0, _selected - 1); else if (e.Key == Key.Enter && FilteredCommands.Count > 0) Execute(FilteredCommands[_selected]); else if (e.Key == Key.Escape) { if (!string.IsNullOrEmpty(SearchBox.Text)) SearchBox.Text = ""; else IsVisible = false; } }
    void Execute(XYPaletteCommand item) { ExecutedText = $"Executed: {item.Label}"; _feedback.Text = ExecutedText; ExecuteRequested?.Invoke(this, item); }
}
