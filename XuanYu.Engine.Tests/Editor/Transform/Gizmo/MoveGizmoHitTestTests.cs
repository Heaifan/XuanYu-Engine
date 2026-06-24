using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;

namespace XuanYu.Engine.Tests.Editor.Transform.Gizmo;

/// <summary>
/// MoveGizmoHitTest 的单元测试。
/// 验证平面块偏移、轴杆偏移、四边形命中/不命中。
/// </summary>
public sealed class MoveGizmoHitTestTests
{
    /// <summary>
    /// Pivot=(200,300)，X→右(350,290)，Y→上(200,170)，Z→左下(180,350)。
    /// </summary>
    private static MoveGizmoLayout CreateLayout()
    {
        return MoveGizmoLayout.Build(
            (200, 300),
            (350, 290),  // X end
            (200, 170),  // Y end
            (180, 350),  // Z end
            false, false, false)!;
    }

    [Fact]
    public void PlaneBlock_DoesNotStartAtPivot()
    {
        var layout = CreateLayout();
        var c = Measure.PlaneXY_Corners(layout);

        Assert.NotEqual(200, c.X0, 1);
        Assert.NotEqual(300, c.Y0, 1);
    }

    [Fact]
    public void AxisShaft_StartsOutsideCenterHandle()
    {
        var layout = CreateLayout();
        var distX = Math.Sqrt(
            Math.Pow(layout.AxisStartX_X - layout.PivotPixelX, 2) +
            Math.Pow(layout.AxisStartY_X - layout.PivotPixelY, 2));

        Assert.True(distX > MoveGizmoLayout.CenterRadius);
    }

    [Fact]
    public void PointerInsidePlaneQuad_HitsPlane()
    {
        var layout = CreateLayout();
        var c = Measure.PlaneXY_Corners(layout);

        // XY 平面块：从 (224,274) 到 (244,253) 的四边形
        var cx = (c.X0 + c.X1 + c.X2 + c.X3) / 4.0;
        var cy = (c.Y0 + c.Y1 + c.Y2 + c.Y3) / 4.0;

        var hit = MoveGizmoHitTest.HitTest(layout, cx, cy);
        Assert.Equal(MoveGizmoElement.PlaneXY, hit);
    }

    [Fact]
    public void PointerOutsidePlaneQuad_DoesNotHitPlane()
    {
        var layout = CreateLayout();
        var hit = MoveGizmoHitTest.HitTest(layout, 0, 0);

        Assert.NotEqual(MoveGizmoElement.PlaneXY, hit);
    }

    [Fact]
    public void PointerAtPivot_HitsViewPlane()
    {
        var layout = CreateLayout();
        var hit = MoveGizmoHitTest.HitTest(layout,
            layout.PivotPixelX, layout.PivotPixelY);

        Assert.Equal(MoveGizmoElement.ViewPlane, hit);
    }

    [Fact]
    public void PointerOnAxisX_Shaft_HitsAxisX()
    {
        var layout = CreateLayout();
        // X 轴起点和终点的中点
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;

        var hit = MoveGizmoHitTest.HitTest(layout, mx, my);
        Assert.Equal(MoveGizmoElement.AxisX, hit);
    }

    [Fact]
    public void PointerOnArrowTip_HitsAxis()
    {
        var layout = CreateLayout();
        var hit = MoveGizmoHitTest.HitTest(layout,
            layout.AxisEndPixelX_AxisX, layout.AxisEndPixelY_AxisX);

        Assert.Equal(MoveGizmoElement.AxisX, hit);
    }

    [Fact]
    public void PlaneXZ_HitsCorrectly()
    {
        var layout = CreateLayout();
        var c = Measure.PlaneXZ_Corners(layout);
        var cx = (c.X0 + c.X1 + c.X2 + c.X3) / 4.0;
        var cy = (c.Y0 + c.Y1 + c.Y2 + c.Y3) / 4.0;

        var hit = MoveGizmoHitTest.HitTest(layout, cx, cy);
        Assert.Equal(MoveGizmoElement.PlaneXZ, hit);
    }

    [Fact]
    public void PlaneYZ_HitsCorrectly()
    {
        var layout = CreateLayout();
        var c = Measure.PlaneYZ_Corners(layout);
        var cx = (c.X0 + c.X1 + c.X2 + c.X3) / 4.0;
        var cy = (c.Y0 + c.Y1 + c.Y2 + c.Y3) / 4.0;

        var hit = MoveGizmoHitTest.HitTest(layout, cx, cy);
        Assert.Equal(MoveGizmoElement.PlaneYZ, hit);
    }

    [Fact]
    public void DegenerateAxes_OnlyCenterHittable()
    {
        var layout = MoveGizmoLayout.Build(
            (200, 300), (200, 300), (200, 300), (200, 300), true, true, true)!;
        var hit = MoveGizmoHitTest.HitTest(layout, 200, 300);

        Assert.Equal(MoveGizmoElement.ViewPlane, hit);
    }

    [Fact]
    public void PointInConvexQuad_Inside_ReturnsTrue()
    {
        var layout = CreateLayout();
        var c = Measure.PlaneXY_Corners(layout);

        var midX = (c.X0 + c.X2) / 2.0;
        var midY = (c.Y0 + c.Y2) / 2.0;

        var hit = MoveGizmoHitTest.HitTest(layout, midX, midY);
        Assert.Equal(MoveGizmoElement.PlaneXY, hit);
    }

    [Fact]
    public void PointFarAway_ReturnsNone()
    {
        var layout = CreateLayout();
        var hit = MoveGizmoHitTest.HitTest(layout, 5000, 5000);
        Assert.Equal(MoveGizmoElement.None, hit);
    }
}
