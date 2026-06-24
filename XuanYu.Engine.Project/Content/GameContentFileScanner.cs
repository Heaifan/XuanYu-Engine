using FluidWarfare.Project.Validation;

namespace FluidWarfare.Project.Content;

/// <summary>根据 GameContentFolderInfo 扫描项目内容目录的一级内容文件入口。校验扩展名，不解析内容。</summary>
public static class GameContentFileScanner
{
    static readonly HashSet<string> IgnoredFileNames = new(StringComparer.Ordinal) { ".gitkeep", ".DS_Store", "Thumbs.db" };

    public static GameContentFileScanResult Scan(string projectDirectory, IReadOnlyList<GameContentFolderInfo> contentFolders)
    {
        if (string.IsNullOrWhiteSpace(projectDirectory) || !Directory.Exists(projectDirectory))
            return new([], [new ProjectValidationIssue("Project.DirectoryMissing", "项目目录不存在。", "")]);
        if (contentFolders is null || contentFolders.Count == 0) return new([], []);
        var validFiles = new List<GameContentFileInfo>(); var issues = new List<ProjectValidationIssue>();
        foreach (var folder in contentFolders)
        {
            var folderPath = Path.Combine(projectDirectory, folder.FolderName);
            if (!Directory.Exists(folderPath)) continue;
            foreach (var subDirPath in Directory.EnumerateDirectories(folderPath))
            { var sn = Path.GetFileName(subDirPath); if (!string.IsNullOrWhiteSpace(sn) && !sn.StartsWith(".")) issues.Add(new("Project.NestedContentDirectoryUnsupported", $"不支持子目录：{folder.FolderName}/{sn}。", $"{folder.FolderName}/{sn}")); }
            foreach (var filePath in Directory.EnumerateFiles(folderPath))
            {
                var fileName = Path.GetFileName(filePath);
                if (string.IsNullOrWhiteSpace(fileName)) continue;
                if (IsIgnoredFile(fileName)) continue;
                var ext = Path.GetExtension(filePath).ToLowerInvariant();
                var relPath = $"{folder.FolderName}/{fileName}";
                if (folder.AllowedExtensions.Count == 0) { issues.Add(new("Project.ContentFileExtensionNotAllowed", $"扩展名不允许：{relPath}。", relPath)); continue; }
                if (!folder.AllowedExtensions.Contains(ext)) { issues.Add(new("Project.ContentFileExtensionNotAllowed", $"扩展名不允许：{relPath}。", relPath)); continue; }
                validFiles.Add(new(folder.FolderName, folder.ContentKind, fileName, relPath, ext));
            }
        }
        return new(validFiles, issues);
    }

    static bool IsIgnoredFile(string n) => n.StartsWith(".") || IgnoredFileNames.Contains(n);
}
