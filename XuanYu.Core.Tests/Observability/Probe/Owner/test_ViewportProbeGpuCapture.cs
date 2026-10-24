using XuanYu.Core.Diagnostics;

namespace XuanYu.Core.Tests.Observability.Probe.Owner;

public sealed class ViewportProbeGpuCaptureTests
{
    [Theory]
    [InlineData("B8G8R8A8Unorm")]
    [InlineData("B8G8R8A8Srgb")]
    public void UsesBgraEncoder(string format)
    {
        byte[] bgra = [30, 20, 10, 255];
        Assert.Equal(VulkanPngEncoder.EncodeBgra(1, 1, bgra),
            ViewportProbe.EncodeGpuPixels(1, 1, format, bgra));
    }

    [Theory]
    [InlineData("R8G8B8A8Unorm")]
    [InlineData("R8G8B8A8Srgb")]
    public void UsesRgbaEncoder(string format)
    {
        byte[] rgba = [10, 20, 30, 255];
        Assert.Equal(VulkanPngEncoder.EncodeRgba(1, 1, rgba),
            ViewportProbe.EncodeGpuPixels(1, 1, format, rgba));
    }

    [Fact]
    public void RejectsUnsupportedFormatBeforeWritingFiles()
    {
        Assert.Throws<NotSupportedException>(
            () => ViewportProbe.EncodeGpuPixels(1, 1, "Unknown", new byte[4]));
    }
}
