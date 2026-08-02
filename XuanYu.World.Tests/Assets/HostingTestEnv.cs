using XuanYu.Core.Identity;
using XuanYu.Editor.Assets;

namespace XuanYu.World.Tests.Assets;

// D4-R1：托管事务测试辅助。所有测试使用独立临时目录，测试结束清理；
// AssetId 使用确定性值，不依赖随机排序。
static class HostingTestEnv
{
    public static string NewDirectory()
    {
        var root = Path.Combine(Path.GetTempPath(), "xy-host-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    public static string NewScene(string directory, string name = "Battle01")
    {
        var path = Path.Combine(directory, name + ".xyscene");
        File.WriteAllText(path, "{}");
        return path;
    }

    public static string NewGlb(string directory, string name, byte[]? content = null)
    {
        var path = Path.Combine(directory, name + ".glb");
        File.WriteAllBytes(path, content ?? [0x67, 0x6C, 0x62, 0x46]);
        return path;
    }

    public static AssetId Asset(string hex) =>
        AssetId.TryParse("asset_" + hex.PadLeft(32, '0'), out var id) ? id : default;

    public static void Cleanup(string directory)
    {
        try { if (Directory.Exists(directory)) Directory.Delete(directory, true); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}
