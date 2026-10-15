using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

sealed class FakeRuntime : IXycRuntime
{
    XycCamera _camera = new(new(0, 0, 10), new(0, 0, -1), new(0, 1, 0),
        75, 0.05, 1000, "Perspective", 0, new(0, 0, 0), 1);
    public string InstanceId => "test-instance";
    public XycRuntimeInfo InspectRuntime() => new("XYC-API/1", InstanceId,
        "test", "NOT-RUN", "test", "test", "test");
    public XycScene InspectScene() => new("scene-test", 3, "未加载", 0);
    public XycCamera InspectCamera() => _camera;
    public XycCamera SetTestPose(XycCamera pose)
    {
        _camera = pose with { Revision = _camera.Revision + 1 };
        return _camera;
    }
    public XycCamera OrbitCamera(double deltaX, double deltaY)
    {
        _camera = _camera with
        {
            Position = _camera.Position with { X = _camera.Position.X + deltaX },
            Revision = _camera.Revision + 1
        };
        return _camera;
    }
    public bool ArmPresentedCapture(long cameraRevision) => cameraRevision > 0;
    public bool ReleasePresentedCapture() => true;
}
