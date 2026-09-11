using System.IO;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-DOC-A-R1：Map Content Navigator 与 Manifest 最小 UI 接线合同。
public sealed class UiMapManifestNavigationTests
{
    static readonly string MapEditor = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapEditorPanel.axaml"));
    static readonly string DatasetPanel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "DatasetPanel.axaml"));

    [Fact]
    public void Navigator_has_base_environment_and_dataset_entries()
    {
        Assert.Contains("Id=\"base\" Label=\"基础\"", MapEditor);
        Assert.Contains("Id=\"environment\" Label=\"环境\"", MapEditor);
        Assert.Contains("Id=\"data\" Label=\"数据\"", MapEditor);
        Assert.Contains("DatasetEmptyState", DatasetPanel);
    }

    [Fact]
    public void Navigator_has_no_dataset_registry_commands()
    {
        Assert.DoesNotContain("新建道路数据集", MapEditor);
        Assert.DoesNotContain("删除 Dataset", MapEditor);
        Assert.DoesNotContain("Region JSON", MapEditor);
    }
}
