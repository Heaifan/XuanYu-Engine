using System.IO;
using System.Text.RegularExpressions;

namespace XuanYu.World.Tests.UiTokens;

public sealed class UiCanonicalVersionContractTests
{
    static string RootPath(params string[] segments) => Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", Path.Combine(segments));

    static string VersionSource() => File.ReadAllText(RootPath("Directory.Build.props"));

    static (string ProductVersion, string VersionPrefix, string AssemblyVersion, string FileVersion, string InformationalVersion) Versions()
    {
        var content = VersionSource();
        static string Value(string source, string element) =>
            Regex.Match(source, $"<{element}>([^<]+)</{element}>").Groups[1].Value;
        return (
            Value(content, "ProductVersion"),
            Value(content, "VersionPrefix"),
            Value(content, "AssemblyVersion"),
            Value(content, "FileVersion"),
            Value(content, "InformationalVersion"));
    }

    [Fact]
    public void Canonical_version_uses_the_world_authoring_release_sequence() =>
        Assert.Equal("v" + Versions().VersionPrefix + "-fix", Versions().ProductVersion);

    [Fact]
    public void Version_source_declares_all_assembly_version_fields()
    {
        var content = VersionSource();
        var versions = Versions();
        Assert.Contains($"<AssemblyVersion>{versions.AssemblyVersion}</AssemblyVersion>", content);
        Assert.Contains($"<FileVersion>{versions.FileVersion}</FileVersion>", content);
        Assert.Contains($"<InformationalVersion>{versions.InformationalVersion}</InformationalVersion>", content);
    }

    [Fact]
    public void Run_bat_reads_version_from_the_central_source()
    {
        var content = File.ReadAllText(RootPath("run.bat"));
        Assert.Contains("scripts\\resolve-version.ps1", content);
        Assert.DoesNotContain("0.2.28.77-fix3", content);
    }

    [Fact]
    public void Editor_window_uses_fallback_and_no_legacy_version()
    {
        var content = File.ReadAllText(RootPath("XuanYu.Editor.UI", "Win", "UiWin.axaml"));
        Assert.Contains("Title=\"玄域引擎编辑器\"", content);
        Assert.DoesNotContain("0.2.28.77-fix3", content);
    }

    [Fact]
    public void Document_window_title_uses_the_assembly_version_source()
    {
        var content = File.ReadAllText(RootPath(
            "XuanYu.Editor.UI", "Vm", "Scene", "UiVm.SceneDocument.cs"));
        Assert.Contains("AssemblyInformationalVersionAttribute", content);
        Assert.DoesNotContain("0.2.28.77-fix3", content);
    }

    [Fact]
    public void Changelog_starts_with_the_v03_baseline()
    {
        var content = File.ReadAllText(RootPath("changelog.md"));
        Assert.StartsWith($"## v{Versions().VersionPrefix}-fix ·", content.TrimStart());
    }
}
