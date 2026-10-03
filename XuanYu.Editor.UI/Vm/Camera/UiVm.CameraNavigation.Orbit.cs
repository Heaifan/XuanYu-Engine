using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly List<OrbitProbeEvent> _orbitProbeEvents = [];
    OrbitSessionState? _orbitSession;
    public IReadOnlyList<OrbitProbeEvent> OrbitProbeEvents => _orbitProbeEvents;

    OrbitPivotResolution ResolveOrbitPivot(ViewportState viewport)
    {
        if (TrySelectedEntityKey(out var key) && _sceneState.TryGetEntity(key, out var entity))
            return OrbitPivotAuthority.Resolve(entity.Transform.Position,
                GroundPickResult.Invalid, GroundPickResult.Invalid, _observationCenter);
        var projection = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        var centerX = viewport.LogicalX + (viewport.LogicalWidth * 0.5);
        var centerY = viewport.LogicalY + (viewport.LogicalHeight * 0.5);
        var ray = WorldRayFactory.FromViewportPoint(projection, centerX, centerY);
        var terrain = TerrainWorld is null ? null : new TerrainWorldGroundSurface(TerrainWorld);
        var terrainHit = GroundPickResolver.Resolve(ray, terrain, MapSession.CurrentMap.Surface.BaseHeightMeters);
        var planeHit = terrainHit.IsValid ? GroundPickResult.Invalid :
            GroundPickResolver.Resolve(ray, null, MapSession.CurrentMap.Surface.BaseHeightMeters);
        return OrbitPivotAuthority.Resolve(null, terrainHit, planeHit, _observationCenter);
    }

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
