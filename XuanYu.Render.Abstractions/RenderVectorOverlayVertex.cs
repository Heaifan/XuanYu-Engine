using XuanYu.Core.Math;

namespace XuanYu.Render.Abstractions;

public readonly record struct RenderVectorOverlayVertex(
    Vector3d Position, Vector3d Secondary, double U, double V);
