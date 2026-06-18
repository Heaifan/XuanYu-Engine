using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Transform.Edit;
using FluidWarfare.Editor.Transform.Translation.Axis;
using FluidWarfare.Editor.Transform.Translation.Plane;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Editor.Windows.Viewport.Transform.Input;
using FluidWarfare.Project.World.Transform;
using FluidWarfare.Render.Camera;
using FluidWarfare.Render.Camera.Navigation;
using FluidWarfare.Render.Vulkan.Camera;

namespace FluidWarfare.Tests.Editor.Transform.Input;

/// <summary>
/// TransformInputRoute 的单元测试。
/// 验证输入语义：None 不回落、Axis/Plane 启动需 camera snapshot。
/// </summary>
public sealed class TransformInputRouteTests
{
    private static readonly Vector3d Pivot = new(10, 20, 30);

    /// <summary>最小有效 camera snapshot，用于轴拖动测试。</summary>
    private static PresentedCameraSnapshot CreateTestSnapshot()
    {
        // 透视投影矩阵：从 (30,-30,50) 看向 Pivot (10,20,30)
        return new PresentedCameraSnapshot
        {
            CameraPose = new SceneCameraPose
            {
                PositionX = 30, PositionY = -30, PositionZ = 50,
                TargetX = 10, TargetY = 20, TargetZ = 30,
                UpX = 0, UpY = 0, UpZ = 1,
                FieldOfViewDegrees = 45,
                NearPlane = 0.1f, FarPlane = 200,
                ProjectionMode = SceneProjectionMode.Perspective,
                OrthographicHeight = 40,
                Revision = 1
            },
            ViewProjection = CreateTestViewProjection(),
            InverseViewProjection = CreateTestInverseViewProjection(),
            ViewportWidth = 800,
            ViewportHeight = 600,
            FrameIndex = 1,
            CameraRevision = 1
        };
    }

    /// <summary>简化的透视投影矩阵。</summary>
    private static float[] CreateTestViewProjection() =>
    [
        1.0f, 0, 0, 0,
        0, 1.0f, 0, 0,
        0, 0, -1.003f, -1.0f,
        0, 0, -0.1003f, 0
    ];

    private static double[] CreateTestInverseViewProjection()
    {
        // 单位矩阵逆（近似），够让 VulkanSceneRayBuilder 通过验证
        return
        [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, -0.997, 0,
            0, 0, -0.997, 1
        ];
    }

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

    private static MoveGizmoLayout CreateLayout() =>
        MoveGizmoLayout.Build(
            (200, 300), (350, 290), (200, 170), (180, 350),
            false, false, false)!;

    [Fact]
    public void NoHover_PointerDownReturnsNotHandled()
    {
        var route = new TransformInputRoute();
        var result = route.OnPointerPressed(1, 400, 300, Pivot, null);
        Assert.False(result.Handled);
        Assert.False(result.Started);
    }

    [Fact]
    public void PointerDown_NonPrimaryButton_ReturnsNotHandled()
    {
        var route = new TransformInputRoute();
        var layout = CreateLayout();
        route.UpdateGizmoHover(200, 300, layout);
        var result = route.OnPointerPressed(2, 200, 300, Pivot, null);
        Assert.False(result.Handled);
    }

    [Fact]
    public void HoverAxisX_WithSnapshot_StartsDrag()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;
        route.UpdateGizmoHover(mx, my, layout);

        var result = route.OnPointerPressed(1, mx, my, Pivot, CreateTestSnapshot());
        Assert.True(result.Started);
    }

    [Fact]
    public void HoverAxisX_WithoutSnapshot_ReturnsNotHandled()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;
        route.UpdateGizmoHover(mx, my, layout);

        var result = route.OnPointerPressed(1, mx, my, Pivot, null);
        Assert.False(result.Started);
    }

    [Fact]
    public void ViewPlane_WithoutSnapshot_ReturnsNotHandled()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        route.UpdateGizmoHover(200, 300, layout);

        var result = route.OnPointerPressed(1, 200, 300, Pivot, null);
        Assert.False(result.Started);
    }

    [Fact]
    public void PlaneXY_WithoutSnapshot_ReturnsNotHandled()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        var c = Measure.PlaneXY_Corners(layout);
        var cx = (c.X0 + c.X2) / 2.0;
        var cy = (c.Y0 + c.Y2) / 2.0;
        route.UpdateGizmoHover(cx, cy, layout);

        var result = route.OnPointerPressed(1, cx, cy, Pivot, null);
        Assert.False(result.Started);
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
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;
        route.UpdateGizmoHover(mx, my, layout);
        route.OnPointerPressed(1, mx, my, Pivot, CreateTestSnapshot());

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
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;
        route.UpdateGizmoHover(mx, my, layout);
        route.OnPointerPressed(1, mx, my, Pivot, CreateTestSnapshot());

        var released = route.OnPointerReleased();
        Assert.True(released.Handled);
        Assert.True(released.Ended);
        Assert.False(route.Gizmo.IsDragging);
    }

    [Fact]
    public void OnPointerMoved_WithoutDrag_ReturnsDefault()
    {
        var route = new TransformInputRoute();
        var result = route.OnPointerMoved(400, 300);
        Assert.False(result.Handled);
    }

    [Fact]
    public void AxisDrag_OnPointerMoved_ReturnsPreview()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;
        route.UpdateGizmoHover(mx, my, layout);
        route.OnPointerPressed(1, mx, my, Pivot, CreateTestSnapshot());

        var moved = route.OnPointerMoved(mx + 10, my + 5);
        Assert.True(moved.Handled);
    }

    [Fact]
    public void CancelDrag_AfterMove_ClearsSession()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;
        route.UpdateGizmoHover(mx, my, layout);
        route.OnPointerPressed(1, mx, my, Pivot, CreateTestSnapshot());
        route.OnPointerMoved(mx + 10, my + 5);

        route.CancelDrag();
        Assert.False(route.Session.IsActive);
        Assert.False(route.Gizmo.IsDragging);
    }
}
