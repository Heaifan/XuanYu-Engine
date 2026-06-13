using FluidWarfare.Render.Camera;
using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Tests.Render.ViewportNavigation;

public sealed class ViewportNavigationLayoutTests
{
    private static SceneCameraPose DefaultPose => SceneCameraPose.FromOrbitState(
        SceneOrbitCameraMotion.CreateDefault(), 1);

    [Fact]
    public void Compute_ReturnsLayout()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        Assert.NotNull(layout);
        Assert.Equal(1280, layout.ViewportWidth);
        Assert.Equal(720, layout.ViewportHeight);
    }

    [Fact]
    public void GizmoCenter_IsInTopRightCorner()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        Assert.Equal(1280 - ViewportNavigationLayout.MarginRight - ViewportNavigationLayout.GizmoSize / 2f,
            layout.GizmoCenterX, 3);
        Assert.Equal(ViewportNavigationLayout.MarginTop + ViewportNavigationLayout.GizmoSize / 2f,
            layout.GizmoCenterY, 3);
    }

    [Fact]
    public void NavigationButtons_AreStackedUnderCenteredGizmo()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);

        Assert.Equal(layout.GizmoCenterX, layout.ZoomButtonRect.X + layout.ZoomButtonRect.W / 2f, 3);
        Assert.Equal(layout.ZoomButtonRect.X, layout.PanButtonRect.X, 3);
        Assert.Equal(layout.PanButtonRect.X, layout.FrameButtonRect.X, 3);
        Assert.Equal(layout.FrameButtonRect.X, layout.ProjectionButtonRect.X, 3);
        Assert.True(layout.ZoomButtonRect.Y > layout.GizmoCenterY);
        Assert.Equal(ViewportNavigationLayout.ButtonSize + ViewportNavigationLayout.ButtonSpacing,
            layout.PanButtonRect.Y - layout.ZoomButtonRect.Y, 3);
    }

    [Fact]
    public void Compute_HasSixAxisProjections()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        Assert.Equal(6, layout.AxisProjections.Count);
    }

    [Fact]
    public void AxisProjections_HaveCorrectElements()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        var elements = layout.AxisProjections.Select(a => a.Element).ToHashSet();
        Assert.Contains(ViewportNavigationElement.PositiveX, elements);
        Assert.Contains(ViewportNavigationElement.NegativeX, elements);
        Assert.Contains(ViewportNavigationElement.PositiveY, elements);
        Assert.Contains(ViewportNavigationElement.NegativeY, elements);
        Assert.Contains(ViewportNavigationElement.PositiveZ, elements);
        Assert.Contains(ViewportNavigationElement.NegativeZ, elements);
    }

    [Fact]
    public void AxisProjections_UseExpectedColors()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        var posX = layout.AxisProjections.First(a => a.Element == ViewportNavigationElement.PositiveX);
        Assert.Equal(0xF0 / 255f, posX.Color.R, 3);
        Assert.Equal(0x4B / 255f, posX.Color.G, 3);
        Assert.Equal(0x3E / 255f, posX.Color.B, 3);

        var posY = layout.AxisProjections.First(a => a.Element == ViewportNavigationElement.PositiveY);
        Assert.Equal(0x65 / 255f, posY.Color.R, 3);
        Assert.Equal(0xC8 / 255f, posY.Color.G, 3);
        Assert.Equal(0x4A / 255f, posY.Color.B, 3);

        var posZ = layout.AxisProjections.First(a => a.Element == ViewportNavigationElement.PositiveZ);
        Assert.Equal(0x39 / 255f, posZ.Color.R, 3);
        Assert.Equal(0x7B / 255f, posZ.Color.G, 3);
        Assert.Equal(0xFF / 255f, posZ.Color.B, 3);
    }

    [Fact]
    public void PositiveAxis_HasLargerRadiusThanNegative()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        // For any axis, positive should have larger radius when depth > 0
        // In default view (Yaw=135,Pitch=45), +X faces roughly toward camera
        var posX = layout.AxisProjections.First(a => a.Element == ViewportNavigationElement.PositiveX);
        var negX = layout.AxisProjections.First(a => a.Element == ViewportNavigationElement.NegativeX);

        if (posX.Depth > 0)
            Assert.True(posX.Radius > negX.Radius);
        else
            Assert.True(posX.Radius < negX.Radius);
    }

    [Fact]
    public void HitTest_AxisEnd_ReturnsCorrectElement()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        // Click on the center of each axis projection
        foreach (var proj in layout.AxisProjections)
        {
            var hit = layout.HitTest(proj.ScreenX, proj.ScreenY);
            Assert.Equal(proj.Element, hit);
        }
    }

    [Fact]
    public void HitTest_GizmoCenter_ReturnsGizmoCenter()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        var hit = layout.HitTest(layout.GizmoCenterX, layout.GizmoCenterY);
        Assert.Equal(ViewportNavigationElement.GizmoCenter, hit);
    }

    [Fact]
    public void HitTest_ZoomButton_ReturnsZoom()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        var btn = layout.ZoomButtonRect;
        var hit = layout.HitTest(btn.X + 1, btn.Y + 1);
        Assert.Equal(ViewportNavigationElement.ZoomButton, hit);
    }

    [Fact]
    public void HitTest_PanButton_ReturnsPan()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        var btn = layout.PanButtonRect;
        var hit = layout.HitTest(btn.X + 1, btn.Y + 1);
        Assert.Equal(ViewportNavigationElement.PanButton, hit);
    }

    [Fact]
    public void HitTest_FrameButton_ReturnsFrame()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        var btn = layout.FrameButtonRect;
        var hit = layout.HitTest(btn.X + 1, btn.Y + 1);
        Assert.Equal(ViewportNavigationElement.FrameButton, hit);
    }

    [Fact]
    public void HitTest_ProjectionButton_ReturnsProjection()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        var btn = layout.ProjectionButtonRect;
        var hit = layout.HitTest(btn.X + 1, btn.Y + 1);
        Assert.Equal(ViewportNavigationElement.ProjectionButton, hit);
    }

    [Fact]
    public void HitTest_OutsideOverlay_ReturnsNone()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        // Bottom-left corner — far from overlay
        var hit = layout.HitTest(10, 700);
        Assert.Equal(ViewportNavigationElement.None, hit);
    }

    [Fact]
    public void ElementToAction_MapsCorrectly()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        Assert.Equal(ViewportNavigationAction.SnapPositiveX, layout.ElementToAction(ViewportNavigationElement.PositiveX));
        Assert.Equal(ViewportNavigationAction.SnapNegativeX, layout.ElementToAction(ViewportNavigationElement.NegativeX));
        Assert.Equal(ViewportNavigationAction.Orbit, layout.ElementToAction(ViewportNavigationElement.GizmoCenter));
        Assert.Equal(ViewportNavigationAction.Zoom, layout.ElementToAction(ViewportNavigationElement.ZoomButton));
        Assert.Equal(ViewportNavigationAction.Pan, layout.ElementToAction(ViewportNavigationElement.PanButton));
        Assert.Equal(ViewportNavigationAction.Frame, layout.ElementToAction(ViewportNavigationElement.FrameButton));
        Assert.Equal(ViewportNavigationAction.ToggleProjection, layout.ElementToAction(ViewportNavigationElement.ProjectionButton));
        Assert.Equal(ViewportNavigationAction.None, layout.ElementToAction(ViewportNavigationElement.None));
    }

    [Fact]
    public void CameraRotation_ChangesAxisProjection()
    {
        // Default Yaw=135, Pitch=45
        var defaultLayout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);

        // After orbit to Yaw=0, Pitch=45 (looking toward -Y)
        var rotatedState = SceneOrbitCameraMotion.CreateDefault() with { Yaw = 0 };
        var rotatedPose = SceneCameraPose.FromOrbitState(rotatedState, 2);
        var rotatedLayout = ViewportNavigationLayout.Compute(1280, 720, rotatedPose);

        // The +Y axis should be at a different screen position after rotation
        var defaultPosY = defaultLayout.AxisProjections.First(a => a.Element == ViewportNavigationElement.PositiveY);
        var rotatedPosY = rotatedLayout.AxisProjections.First(a => a.Element == ViewportNavigationElement.PositiveY);
        Assert.NotEqual(defaultPosY.ScreenX, rotatedPosY.ScreenX, 1);
    }

    [Fact]
    public void Resize_RebuildsLayout()
    {
        var small = ViewportNavigationLayout.Compute(640, 480, DefaultPose);
        var large = ViewportNavigationLayout.Compute(1920, 1080, DefaultPose);

        // Different viewport sizes -> different gizmo positions
        Assert.NotEqual(small.ViewportWidth, large.ViewportWidth);
        Assert.NotEqual(small.GizmoCenterX, large.GizmoCenterX);

        var oldZoomCenterX = small.ZoomButtonRect.X + small.ZoomButtonRect.W / 2f;
        var oldZoomCenterY = small.ZoomButtonRect.Y + small.ZoomButtonRect.H / 2f;
        Assert.Equal(ViewportNavigationElement.None, large.HitTest(oldZoomCenterX, oldZoomCenterY));
    }

    [Fact]
    public void SmallViewport_ScalesDown()
    {
        var tiny = ViewportNavigationLayout.Compute(200, 150, DefaultPose);
        Assert.True(tiny.Scale < 1f);
    }

    [Fact]
    public void AxisProjections_HaveVaryingDepth()
    {
        // Verify that front-facing axes have depth > 0 and back-facing have depth < 0
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        var depths = layout.AxisProjections.Select(a => a.Depth).ToArray();

        // In default pose (Yaw=135, Pitch=45), some axes face toward camera, some away
        Assert.Contains(depths, d => d > 0);
        Assert.Contains(depths, d => d < 0);
    }

    [Fact]
    public void HitTest_OverlappingAxes_ReturnsFrontAxis()
    {
        var state = SceneNavigationCameraMotion.SnapToView(
            SceneOrbitCameraMotion.CreateDefault(),
            FluidWarfare.Render.Camera.Navigation.SceneNavigationView.PositiveX);
        var pose = SceneCameraPose.FromOrbitState(state, 2);
        var layout = ViewportNavigationLayout.Compute(1280, 720, pose);

        // 从 +X 看向场景时，±X 轴端投影重叠；-X 朝向相机，应获得点击。
        var hit = layout.HitTest(layout.GizmoCenterX, layout.GizmoCenterY);
        Assert.Equal(ViewportNavigationElement.NegativeX, hit);
    }

    [Fact]
    public void HitTest_InsideGizmoOrbitArea_ReturnsGizmoCenter()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        var offset = layout.GizmoOrbitCircle.Radius * 0.70f;

        var hit = layout.HitTest(
            layout.GizmoCenterX + offset,
            layout.GizmoCenterY + offset);

        Assert.Equal(ViewportNavigationElement.GizmoCenter, hit);
    }

    [Theory]
    [InlineData(FluidWarfare.Render.Camera.Navigation.SceneNavigationView.PositiveZ)]
    [InlineData(FluidWarfare.Render.Camera.Navigation.SceneNavigationView.NegativeZ)]
    public void VerticalViews_ProduceFiniteLayout(
        FluidWarfare.Render.Camera.Navigation.SceneNavigationView view)
    {
        var state = SceneNavigationCameraMotion.SnapToView(
            SceneOrbitCameraMotion.CreateDefault(), view);
        var pose = SceneCameraPose.FromOrbitState(state, 3);
        var layout = ViewportNavigationLayout.Compute(1280, 720, pose);

        Assert.All(layout.AxisProjections, axis =>
        {
            Assert.True(float.IsFinite(axis.ScreenX));
            Assert.True(float.IsFinite(axis.ScreenY));
            Assert.True(float.IsFinite(axis.Depth));
            Assert.True(float.IsFinite(axis.Radius));
        });
    }

    [Fact]
    public void EveryInteractiveElement_MapsToAction()
    {
        var layout = ViewportNavigationLayout.Compute(1280, 720, DefaultPose);
        foreach (var element in Enum.GetValues<ViewportNavigationElement>())
        {
            var action = layout.ElementToAction(element);
            if (element == ViewportNavigationElement.None)
                Assert.Equal(ViewportNavigationAction.None, action);
            else
                Assert.NotEqual(ViewportNavigationAction.None, action);
        }
    }

    [Fact]
    public void TinyViewport_InteractiveBoundsRemainFinite()
    {
        var layout = ViewportNavigationLayout.Compute(180, 140, DefaultPose);
        Assert.True(float.IsFinite(layout.GizmoCenterX));
        Assert.True(float.IsFinite(layout.GizmoCenterY));
        Assert.True(layout.Scale > 0f);
        Assert.True(layout.ZoomButtonRect.W > 0f);
        Assert.True(layout.ProjectionButtonRect.H > 0f);
    }
}
