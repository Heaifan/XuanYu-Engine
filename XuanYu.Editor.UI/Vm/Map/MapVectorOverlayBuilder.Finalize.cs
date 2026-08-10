using XuanYu.Core.Math;
using XuanYu.Core.Spatial;

namespace XuanYu.Editor.UI;

sealed partial class MapVectorOverlayBuilder
{
    SpatialAabb Bounds()
    {
        var points = _vertices.Select(x => x.Position).ToArray();
        if (points.Length == 0) return new(Vector3d.Zero, Vector3d.Zero);
        var min = new Vector3d(points.Min(x => x.X), points.Min(x => x.Y), points.Min(x => x.Z));
        var max = new Vector3d(points.Max(x => x.X), points.Max(x => x.Y), points.Max(x => x.Z));
        return new(min, max);
    }

    int Revision()
    {
        var hash = new HashCode();
        foreach (var v in _vertices)
        {
            hash.Add(v.Position); hash.Add(v.Secondary); hash.Add(v.U); hash.Add(v.V);
        }
        foreach (var i in _indices) hash.Add(i);
        foreach (var p in _primitives) hash.Add(p);
        return Math.Abs(hash.ToHashCode()) | 1;
    }
}
