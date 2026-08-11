using System.IO;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-DOC-A-R1：Map Content Navigator 与 Manifest 最小 UI 接线合同。
public sealed class UiMapManifestNavigationTests
{
    static readonly string MapEditor = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapEditorPanel.axaml"));

    [Fact]
    public void Navigator_has_base_environment_and_dataset_entries()
    {
        Assert.Contains("Header=\"地图基础\"", MapEditor);
        Assert.Contains("Header=\"地图环境\"", MapEditor);
        Assert.Contains("Header=\"数据集\"", MapEditor);
        Assert.Contains("DatasetEmptyState", MapEditor);
    }

    [Fact]
    public void Navigator_has_no_dataset_registry_commands()
    {
        Assert.DoesNotContain("新建道路数据集", MapEditor);
        Assert.DoesNotContain("删除 Dataset", MapEditor);
        Assert.DoesNotContain("Region JSON", MapEditor);
    }
}
