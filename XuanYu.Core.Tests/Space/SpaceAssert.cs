using XuanYu.Core.Math;

namespace XuanYu.Core.Tests.Space;

static class SpaceAssert
{
    public static void Near(double expected, double actual)
    {
        Assert.Equal(expected, actual, precision: 6);
    }

    public static void Near(Vector3d expected, Vector3d actual)
    {
        Near(expected.X, actual.X);
        Near(expected.Y, actual.Y);
        Near(expected.Z, actual.Z);
    }
}
