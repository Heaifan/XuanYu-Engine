using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Render.Vulkan.Scene3D.Overlay;

/// <summary>将 ViewportNavigationLayout 转换为 Overlay 三角形顶点。Build 入口 + 颜色工具。</summary>
public static partial class VulkanNavigationOverlayGeometry
{
    public const int MaxVertexCapacity = 4096;
    const int CircleSegments = 20;
    const float HoverBrighten = 0.3f;
    const float PressedDarken = 0.2f;

    public static VulkanOverlayVertex[] Build(ViewportNavigationLayout layout,
        ViewportNavigationElement hovered, ViewportNavigationElement pressed,
        string projectionModeText)
    {
        var verts = new List<VulkanOverlayVertex>(MaxVertexCapacity);
        var scale = layout.Scale; var cx = layout.GizmoCenterX; var cy = layout.GizmoCenterY;
        var orbitOut = pressed == ViewportNavigationElement.GizmoCenter ? (0.92f, 0.92f, 0.96f)
            : hovered == ViewportNavigationElement.GizmoCenter ? (0.72f, 0.72f, 0.78f) : (0.30f, 0.30f, 0.35f);
        DrawCircleOutline(verts, cx, cy, layout.GizmoOrbitCircle.Radius, 1.2f * scale, orbitOut);

        foreach (var p in layout.AxisProjections) { if (p.Depth >= 0) continue;
            DrawAxisEndCircle(verts, p.ScreenX, p.ScreenY, p.Radius, AdjustColor(p.Color, hovered == p.Element, pressed == p.Element)); }

        var axisLen = ViewportNavigationLayout.AxisLength;
        foreach (var p in layout.AxisProjections) {
            var dx = (p.ScreenX - cx) / (axisLen * scale); var dy = (p.ScreenY - cy) / (axisLen * scale);
            var len = MathF.Sqrt(dx * dx + dy * dy); if (len < 0.01f) continue; dx /= len; dy /= len;
            DrawThickLine(verts, cx, cy, p.ScreenX, p.ScreenY, (p.Depth > 0 ? 2f : 1.2f) * scale, p.Color); }

        var cc = pressed == ViewportNavigationElement.GizmoCenter ? (0.85f, 0.85f, 0.90f)
            : hovered == ViewportNavigationElement.GizmoCenter ? (0.65f, 0.65f, 0.72f) : (0.40f, 0.40f, 0.45f);
        DrawFilledCircle(verts, cx, cy, layout.GizmoCenterCircle.Radius, cc);
        DrawCircleOutline(verts, cx, cy, layout.GizmoCenterCircle.Radius + 2f * scale, 1.5f * scale, (0.5f, 0.5f, 0.55f));

        foreach (var p in layout.AxisProjections) { if (p.Depth < 0) continue;
            DrawAxisEndCircle(verts, p.ScreenX, p.ScreenY, p.Radius, AdjustColor(p.Color, hovered == p.Element, pressed == p.Element)); }

        foreach (var p in layout.AxisProjections) { if (p.Depth < 0 || p.Radius <= 4f) continue;
            DrawLetter(verts, p.ScreenX, p.ScreenY, p.Radius * 0.42f, GetAxisLabel(p.Element), (0.96f, 0.96f, 0.98f)); }

        DrawNavigationButton(verts, layout.PanButtonRect, hovered == ViewportNavigationElement.PanButton, pressed == ViewportNavigationElement.PanButton, "Pan");
        DrawNavigationButton(verts, layout.FrameButtonRect, hovered == ViewportNavigationElement.FrameButton, pressed == ViewportNavigationElement.FrameButton, "Frame");
        DrawNavigationButton(verts, layout.ProjectionButtonRect, hovered == ViewportNavigationElement.ProjectionButton, pressed == ViewportNavigationElement.ProjectionButton, projectionModeText);
        return verts.ToArray();
    }

    static (float R, float G, float B) AdjustColor((float R, float G, float B) c, bool h, bool p) =>
        p ? (c.R * PressedDarken, c.G * PressedDarken, c.B * PressedDarken)
        : h ? (MathF.Min(1, c.R + HoverBrighten), MathF.Min(1, c.G + HoverBrighten), MathF.Min(1, c.B + HoverBrighten)) : c;
}
