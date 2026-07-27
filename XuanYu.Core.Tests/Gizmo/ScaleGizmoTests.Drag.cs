using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;

namespace XuanYu.Core.Tests.Gizmo;

public sealed partial class ScaleGizmoTests
{
    [Fact]
    public void Drag_X_handle_only_modifies_x_component()
    {
        var (drag, _) = XDrag(ScaleGizmoHandle.X, new Vector3d(1, 1, 1));
        var pulled = Pull(drag.handlePoint, drag.axisDir, 110);
        var scale = drag.drag.Solve(pulled.X, pulled.Y);
        Assert.True(scale.X > 1.5);
        Assert.Equal(1.0, scale.Y);
        Assert.Equal(1.0, scale.Z);
    }

    [Fact]
    public void Drag_Y_handle_only_modifies_y_component()
    {
        var (drag, _) = XDrag(ScaleGizmoHandle.Y, new Vector3d(1, 1, 1));
        var pulled = Pull(drag.handlePoint, drag.axisDir, 110);
        var scale = drag.drag.Solve(pulled.X, pulled.Y);
        Assert.Equal(1.0, scale.X);
        Assert.True(scale.Y > 1.5);
        Assert.Equal(1.0, scale.Z);
    }

    [Fact]
    public void Drag_Z_handle_only_modifies_z_component()
    {
        var (drag, _) = XDrag(ScaleGizmoHandle.Z, new Vector3d(1, 1, 1));
        var pulled = Pull(drag.handlePoint, drag.axisDir, 110);
        var scale = drag.drag.Solve(pulled.X, pulled.Y);
        Assert.Equal(1.0, scale.X);
        Assert.Equal(1.0, scale.Y);
        Assert.True(scale.Z > 1.5);
    }

    [Fact]
    public void Drag_Uniform_scales_all_three_equally()
    {
        var (drag, layout) = XDrag(ScaleGizmoHandle.Uniform, new Vector3d(1, 1, 1));
        var pulled = new ScreenPoint(layout.Center.X, layout.Center.Y - 110);
        var scale = drag.drag.Solve(pulled.X, pulled.Y);
        Assert.True(scale.X > 1.5 && scale.Y > 1.5 && scale.Z > 1.5);
        Assert.True(Near(scale.X, scale.Y, 1e-9) && Near(scale.Y, scale.Z, 1e-9));
    }
}
