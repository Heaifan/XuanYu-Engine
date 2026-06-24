using System.Text.Json;
using XuanYu.Engine.Project.World.Documents;

namespace XuanYu.Engine.Project.World.SaveLoad;

/// <summary>WorldDocument JSON 读取器。从 .world.json 文件读取 WorldDocument。</summary>
public static class WorldDocumentReader
{
    public static WorldDocumentReadResult Read(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return WorldDocumentReadResult.Fail("World 文件路径不能为空。");

        if (!File.Exists(filePath))
            return WorldDocumentReadResult.Fail($"World 文件不存在：{filePath}");

        try
        {
            var json = File.ReadAllText(filePath);
            var document = JsonSerializer.Deserialize<WorldDocument>(json, WorldDocumentJsonOptions.Options);
            if (document is null)
                return WorldDocumentReadResult.Fail("World 文件内容为空。");
            return WorldDocumentReadResult.Success(document);
        }
        catch (JsonException ex)
        {
            return WorldDocumentReadResult.Fail($"World 文件 JSON 格式错误：{ex.Message}");
        }
        catch (Exception ex)
        {
            return WorldDocumentReadResult.Fail($"读取 World 文件失败：{ex.Message}");
        }
    }
}
