using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render.Terrain;

internal static class VulkanTerrainGpuMeshIndexValidator
{
    public static bool TryValidate(TerrainChunkMesh mesh, out string error)
    {
        var vertexCount = mesh.Vertices.Count;
        for (var index = 0; index < mesh.Indices.Count; index++)
        {
            var vertexIndex = mesh.Indices[index];
            if (vertexIndex < (uint)vertexCount) continue;
            error = $"索引越界：index={vertexIndex}, vertexCount={vertexCount}。";
            return false;
        }
        error = "";
        return true;
    }
}
