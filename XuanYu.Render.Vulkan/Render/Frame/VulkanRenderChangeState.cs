using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

[Flags]
internal enum VulkanCacheInvalidation
{
    None = 0, Scene = 1, MapSurface = 2, Terrain = 4,
    VectorOverlay = 8, StaticModels = 16
}

[Flags]
internal enum VulkanUploadRequirement
{
    None = 0, MapSurface = 1, Terrain = 2, VectorOverlay = 4, StaticModels = 8
}

internal readonly record struct VulkanRenderChangeSet(
    VulkanCacheInvalidation CacheInvalidation,
    VulkanUploadRequirement UploadRequired,
    bool FrameDirty);

internal static class VulkanRenderChangeConsumer
{
    public static VulkanRenderChangeSet Compare(RenderProjection? previous, RenderProjection next)
    {
        var invalidation = VulkanCacheInvalidation.None;
        if (previous is null || !Equals(previous.Value.Camera, next.Camera) ||
            !Equals(previous.Value.Entities, next.Entities)) invalidation |= VulkanCacheInvalidation.Scene;
        if (previous is null || previous.Value.Map != next.Map) invalidation |= VulkanCacheInvalidation.MapSurface;
        if (previous is null || !SameTerrain(previous.Value, next)) invalidation |= VulkanCacheInvalidation.Terrain;
        if (previous is null || !SameOverlay(previous.Value, next)) invalidation |= VulkanCacheInvalidation.VectorOverlay;
        if (previous is null || !SameModels(previous.Value, next)) invalidation |= VulkanCacheInvalidation.StaticModels;
        var upload = VulkanUploadRequirement.None;
        if (invalidation.HasFlag(VulkanCacheInvalidation.MapSurface)) upload |= VulkanUploadRequirement.MapSurface;
        if (invalidation.HasFlag(VulkanCacheInvalidation.Terrain)) upload |= VulkanUploadRequirement.Terrain;
        if (invalidation.HasFlag(VulkanCacheInvalidation.VectorOverlay)) upload |= VulkanUploadRequirement.VectorOverlay;
        if (invalidation.HasFlag(VulkanCacheInvalidation.StaticModels)) upload |= VulkanUploadRequirement.StaticModels;
        return new(invalidation, upload, invalidation != VulkanCacheInvalidation.None);
    }

    static bool SameTerrain(RenderProjection a, RenderProjection b) =>
        a.TerrainResources.Select(x => (x.TerrainId, x.Revision, x.CellSizeMeters, x.Heightfield.CellSizeYMeters,
            x.WorldOrigin)).SequenceEqual(b.TerrainResources.Select(x =>
            (x.TerrainId, x.Revision, x.CellSizeMeters, x.Heightfield.CellSizeYMeters, x.WorldOrigin)));

    static bool SameOverlay(RenderProjection a, RenderProjection b) =>
        a.VectorOverlayResources.Select(x => (x.Key, x.Revision)).SequenceEqual(
            b.VectorOverlayResources.Select(x => (x.Key, x.Revision)));

    static bool SameModels(RenderProjection a, RenderProjection b) =>
        a.StaticModelResources.Select(x => (x.Key, x.Revision)).SequenceEqual(
            b.StaticModelResources.Select(x => (x.Key, x.Revision)));
}
