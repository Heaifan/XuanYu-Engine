using System.Text.Json;
using FluidWarfare.Core.Results;
using FluidWarfare.Project.Validation;

namespace FluidWarfare.Project.Loading;

/// <summary>读取并验证 game.project.json 文件。供 GameProjectLoader 内部使用。</summary>
internal static class GameProjectManifestReader
{
    const string ManifestFileName = "game.project.json";
    const int CurrentSchemaVersion = 1;

    public static ManifestReadResult Read(string projectDirectory)
    {
        var manifestPath = Path.Combine(projectDirectory, ManifestFileName);
        if (!File.Exists(manifestPath))
            return ManifestReadResult.Fail("Project.ManifestMissing",
                "项目文件 game.project.json 不存在。");

        ProjectManifestDto? manifest;
        try
        {
            var json = File.ReadAllText(manifestPath);
            manifest = JsonSerializer.Deserialize<ProjectManifestDto>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (JsonException)
        {
            return ManifestReadResult.Fail("Project.ManifestInvalid", "项目文件格式无效。");
        }
        catch (NotSupportedException)
        {
            return ManifestReadResult.Fail("Project.ManifestInvalid", "项目文件格式无效。");
        }

        if (manifest is null)
            return ManifestReadResult.Fail("Project.ManifestInvalid", "项目文件格式无效。");

        if (manifest.SchemaVersion is null)
            return ManifestReadResult.Fail("Project.SchemaVersionMissing", "项目契约版本不能为空。");

        if (manifest.SchemaVersion.Value != CurrentSchemaVersion)
            return ManifestReadResult.Fail("Project.SchemaVersionUnsupported",
                $"项目契约版本不受支持：{manifest.SchemaVersion.Value}。");

        if (string.IsNullOrWhiteSpace(manifest.ProjectId))
            return ManifestReadResult.Fail("Project.ProjectIdMissing", "项目编号不能为空。");

        if (string.IsNullOrWhiteSpace(manifest.DisplayName))
            return ManifestReadResult.Fail("Project.DisplayNameMissing", "项目显示名称不能为空。");

        return ManifestReadResult.Ok(
            manifest.SchemaVersion.Value,
            manifest.ProjectId.Trim(),
            manifest.DisplayName.Trim(),
            manifest.Description?.Trim() ?? string.Empty,
            manifest.ContentFolders);
    }
}

internal sealed record ProjectManifestDto(
    int? SchemaVersion, string? ProjectId, string? DisplayName,
    string? Description, JsonElement ContentFolders);

internal sealed record ManifestReadResult(
    EngineResult Result,
    IReadOnlyList<ProjectValidationIssue> Issues,
    int SchemaVersion,
    string ProjectId,
    string DisplayName,
    string Description,
    JsonElement ContentFoldersJson)
{
    public bool IsSuccess => Result.IsSuccess;

    public static ManifestReadResult Ok(
        int schemaVersion, string projectId, string displayName,
        string description, JsonElement contentFolders) =>
        new(EngineResult.Success(), [], schemaVersion, projectId,
            displayName, description, contentFolders);

    public static ManifestReadResult Fail(string code, string message)
    {
        var error = EngineError.Create(code, message);
        var issue = new ProjectValidationIssue(code, message, "");
        return new ManifestReadResult(
            EngineResult.Fail(error), [issue], 0, "", "", "", default);
    }
}
