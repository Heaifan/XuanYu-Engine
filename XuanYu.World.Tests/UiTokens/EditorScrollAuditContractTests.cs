using System.Diagnostics;

namespace XuanYu.World.Tests.UiTokens;

public sealed class EditorScrollAuditContractTests
{
    static string Root => Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI");

    [Fact]
    public void Inspector_uses_sections_without_total_scroll_host()
    {
        var inspector = File.ReadAllText(Path.Combine(Root, "Right", "InspectorPanel.axaml"));
        var map = File.ReadAllText(Path.Combine(Root, "Right", "MapEditorPanel.axaml"));
        Assert.DoesNotContain("InspectorScrollViewer", inspector);
        Assert.DoesNotContain("<ScrollViewer", inspector);
        Assert.Contains("XYPager", map);
        Assert.Contains("XYInspectorSection", map);
    }

    [Fact]
    public void Scroll_audit_script_passes_current_editor_ui()
    {
        var repo = Path.GetFullPath(Path.Combine(Root, ".."));
        var script = Path.Combine(repo, "scripts", "editor-scroll-audit.ps1");
        var shell = Environment.GetEnvironmentVariable("ComSpec") ?? "powershell.exe";
        var info = new ProcessStartInfo(shell, $"/c powershell -NoProfile -ExecutionPolicy Bypass -File \"{script}\"")
        { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        using var process = Process.Start(info)!;
        process.WaitForExit();
        Assert.Equal(0, process.ExitCode);
        Assert.Contains("PASS", process.StandardOutput.ReadToEnd());
    }
}
