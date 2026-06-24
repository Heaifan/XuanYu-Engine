namespace FluidWarfare.Project.Content;

/// <summary>
/// 表示项目中一个合法内容文件入口。
/// 只保存文件入口数据，不读取文件内容，不解析 JSON，不解析图片。
/// </summary>
public sealed record GameContentFileInfo(
    string FolderName,
    string ContentKind,
    string FileName,
    string RelativePath,
    string Extension);
