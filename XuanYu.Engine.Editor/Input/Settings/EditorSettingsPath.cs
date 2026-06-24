namespace FluidWarfare.Editor.Input.Settings;

/// <summary>
/// 编辑器设置文件路径。
/// 文件保存在 %APPDATA%/FluidWarfare/Editor/editor.settings.json。
/// </summary>
public static class EditorSettingsPath
{
    private const string AppFolderName = "FluidWarfare";
    private const string EditorFolderName = "Editor";
    private const string SettingsFileName = "editor.settings.json";
    private const string SettingsBakExtension = ".bak";

    /// <summary>设置文件完整路径。</summary>
    public static string GetSettingsFilePath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var dir = Path.Combine(appData, AppFolderName, EditorFolderName);
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, SettingsFileName);
    }

    /// <summary>备份文件路径。</summary>
    public static string GetBackupFilePath()
    {
        return GetSettingsFilePath() + SettingsBakExtension;
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
