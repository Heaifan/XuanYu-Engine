using System.Numerics;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Core.Tests.Space;

public sealed class ReverseZCpuFloatQuantizationTests
{
    [Fact]
    public void ReverseZProductionMatrixImprovesCpuFloatDepthResolutionAtLongRange()
    {
        foreach (var distance in new[] { 1_000.0, 10_000, 20_000, 50_000, 100_000, 200_000, 500_000 })
        {
            var far = global::System.Math.Max(1_000.0, distance * 4.0);
            var reverse = Resolution(distance, 0.05, far, true);
            var forward = Resolution(distance, 0.05, far, false);
            Assert.True(reverse < forward, $"distance={distance}; forward={forward}; reverse={reverse}");
        }
    }

    static double Resolution(double distance, double near, double far, bool reverse)
    {
        var matrix = reverse
            ? ReverseZProjection.CreatePerspective(60, 4.0 / 3.0, near, far)
            : Matrix4x4.CreatePerspectiveFieldOfView((float)(60 * global::System.Math.PI / 180.0), 4.0f / 3.0f, (float)near, (float)far);
        var clip = Vector4.Transform(new Vector4(0, 0, (float)distance, 1), matrix);
        var depth = clip.Z / clip.W;
        var adjacent = reverse ? float.BitDecrement(depth) : float.BitIncrement(depth);
        Matrix4x4.Invert(matrix, out var inverse);
        var world = Vector4.Transform(new Vector4(0, 0, adjacent, 1), inverse);
        var adjacentDistance = world.Z / world.W;
        return global::System.Math.Abs(adjacentDistance - distance);
    }
}
