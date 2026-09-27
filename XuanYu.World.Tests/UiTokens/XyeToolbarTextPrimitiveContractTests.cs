namespace XuanYu.World.Tests.UiTokens;

public sealed class XyeToolbarTextPrimitiveContractTests
{
    static string Read(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    [Fact]
    public void Single_line_toolbar_labels_use_xyui_text_primitives()
    {
        var files = new[]
        {
            "Top/EditToolsModule.axaml",
            "Top/FileModule.axaml",
            "Top/ViewModule.axaml",
            "Top/SnapModule.axaml",
            "Top/RuntimeStatusModule.axaml"
        };
        foreach (var file in files)
        {
            var source = Read(file);
            Assert.DoesNotContain("<TextBlock Text=", source);
        }
    }

    [Fact]
    public void Context_toolbar_uses_xyui4_hover_state_for_draw_action()
    {
        var source = Read("Top/ContextToolBar.axaml");
        Assert.Contains("<xy:XYHoverState", source);
        Assert.Contains("x:Name=\"DrawSplitButton\"", source);
        Assert.Contains("IsSelected=\"{Binding IsDrawingTransactionActive}\"", source);
    }
}
