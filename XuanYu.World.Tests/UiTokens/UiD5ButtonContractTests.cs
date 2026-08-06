using System.IO;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5：按钮治理合同——D5-FIX-01 内容居中、完整状态（Normal/Hover/Pressed/
// Focused/Disabled）、危险按钮、基线按钮色清零。
public sealed class UiD5ButtonContractTests
{
    static readonly string D5 = Read("Design/UiStyles.D5.axaml");
    static readonly string Ui = Read("Ui.axaml");

    static string Read(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    [Fact]
    public void Button_content_is_centered_horizontally_and_vertically()
    {
        // D5-FIX-01：统一水平/垂直居中（禁止逐按钮 Margin 偏移修补）
        Assert.Contains("HorizontalContentAlignment\" Value=\"Center\"", D5);
        Assert.Contains("VerticalContentAlignment\" Value=\"Center\"", D5);
    }

    [Fact]
    public void Button_has_full_state_set()
    {
        Assert.Contains("<Style Selector=\"Button:pointerover\">", D5);
        Assert.Contains("<Style Selector=\"Button:pressed\">", D5);
        Assert.Contains("<Style Selector=\"Button:focus-visible\">", D5);
        Assert.Contains("<Style Selector=\"Button:disabled\">", D5);
    }

    [Fact]
    public void Button_states_use_formal_tokens_and_keep_size_stable()
    {
        Assert.Contains("Color.Hover.Bg", D5);
        Assert.Contains("Color.Focus", D5);
        Assert.Contains("Color.Text.Disabled", D5);
        // 状态切换不跳动：所有状态共用同一 BorderThickness=1（无状态内改边框粗细）
        Assert.DoesNotContain("BorderThickness\" Value=\"2\"", D5);
    }

    [Fact]
    public void Danger_button_uses_danger_token()
    {
        Assert.Contains("<Style Selector=\"Button.uiDanger\">", D5);
        Assert.Contains("Color.Danger", D5);
    }

    [Fact]
    public void Global_button_colors_migrated_to_tokens()
    {
        Assert.Contains("Color.Bg.Panel", Ui);
        Assert.Contains("Color.Border.Default", Ui);
        Assert.Contains("Color.Text.Primary", Ui);
        Assert.DoesNotContain("Background\" Value=\"#eef3fa\"", Ui);  // 旧按钮底色已迁移
        Assert.DoesNotContain("BorderBrush\" Value=\"#d4ddea\"", Ui);
        Assert.DoesNotContain("Foreground\" Value=\"#26324a\"", Ui);
    }
}
