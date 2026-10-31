using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.Tree;

public sealed class test_ProjectTreePresenter
{
    [Fact]
    public void Build_and_toggle_own_tree_projection_and_notify_items()
    {
        var owner = new SceneStateOwner();
        var presenter = new ProjectTreePresenter();
        var items = presenter.Build("场景", "场景", owner.Entities);
        var root = Assert.Single(items, item => item.Key == "scene:current");
        Assert.True(root.CanToggle);
        Assert.Equal(2, items.Count);

        Assert.True(presenter.Toggle(root));

        Assert.Single(presenter.Build("场景", "场景", owner.Entities));
        Assert.Contains("scene:current", presenter.CollapsedKeys);
    }

    [Fact]
    public void Selected_item_is_a_notified_display_projection()
    {
        var presenter = new ProjectTreePresenter();
        var node = Assert.Single(presenter.Build("场景", "场景", []));
        Assert.True(presenter.SetSelectedItem(node));

        Assert.Same(node, presenter.SelectedItem);
        Assert.False(presenter.SetSelectedItem(node));
    }

    [Fact]
    public void Hierarchy_selection_is_owned_by_the_tree_presenter()
    {
        var presenter = new ProjectTreePresenter();
        var node = new EditorTreeNode("entity", "实体", "对象", "场景/实体", 1, "entity");

        Assert.True(presenter.SetSelectedHierarchyItem(node));
        Assert.Same(node, presenter.SelectedHierarchyItem);
        Assert.False(presenter.SetSelectedHierarchyItem(node));
        Assert.True(presenter.SetSelectedHierarchyItem(null));
        Assert.Null(presenter.SelectedHierarchyItem);
    }

    [Fact]
    public void UiVm_keeps_project_tree_notification_name_and_entity_selection()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.RunCommand.Execute("添加立方体");
        var root = vm.ProjectItems[0];
        var notifications = new List<string>();
        vm.PropertyChanged += (_, args) => notifications.Add(args.PropertyName!);

        vm.ToggleProjectNode(root);

        Assert.Equal(1, notifications.Count(name => name == nameof(vm.ProjectItems)));
        Assert.Single(vm.ProjectItems);
        vm.ToggleProjectNode(root);
        var entity = vm.ProjectItems.Single(item => item.IsEntity);
        vm.SelectedProjectItem = null;
        vm.SelectedProjectItem = entity;
        Assert.Equal(entity.Key, vm.SelectedProjectItem!.Key);
        Assert.Contains(nameof(vm.SelectedProjectItem), notifications);
    }
}
