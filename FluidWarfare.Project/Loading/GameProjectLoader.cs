using System.Text.Json;
using FluidWarfare.Core.Results;
using FluidWarfare.Project.Metadata;

namespace FluidWarfare.Project.Loading;

public static class GameProjectLoader
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

        ProjectManifest? manifest;
        try
        {
            var json = File.ReadAllText(manifestPath);
            manifest = JsonSerializer.Deserialize<ProjectManifest>(
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

        var contentFolders = manifest.ContentFolders?
            .Where(folder => !string.IsNullOrWhiteSpace(folder))
            .Select(folder => folder.Trim())
            .ToArray() ?? [];

        var project = new GameProjectInfo(
            manifest.ProjectId.Trim(),
            manifest.DisplayName.Trim(),
            manifest.Description?.Trim() ?? string.Empty,
            contentFolders);

        return new GameProjectLoadResult(EngineResult.Success(), project);
    }

    private static GameProjectLoadResult Fail(string code, string message)
    {
        var error = EngineError.Create(code, message);
        return new GameProjectLoadResult(EngineResult.Fail(error), null);
    }

    private sealed record ProjectManifest(
        string? ProjectId,
        string? DisplayName,
        string? Description,
        IReadOnlyList<string>? ContentFolders);
}
