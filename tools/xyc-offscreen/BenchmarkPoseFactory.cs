using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

internal sealed record BenchmarkPose(string Name, RenderCameraProjection Camera,
    bool CameraInsideChunkAabb, string ChunkIdentity);

internal static class BenchmarkPoseFactory
{
    public static IReadOnlyList<BenchmarkPose> Create(TerrainRenderResource terrain)
    {
        var field = terrain.Heightfield;
        var width = (field.Width - 1) * field.CellSizeMeters;
        var height = (field.Height - 1) * field.CellSizeYMeters;
        var center = field.ElevationAt(field.Height / 2, field.Width / 2);
        var x = terrain.WorldOrigin.X + width / 2;
        var y = terrain.WorldOrigin.Y + height / 2;
        var span = Math.Max(width, height);
        return [
            TopDown("far-topdown", x, y, center + span * 1.15, span * 4, 1),
            TopDown("mid-2km", x, y, center + 2000, 100000, 2),
            Oblique("near-oblique", x - 800, y - 600, center + 250,
                x + 300, y + 200, center, span * 4, 3),
            ChunkEntry(terrain, x, y, center, span * 4, 4)
        ];
    }

    static BenchmarkPose TopDown(string name, double x, double y, double z, double far, int rev) =>
        new(name, new(new(x, y, z), new(0, 0, -1), new(0, 1, 0), 45, 0.1, far, rev), false, "NONE");

    static BenchmarkPose Oblique(string name, double x, double y, double z,
        double tx, double ty, double tz, double far, int rev)
    {
        var forward = (new Vector3d(tx, ty, tz) - new Vector3d(x, y, z)).Normalize();
        var up = (Vector3d.UnitZ - forward * forward.Dot(Vector3d.UnitZ)).Normalize();
        return new(name, new(new(x, y, z), forward, up, 45, 0.1, far, rev), false, "NEAR_TILE_CENTER");
    }

    static BenchmarkPose ChunkEntry(TerrainRenderResource terrain, double x, double y,
        double ground, double far, int rev)
    {
        var field = terrain.Heightfield;
        var chunks = TerrainChunkPartitioner.Partition(field, terrain.TerrainId, terrain.Revision);
        var sampleX = field.Width / 2; var sampleY = field.Height / 2;
        var chunk = chunks.FirstOrDefault(c => sampleX >= c.StartSampleX &&
            sampleX <= c.StartSampleX + c.CellCountX && sampleY >= c.StartSampleY &&
            sampleY <= c.StartSampleY + c.CellCountY);
        var inside = chunk is not null && x - terrain.WorldOrigin.X >= chunk.WorldBounds.MinX &&
            x - terrain.WorldOrigin.X <= chunk.WorldBounds.MaxX &&
            y - terrain.WorldOrigin.Y >= chunk.WorldBounds.MinY &&
            y - terrain.WorldOrigin.Y <= chunk.WorldBounds.MaxY &&
            ground >= chunk.WorldBounds.MinZ && ground <= chunk.WorldBounds.MaxZ;
        var pose = Oblique("chunk-aabb-entry", x, y, ground, x + 500, y + 250,
            ground - 300, far, rev);
        return pose with { CameraInsideChunkAabb = inside,
            ChunkIdentity = chunk is null ? "UNKNOWN" : $"{chunk.ChunkX},{chunk.ChunkY}" };
    }
}
