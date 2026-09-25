using Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class DiagnosticTrackedIdentityTests
{
    [Fact]
    public void Explicit_name_wins_over_content_and_debug_id()
    {
        var target = new Button { Name = "SaveButton", Content = "聚焦" };
        XYDiagnostic.SetDebugId(target, "XYE.SAVE");
        var snapshot = DiagnosticElementSnapshot.Capture(DiagnosticProbeResolver.Resolve(target));
        Assert.Equal("SaveButton", DisplayName(snapshot));
        Assert.Equal("SaveButton", snapshot.InstanceName);
    }

    [Fact]
    public void Content_is_display_name_but_not_instance_name()
    {
        var target = new Button { Content = "聚焦" };
        XYDiagnostic.SetDebugId(target, "XYE.FOCUS");
        var snapshot = DiagnosticElementSnapshot.Capture(DiagnosticProbeResolver.Resolve(target));
        Assert.Equal("聚焦", DisplayName(snapshot));
        Assert.Equal("N/A", snapshot.InstanceName);
    }

    [Fact]
    public void Debug_id_then_component_type_are_display_fallbacks()
    {
        var target = new Border();
        XYDiagnostic.SetDebugId(target, "XYE.EMPTY");
        Assert.Equal("XYE.EMPTY", DisplayName(DiagnosticElementSnapshot.Capture(DiagnosticProbeResolver.Resolve(target))));
        Assert.Equal("Border", DisplayName(DiagnosticElementSnapshot.Capture(DiagnosticProbeResolver.Resolve(new Border()))));
    }

    static string? DisplayName(DiagnosticElementSnapshot snapshot) =>
        snapshot.GetType().GetProperty("TargetDisplayName")?.GetValue(snapshot) as string;
}
