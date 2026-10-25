namespace XuanYu.Render.Abstractions;

public static partial class TerrainChunkMeshBuilder
{
    public static TerrainChunkMesh Build(TerrainHeightfield field,
        TerrainChunkDescriptor chunk, TerrainLodLevel lodLevel, double verticalExaggeration)
    {
        ArgumentNullException.ThrowIfNull(field);
        ArgumentNullException.ThrowIfNull(chunk);
        if (!chunk.HasValidSampleRange(field))
            throw new ArgumentException("Chunk 的采样范围超出传入的 Heightfield。", nameof(chunk));
        if (!double.IsFinite(verticalExaggeration) || verticalExaggeration <= 0)
            throw new ArgumentOutOfRangeException(nameof(verticalExaggeration));
        var stride = lodLevel.Stride();
        var transform = new TerrainRenderTransform(verticalExaggeration);
        var xs = Coordinates(chunk.StartSampleX, chunk.CellCountX, stride);
        var ys = Coordinates(chunk.StartSampleY, chunk.CellCountY, stride);
        var vertices = new List<TerrainMeshVertex>(xs.Count * ys.Count);
        foreach (var y in ys)
            foreach (var x in xs)
                vertices.Add(Vertex(field, chunk, x, y, transform));
        var indices = new List<uint>();
        for (var y = 0; y < ys.Count - 1; y++)
            for (var x = 0; x < xs.Count - 1; x++)
            {
                if (Missing(field, xs[x], ys[y]) || Missing(field, xs[x + 1], ys[y]) ||
                    Missing(field, xs[x], ys[y + 1]) || Missing(field, xs[x + 1], ys[y + 1]))
                    continue;
                AddQuad(indices, y * xs.Count + x, (y + 1) * xs.Count + x, xs.Count);
            }
        AddSkirts(vertices, indices, field, xs, ys, verticalExaggeration);
        return new(vertices, indices, xs.Count * ys.Count, stride);
    }

    static List<int> Coordinates(int start, int cells, int stride)
    {
        var result = new List<int>();
        for (var offset = 0; offset < cells; offset += stride) result.Add(start + offset);
        if (result.Count == 0 || result[^1] != start + cells) result.Add(start + cells);
        return result;
    }

    static TerrainMeshVertex Vertex(TerrainHeightfield field, TerrainChunkDescriptor chunk,
        int x, int y, TerrainRenderTransform transform)
    {
        var normal = TerrainNormalBuilder.Build(field, field.CellSizeMeters,
            field.CellSizeYMeters, transform, y, x);
        return new((x - chunk.StartSampleX) * field.CellSizeMeters,
            (y - chunk.StartSampleY) * field.CellSizeYMeters,
            transform.VisualHeight(field.ElevationAt(y, x)),
            normal.X, normal.Y, normal.Z);
    }

    static bool Missing(TerrainHeightfield f, int x, int y) =>
        f.NoDataMask[y * f.Width + x];

    static void AddQuad(List<uint> indices, int topLeft, int bottomLeft, int width)
    {
        var topRight = topLeft + 1;
        var bottomRight = bottomLeft + 1;
        indices.AddRange([(uint)topLeft, (uint)bottomLeft, (uint)topRight,
            (uint)topRight, (uint)bottomLeft, (uint)bottomRight]);
    }

}
