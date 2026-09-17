namespace XuanYu.World.Tests.UiTokens;

public sealed class TopWorkspaceSelectorR1Tests
{
    static string ReadSelector() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..",
        "XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml"));

    [Fact]
    public void Selector_uses_native_workspace_switcher()
    {
        var selector = ReadSelector() + File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml.cs"));
        Assert.Contains("XYWorkspaceSwitcher", selector);
        Assert.Contains("管理模式", selector);
        Assert.DoesNotContain("XYMenuBarItem", selector);
        Assert.DoesNotContain("Shortcut=\"暂未开放\"", selector);
    }

    [Fact]
    public void Unavailable_workspace_options_are_disabled_without_commands()
    {
        var selector = ReadSelector() + File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Workspace", "WorkspaceSelector.axaml.cs"));
        Assert.Contains("场景编辑（暂未开放）", selector);
        Assert.Contains("调试（暂未开放）", selector);
        Assert.Contains("false));", selector);
        Assert.DoesNotContain("CommandParameter=\"Scene", selector);
        Assert.DoesNotContain("CommandParameter=\"Debug", selector);
    }

    static int Count(string text, string value) =>
        text.Split(value, StringSplitOptions.None).Length - 1;
}
