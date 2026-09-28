using System.IO;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class TopLeftInteractionR1Tests
{
    static readonly string Root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");

    [Fact]
    public void Create_cube_commits_scene_hierarchy_selection_and_inspector()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);

        vm.RunCommand.Execute("添加立方体");

        Assert.Contains(vm.HierarchyItems, item => item.Title == "立方体" && item.IsEntity);
        Assert.Equal("立方体", vm.SelectionTitle);
        Assert.Equal("立方体", vm.InspectorSelectionTitle);
        Assert.True(vm.HasInspectorSelection);
        Assert.Contains(vm.InspectorFields, field => field.Label == "名称" && field.Value == "立方体");
    }

    [Fact]
    public void Transform_availability_follows_edit_mode_and_canonical_selection()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.False(vm.CanTransformSelectedEntity);

        vm.RunCommand.Execute("添加立方体");
        Assert.True(vm.HasTransformableSelection);
        Assert.True(vm.CanTransformSelectedEntity);
        Assert.True(vm.SelectToolCommand.CanExecute("移动"));
        Assert.True(vm.SelectToolCommand.CanExecute("旋转"));
        Assert.True(vm.SelectToolCommand.CanExecute("缩放"));
        vm.SelectToolCommand.Execute("移动");
        Assert.True(vm.IsEditMode);
        Assert.True(vm.IsMoveTool);

        vm.SelectedHierarchyItem = null;
        Assert.False(vm.HasSelection);
        Assert.True(vm.IsMapInspector);
        Assert.True(vm.IsMapWorkspaceInspectorVisible);
        Assert.False(vm.HasTransformableSelection);
        Assert.False(vm.CanTransformSelectedEntity);
    }

    [Fact]
    public void Left_and_viewport_projections_share_canonical_selection()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.RunCommand.Execute("添加立方体");
        var cube = Assert.Single(vm.ProjectItems, item => item.IsEntity);

        vm.SelectedProjectItem = cube;
        Assert.Equal(cube.Key, vm.SelectionKey);
        Assert.Equal(cube.Key, vm.SelectedHierarchyItem!.Key);
        Assert.Equal("立方体", vm.InspectorSelectionTitle);

        var hierarchyCube = Assert.Single(vm.HierarchyItems, item => item.Key == cube.Key);
        vm.SelectedHierarchyItem = hierarchyCube;
        Assert.Equal(cube.Key, vm.SelectedProjectItem!.Key);
    }

    [Fact]
    public void Top_uses_one_new_menu_and_xyui_icons_for_actions()
    {
        var file = Read("XuanYu.Editor.UI", "Top", "FileModule.axaml");
        var runtime = Read("XuanYu.Editor.UI", "Top", "RuntimeStatusModule.axaml");
        var top = Read("XuanYu.Editor.UI", "Top", "Top.axaml");
        var tools = Read("XuanYu.Editor.UI", "Top", "EditToolsModule.axaml");
        var view = Read("XuanYu.Editor.UI", "Top", "ViewModule.axaml");
        var snap = Read("XuanYu.Editor.UI", "Top", "SnapModule.axaml");

        Assert.Contains("Label=\"新建\" Icon=\"NewFile\"", file);
        Assert.Contains("<xy:XYCaption Text=\"菜单\"", file);
        Assert.Contains("Classes=\"xyui-toolbar-menu-item\"", file);
        Assert.Contains("IconSize=\"Medium\"", file);
        Assert.Contains("CommandParameter=\"新建\"", file);
        Assert.Contains("CommandParameter=\"添加立方体\"", file);
        Assert.DoesNotContain("Label=\"添加\"", file);
        Assert.DoesNotContain("<Path", file);
        Assert.DoesNotContain("<Path", runtime);
        Assert.DoesNotContain("<Path", top + tools + view + snap);
        Assert.DoesNotContain("Stroke=\"White\"", runtime);
        Assert.Contains("Icon=\"Play\"", runtime);
    }

    static string Read(params string[] path) => File.ReadAllText(Path.Combine([Root, .. path]));
}
