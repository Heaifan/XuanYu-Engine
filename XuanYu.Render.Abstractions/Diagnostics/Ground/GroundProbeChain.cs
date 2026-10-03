using XuanYu.Core.Diagnostics;
using XuanYu.Core.Space;

namespace XuanYu.Render.Abstractions;

public static class GroundProbeChain
{
    static long _sequence;
    static long _frame = -1;
    static bool _seen, _g0, _g1, _g2, _g3, _map, _requested, _patch;
    public static long SequenceId => Interlocked.Read(ref _sequence);

    public static long Snapshot(long frame, MapRenderSnapshot map, bool hasWorldTerrain)
    {
        Interlocked.Increment(ref _sequence); _frame = frame;
        _seen = _g0 = _g1 = _g2 = _g3 = false; _map = map.HasMap; _requested = false; _patch = false;
        _g0 = true; _seen = true;
        Log($"G0;FrameId={frame};GroundSequenceId={SequenceId};Executed=YES;HasMap={YN(_map)};ShowGround={YN(map.ShowGround)};HasWorldTerrain={YN(hasWorldTerrain)};MapWidth={map.WidthMeters:0.###};MapDepth={map.DepthMeters:0.###};MapSnapshotIdentity={map.MapId};MapSnapshotRevision={map.SourceChangeSequence}");
        return SequenceId;
    }

    public static void FrameSeen(long frame, bool projectionAvailable)
    {
        if (_seen && _frame == frame) return;
        _frame = frame; _seen = true; _g0 = _g1 = _g2 = _g3 = false; _map = false; _requested = false; _patch = false;
        Log($"FRAME;FrameId={frame};GroundFrameSeen=YES;ProjectionAvailable={YN(projectionAvailable)};G0Executed=NO;G1Executed=NO;G2Executed=NO;G3Executed=NO");
    }

    public static void Patch(long frame, ViewProjectionState state, MapRenderSnapshot map,
        ReferencePlanePatchPlacement patch, bool entered, bool skipped, string reason, bool patchValid, bool vertexReady)
    {
        _g1 = true; _patch = patchValid;
        var samples = ReferencePlaneDiagnosticProbe.Measure(state, map.BaseHeightMeters, patch, out var valid);
        Log($"G1;FrameId={frame};GroundSequenceId={SequenceId};PatchMapSnapshotIdentity={map.MapId};PatchMapSnapshotRevision={map.SourceChangeSequence};PatchUpdateEntered={YN(entered)};PatchUpdateSkipped={YN(skipped)};SkipReason={reason};FootprintSampleCount=8;FootprintValidCount={valid};PatchValid={YN(patchValid)};PatchCenter=({patch.CenterX:0.###},{patch.CenterY:0.###});PatchHalfExtent=({patch.WidthMeters / 2:0.###},{patch.DepthMeters / 2:0.###});VertexBufferReady={YN(vertexReady)};CameraFarPlane={state.Camera.FarPlane:0.###};{samples}");
    }

    public static void DrawPlan(long frame, RenderProjection projection, bool requested, string reason)
    {
        _g2 = true; _requested = requested;
        Log($"G2;FrameId={frame};GroundSequenceId={SequenceId};HasMap={YN(projection.HasMap)};ShowGround={YN(projection.Map.ShowGround)};HasWorldTerrain={YN(projection.TerrainResources.Count != 0)};MapGroundRequested={YN(requested)};DecisionReason={reason}");
        if (_g0 && !_map && requested) Log("GROUND_DRAWPLAN_CONTRACT_MISMATCH");
    }

    public static void DrawCommand(long frame, bool issued, bool pipeline, bool vertex,
        bool index, int vertices, int indices, string vb, string ib, string reason)
    {
        _g3 = true;
        Log($"G3;FrameId={frame};GroundSequenceId={SequenceId};ActualDrawCommandIssued={YN(issued)};PipelineReady={YN(pipeline)};VertexBufferReady={YN(vertex)};IndexBufferReady={YN(index)};VertexBuffer={vb};IndexBuffer={ib};VertexCount={vertices};IndexCount={indices};ActualDrawSkipReason={reason}");
        if (_g2 && !_requested && issued) Log("GROUND_DRAW_WITHOUT_PLAN");
        if (_g1 && !_patch && issued) Log("GROUND_DRAW_WITH_INVALID_PATCH");
        if (_g2 && _requested && !issued) Log("GROUND_VULKAN_DRAW_GATE_BLOCKED");
    }

    public static void FrameSummary(long frame) =>
        Log($"SUMMARY;FrameId={frame};GroundFrameSeen=YES;G0Executed={YN(_g0)};G1Executed={YN(_g1)};G2Executed={YN(_g2)};G3Executed={YN(_g3)}");

    static void Log(string message) => ViewportProbe.Log("ground-chain", $"[{message}]");
    static string YN(bool value) => value ? "YES" : "NO";
}
