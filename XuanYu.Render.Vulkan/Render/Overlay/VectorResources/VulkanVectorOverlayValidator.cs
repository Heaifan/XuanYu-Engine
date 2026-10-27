using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.VectorOverlay;

public static class VulkanVectorOverlayValidator
{
    public static bool Validate(RenderVectorOverlayResource resource, out string error)
    {
        error = "";
        if (!resource.Key.IsValid) return Fail("invalid key", out error);
        if (resource.Vertices.Count == 0 || resource.Indices.Count == 0)
            return Fail("empty geometry", out error);
        foreach (var index in resource.Indices)
            if (index >= resource.Vertices.Count) return Fail("index out of range", out error);
        foreach (var p in resource.Primitives)
        {
            if (p.FirstIndex < 0 || p.IndexCount <= 0 || p.IndexCount > resource.Indices.Count ||
                p.FirstIndex > resource.Indices.Count - p.IndexCount)
                return Fail("primitive range", out error);
            for (var offset = p.FirstIndex; offset < p.FirstIndex + p.IndexCount; offset++)
            {
                var finalIndex = (long)resource.Indices[offset] + p.BaseVertex;
                if (finalIndex < 0 || finalIndex >= resource.Vertices.Count)
                    return Fail("final index out of range", out error);
            }
        }
        return true;
    }

    static bool Fail(string reason, out string error) { error = reason; return false; }
}
