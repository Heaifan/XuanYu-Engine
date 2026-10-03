using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Editor.MapEditing;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Import;
using XuanYu.World.Map;
using Xunit.Abstractions;

namespace XuanYu.Core.Tests.Camera;

public sealed partial class DemOrbitFrozenPivotR1CTests
{
    [Fact(Skip = "BLOCKED: real HGT fixture unavailable; no synthetic terrain is permitted.")]
    public void TerrainOrbit_UsesTerrainXYZ()
    {
        var world = ReadExistingHgtFixture();
        var surface = new TerrainWorldGroundSurface(world);
        var query = surface.QuerySurface(new(0, 0));
        Assert.True(query.IsValid);
        var pivot = new Vector3d(0, 0, query.SurfaceZ);
        Assert.True(FrozenOrbitSession.TryBegin(StartCamera(pivot), pivot, out var session));
        Assert.True(session!.TryMove(0.4, 0.1, 2, out var result, out _));
        var error = result.ObservationCenter.DistanceTo(pivot);
        Report("Pivot XYZ error", error);
        Assert.Equal(SurfaceBindingKind.Terrain, surface.Binding.Kind);
        Assert.InRange(error, 0, 1e-10);
    }

    [Fact(Skip = "BLOCKED: real HGT fixture unavailable; no synthetic terrain is permitted.")]
    public void TerrainEdgeOrbit_UsesStableSource()
    {
        var surface = new TerrainWorldGroundSurface(ReadExistingHgtFixture());
        var ray = new WorldRay(new(10, 10, 100), new(0, 0, -1));
        var terrainHit = GroundPickResolver.Resolve(ray, surface, 2);
        var fallback = GroundPickResolver.Resolve(ray, null, 2);
        Assert.False(terrainHit.IsValid);
        Assert.Equal(SurfaceBindingKind.ReferencePlane, fallback.SurfaceBinding.Kind);
        Assert.Equal(2, fallback.ResolvedElevation);
        Report("Surface resolve count", 1);
    }

    static TerrainWorld ReadExistingHgtFixture()
    {
        var path = Directory.EnumerateFiles(FindRepoRoot(), "*.hgt", SearchOption.AllDirectories)
            .FirstOrDefault();
        if (path is null)
            throw Xunit.Sdk.SkipException.ForSkip(
                "BLOCKED: real HGT fixture unavailable; no synthetic terrain is permitted.");
        return TerrainWorld.FromElevationTile(HgtTerrainElevationTileReader.Read(path));
    }

    static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "XuanYu.Engine.slnx")))
            directory = directory.Parent;
        return directory?.FullName ?? AppContext.BaseDirectory;
    }

    static CameraState StartCamera(Vector3d pivot) =>
        new(pivot + new Vector3d(80, -35, 55), Vector3d.UnitX, Vector3d.UnitZ, 60, 0.1, 5000, 1);
}
