using System.Text.Json;
using System.Text.RegularExpressions;
using FluidWarfare.Core.Results;
using FluidWarfare.Project.Content;
using FluidWarfare.Project.Metadata;
using FluidWarfare.Project.Validation;

namespace FluidWarfare.Project.Loading;

public static partial class GameProjectLoader
{
    private const string ManifestFileName = "game.project.json";
    private const int CurrentSchemaVersion = 1;

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

        if (manifest.SchemaVersion is null)
        {
            return Fail("Project.SchemaVersionMissing", "项目契约版本不能为空。");
        }

        if (manifest.SchemaVersion.Value != CurrentSchemaVersion)
        {
            return Fail(
                "Project.SchemaVersionUnsupported",
                $"项目契约版本不受支持：{manifest.SchemaVersion.Value}。");
        }

        if (string.IsNullOrWhiteSpace(manifest.ProjectId))
        {
            return Fail("Project.ProjectIdMissing", "项目编号不能为空。");
        }

        if (string.IsNullOrWhiteSpace(manifest.DisplayName))
        {
            return Fail("Project.DisplayNameMissing", "项目显示名称不能为空。");
        }

        // 解析并校验 contentFolders 声明
        var contentFoldersResult = LoadContentFolders(manifest.ContentFolders);
        if (contentFoldersResult.Result.IsFailure || contentFoldersResult.ContentFolders is null)
        {
            return WithReport(contentFoldersResult.Result, null, contentFoldersResult.Issues);
        }

        var issues = new List<ProjectValidationIssue>();
        var folders = contentFoldersResult.ContentFolders;
        var folderNames = new HashSet<string>(folders.Select(f => f.FolderName), StringComparer.Ordinal);

        // 检查声明目录是否存在
        foreach (var folder in folders)
        {
            if (!Directory.Exists(Path.Combine(projectDirectory, folder.FolderName)))
            {
                issues.Add(new ProjectValidationIssue(
                    "Project.ContentFolderDirectoryMissing",
                    $"项目内容目录不存在：{folder.FolderName}。",
                    folder.FolderName));
            }
        }

        // 查找所有未声明的一级目录
        var undeclaredDirectories = FindAllUndeclaredDirectories(projectDirectory, folderNames);
        foreach (var dirName in undeclaredDirectories)
        {
            issues.Add(new ProjectValidationIssue(
                "Project.UndeclaredContentFolder",
                $"项目存在未声明的内容目录：{dirName}。",
                dirName));
        }

        // 扫描内容文件入口（Scanner 已改为收集所有问题）
        var scanResult = GameContentFileScanner.Scan(projectDirectory, folders);
        issues.AddRange(scanResult.Issues);

        // 构建报告
        if (issues.Count > 0)
        {
            var report = new ProjectValidationReport(issues);
            var firstError = EngineError.Create(issues[0].Code, issues[0].Message);
            return new GameProjectLoadResult(EngineResult.Fail(firstError), null, report);
        }

        var project = new GameProjectInfo(
            manifest.SchemaVersion.Value,
            manifest.ProjectId.Trim(),
            manifest.DisplayName.Trim(),
            manifest.Description?.Trim() ?? string.Empty,
            folders,
            scanResult.ContentFiles);

        return new GameProjectLoadResult(EngineResult.Success(), project, ProjectValidationReport.Empty);
    }

    private static ContentFoldersLoadResult LoadContentFolders(JsonElement contentFoldersElement)
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

            var folderResult = LoadContentFolder(folderElement, folderNames);
            if (folderResult.Result.IsFailure || folderResult.ContentFolder is null)
            {
                return ContentFoldersLoadResult.Fail(folderResult.Result);
            }

            folders.Add(folderResult.ContentFolder);
        }

        return new ContentFoldersLoadResult(EngineResult.Success(), folders, []);
    }

    private static ContentFolderLoadResult LoadContentFolder(
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

    /// <summary>
    /// 查找项目根目录下所有未声明的一级目录。
    /// </summary>
    private static IReadOnlyList<string> FindAllUndeclaredDirectories(
        string projectDirectory,
        HashSet<string> declaredFolderNames)
    {
        return Directory.EnumerateDirectories(projectDirectory)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Where(name => !name!.StartsWith(".", StringComparison.Ordinal))
            .Where(name => !declaredFolderNames.Contains(name!))
            .Select(name => name!)
            .ToList();
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
        var issue = new ProjectValidationIssue(code, message, "");
        var report = new ProjectValidationReport([issue]);
        return new GameProjectLoadResult(EngineResult.Fail(error), null, report);
    }

    private static EngineResult FailResult(string code, string message)
    {
        return EngineResult.Fail(EngineError.Create(code, message));
    }

    private static GameProjectLoadResult WithReport(
        EngineResult result,
        GameProjectInfo? project,
        IReadOnlyList<ProjectValidationIssue> issues)
    {
        var report = new ProjectValidationReport(issues);
        return new GameProjectLoadResult(result, project, report);
    }

    [GeneratedRegex("^[a-z0-9_-]+$")]
    private static partial Regex FolderNameRegex();

    [GeneratedRegex("^[A-Za-z][A-Za-z0-9_-]*$")]
    private static partial Regex ContentKindRegex();

    [GeneratedRegex("^\\.[a-z0-9]+$")]
    private static partial Regex AllowedExtensionRegex();

    private sealed record ProjectManifestDto(
        int? SchemaVersion,
        string? ProjectId,
        string? DisplayName,
        string? Description,
        JsonElement ContentFolders);

    private sealed record ContentFoldersLoadResult(
        EngineResult Result,
        IReadOnlyList<GameContentFolderInfo>? ContentFolders,
        IReadOnlyList<ProjectValidationIssue> Issues)
    {
        public static ContentFoldersLoadResult Fail(string code, string message)
        {
            var issue = new ProjectValidationIssue(code, message, "");
            return new ContentFoldersLoadResult(FailResult(code, message), null, [issue]);
        }

        public static ContentFoldersLoadResult Fail(EngineResult result)
        {
            var issues = result.Error is { IsValid: true }
                ? new List<ProjectValidationIssue>
                {
                    new(result.Error.Value.Code, result.Error.Value.Message, "")
                }
                : [];
            return new ContentFoldersLoadResult(result, null, issues);
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
