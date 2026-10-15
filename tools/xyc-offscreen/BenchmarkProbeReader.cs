using System.Globalization;

internal static class BenchmarkProbeReader
{
    public static List<BenchmarkChunkProbe> Read(string path, string terrainId)
    {
        if (!File.Exists(path)) return [];
        return File.ReadLines(path).Where(line => line.Contains("[TERRAIN-CHUNK]", StringComparison.Ordinal)
                && line.Contains($"TerrainId={terrainId};", StringComparison.Ordinal))
            .Select(Parse).ToList();
    }

    static BenchmarkChunkProbe Parse(string line)
    {
        var fields = line.Split(';').Select(x => x.Split('=', 2))
            .Where(x => x.Length == 2).ToDictionary(x => x[0], x => x[1]);
        double.TryParse(fields.GetValueOrDefault("ProjectedPixels"), CultureInfo.InvariantCulture, out var pixels);
        return new(fields.GetValueOrDefault("Chunk", "UNKNOWN"), fields.GetValueOrDefault("LOD", "UNKNOWN"),
            fields.GetValueOrDefault("CullResult", "UNKNOWN"), pixels, fields.GetValueOrDefault("Result", "UNKNOWN"));
    }
}
