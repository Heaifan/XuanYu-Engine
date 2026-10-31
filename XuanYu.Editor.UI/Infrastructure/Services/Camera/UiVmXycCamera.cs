using Avalonia.Threading;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

using XuanYu.Editor.UI.XycControl;

public sealed partial class UiVm
{
    public IXycRuntime CreateXycRuntime(string instanceId,
        Task<bool>? terrainLoad = null) =>
        new UiVmXycRuntime(this, instanceId, terrainLoad, null);

    internal IXycRuntime CreateXycRuntime(string instanceId,
        Task<bool>? terrainLoad, SilentVulkanHost? silentHost) =>
        new UiVmXycRuntime(this, instanceId, terrainLoad, silentHost);

    internal XycCamera ReadXycCamera()
    {
        var state = _camera;
        return new(ToVector(state.Position), ToVector(state.Forward), ToVector(state.Up),
            state.VerticalFovDegrees, state.NearPlane, state.FarPlane,
            state.Mode.ToString(), state.OrthographicScale,
            ToVector(_observationCenter), state.Revision);
    }

    internal XycCamera WriteXycCamera(XycCamera pose)
    {
        if (!Dispatcher.UIThread.CheckAccess())
            throw new InvalidOperationException("相机写入必须位于 UI 线程。");
        var mode = Enum.Parse<ProjectionMode>(pose.Mode);
        var revision = _cameraController.NextRevisionAfter(_camera.Revision);
        var next = new CameraState(ToCore(pose.Position), ToCore(pose.Forward),
            ToCore(pose.Up), pose.FovDegrees, pose.NearPlane, pose.FarPlane,
            revision, mode, pose.OrthographicScale);
        ApplyCameraFrame(new(next, ToCore(pose.ObservationCenter)), "XYC Test Pose");
        PublishSceneRenderSnapshot();
        return ReadXycCamera();
    }

    static Vector3 ToVector(Vector3d value) => new(value.X, value.Y, value.Z);
    static Vector3d ToCore(Vector3 value) => new(value.X, value.Y, value.Z);
}
