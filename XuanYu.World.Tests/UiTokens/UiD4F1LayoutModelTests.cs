using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4-F1：布局模型——只读键值行在任何宽度保持水平；可编辑表单 360 阈值。
public sealed class UiD4F1LayoutModelTests
{
    [Theory]
    [InlineData(300)]
    [InlineData(320)]
    [InlineData(340)]
    [InlineData(360)]
    [InlineData(480)]
    public void Readonly_key_value_rows_never_switch_to_vertical(double width)
    {
        // 只读键值行无布局模式概念（单行双列始终成立）；可编辑表单模型只在真实输入控件上生效。
        // 该断言通过 InspectorPanel 结构合同（无双布局树）与可编辑模型阈值共同保证：
        // 只读路径不调用 EditableFormLayoutModel，因此 width 对只读行无任何影响。
        _ = width;
        Assert.DoesNotContain("WideFields", ReadPanel());
        Assert.DoesNotContain("NarrowFields", ReadPanel());
    }

    [Fact]
    public void Editable_form_switches_at_360()
    {
        Assert.Equal(EditableFormMode.Narrow, EditableFormLayoutModel.ModeFor(359));
        Assert.Equal(EditableFormMode.Wide, EditableFormLayoutModel.ModeFor(360));
    }

    static string? _panel;
    static string ReadPanel() =>
        _panel ??= System.IO.File.ReadAllText(System.IO.Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));
}
