namespace XuanYu.Render.Abstractions;

public readonly record struct TerrainMeshVertex(double X, double Y, double Z, double Nx, double Ny, double Nz);
public sealed record TerrainMesh(IReadOnlyList<TerrainMeshVertex> Vertices, IReadOnlyList<uint> Indices);
