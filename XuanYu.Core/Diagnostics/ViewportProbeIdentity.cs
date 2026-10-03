using System.Reflection;

namespace XuanYu.Core.Diagnostics;

public static class ViewportProbeIdentity
{
    public const string ProbeName = "INSTRUMENT-VIEWPORT-ZOOM-GROUND-DEM-R1";

    public static string Current()
    {
        var head = Environment.GetEnvironmentVariable("XYE_PROBE_SOURCE_HEAD") ?? "UNKNOWN";
        var dirty = string.Equals(Environment.GetEnvironmentVariable("XYE_PROBE_SOURCE_DIRTY"), "YES", StringComparison.OrdinalIgnoreCase);
        var build = Environment.GetEnvironmentVariable("XYE_PROBE_BUILD_TIMESTAMP") ??
            File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("O");
        return Format(head, dirty, ProbeName, build);
    }

    public static string Format(string head, bool dirty, string probe, string build) =>
        $"HEAD={head};DIRTY={(dirty ? "YES" : "NO")};PROBE={probe};BUILD={build}";
}
