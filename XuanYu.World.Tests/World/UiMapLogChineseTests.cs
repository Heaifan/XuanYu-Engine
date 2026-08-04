using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.World;

// MAP-A-R2-D3-F2：日志中文化——字段名/状态值中文，内部枚举保持英文。
public sealed class UiMapLogChineseTests
{
    static UiVm NewVm() => new(null, () => true);
    static string Apply(UiVm vm, string width)
    {
        vm.MapWidthText = width; vm.MapDepthText = "600"; vm.MapBaseHeightText = "0";
        vm.RunCommand.Execute("应用地图属性");
        return string.Join("\n", vm.LogItems.Select(e => e.Message));
    }

    [Fact]
    public void Apply_success_log_uses_chinese_field_names()
    {
        var text = Apply(NewVm(), "600");

        Assert.Contains("命令=应用地图属性", text);
        Assert.Contains("宽度输入=600", text);
        Assert.Contains("深度输入=600", text);
        Assert.Contains("基础高度输入=0", text);
        Assert.Contains("候选尺寸=600×600", text);
        Assert.Contains("新尺寸=600×600", text);
        Assert.Contains("可撤销=是", text);
        Assert.Contains("可重做=否", text);
        Assert.Contains("原因=地图属性修改", text);
        Assert.Contains("地表=平面", text);
    }
    [Fact]
    public void Apply_success_log_has_no_english_keys()
    {
        var text = Apply(NewVm(), "600");
        var forbidden = new[] { "Command=", "WidthText=", "DepthText=", "BaseHeightText=",
            "MapId=", "Before=", "Candidate=", "StateId=", "ChangeSequence=", "Reason=",
            "Sequence=", "Size=", "BaseHeight=", "Surface=", "After=", "CanUndo=", "CanRedo=",
            "Decision=", "KeyChanged=", "Code=", "StateUnchanged=", "MapPropertiesChanged", "Flat" };

        foreach (var token in forbidden)
            Assert.DoesNotContain(token, text);
    }
    [Fact]
    public void Invalid_size_log_uses_chinese_error_and_unchanged()
    {
        var text = Apply(NewVm(), "50");

        Assert.Contains("错误类型=地图尺寸无效", text);
        Assert.Contains("状态保持不变=是", text);
        Assert.Contains("当前尺寸=10000×10000", text);
    }
    [Fact]
    public void Undo_log_uses_chinese_reason_and_booleans()
    {
        var vm = NewVm();
        vm.MapWidthText = "600"; vm.MapDepthText = "600"; vm.MapBaseHeightText = "0";
        vm.RunCommand.Execute("应用地图属性");
        vm.RunCommand.Execute("撤销地图修改");
        var text = string.Join("\n", vm.LogItems.Select(e => e.Message));

        Assert.Contains("原因=撤销", text);
        Assert.Contains("地图撤销成功", text);
        Assert.Contains("恢复前=600×600", text);
        Assert.Contains("恢复后=10000×10000", text);
        Assert.Contains("可撤销=否", text);
        Assert.Contains("可重做=是", text);
    }
    [Fact]
    public void Display_mappings_cover_key_values()
    {
        Assert.Equal("地图属性修改", UiVm.FormatMapEditReason(MapEditReason.MapPropertiesChanged));
        Assert.Equal("撤销", UiVm.FormatMapEditReason(MapEditReason.Undo));
        Assert.Equal("重做", UiVm.FormatMapEditReason(MapEditReason.Redo));
        Assert.Equal("新建地图", UiVm.FormatMapEditReason(MapEditReason.NewMap));
        Assert.Equal("替换地图", UiVm.FormatMapEditReason(MapEditReason.Replace));
        Assert.Equal("平面", UiVm.FormatSurfaceKind("Flat"));
        Assert.Equal("缓丘", UiVm.FormatSurfaceKind("GentleHillsV1"));
        Assert.Equal("地图尺寸无效", UiVm.FormatErrorCode("InvalidMapSize"));
        Assert.Equal("区域越界", UiVm.FormatErrorCode("RegionWouldBeOutOfBounds"));
        Assert.Equal("是", UiVm.FormatBoolean(true));
        Assert.Equal("否", UiVm.FormatBoolean(false));
    }
}
