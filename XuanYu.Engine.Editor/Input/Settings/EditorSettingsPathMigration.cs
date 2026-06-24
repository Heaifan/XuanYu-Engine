namespace XuanYu.Engine.Editor.Input.Settings;

/// <summary>
/// 将旧 %APPDATA%/FluidWarfare 下的设置文件迁移到 %APPDATA%/XuanYuEngine。
/// 迁移规则：
///   新目录已有设置 → 跳过（不覆盖）。
///   新目录不存在，旧目录存在 → 复制旧文件到新目录。
///   新旧都不存在 → 不做任何操作。
///   迁移失败 → 不阻止编辑器启动。
/// </summary>
public static class EditorSettingsPathMigration
{
    /// <summary>
    /// 执行迁移。在编辑器启动时调用一次。
    /// </summary>
    /// <param name="newDir">新设置目录路径。</param>
    /// <param name="legacyDir">旧设置目录路径。</param>
    public static void MigrateIfNeeded(string newDir, string legacyDir)
    {
        if (Directory.Exists(newDir))
            return; // 新目录已有内容，不覆盖

        if (!Directory.Exists(legacyDir))
            return; // 旧目录也不存在，无需迁移

        try
        {
            Directory.CreateDirectory(newDir);
            foreach (var file in Directory.GetFiles(legacyDir, "*.*"))
            {
                var dest = Path.Combine(newDir, Path.GetFileName(file));
                if (!File.Exists(dest))
                    File.Copy(file, dest);
            }
        }
        catch (Exception ex)
        {
            // 迁移失败不阻止编辑器启动
            System.Diagnostics.Debug.WriteLine(
                $"[设置]旧目录迁移到新目录失败：{ex.Message}");
        }
    }

    /// <summary>返回旧设置文件路径（可能不存在）。</summary>
    public static string GetLegacySettingsFilePath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, EditorSettingsPath.LegacyAppFolder,
            EditorSettingsPath.EditorFolderName,
            EditorSettingsPath.SettingsFileName);
    }

    /// <summary>返回新设置目录路径。</summary>
    public static string GetNewSettingsDirectory()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, EditorSettingsPath.CurrentAppFolder,
            EditorSettingsPath.EditorFolderName);
    }

    /// <summary>返回旧设置目录路径。</summary>
    public static string GetLegacySettingsDirectory()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, EditorSettingsPath.LegacyAppFolder,
            EditorSettingsPath.EditorFolderName);
    }
}
