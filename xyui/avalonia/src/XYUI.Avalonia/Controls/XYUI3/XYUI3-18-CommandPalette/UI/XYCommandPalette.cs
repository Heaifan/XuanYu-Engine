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
    Control Build()
    {
        var root = new Grid { Width = 650, Height = 360 }; var surface = new Border { Margin = new Thickness(18, 14, 18, 20), Classes = { "xyui-palette-surface" } }; var body = new Grid(); var details = new StackPanel { Margin = new Thickness(326, 70, 10, 10), Spacing = 8, Classes = { "xyui-palette-details" } }; details.Children.Add(new TextBlock { Text = "创建道路", Classes = { "xyui-palette-detail-title" } }); details.Children.Add(new TextBlock { Text = "在当前地图数据集中创建道路对象。" }); details.Children.Add(new TextBlock { Text = "分类　地图命令" }); details.Children.Add(new TextBlock { Text = "快捷键　Ctrl+D" }); details.Children.Add(new TextBlock { Text = "Enter 执行" }); body.Children.Add(details); SearchBox.Margin = new Thickness(28, 24, 28, 0); SearchBox.HorizontalAlignment = HorizontalAlignment.Stretch; SearchBox.VerticalAlignment = VerticalAlignment.Top; body.Children.Add(SearchBox); _results.Margin = new Thickness(28, 72, 336, 16); body.Children.Add(_results); var divider = new Border { Width = 1, Margin = new Thickness(326, 70, 0, 16), Classes = { "xyui-palette-divider" } }; body.Children.Add(divider); _feedback.Classes.Add("xyui-palette-feedback"); body.Children.Add(_feedback); surface.Child = body; root.Children.Add(surface); return root;
    }
    void Refresh() { var query = SearchBox.Text?.Trim() ?? ""; FilteredCommands = Commands.Where(x => string.IsNullOrEmpty(query) || x.Label.Contains(query, StringComparison.OrdinalIgnoreCase) || x.Prefix == query).ToArray(); _selected = Math.Clamp(_selected, 0, Math.Max(0, FilteredCommands.Count - 1)); _results.Children.Clear(); foreach (var item in FilteredCommands) { var button = new Button { Content = item.Label, Height = 30, HorizontalContentAlignment = HorizontalAlignment.Left, Classes = { "xyui-palette-result" } }; button.Click += (_, _) => Execute(item); _results.Children.Add(button); } }
    void OnKeyDown(object? sender, KeyEventArgs e) { if (e.Key == Key.Down) _selected = Math.Min(_selected + 1, FilteredCommands.Count - 1); else if (e.Key == Key.Up) _selected = Math.Max(0, _selected - 1); else if (e.Key == Key.Enter && FilteredCommands.Count > 0) Execute(FilteredCommands[_selected]); else if (e.Key == Key.Escape) { if (!string.IsNullOrEmpty(SearchBox.Text)) SearchBox.Text = ""; else IsVisible = false; } }
    void Execute(XYPaletteCommand item) { ExecutedText = $"Executed: {item.Label}"; _feedback.Text = ExecutedText; ExecuteRequested?.Invoke(this, item); }
}
