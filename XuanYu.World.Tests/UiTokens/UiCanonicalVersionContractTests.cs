using System.IO;

namespace XuanYu.World.Tests.UiTokens;

public sealed class UiCanonicalVersionContractTests
{
    static string RootPath(params string[] segments) => Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", Path.Combine(segments));

    const string ExpectedVersion = "v0.3.0.0-r1";

    static string VersionSource() => File.ReadAllText(RootPath("Directory.Build.props"));

    [Fact]
    public void Canonical_version_uses_the_world_authoring_release_sequence() =>
        Assert.Contains("<Version>0.3.0.0-r1</Version>", VersionSource());

    [Fact]
    public void Version_source_declares_all_assembly_version_fields()
    {
        var content = VersionSource();
        Assert.Contains("<AssemblyVersion>0.3.0.0</AssemblyVersion>", content);
        Assert.Contains("<FileVersion>0.3.0.0</FileVersion>", content);
        Assert.Contains("<InformationalVersion>v0.3.0.0-r1</InformationalVersion>", content);
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
        Assert.StartsWith($"## {ExpectedVersion} · WORLD AUTHORING / TERRAIN FOUNDATION CUT-1", content.TrimStart());
    }
}
