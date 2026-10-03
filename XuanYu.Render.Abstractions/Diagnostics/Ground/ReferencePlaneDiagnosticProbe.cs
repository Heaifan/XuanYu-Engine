using System.Text;
using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Render.Abstractions;

static class ReferencePlaneDiagnosticProbe
{
    const double ParallelEpsilon = 0.001;

    public static string Measure(ViewProjectionState state, double height,
        ReferencePlanePatchPlacement patch, out int validCount)
    {
        Span<Sample> samples = stackalloc Sample[8];
        var v = state.Viewport;
        samples[0] = Probe(state, v.LogicalX, v.LogicalY + v.LogicalHeight, 0, height);
        samples[1] = Probe(state, v.LogicalX + v.LogicalWidth / 2, v.LogicalY + v.LogicalHeight, 1, height);
        samples[2] = Probe(state, v.LogicalX + v.LogicalWidth, v.LogicalY + v.LogicalHeight, 2, height);
        samples[3] = Probe(state, v.LogicalX, v.LogicalY + v.LogicalHeight / 2, 3, height);
        samples[4] = Probe(state, v.LogicalX + v.LogicalWidth / 2, v.LogicalY + v.LogicalHeight / 2, 4, height);
        samples[5] = Probe(state, v.LogicalX + v.LogicalWidth, v.LogicalY + v.LogicalHeight / 2, 5, height);
        samples[6] = Probe(state, v.LogicalX, v.LogicalY, 6, height);
        samples[7] = Probe(state, v.LogicalX + v.LogicalWidth / 2, v.LogicalY, 7, height);
        var topRight = Probe(state, v.LogicalX + v.LogicalWidth, v.LogicalY, 8, height);
        validCount = 0;
        var minX = double.PositiveInfinity; var maxX = double.NegativeInfinity;
        var minY = double.PositiveInfinity; var maxY = double.NegativeInfinity;
        var minXBy = "N/A"; var maxXBy = "N/A"; var minYBy = "N/A"; var maxYBy = "N/A";
        var text = new StringBuilder(4096);
        text.Append($"CameraPosition={state.Camera.Position};CameraForward={state.Camera.Forward};CameraUp={state.Camera.Up};CameraFov={state.Camera.VerticalFovDegrees:0.###};CameraNearPlane={state.Camera.NearPlane:0.###};CameraFarPlane={state.Camera.FarPlane:0.###};ViewportWidth={v.LogicalWidth:0.###};ViewportHeight={v.LogicalHeight:0.###};RenderOrigin={state.RenderOrigin};");
        for (var i = 0; i < 8; i++) Append(ref validCount, ref minX, ref maxX, ref minY, ref maxY, ref minXBy, ref maxXBy, ref minYBy, ref maxYBy, samples[i], text);
        Append(ref validCount, ref minX, ref maxX, ref minY, ref maxY, ref minXBy, ref maxXBy, ref minYBy, ref maxYBy, topRight, text);
        text.Append($"ValidGroundBounds=({minX:0.###},{maxX:0.###},{minY:0.###},{maxY:0.###});MinXDefinedBy={minXBy};MaxXDefinedBy={maxXBy};MinYDefinedBy={minYBy};MaxYDefinedBy={maxYBy};");
        AppendPatchProjection(state, patch, height, text);
        return text.ToString();
    }

    static Sample Probe(ViewProjectionState state, double x, double y, int name, double height)
    {
        var ray = WorldRayFactory.FromViewportPoint(state, x, y);
        var parallel = !double.IsFinite(ray.Direction.Z) || System.Math.Abs(ray.Direction.Z) < ParallelEpsilon;
        var distance = parallel ? double.NaN : (height - ray.Origin.Z) / ray.Direction.Z;
        var far = double.IsFinite(distance) && distance > state.Camera.FarPlane;
        var valid = !parallel && double.IsFinite(distance) && distance > 0 && !far;
        var world = valid ? ray.Origin + ray.Direction * distance : default;
        return new(name, x, y, ray.Origin, ray.Direction, parallel, far, valid, distance, world);
    }

