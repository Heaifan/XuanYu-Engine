using Avalonia.Threading;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

using XuanYu.Editor.UI.XycControl;

public sealed partial class UiVm
{
    public IXycRuntime CreateXycRuntime(string instanceId) =>
        new UiVmXycRuntime(this, instanceId);

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
        _cameraRevision = Math.Max(_cameraRevision, _camera.Revision) + 1;
        var next = new CameraState(ToCore(pose.Position), ToCore(pose.Forward),
            ToCore(pose.Up), pose.FovDegrees, pose.NearPlane, pose.FarPlane,
            _cameraRevision, mode, pose.OrthographicScale);
        ApplyCameraFrame(new(next, ToCore(pose.ObservationCenter)), "XYC Test Pose");
        PublishSceneRenderSnapshot();
        return ReadXycCamera();
    }

    static Vector3 ToVector(Vector3d value) => new(value.X, value.Y, value.Z);
    static Vector3d ToCore(Vector3 value) => new(value.X, value.Y, value.Z);
}

sealed class UiVmXycRuntime(UiVm vm, string instanceId) : IXycRuntime
{
    public string InstanceId { get; } = instanceId;
    public XycRuntimeInfo InspectRuntime() => OnUi(() => new XycRuntimeInfo("XYC-API/1", InstanceId,
        "isolated-test-instance", "windowed-editor-instance; offscreen=NOT-IMPLEMENTED",
        typeof(UiVm).Assembly.GetName().Version?.ToString() ?? "UNKNOWN",
        Environment.GetEnvironmentVariable("XYC_BRANCH") ?? "UNKNOWN", "UNKNOWN"));
    public XycScene InspectScene() => OnUi(() => new XycScene(vm.SceneId,
        vm.SceneRevision, vm.TerrainStatus, vm.SceneEntityCount));
    public XycCamera InspectCamera() => OnUi(vm.ReadXycCamera);
    public XycCamera SetTestPose(XycCamera pose) => OnUi(() => vm.WriteXycCamera(pose));

    static T OnUi<T>(Func<T> action) => Dispatcher.UIThread.CheckAccess()
        ? action() : Dispatcher.UIThread.InvokeAsync(action).GetAwaiter().GetResult();
}
