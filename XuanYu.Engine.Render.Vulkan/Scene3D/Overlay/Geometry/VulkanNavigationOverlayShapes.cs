using XuanYu.Engine.Render.ViewportNavigation;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Overlay;

/// <summary>Overlay Shape 绘制：字母标签 + 导航按钮。</summary>
public static partial class VulkanNavigationOverlayGeometry
{
    static string GetAxisLabel(ViewportNavigationElement el) => el switch
    {
        ViewportNavigationElement.PositiveX => "X", ViewportNavigationElement.PositiveY => "Y",
        ViewportNavigationElement.PositiveZ => "Z", _ => ""
    };

    static void DrawLetter(List<VulkanOverlayVertex> verts, float cx, float cy, float sz, string l, (float R, float G, float B) c)
    {
        switch (l)
        {
            case "X":
                DrawThickLine(verts, cx - sz, cy - sz, cx + sz, cy + sz, sz * 0.4f, c);
                DrawThickLine(verts, cx + sz, cy - sz, cx - sz, cy + sz, sz * 0.4f, c); break;
            case "Y":
                DrawThickLine(verts, cx - sz, cy - sz * 0.3f, cx, cy, sz * 0.4f, c);
                DrawThickLine(verts, cx + sz, cy - sz * 0.3f, cx, cy, sz * 0.4f, c);
                DrawThickLine(verts, cx, cy, cx, cy + sz * 0.7f, sz * 0.4f, c); break;
            case "Z":
                DrawThickLine(verts, cx - sz, cy - sz, cx + sz, cy - sz, sz * 0.4f, c);
                DrawThickLine(verts, cx + sz, cy - sz, cx - sz, cy + sz, sz * 0.4f, c);
                DrawThickLine(verts, cx - sz, cy + sz, cx + sz, cy + sz, sz * 0.4f, c); break;
        }
    }

    static void DrawNavigationButton(List<VulkanOverlayVertex> verts, Rect rect, bool h, bool p, string label)
    {
        var bri = p ? 0.3f : (h ? 0.5f : 0.2f); var bb = p ? 0.6f : (h ? 0.9f : 0.4f);
        verts.Add(new VulkanOverlayVertex(rect.X, rect.Y, bri, bri, bri, 0.8f));
        verts.Add(new VulkanOverlayVertex(rect.X + rect.W, rect.Y, bri, bri, bri, 0.8f));
        verts.Add(new VulkanOverlayVertex(rect.X, rect.Y + rect.H, bri, bri, bri, 0.8f));
        verts.Add(new VulkanOverlayVertex(rect.X + rect.W, rect.Y, bri, bri, bri, 0.8f));
        verts.Add(new VulkanOverlayVertex(rect.X + rect.W, rect.Y + rect.H, bri, bri, bri, 0.8f));
        verts.Add(new VulkanOverlayVertex(rect.X, rect.Y + rect.H, bri, bri, bri, 0.8f));
        var b = 1.5f; var icx = rect.X + rect.W / 2f; var icy = rect.Y + rect.H / 2f; var isz = rect.W * 0.35f;
        var ic = (bb * 1.3f, bb * 1.3f, bb * 1.3f); var lw = 2.5f;
        DrawThickLine(verts, rect.X, rect.Y, rect.X + rect.W, rect.Y, b, (bb, bb, bb));
        DrawThickLine(verts, rect.X + rect.W, rect.Y, rect.X + rect.W, rect.Y + rect.H, b, (bb, bb, bb));
        DrawThickLine(verts, rect.X + rect.W, rect.Y + rect.H, rect.X, rect.Y + rect.H, b, (bb, bb, bb));
        DrawThickLine(verts, rect.X, rect.Y + rect.H, rect.X, rect.Y, b, (bb, bb, bb));
        switch (label)
        {
            case "Zoom":
                DrawCircleOutline(verts, icx - isz * 0.15f, icy - isz * 0.15f, isz * 0.55f, lw, ic);
                DrawThickLine(verts, icx + isz * 0.3f, icy + isz * 0.3f, icx + isz * 0.9f, icy + isz * 0.9f, lw + 0.5f, ic); break;
            case "Pan":
                DrawCircleOutline(verts, icx, icy + isz * 0.15f, isz * 0.35f, 2f, ic);
                for (var fi = 0; fi < 4; fi++) { var fx = icx - isz * 0.4f + fi * isz * 0.27f; var fy = icy - isz * 0.15f;
                    if (fi == 3) fx += isz * 0.08f; DrawThickLine(verts, fx, fy, fx, fy - isz * 0.35f, 2.5f, ic);
                    DrawFilledCircle(verts, fx, fy - isz * 0.35f, 2f, ic); } break;
            case "Frame":
                DrawCircleOutline(verts, icx, icy, isz * 0.45f, 2.5f, ic);
                DrawThickLine(verts, icx - isz * 0.8f, icy, icx + isz * 0.8f, icy, 2.5f, ic);
                DrawThickLine(verts, icx, icy - isz * 0.8f, icx, icy + isz * 0.8f, 2.5f, ic); break;
            case "Persp":
                DrawThickLine(verts, icx - isz * 0.9f, icy + isz * 0.7f, icx + isz * 0.9f, icy + isz * 0.7f, 2.5f, ic);
                DrawThickLine(verts, icx + isz * 0.9f, icy + isz * 0.7f, icx + isz * 0.5f, icy - isz * 0.7f, 2.5f, ic);
                DrawThickLine(verts, icx + isz * 0.5f, icy - isz * 0.7f, icx - isz * 0.5f, icy - isz * 0.7f, 2.5f, ic);
                DrawThickLine(verts, icx - isz * 0.5f, icy - isz * 0.7f, icx - isz * 0.9f, icy + isz * 0.7f, 2.5f, ic); break;
            case "Ortho":
                DrawThickLine(verts, icx - isz * 0.7f, icy - isz * 0.7f, icx + isz * 0.7f, icy - isz * 0.7f, 2.5f, ic);
                DrawThickLine(verts, icx + isz * 0.7f, icy - isz * 0.7f, icx + isz * 0.7f, icy + isz * 0.7f, 2.5f, ic);
                DrawThickLine(verts, icx + isz * 0.7f, icy + isz * 0.7f, icx - isz * 0.7f, icy + isz * 0.7f, 2.5f, ic);
                DrawThickLine(verts, icx - isz * 0.7f, icy + isz * 0.7f, icx - isz * 0.7f, icy - isz * 0.7f, 2.5f, ic); break;
        }
    }
}
