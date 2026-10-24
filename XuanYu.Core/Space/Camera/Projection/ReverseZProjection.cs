using System.Numerics;

namespace XuanYu.Core.Space;

public static class ReverseZProjection
{
    public static Matrix4x4 CreatePerspective(double verticalFovDegrees, double aspect, double near, double far)
    {
        ValidatePerspective(verticalFovDegrees, aspect, near, far);
        var y = 1.0 / global::System.Math.Tan(verticalFovDegrees * global::System.Math.PI / 360.0);
        var x = y / aspect;
        var range = far - near;
        return new Matrix4x4(
            (float)x, 0, 0, 0,
            0, (float)y, 0, 0,
            0, 0, (float)(near / range), -1,
            0, 0, (float)(near * far / range), 0);
    }

    public static Matrix4x4 CreateOrthographic(double width, double height, double near, double far)
    {
        ValidatePositive(width, nameof(width));
        ValidatePositive(height, nameof(height));
        ValidatePlanes(near, far);
        var range = far - near;
        return new Matrix4x4(
            (float)(2.0 / width), 0, 0, 0,
            0, (float)(2.0 / height), 0, 0,
            0, 0, (float)(1.0 / range), 0,
            0, 0, (float)(far / range), 1);
    }

    static void ValidatePerspective(double fov, double aspect, double near, double far)
    {
        if (!double.IsFinite(fov) || fov <= 0 || fov >= 180) throw new ArgumentOutOfRangeException(nameof(fov));
        ValidatePositive(aspect, nameof(aspect));
        ValidatePlanes(near, far);
    }

    static void ValidatePlanes(double near, double far)
    {
        ValidatePositive(near, nameof(near));
        if (!double.IsFinite(far) || far <= near) throw new ArgumentOutOfRangeException(nameof(far));
    }

    static void ValidatePositive(double value, string name)
    {
        if (!double.IsFinite(value) || value <= 0) throw new ArgumentOutOfRangeException(name);
    }
}
