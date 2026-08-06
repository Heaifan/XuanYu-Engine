using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5（二次纠偏，用户方案）：输入阶段真实校验（8 项）。
// 输入阶段：轻量规则（非法字符/NaN/Infinity/明显超界），错误不得消失；
// 失焦：完整单字段（格式+范围）；提交：跨字段；不清空输入；首错聚焦。
public sealed class UiD5InputValidationTests
{
    static UiVm NewVm() => new(null, () => true);

    [Fact]
    public void Invalid_after_invalid_keeps_error()
    {
        // 已有错误后继续输入另一个非法值，错误仍存在
        var vm = NewVm();
        vm.MapWidthText = "abc";
        Assert.NotEqual("", vm.MapWidthError);
        vm.MapWidthText = "abcd"; // 继续非法输入
        Assert.NotEqual("", vm.MapWidthError); // 错误不得消失
    }
    [Fact]
    public void Invalid_then_valid_clears_error_immediately()
    {
        // 非法值改为合法值，错误立即清除
        var vm = NewVm();
        vm.MapWidthText = "abc";
        Assert.NotEqual("", vm.MapWidthError);
        vm.MapWidthText = "600";
        Assert.Equal("", vm.MapWidthError);
    }
    [Fact]
    public void Input_stage_marks_only_the_typed_field()
    {
        // 输入阶段只标记被输入字段（宽度合法、深度非法，只标记深度）
        var vm = NewVm();
        vm.MapDepthText = "xyz";
        Assert.Equal("", vm.MapWidthError);
        Assert.NotEqual("", vm.MapDepthError);
        Assert.Equal("", vm.MapBaseHeightError);
    }
    [Fact]
    public void Input_stage_light_rules_do_not_touch_other_fields()
    {
        // 输入阶段只执行轻量规则：不触发其他字段、不做跨字段校验
        var vm = NewVm();
        vm.MapWidthText = "50"; // 输入阶段：明显超界（< 100）即报
        Assert.NotEqual("", vm.MapWidthError);
        Assert.Equal("", vm.MapDepthError); // 其他字段不受影响
        Assert.Equal("", vm.MapBaseHeightError);
    }
    [Fact]
    public void Input_stage_reports_obvious_out_of_range()
    {
        var vm = NewVm();
        vm.MapWidthText = "50";     // 50 < MinSizeMeters(100)
        Assert.Contains("必须位于 100～1000000 米之间", vm.MapWidthError);
        vm.MapWidthText = "2000000"; // > MaxSizeMeters(1000000)
        Assert.Contains("必须位于 100～1000000 米之间", vm.MapWidthError);
    }
    [Fact]
    public void Incomplete_input_keeps_existing_error_state()
    {
        // 输入中态（-、.、1. 等临时文本）：不清除已有错误，也不新置错
        var vm = NewVm();
        vm.MapWidthText = "abc";
        Assert.NotEqual("", vm.MapWidthError);
        vm.MapWidthText = "1."; // 输入中
        Assert.NotEqual("", vm.MapWidthError); // 已有错误保持
        vm.MapWidthText = "1e-"; // 输入中
        Assert.NotEqual("", vm.MapWidthError);
    }
    [Fact]
    public void Lost_focus_runs_full_field_validation()
    {
        // 失焦执行完整单字段规则（格式 + 范围）——直接调用失焦校验入口
        var vm = NewVm();
        vm.ValidateMapField("宽度", "50", out _); // 失焦校验：范围错误
        Assert.Contains("必须位于 100～1000000 米之间", vm.MapWidthError);
        vm.ValidateMapField("宽度", "600", out _);
        Assert.Equal("", vm.MapWidthError);
    }
    [Fact]
    public void Validation_failure_keeps_input_and_first_error_is_focused()
    {
        // 校验失败不清空输入；提交定位第一处错误（自动聚焦依据）
        var vm = NewVm();
        vm.MapWidthText = "100";
        vm.MapDepthText = "xyz";
        vm.RunCommand.Execute("应用地图属性");
        Assert.Equal("100", vm.MapWidthText);   // 输入保留
        Assert.Equal("xyz", vm.MapDepthText);   // 输入保留
        Assert.Equal("深度", vm.FirstInvalidField);
    }
}
