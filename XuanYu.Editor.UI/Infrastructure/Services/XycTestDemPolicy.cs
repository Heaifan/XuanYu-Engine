using System.Security.Cryptography;

namespace XuanYu.Editor.UI.XycControl;

public static class XycTestDemPolicy
{
    public const string SourceSha256 = "BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1";

    public static string Resolve(string applicationBaseDirectory, string path)
    {
        var root = Path.GetFullPath(Path.Combine(applicationBaseDirectory, "../../../../"));
        var expected = Path.GetFullPath(Path.Combine(root,
            "artifacts/diagnostics/wp2-p4-r3/n24e121.hgt"));
        var candidate = Path.GetFullPath(path);
        if (!string.Equals(candidate, expected, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("XYC 只允许读取固定 n24e121 测试 DEM。");
        using var stream = File.OpenRead(candidate);
        var hash = Convert.ToHexString(SHA256.HashData(stream));
        if (!string.Equals(hash, SourceSha256, StringComparison.Ordinal))
            throw new InvalidDataException("n24e121 测试 DEM SHA-256 与登记值不符。");
        return candidate;
    }
}
