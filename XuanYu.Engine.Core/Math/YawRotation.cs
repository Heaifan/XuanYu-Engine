using System.Globalization;

namespace FluidWarfare.Core.Math;

public readonly record struct YawRotation
{
    private const double FullTurnDegrees = 360.0;

    private YawRotation(double degrees)
    {
        Degrees = NormalizeDegrees(degrees);
    }

    public double Degrees { get; }

    public double Radians => Degrees * global::System.Math.PI / 180.0;

    public Vector3d ForwardOnXZPlane => new(global::System.Math.Sin(Radians), 0.0, global::System.Math.Cos(Radians));

    public static YawRotation FromDegrees(double degrees)
    {
        if (!double.IsFinite(degrees))
        {
            throw new ArgumentOutOfRangeException(nameof(degrees), degrees, "Yaw 角度必须是有限数。");
        }

        return new YawRotation(degrees);
    }

    public static YawRotation FromRadians(double radians)
    {
        if (!double.IsFinite(radians))
        {
            throw new ArgumentOutOfRangeException(nameof(radians), radians, "Yaw 弧度必须是有限数。");
        }

        return new YawRotation(radians * 180.0 / global::System.Math.PI);
    }

    public override string ToString()
    {
        return string.Create(CultureInfo.InvariantCulture, $"YawRotation({Degrees:0.###}deg)");
    }

    private static double NormalizeDegrees(double degrees)
    {
        var normalized = degrees % FullTurnDegrees;

        return normalized < 0.0 ? normalized + FullTurnDegrees : normalized;
    }
}
