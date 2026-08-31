using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed record XYPaletteCommand(string Label, string? Prefix = null);

public sealed class XYCommandPalette : Border
{
    readonly StackPanel _results = new(); readonly TextBlock _feedback = new(); readonly TextBlock _detailTitle = new(); readonly TextBlock _detailDescription = new(); int _selected;
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
        var root = new Grid { Width = 650, Height = 360 }; var surface = new Border { Margin = new Thickness(18, 14, 18, 20), Classes = { "xyui-palette-surface" } }; var body = new Grid(); var details = new StackPanel { Margin = new Thickness(326, 70, 10, 10), Spacing = 8, Classes = { "xyui-palette-details" } }; _detailTitle.Classes.Add("xyui-palette-detail-title"); details.Children.Add(_detailTitle); details.Children.Add(_detailDescription); details.Children.Add(new TextBlock { Text = "分类　地图命令" }); details.Children.Add(new TextBlock { Text = "快捷键　Ctrl+D" }); details.Children.Add(new TextBlock { Text = "Enter 执行" }); body.Children.Add(details); SearchBox.Margin = new Thickness(28, 24, 28, 0); SearchBox.HorizontalAlignment = HorizontalAlignment.Stretch; SearchBox.VerticalAlignment = VerticalAlignment.Top; body.Children.Add(SearchBox); _results.Margin = new Thickness(28, 72, 336, 16); body.Children.Add(_results); var divider = new Border { Width = 1, Margin = new Thickness(326, 70, 0, 16), Classes = { "xyui-palette-divider" } }; body.Children.Add(divider); _feedback.Classes.Add("xyui-palette-feedback"); _feedback.Margin = new Thickness(326, 300, 10, 0); body.Children.Add(_feedback); surface.Child = body; root.Children.Add(surface); return root;
    }
    void Refresh() { var raw = SearchBox.Text?.Trim() ?? ""; var query = ParseQuery(raw); FilteredCommands = string.IsNullOrEmpty(query.Search) ? Commands.Take(4).ToArray() : Commands.Where(x => x.Label.Contains(query.Search, StringComparison.OrdinalIgnoreCase) && (string.IsNullOrEmpty(query.Scope) || x.Prefix == query.Scope)).ToArray(); _selected = Math.Clamp(_selected, 0, Math.Max(0, FilteredCommands.Count - 1)); _results.Children.Clear(); for (var i = 0; i < FilteredCommands.Count; i++) { var item = FilteredCommands[i]; var button = new Button { Content = item.Label, Height = 30, HorizontalContentAlignment = HorizontalAlignment.Left, Classes = { "xyui-palette-result" } }; button.Classes.Set("xyui-palette-result-selected", i == _selected); var index = i; button.Click += (_, _) => { _selected = index; Refresh(); Execute(item); }; _results.Children.Add(button); } var selected = FilteredCommands.Count == 0 ? null : FilteredCommands[_selected]; _detailTitle.Text = selected?.Label ?? "无匹配命令"; _detailDescription.Text = selected is null ? "请输入命令或搜索词。" : $"执行“{selected.Label}”命令。"; }
    void OnKeyDown(object? sender, KeyEventArgs e) { if (e.Key == Key.Down) { _selected = Math.Min(_selected + 1, FilteredCommands.Count - 1); Refresh(); } else if (e.Key == Key.Up) { _selected = Math.Max(0, _selected - 1); Refresh(); } else if (e.Key == Key.Enter && FilteredCommands.Count > 0) Execute(FilteredCommands[_selected]); else if (e.Key == Key.Escape) { if (!string.IsNullOrEmpty(SearchBox.Text)) SearchBox.Text = ""; else IsVisible = false; } }
    static (string Scope, string Search) ParseQuery(string value) { if (value.Length > 1 && value[1] == ' ') return (value[..1], value[2..]); return ("", value); }
    void Execute(XYPaletteCommand item) { ExecutedText = $"Executed: {item.Label}"; _feedback.Text = ExecutedText; ExecuteRequested?.Invoke(this, item); }
}
