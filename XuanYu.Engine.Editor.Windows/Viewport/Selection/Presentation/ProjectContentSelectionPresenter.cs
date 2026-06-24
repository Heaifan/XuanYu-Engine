using XuanYu.Engine.Editor.Windows.Shell;
using XuanYu.Engine.Project.Content;

namespace XuanYu.Engine.Editor.Windows.Viewport.Selection.Presentation;

/// <summary>项目文件选择 → Inspector / StatusBar / Log 展示。纯转换，不接触 UI 控件。</summary>
public sealed class ProjectContentSelectionPresenter
{
    public ProjectContentSelectionResult Present(string? relativePath, IReadOnlyList<GameContentFileInfo>? contentFiles)
    {
        if (relativePath is null)
            return new ProjectContentSelectionResult(new EditorSelection("项目文件", "无", ""), null, string.Empty);

        var fileInfo = contentFiles?.FirstOrDefault(f => f.RelativePath.Replace('\\', '/') == relativePath);
        if (fileInfo is not null)
        {
            var selection = new EditorSelection("项目文件", fileInfo.FileName,
                $"路径：{fileInfo.RelativePath}\n类型：{fileInfo.ContentKind}\n目录：{fileInfo.FolderName}");
            return new ProjectContentSelectionResult(selection, fileInfo.FileName,
                $"项目文件已选择：{relativePath}");
        }

        return new ProjectContentSelectionResult(
            new EditorSelection("项目文件", relativePath, relativePath), relativePath,
            $"项目文件已选择：{relativePath}");
    }
}
