using XuanYu.Render.Vulkan.Render.Generation;
using Xunit;

namespace XuanYu.World.Tests.Render;

public class VulkanPresentGenerationPolicyTests
{
    [Theory]
    [InlineData(0u, 3u, true, true)]
    [InlineData(2u, 3u, true, true)]
    [InlineData(3u, 3u, true, false)]
    [InlineData(0u, 3u, false, false)]
    public void SelectsOnlyInRangeImagesFromCurrentGeneration(uint index, uint count,
        bool current, bool expected)
        => Assert.Equal(expected, VulkanPresentGenerationPolicy.CanSelectImage(index, count, current));

    [Fact]
    public void Reused_image_index_is_rejected_for_retired_generation()
    {
        const uint imageIndex = 2;
        Assert.True(VulkanPresentGenerationPolicy.CanSelectImage(imageIndex, 3, true));
        Assert.False(VulkanPresentGenerationPolicy.CanSelectImage(imageIndex, 3, false));
    }

    [Theory]
    [InlineData(true, true, false, false, true)]
    [InlineData(true, false, false, false, false)]
    [InlineData(false, true, false, false, false)]
    [InlineData(true, true, true, false, false)]
    [InlineData(false, false, true, true, false)]
    public void RetiresOnlyAfterCompletionProof(bool graphics, bool present, bool unknown,
        bool deviceLost, bool expected)
        => Assert.Equal(expected, VulkanPresentGenerationPolicy.CanRetire(graphics, present,
            unknown, deviceLost));
}
