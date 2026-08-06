using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5-FINAL：地图状态四态（内存基线 ≠ 已保存到磁盘）。
// 无路径+无修改 → 未落盘；无路径+有修改 → 未保存；有路径+无修改 → 已保存；有路径+有修改 → 有未保存修改。
public sealed class UiD5MapStatusTests
{
    static UiVm NewVm() => new(null, () => true);

    [Fact]
    public void No_path_no_changes_shows_unsaved_to_disk()
    {
        var vm = NewVm();
        Assert.Equal("未落盘", vm.MapStatusText);
    }

    [Fact]
    public void No_path_form_changes_shows_unsaved()
    {
        var vm = NewVm();
        vm.MapWidthText = "12000"; // 仅表单修改（未应用）
        Assert.Equal("未保存", vm.MapStatusText);
    }

    [Fact]
    public void No_path_layer_changes_shows_unsaved()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层"); // 图层修改（IsDirty）
        Assert.Equal("未保存", vm.MapStatusText);
    }

    [Fact]
    public void With_path_no_changes_shows_saved()
    {
        var vm = NewVm();
        vm.MapSession.MarkSaved("C:/maps/a.xymap");
        Assert.Equal("已保存", vm.MapStatusText);
    }

    [Fact]
    public void With_path_form_changes_shows_modified()
    {
        var vm = NewVm();
        vm.MapSession.MarkSaved("C:/maps/a.xymap");
        vm.MapWidthText = "12000";
        Assert.Equal("有未保存修改", vm.MapStatusText);
    }

    [Fact]
    public void With_path_layer_changes_shows_modified()
    {
        var vm = NewVm();
        vm.MapSession.MarkSaved("C:/maps/a.xymap");
        vm.RunCommand.Execute("添加图层");
        Assert.Equal("有未保存修改", vm.MapStatusText);
    }

    [Fact]
    public void Undo_back_to_save_point_restores_status()
    {
        var vm = NewVm();
        vm.MapSession.MarkSaved("C:/maps/a.xymap");
        vm.MapWidthText = "12000";
        vm.MapDepthText = "12000";
        vm.RunCommand.Execute("应用地图属性");
        Assert.Equal("有未保存修改", vm.MapStatusText);
        vm.RunCommand.Execute("撤销地图修改");
        Assert.Equal("已保存", vm.MapStatusText); // Undo 回保存点
    }

    [Fact]
    public void MarkBaseline_does_not_change_file_path()
    {
        var vm = NewVm();
        vm.MapSession.MarkSaved("C:/maps/a.xymap");
        vm.MapSession.MarkBaseline(); // 内存基线不动路径
        Assert.Equal("C:/maps/a.xymap", vm.MapSession.CurrentFilePath);
        Assert.Equal("已保存", vm.MapStatusText);
    }
}
