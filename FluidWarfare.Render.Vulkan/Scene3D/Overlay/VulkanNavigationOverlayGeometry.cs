using FluidWarfare.Render.ViewportNavigation;

namespace FluidWarfare.Render.Vulkan.Scene3D.Overlay;

/// <summary>
/// 将 ViewportNavigationLayout 转换为 Overlay 三角形顶点。
/// 所有圆、线、字母都表示为三角形列表。
/// </summary>
public static class VulkanNavigationOverlayGeometry
{
    /// <summary>固定容量上限。</summary>
    public const int MaxVertexCapacity = 4096;

    /// <summary>圆的分段数。</summary>
    private const int CircleSegments = 20;

    // ─── 颜色常量 ────────────────────────────────────────────

    private const float HoverBrighten = 0.3f;
    private const float PressedDarken = 0.2f;

    // ─── 构建入口 ─────────────────────────────────────────────

    /// <summary>
    /// 根据布局生成三角形顶点。
    /// </summary>
    /// <param name="layout">平台无关布局。</param>
    /// <param name="hovered">当前 Hover 元素。</param>
    /// <param name="pressed">当前 Pressed 元素。</param>
    /// <param name="projectionModeText">投影模式显示文本（"Persp"/"Ortho"）。</param>
    /// <returns>顶点列表。</returns>
    public static VulkanOverlayVertex[] Build(
        ViewportNavigationLayout layout,
        ViewportNavigationElement hovered,
        ViewportNavigationElement pressed,
        string projectionModeText)
    {
        var verts = new List<VulkanOverlayVertex>(MaxVertexCapacity);

        var scale = layout.Scale;
        var cx = layout.GizmoCenterX;
        var cy = layout.GizmoCenterY;

        // 1. 背面轴端（先画）
        foreach (var proj in layout.AxisProjections)
        {
            if (proj.Depth >= 0) continue; // skip front
            DrawAxisEndCircle(verts, proj.ScreenX, proj.ScreenY, proj.Radius,
                AdjustColor(proj.Color, hovered == proj.Element, pressed == proj.Element));
        }

        // 2. 轴线（所有六条）
        foreach (var proj in layout.AxisProjections)
        {
            var axisLen = ViewportNavigationLayout.AxisLength;
            var dirX = (proj.ScreenX - cx) / (axisLen * scale);
            var dirY = (proj.ScreenY - cy) / (axisLen * scale);
            var len = MathF.Sqrt(dirX * dirX + dirY * dirY);
            if (len < 0.01f) continue;
            dirX /= len; dirY /= len;

            // 从中心到轴端的线段（带厚度）
            var thickness = (proj.Depth > 0 ? 2f : 1.2f) * scale;
            DrawThickLine(verts, cx, cy, proj.ScreenX, proj.ScreenY, thickness,
                AdjustColor(proj.Color, false, false));
        }

        // 3. 中心圆
        var centerColor = hovered == ViewportNavigationElement.GizmoCenter
            ? (0.6f, 0.6f, 0.65f)
            : (0.4f, 0.4f, 0.45f);
        DrawFilledCircle(verts, cx, cy, layout.GizmoCenterCircle.Radius, centerColor);
        // 中心外圈
        DrawCircleOutline(verts, cx, cy, layout.GizmoCenterCircle.Radius + 2f * scale, 1.5f * scale,
            (0.5f, 0.5f, 0.55f));

        // 4. 正面轴端（后画，在最上面）
        foreach (var proj in layout.AxisProjections)
        {
            if (proj.Depth < 0) continue; // skip back
            DrawAxisEndCircle(verts, proj.ScreenX, proj.ScreenY, proj.Radius,
                AdjustColor(proj.Color, hovered == proj.Element, pressed == proj.Element));
        }

        // 5. X/Y/Z 字母（只在正轴端画）
        foreach (var proj in layout.AxisProjections)
        {
            if (proj.Depth < 0) continue;
            var label = GetAxisLabel(proj.Element);
            if (label.Length > 0)
            {
                var lr = AdjustColor(proj.Color, false, false);
                // Only draw letter when radius > 4
                if (proj.Radius > 4f)
                    DrawLetter(verts, proj.ScreenX, proj.ScreenY, proj.Radius * 0.5f, label, lr);
            }
        }

        // 6. 四个导航按钮
        DrawNavigationButton(verts, layout.ZoomButtonRect, hovered == ViewportNavigationElement.ZoomButton,
            pressed == ViewportNavigationElement.ZoomButton, "Zoom");
        DrawNavigationButton(verts, layout.PanButtonRect, hovered == ViewportNavigationElement.PanButton,
            pressed == ViewportNavigationElement.PanButton, "Pan");
        DrawNavigationButton(verts, layout.FrameButtonRect, hovered == ViewportNavigationElement.FrameButton,
            pressed == ViewportNavigationElement.FrameButton, "Frame");
        DrawNavigationButton(verts, layout.ProjectionButtonRect, hovered == ViewportNavigationElement.ProjectionButton,
            pressed == ViewportNavigationElement.ProjectionButton, projectionModeText);

        return verts.ToArray();
    }

