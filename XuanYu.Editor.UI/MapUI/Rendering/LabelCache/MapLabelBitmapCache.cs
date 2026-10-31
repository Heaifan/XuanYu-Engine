using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed class MapLabelBitmapCache : IDisposable
{
    readonly Dictionary<string, RenderLabelBitmap> _entries = [];

    public int Count => _entries.Count;

    public RenderLabelBitmap GetOrCreate(RenderVectorOverlayLabel label)
    {
        if (_entries.TryGetValue(label.CacheKey, out var bitmap)) return bitmap;
        bitmap = MapLabelRasterizer.Rasterize(label);
        _entries[label.CacheKey] = bitmap;
        return bitmap;
    }

    public bool Remove(string cacheKey) => _entries.Remove(cacheKey);

    public int Trim(Func<RenderLabelBitmap, bool> keep)
    {
        var removed = 0;
        foreach (var key in _entries.Where(x => !keep(x.Value)).Select(x => x.Key).ToArray())
            if (_entries.Remove(key)) removed++;
        return removed;
    }

    public void Clear() => _entries.Clear();

    public void Dispose() => Clear();
}
