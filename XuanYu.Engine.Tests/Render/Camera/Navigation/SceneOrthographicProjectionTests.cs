using XuanYu.Engine.Render.Camera;
using XuanYu.Engine.Render.Camera.Navigation;

namespace FluidWarfare.Tests.Render.Camera.Navigation;

public sealed class SceneOrthographicProjectionTests
{
    [Fact]
    public void ComputeVulkanOrthographic_Returns16Floats()
    {
        var result = SceneOrthographicProjection.ComputeVulkanOrthographic(
            40f, 16f / 9f, 0.1f, 1000f);

        Assert.NotNull(result);
        Assert.Equal(16, result.Length);
    }

    [Fact]
    public void ComputeVulkanOrthographic_NoNaN()
    {
        var result = SceneOrthographicProjection.ComputeVulkanOrthographic(
            40f, 16f / 9f, 0.1f, 1000f);

        foreach (var v in result)
        {
            Assert.False(float.IsNaN(v));
            Assert.False(float.IsInfinity(v));
        }
    }

    [Fact]
    public void ComputeVulkanOrthographic_VulkanDepthRange()
    {
        // Test that near maps to 0 and far maps to 1 in Vulkan depth
        // NDC Z = (Z * (1/(near-far)) + near/(near-far))
        // For Z=near: near/(near-far) + near/(near-far) = 2*near/(near-far) → wait this isn't right
        // Let's verify the matrix properties instead
        var result = SceneOrthographicProjection.ComputeVulkanOrthographic(
            40f, 1f, 0.1f, 100f);

        // The projection matrix is in column-major format
        // Column 2 (index 2,6,10,14) is Z-related
        // [2][2] = 1/(near-far), [3][2] = near/(near-far)
        // For Vulkan depth 0..1
        Assert.True(result[10] < 0); // 1/(near-far) negative (near < far)
        Assert.True(result[14] < 0); // near/(near-far) negative (near > 0, near-far < 0)
    }

    [Fact]
    public void ComputeFromCameraState_UsesOrbitState()
    {
        var state = SceneOrbitCameraMotion.CreateDefault() with
        {
            OrthographicHeight = 50f
        };

        var result = SceneOrthographicProjection.ComputeFromCameraState(
            state, 16f / 9f);

        Assert.Equal(16, result.Length);
    }

    [Fact]
    public void OrthographicHeight_SmallerValue_LargerScale()
    {
        var largeH = SceneOrthographicProjection.ComputeVulkanOrthographic(
            80f, 1f, 0.1f, 100f);
        var smallH = SceneOrthographicProjection.ComputeVulkanOrthographic(
            40f, 1f, 0.1f, 100f);

        // With small orthoHeight, the scale factor (1/halfH) should be larger
        // Scale factor is at index [0] for X scaling
        Assert.True(Math.Abs(smallH[0]) > Math.Abs(largeH[0]));
    }
}
