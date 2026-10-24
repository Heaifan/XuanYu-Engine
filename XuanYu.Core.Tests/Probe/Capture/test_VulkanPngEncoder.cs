using XuanYu.Core.Diagnostics;

namespace XuanYu.Core.Tests.Probe.Capture;

public sealed class VulkanPngEncoderTests
{
    [Fact]
    public void EquivalentBgraAndRgbaProduceSamePng()
    {
        byte[] bgra = [30, 20, 10, 255];
        byte[] rgba = [10, 20, 30, 255];
        var first = VulkanPngEncoder.EncodeBgra(1, 1, bgra);
        var second = VulkanPngEncoder.EncodeRgba(1, 1, rgba);
        Assert.Equal(second, first);
        Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, first[..8]);
    }

    [Fact]
    public void RejectsInvalidDimensionsOrPixelLengths()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => VulkanPngEncoder.EncodeRgba(0, 1, new byte[4]));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => VulkanPngEncoder.EncodeRgba(1, 1, new byte[3]));
    }
}
