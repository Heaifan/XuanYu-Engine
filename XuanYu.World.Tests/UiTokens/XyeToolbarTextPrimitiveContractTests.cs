namespace XuanYu.World.Tests.UiTokens;

public sealed class XyeToolbarTextPrimitiveContractTests
{
    static string Read(string rel) => File.ReadAllText(Path.Combine(TestSourceRoot.Root,  "XuanYu.Editor.UI", rel));

    [Fact]
    public void Single_line_toolbar_labels_use_xyui_text_primitives()
    {
        var files = new[]
        {
            "Panels/Tools/ContextTools/EditToolsModule.axaml",
            "Panels/Tools/WorkspaceModules/FileModule.axaml",
            "Panels/Tools/ViewTools/ViewModule.axaml",
            "Panels/Tools/ContextTools/SnapModule.axaml",
            "Panels/Tools/WorkspaceModules/RuntimeStatusModule.axaml"
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
        var source = Read("Panels/Tools/ContextTools/ContextToolBar.axaml");
        Assert.Contains("<xy:XYHoverState", source);
        Assert.Contains("x:Name=\"DrawSplitButton\"", source);
        Assert.Contains("IsSelected=\"{Binding IsDrawingTransactionActive}\"", source);
    }
}
