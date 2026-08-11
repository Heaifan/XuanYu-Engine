using System.Security.Cryptography;
using System.Text;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapDocument;

public static class MapDatasetLayerIdProjection
{
    const string Prefix = "xuanyu-dataset-layer-v1:";

    public static MapLayerId Project(string datasetId)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(Prefix + datasetId));
        var value = Convert.ToHexString(bytes.AsSpan(0, 16)).ToLowerInvariant();
        return MapLayerId.TryParse(value, out var layerId)
            ? layerId : throw new InvalidOperationException("Dataset Layer ID 投影无效。");
    }
}
