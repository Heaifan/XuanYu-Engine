using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Editor.UI;

public sealed partial class UnsavedChangesConfirmationWindow : Window
{
    bool _completed;

    public UnsavedChangesConfirmationWindow()
    {
        InitializeComponent();
        Opened += (_, _) => SaveButton.Focus();
    }

    public static Task<string> ShowAsync(Window owner) =>
        new UnsavedChangesConfirmationWindow().ShowDialog<string>(owner);

    void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape) Complete("cancel");
        if (e.Key == Key.Enter) Complete("save");
    }

    void Save_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Complete("save");
    void Discard_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Complete("discard");
    void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Complete("cancel");

    void Complete(string result)
    {
        if (_completed) return;
        _completed = true;
        Close(result);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!_completed) Complete("cancel");
        base.OnClosing(e);
    }
}
