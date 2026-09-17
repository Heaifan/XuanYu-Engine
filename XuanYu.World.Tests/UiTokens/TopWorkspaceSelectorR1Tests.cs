namespace XuanYu.World.Tests.UiTokens;

public sealed class TopWorkspaceSelectorR1Tests
{
    static string ReadSelector() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..",
        "XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml"));

    [Fact]
    public void Selector_has_one_current_workspace_menu_and_three_options()
    {
        var selector = ReadSelector();
        Assert.Equal(1, Count(selector, "<xy:XYMenuBarItem"));
        Assert.DoesNotContain("<xy:XYButton", selector);
        Assert.Contains("Label=\"{Binding CurrentEditorModeText}\"", selector);
        Assert.Contains("Label=\"要素编辑\"", selector);
        Assert.Contains("Label=\"场景编辑\"", selector);
        Assert.Contains("Label=\"调试\"", selector);
        Assert.Contains("暂未开放", selector);
    }

    [Fact]
    public void Unavailable_workspace_options_are_disabled_without_commands()
    {
        var selector = ReadSelector();
        Assert.Contains("Label=\"场景编辑\"", selector);
        Assert.Contains("Label=\"调试\"", selector);
        Assert.Equal(2, Count(selector, "IsEnabled=\"False\""));
        Assert.DoesNotContain("CommandParameter=\"Scene", selector);
        Assert.DoesNotContain("CommandParameter=\"Debug", selector);
    }

    static int Count(string text, string value) =>
        text.Split(value, StringSplitOptions.None).Length - 1;
}
