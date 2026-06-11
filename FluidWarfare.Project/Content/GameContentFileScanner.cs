using FluidWarfare.Project.Validation;

namespace FluidWarfare.Project.Content;

/// <summary>
/// 根据 GameContentFolderInfo 扫描项目内容目录中
/// 的合法一级内容文件入口，返回合法文件入口并收集文件级校验问题。
/// 只扫描文件入口，只校验扩展名，不解析文件内容，不写日志，不依赖 Editor。
/// </summary>
public static class GameContentFileScanner
{
    private static readonly HashSet<string> IgnoredFileNames =
        new(StringComparer.Ordinal)
        {
            ".gitkeep",
            ".DS_Store",
            "Thumbs.db"
        };

    /// <summary>
    /// 扫描已声明内容目录中的一级内容文件，收集所有合法文件入口和校验问题。
    /// 不因单个文件错误而停止扫描。
    /// </summary>
    /// <param name="projectDirectory">项目根目录。</param>
    /// <param name="contentFolders">已声明的内容目录列表。</param>
    /// <returns>扫描结果，包含合法内容文件入口列表和校验问题列表。</returns>
    public static GameContentFileScanResult Scan(
        string projectDirectory,
        IReadOnlyList<GameContentFolderInfo> contentFolders)
    {
        if (string.IsNullOrWhiteSpace(projectDirectory) || !Directory.Exists(projectDirectory))
        {
            return new GameContentFileScanResult(
                [],
                [new ProjectValidationIssue(
                    "Project.DirectoryMissing",
                    "项目目录不存在。",
                    "")]);
        }

        if (contentFolders is null || contentFolders.Count == 0)
        {
            return new GameContentFileScanResult([], []);
        }

        var validFiles = new List<GameContentFileInfo>();
        var issues = new List<ProjectValidationIssue>();

        foreach (var folder in contentFolders)
        {
            var folderPath = Path.Combine(projectDirectory, folder.FolderName);

            if (!Directory.Exists(folderPath))
            {
                continue;
            }

            // 检查嵌套子目录 —— 第一版暂不支持，收集但不中断
            foreach (var subDirPath in Directory.EnumerateDirectories(folderPath))
            {
                var subDirName = Path.GetFileName(subDirPath);

                if (string.IsNullOrWhiteSpace(subDirName) || subDirName.StartsWith(".", StringComparison.Ordinal))
                {
                    continue;
                }

                issues.Add(new ProjectValidationIssue(
                    "Project.NestedContentDirectoryUnsupported",
                    $"项目内容目录暂不支持子目录：{folder.FolderName}/{subDirName}。",
                    $"{folder.FolderName}/{subDirName}"));
            }

            // 扫描一级文件
            foreach (var filePath in Directory.EnumerateFiles(folderPath))
            {
                var fileName = Path.GetFileName(filePath);

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    continue;
                }

                // 忽略保留占位文件与隐藏文件
                if (IsIgnoredFile(fileName))
                {
                    continue;
                }

                var extension = Path.GetExtension(filePath).ToLowerInvariant();
                var relativePath = $"{folder.FolderName}/{fileName}";

                // allowedExtensions 为空时不接受普通内容文件
                if (folder.AllowedExtensions.Count == 0)
                {
                    issues.Add(new ProjectValidationIssue(
                        "Project.ContentFileExtensionNotAllowed",
                        $"项目内容文件扩展名不允许：{relativePath}。",
                        relativePath));
                    continue;
                }

                // 校验扩展名必须在 allowedExtensions 中
                if (!folder.AllowedExtensions.Contains(extension))
                {
                    issues.Add(new ProjectValidationIssue(
                        "Project.ContentFileExtensionNotAllowed",
                        $"项目内容文件扩展名不允许：{relativePath}。",
                        relativePath));
                    continue;
                }

                validFiles.Add(new GameContentFileInfo(
                    folder.FolderName,
                    folder.ContentKind,
                    fileName,
                    relativePath,
                    extension));
            }
        }

        return new GameContentFileScanResult(validFiles, issues);
    }

    private static bool IsIgnoredFile(string fileName)
    {
        return fileName.StartsWith(".", StringComparison.Ordinal) ||
               IgnoredFileNames.Contains(fileName);
    }
}
