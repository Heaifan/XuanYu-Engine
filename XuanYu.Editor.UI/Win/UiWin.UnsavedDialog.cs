using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    async Task<bool> ConfirmUnsavedBeforeContinue(UiVm vm)
    {
        if (!vm.IsSceneDirty) return true;
        var choice = await ShowUnsavedDialog();
        if (choice == "cancel") return false;
        if (choice == "discard") return true;
        return await SaveExistingOrPick(vm);
    }

    Task<string?> ShowUnsavedDialog()
    {
        var dialog = new Window
        {
            Title = "未保存的场景",
            Width = 420,
            Height = 210,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            WindowDecorations = Avalonia.Controls.WindowDecorations.None,
            CanResize = false,
            Background = Brushes.Transparent
        };
        var text = new TextBlock
        {
            Text = "当前场景有未保存修改。",
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
            Foreground = Brush.Parse("#243447"),
            Margin = new Avalonia.Thickness(18, 16, 18, 4)
        };
        var hint = new TextBlock
        {
            Text = "继续操作前请选择保存、放弃修改或取消。",
            Foreground = Brush.Parse("#64748b"),
            Margin = new Avalonia.Thickness(18, 0, 18, 12)
        };
        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Margin = new Avalonia.Thickness(18, 0, 18, 18)
        };
        buttons.Children.Add(Button("保存", "save", dialog, true));
        buttons.Children.Add(Button("不保存", "discard", dialog));
        buttons.Children.Add(Button("取消", "cancel", dialog));
        var title = new TextBlock
        {
            Text = "未保存的场景",
            FontSize = 13,
            Foreground = Brush.Parse("#64748b"),
            Margin = new Avalonia.Thickness(18, 14, 18, 0)
        };
        var body = new DockPanel { Children = { title, text, hint, buttons } };
        DockPanel.SetDock(title, Dock.Top);
        DockPanel.SetDock(text, Dock.Top);
        DockPanel.SetDock(hint, Dock.Top);
        DockPanel.SetDock(buttons, Dock.Bottom);
        dialog.Content = new Border
        {
            Background = Brush.Parse("#fbfdff"),
            BorderBrush = Brush.Parse("#d5dfec"),
            BorderThickness = new Avalonia.Thickness(1),
            CornerRadius = new Avalonia.CornerRadius(10),
            Padding = new Avalonia.Thickness(2),
            Child = body
        };
        return dialog.ShowDialog<string?>(this);
    }

    static Button Button(string text, string result, Window owner, bool primary = false)
    {
        var button = new Button
        {
            Content = text,
            MinWidth = 76,
            MinHeight = 32,
            Padding = new Avalonia.Thickness(12, 4),
            Background = Brush.Parse(primary ? "#e9f2ff" : "#ffffff"),
            BorderBrush = Brush.Parse(primary ? "#94b9e8" : "#d5dfec"),
            BorderThickness = new Avalonia.Thickness(1),
            Foreground = Brush.Parse(primary ? "#185aa6" : "#2f3d52"),
            CornerRadius = new Avalonia.CornerRadius(5)
        };
        button.Click += (_, _) => owner.Close(result);
        return button;
    }
}
