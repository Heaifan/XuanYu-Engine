using XuanYu.Engine.Core.Results;
using XuanYu.Engine.Project.Content;
using XuanYu.Engine.Project.Metadata;
using XuanYu.Engine.Project.Validation;

namespace XuanYu.Engine.Project.Loading;

/// <summary>
/// 项目加载主入口。委托 ManifestReader 加载清单，FolderParser 解析目录，
/// 自行完成目录校验和结果构建。
/// </summary>
public static class GameProjectLoader
{
    public static GameProjectLoadResult LoadFromDirectory(string projectDirectory)
    {
        if (string.IsNullOrWhiteSpace(projectDirectory) || !Directory.Exists(projectDirectory))
            return Fail("Project.DirectoryMissing", "项目目录不存在。");

        var manifest = GameProjectManifestReader.Read(projectDirectory);
        if (!manifest.IsSuccess)
            return WithReport(manifest.Result, null, manifest.Issues);

        var foldersResult = GameProjectFolderParser.Parse(manifest.ContentFoldersJson);
        if (foldersResult.Result.IsFailure || foldersResult.Folders is null)
            return WithReport(foldersResult.Result, null, foldersResult.Issues);

        var issues = new List<ProjectValidationIssue>();
        var folders = foldersResult.Folders;
        var folderNames = new HashSet<string>(folders.Select(f => f.FolderName), StringComparer.Ordinal);

        foreach (var folder in folders)
        {
            if (!Directory.Exists(Path.Combine(projectDirectory, folder.FolderName)))
                issues.Add(new ProjectValidationIssue("Project.ContentFolderDirectoryMissing",
                    $"项目内容目录不存在：{folder.FolderName}。", folder.FolderName));
        }

        var undeclared = FindAllUndeclaredDirectories(projectDirectory, folderNames);
        issues.AddRange(undeclared.Select(d => new ProjectValidationIssue(
            "Project.UndeclaredContentFolder", $"项目存在未声明的内容目录：{d}。", d)));

        var scanResult = GameContentFileScanner.Scan(projectDirectory, folders);
        issues.AddRange(scanResult.Issues);

        if (issues.Count > 0)
        {
            var report = new ProjectValidationReport(issues);
            var first = EngineError.Create(issues[0].Code, issues[0].Message);
            return new GameProjectLoadResult(EngineResult.Fail(first), null, report);
        }

        var project = new GameProjectInfo(
            manifest.SchemaVersion, manifest.ProjectId, manifest.DisplayName,
            manifest.Description, folders, scanResult.ContentFiles);

        return new GameProjectLoadResult(EngineResult.Success(), project, ProjectValidationReport.Empty);
    }

    static IReadOnlyList<string> FindAllUndeclaredDirectories(string projectDir, HashSet<string> declared)
    {
        return Directory.EnumerateDirectories(projectDir)
            .Select(Path.GetFileName)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Where(n => !n!.StartsWith(".", StringComparison.Ordinal))
            .Where(n => !declared.Contains(n!))
            .Select(n => n!)
            .ToList();
    }

    static GameProjectLoadResult Fail(string code, string message)
    {
        var error = EngineError.Create(code, message);
        var issue = new ProjectValidationIssue(code, message, "");
        return new GameProjectLoadResult(EngineResult.Fail(error), null, new ProjectValidationReport([issue]));
    }

    static GameProjectLoadResult WithReport(EngineResult result, GameProjectInfo? project,
        IReadOnlyList<ProjectValidationIssue> issues)
    {
        return new GameProjectLoadResult(result, project, new ProjectValidationReport(issues));
    }
}
