using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace XuanYu.Core.Diagnostics;

public static class VulkanPngEncoder
{
    static readonly byte[] Signature = [137, 80, 78, 71, 13, 10, 26, 10];

    public static byte[] EncodeBgra(int width, int height, ReadOnlySpan<byte> bgra)
        => Encode(width, height, bgra, true);

    public static byte[] EncodeRgba(int width, int height, ReadOnlySpan<byte> rgba)
        => Encode(width, height, rgba, false);

    static byte[] Encode(int width, int height, ReadOnlySpan<byte> pixels, bool bgra)
    {
        if (width <= 0 || height <= 0 || pixels.Length != checked(width * height * 4))
            throw new ArgumentOutOfRangeException(nameof(pixels));
        using var scanlines = new MemoryStream(checked((width * 4 + 1) * height));
        for (var y = 0; y < height; y++)
        {
            scanlines.WriteByte(0);
            var row = pixels.Slice(y * width * 4, width * 4);
            for (var x = 0; x < row.Length; x += 4)
            {
                scanlines.WriteByte(row[x + (bgra ? 2 : 0)]);
                scanlines.WriteByte(row[x + 1]);
                scanlines.WriteByte(row[x + (bgra ? 0 : 2)]);
                scanlines.WriteByte(row[x + 3]);
            }
        }
        using var compressed = new MemoryStream();
        using (var zlib = new ZLibStream(compressed, CompressionLevel.Fastest, true))
            zlib.Write(scanlines.GetBuffer().AsSpan(0, (int)scanlines.Length));
        using var png = new MemoryStream();
        png.Write(Signature);
        Span<byte> header = stackalloc byte[13];
        BinaryPrimitives.WriteUInt32BigEndian(header, (uint)width);
        BinaryPrimitives.WriteUInt32BigEndian(header[4..], (uint)height);
        header[8] = 8; header[9] = 6;
        WriteChunk(png, "IHDR", header);
        WriteChunk(png, "IDAT", compressed.ToArray());
        WriteChunk(png, "IEND", []);
        return png.ToArray();
    }

    static void WriteChunk(Stream output, string name, ReadOnlySpan<byte> payload)
    {
        Span<byte> size = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(size, (uint)payload.Length);
        output.Write(size);
        var type = Encoding.ASCII.GetBytes(name);
        output.Write(type); output.Write(payload);
        var crcInput = new byte[type.Length + payload.Length];
        type.CopyTo(crcInput, 0); payload.CopyTo(crcInput.AsSpan(type.Length));
        BinaryPrimitives.WriteUInt32BigEndian(size, Crc32(crcInput));
        output.Write(size);
    }

    static uint Crc32(ReadOnlySpan<byte> bytes)
    {
        var crc = uint.MaxValue;
        foreach (var value in bytes)
        {
            crc ^= value;
            for (var bit = 0; bit < 8; bit++)
                crc = (crc & 1) == 0 ? crc >> 1 : 0xEDB88320u ^ (crc >> 1);
        }
        return ~crc;
    }
}
