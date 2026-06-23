using System.Text.RegularExpressions;
using System.Text.Json;
using FluidWarfare.Core.Results;
using FluidWarfare.Project.Content;
using FluidWarfare.Project.Validation;

namespace FluidWarfare.Project.Loading;

internal static partial class GameProjectFolderParser
{
    public static FolderParseResult Parse(JsonElement element)
    {
        if (element.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            return FolderParseResult.Fail("Project.ContentFoldersMissing", "项目内容目录声明不能为空。");
        if (element.ValueKind != JsonValueKind.Array)
            return FolderParseResult.Fail("Project.ContentFolderInvalid", "项目内容目录声明格式无效。");
        var items = element.EnumerateArray().ToArray();
        if (items.Length == 0)
            return FolderParseResult.Fail("Project.ContentFoldersMissing", "项目内容目录声明不能为空。");

        var folders = new List<GameContentFolderInfo>();
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var item in items)
        {
            if (item.ValueKind != JsonValueKind.Object)
                return FolderParseResult.Fail("Project.ContentFolderInvalid", "项目内容目录声明格式无效。");
            var r = ParseItem(item, names);
            if (r.Result.IsFailure || r.Folder is null)
                return FolderParseResult.Fail(r.Result);
            folders.Add(r.Folder);
        }
        return FolderParseResult.Ok(folders);
    }

    static FolderItemResult ParseItem(JsonElement e, HashSet<string> names)
    {
        var name = GetString(e, "folderName");
        if (string.IsNullOrWhiteSpace(name))
            return FolderItemResult.Fail("Project.ContentFolderNameMissing", "项目内容目录名不能为空。");
        name = name.Trim();
        if (!FolderNameRegex().IsMatch(name))
            return FolderItemResult.Fail("Project.ContentFolderNameInvalid", $"项目内容目录名格式无效：{name}。");
        if (!names.Add(name))
            return FolderItemResult.Fail("Project.ContentFolderDuplicated", $"项目内容目录重复声明：{name}。");

        var displayName = GetString(e, "displayName");
        if (string.IsNullOrWhiteSpace(displayName))
            return FolderItemResult.Fail("Project.ContentFolderDisplayNameMissing", "项目内容目录显示名不能为空。");
        var description = GetString(e, "description");
        if (string.IsNullOrWhiteSpace(description))
            return FolderItemResult.Fail("Project.ContentFolderDescriptionMissing", "项目内容目录说明不能为空。");

        var contentKind = GetString(e, "contentKind");
        if (string.IsNullOrWhiteSpace(contentKind))
            return FolderItemResult.Fail("Project.ContentFolderKindMissing", "项目内容类型不能为空。");
        contentKind = contentKind.Trim();
        if (!ContentKindRegex().IsMatch(contentKind))
            return FolderItemResult.Fail("Project.ContentKindInvalid", $"项目内容类型格式无效：{contentKind}。");

        var extResult = GameProjectExtensionParser.Parse(e);
        if (extResult.Result.IsFailure || extResult.AllowedExtensions is null)
            return FolderItemResult.Fail(extResult.Result);

        return FolderItemResult.Ok(new GameContentFolderInfo(
            name, displayName.Trim(), description.Trim(), contentKind,
            GetBoolean(e, "isRequired"), extResult.AllowedExtensions));
    }

    static string? GetString(JsonElement e, string p) =>
        e.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;
    static bool GetBoolean(JsonElement e, string p) =>
        e.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.True;

    [GeneratedRegex("^[a-z0-9_-]+$")] private static partial Regex FolderNameRegex();
    [GeneratedRegex("^[A-Za-z][A-Za-z0-9_-]*$")] private static partial Regex ContentKindRegex();
}

sealed record FolderParseResult(EngineResult Result, IReadOnlyList<GameContentFolderInfo>? Folders, IReadOnlyList<ProjectValidationIssue> Issues)
{
    public static FolderParseResult Ok(IReadOnlyList<GameContentFolderInfo> f) => new(EngineResult.Success(), f, []);
    public static FolderParseResult Fail(string code, string m)
    {
        var e = EngineError.Create(code, m);
        return new FolderParseResult(EngineResult.Fail(e), null, [new ProjectValidationIssue(code, m, "")]);
    }
    public static FolderParseResult Fail(EngineResult r)
    {
        var issues = r.Error is { IsValid: true }
            ? new List<ProjectValidationIssue> { new(r.Error.Value.Code, r.Error.Value.Message, "") }
            : [];
        return new FolderParseResult(r, null, issues);
    }
}

sealed record FolderItemResult(EngineResult Result, GameContentFolderInfo? Folder)
{
    public static FolderItemResult Ok(GameContentFolderInfo f) => new(EngineResult.Success(), f);
    public static FolderItemResult Fail(string code, string m) => new(EngineResult.Fail(EngineError.Create(code, m)), null);
    public static FolderItemResult Fail(EngineResult r) => new(r, null);
}
