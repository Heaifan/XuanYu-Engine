using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Render;

public readonly record struct StartupGridState(
    bool GridAvailable,
    bool GridDrawPlanned,
    bool CameraValid,
    bool WorldPlaneVisible)
{
    public static StartupGridState From(RenderProjection projection)
    {
        var plan = RenderDrawPlan.GetFrameDrawPlan(projection);
        var grid = plan.Any(x => x.Kind == RenderDrawKind.EditorReferenceGrid);
        return new(projection.AssistState.ShowGrid, grid,
            projection.Camera.FarPlane > projection.Camera.NearPlane,
            grid);
    }
}
