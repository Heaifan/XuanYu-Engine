namespace XuanYu.Render.Abstractions.Diagnostics;

public static class RendererBuildIdentity
{
    public static string Current()
    {
        var head = ReadIdentity("XYE_SOURCE_HEAD");
        var dirty = ReadIdentity("XYE_SOURCE_DIRTY");
        var fingerprint = ReadIdentity("XYE_SOURCE_FINGERPRINT");
        var assembly = typeof(RendererBuildIdentity).Assembly;
        var build = Environment.GetEnvironmentVariable("XYE_BUILD_TIMESTAMP") ??
            File.GetLastWriteTimeUtc(assembly.Location).ToString("O");
        var version = assembly.GetName().Version?.ToString() ?? "UNKNOWN";
        return $"HEAD={head};DIRTY={dirty};TREE={fingerprint};BUILD={build};VERSION={version}";
    }

    static string ReadIdentity(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        return string.IsNullOrWhiteSpace(value) ? "UNKNOWN" : value.Trim();
    }
}
