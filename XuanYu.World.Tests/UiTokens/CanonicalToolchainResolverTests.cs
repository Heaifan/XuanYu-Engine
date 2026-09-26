using System.Diagnostics;

namespace XuanYu.World.Tests.UiTokens;

public sealed class CanonicalToolchainResolverTests
{
    static string RootPath(params string[] segments) => Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", Path.Combine(segments));

    static string Read(params string[] segments) => File.ReadAllText(RootPath(segments));

    [Fact]
    public void Run_bat_uses_the_canonical_resolver() =>
        Assert.Contains("scripts\\resolve-dotnet.ps1", Read("run.bat"));

    [Fact]
    public void Wrapper_consumes_the_canonical_resolver_without_machine_paths()
    {
        var content = Read("scripts", "xye-dotnet.ps1");
        Assert.Contains("resolve-dotnet.ps1", content);
        Assert.DoesNotContain(":\\MyApp\\sdk-dotnet", content);
        Assert.DoesNotContain(":\\DevTools\\dotnet", content);
    }

    [Fact]
    public void Bootstrap_consumes_the_canonical_resolver() =>
        Assert.Contains("resolve-dotnet.ps1", Read("scripts", "xye-bootstrap.ps1"));

    [Fact]
    public void Repository_documents_require_resolver_first()
    {
        Assert.Contains("Repository Bootstrap", Read("AGENTS.md"));
        Assert.Contains("Resolver First", Read("AGENTS.md"));
        var rules = Read("docs", "dev-rules.md");
        Assert.Contains("xye-dotnet.ps1", rules);
        Assert.DoesNotContain("\ndotnet build", rules);
        Assert.DoesNotContain("\ndotnet test", rules);
        Assert.DoesNotContain("\ndotnet restore", rules);
    }

    [Fact]
    public void Resolver_returns_a_runnable_dotnet_with_an_sdk()
    {
        var script = RootPath("scripts", "resolve-dotnet.ps1");
        var result = RunPowerShell($"-NoProfile -ExecutionPolicy Bypass -File \"{script}\"");
        Assert.Equal(0, result.ExitCode);
        var dotnet = result.Stdout.Trim();
        Assert.True(File.Exists(dotnet), result.Stdout + result.Stderr);
        var info = Run(dotnet, "--list-sdks");
        Assert.Equal(0, info.ExitCode);
        Assert.NotEmpty(info.Stdout.Trim());
    }

    static ProcessResult RunPowerShell(string arguments) => Run("powershell.exe", arguments);

    static ProcessResult Run(string fileName, string arguments)
    {
        using var process = Process.Start(new ProcessStartInfo(fileName, arguments)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        })!;
        process.WaitForExit();
        return new(process.ExitCode, process.StandardOutput.ReadToEnd(), process.StandardError.ReadToEnd());
    }

    readonly record struct ProcessResult(int ExitCode, string Stdout, string Stderr);
}
