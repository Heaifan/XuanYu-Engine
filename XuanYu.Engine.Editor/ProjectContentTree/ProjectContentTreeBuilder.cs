using XuanYu.Engine.Project.Content;
using XuanYu.Engine.Project.Metadata;

namespace FluidWarfare.Editor.ProjectContentTreeModel;

/// <summary>
/// 从 GameProjectInfo 构建 ProjectContentTree。
/// 目录顺序严格遵循 contentFolders 声明顺序，空目录仍显示。
/// 文件扩展名必须符合目录允许扩展名。
/// 路径统一使用 /。
/// </summary>
public static class ProjectContentTreeBuilder
{
    /// <summary>
    /// 从项目元数据构建内容树。
    /// </summary>
    public static ProjectContentTree Build(GameProjectInfo projectInfo)
    {
        var fileNodes = new Dictionary<string, ProjectContentTreeNode>();
        var folderNodes = new List<ProjectContentTreeNode>();

        foreach (var folder in projectInfo.ContentFolders)
        {
            // 该目录下的文件
            var folderFiles = projectInfo.ContentFiles
                .Where(f => f.FolderName == folder.FolderName)
                .OrderBy(f => f.FileName, StringComparer.Ordinal)
                .ThenBy(f => f.RelativePath, StringComparer.Ordinal)
                .ToList();

            var childNodes = new List<ProjectContentTreeNode>();

            foreach (var file in folderFiles)
            {
                var normalizedPath = file.RelativePath.Replace('\\', '/');
                var nodeId = $"file:{normalizedPath}";

                if (fileNodes.ContainsKey(normalizedPath))
                    throw new InvalidOperationException(
                        $"项目内容树：重复的相对路径 '{normalizedPath}'。");

                var fileNode = new ProjectContentTreeNode(
                    nodeId,
                    ProjectContentTreeNodeKind.ContentFile,
                    file.FileName,
                    normalizedPath,
                    file.ContentKind,
                    "file",
                    true,
                    []);
                childNodes.Add(fileNode);
                fileNodes[normalizedPath] = fileNode;
            }

            var folderNodeId = $"folder:{folder.FolderName}";
            var folderNode = new ProjectContentTreeNode(
                folderNodeId,
                ProjectContentTreeNodeKind.ContentFolder,
                $"{folder.DisplayName} ({folderFiles.Count})",
                null,
                folder.ContentKind,
                "folder",
                false,
                childNodes.AsReadOnly());
            folderNodes.Add(folderNode);
        }

        var root = new ProjectContentTreeNode(
            $"project:{projectInfo.ProjectId}",
            ProjectContentTreeNodeKind.ProjectRoot,
            projectInfo.DisplayName,
            null, null, "project", false,
            folderNodes.AsReadOnly());

        var nodeCount = 1 + folderNodes.Count + fileNodes.Count;
        return new ProjectContentTree(root, nodeCount,
            folderNodes.Count, fileNodes.Count, fileNodes);
    }
}
