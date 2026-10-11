using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace XuanYu.Render.Abstractions;

static class TerrainSnapshotFingerprint
{
    static readonly ConcurrentDictionary<string, string> Cache = new();

    public static string Compute(TerrainRenderResource resource) => Cache.GetOrAdd(
        $"{resource.TerrainId}:{resource.Revision}", _ => Hash(resource));

    static string Hash(TerrainRenderResource resource)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        hash.AppendData(MemoryMarshal.AsBytes(
            resource.Heightfield.ElevationMeters.ToArray().AsSpan()));
        hash.AppendData(MemoryMarshal.AsBytes(
            resource.Heightfield.NoDataMask.ToArray().AsSpan()));
        return Convert.ToHexString(hash.GetHashAndReset());
    }
}
