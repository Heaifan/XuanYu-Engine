namespace XuanYu.Editor.UI.XycControl;

public interface IXycRuntime
{
    string InstanceId { get; }
    XycRuntimeInfo InspectRuntime();
    XycScene InspectScene();
    XycCamera InspectCamera();
    XycCamera SetTestPose(XycCamera pose);
    XycCamera OrbitCamera(double deltaX, double deltaY);
    bool ArmPresentedCapture(long cameraRevision);
    bool ReleasePresentedCapture();
}
