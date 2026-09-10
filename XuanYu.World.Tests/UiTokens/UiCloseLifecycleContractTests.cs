using System;
using System.IO;

namespace XuanYu.World.Tests.UiTokens;

public sealed class UiCloseLifecycleContractTests
{
    static string Read(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    static string ReadApp(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    [Fact]
    public void Close_confirmation_is_deferred_until_window_close_event_returns()
    {
        var code = Read("Win/UiWin.CloseLifecycle.cs");
        Assert.DoesNotContain("protected override async void OnClosing", code);
        Assert.Contains("e.Cancel = true;", code);
        Assert.Contains("if (_closePromptActive)", code);
        Assert.Contains("Dispatcher.UIThread.Post(() =>", code);
        Assert.Contains("_ = ConfirmCloseAsync(vm);", code);
        Assert.Contains("_allowClosing = true;", code);
        Assert.Contains("finally", code);
    }

    [Fact]
    public void Dialog_card_is_above_input_blocking_overlay()
    {
        var axaml = Read("Win/UiWin.axaml");
        Assert.Contains("x:Name=\"DialogOverlay\" ZIndex=\"90\"", axaml);
        Assert.Contains("x:Name=\"DialogCard\" ZIndex=\"100\"", axaml);
    }

    [Fact]
    public void Editor_lifetime_exits_when_main_window_closes()
    {
        var app = ReadApp("Bootstrap/App.axaml.cs");
        Assert.Contains("desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;", app);
    }
}
