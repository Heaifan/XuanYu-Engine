using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.World.Tests.Xyc;

public sealed class XycUiCommandProcessorTests
{
    [Fact]
    public void UI_commands_require_explicit_UI_host()
    {
        var processor = new XycCommandProcessor(new XycUiTestRuntime(), false);
        Assert.Equal("UI_HOST_REQUIRED", processor.Execute(Request("ui.window.inspect")).ErrorCode);
    }

    [Fact]
    public void UI_host_routes_inspection_theme_and_capture_commands()
    {
        var ui = new XycUiTestRuntime();
        var processor = new XycCommandProcessor(ui, false, ui);
        Assert.True(processor.Execute(Request("ui.window.inspect")).Success);
        Assert.Equal("Dark", processor.Execute(Request("ui.theme.set", new { theme = "Dark" }))
            .ObservedState is XycUiThemeState theme ? theme.Actual : null);
        Assert.True(processor.Execute(Request("ui.region.inspect")).Success);
        Assert.True(processor.Execute(Request("ui.capture", new { fileName = "Dark-1360.png" })).Success);
        Assert.Equal(1, ui.Captures);
    }

    [Fact]
    public void Invalid_size_theme_and_capture_path_are_rejected()
    {
        var ui = new XycUiTestRuntime();
        var processor = new XycCommandProcessor(ui, false, ui);
        Assert.Equal("INVALID_INPUT", processor.Execute(Request("ui.window.resize",
            new { widthDip = 600, heightDip = 820 })).ErrorCode);
        Assert.Equal("INVALID_INPUT", processor.Execute(Request("ui.theme.set",
            new { theme = "System" })).ErrorCode);
        Assert.Equal("INVALID_INPUT", processor.Execute(Request("ui.capture",
            new { fileName = "../../capture.png" })).ErrorCode);
    }

    [Fact]
    public void Theme_setting_updates_production_scope_without_TOP_specific_controls()
    {
        var scope = new ThemeVariantScope();
        XycUiRuntime.SetRequestedTheme(scope, "Dark");
        Assert.Equal(ThemeVariant.Dark, scope.RequestedThemeVariant);
        XycUiRuntime.SetRequestedTheme(scope, "Light");
        Assert.Equal(ThemeVariant.Light, scope.RequestedThemeVariant);
    }

    [Fact]
    public void Theme_setting_updates_application_variant_when_TOP_scope_is_absent()
    {
        var app = new Application();
        XycUiRuntime.SetApplicationTheme(app, "Dark");
        Assert.Equal(ThemeVariant.Dark, app.RequestedThemeVariant);
    }

    static XycRequest Request(string command, object? args = null) => new("XYC-API/1",
        Guid.NewGuid().ToString("N"), "test-instance", command,
        JsonSerializer.SerializeToElement(args ?? new { }));

}
