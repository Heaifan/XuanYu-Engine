namespace XuanYu.Editor.UI.XycControl;

public interface IXycRuntime
{
    string InstanceId { get; }
    XycRuntimeInfo InspectRuntime();
    XycScene InspectScene();
    XycCamera InspectCamera();
    XycCamera SetTestPose(XycCamera pose);
    XycCamera OrbitCamera(double deltaX, double deltaY);
    XycDollyResult DollyCamera(double wheelDelta);
    bool ArmPresentedCapture(long cameraRevision);
    bool ReleasePresentedCapture();
    SilentHostDiagnostics? InspectSilentHost() => null;
}
