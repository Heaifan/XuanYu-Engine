namespace XuanYu.World.Tests.UiTokens;

public sealed class UiDebtBaselineWin32Tests
{
    [Theory]
    [InlineData("XuanYu.Editor.UI/Infrastructure/Controls/Capture/XycUiNativeCapture.cs", "XycUiNativeCapture.if", "0x00CC0020")]
    [InlineData("XuanYu.Editor.UI/Infrastructure/Resources/Host/SilentNativeWindow.cs", "SilentNativeWindow.WsPopup", "0x80000000")]
    [InlineData("XuanYu.Editor.UI/Infrastructure/Resources/Host/SilentNativeWindow.cs", "SilentNativeWindow.WsVisible", "0x10000000")]
    public void Win32_constants_are_not_classified_as_ui_colors(string path, string locator, string value)
    {
        var source = File.ReadAllText(Path.Combine(TestSourceRoot.Root, path));
        Assert.DoesNotContain(UiSourceContractAnalyzer.AnalyzeCs(source, path),
            x => x.Locator == locator && x.Kind == UiRuleKind.CsHexColor && x.Property == "Uint" && x.Value == value);
        var nearbyValue = value switch
        {
            "0x00CC0020" => "0x00CC0021",
            "0x80000000" => "0x80000001",
            _ => "0x10000001",
        };
        Assert.Contains(UiSourceContractAnalyzer.AnalyzeCs($"class NativeFlags {{ const uint Flag = {nearbyValue}; }}", path),
            x => x.Kind == UiRuleKind.CsHexColor && x.Property == "Uint");
    }
}
