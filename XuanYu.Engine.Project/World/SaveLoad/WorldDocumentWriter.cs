using System.Text.Json;
using XuanYu.Engine.Project.World.Documents;

namespace XuanYu.Engine.Project.World.SaveLoad;

/// <summary>WorldDocument JSON 写入器。将 WorldDocument 保存为 .world.json 文件。</summary>
public static class WorldDocumentWriter
{
    public static WorldDocumentWriteResult Write(string filePath, WorldDocument document)
    {
        try
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var json = JsonSerializer.Serialize(document, WorldDocumentJsonOptions.Options);
            File.WriteAllText(filePath, json);
            return WorldDocumentWriteResult.Success();
        }
        catch (Exception ex)
        {
            return WorldDocumentWriteResult.Fail($"保存 World 文件失败：{ex.Message}");
        }
    }
}
