using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace XuanYu.Editor.UI;

// D4：UiWin 错误/警告弹窗实现。复用 UiWin.UnsavedDialog 的窗口构建风格，
// 不引入第三方 MessageBox 包；确定按钮关闭。
public sealed partial class UiWin : IEditorDialogService
{
    Task IEditorDialogService.ShowErrorAsync(string title, string message) =>
        ShowMessageDialog(title, message, "#fdeeee", "#a43f3f");

    Task IEditorDialogService.ShowWarningAsync(string title, string message) =>
        ShowMessageDialog(title, message, "#fff7df", "#8a6417");

    Task ShowMessageDialog(string title, string message, string background, string accent)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 480,
            Height = 240,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };
        var titleText = new TextBlock
        {
            Text = title,
            FontSize = 15,
            FontWeight = FontWeight.SemiBold,
            Foreground = Brush.Parse(accent),
            Margin = new Avalonia.Thickness(18, 16, 18, 4)
        };
        var body = new TextBlock
        {
            Text = message,
            FontSize = 14,
            Foreground = Brush.Parse("#243447"),
            TextWrapping = TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(18, 4, 18, 12)
        };
        var ok = new Button
        {
            Content = "确定",
            MinWidth = 76,
            MinHeight = 32,
            Padding = new Avalonia.Thickness(12, 4),
            Background = Brush.Parse("#e9f2ff"),
            BorderBrush = Brush.Parse("#94b9e8"),
            Foreground = Brush.Parse("#185aa6"),
            CornerRadius = new Avalonia.CornerRadius(5)
        };
        ok.Click += (_, _) => dialog.Close();
        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
            Margin = new Avalonia.Thickness(18, 0, 18, 18)
        };
        buttons.Children.Add(ok);
        var content = new DockPanel { Children = { titleText, body, buttons } };
        DockPanel.SetDock(titleText, Dock.Top);
        DockPanel.SetDock(body, Dock.Top);
        DockPanel.SetDock(buttons, Dock.Bottom);
        dialog.Content = new Border
        {
            Background = Brush.Parse(background),
            BorderBrush = Brush.Parse("#d5dfec"),
            BorderThickness = new Avalonia.Thickness(1),
            CornerRadius = new Avalonia.CornerRadius(10),
            Padding = new Avalonia.Thickness(2),
            Child = content
        };
        return dialog.ShowDialog(this);
    }
}
