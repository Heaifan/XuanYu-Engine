using System.Reflection;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycSilentWindowDecisionTests
{
    [Fact]
    public void Ignores_window_events_owned_by_other_processes()
    {
        Assert.Equal("None", Evaluate(targetPid: 41, eventPid: 42, isShow: true,
            isTopLevel: true, isVisible: true, isForegroundOwner: false).ToString());
    }

    [Fact]
    public void Fails_closed_when_target_shows_a_visible_top_level_window()
    {
        Assert.Equal("VisibleWindow", Evaluate(targetPid: 41, eventPid: 41, isShow: true,
            isTopLevel: true, isVisible: true, isForegroundOwner: false).ToString());
    }

    [Fact]
    public void Ignores_hidden_and_child_window_show_events()
    {
        Assert.Equal("None", Evaluate(targetPid: 41, eventPid: 41, isShow: true,
            isTopLevel: false, isVisible: true, isForegroundOwner: false).ToString());
        Assert.Equal("None", Evaluate(targetPid: 41, eventPid: 41, isShow: true,
            isTopLevel: true, isVisible: false, isForegroundOwner: false).ToString());
    }

    [Fact]
    public void Fails_closed_when_target_owns_foreground_focus()
    {
        Assert.Equal("ForegroundFocus", Evaluate(targetPid: 41, eventPid: 0, isShow: false,
            isTopLevel: false, isVisible: false, isForegroundOwner: true).ToString());
    }

    static object Evaluate(int targetPid, int eventPid, bool isShow, bool isTopLevel,
        bool isVisible, bool isForegroundOwner)
    {
        var cliAssembly = TestSourceRoot.ProjectOutput("XuanYu.Editor.Control.Cli", "XuanYu.Editor.Control.Cli.dll");
        Assert.True(File.Exists(cliAssembly), $"Missing CLI assembly: {cliAssembly}");
        var assembly = Assembly.LoadFrom(cliAssembly);
        var type = assembly.GetType("SilentProcessDecision", throwOnError: true)!;
        var method = type.GetMethod("Evaluate", BindingFlags.Public | BindingFlags.Static)!;
        return method.Invoke(null, [targetPid, eventPid, isShow, isTopLevel, isVisible,
            isForegroundOwner])!;
    }
}
