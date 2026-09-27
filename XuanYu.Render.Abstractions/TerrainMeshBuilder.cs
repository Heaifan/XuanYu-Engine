namespace XuanYu.Render.Abstractions;

public static class TerrainMeshBuilder
{
    public static TerrainMesh Build(TerrainRenderResource resource, TerrainRenderTransform transform)
    {
        var field = resource.Heightfield;
        var vertices = new List<TerrainMeshVertex>(field.Width * field.Height);
        for (var row = 0; row < field.Height; row++)
            for (var column = 0; column < field.Width; column++)
            {
                var normal = TerrainNormalBuilder.Build(field, resource.CellSizeMeters, transform, row, column);
                vertices.Add(new(resource.WorldOrigin.X + column * resource.CellSizeMeters,
                    resource.WorldOrigin.Y + row * resource.CellSizeMeters,
                    transform.VisualHeight(field.ElevationAt(row, column)),
                    normal.X,
                    normal.Y, normal.Z));
            }
        var indices = new List<uint>(resource.TriangleIndexCount);
        for (var row = 0; row < field.Height - 1; row++)
            for (var column = 0; column < field.Width - 1; column++)
            {
                var topLeft = (uint)(row * field.Width + column);
                var topRight = topLeft + 1; var bottomLeft = topLeft + (uint)field.Width;
                var bottomRight = bottomLeft + 1;
                indices.AddRange([topLeft, bottomLeft, topRight, topRight, bottomLeft, bottomRight]);
            }
        return new(vertices, indices);
    }
}
