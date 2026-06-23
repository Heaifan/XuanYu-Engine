using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Bindings;
using AM = Avalonia.Media;

namespace FluidWarfare.Editor.Windows.Preferences;

sealed class EditorPreferencesBindingList
{
    readonly EditorPreferencesDraftHandler _draft;
    readonly Panel _container;
    readonly Func<string, string, Button> _createBtn;

    public EditorPreferencesBindingList(EditorPreferencesDraftHandler draft, Panel container,
        Func<string, string, Button> createBtnCallback)
    { _draft = draft; _container = container; _createBtn = createBtnCallback; }

    public event Action<string>? RestoreClicked;

    public void Repopulate(string? searchFilter = null)
    {
        if (_container is null) return;
        _container.Children.Clear();

        var cats = new[] { "全局", "视口导航", "标准视图", "Transform", "工具" };
        var actions = EditorInputActionCatalog.All.Where(a => a.IsUserConfigurable);

        if (!string.IsNullOrWhiteSpace(searchFilter))
        {
            var f = searchFilter.Trim().ToLowerInvariant();
            actions = actions.Where(a => a.DisplayName.ToLowerInvariant().Contains(f) || a.Id.ToLowerInvariant().Contains(f));
        }

        foreach (var cat in cats)
        {
            var ca = actions.Where(a => a.Category == cat).ToList();
            if (ca.Count == 0) continue;
            _container.Children.Add(new TextBlock { Text = cat, Foreground = new SolidColorBrush(AM.Color.Parse("#999")), Margin = new Thickness(0, 10, 0, 2), FontSize = 13, FontWeight = FontWeight.Bold });
            foreach (var def in ca)
                _container.Children.Add(CreateRow(def));
        }
    }

    Border CreateRow(EditorInputActionDefinition def)
    {
        var row = new Border { Background = new SolidColorBrush(AM.Color.Parse("#2A2F36")), BorderBrush = new SolidColorBrush(AM.Color.Parse("#414852")), BorderThickness = new Thickness(0, 0, 0, 1), Padding = new Thickness(8, 4), Margin = new Thickness(0, 1) };
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("160,Auto,8,Auto,8,Auto") };
        grid.Children.Add(new TextBlock { Text = def.DisplayName, VerticalAlignment = VerticalAlignment.Center, Foreground = new SolidColorBrush(AM.Color.Parse("#DDD")), FontSize = 12 });
        Grid.SetColumn(grid.Children[^1], 0);
        var pb = _createBtn(def.Id, "primary");
        Grid.SetColumn(pb, 1); grid.Children.Add(pb);
        var sb = _createBtn(def.Id, "secondary");
        Grid.SetColumn(sb, 3); grid.Children.Add(sb);
        var rb = new Button { Content = "恢复", Tag = def.Id, Padding = new Thickness(6, 2), Background = new SolidColorBrush(Colors.Transparent), BorderThickness = new Thickness(0), Foreground = new SolidColorBrush(AM.Color.Parse("#888")), FontSize = 11, IsVisible = _draft.HasOverride(def.Id) };
        rb.Click += OnRestore;
        Grid.SetColumn(rb, 5); grid.Children.Add(rb);
        row.Child = grid;
        return row;
    }

    void OnRestore(object? sender, RoutedEventArgs e) { if (sender is Button btn && btn.Tag is string id) RestoreClicked?.Invoke(id); }

    public Button CreateBindingButton(string actionId, string slot)
    {
        var g = _draft.GetEffective(actionId, slot);
        var btn = new Button
        {
            Content = EditorPreferencesFormatText.FormatGestureText(g), Tag = (actionId, slot),
            MinWidth = slot == "primary" ? 130 : 110, Padding = new Thickness(8, 3),
            Background = new SolidColorBrush(AM.Color.Parse("#353B44")),
            BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(AM.Color.Parse("#555")),
            Foreground = new SolidColorBrush(g is null ? AM.Color.Parse("#666") : AM.Color.Parse("#CCC")),
            FontSize = 11
        };
        return btn;
    }
}
