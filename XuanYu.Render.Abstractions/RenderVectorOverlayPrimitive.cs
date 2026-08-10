namespace XuanYu.Render.Abstractions;

public enum RenderVectorOverlayPrimitiveKind
{
    Fill,
    Stroke,
    Marker
}

public readonly record struct RenderVectorOverlayPrimitive(
    int FirstIndex,
    int IndexCount,
    int BaseVertex,
    RenderVectorOverlayPrimitiveKind Kind,
    RenderStaticModelColor Color,
    double WidthDip,
    double RadiusDip);
