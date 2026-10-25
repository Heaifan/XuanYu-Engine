namespace XuanYu.Render.Abstractions;

public static partial class TerrainChunkMeshBuilder
{
    static void AddSkirts(List<TerrainMeshVertex> vertices, List<uint> indices,
        TerrainHeightfield field, List<int> xs, List<int> ys, double exaggeration)
    {
        AddEdge(vertices, indices, field, exaggeration,
            Enumerable.Range(0, xs.Count).Select(x => x), xs.Count, 0, xs, ys, true);
        AddEdge(vertices, indices, field, exaggeration,
            Enumerable.Range(0, xs.Count).Select(x => (ys.Count - 1) * xs.Count + x),
            xs.Count, ys.Count - 1, xs, ys, true);
        AddEdge(vertices, indices, field, exaggeration,
            Enumerable.Range(0, ys.Count).Select(y => y * xs.Count), ys.Count, 0, ys, xs, false);
        AddEdge(vertices, indices, field, exaggeration,
            Enumerable.Range(0, ys.Count).Select(y => y * xs.Count + xs.Count - 1),
            ys.Count, xs.Count - 1, ys, xs, false);
    }

    static void AddEdge(List<TerrainMeshVertex> vertices, List<uint> indices,
        TerrainHeightfield field, double exaggeration, IEnumerable<int> edge,
        int count, int fixedIndex, List<int> varying, List<int> other, bool horizontal)
    {
        var edgeArray = edge.ToArray();
        var baseIndex = vertices.Count;
        foreach (var index in edgeArray)
        {
            var v = vertices[index];
            vertices.Add(v with { Z = v.Z - Math.Max(1,
                Math.Max(field.CellSizeMeters, field.CellSizeYMeters) * exaggeration) });
        }
        for (var i = 0; i < count - 1; i++)
        {
            var x0 = horizontal ? varying[i] : other[fixedIndex];
            var y0 = horizontal ? other[fixedIndex] : varying[i];
            var x1 = horizontal ? varying[i + 1] : other[fixedIndex];
            var y1 = horizontal ? other[fixedIndex] : varying[i + 1];
            if (Missing(field, x0, y0) || Missing(field, x1, y1)) continue;
            var a = edgeArray[i]; var b = edgeArray[i + 1];
            var c = (uint)(baseIndex + i); var d = c + 1;
            indices.AddRange([(uint)a, c, (uint)b, (uint)b, c, d]);
        }
    }
}
