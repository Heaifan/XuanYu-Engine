using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Diagnostics;

public static partial class ViewportProbe
{
    public static long BeginWheel(double x, double y, double delta, string mode, string tool)
    {
        if (!Enabled) return 0;
        var id = NewWheel(); SetWheel(id);
        Log("input-camera", $"[INPUT] Wheel={id};Pointer=({x:0.###},{y:0.###});Delta={delta:0.###};ActiveMode={mode};ActiveTool={tool};Handler=UiVmD1Handler");
        return id;
    }

    public static void Operation(string operation) =>
        Log("input-camera", $"[INPUT] Operation={operation}");

    public static void EndWheel(long id, bool handled) =>
        Log("input-camera", $"[INPUT] Wheel={id};Handled={handled}");

    public static void CameraWriter(string writer, CameraState oldCamera,
        Vector3d oldPivot, CameraState nextCamera, Vector3d nextPivot)
    {
        if (!Enabled) return;
        var delta = nextCamera.Position - oldCamera.Position;
        var direction = delta.IsZero ? 0 : delta.Normalize().Dot(oldCamera.Forward);
        var cross = delta.IsZero ? 0 : delta.Normalize().Cross(oldCamera.Forward).Length;
        Log("input-camera", $"[CAMERA-WRITER] WriterId={writer};CallSite=UiVm;OldPosition={oldCamera.Position};NewPosition={nextCamera.Position};OldPivot={oldPivot};NewPivot={nextPivot};DeltaPosition={delta};DotDeltaForward={direction:0.########};CrossDeltaForward={cross:0.########};ForwardDelta={(nextCamera.Forward - oldCamera.Forward).Length:0.########};PivotDelta={(nextPivot - oldPivot).Length:0.########};FovDelta={nextCamera.VerticalFovDegrees - oldCamera.VerticalFovDegrees:0.########}");
    }
}
