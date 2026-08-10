namespace XuanYu.Editor.Camera;

public static partial class CameraNavigation
{
    public const double MaxDistanceMeters = 1_000_000.0;

    static double FarPlaneFor(double nearPlane, double distance) =>
        global::System.Math.Max(nearPlane * 10.0, distance * 4.0);
}
