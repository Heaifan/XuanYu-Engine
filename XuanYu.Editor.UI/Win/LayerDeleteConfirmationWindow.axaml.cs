using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

// MAP-DATA-A-R2-F2-F2-F1：独立 TopLevel 避开 Vulkan NativeControlHost 的 airspace。
public sealed partial class LayerDeleteConfirmationWindow : Window
{
    bool _completed;

    public LayerDeleteConfirmationWindow() : this("", "", "") { }

    public LayerDeleteConfirmationWindow(string layerName, string layerType, string intent)
    {
        InitializeComponent();
        Message.Text = $"{intent}\n\n目标：{layerName}（{layerType}）";
        Opened += (_, _) => CancelButton.Focus();
    }

    public static Task<bool> ShowAsync(Window owner, string layerName, string layerType, string intent) =>
        new LayerDeleteConfirmationWindow(layerName, layerType, intent).ShowDialog<bool>(owner);

    void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Complete(false);
        if (e.Key == Key.Enter) Complete(false);
    }

    void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Complete(false);

    void Delete_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Complete(true);

    void Complete(bool result)
    {
        if (_completed) return;
        _completed = true;
        Close(result);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!_completed) Complete(false);
        base.OnClosing(e);
    }
}