    // ─── 图元 ───────────────────────────────────────────────────

    private static void DrawFilledCircle(List<VulkanOverlayVertex> verts,
        float cx, float cy, float radius, (float R, float G, float B) color)
    {
        if (radius < 0.5f) return;
        // Triangle fan from center
        for (var i = 0; i < CircleSegments; i++)
        {
            var a1 = 2f * MathF.PI * i / CircleSegments;
            var a2 = 2f * MathF.PI * (i + 1) / CircleSegments;
            verts.Add(new VulkanOverlayVertex(cx, cy, color.R, color.G, color.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + MathF.Cos(a1) * radius, cy + MathF.Sin(a1) * radius, color.R, color.G, color.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + MathF.Cos(a2) * radius, cy + MathF.Sin(a2) * radius, color.R, color.G, color.B, 1));
        }
    }

    private static void DrawCircleOutline(List<VulkanOverlayVertex> verts,
        float cx, float cy, float radius, float thickness, (float R, float G, float B) color)
    {
        if (radius < 0.5f || thickness < 0.5f) return;
        var outer = radius + thickness * 0.5f;
        var inner = radius - thickness * 0.5f;
        for (var i = 0; i < CircleSegments; i++)
        {
            var a1 = 2f * MathF.PI * i / CircleSegments;
            var a2 = 2f * MathF.PI * (i + 1) / CircleSegments;
            var c1 = MathF.Cos(a1); var s1 = MathF.Sin(a1);
            var c2 = MathF.Cos(a2); var s2 = MathF.Sin(a2);
            verts.Add(new VulkanOverlayVertex(cx + c1 * inner, cy + s1 * inner, color.R, color.G, color.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + c2 * outer, cy + s2 * outer, color.R, color.G, color.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + c2 * inner, cy + s2 * inner, color.R, color.G, color.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + c1 * inner, cy + s1 * inner, color.R, color.G, color.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + c1 * outer, cy + s1 * outer, color.R, color.G, color.B, 1));
            verts.Add(new VulkanOverlayVertex(cx + c2 * outer, cy + s2 * outer, color.R, color.G, color.B, 1));
        }
    }

    private static void DrawThickLine(List<VulkanOverlayVertex> verts,
        float x1, float y1, float x2, float y2, float thickness,
        (float R, float G, float B) color)
    {
        var dx = x2 - x1;
        var dy = y2 - y1;
        var len = MathF.Sqrt(dx * dx + dy * dy);
        if (len < 0.01f) return;
        dx /= len; dy /= len;
        var px = -dy * thickness * 0.5f;
        var py = dx * thickness * 0.5f;

        verts.Add(new VulkanOverlayVertex(x1 + px, y1 + py, color.R, color.G, color.B, 1));
        verts.Add(new VulkanOverlayVertex(x1 - px, y1 - py, color.R, color.G, color.B, 1));
        verts.Add(new VulkanOverlayVertex(x2 + px, y2 + py, color.R, color.G, color.B, 1));
        verts.Add(new VulkanOverlayVertex(x1 - px, y1 - py, color.R, color.G, color.B, 1));
        verts.Add(new VulkanOverlayVertex(x2 - px, y2 - py, color.R, color.G, color.B, 1));
        verts.Add(new VulkanOverlayVertex(x2 + px, y2 + py, color.R, color.G, color.B, 1));
    }

