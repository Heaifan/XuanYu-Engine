namespace XuanYu.World.Terrain.Import;

public sealed class TerrainSourceReadException(string message, Exception? inner = null) : Exception(message, inner);
