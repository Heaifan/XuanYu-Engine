using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Project.World.Transform;
using XuanYu.Engine.Render.Vulkan.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;

namespace XuanYu.Engine.Tests.Editor.Transform.Drag;

public sealed class TransformDragRouteTests
{
    static readonly EntityId TestEntityId = EntityId.FromInt(1);
    static readonly SceneTransform TestTransform = new(new Vector3d(10, 20, 30), default, default);
    static readonly PresentedCameraSnapshot InvalidCamera = PresentedCameraSnapshot.Empty;

    static TransformStartSnapshot MakeSnapshot() => new(
        TestEntityId, TestTransform, false, InvalidCamera, default);

    [Fact]
    public void Cancel_WhenNotActive_ReturnsNull()
    {
        var route = new TransformDragRoute();
        Assert.Null(route.Cancel());
    }

    [Fact]
    public void Move_WhenNotActive_ReturnsNotHandled()
    {
        var route = new TransformDragRoute();
        Assert.False(route.Move(100, 200).Handled);
    }

    [Fact]
    public void Confirm_WhenNotActive_DoesNotThrow()
    {
        var route = new TransformDragRoute();
        route.Confirm(); // should be no-op
        Assert.False(route.IsActive);
    }

    [Fact]
    public void Begin_WithInvalidCamera_ReturnsFalse()
    {
        var route = new TransformDragRoute();
        var snap = MakeSnapshot();
        Assert.False(route.Begin(MoveGizmoElement.ViewPlane, 100, 200, snap));
        Assert.False(route.IsActive);
    }

    [Fact]
    public void Begin_AfterFailedStart_IsNotActive()
    {
        var route = new TransformDragRoute();
        // First begin fails (invalid camera)
        route.Begin(MoveGizmoElement.ViewPlane, 100, 200, MakeSnapshot());
        // The route should be inactive after failed begin
        Assert.False(route.IsActive);
    }
}
