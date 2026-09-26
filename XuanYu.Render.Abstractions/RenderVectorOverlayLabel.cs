using XuanYu.Core.Math;

namespace XuanYu.Render.Abstractions;

public readonly record struct RenderVectorOverlayLabel(
    string Text,
    string CacheKey,
    Vector3d Anchor,
    double FontSizeDip,
    double DpiScale,
    RenderStaticModelColor Color);
