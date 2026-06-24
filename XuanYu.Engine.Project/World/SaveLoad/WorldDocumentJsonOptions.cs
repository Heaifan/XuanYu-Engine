using System.Text.Json;

namespace XuanYu.Engine.Project.World.SaveLoad;

/// <summary>World 文档 JSON 序列化共享选项。</summary>
public static class WorldDocumentJsonOptions
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
