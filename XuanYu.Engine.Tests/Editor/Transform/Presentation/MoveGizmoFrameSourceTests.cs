using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Render.Camera;
using XuanYu.Engine.Render.Camera.Navigation;
using XuanYu.Engine.Render.Vulkan.Camera;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Presentation;

namespace XuanYu.Engine.Tests.Editor.Transform.Presentation;

public sealed class MoveGizmoFrameSourceTests
{
    static readonly EntityId s_validEntity = EntityId.FromInt(42);
    static readonly EntityId s_invalidEntity = EntityId.None;

    static PresentedCameraSnapshot MakeValidCamera()
    {
        var orbit = SceneOrbitCameraMotion.CreateDefault();
        var pose = SceneCameraPose.FromOrbitState(orbit, 1);
        var cam = new VulkanCameraInfo(
            pose.PositionX, pose.PositionY, pose.PositionZ,
            pose.TargetX, pose.TargetY, pose.TargetZ,
            pose.UpX, pose.UpY, pose.UpZ,
            pose.FieldOfViewDegrees, pose.NearPlane, pose.FarPlane);
        var vp = VulkanCameraMatrices.ComputeVulkanMVP(cam, 1280f / 720f);
        VulkanMatrixInvert.TryInvert(vp, out var inv, out _);
        return new PresentedCameraSnapshot
        {
            CameraPose = pose,
            ViewProjection = vp,
            InverseViewProjection = inv!,
            ViewportWidth = 1280,
            ViewportHeight = 720,
            FrameIndex = 1,
            CameraRevision = 1
        };
    }

    static MoveGizmoFrameInput MakeInput(bool moveToolActive, EntityId entityId) =>
        new(moveToolActive, entityId, Vector3d.Zero, MakeValidCamera(), MoveGizmoElement.None, 1);

    [Fact]
    public void Build_WithMoveToolActiveAndSelectedEntity_ReturnsNonEmpty()
    {
        var result = MoveGizmoFrameSource.Build(MakeInput(true, s_validEntity));
        Assert.NotEqual(MoveGizmoFrameResult.Empty, result);
        Assert.False(result.IsEmpty);
    }

    [Fact]
    public void Build_WithMoveToolActiveAndNoEntity_ReturnsNonEmpty()
    {
        // AND 条件：MoveToolActive=true, !IsValid=true → false && true = false → 不 Empty
        // 不 Empty 意味着继续执行，即使没有有效实体
        var result = MoveGizmoFrameSource.Build(MakeInput(true, s_invalidEntity));
        Assert.NotEqual(MoveGizmoFrameResult.Empty, result);
    }

    [Fact]
    public void Build_WithMoveToolInactiveAndSelectedEntity_ReturnsNonEmpty()
    {
        // AND 条件：MoveToolActive=false, !IsValid=false → true && false = false → 不 Empty
        // 9.0Y-1 可见性条件从 OR 改 AND 后：选中实体时即使非 Move 工具也显示 Gizmo
        var result = MoveGizmoFrameSource.Build(MakeInput(false, s_validEntity));
        Assert.NotEqual(MoveGizmoFrameResult.Empty, result);
    }

    [Fact]
    public void Build_WithMoveToolInactiveAndNoEntity_ReturnsEmpty()
    {
        // AND 条件：!false && !false = true → Empty
        var result = MoveGizmoFrameSource.Build(MakeInput(false, s_invalidEntity));
        Assert.Equal(MoveGizmoFrameResult.Empty, result);
    }

    [Fact]
    public void Build_WithInvalidCamera_ReturnsEmpty()
    {
        var camera = MakeValidCamera() with { ViewProjection = null!, InverseViewProjection = null! };
        var input = new MoveGizmoFrameInput(true, s_validEntity, Vector3d.Zero, camera, MoveGizmoElement.None, 1);
        // IsValid 检查 ViewProjection 长度 → false
        // 但 MoveGizmoFrameSource 自己检查 camera.IsValid
        // camera.IsValid 检查：!camera.IsValid → camera 是 PresentedCameraSnapshot，
        //   IsValid 可能检查 ViewProjection != null
        // Let's use ViewProjection with wrong length
        var cam2 = MakeValidCamera() with { ViewProjection = [1, 2, 3] };
        var input2 = new MoveGizmoFrameInput(true, s_validEntity, Vector3d.Zero, cam2, MoveGizmoElement.None, 1);
        var result = MoveGizmoFrameSource.Build(input2);
        Assert.Equal(MoveGizmoFrameResult.Empty, result);
    }
}