    private static void DrawAxisEndCircle(List<VulkanOverlayVertex> verts,
        float cx, float cy, float radius, (float R, float G, float B) color)
    {
        DrawFilledCircle(verts, cx, cy, radius, color);
        // Axis end outline
        if (radius > 4f)
            DrawCircleOutline(verts, cx, cy, radius, 1.5f, (color.R * 0.7f, color.G * 0.7f, color.B * 0.7f));
    }

    // ─── 字母 ───────────────────────────────────────────────────

    private static string GetAxisLabel(ViewportNavigationElement element) => element switch
    {
        ViewportNavigationElement.PositiveX => "X",
        ViewportNavigationElement.PositiveY => "Y",
        ViewportNavigationElement.PositiveZ => "Z",
        _ => ""
    };

    private static void DrawLetter(List<VulkanOverlayVertex> verts,
        float cx, float cy, float size, string letter, (float R, float G, float B) color)
    {
        // Draw letters as simple cross/line patterns
        switch (letter)
        {
            case "X":
                // Two crossed lines
                DrawThickLine(verts, cx - size, cy - size, cx + size, cy + size, size * 0.4f, color);
                DrawThickLine(verts, cx + size, cy - size, cx - size, cy + size, size * 0.4f, color);
                break;
            case "Y":
                // Y shape: two upper diagonals + one vertical lower
                DrawThickLine(verts, cx - size, cy - size * 0.3f, cx, cy, size * 0.4f, color);
                DrawThickLine(verts, cx + size, cy - size * 0.3f, cx, cy, size * 0.4f, color);
                DrawThickLine(verts, cx, cy, cx, cy + size * 0.7f, size * 0.4f, color);
                break;
            case "Z":
                // Z shape: top, diagonal, bottom
                DrawThickLine(verts, cx - size, cy - size, cx + size, cy - size, size * 0.4f, color);
                DrawThickLine(verts, cx + size, cy - size, cx - size, cy + size, size * 0.4f, color);
                DrawThickLine(verts, cx - size, cy + size, cx + size, cy + size, size * 0.4f, color);
                break;
        }
    }

    // ─── 导航按钮 ───────────────────────────────────────────────

