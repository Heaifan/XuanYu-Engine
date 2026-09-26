namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainCut1UiContractTests
{
    [Fact]
    public void Terrain_context_exposes_only_select_dem_import_and_terrain_menu()
    {
        var source = TestSource.Read("XuanYu.Editor.UI/Top/ContextToolBar.axaml");
        var code = TestSource.Read("XuanYu.Editor.UI/Top/ContextToolBar.axaml.cs");

        Assert.Contains("选择", source);
        Assert.Contains("导入DEM", source);
        Assert.Contains("Content=\"{Binding ContextToolbarButtonLabel}\"", source);
        Assert.Contains("xy:XYSplitButton", source);
        Assert.Contains("new(\"terrain\", \"地形\")", code);
        Assert.Contains("category.Id == \"terrain\"", code);
        Assert.Contains("EnterTerrainContext", code);
        Assert.Contains("MenuCommand", code);
        foreach (var forbidden in new[] { "移动", "旋转", "缩放", "编辑地形", "抬高", "压低", "平滑", "整平" })
            Assert.DoesNotContain(forbidden, source);
    }

    [Fact]
    public void Terrain_inspector_exposes_required_real_metadata()
    {
        var source = TestSource.Read("XuanYu.Editor.UI/Right/InspectorPanel.axaml");

        foreach (var label in new[] { "网格", "分辨率", "最低高程", "最高高程", "NoData", "垂直夸张" })
            Assert.Contains(label, source);
    }

    [Fact]
    public void Official_manual_acceptance_entry_is_run_bat()
    {
        Assert.True(File.Exists(TestSource.RootPath("run.bat")));
    }
}

static class TestSource
{
    public static string RootPath(string relative) =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../", relative));

    public static string Read(string relative) => File.ReadAllText(RootPath(relative));
}
