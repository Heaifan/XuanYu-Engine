using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYCommandPaletteItem : Border
{
    public XYPaletteCommand Command { get; }
    public bool IsSelected { get => Classes.Contains("xyui-palette-result-selected"); set => Classes.Set("xyui-palette-result-selected", value); }
    public event EventHandler? Invoked;
    public event EventHandler? PreviewRequested;
    public XYCommandPaletteItem(XYPaletteCommand command)
    {
        Command = command; Classes.Add("xyui-palette-result"); Child = new TextBlock { Text = command.Label, VerticalAlignment = VerticalAlignment.Center, Classes = { "xyui-palette-result-label" } };
        IsEnabled = command.IsEnabled; PointerEntered += (_, _) => { Classes.Set("xyui-palette-result-hover", true); PreviewRequested?.Invoke(this, EventArgs.Empty); }; PointerExited += (_, _) => Classes.Set("xyui-palette-result-hover", false); PointerPressed += OnPointerPressed;
    }
    void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    { if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && IsEnabled) { Invoked?.Invoke(this, EventArgs.Empty); e.Handled = true; } }
}
