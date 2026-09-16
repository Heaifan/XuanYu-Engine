using System.Diagnostics;

namespace XuanYu.World.Tests.UiTokens;

public sealed class EditorScrollAuditContractTests
{
    static string Root => Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI");

    [Fact]
    public void Inspector_has_one_right_content_scroll_host_and_fixed_shell()
    {
        var inspector = File.ReadAllText(Path.Combine(Root, "Right", "InspectorPanel.axaml"));
        var map = File.ReadAllText(Path.Combine(Root, "Right", "MapEditorPanel.axaml"));
        Assert.Contains("x:Name=\"SearchBox\"", inspector);
        Assert.Contains("XYNavigationRail", inspector);
        Assert.Equal(1, inspector.Split("<ScrollViewer", StringSplitOptions.None).Length - 1);
        Assert.DoesNotContain("<ScrollViewer", map);
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
