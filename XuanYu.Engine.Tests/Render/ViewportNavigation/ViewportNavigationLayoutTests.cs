using XuanYu.Engine.Render.Camera;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.ViewportNavigation;

namespace FluidWarfare.Tests.Render.ViewportNavigation;

public sealed class ViewportNavigationLayoutTests
{
    private static SceneCameraPose DefaultPose => SceneCameraPose.FromOrbitState(
        SceneOrbitCameraMotion.CreateDefault(), 1);

    [Fact]
    public void Compute_ReturnsLayout()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        Assert.NotNull(layout);
        Assert.Equal(1280, layout.ViewportWidth);
        Assert.Equal(720, layout.ViewportHeight);
    }

    [Fact]
    public void GizmoCenter_IsInTopRightCorner()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        Assert.Equal(1280 - ViewportNavigationLayout.MarginRight - ViewportNavigationLayout.GizmoSize / 2f,
            layout.GizmoCenterX, 3);
        Assert.Equal(ViewportNavigationLayout.MarginTop + ViewportNavigationLayout.GizmoSize / 2f,
            layout.GizmoCenterY, 3);
    }

    [Fact]
    public void NavigationButtons_AreStackedUnderCenteredGizmo()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        Assert.Equal(layout.GizmoCenterX, layout.PanButtonRect.X + layout.PanButtonRect.W / 2f, 3);
        Assert.Equal(layout.PanButtonRect.X, layout.FrameButtonRect.X, 3);
        Assert.Equal(layout.FrameButtonRect.X, layout.ProjectionButtonRect.X, 3);
        Assert.True(layout.PanButtonRect.Y > layout.GizmoCenterY);
        Assert.Equal(ViewportNavigationLayout.ButtonSize + ViewportNavigationLayout.ButtonSpacing,
            layout.FrameButtonRect.Y - layout.PanButtonRect.Y, 3);
    }

    [Fact]
    public void Compute_HasSixAxisProjections()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        Assert.Equal(6, layout.AxisProjections.Count);
    }

    [Fact]
    public void AxisProjections_HaveCorrectElements()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
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
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
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
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
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
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        foreach (var proj in layout.AxisProjections)
        {
            var hit = ViewportNavigationHitTest.HitTest(proj.ScreenX, proj.ScreenY, layout);
            Assert.Equal(proj.Element, hit);
        }
    }

    [Fact]
    public void HitTest_GizmoCenter_ReturnsGizmoCenter()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        var hit = ViewportNavigationHitTest.HitTest(layout.GizmoCenterX, layout.GizmoCenterY, layout);
        Assert.Equal(ViewportNavigationElement.GizmoCenter, hit);
    }

    [Fact]
    public void HitTest_PanButton_ReturnsPan()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        var btn = layout.PanButtonRect;
        var hit = ViewportNavigationHitTest.HitTest(btn.X + 1, btn.Y + 1, layout);
        Assert.Equal(ViewportNavigationElement.PanButton, hit);
    }

    [Fact]
    public void HitTest_FrameButton_ReturnsFrame()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        var btn = layout.FrameButtonRect;
        var hit = ViewportNavigationHitTest.HitTest(btn.X + 1, btn.Y + 1, layout);
        Assert.Equal(ViewportNavigationElement.FrameButton, hit);
    }

    [Fact]
    public void HitTest_ProjectionButton_ReturnsProjection()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        var btn = layout.ProjectionButtonRect;
        var hit = ViewportNavigationHitTest.HitTest(btn.X + 1, btn.Y + 1, layout);
        Assert.Equal(ViewportNavigationElement.ProjectionButton, hit);
    }

    [Fact]
    public void HitTest_OutsideOverlay_ReturnsNone()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        var hit = ViewportNavigationHitTest.HitTest(10, 700, layout);
        Assert.Equal(ViewportNavigationElement.None, hit);
    }

    [Fact]
    public void ElementToAction_MapsCorrectly()
    {
        Assert.Equal(ViewportNavigationAction.SnapPositiveX,
            ViewportNavigationHitTest.ElementToAction(ViewportNavigationElement.PositiveX));
        Assert.Equal(ViewportNavigationAction.SnapNegativeX,
            ViewportNavigationHitTest.ElementToAction(ViewportNavigationElement.NegativeX));
        Assert.Equal(ViewportNavigationAction.Orbit,
            ViewportNavigationHitTest.ElementToAction(ViewportNavigationElement.GizmoCenter));
        Assert.Equal(ViewportNavigationAction.Pan,
            ViewportNavigationHitTest.ElementToAction(ViewportNavigationElement.PanButton));
        Assert.Equal(ViewportNavigationAction.Frame,
            ViewportNavigationHitTest.ElementToAction(ViewportNavigationElement.FrameButton));
        Assert.Equal(ViewportNavigationAction.ToggleProjection,
            ViewportNavigationHitTest.ElementToAction(ViewportNavigationElement.ProjectionButton));
        Assert.Equal(ViewportNavigationAction.None,
            ViewportNavigationHitTest.ElementToAction(ViewportNavigationElement.None));
    }

    [Fact]
    public void CameraRotation_ChangesAxisProjection()
    {
        var defaultLayout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        var rotatedState = SceneOrbitCameraMotion.CreateDefault() with { Yaw = 0 };
        var rotatedPose = SceneCameraPose.FromOrbitState(rotatedState, 2);
        var rotatedLayout = ViewportNavigationLayoutCompute.Compute(1280, 720, rotatedPose);

        var defaultPosY = defaultLayout.AxisProjections.First(a => a.Element == ViewportNavigationElement.PositiveY);
        var rotatedPosY = rotatedLayout.AxisProjections.First(a => a.Element == ViewportNavigationElement.PositiveY);
        Assert.NotEqual(defaultPosY.ScreenX, rotatedPosY.ScreenX, 1);
    }

    [Fact]
    public void Resize_RebuildsLayout()
    {
        var small = ViewportNavigationLayoutCompute.Compute(640, 480, DefaultPose);
        var large = ViewportNavigationLayoutCompute.Compute(1920, 1080, DefaultPose);

        Assert.NotEqual(small.ViewportWidth, large.ViewportWidth);
        Assert.NotEqual(small.GizmoCenterX, large.GizmoCenterX);

        var oldPx = small.PanButtonRect.X + small.PanButtonRect.W / 2f;
        var oldPy = small.PanButtonRect.Y + small.PanButtonRect.H / 2f;
        Assert.Equal(ViewportNavigationElement.None,
            ViewportNavigationHitTest.HitTest(oldPx, oldPy, large));
    }

    [Fact]
    public void SmallViewport_ScalesDown()
    {
        var tiny = ViewportNavigationLayoutCompute.Compute(200, 150, DefaultPose);
        Assert.True(tiny.Scale < 1f);
    }

    [Fact]
    public void AxisProjections_HaveVaryingDepth()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        var depths = layout.AxisProjections.Select(a => a.Depth).ToArray();
        Assert.Contains(depths, d => d > 0);
        Assert.Contains(depths, d => d < 0);
    }

    [Fact]
    public void HitTest_OverlappingAxes_ReturnsFrontAxis()
    {
        var state = SceneNavigationCameraMotion.SnapToView(
            SceneOrbitCameraMotion.CreateDefault(), SceneNavigationView.PositiveX);
        var pose = SceneCameraPose.FromOrbitState(state, 2);
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, pose);

        var hit = ViewportNavigationHitTest.HitTest(
            layout.GizmoCenterX, layout.GizmoCenterY, layout);
        Assert.Equal(ViewportNavigationElement.NegativeX, hit);
    }

    [Fact]
    public void HitTest_InsideGizmoOrbitArea_ReturnsGizmoCenter()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, DefaultPose);
        var offset = layout.GizmoOrbitCircle.Radius * 0.70f;
        var hit = ViewportNavigationHitTest.HitTest(
            layout.GizmoCenterX + offset, layout.GizmoCenterY + offset, layout);
        Assert.Equal(ViewportNavigationElement.GizmoCenter, hit);
    }

    [Theory]
    [InlineData(SceneNavigationView.PositiveZ)]
    [InlineData(SceneNavigationView.NegativeZ)]
    public void VerticalViews_ProduceFiniteLayout(SceneNavigationView view)
    {
        var state = SceneNavigationCameraMotion.SnapToView(
            SceneOrbitCameraMotion.CreateDefault(), view);
        var pose = SceneCameraPose.FromOrbitState(state, 3);
        var layout = ViewportNavigationLayoutCompute.Compute(1280, 720, pose);

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
        foreach (var element in Enum.GetValues<ViewportNavigationElement>())
        {
            var action = ViewportNavigationHitTest.ElementToAction(element);
            if (element == ViewportNavigationElement.None)
                Assert.Equal(ViewportNavigationAction.None, action);
            else
                Assert.NotEqual(ViewportNavigationAction.None, action);
        }
    }

    [Fact]
    public void TinyViewport_InteractiveBoundsRemainFinite()
    {
        var layout = ViewportNavigationLayoutCompute.Compute(180, 140, DefaultPose);
        Assert.True(float.IsFinite(layout.GizmoCenterX));
        Assert.True(float.IsFinite(layout.GizmoCenterY));
        Assert.True(layout.Scale > 0f);
        Assert.True(layout.PanButtonRect.W > 0f);
        Assert.True(layout.ProjectionButtonRect.H > 0f);
    }
}
