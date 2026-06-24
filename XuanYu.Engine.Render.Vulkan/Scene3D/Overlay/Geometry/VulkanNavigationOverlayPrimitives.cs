namespace XuanYu.Engine.Render.Vulkan.Scene3D.Overlay;

/// <summary>Overlay 绘图图元：填充圆、圆环、粗线、轴端圆。</summary>
public static partial class VulkanNavigationOverlayGeometry
{
    static void DrawFilledCircle(List<VulkanOverlayVertex> verts, float cx, float cy, float r, (float R, float G, float B) c)
    {
        if (r < 0.5f) return;
        for (var i = 0; i < CircleSegments; i++)
        { var a1 = 2f * MathF.PI * i / CircleSegments; var a2 = 2f * MathF.PI * (i + 1) / CircleSegments;
            verts.Add(new VulkanOverlayVertex(cx, cy, c.R, c.G, c.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + MathF.Cos(a1) * r, cy + MathF.Sin(a1) * r, c.R, c.G, c.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + MathF.Cos(a2) * r, cy + MathF.Sin(a2) * r, c.R, c.G, c.B, 1)); }
    }

    static void DrawCircleOutline(List<VulkanOverlayVertex> verts, float cx, float cy, float r, float t, (float R, float G, float B) c)
    {
        if (r < 0.5f || t < 0.5f) return; var o = r + t * 0.5f; var n = r - t * 0.5f;
        for (var i = 0; i < CircleSegments; i++)
        { var a1 = 2f * MathF.PI * i / CircleSegments; var a2 = 2f * MathF.PI * (i + 1) / CircleSegments;
            var c1 = MathF.Cos(a1); var s1 = MathF.Sin(a1); var c2 = MathF.Cos(a2); var s2 = MathF.Sin(a2);
            verts.Add(new VulkanOverlayVertex(cx + c1 * n, cy + s1 * n, c.R, c.G, c.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + c2 * o, cy + s2 * o, c.R, c.G, c.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + c2 * n, cy + s2 * n, c.R, c.G, c.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + c1 * n, cy + s1 * n, c.R, c.G, c.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + c1 * o, cy + s1 * o, c.R, c.G, c.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + c2 * o, cy + s2 * o, c.R, c.G, c.B, 1)); }
    }

    static void DrawThickLine(List<VulkanOverlayVertex> verts, float x1, float y1, float x2, float y2, float t, (float R, float G, float B) c)
    {
        var dx = x2 - x1; var dy = y2 - y1; var len = MathF.Sqrt(dx * dx + dy * dy);
        if (len < 0.01f) return; dx /= len; dy /= len; var px = -dy * t * 0.5f; var py = dx * t * 0.5f;
        verts.Add(new VulkanOverlayVertex(x1 + px, y1 + py, c.R, c.G, c.B, 1));
        verts.Add(new VulkanOverlayVertex(x1 - px, y1 - py, c.R, c.G, c.B, 1));
        verts.Add(new VulkanOverlayVertex(x2 + px, y2 + py, c.R, c.G, c.B, 1));
        verts.Add(new VulkanOverlayVertex(x1 - px, y1 - py, c.R, c.G, c.B, 1));
        verts.Add(new VulkanOverlayVertex(x2 - px, y2 - py, c.R, c.G, c.B, 1));
        verts.Add(new VulkanOverlayVertex(x2 + px, y2 + py, c.R, c.G, c.B, 1));
    }

    static void DrawAxisEndCircle(List<VulkanOverlayVertex> verts, float cx, float cy, float r, (float R, float G, float B) c)
    {
        DrawFilledCircle(verts, cx, cy, r, c);
        if (r > 4f) DrawCircleOutline(verts, cx, cy, r, 1.5f, (c.R * 0.7f, c.G * 0.7f, c.B * 0.7f));
    }
}
