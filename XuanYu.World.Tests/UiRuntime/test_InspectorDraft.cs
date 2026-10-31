using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class test_InspectorDraft
{
    [Fact]
    public void Draft_text_changes_notify_direct_binding_paths()
    {
        var draft = new InspectorDraft();
        var notifications = new List<string>();
        var observable = Assert.IsAssignableFrom<System.ComponentModel.INotifyPropertyChanged>(draft);
        observable.PropertyChanged += (_, args) => notifications.Add(args.PropertyName!);

        draft.SetEntityNameText("实体草稿");
        draft.SetLayerNameText("图层草稿");

        Assert.Contains(nameof(draft.EntityNameText), notifications);
        Assert.Contains(nameof(draft.LayerNameText), notifications);
    }

    [Fact]
    public void Entity_name_draft_captures_target_and_cancel_restores_start_text()
    {
        var target = new InspectorEditTarget(InspectorObjectKind.Entity, "entity:7", "Entity.Basic.Name");
        var draft = new InspectorDraft();
        draft.BeginEntityNameEdit(target, "旧名称");
        draft.SetEntityNameText("未提交名称");

        Assert.True(draft.CancelEntityNameEdit());

        Assert.Equal("旧名称", draft.EntityNameText);
        Assert.Null(draft.EntityNameTarget);
        Assert.False(draft.CancelEntityNameEdit());
    }

    [Fact]
    public void Transform_input_is_parsed_and_validated_before_commit()
    {
        var current = CommittedTransform.Identity;

        Assert.True(InspectorDraft.TryParseNumber("2.5", out var value));
        Assert.True(InspectorDraft.TryBuildTransform(current, "位置", "x", value, out var next, out _));
        Assert.Equal(2.5, next.Position.X);
        Assert.False(InspectorDraft.TryParseNumber("NaN", out _));
        Assert.False(InspectorDraft.TryBuildTransform(current, "缩放", "X", 0, out _, out var error));
        Assert.Contains("缩放", error);
    }

    [Fact]
    public void Entity_name_cancel_restores_binding_without_committing_model()
    {
        var vm = new UiVm(null, () => true);
        var entity = vm.HierarchyItems.First(item => item.IsEntity);
        vm.SelectedHierarchyItem = entity;
        var original = vm.InspectorEntityNameText;
        vm.BeginInspectorEntityNameEdit();
        vm.InspectorEntityNameText = "临时草稿";

        vm.CancelInspectorEntityNameEdit();

        Assert.Equal(original, vm.InspectorEntityNameText);
        Assert.Equal(original, vm.HierarchyItems.Single(item => item.Key == entity.Key).Title);
        Assert.False(vm.CommitInspectorEntityName());
        Assert.Equal(original, vm.HierarchyItems.Single(item => item.Key == entity.Key).Title);
    }
}
