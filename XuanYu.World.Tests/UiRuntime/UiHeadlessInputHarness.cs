using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;

namespace XuanYu.World.Tests.UiRuntime;

internal static class UiHeadlessInputHarness
{
    internal static void Click(
        Window window,
        Control target,
        RawInputModifiers modifiers = RawInputModifiers.None)
    {
        var point = target.TranslatePoint(new Point(4, 4), window)
            ?? throw new InvalidOperationException("Target is not attached to the test window.");
        Assert.True(target.IsEffectivelyVisible, "Target is not visible in the test window.");
        Assert.True(new Rect(target.Bounds.Size).Contains(new Point(4, 4)), "Click point is outside target bounds.");
        window.Activate();
        Drain();
        window.MouseMove(point, modifiers);
        Drain();
        window.MouseDown(point, MouseButton.Left, modifiers);
        Drain();
        window.MouseUp(point, MouseButton.Left, modifiers);
        Drain();
    }

    private static void Drain() => Dispatcher.UIThread.RunJobs();
}
