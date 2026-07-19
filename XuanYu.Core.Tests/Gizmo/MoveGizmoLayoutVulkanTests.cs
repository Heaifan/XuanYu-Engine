using XuanYu.Core.Gizmo;

namespace XuanYu.Core.Tests.Gizmo;

public sealed partial class MoveGizmoLayoutTests
{
    [Fact]
    public void Oblique_camera_projects_axes_like_vulkan_viewport()
    {
        var layout = Layout();
        var origin = layout.Segments[0].Start;
        var x = layout.Segments.Single(item => item.Axis == MoveGizmoAxis.X);
        var y = layout.Segments.Single(item => item.Axis == MoveGizmoAxis.Y);
        var z = layout.Segments.Single(item => item.Axis == MoveGizmoAxis.Z);

        Assert.True(x.End.X < origin.X);
        Assert.True(y.End.Y > origin.Y);
        Assert.True(z.End.X < origin.X);
    }
}
