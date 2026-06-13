using System.Text.Json;

namespace FluidWarfare.Editor.Input.Settings;

/// <summary>
/// 原子方式保存编辑器设置文件。
/// </summary>
public static class EditorSettingsWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// 原子保存设置文档。
    /// </summary>
    /// <param name="document">要保存的设置。</param>
    /// <param name="errorMessage">保存失败时的错误信息。</param>
    /// <returns>是否成功。</returns>
    public static bool TrySave(EditorSettingsDocument document, out string? errorMessage)
    {
        errorMessage = null;

        try
        {
            var finalPath = EditorSettingsPath.GetSettingsFilePath();
            var tempPath = finalPath + ".tmp";
            var bakPath = EditorSettingsPath.GetBackupFilePath();

            var json = JsonSerializer.Serialize(document, JsonOptions);

            // 写入临时文件
            File.WriteAllText(tempPath, json);
            File.SetAttributes(tempPath, FileAttributes.Normal);

            // 验证可重新读取
            try
            {
                var verify = JsonSerializer.Deserialize<EditorSettingsDocument>(
                    File.ReadAllText(tempPath), JsonOptions);
                if (verify is null)
                {
                    errorMessage = "验证临时设置文件失败：反序列化返回 null。";
                    File.Delete(tempPath);
                    return false;
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"验证临时设置文件失败：{ex.Message}。";
                File.Delete(tempPath);
                return false;
            }

            // 备份旧文件
            if (File.Exists(finalPath))
            {
                if (File.Exists(bakPath))
                    File.Delete(bakPath);
                File.Move(finalPath, bakPath);
            }

            // 原子替换
            File.Move(tempPath, finalPath);

            return true;
        }
        catch (Exception ex)
        {
            errorMessage = $"保存设置文件失败：{ex.Message}。";
            return false;
        }
    }
}
