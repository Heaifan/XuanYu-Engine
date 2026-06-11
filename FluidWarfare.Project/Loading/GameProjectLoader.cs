using System.Text.Json;
using System.Text.RegularExpressions;
using FluidWarfare.Core.Results;
using FluidWarfare.Project.Content;
using FluidWarfare.Project.Metadata;

namespace FluidWarfare.Project.Loading;

public static partial class GameProjectLoader
{
    private const string ManifestFileName = "game.project.json";

    public static GameProjectLoadResult LoadFromDirectory(string projectDirectory)
    {
        if (string.IsNullOrWhiteSpace(projectDirectory) || !Directory.Exists(projectDirectory))
        {
            return Fail("Project.DirectoryMissing", "项目目录不存在。");
        }

        var manifestPath = Path.Combine(projectDirectory, ManifestFileName);
        if (!File.Exists(manifestPath))
        {
            return Fail("Project.ManifestMissing", "项目文件 game.project.json 不存在。");
        }

        ProjectManifestDto? manifest;
        try
        {
            var json = File.ReadAllText(manifestPath);
            manifest = JsonSerializer.Deserialize<ProjectManifestDto>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException)
        {
            return Fail("Project.ManifestInvalid", "项目文件格式无效。");
        }
        catch (NotSupportedException)
        {
            return Fail("Project.ManifestInvalid", "项目文件格式无效。");
        }

        if (manifest is null)
        {
            return Fail("Project.ManifestInvalid", "项目文件格式无效。");
        }

        if (string.IsNullOrWhiteSpace(manifest.ProjectId))
        {
            return Fail("Project.ProjectIdMissing", "项目编号不能为空。");
        }

        if (string.IsNullOrWhiteSpace(manifest.DisplayName))
        {
            return Fail("Project.DisplayNameMissing", "项目显示名称不能为空。");
        }

        var contentFoldersResult = LoadContentFolders(projectDirectory, manifest.ContentFolders);
        if (contentFoldersResult.Result.IsFailure || contentFoldersResult.ContentFolders is null)
        {
            return new GameProjectLoadResult(contentFoldersResult.Result, null);
        }

        var project = new GameProjectInfo(
            manifest.ProjectId.Trim(),
            manifest.DisplayName.Trim(),
            manifest.Description?.Trim() ?? string.Empty,
            contentFoldersResult.ContentFolders);

        return new GameProjectLoadResult(EngineResult.Success(), project);
    }

    private static ContentFoldersLoadResult LoadContentFolders(
        string projectDirectory,
        JsonElement contentFoldersElement)
    {
        if (contentFoldersElement.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            return ContentFoldersLoadResult.Fail("Project.ContentFoldersMissing", "项目内容目录声明不能为空。");
        }

        if (contentFoldersElement.ValueKind != JsonValueKind.Array)
        {
            return ContentFoldersLoadResult.Fail("Project.ContentFolderInvalid", "项目内容目录声明格式无效。");
        }

        var folderElements = contentFoldersElement.EnumerateArray().ToArray();
        if (folderElements.Length == 0)
        {
            return ContentFoldersLoadResult.Fail("Project.ContentFoldersMissing", "项目内容目录声明不能为空。");
        }

        var folders = new List<GameContentFolderInfo>();
        var folderNames = new HashSet<string>(StringComparer.Ordinal);

        foreach (var folderElement in folderElements)
        {
            if (folderElement.ValueKind != JsonValueKind.Object)
            {
                return ContentFoldersLoadResult.Fail("Project.ContentFolderInvalid", "项目内容目录声明格式无效。");
            }

            var folderResult = LoadContentFolder(projectDirectory, folderElement, folderNames);
            if (folderResult.Result.IsFailure || folderResult.ContentFolder is null)
            {
                return ContentFoldersLoadResult.Fail(folderResult.Result);
            }

            folders.Add(folderResult.ContentFolder);
        }

        var undeclaredDirectory = FindUndeclaredDirectory(projectDirectory, folderNames);
        if (undeclaredDirectory is not null)
        {
            return ContentFoldersLoadResult.Fail(
                "Project.UndeclaredContentFolder",
                $"项目存在未声明的内容目录：{undeclaredDirectory}。");
        }

        return new ContentFoldersLoadResult(EngineResult.Success(), folders);
    }

    private static ContentFolderLoadResult LoadContentFolder(
        string projectDirectory,
        JsonElement folderElement,
        HashSet<string> folderNames)
    {
        var folderName = GetString(folderElement, "folderName");
        if (string.IsNullOrWhiteSpace(folderName))
        {
            return ContentFolderLoadResult.Fail("Project.ContentFolderNameMissing", "项目内容目录名不能为空。");
        }

        folderName = folderName.Trim();
        if (!FolderNameRegex().IsMatch(folderName))
        {
            return ContentFolderLoadResult.Fail(
                "Project.ContentFolderNameInvalid",
                $"项目内容目录名格式无效：{folderName}。");
        }

        if (!folderNames.Add(folderName))
        {
            return ContentFolderLoadResult.Fail(
                "Project.ContentFolderDuplicated",
                $"项目内容目录重复声明：{folderName}。");
        }

        var displayName = GetString(folderElement, "displayName");
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return ContentFolderLoadResult.Fail("Project.ContentFolderDisplayNameMissing", "项目内容目录显示名不能为空。");
        }

