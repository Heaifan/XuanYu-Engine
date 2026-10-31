using System.IO;

namespace XuanYu.World.Tests.UiTokens;

public sealed class TerrainTopContextContractTests
{
    static string Read(string rel) => File.ReadAllText(Path.Combine(TestSourceRoot.Root,  "XuanYu.Editor.UI", rel));

    [Fact]
    public void Terrain_context_has_real_import_and_context_switch_contract()
    {
        var toolbar = Read("Panels/Tools/ContextTools/ContextToolBar.axaml");
        var code = Read("Panels/Tools/ContextTools/ContextToolBar.axaml.cs");
        var vm = Read("Panels/Workspace/WorkspaceCommands/UiVm.TerrainContext.cs");
        Assert.Contains("导入DEM", toolbar);
        Assert.Contains("ImportTerrainCommand", toolbar);
        Assert.Contains("EnterTerrainContext", code);
        Assert.Contains("EnterRegionContext", code);
        Assert.Contains("FileCommandRequested", vm);
        Assert.Contains("导入DEM", vm);
        Assert.DoesNotContain("编辑地形", toolbar);
        Assert.DoesNotContain("抬高", toolbar);
        Assert.DoesNotContain("压低", toolbar);
        Assert.DoesNotContain("平滑", toolbar);
        Assert.DoesNotContain("整平", toolbar);
        Assert.DoesNotContain("半径", toolbar);
        Assert.DoesNotContain("强度", toolbar);
    }

    [Fact]
    public void Terrain_mode_does_not_render_region_transform_module()
    {
        var editTools = Read("Panels/Tools/ContextTools/EditToolsModule.axaml");
        Assert.Contains("IsVisible=\"{Binding IsRegionEditMode}\"", editTools);
        Assert.Contains("IsVisible=\"{Binding IsTerrainContext}\"", Read("Panels/Tools/ContextTools/ContextToolBar.axaml"));
        Assert.Contains("导入DEM", Read("Panels/Tools/ContextTools/ContextToolBar.axaml"));
    }
}
