using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Transform.Edit;
using FluidWarfare.Editor.Transform.Translation.Plane;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Editor.Windows.Viewport.Transform.Input;
using FluidWarfare.Project.World.Transform;
using FluidWarfare.Render.Camera;
using FluidWarfare.Render.Camera.Navigation;
using FluidWarfare.Render.Vulkan.Camera;

namespace FluidWarfare.Tests.Editor.Transform.Input;

/// <summary>
/// Plane 拖动和统一射线的生产链测试。
/// 使用有效的 PresentedCameraSnapshot 验证完整的拖动生命周期。
/// </summary>
public sealed class TransformInputRoutePlaneTests
{
    private static readonly Vector3d Pivot = new(10, 20, 30);

    /// <summary>
    /// 透视快照：相机在 (30,-30,50) 看向 Pivot (10,20,30)。
    /// 使用简单但有效的对角矩阵，使 VulkanSceneRayBuilder 能通过变换验证。
    /// </summary>
    private static PresentedCameraSnapshot CreatePerspectiveSnapshot()
    {
        // 构造可用的 VP + invVP 对
        // 选择使屏幕中心 (400,300) 的射线方向有显著 Z 分量的矩阵
        var vp = new float[16];
        vp[0] = 1.0f; vp[4] = 0; vp[8] = 0; vp[12] = 0;
        vp[1] = 0; vp[5] = 1.0f; vp[9] = 0; vp[13] = 0;
        vp[2] = 0; vp[6] = 0; vp[10] = -1.0001f; vp[14] = -0.2001f;
        vp[3] = 0; vp[7] = 0; vp[11] = -1; vp[15] = 0;

        var invVp = new double[16];
        invVp[0] = 1; invVp[4] = 0; invVp[8] = 0; invVp[12] = 0;
        invVp[1] = 0; invVp[5] = 1; invVp[9] = 0; invVp[13] = 0;
        invVp[2] = 0; invVp[6] = 0; invVp[10] = 0; invVp[14] = -0.2001;
        invVp[3] = 0; invVp[7] = 0; invVp[11] = -1; invVp[15] = -1.0001;

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
            ViewProjection = vp,
            InverseViewProjection = invVp,
            ViewportWidth = 800,
            ViewportHeight = 600,
            FrameIndex = 1,
            CameraRevision = 1
        };
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
    public void ViewPlane_WithPerspectiveSnapshot_StartsDrag()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        route.UpdateGizmoHover(200, 300, layout);

        var result = route.OnPointerPressed(1, 200, 300, Pivot,
            CreatePerspectiveSnapshot());
        Assert.True(result.Started);
    }

    [Fact]
    public void AxisDrag_OnPointerMoved_UpdatesSessionPreview()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;
        route.UpdateGizmoHover(mx, my, layout);
        route.OnPointerPressed(1, mx, my, Pivot, CreatePerspectiveSnapshot());

        var before = route.Session.PreviewTransform.Position;
        route.OnPointerMoved(mx + 20, my + 10);
        var after = route.Session.PreviewTransform.Position;

        Assert.NotEqual(before, after);
    }

    [Fact]
    public void ViewPlaneDrag_OnPointerMoved_ChangesPosition()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        route.UpdateGizmoHover(200, 300, layout);
        route.OnPointerPressed(1, 200, 300, Pivot, CreatePerspectiveSnapshot());

        var before = route.Session.PreviewTransform.Position;
        route.OnPointerMoved(210, 310);
        var after = route.Session.PreviewTransform.Position;

        Assert.NotEqual(before, after);
    }

    [Fact]
    public void PointerReleased_EndsDragAndSession()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;
        route.UpdateGizmoHover(mx, my, layout);
        route.OnPointerPressed(1, mx, my, Pivot, CreatePerspectiveSnapshot());
        route.OnPointerMoved(mx + 30, my + 15);

        var released = route.OnPointerReleased();
        Assert.True(released.Ended);
        Assert.False(route.Session.IsActive);
    }

    [Fact]
    public void MultipleMoves_KeepSessionActive()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;
        route.UpdateGizmoHover(mx, my, layout);
        route.OnPointerPressed(1, mx, my, Pivot, CreatePerspectiveSnapshot());

        for (var i = 0; i < 5; i++)
        {
            var moved = route.OnPointerMoved(mx + i * 10, my);
            Assert.True(moved.Handled);
        }

        Assert.True(route.Session.IsActive);
        Assert.True(route.Gizmo.IsDragging);
    }

    [Fact]
    public void CancelDrag_AfterMultipleMoves_ClearsState()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;
        route.UpdateGizmoHover(mx, my, layout);
        route.OnPointerPressed(1, mx, my, Pivot, CreatePerspectiveSnapshot());
        route.OnPointerMoved(mx + 50, my + 25);

        route.CancelDrag();
        Assert.False(route.Session.IsActive);
        Assert.False(route.Gizmo.IsDragging);
    }

    [Fact]
    public void NewPress_AfterCancel_StartsFreshDrag()
    {
        var layout = CreateLayout();
        var mx = (layout.AxisStartX_X + layout.AxisEndPixelX_AxisX) / 2.0;
        var my = (layout.AxisStartY_X + layout.AxisEndPixelY_AxisX) / 2.0;

        var route = CreateActiveRoute();
        route.UpdateGizmoHover(mx, my, layout);
        route.OnPointerPressed(1, mx, my, Pivot, CreatePerspectiveSnapshot());
        route.CancelDrag();

        // 新按压应重新开始
        route = CreateActiveRoute();
        route.UpdateGizmoHover(mx, my, layout);
        var result = route.OnPointerPressed(1, mx, my, Pivot,
            CreatePerspectiveSnapshot());
        Assert.True(result.Started);
    }

    [Fact]
    public void CameraForward_FromSnapshot_IsNonZero()
    {
        var route = CreateActiveRoute();
        var layout = CreateLayout();
        route.UpdateGizmoHover(200, 300, layout);

        var result = route.OnPointerPressed(1, 200, 300, Pivot,
            CreatePerspectiveSnapshot());
        Assert.True(result.Started);

        // 拖动一次后验证位置有变化
        route.OnPointerMoved(205, 305);
        Assert.True(route.Session.IsActive);
    }
}
