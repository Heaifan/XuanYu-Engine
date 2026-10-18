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
        var processor = new XycCommandProcessor(new Runtime(), false);
        Assert.Equal("UI_HOST_REQUIRED", processor.Execute(Request("ui.window.inspect")).ErrorCode);
    }

    [Fact]
    public void UI_host_routes_inspection_theme_and_capture_commands()
    {
        var ui = new UiRuntime();
        var processor = new XycCommandProcessor(new Runtime(), false, ui);
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
        var processor = new XycCommandProcessor(new Runtime(), false, new UiRuntime());
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

    sealed class Runtime : IXycRuntime
    {
        public string InstanceId => "test-instance";
        public XycRuntimeInfo InspectRuntime() => throw new NotImplementedException();
        public XycScene InspectScene() => throw new NotImplementedException();
        public XycCamera InspectCamera() => throw new NotImplementedException();
        public XycCamera SetTestPose(XycCamera pose) => throw new NotImplementedException();
        public XycCamera OrbitCamera(double x, double y) => throw new NotImplementedException();
        public XycDollyResult DollyCamera(double delta) => throw new NotImplementedException();
        public bool ArmPresentedCapture(long revision) => throw new NotImplementedException();
        public bool ReleasePresentedCapture() => throw new NotImplementedException();
    }

    sealed class UiRuntime : IXycUiRuntime
    {
        public int Captures { get; private set; }
        public XycUiWindowState InspectWindow() => new(1, "0x1", true, false, "Normal", 1360, 820, 1360, 820, 96, 1);
        public XycUiWindowState ResizeWindow(double w, double h) => InspectWindow();
        public XycUiThemeState InspectTheme() => new("Dark", "Dark");
        public XycUiThemeState SetTheme(string theme) => new(theme, theme);
        public XycUiRegionState InspectTop() => new("TOP", 0, 0, 1360, 69, 0, 0, 1360, 69);
        public XycUiCaptureState CaptureTop(string name)
        {
            Captures++;
            return new(name, Path.ChangeExtension(name, ".json"), "sha256", 1360, 69,
                InspectTop(), InspectWindow(), InspectTheme(), "build", "test", DateTimeOffset.UtcNow);
        }
    }
}
