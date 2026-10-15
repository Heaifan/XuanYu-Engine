using System.Text.RegularExpressions;

internal sealed record ChunkRow(int X, int Y, int Lod, string Cull, string Result, int IndexCount);

internal static class B4FrameEvidence
{
    const byte SkyR = 89, SkyG = 140, SkyB = 204;

    public static int CountTerrain(byte[] rgba)
    {
        var count = 0;
        for (var i = 0; i < rgba.Length; i += 4)
            if (rgba[i] != SkyR || rgba[i + 1] != SkyG || rgba[i + 2] != SkyB) count++;
        return count;
    }

    public static IReadOnlyList<ChunkRow> ReadChunks(string path, long revision)
    {
        if (!File.Exists(path)) return [];
        var prefix = $"CameraRevision={revision}";
        return File.ReadLines(path).Where(x => x.Contains("[TERRAIN-CHUNK]", StringComparison.Ordinal)
                && x.Contains(prefix, StringComparison.Ordinal)).Select(ParseChunk).ToArray();
    }

    static ChunkRow ParseChunk(string line)
    {
        string Field(string key) => Regex.Match(line, $@"(?:^|;){key}=([^;]+)").Groups[1].Value;
        var xy = Field("Chunk").Split(',');
        return new(int.Parse(xy[0]), int.Parse(xy[1]), int.Parse(Field("LOD")),
            Field("CullResult"), Field("Result"), int.Parse(Field("IndexCount")));
    }
}
