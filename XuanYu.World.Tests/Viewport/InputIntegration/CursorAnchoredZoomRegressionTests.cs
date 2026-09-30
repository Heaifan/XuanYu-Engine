using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Gizmo;
using XuanYu.Editor.Input;
using XuanYu.Editor.Camera;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed partial class CursorAnchoredZoomRegressionTests
{
    public static IEnumerable<object[]> CursorPositions =>
    [
        [400d, 300d],
        [80d, 90d],
        [720d, 510d],
    ];

    [Fact]
    public void Perspective_wheel_preserves_reference_plane_anchor_at_off_center_cursor()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        var viewport = vm.CurrentViewport;
        const double cursorX = 620;
        const double cursorY = 180;
        var before = ViewProjectionState.Create(vm.RenderSnapshot.CameraState, viewport);
        var anchor = ReferencePlaneAnchor(before, cursorX, cursorY, 0);
        var beforeScreen = before.ProjectWorldPoint(anchor);

        vm.ViewportInput.Sink.Handle(Wheel(cursorX, cursorY, 1));

        var after = ViewProjectionState.Create(vm.RenderSnapshot.CameraState, viewport);
        var afterScreen = after.ProjectWorldPoint(anchor);
        var drift = Distance(beforeScreen, afterScreen);

        Assert.True(drift <= 2, $"reference-plane anchor drifted {drift:F3}px");
    }

    [Theory]
    [MemberData(nameof(CursorPositions))]
    public void Perspective_zoom_in_and_out_preserve_anchor_at_center_and_corners(double x, double y)
    {
        var camera = DefaultEditorCamera.Create(1);
        var center = DefaultEditorCamera.Target;
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var anchor = ReferencePlaneAnchor(ViewProjectionState.Create(camera, viewport), x, y, 0);

        var zoomedIn = Apply(camera, center, anchor, 1, 10, out var inCenter);
        var inDrift = AnchorDrift(zoomedIn, inCenter, viewport, anchor, x, y);
        var zoomedOut = Apply(zoomedIn, inCenter, anchor, -1, 10, out var outCenter);
        var outDrift = AnchorDrift(zoomedOut, outCenter, viewport, anchor, x, y);

        Assert.True(inDrift <= 2, $"zoom-in anchor drifted {inDrift:F3}px at ({x},{y})");
        Assert.True(outDrift <= 2, $"zoom-out anchor drifted {outDrift:F3}px at ({x},{y})");
    }

    [Theory]
    [MemberData(nameof(CursorPositions))]
    public void Orthographic_zoom_preserves_off_center_anchor(double x, double y)
    {
        var camera = new CameraState(new(0, 0, 100), new(0, 0, -1), Vector3d.UnitY,
            60, 0.1, 1000, 1, ProjectionMode.Orthographic, 100);
        var center = Vector3d.Zero;
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var anchor = ReferencePlaneAnchor(ViewProjectionState.Create(camera, viewport), x, y, 0);
        var result = Apply(camera, center, anchor, 1, 10, out var resultCenter);

        var drift = AnchorDrift(result, resultCenter, viewport, anchor, x, y);

        Assert.True(drift <= 2, $"orthographic anchor drifted {drift:F3}px at ({x},{y})");
    }

    [Fact]
    public void Terrain_anchor_keeps_its_screen_position_when_elevation_is_nonzero()
    {
        var camera = DefaultEditorCamera.Create(1);
        var center = DefaultEditorCamera.Target;
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var anchor = ReferencePlaneAnchor(ViewProjectionState.Create(camera, viewport), 620, 180, -2);
        var result = Apply(camera, center, anchor, 1, 1, out var resultCenter);

        var drift = AnchorDrift(result, resultCenter, viewport, anchor, 620, 180);

        Assert.True(drift <= 2, $"terrain anchor drifted {drift:F3}px");
    }

}
