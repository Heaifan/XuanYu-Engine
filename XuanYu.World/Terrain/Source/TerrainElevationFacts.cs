namespace XuanYu.World.Terrain.Source;

public enum TerrainElevationUnit { Meter }
public enum TerrainVerticalDatum { Egm96Geoid }
public enum TerrainSourceFormat { NasademHgt }

public readonly record struct TerrainElevationResult(bool IsValid, double ElevationMeters)
{
    public static TerrainElevationResult Invalid => new(false, 0);
}

public enum TerrainElevationInterpolation { Nearest, Bilinear }
