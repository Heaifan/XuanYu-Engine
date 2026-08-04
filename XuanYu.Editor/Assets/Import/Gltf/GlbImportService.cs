using SharpGLTF.Schema2;

namespace XuanYu.Editor.Assets;

public sealed class GlbImportService
{
    public StaticModelImportResult ImportFile(string path)
    {
        if (!File.Exists(path))
            return StaticModelImportResult.Fail(StaticModelImportErrorCode.SourceNotFound, "GLB 源文件不存在。", path);
        try
        {
            using var stream = File.OpenRead(path);
            return ImportStream(stream, Path.GetFileName(path));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return StaticModelImportResult.Fail(StaticModelImportErrorCode.SourceUnreadable, "GLB 源文件无法读取。", ex.Message);
        }
    }

    public StaticModelImportResult ImportStream(Stream stream, string displayName)
    {
        try
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ImportBytes(ms.ToArray(), displayName);
        }
        catch (OutOfMemoryException ex)
        {
            return StaticModelImportResult.Fail(StaticModelImportErrorCode.UnexpectedImportFailure, "模型过大，无法导入。", ex.Message);
        }
    }

    public StaticModelImportResult ImportBytes(byte[] bytes, string displayName)
    {
        var containerResult = GlbContainerReader.Read(bytes, out var container);
        if (!containerResult.Succeeded) return containerResult;
        if (GltfJsonAccess.TryArray(container!.Json.RootElement, "extensionsRequired", out var required) &&
            required.GetArrayLength() > 0)
            return StaticModelImportResult.Fail(
                StaticModelImportErrorCode.UnsupportedRequiredExtension,
                "GLB 包含当前不支持的必需扩展。",
                string.Join(",", required.EnumerateArray().Select(x => x.GetString())));
        var parserResult = ValidateWithSharpGltf(bytes);
        if (!parserResult.Succeeded) return parserResult;
        return new GltfStaticModelImporter(container, displayName, bytes.Length).Import();
    }

    static StaticModelImportResult ValidateWithSharpGltf(byte[] bytes)
    {
        try
        {
            var model = ModelRoot.ParseGLB(new ArraySegment<byte>(bytes));
            if (model.ExtensionsRequired.Count() > 0)
                return StaticModelImportResult.Fail(
                    StaticModelImportErrorCode.UnsupportedRequiredExtension,
                    "GLB 包含当前不支持的必需扩展。",
                    string.Join(",", model.ExtensionsRequired));
            return StaticModelImportResult.Success(null!);
        }
        catch (Exception ex)
        {
            return StaticModelImportResult.Fail(StaticModelImportErrorCode.ParserFailure, "GLB 解析失败。", ex.Message);
        }
    }
}
