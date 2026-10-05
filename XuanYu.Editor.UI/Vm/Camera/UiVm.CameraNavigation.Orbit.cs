using XuanYu.Core.Math;
using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly List<OrbitProbeEvent> _orbitProbeEvents = [];
    OrbitSessionState? _orbitSession;
    public IReadOnlyList<OrbitProbeEvent> OrbitProbeEvents => _orbitProbeEvents;

    OrbitPivotResolution ResolveOrbitPivot() => new(_observationCenter,
        OrbitPivotSource.ObservationCenter, GroundPickResult.Invalid, GroundPickResult.Invalid);

    void StartOrbitProbe(long sessionId, OrbitPivotResolution resolution)
    {
        _orbitSession = new(sessionId, resolution.Source, resolution.SurfaceSource,
            resolution.Pivot, resolution.TerrainHit, resolution.ReferencePlaneHit);
        AddOrbitProbe(OrbitProbePhase.Begin, sessionId, resolution, Vector3d.Zero, 1);
    }

    void AddOrbitProbe(OrbitProbePhase phase, long sessionId, OrbitPivotResolution resolution,
        Vector3d delta, int resolveCount) => _orbitProbeEvents.Add(new(sessionId, phase,
            resolution.Source, resolution.Pivot, resolution.SurfaceSource,
            resolution.TerrainHit, resolution.ReferencePlaneHit, delta, resolveCount));

    void AddFrozenOrbitProbe(OrbitProbePhase phase, Vector3d delta)
    {
        if (_orbitSession is not { } orbit) return;
        AddOrbitProbe(phase, orbit.SessionId, new(orbit.Pivot, orbit.PivotSource,
            orbit.TerrainHit, orbit.ReferencePlaneHit), delta, 0);
    }
}
