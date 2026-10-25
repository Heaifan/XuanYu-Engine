using XuanYu.Core.Math;

namespace XuanYu.Render.Abstractions;

public readonly record struct RenderStaticModelVertex(
    Vector3d Position,
    Vector3d Normal,
    double U,
    double V);
