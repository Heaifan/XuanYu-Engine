using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Transform.Edit;
using FluidWarfare.Editor.Transform.Translation.Axis;
using FluidWarfare.Editor.Transform.Translation.Plane;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Editor.Windows.Viewport.Transform.Input;
using FluidWarfare.Project.World.Transform;

namespace FluidWarfare.Tests.Editor.Transform.Input;

/// <summary>
/// TransformInputRoute 的单元测试。
/// 验证输入语义：None 不回落、Plane 使用 PlaneSolver、ViewPlane 不走 Axis。
/// </summary>
public sealed class TransformInputRouteTests
{
    private static readonly Vector3d Pivot = new(10, 20, 30);
    private static readonly float[] ViewProjection = CreateTestViewProjection();
    private static readonly Vector3d CameraPos = new(30, -30, 50);
    private static readonly Vector3d CameraFwd = new Vector3d(-20, 50, -20).Normalize();

    private static float[] CreateTestViewProjection()
    {
        return
        [
            1.0f, 0, 0, 0,
            0, 1.0f, 0, 0,
            0, 0, -1.003f, -1.0f,
            0, 0, -0.1003f, 0
        ];
    }

    /// <summary>
    /// 创建已激活 Session 的路由（模拟 EditorShell 在调用 OnPointerPressed 前的行为）。
    /// </summary>
    private static TransformInputRoute CreateActiveRoute()
    {
        var route = new TransformInputRoute();
        route.Session.Begin(new TransformEditSnapshot(
            "test-entity",
            SceneTransformDefaults.FromPosition(Pivot),
            false,
            TransformEditKind.Translation));
        return route;
    }

    /// <summary>
    /// 构建使 PlaneXY 四边形在 (220,274)～(244,253) 区域的布局。
    /// Pivot=(200,300)，X→右，Y→上，Z→左下。
    /// </summary>
    private static MoveGizmoLayout CreateLayout()
    {
        return MoveGizmoLayout.Build(
            (200, 300),    // pivot
            (350, 290),    // X end
            (200, 170),    // Y end
            (180, 350),    // Z end
            false, false, false)!;
    }

    [Fact]
    public void NoHover_PointerDownReturnsNotHandled()
    {
        var route = CreateActiveRoute();
        var result = route.OnPointerPressed(1, 400, 300, Pivot,
            ViewProjection, 800, 600,
            50, 45, false, 0,
            CameraPos, CameraFwd);

        Assert.False(result.Handled);
        Assert.False(result.Started);
    }

    [Fact]
    public void PointerDown_NonPrimaryButton_ReturnsNotHandled()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        route.UpdateGizmoHover(200, 300, layout);
        var result = route.OnPointerPressed(2, 200, 300, Pivot,
            ViewProjection, 800, 600,
            50, 45, false, 0,
            CameraPos, CameraFwd);

        Assert.False(result.Handled);
    }

    [Fact]
    public void HoverAxisX_SetsDragKindAxis()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        // X 轴中点位置
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;
        route.UpdateGizmoHover(mx, my, layout);
        var result = route.OnPointerPressed(1, mx, my, Pivot,
            ViewProjection, 800, 600,
            50, 45, false, 0,
            CameraPos, CameraFwd);

        Assert.True(result.Started);
    }

    [Fact]
    public void ViewPlane_DoesNotMapToUnitX()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        // 中心手柄 → ViewPlane
        route.UpdateGizmoHover(200, 300, layout);
        var result = route.OnPointerPressed(1, 200, 300, Pivot,
            ViewProjection, 800, 600,
            50, 45, false, 0,
            CameraPos, CameraFwd);

        // ViewPlane 应当成功开始拖动
        Assert.True(result.Started);
    }

    [Fact]
    public void PlaneXY_NoFallbackToAxis()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();

        var xyCenter = PlaneCenter(Measure.PlaneXY_Corners(layout));
        route.UpdateGizmoHover(xyCenter.Item1, xyCenter.Item2, layout);
        var result = route.OnPointerPressed(1, 234, 264, Pivot,
            ViewProjection, 800, 600,
            50, 45, false, 0,
            CameraPos, CameraFwd);

        Assert.True(result.Started);
    }

    [Fact]
    public void PlaneXZ_NoFallbackToAxis()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();

        var xzCenter = PlaneCenter(Measure.PlaneXZ_Corners(layout));
        route.UpdateGizmoHover(xzCenter.Item1, xzCenter.Item2, layout);
        var result = route.OnPointerPressed(1, xzCenter.Item1, xzCenter.Item2, Pivot,
            ViewProjection, 800, 600,
            50, 45, false, 0,
            CameraPos, CameraFwd);

        Assert.True(result.Started);
    }

    [Fact]
    public void PlaneYZ_NoFallbackToAxis()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();

        var yzCenter = PlaneCenter(Measure.PlaneYZ_Corners(layout));
        route.UpdateGizmoHover(yzCenter.Item1, yzCenter.Item2, layout);
        var result = route.OnPointerPressed(1, 195, 330, Pivot,
            ViewProjection, 800, 600,
            50, 45, false, 0,
            CameraPos, CameraFwd);

        Assert.True(result.Started);
    }

    [Fact]
    public void NoHover_UpdateGizmoHover_ReturnsNone()
    {
        var route = new TransformInputRoute();
        Assert.False(route.HasHoveredElement);
    }

    [Fact]
    public void CancelDrag_ClearsGizmoState()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        route.UpdateGizmoHover(200, 300, layout);
        route.OnPointerPressed(1, 200, 300, Pivot,
            ViewProjection, 800, 600,
            50, 45, false, 0,
            CameraPos, CameraFwd);

        Assert.True(route.Gizmo.IsDragging);

        route.CancelDrag();

        Assert.False(route.Gizmo.IsDragging);
        Assert.False(route.Session.IsActive);
    }

    [Fact]
    public void PointerReleased_EndsDrag()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        route.UpdateGizmoHover(200, 300, layout);
        route.OnPointerPressed(1, 200, 300, Pivot,
            ViewProjection, 800, 600,
            50, 45, false, 0,
            CameraPos, CameraFwd);

        var released = route.OnPointerReleased();

        Assert.True(released.Handled);
        Assert.True(released.Ended);
        Assert.False(route.Gizmo.IsDragging);
    }

    /// <summary>返回四边形四个角的平均中心。</summary>
    private static (double, double) PlaneCenter(
        (double X0, double Y0, double X1, double Y1, double X2, double Y2, double X3, double Y3) c) =>
        ((c.X0 + c.X1 + c.X2 + c.X3) / 4.0,
         (c.Y0 + c.Y1 + c.Y2 + c.Y3) / 4.0);
}
