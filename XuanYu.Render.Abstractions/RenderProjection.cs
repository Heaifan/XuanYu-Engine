using XuanYu.Core.Map;
using XuanYu.Core.Math;

namespace XuanYu.Render.Abstractions;

public readonly record struct RenderProjection(
    RenderCameraProjection Camera,
    IReadOnlyList<RenderEntityProjection> Entities,
    bool GizmoVisible,
    Vector3d GizmoPosition,
    bool RotateGizmoVisible = false,
    double RotateGizmoWorldRadius = 1.2,
    bool ScaleGizmoVisible = false,
    double ScaleGizmoWorldRadius = 1.2,
    Vector3d GizmoRotation = default,
    EditorViewportAssistState Assist = default,
    double MoveGizmoWorldRadius = 1.2,
    IReadOnlyList<RenderStaticModelResource>? StaticModels = null,
    MapRenderSnapshot Map = default,
    double ViewportDpiScale = 1.0)
{
    public int EntityCount => Entities.Count;
    public EditorViewportAssistState AssistState => Assist;
    public IReadOnlyList<RenderStaticModelResource> StaticModelResources => StaticModels ?? [];
    public bool HasMap => Map.HasMap;
}
