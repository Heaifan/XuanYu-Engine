using Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticR1FloatingRuntimeTests
{
    [Fact]
    public void Locked_probe_ignores_later_hover_until_escape()
    {
        var first = new Button { Name = "First" };
        var second = new Button { Name = "Second" };
        var host = new DiagnosticOverlayHost();
        host.SetProbeResult(DiagnosticProbeResolver.Resolve(first));
        host.LockProbe();
        host.SetProbeResult(DiagnosticProbeResolver.Resolve(second));
        Assert.Same(first, host.CurrentProbeResult?.DeepVisual);
        Assert.True(host.IsProbeLocked);
        host.UnlockProbe();
        Assert.False(host.IsProbeLocked);
    }
}
