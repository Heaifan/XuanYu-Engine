using System.Diagnostics;
using System.Security.Cryptography;

internal static class XycCliStartConfiguration
{
    public static ProcessStartInfo Create(string appPath, string sessionId, string? demPath,
        bool diagnosticWrite, bool silent, bool uiHost = false, string? evidenceDirectory = null)
    {
        var start = new ProcessStartInfo(appPath)
        {
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = false
        };
        start.ArgumentList.Add("--xyc-stdio");
        if (silent) start.ArgumentList.Add("--xyc-silent");
        if (uiHost) start.ArgumentList.Add("--xyc-ui");
        start.ArgumentList.Add("--xyc-session");
        start.ArgumentList.Add(sessionId);
        if (diagnosticWrite) start.ArgumentList.Add("--xyc-diagnostic-write");
        if (demPath is not null)
        {
            start.ArgumentList.Add("--xyc-test-dem");
            start.ArgumentList.Add(demPath);
        }
        if (evidenceDirectory is not null)
            start.Environment["XYC_UI_EVIDENCE_DIR"] = evidenceDirectory;
        if (uiHost)
        {
            start.Environment["XYC_BUILD_IDENTITY"] = BuildIdentity(appPath);
            start.Environment["XYC_HOST_MODE"] = "production-ui";
        }

        if (silent) ConfigureProbe(start);
        return start;
    }

    static string BuildIdentity(string appPath)
    {
        var appDirectory = Path.GetDirectoryName(appPath)!;
        var appHash = Hash(appPath);
        var uiPath = Path.Combine(appDirectory, "XuanYu.Editor.UI.dll");
        return $"Editor.App.exe={appHash};XuanYu.Editor.UI.dll={Hash(uiPath)}";
    }

    static string Hash(string path) =>
        Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));

    private static void ConfigureProbe(ProcessStartInfo start)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(start.FileName)!, "../../../../"));
        var workspace = Path.Combine(projectRoot, ".xye-workspace");
        var runId = $"silent-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}";
        var directory = Path.Combine(workspace, "Diagnostics", runId);
        Directory.CreateDirectory(directory);
        start.Environment["XYE_PROBE_VIEWPORT"] = "1";
        start.Environment["XYE_TERRAIN_REPLAY"] = "1";
        start.Environment["XYE_WORKSPACE_ROOT"] = workspace;
        start.Environment["XYE_TASK_RUN_ID"] = runId;
        start.Environment["XYE_DIAGNOSTICS_DIRECTORY"] = directory;
        start.Environment["XYE_PROBE_DIRECTORY"] = directory;
        Console.Error.WriteLine($"XYC silent monitor phase=probe-configured; directory={directory}");
    }
}
