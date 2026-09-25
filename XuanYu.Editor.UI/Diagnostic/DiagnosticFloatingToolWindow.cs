using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public sealed class DiagnosticFloatingToolWindow : Window
{
    Control? _dragSurface;
    Action? _dragStarted, _dragEnded;

    public DiagnosticFloatingToolWindow(Window owner)
    {
        Owner = owner;
        WindowDecorations = WindowDecorations.None;
        CanResize = false;
        ShowInTaskbar = false;
        WindowStartupLocation = WindowStartupLocation.Manual;
        SizeToContent = SizeToContent.WidthAndHeight;
        ShowActivated = false;
    }

    public void SetDragSurface(Control surface, Action started, Action ended)
    {
        if (_dragSurface is not null) DetachDragSurface();
        _dragSurface = surface; _dragStarted = started; _dragEnded = ended;
        surface.PointerPressed += OnDragPressed;
        surface.PointerReleased += OnDragReleased;
        surface.PointerCaptureLost += OnDragCaptureLost;
    }

    public void ReplaceContent(Control content) => Content = content;

    public void ReassertOwnedZOrder()
    {
        ReassertOwnedZOrderForProbe();
    }

    internal bool ReassertOwnedZOrderForProbe()
    {
        if (!OperatingSystem.IsWindows() || TryGetPlatformHandle() is not { Handle: var handle }) return false;
        return SetWindowPos(handle, HwndTop, 0, 0, 0, 0, NoActivate | NoMove | NoSize | ShowWindow);
    }

    void OnDragPressed(object? sender, PointerPressedEventArgs e)
    {
        if (IsButtonHit(e.Source)) return;
        _dragStarted?.Invoke(); BeginMoveDrag(e);
    }

    void OnDragReleased(object? sender, PointerReleasedEventArgs e) => _dragEnded?.Invoke();
    void OnDragCaptureLost(object? sender, PointerCaptureLostEventArgs e) => _dragEnded?.Invoke();

    void DetachDragSurface()
    {
        _dragSurface!.PointerPressed -= OnDragPressed;
        _dragSurface.PointerReleased -= OnDragReleased;
        _dragSurface.PointerCaptureLost -= OnDragCaptureLost;
    }

    static bool IsButtonHit(object? source) => source is Visual visual &&
        (visual is Button || visual.GetVisualAncestors().Any(x => x is Button));

    protected override void OnClosed(EventArgs e)
    {
        if (_dragSurface is not null) DetachDragSurface();
        base.OnClosed(e);
    }

    const uint NoSize = 0x0001, NoMove = 0x0002, ShowWindow = 0x0040, NoActivate = 0x0010;
    static readonly nint HwndTop = 0;
    [System.Runtime.InteropServices.DllImport("user32")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    static extern bool SetWindowPos(nint hWnd, nint after, int x, int y, int cx, int cy, uint flags);
}
