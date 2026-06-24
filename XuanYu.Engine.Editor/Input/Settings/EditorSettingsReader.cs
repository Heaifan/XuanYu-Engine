using System.Text.Json;

namespace XuanYu.Engine.Editor.Input.Settings;

/// <summary>
/// 读取编辑器设置文件。失败时回退默认。
/// </summary>
public static class EditorSettingsReader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <summary>
    /// 读取设置文件。文件不存在返回默认。损坏时回退默认并保留损坏文件。
    /// </summary>
    /// <param name="warningMessage">输出警告信息（无警告时为空）。</param>
    /// <returns>设置文档。</returns>
    public static EditorSettingsDocument Read(out string? warningMessage)
    {
        warningMessage = null;
        var path = EditorSettingsPath.GetSettingsFilePath();

        if (!File.Exists(path))
            return new EditorSettingsDocument();

        try
        {
            var json = File.ReadAllText(path);
            var doc = JsonSerializer.Deserialize<EditorSettingsDocument>(json, JsonOptions);

            if (doc is null)
            {
                PreserveInvalidFile(path, "反序列化返回 null");
                warningMessage = "编辑器设置文件无法读取，已恢复 Blender 默认键位。";
                return new EditorSettingsDocument();
            }

            if (doc.SchemaVersion != 1)
            {
                PreserveInvalidFile(path, $"不支持的结构版本：{doc.SchemaVersion}");
                warningMessage = "编辑器设置文件版本不兼容，已恢复 Blender 默认键位。";
                return new EditorSettingsDocument();
            }

            return doc;
        }
        catch (JsonException ex)
        {
            PreserveInvalidFile(path, $"JSON 解析错误：{ex.Message}");
            warningMessage = "编辑器设置文件无法读取，已恢复 Blender 默认键位。";
            return new EditorSettingsDocument();
        }
        catch (IOException ex)
        {
            warningMessage = $"编辑器设置文件读取 I/O 错误：{ex.Message}，使用默认键位。";
            return new EditorSettingsDocument();
        }
    }

    private static void PreserveInvalidFile(string originalPath, string reason)
    {
        try
        {
            var invalidPath = EditorSettingsPath.GetInvalidFilePath();
            File.Move(originalPath, invalidPath);
            System.Diagnostics.Debug.WriteLine(
                $"[设置]保留损坏文件：{originalPath} → {invalidPath}（原因：{reason}）");
        }
        catch
        {
            // 无法重命名时忽略，不阻止 Editor 启动
        }
    }
}
