using System.IO;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5：表单状态合同——输入框完整状态、错误非仅颜色（图标+文字+边框）。
public sealed class UiD5FormContractTests
{
    static readonly string D5 = Read("Design/UiStyles.D5.axaml");
    static readonly string Form = Read("Right/MapFormPanel.axaml");

    static string Read(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    [Fact]
    public void Text_box_has_full_state_set()
    {
        Assert.Contains("<Style Selector=\"TextBox\">", D5);
        Assert.Contains("<Style Selector=\"TextBox:pointerover\">", D5);
        Assert.Contains("<Style Selector=\"TextBox:focus\">", D5);
        Assert.Contains("<Style Selector=\"TextBox:disabled\">", D5);
    }

    [Fact]
    public void Text_box_error_and_warning_states_exist()
    {
        Assert.Contains("<Style Selector=\"TextBox.error\">", D5);
        Assert.Contains("<Style Selector=\"TextBox.warning\">", D5);
        Assert.Contains("Color.Error", D5);
        Assert.Contains("Color.Warning", D5);
    }

    [Fact]
    public void Map_form_inputs_bind_field_level_errors()
    {
        // D5 纠偏：每个输入框只绑定自身字段错误（宽/窄两套布局各 3 处）
        Assert.Equal(2, CountOccurrences(Form, "Classes.error=\"{Binding MapWidthError}\""));
        Assert.Equal(2, CountOccurrences(Form, "Classes.error=\"{Binding MapDepthError}\""));
        Assert.Equal(2, CountOccurrences(Form, "Classes.error=\"{Binding MapBaseHeightError}\""));
        Assert.DoesNotContain("Classes.error=\"{Binding IsMapFormError}\"", Form); // 输入框不再统一全局染红
    }

    [Fact]
    public void Form_error_feedback_is_not_color_only()
    {
        // 错误反馈 = 图标 + 文字 + 输入框错误边框（三重表达）
        Assert.Contains("ErrorIcon", Form);
        Assert.Contains("IsMapFormError", Form);
        Assert.Contains("Color.Error", Form);
    }

    [Fact]
    public void Form_error_feedback_hidden_when_no_error()
    {
        Assert.Contains("IsVisible=\"{Binding IsMapFormError}\"", Form);
    }

    static int CountOccurrences(string text, string needle) =>
        System.Text.RegularExpressions.Regex.Matches(text, System.Text.RegularExpressions.Regex.Escape(needle)).Count;
}
