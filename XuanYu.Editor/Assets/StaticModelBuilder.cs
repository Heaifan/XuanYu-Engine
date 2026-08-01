using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.Editor.Assets;

sealed class StaticModelBuilder
{
    readonly List<StaticModelVertex> _vertices = [];
    readonly List<uint> _indices = [];
    readonly List<StaticModelPrimitive> _primitives = [];
    Vector3d? _min;
    Vector3d? _max;

    public int VertexCount => _vertices.Count;
    public int IndexCount => _indices.Count;

    public void AddPrimitive(
        IReadOnlyList<StaticModelVertex> vertices,
        IReadOnlyList<uint> indices,
        StaticModelColor color)
    {
        var baseVertex = _vertices.Count;
        var firstIndex = _indices.Count;
        _vertices.AddRange(vertices);
        foreach (var index in indices) _indices.Add(checked((uint)(index + baseVertex)));
        _primitives.Add(new StaticModelPrimitive(firstIndex, indices.Count, baseVertex, color));
        foreach (var vertex in vertices) Include(vertex.Position);
    }

    public StaticModelData Build(string displayName, int sourceBytes, IReadOnlyList<StaticModelImportWarning> warnings)
    {
        if (_min is null || _max is null) throw new InvalidOperationException("模型没有 Bounds。");
        return new StaticModelData(_vertices.ToArray(), _indices.ToArray(), _primitives.ToArray(),
            new SpatialAabb(_min.Value, _max.Value),
            new StaticModelImportMetadata(displayName, "WORLD-C-R4-D1", sourceBytes), warnings);
    }

    void Include(Vector3d value)
    {
        if (!double.IsFinite(value.X) || !double.IsFinite(value.Y) || !double.IsFinite(value.Z))
            throw new InvalidOperationException("顶点包含非法数值。");
        _min = _min is null ? value : new Vector3d(Math.Min(_min.Value.X, value.X), Math.Min(_min.Value.Y, value.Y), Math.Min(_min.Value.Z, value.Z));
        _max = _max is null ? value : new Vector3d(Math.Max(_max.Value.X, value.X), Math.Max(_max.Value.Y, value.Y), Math.Max(_max.Value.Z, value.Z));
    }
}
