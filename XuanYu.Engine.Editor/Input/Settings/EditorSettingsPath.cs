namespace XuanYu.Engine.Editor.Input.Settings;

/// <summary>
/// 编辑器设置文件路径与文件夹名常量。
/// 当前目录 %APPDATA%/XuanYuEngine/Editor/editor.settings.json。
/// 旧目录 %APPDATA%/FluidWarfare/Editor/ 已保留兼容迁移。
/// </summary>
public static class EditorSettingsPath
{
    /// <summary>当前用户数据目录名。</summary>
    public const string CurrentAppFolder = "XuanYuEngine";

    /// <summary>旧用户数据目录名（仅供迁移读取）。</summary>
    public const string LegacyAppFolder = "FluidWarfare";

    /// <summary>编辑器子目录名。</summary>
    public const string EditorFolderName = "Editor";

    /// <summary>设置文件名。</summary>
    public const string SettingsFileName = "editor.settings.json";

    /// <summary>备份文件扩展名。</summary>
    public const string SettingsBakExtension = ".bak";
    /// <summary>设置文件完整路径。首次调用时触发旧→新目录迁移。</summary>
    public static string GetSettingsFilePath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var newDir = Path.Combine(appData, EditorSettingsPath.CurrentAppFolder,
            EditorSettingsPath.EditorFolderName);
        var legacyDir = Path.Combine(appData, EditorSettingsPath.LegacyAppFolder,
            EditorSettingsPath.EditorFolderName);

        // 首次调用时执行一次迁移（幂等）
        EditorSettingsPathMigration.MigrateIfNeeded(newDir, legacyDir);

        Directory.CreateDirectory(newDir);
        return Path.Combine(newDir, EditorSettingsPath.SettingsFileName);
    }

    /// <summary>备份文件路径。</summary>
    public static string GetBackupFilePath()
    {
        return GetSettingsFilePath() + EditorSettingsPath.SettingsBakExtension;
    }

    /// <summary>损坏文件重命名路径。</summary>
    public static string GetInvalidFilePath()
    {
        var ts = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        return Path.Combine(
            Path.GetDirectoryName(GetSettingsFilePath())!,
            $"editor.settings.invalid-{ts}.json");
    }

    /// <summary>设置文件所在目录。</summary>
    public static string GetSettingsDirectory()
    {
        return Path.GetDirectoryName(GetSettingsFilePath())!;
    }
}
