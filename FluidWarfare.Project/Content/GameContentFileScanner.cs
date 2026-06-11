using FluidWarfare.Core.Results;

namespace FluidWarfare.Project.Content;

/// <summary>
/// 根据 GameContentFolderInfo 扫描项目内容目录中
/// 的合法一级内容文件入口，并返回 GameContentFileInfo 列表或错误。
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
    /// 扫描已声明内容目录中的一级内容文件，并根据 allowedExtensions 校验扩展名。
    /// </summary>
    /// <param name="projectDirectory">项目根目录。</param>
    /// <param name="contentFolders">已声明的内容目录列表。</param>
    /// <param name="contentFiles">扫描到的合法内容文件入口列表。</param>
    /// <returns>扫描结果，成功或失败。</returns>
    public static EngineResult Scan(
        string projectDirectory,
        IReadOnlyList<GameContentFolderInfo> contentFolders,
        out IReadOnlyList<GameContentFileInfo> contentFiles)
    {
        contentFiles = [];

        if (string.IsNullOrWhiteSpace(projectDirectory) || !Directory.Exists(projectDirectory))
        {
            return EngineResult.Fail(EngineError.Create(
                "Project.DirectoryMissing",
                "项目目录不存在。"));
        }

        if (contentFolders is null || contentFolders.Count == 0)
        {
            return EngineResult.Success();
        }

        var result = new List<GameContentFileInfo>();

        foreach (var folder in contentFolders)
        {
            var folderPath = Path.Combine(projectDirectory, folder.FolderName);

            if (!Directory.Exists(folderPath))
            {
                continue;
            }

            // 先检查嵌套子目录 —— 第一版暂不支持
            foreach (var subDirPath in Directory.EnumerateDirectories(folderPath))
            {
                var subDirName = Path.GetFileName(subDirPath);

                if (string.IsNullOrWhiteSpace(subDirName) || subDirName.StartsWith(".", StringComparison.Ordinal))
                {
                    continue;
                }

                return EngineResult.Fail(EngineError.Create(
                    "Project.NestedContentDirectoryUnsupported",
                    $"项目内容目录暂不支持子目录：{folder.FolderName}/{subDirName}。"));
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

                // allowedExtensions 为空时不接受普通内容文件
                if (folder.AllowedExtensions.Count == 0)
                {
                    return EngineResult.Fail(EngineError.Create(
                        "Project.ContentFileExtensionNotAllowed",
                        $"项目内容文件扩展名不允许：{folder.FolderName}/{fileName}。"));
                }

                // 校验扩展名必须在 allowedExtensions 中
                if (!folder.AllowedExtensions.Contains(extension))
                {
                    return EngineResult.Fail(EngineError.Create(
                        "Project.ContentFileExtensionNotAllowed",
                        $"项目内容文件扩展名不允许：{folder.FolderName}/{fileName}。"));
                }

                var relativePath = $"{folder.FolderName}/{fileName}";

                result.Add(new GameContentFileInfo(
                    folder.FolderName,
                    folder.ContentKind,
                    fileName,
                    relativePath,
                    extension));
            }
        }

        contentFiles = result;
        return EngineResult.Success();
    }

    private static bool IsIgnoredFile(string fileName)
    {
        return fileName.StartsWith(".", StringComparison.Ordinal) ||
               IgnoredFileNames.Contains(fileName);
    }
}
