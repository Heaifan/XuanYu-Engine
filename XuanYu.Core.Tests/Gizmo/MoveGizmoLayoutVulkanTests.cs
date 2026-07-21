using XuanYu.Core.Gizmo;

namespace XuanYu.Core.Tests.Gizmo;

public sealed partial class MoveGizmoLayoutTests
{
    [Fact]
    public void Oblique_z_up_camera_projects_world_up_toward_screen_top()
    {
        var layout = Layout();
        var origin = layout.Segments[0].Start;
        var x = layout.Segments.Single(item => item.Axis == MoveGizmoAxis.X);
        var y = layout.Segments.Single(item => item.Axis == MoveGizmoAxis.Y);
        var z = layout.Segments.Single(item => item.Axis == MoveGizmoAxis.Z);

        Assert.NotEqual(origin, x.End);
        Assert.NotEqual(origin, y.End);
        Assert.True(z.End.Y < origin.Y);
    }
}
