using XuanYu.Core.Math;
using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public enum OrbitProbePhase { Begin, Move, End }

public sealed record OrbitProbeEvent(
    long OrbitSessionId,
    OrbitProbePhase Phase,
    OrbitPivotSource PivotSource,
    Vector3d Pivot,
    string SurfaceSource,
    GroundPickResult TerrainHit,
    GroundPickResult ReferencePlaneHit,
    Vector3d MouseDelta,
    int PivotResolveCount);

public sealed record OrbitSessionState(
    long SessionId,
    OrbitPivotSource PivotSource,
    string SurfaceSource,
    Vector3d Pivot,
    GroundPickResult TerrainHit,
    GroundPickResult ReferencePlaneHit);