        var description = GetString(folderElement, "description");
        if (string.IsNullOrWhiteSpace(description))
        {
            return ContentFolderLoadResult.Fail("Project.ContentFolderDescriptionMissing", "项目内容目录说明不能为空。");
        }

        var contentKind = GetString(folderElement, "contentKind");
        if (string.IsNullOrWhiteSpace(contentKind))
        {
            return ContentFolderLoadResult.Fail("Project.ContentFolderKindMissing", "项目内容类型不能为空。");
        }

        contentKind = contentKind.Trim();
        if (!ContentKindRegex().IsMatch(contentKind))
        {
            return ContentFolderLoadResult.Fail(
                "Project.ContentKindInvalid",
                $"项目内容类型格式无效：{contentKind}。");
        }

        var extensionsResult = LoadAllowedExtensions(folderElement);
        if (extensionsResult.Result.IsFailure || extensionsResult.AllowedExtensions is null)
        {
            return ContentFolderLoadResult.Fail(extensionsResult.Result);
        }

        if (!Directory.Exists(Path.Combine(projectDirectory, folderName)))
        {
            return ContentFolderLoadResult.Fail(
                "Project.ContentFolderDirectoryMissing",
                $"项目内容目录不存在：{folderName}。");
        }

        var contentFolder = new GameContentFolderInfo(
            folderName,
            displayName.Trim(),
            description.Trim(),
            contentKind,
            GetBoolean(folderElement, "isRequired"),
            extensionsResult.AllowedExtensions);

        return new ContentFolderLoadResult(EngineResult.Success(), contentFolder);
    }

    private static AllowedExtensionsLoadResult LoadAllowedExtensions(JsonElement folderElement)
    {
        if (!folderElement.TryGetProperty("allowedExtensions", out var allowedExtensionsElement) ||
            allowedExtensionsElement.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            return new AllowedExtensionsLoadResult(EngineResult.Success(), []);
        }

        if (allowedExtensionsElement.ValueKind != JsonValueKind.Array)
        {
            return AllowedExtensionsLoadResult.Fail(
                "Project.AllowedExtensionInvalid",
                "项目内容目录允许扩展名格式无效：allowedExtensions。");
        }

        var extensions = new List<string>();
        foreach (var extensionElement in allowedExtensionsElement.EnumerateArray())
        {
            if (extensionElement.ValueKind != JsonValueKind.String)
            {
                return AllowedExtensionsLoadResult.Fail(
                    "Project.AllowedExtensionInvalid",
                    "项目内容目录允许扩展名格式无效：allowedExtensions。");
            }

            var extension = extensionElement.GetString()?.Trim() ?? string.Empty;
            if (!AllowedExtensionRegex().IsMatch(extension))
            {
                return AllowedExtensionsLoadResult.Fail(
                    "Project.AllowedExtensionInvalid",
                    $"项目内容目录允许扩展名格式无效：{extension}。");
            }

            extensions.Add(extension.ToLowerInvariant());
        }

        return new AllowedExtensionsLoadResult(EngineResult.Success(), extensions);
    }

    private static string? FindUndeclaredDirectory(string projectDirectory, HashSet<string> declaredFolderNames)
    {
        return Directory.EnumerateDirectories(projectDirectory)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Where(name => !name!.StartsWith(".", StringComparison.Ordinal))
            .FirstOrDefault(name => !declaredFolderNames.Contains(name!));
    }

    private static string? GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static bool GetBoolean(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) &&
               property.ValueKind == JsonValueKind.True;
    }

    private static GameProjectLoadResult Fail(string code, string message)
    {
        var error = EngineError.Create(code, message);
        return new GameProjectLoadResult(EngineResult.Fail(error), null);
    }

    private static EngineResult FailResult(string code, string message)
    {
        return EngineResult.Fail(EngineError.Create(code, message));
    }

    [GeneratedRegex("^[a-z0-9_-]+$")]
    private static partial Regex FolderNameRegex();

    [GeneratedRegex("^[A-Za-z][A-Za-z0-9_-]*$")]
    private static partial Regex ContentKindRegex();

    [GeneratedRegex("^\\.[a-z0-9]+$")]
    private static partial Regex AllowedExtensionRegex();

    private sealed record ProjectManifestDto(
        string? ProjectId,
        string? DisplayName,
        string? Description,
        JsonElement ContentFolders);

    private sealed record ContentFoldersLoadResult(
        EngineResult Result,
        IReadOnlyList<GameContentFolderInfo>? ContentFolders)
    {
        public static ContentFoldersLoadResult Fail(string code, string message)
        {
            return new ContentFoldersLoadResult(FailResult(code, message), null);
        }

        public static ContentFoldersLoadResult Fail(EngineResult result)
        {
            return new ContentFoldersLoadResult(result, null);
        }
    }

    private sealed record ContentFolderLoadResult(
        EngineResult Result,
        GameContentFolderInfo? ContentFolder)
    {
        public static ContentFolderLoadResult Fail(string code, string message)
        {
            return new ContentFolderLoadResult(FailResult(code, message), null);
        }

        public static ContentFolderLoadResult Fail(EngineResult result)
        {
            return new ContentFolderLoadResult(result, null);
        }
    }

    private sealed record AllowedExtensionsLoadResult(
        EngineResult Result,
        IReadOnlyList<string>? AllowedExtensions)
    {
        public static AllowedExtensionsLoadResult Fail(string code, string message)
        {
            return new AllowedExtensionsLoadResult(FailResult(code, message), null);
        }
    }
}
