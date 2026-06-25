using System.IO;

namespace XuanYu.Engine.Editor.Windows.Shell.Diagnostics;

/// <summary>探针静态路由。输出到终端并追加到审计日志文件，不写 UI。</summary>
public static class EditorProbe
{
    static readonly object _lock = new();
    static readonly string _logPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "XuanYuEngine",
        "editor_probe.log");

    public static void Write(string module, string stage, string message)
    {
        var line = $"[{DateTime.Now:HH:mm:ss.fff}][探针][{module}][{stage}] {message}";
        Console.WriteLine(line);
        try
        {
            var dir = Path.GetDirectoryName(_logPath)!;
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, "editor_probe.log");
            lock (_lock)
            {
                File.AppendAllText(path, line + Environment.NewLine, System.Text.Encoding.UTF8);
            }
        }
        catch (Exception ex)
        {
            try
            {
                var errPath = Path.Combine(Path.GetTempPath(), "xuan_probe_error.log");
                File.AppendAllText(errPath, $"[{DateTime.Now}] 探针文件写入失败: {ex}{Environment.NewLine}", System.Text.Encoding.UTF8);
            }
            catch { }
        }
    }
}