    private static void DrawNavigationButton(List<VulkanOverlayVertex> verts,
        ViewportNavigationLayout.Rect rect, bool hovered, bool pressed, string label)
    {
        var brightness = pressed ? 0.3f : (hovered ? 0.5f : 0.2f);
        var borderBrightness = pressed ? 0.6f : (hovered ? 0.9f : 0.4f);

        // Button background
        verts.Add(new VulkanOverlayVertex(rect.X, rect.Y, brightness, brightness, brightness, 0.8f));
        verts.Add(new VulkanOverlayVertex(rect.X + rect.W, rect.Y, brightness, brightness, brightness, 0.8f));
        verts.Add(new VulkanOverlayVertex(rect.X, rect.Y + rect.H, brightness, brightness, brightness, 0.8f));
        verts.Add(new VulkanOverlayVertex(rect.X + rect.W, rect.Y, brightness, brightness, brightness, 0.8f));
        verts.Add(new VulkanOverlayVertex(rect.X + rect.W, rect.Y + rect.H, brightness, brightness, brightness, 0.8f));
        verts.Add(new VulkanOverlayVertex(rect.X, rect.Y + rect.H, brightness, brightness, brightness, 0.8f));

        // Button border
        var border = 1.5f;
        var bx = rect.X; var by = rect.Y; var bw = rect.W; var bh = rect.H;
        DrawThickLine(verts, bx, by, bx + bw, by, border, (borderBrightness, borderBrightness, borderBrightness));
        DrawThickLine(verts, bx + bw, by, bx + bw, by + bh, border, (borderBrightness, borderBrightness, borderBrightness));
        DrawThickLine(verts, bx + bw, by + bh, bx, by + bh, border, (borderBrightness, borderBrightness, borderBrightness));
        DrawThickLine(verts, bx, by + bh, bx, by, border, (borderBrightness, borderBrightness, borderBrightness));

        // Button label as simple icon shapes
        var icx = bx + bw / 2f;
        var icy = by + bh / 2f;
        var isize = bw * 0.25f;
        var iconColor = (borderBrightness, borderBrightness, borderBrightness);

        switch (label)
        {
            case "Zoom":
                // Magnifying glass: circle + handle
                DrawCircleOutline(verts, icx - isize * 0.2f, icy - isize * 0.2f, isize * 0.6f, 2f, iconColor);
                DrawThickLine(verts, icx + isize * 0.3f, icy + isize * 0.3f, icx + isize * 0.8f, icy + isize * 0.8f, 3f, iconColor);
                break;
            case "Pan":
                // Small cross/hand shape
                DrawThickLine(verts, icx - isize, icy, icx + isize, icy, 2f, iconColor);
                DrawThickLine(verts, icx, icy - isize, icx, icy + isize, 2f, iconColor);
                break;
            case "Frame":
                // Crosshair: circle + cross
                DrawCircleOutline(verts, icx, icy, isize * 0.5f, 2f, iconColor);
                DrawThickLine(verts, icx - isize * 0.8f, icy, icx + isize * 0.8f, icy, 2f, iconColor);
                DrawThickLine(verts, icx, icy - isize * 0.8f, icx, icy + isize * 0.8f, 2f, iconColor);
                break;
            case "Persp":
                // Perspective: trapezoid
                DrawThickLine(verts, icx - isize * 0.8f, icy + isize * 0.6f, icx + isize * 0.8f, icy + isize * 0.6f, 2f, iconColor);
                DrawThickLine(verts, icx + isize * 0.8f, icy + isize * 0.6f, icx + isize * 0.5f, icy - isize * 0.6f, 2f, iconColor);
                DrawThickLine(verts, icx + isize * 0.5f, icy - isize * 0.6f, icx - isize * 0.5f, icy - isize * 0.6f, 2f, iconColor);
                DrawThickLine(verts, icx - isize * 0.5f, icy - isize * 0.6f, icx - isize * 0.8f, icy + isize * 0.6f, 2f, iconColor);
                break;
            case "Ortho":
                // Orthographic: rectangle
                DrawThickLine(verts, icx - isize * 0.6f, icy - isize * 0.6f, icx + isize * 0.6f, icy - isize * 0.6f, 2f, iconColor);
                DrawThickLine(verts, icx + isize * 0.6f, icy - isize * 0.6f, icx + isize * 0.6f, icy + isize * 0.6f, 2f, iconColor);
                DrawThickLine(verts, icx + isize * 0.6f, icy + isize * 0.6f, icx - isize * 0.6f, icy + isize * 0.6f, 2f, iconColor);
                DrawThickLine(verts, icx - isize * 0.6f, icy + isize * 0.6f, icx - isize * 0.6f, icy - isize * 0.6f, 2f, iconColor);
                break;
        }
    }

    // ─── 颜色调整 ───────────────────────────────────────────────

    private static (float R, float G, float B) AdjustColor(
        (float R, float G, float B) baseColor, bool hovered, bool pressed)
    {
        if (pressed) return (baseColor.R * PressedDarken, baseColor.G * PressedDarken, baseColor.B * PressedDarken);
        if (hovered) return (MathF.Min(1, baseColor.R + HoverBrighten), MathF.Min(1, baseColor.G + HoverBrighten), MathF.Min(1, baseColor.B + HoverBrighten));
        return baseColor;
    }
}
