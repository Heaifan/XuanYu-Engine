using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYCommandPalette
{
    XYMenu CreateScopeMenu() => new(ScopeItem("全部", null), ScopeItem("命令", XYPaletteCommandType.Command), ScopeItem("对象", XYPaletteCommandType.Object), ScopeItem("导航", XYPaletteCommandType.Navigation), ScopeItem("设置", XYPaletteCommandType.Setting));
    XYMenuItem ScopeItem(string label, XYPaletteCommandType? scope) { var item = new XYMenuItem { Label = label }; item.SelectionRequested += (_, _) => { ScopeFilter = scope; SearchBox.IsFilterOpen = false; ScopeMenu.Close(); Refresh(); }; return item; }
    void OnFilterRequested(object? sender, EventArgs e) { if (SearchBox.IsFilterOpen) { ScopeMenu.ApplyOverlayStyling(); ScopeMenu.Open(); } else ScopeMenu.Close(); }

    void Refresh()
    {
        var query = ParseQuery(SearchBox.Text?.Trim() ?? ""); var scope = query.Scope ?? ScopeFilter; var source = string.IsNullOrEmpty(query.Search) ? RecentItems : Commands;
        FilteredCommands = source.Where(x => !scope.HasValue || x.Type == scope).Where(x => string.IsNullOrEmpty(query.Search) || x.Label.Contains(query.Search, StringComparison.OrdinalIgnoreCase) || x.Keywords.Any(k => k.Contains(query.Search, StringComparison.OrdinalIgnoreCase))).ToArray();
        _selected = FilteredCommands.Count == 0 ? -1 : Math.Clamp(_selected < 0 ? 0 : _selected, 0, FilteredCommands.Count - 1); _results.Children.Clear();
        for (var i = 0; i < FilteredCommands.Count; i++) { var item = new XYCommandPaletteItem(FilteredCommands[i]) { IsSelected = i == _selected }; var index = i; item.PreviewRequested += (_, _) => Select(index); item.Invoked += (_, _) => { Select(index); Execute(item.Command); }; _results.Children.Add(item); }
        UpdateDetails(_selected < 0 ? null : FilteredCommands[_selected]);
    }
    void Select(int index) { if (index < 0 || index >= FilteredCommands.Count) return; _selected = index; foreach (var item in _results.Children.OfType<XYCommandPaletteItem>()) item.IsSelected = ReferenceEquals(item.Command, FilteredCommands[index]); UpdateDetails(FilteredCommands[index]); _results.Children.OfType<XYCommandPaletteItem>().ElementAt(index).BringIntoView(); }
    void Execute(XYPaletteCommand item) { if (!item.IsEnabled) return; ExecuteRequested?.Invoke(this, item); Close(); }
    void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Down) { Select(Math.Min(_selected + 1, FilteredCommands.Count - 1)); e.Handled = true; }
        else if (e.Key == Key.Up) { Select(Math.Max(0, _selected - 1)); e.Handled = true; }
        else if (e.Key == Key.Enter) { if (_selected >= 0) Execute(FilteredCommands[_selected]); e.Handled = true; }
        else if (e.Key == Key.Escape) { if (!string.IsNullOrEmpty(SearchBox.Text)) SearchBox.Text = ""; else Close(); e.Handled = true; }
    }
    static (XYPaletteCommandType? Scope, string Search) ParseQuery(string value)
    {
        if (value.Length > 1 && value[1] == ' ') return (value[0] switch { '>' => XYPaletteCommandType.Command, '@' => XYPaletteCommandType.Object, '#' => XYPaletteCommandType.Navigation, ':' => XYPaletteCommandType.Setting, _ => null }, value[2..]);
        return (null, value);
    }
}
