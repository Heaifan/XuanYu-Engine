using System.Text.RegularExpressions;
using System.Text.Json;
using XuanYu.Engine.Core.Results;
using XuanYu.Engine.Project.Validation;

namespace XuanYu.Engine.Project.Loading;

/// <summary>解析 allowedExtensions JSON 数组 + 格式校验。供 GameProjectFolderParser 调用。</summary>
internal static partial class GameProjectExtensionParser
{
    public static AllowedExtensionsLoadResult Parse(JsonElement folderElement)
    {
        if (!folderElement.TryGetProperty("allowedExtensions", out var exts) ||
            exts.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            return AllowedExtensionsLoadResult.Ok([]);

        if (exts.ValueKind != JsonValueKind.Array)
            return AllowedExtensionsLoadResult.Fail("Project.AllowedExtensionInvalid",
                "项目内容目录允许扩展名格式无效：allowedExtensions。");

        var extensions = new List<string>();
        foreach (var ext in exts.EnumerateArray())
        {
            if (ext.ValueKind != JsonValueKind.String)
                return AllowedExtensionsLoadResult.Fail("Project.AllowedExtensionInvalid",
                    "项目内容目录允许扩展名格式无效：allowedExtensions。");

            var extension = ext.GetString()?.Trim() ?? string.Empty;
            if (!AllowedExtensionRegex().IsMatch(extension))
                return AllowedExtensionsLoadResult.Fail("Project.AllowedExtensionInvalid",
                    $"项目内容目录允许扩展名格式无效：{extension}。");

            extensions.Add(extension.ToLowerInvariant());
        }

        return AllowedExtensionsLoadResult.Ok(extensions);
    }

    [GeneratedRegex("^\\.[a-z0-9]+$")]
    private static partial Regex AllowedExtensionRegex();
}

internal sealed record AllowedExtensionsLoadResult(
    EngineResult Result,
    IReadOnlyList<string>? AllowedExtensions)
{
    public static AllowedExtensionsLoadResult Ok(IReadOnlyList<string> extensions) =>
        new(EngineResult.Success(), extensions);

    public static AllowedExtensionsLoadResult Fail(string code, string message) =>
        new(EngineResult.Fail(EngineError.Create(code, message)), null);
}
