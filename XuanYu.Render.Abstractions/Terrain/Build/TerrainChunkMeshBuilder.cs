namespace XuanYu.Render.Abstractions;

public static class TerrainChunkMeshBuilder
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
                AddQuad(indices, y * xs.Count + x, (y + 1) * xs.Count + x, xs.Count);
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

    static void AddQuad(List<uint> indices, int topLeft, int bottomLeft, int width)
    {
        var topRight = topLeft + 1;
        var bottomRight = bottomLeft + 1;
        indices.AddRange([(uint)topLeft, (uint)bottomLeft, (uint)topRight,
            (uint)topRight, (uint)bottomLeft, (uint)bottomRight]);
    }

    static void AddSkirts(List<TerrainMeshVertex> vertices, List<uint> indices,
        TerrainHeightfield field, List<int> xs, List<int> ys, double exaggeration)
    {
        var surface = vertices.Count;
        AddEdge(vertices, indices, field, exaggeration, surface,
            Enumerable.Range(0, xs.Count).Select(x => x), xs.Count, 0, xs, ys, true);
        AddEdge(vertices, indices, field, exaggeration, surface,
            Enumerable.Range(0, xs.Count).Select(x => (ys.Count - 1) * xs.Count + x),
            xs.Count, ys.Count - 1, xs, ys, true);
        AddEdge(vertices, indices, field, exaggeration, surface,
            Enumerable.Range(0, ys.Count).Select(y => y * xs.Count), ys.Count, 0, ys, xs, false);
        AddEdge(vertices, indices, field, exaggeration, surface,
            Enumerable.Range(0, ys.Count).Select(y => y * xs.Count + xs.Count - 1),
            ys.Count, xs.Count - 1, ys, xs, false);
    }

    static void AddEdge(List<TerrainMeshVertex> vertices, List<uint> indices,
        TerrainHeightfield field, double exaggeration, int surface, IEnumerable<int> edge,
        int count, int fixedIndex, List<int> varying, List<int> other, bool horizontal)
    {
        var baseIndex = vertices.Count;
        foreach (var i in edge)
        {
            var v = vertices[i];
            vertices.Add(v with { Z = v.Z - Math.Max(1,
                Math.Max(field.CellSizeMeters, field.CellSizeYMeters) * exaggeration) });
        }
        var edgeArray = edge.ToArray();
        for (var i = 0; i < count - 1; i++)
        {
            var a = edgeArray[i]; var b = edgeArray[i + 1];
            var c = (uint)(baseIndex + i); var d = c + 1;
            indices.AddRange([(uint)a, c, (uint)b, (uint)b, c, d]);
        }
    }
}
