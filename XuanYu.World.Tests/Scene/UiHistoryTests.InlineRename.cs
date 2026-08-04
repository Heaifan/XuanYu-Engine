using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed partial class UiHistoryTests
{
    [Fact]
    public void Rename_entry_prepares_the_complete_current_name_for_replacement()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.AddCubeEntity();
        Assert.True(vm.RenameSelectedEntity("指挥所"));

        Assert.True(vm.BeginRenameSelectedEntity());

        Assert.True(vm.SelectedHierarchyItem!.IsRenaming);
        Assert.Equal("指挥所", vm.SelectedHierarchyItem.RenameText);
    }

    [Fact]
    public void Visible_inline_rename_focuses_then_selects_all_after_layout()
    {
        var actions = new List<string>();
        Action? queued = null;

        InlineRenameActivation.Schedule(() => true, action => queued = action,
            () => actions.Add("Focus"), () => actions.Add("SelectAll"));
        Assert.Empty(actions);

        queued!();

        Assert.Equal(["Focus", "SelectAll"], actions);
    }

    [Fact]
    public void F2_and_hierarchy_context_use_the_same_inline_rename_contract()
    {
        var f2 = new UiVm(null, () => true, seedInitialScene: false);
        var context = new UiVm(null, () => true, seedInitialScene: false);
        f2.AddCubeEntity();
        context.AddCubeEntity();

        Assert.True(f2.BeginRenameFromShortcut());
        Assert.True(context.BeginRenameFromHierarchyContext());

        Assert.True(f2.SelectedHierarchyItem!.IsRenaming);
        Assert.True(context.SelectedHierarchyItem!.IsRenaming);
        Assert.Equal(f2.SelectedHierarchyItem.RenameText,
            context.SelectedHierarchyItem.RenameText);
    }
}
