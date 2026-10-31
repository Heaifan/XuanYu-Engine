using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

static class TerrainContentFingerprint
{
    public static ulong ForHeightfield(TerrainHeightfield heightfield)
    {
        ArgumentNullException.ThrowIfNull(heightfield);
        var hash = 1469598103934665603UL;
        Add(ref hash, heightfield.Width); Add(ref hash, heightfield.Height);
        Add(ref hash, heightfield.CellSizeMeters);
        Add(ref hash, heightfield.CellSizeYMeters);
        for (var index = 0; index < heightfield.ElevationMeters.Count; index++)
        {
            Add(ref hash, heightfield.ElevationMeters[index]);
            Add(ref hash, heightfield.NoDataMask[index]);
        }
        return hash;
    }

    static void Add(ref ulong hash, int value) => Add(ref hash, unchecked((ulong)value));
    static void Add(ref ulong hash, bool value) => Add(ref hash, value ? 1UL : 0UL);
    static void Add(ref ulong hash, double value) => Add(ref hash,
        unchecked((ulong)BitConverter.DoubleToInt64Bits(value)));

    static void Add(ref ulong hash, ulong value)
    {
        hash ^= value;
        hash *= 1099511628211UL;
    }
}
