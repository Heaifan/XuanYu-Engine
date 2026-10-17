using System.Diagnostics;

internal static class XycCliStartConfiguration
{
    public static ProcessStartInfo Create(string appPath, string sessionId, string? demPath,
        bool diagnosticWrite, bool silent)
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
        start.ArgumentList.Add("--xyc-session");
        start.ArgumentList.Add(sessionId);
        if (diagnosticWrite) start.ArgumentList.Add("--xyc-diagnostic-write");
        if (demPath is not null)
        {
            start.ArgumentList.Add("--xyc-test-dem");
            start.ArgumentList.Add(demPath);
        }

        if (silent) ConfigureProbe(start);
        return start;
    }

    private static void ConfigureProbe(ProcessStartInfo start)
    {
        var directory = Path.Combine(Path.GetTempPath(), "XuanYuEngine", "XycSilentProbe",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        start.Environment["XYE_PROBE_VIEWPORT"] = "1";
        start.Environment["XYE_TERRAIN_REPLAY"] = "1";
        start.Environment["XYE_PROBE_DIRECTORY"] = directory;
        Console.Error.WriteLine($"XYC silent monitor phase=probe-configured; directory={directory}");
    }
}
