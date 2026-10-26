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
            if (p.FirstIndex < 0 || p.IndexCount <= 0 || p.FirstIndex + p.IndexCount > resource.Indices.Count)
                return Fail("primitive range", out error);
        return true;
    }

    static bool Fail(string reason, out string error) { error = reason; return false; }
}
