using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.StaticModels;

static class VulkanStaticModelValidator
{
    public static bool Validate(RenderStaticModelResource m, out string error)
    {
        error = "";
        if (!m.Key.IsValid) return Fail("invalid key", out error);
        if (m.Vertices.Count == 0) return Fail("empty vertices", out error);
        if (m.Indices.Count == 0) return Fail("empty indices", out error);
        if (m.Primitives.Count == 0) return Fail("empty primitives", out error);
        try
        {
            _ = checked(m.Vertices.Count * (int)VulkanStaticModelVertex.Stride);
            _ = checked(m.Indices.Count * sizeof(uint));
        }
        catch (OverflowException) { return Fail("byte size overflow", out error); }
        foreach (var p in m.Primitives)
        {
            if (p.FirstIndex < 0 || p.IndexCount <= 0) return Fail("bad primitive range", out error);
            if (p.BaseVertex != 0) return Fail("non-zero BaseVertex not supported", out error);
            if (p.FirstIndex + p.IndexCount > m.Indices.Count) return Fail("primitive overflow", out error);
        }
        return true;
    }

    static bool Fail(string reason, out string error) { error = reason; return false; }
}
