using System.Buffers.Binary;
using XuanYu.World.Terrain.Import;

namespace XuanYu.World.Tests.Terrain;

static class TerrainImportAcceptanceFixture
{
    public sealed class ProgressCapture : IProgress<TerrainImportProgress>
    {
        public List<TerrainImportProgress> Values { get; } = [];
        public void Report(TerrainImportProgress value) => Values.Add(value);
    }

    public static MemoryStream HgtStream(params short[] values)
    {
        var stream = new MemoryStream();
        Span<byte> sample = stackalloc byte[2];
        foreach (var value in values)
        {
            BinaryPrimitives.WriteInt16BigEndian(sample, value);
            stream.Write(sample);
        }

        stream.Position = 0;
        return stream;
    }
}
