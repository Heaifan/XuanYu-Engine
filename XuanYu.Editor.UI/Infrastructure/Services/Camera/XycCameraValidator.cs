using System.Text.Json;

namespace XuanYu.Editor.UI.XycControl;

internal static class XycCameraValidator
{
    public static void Validate(XycCamera pose)
    {
        var values = new[] { pose.Position.X, pose.Position.Y, pose.Position.Z,
            pose.Forward.X, pose.Forward.Y, pose.Forward.Z, pose.Up.X,
            pose.Up.Y, pose.Up.Z, pose.FovDegrees, pose.NearPlane, pose.FarPlane,
            pose.OrthographicScale, pose.ObservationCenter.X,
            pose.ObservationCenter.Y, pose.ObservationCenter.Z };
        if (values.Any(value => !double.IsFinite(value))) throw new JsonException();
        if (pose.Mode is not ("Perspective" or "Orthographic") || pose.NearPlane <= 0 ||
            pose.FarPlane <= pose.NearPlane || pose.FovDegrees is <= 1 or >= 179 ||
            pose.Mode == "Orthographic" && pose.OrthographicScale <= 0)
            throw new JsonException();
        var forwardLength = Length(pose.Forward);
        var upLength = Length(pose.Up);
        if (forwardLength < 1e-8 || upLength < 1e-8 ||
            Math.Abs(Dot(pose.Forward, pose.Up) / (forwardLength * upLength)) > 1e-6)
            throw new JsonException();
    }

    static double Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    static double Length(Vector3 value) => Math.Sqrt(Dot(value, value));
}
