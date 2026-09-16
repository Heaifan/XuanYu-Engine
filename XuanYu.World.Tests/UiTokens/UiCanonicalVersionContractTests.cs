using System.IO;
using System.Text.RegularExpressions;

namespace XuanYu.World.Tests.UiTokens;

public sealed class UiCanonicalVersionContractTests
{
    static string RootPath(params string[] segments) => Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", Path.Combine(segments));

    static string CanonicalVersion()
    {
        var project = File.ReadAllText(RootPath("XuanYu.Editor.App", "XuanYu.Editor.App.csproj"));
        var match = Regex.Match(project, "<Version>([^<]+)</Version>");
        return $"v{match.Groups[1].Value}";
    }

    [Fact]
    public void Canonical_version_uses_the_formal_four_part_rz_sequence() =>
        Assert.Matches(@"^v\d+\.\d+\.\d+\.\d+-(?:rz|fix\d*|vk|stab)$", CanonicalVersion());

    [Fact]
    public void Run_bat_declares_canonical_version()
    {
        var content = File.ReadAllText(RootPath("run.bat"));
        Assert.Contains($"title XuanYu Engine Editor {CanonicalVersion()}", content);
    }

    [Fact]
    public void Editor_window_declares_canonical_version()
    {
        var content = File.ReadAllText(RootPath("XuanYu.Editor.UI", "Win", "UiWin.axaml"));
        Assert.Contains($"Title=\"玄域引擎编辑器 {CanonicalVersion()}\"", content);
    }

    [Fact]
    public void Document_window_title_declares_canonical_version()
    {
        var content = File.ReadAllText(RootPath(
            "XuanYu.Editor.UI", "Vm", "Scene", "UiVm.SceneDocument.cs"));
        Assert.Contains($"DocumentWindowTitle => $\"玄域引擎编辑器 {CanonicalVersion()} - {{DocumentTitle}}\";", content);
    }

    [Fact]
    public void Changelog_declares_canonical_version()
    {
        var content = File.ReadAllText(RootPath("changelog.md"));
        Assert.Contains(CanonicalVersion(), content);
        Assert.StartsWith($"## {CanonicalVersion()}", content.TrimStart());
    }
}
