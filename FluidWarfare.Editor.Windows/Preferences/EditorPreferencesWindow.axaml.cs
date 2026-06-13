using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Bindings;
using AM = Avalonia.Media;

namespace FluidWarfare.Editor.Windows.Preferences;

public partial class EditorPreferencesWindow : Window
{
    public EditorPreferencesWindow()
    {
        InitializeComponent();
        Title = "偏好设置";
        PopulateBindings();
    }

    private void PopulateBindings()
    {
        if (BindingsContainer is null) return;
        BindingsContainer.Children.Clear();

        var categories = new[] { "全局", "视口导航", "标准视图", "Transform", "工具" };

        foreach (var cat in categories)
        {
            var catActions = EditorInputActionCatalog.All
                .Where(a => a.IsUserConfigurable && a.Category == cat);
            if (!catActions.Any()) continue;

            BindingsContainer.Children.Add(new TextBlock
            {
                Text = cat,
                Foreground = new SolidColorBrush(AM.Color.Parse("#999")),
                Margin = new Thickness(0, 12, 0, 4),
                FontSize = 13
            });

            foreach (var def in catActions)
            {
                BindingsContainer.Children.Add(CreateBindingRow(def));
            }
        }
    }

    private static Border CreateBindingRow(EditorInputActionDefinition def)
    {
        var primaryText = GetDefaultPrimaryText(def.Id);
        var secondaryText = "";

        var row = new Border
        {
            Background = new SolidColorBrush(AM.Color.Parse("#2A2F36")),
            BorderBrush = new SolidColorBrush(AM.Color.Parse("#414852")),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(8, 6),
            Margin = new Thickness(0, 1)
        };

        var panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };

        panel.Children.Add(new TextBlock
        {
            Text = def.DisplayName,
            Width = 160,
            Foreground = new SolidColorBrush(AM.Color.Parse("#DDD")),
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        });

        panel.Children.Add(new Button
        {
            Content = primaryText,
            MinWidth = 120,
            Padding = new Thickness(8, 4),
            Background = new SolidColorBrush(AM.Color.Parse("#353B44")),
            BorderThickness = new Thickness(1),
            BorderBrush = new SolidColorBrush(AM.Color.Parse("#555")),
            Foreground = new SolidColorBrush(AM.Color.Parse("#CCC"))
        });

        panel.Children.Add(new Button
        {
            Content = secondaryText,
            MinWidth = 100,
            Padding = new Thickness(8, 4),
            Background = new SolidColorBrush(AM.Color.Parse("#353B44")),
            BorderThickness = new Thickness(1),
            BorderBrush = new SolidColorBrush(AM.Color.Parse("#555")),
            Foreground = new SolidColorBrush(AM.Color.Parse("#888"))
        });

        panel.Children.Add(new Button
        {
            Content = "恢复",
            Padding = new Thickness(8, 4),
            Background = new SolidColorBrush(Colors.Transparent),
            BorderThickness = new Thickness(0),
            Foreground = new SolidColorBrush(AM.Color.Parse("#888")),
            FontSize = 11
        });

        row.Child = panel;
        return row;
    }

    private static string GetDefaultPrimaryText(string actionId)
    {
        var action = EditorInputActionCatalog.FindById(actionId);
        if (action is null) return "未绑定";
        // Return a reasonable display hint
        return actionId switch
        {
            "viewport.orbit" => "中键拖动",
            "viewport.pan" => "Shift+中键拖动",
            "viewport.dolly" => "Ctrl+中键拖动",
            "viewport.zoom" => "滚轮",
            "viewport.frame_all" => "Home",
            "viewport.frame_selected" => "小键盘句点",
            "viewport.toggle_projection" => "小键盘5",
            "viewport.view_front" => "小键盘1",
            "viewport.view_back" => "Ctrl+小键盘1",
            "viewport.view_right" => "小键盘3",
            "viewport.view_left" => "Ctrl+小键盘3",
            "viewport.view_top" => "小键盘7",
            "viewport.view_bottom" => "Ctrl+小键盘7",
            "editor.open_preferences" => "Ctrl+,",
            "tool.cancel_current" => "Esc",
            "transform.apply" => "Enter",
            "transform.reset_draft" => "Esc",
            _ => "未绑定"
        };
    }

    private void OnRestoreAllClicked(object? sender, RoutedEventArgs e)
    {
        PopulateBindings();
    }
}