    static void Append(ref int count, ref double minX, ref double maxX, ref double minY, ref double maxY, ref string minXBy, ref string maxXBy, ref string minYBy, ref string maxYBy, Sample p, StringBuilder text)
    {
        if (p.Valid) { count++; if (p.World.X < minX) { minX = p.World.X; minXBy = Name(p.NameId); } if (p.World.X > maxX) { maxX = p.World.X; maxXBy = Name(p.NameId); } if (p.World.Y < minY) { minY = p.World.Y; minYBy = Name(p.NameId); } if (p.World.Y > maxY) { maxY = p.World.Y; maxYBy = Name(p.NameId); } }
        text.Append($"{Name(p.NameId)}=Screen=({p.X:0.###},{p.Y:0.###});RayOrigin={p.Origin};RayDirection={p.Direction};RayDirectionZ={p.Direction.Z:0.######};IntersectionValid={Yn(p.Valid)};IntersectionDistance={p.Distance:0.###};RejectedByParallelEpsilon={Yn(p.Parallel)};RejectedByFarPlane={Yn(p.Far)};WorldIntersection={(p.Valid ? p.World.ToString() : "N/A")};");
    }

    static void AppendPatchProjection(ViewProjectionState state, ReferencePlanePatchPlacement patch, double height, StringBuilder text)
    {
        Span<Vector3d> corners = stackalloc Vector3d[4]; corners[0] = new(patch.MinX, patch.MinY, height); corners[1] = new(patch.MaxX, patch.MinY, height); corners[2] = new(patch.MaxX, patch.MaxY, height); corners[3] = new(patch.MinX, patch.MaxY, height);
        var minX = double.PositiveInfinity; var maxX = double.NegativeInfinity; var minY = double.PositiveInfinity; var maxY = double.NegativeInfinity; var projectedCount = 0;
        for (var i = 0; i < 4; i++) { var projected = state.TryProjectWorldPoint(corners[i], out var p); text.Append($"PatchCorner{i}Screen={(projected ? $"({p.X:0.###},{p.Y:0.###})" : "N/A")};"); if (projected) { projectedCount++; minX = System.Math.Min(minX, p.X); maxX = System.Math.Max(maxX, p.X); minY = System.Math.Min(minY, p.Y); maxY = System.Math.Max(maxY, p.Y); } }
        text.Append($"PatchProjectedCornerCount={projectedCount};PatchProjectedMinX={minX:0.###};PatchProjectedMaxX={maxX:0.###};PatchProjectedMinY={minY:0.###};PatchProjectedMaxY={maxY:0.###};GroundVisibleViewportRegion=viewport-samples;ViewportGroundSamplesInsidePatch=PROBE_BY_WORLD_CONTAINS;PatchWorldBounds=({patch.MinX:0.###},{patch.MaxX:0.###},{patch.MinY:0.###},{patch.MaxY:0.###});PatchRenderBounds=({patch.MinX - state.RenderOrigin.X:0.###},{patch.MaxX - state.RenderOrigin.X:0.###},{patch.MinY - state.RenderOrigin.Y:0.###},{patch.MaxY - state.RenderOrigin.Y:0.###});PATCH_EDGE_EXPOSED={(projectedCount > 0 && (minY > state.Viewport.LogicalY || maxY < state.Viewport.LogicalY + state.Viewport.LogicalHeight) ? "YES" : "NO")};");
    }

    readonly record struct Sample(int NameId, double X, double Y, Vector3d Origin, Vector3d Direction, bool Parallel, bool Far, bool Valid, double Distance, Vector3d World);
    static string Name(int id) => id switch { 0 => "BottomLeft", 1 => "BottomCenter", 2 => "BottomRight", 3 => "CenterLeft", 4 => "Center", 5 => "CenterRight", 6 => "TopLeft", 7 => "TopCenter", _ => "TopRight" };
    static string Yn(bool value) => value ? "YES" : "NO";
}
