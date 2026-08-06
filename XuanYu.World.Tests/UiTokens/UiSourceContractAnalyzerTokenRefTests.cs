namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D3：分析器对 {StaticResource} 正式 Token 引用豁免（Setter 与内联属性）。
// 未登记的字面量仍然报告（见 UiSourceContractAnalyzerTests.Unregistered_values_are_reported）。
public sealed class UiSourceContractAnalyzerTokenRefTests
{
    [Fact]
    public void Static_resource_value_references_pass()
    {
        const string text = """
            <Style Selector="Button.tabNavBtn">
              <Setter Property="MinHeight" Value="{StaticResource Control.Height.Standard}"/>
              <Setter Property="CornerRadius" Value="{StaticResource Radius.Standard}"/>
            </Style>
            <TextBlock FontSize="{StaticResource Font.Body.Size}"/>
            """;
        Assert.Empty(UiSourceContractAnalyzer.AnalyzeAxaml(text, "Xaml"));
    }

    [Fact]
    public void Unregistered_literal_after_exemption_still_fails()
    {
        const string text = """
            <Style Selector="Button.x">
              <Setter Property="FontSize" Value="15"/>
            </Style>
            """;
        Assert.Contains(UiSourceContractAnalyzer.AnalyzeAxaml(text, "X"),
            v => v.Kind == UiRuleKind.FontSize && v.Value == "15");
    }
}
