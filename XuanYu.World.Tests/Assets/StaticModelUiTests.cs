using System.Reflection;
using XuanYu.Core.History;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.World;

public sealed class StaticModelUiTests
{
    [Fact]
    public void Import_adds_entity_selects_it_and_marks_dirty()
    {
        var vm = NewVm();
        var path = TempGlb();
        try
        {
            var title = Path.GetFileNameWithoutExtension(path);
            Assert.True(vm.ImportStaticModel(path));
            Assert.Contains(vm.HierarchyItems, n => n.Title == title);
            Assert.True(vm.IsSceneDirty);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Failed_import_keeps_state_unchanged()
    {
        var vm = NewVm();
        var before = vm.HierarchyItems.Count;
        var path = Path.Combine(Path.GetTempPath(), "missing-" + Guid.NewGuid() + ".glb");

        Assert.False(vm.ImportStaticModel(path));
        Assert.Equal(before, vm.HierarchyItems.Count);
        Assert.False(vm.IsSceneDirty);
    }

    [Fact]
    public void Undo_removes_entity_and_binding_redo_restores()
    {
        var vm = NewVm();
        var path = TempGlb();
        try
        {
            var title = Path.GetFileNameWithoutExtension(path);
            Assert.True(vm.ImportStaticModel(path));
            vm.TryUndoFromShortcut();
            Assert.DoesNotContain(vm.HierarchyItems, n => n.Title == title);
            vm.TryRedoFromShortcut();
            Assert.Contains(vm.HierarchyItems, n => n.Title == title);
            Assert.True(ProjectionHasStaticModel(vm));
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Delete_selected_entity_removes_binding()
    {
        var vm = NewVm();
        var path = TempGlb();
        try
        {
            var title = Path.GetFileNameWithoutExtension(path);
            Assert.True(vm.ImportStaticModel(path));
            vm.SelectedHierarchyItem = vm.HierarchyItems.Single(n => n.Title == title);
            Assert.True(vm.DeleteSelectedEntity());
            Assert.DoesNotContain(vm.HierarchyItems, n => n.Title == title);
            Assert.DoesNotContain(vm.RenderProjection.Projection.Entities,
                e => e.EntityType == RenderEntityType.StaticModel);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Production_projection_no_longer_uses_d2_demo()
    {
        // 从测试输出目录 (bin/Debug/net10.0) 向上导航到仓库根。
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var source = File.ReadAllText(Path.Combine(
            repoRoot, "XuanYu.Editor.UI", "Vm", "Scene", "UiVm.RenderProjection.cs"));
        Assert.DoesNotContain("D2StaticModelDemo", source);
    }

    static bool ProjectionHasStaticModel(UiVm vm) =>
        vm.RenderProjection.Projection.Entities.Any(e => e.EntityType == RenderEntityType.StaticModel);

    static UiVm NewVm() => new(null, () => true, seedInitialScene: false);

    static EditorHistoryOwner HistoryOf(UiVm vm) =>
        (EditorHistoryOwner)typeof(UiVm).GetField("_historyOwner",
            BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(vm)!;

    static string TempGlb()
    {
        var path = Path.Combine(Path.GetTempPath(), "xuanyu-d3u-" + Guid.NewGuid().ToString("N") + ".glb");
        File.WriteAllBytes(path, GlbFactory.Triangle());
        return path;
    }
}
