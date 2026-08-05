using System.IO;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-A-R2-D4-F3 预验收补丁：状态图标真实消费（A）、插入线通知（B）、整行插入线（C）、清理（D）。
public sealed class UiLayerStateFeedbackTests
{
    static readonly string Panel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "LayerPanel.axaml"));

    static UiVm NewVm() => new(null, () => true);

    [Fact]
    public void A_icons_consume_hidden_and_unlocked_shapes()
    {
        Assert.Contains("{StaticResource HiddenIcon}", Panel);
        Assert.Contains("{Binding IsHidden}", Panel);
        Assert.Contains("{StaticResource UnlockedIcon}", Panel);
        Assert.Contains("{Binding IsUnlocked}", Panel);
        Assert.Contains("{StaticResource VisibleIcon}", Panel);
        Assert.Contains("{StaticResource LockedIcon}", Panel);
    }

    [Fact]
    public void B_drop_target_raises_property_changed()
    {
        var vm = NewVm();
        var row = vm.LayerItems[0];
        var raised = new List<string?>();
        row.PropertyChanged += (_, e) => raised.Add(e.PropertyName);
        vm.SetDropTarget(0);
        Assert.Contains("IsDropBefore", raised);
        Assert.True(row.IsDropBefore);
    }

    [Fact]
    public void C_drop_line_spans_full_row()
    {
        Assert.Contains("Grid.ColumnSpan=\"6\"", Panel);
        Assert.Contains("ZIndex=\"10\"", Panel);
    }

    [Fact]
    public void D_clearing_target_hides_all_drop_lines()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        vm.SetDropTarget(0);
        Assert.True(vm.LayerItems[0].IsDropBefore);
        vm.SetDropTarget(null);
        Assert.All(vm.LayerItems, r => Assert.False(r.IsDropBefore));
    }

    [Fact]
    public void E_same_position_drag_is_silent_noop()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        var stateId = vm.MapSession.CurrentStateId;
        var logCount = vm.LogItems.Count;
        var footer = vm.FooterMessage;
        var canUndo = vm.MapSession.CanUndo;
        var id = vm.LayerItems[0].LayerId.Value;
        vm.CommitLayerDrag(id, 0); // 区域 2 已在最上方（位置 0）
        Assert.Equal(stateId, vm.MapSession.CurrentStateId);
        Assert.Equal(logCount, vm.LogItems.Count);
        Assert.Equal(footer, vm.FooterMessage);
        Assert.Equal(canUndo, vm.MapSession.CanUndo); // No-op 不新增历史
    }
}
