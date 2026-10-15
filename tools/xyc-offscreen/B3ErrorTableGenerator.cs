using System.Security.Cryptography;
using System.Text.Json;
using XuanYu.Core.Space.Diagnostics;
using XuanYu.Editor.UI;
using XuanYu.World.Terrain.Import;

internal static class B3ErrorTableGenerator
{
    public static int Run(string[] args)
    {
        var values = args.Chunk(2).ToDictionary(x => x[0], x => x[1], StringComparer.OrdinalIgnoreCase);
        var sourcePath = Path.GetFullPath(values["--source"]); var output = Path.GetFullPath(values["--out"]);
        var samples = int.Parse(values.GetValueOrDefault("--samples", "2049"));
        var cells = int.Parse(values.GetValueOrDefault("--chunk", "240"));
        var manifestPath = values.GetValueOrDefault("--manifest");
        if (File.Exists(output) || samples < 2 || cells < 1) throw new InvalidOperationException("输出已存在或参数无效");
        var tile = TerrainSourceImport.ReadMany([sourcePath]).Tiles.Single();
        var resource = tile.ToRenderSnapshot("n24e121", 1, samples);
        var chunks = string.IsNullOrWhiteSpace(manifestPath)
            ? CreateChunks(resource.Heightfield.Width, resource.Heightfield.Height, cells)
            : CreateSelectedChunks(resource.Heightfield.Width, resource.Heightfield.Height, cells, manifestPath);
        var source = new TerrainErrorGrid(tile.Width, tile.Height, tile.Raster.ElevationMeters,
            tile.Raster.NoDataMask);
        var render = new TerrainErrorGrid(resource.Heightfield.Width, resource.Heightfield.Height,
            resource.Heightfield.ElevationMeters, resource.Heightfield.NoDataMask);
        var report = TerrainMeshErrorAnalyzer.Analyze(source, render, chunks,
            new(SourceRowsSouthToNorth: true));
        var errors = report.RenderToMeshByChunkLod.Select(x => new
        { x.ChunkX, x.ChunkY, x.Lod, ValidCount = x.SourceToFinalMesh?.ValidCount ?? 0,
            P95VisualErrorMeters = x.SourceToFinalMesh?.P95VisualMeters ?? 0,
            MaxVisualErrorMeters = x.SourceToFinalMesh?.MaxVisualMeters ?? 0 }).ToArray();
        var document = new { schema = "XYC-B3-SSE-ERRORS/1", terrainId = "n24e121", revision = 1,
            samples, chunkCells = cells, sourceSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(sourcePath))),
            scope = string.IsNullOrWhiteSpace(manifestPath) ? "ALL_CHUNKS_ALL_LODS" : "VISIBLE_CHUNKS_SELECTED_LOD",
            sourceToRender = new { report.SourceToRender.ValidCount, report.SourceToRender.P95Meters,
                report.SourceToRender.P95VisualMeters, report.SourceToRender.MaxVisualMeters },
            sourceToFinalMesh = new { report.SourceToFinalMesh.ValidCount, report.SourceToFinalMesh.P95Meters,
                report.SourceToFinalMesh.P95VisualMeters, report.SourceToFinalMesh.MaxVisualMeters }, errors };
        Directory.CreateDirectory(Path.GetDirectoryName(output)!);
        File.WriteAllText(output, JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"B3 error table: {output}; samples={samples}; chunks={chunks.Select(x => (x.ChunkX, x.ChunkY)).Distinct().Count()}; records={errors.Length}");
        Console.WriteLine($"Source→Render P95={report.SourceToRender.P95VisualMeters:F3}m; final mesh max={report.SourceToFinalMesh.MaxVisualMeters:F3}m");
        return 0;
    }

    static IReadOnlyList<TerrainErrorChunk> CreateChunks(int width, int height, int cells)
    {
        var result = new List<TerrainErrorChunk>();
        for (var y = 0; y < height - 1; y += cells) for (var x = 0; x < width - 1; x += cells)
            for (var lod = 0; lod <= 4; lod++)
                result.Add(new(x / cells, y / cells, x, y, Math.Min(cells, width - 1 - x),
                    Math.Min(cells, height - 1 - y), lod, 1 << lod));
        return result;
    }

    static IReadOnlyList<TerrainErrorChunk> CreateSelectedChunks(int width, int height, int cells, string manifest)
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(manifest));
        var probes = doc.RootElement.GetProperty("Configurations")[0].GetProperty("ChunkProbes");
        var chunks = new List<TerrainErrorChunk>();
        foreach (var probe in probes.EnumerateArray())
        {
            if (probe.GetProperty("Result").GetString() != "DRAW_RECORDED") continue;
            var pair = probe.GetProperty("Chunk").GetString()!.Split(',');
            var x = int.Parse(pair[0]); var y = int.Parse(pair[1]);
            var lod = int.Parse(probe.GetProperty("Lod").GetString()!);
            var startX = x * cells; var startY = y * cells;
            chunks.Add(new(x, y, startX, startY, Math.Min(cells, width - 1 - startX),
                Math.Min(cells, height - 1 - startY), lod, 1 << lod));
        }
        if (chunks.Count == 0) throw new InvalidDataException("Manifest has no visible drawn chunks");
        return chunks;
    }
}
