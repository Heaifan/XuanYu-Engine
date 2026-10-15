using System.Buffers.Binary;
using System.IO.Compression;
using XuanYu.Core.Diagnostics;

namespace XuanYu.World.Tests.Xyc;

public sealed class VulkanPngEncoderTests
{
    [Fact]
    public void Encodes_gpu_bgra_pixel_as_a_valid_rgba_png_scanline()
    {
        var png = VulkanPngEncoder.EncodeBgra(1, 1, [3, 2, 1, 255]);
        Assert.Equal(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }, png[..8]);
        var offset = 8;
        byte[] compressed = [];
        while (offset < png.Length)
        {
            var length = (int)BinaryPrimitives.ReadUInt32BigEndian(png.AsSpan(offset, 4));
            var type = System.Text.Encoding.ASCII.GetString(png, offset + 4, 4);
            if (type == "IDAT") compressed = png.AsSpan(offset + 8, length).ToArray();
            offset += 12 + length;
        }
        using var input = new MemoryStream(compressed);
        using var zlib = new ZLibStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        zlib.CopyTo(output);
        Assert.Equal(new byte[] { 0, 1, 2, 3, 255 }, output.ToArray());
    }
}
