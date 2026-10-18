using System.Text.Json;

namespace XuanYu.Editor.UI.XycControl;

internal static class XycUiCommandProcessor
{
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    static readonly HashSet<string> CaptureNames = new(StringComparer.Ordinal)
        { "Light-1360.png", "Dark-1360.png", "Light-1920.png", "Dark-1920.png" };
    static readonly HashSet<string> Commands = new(StringComparer.Ordinal)
        { "ui.window.inspect", "ui.window.resize", "ui.theme.inspect", "ui.theme.set",
            "ui.region.inspect", "ui.capture", "ui.control.list", "ui.control.inspect",
            "ui.control.invoke", "ui.state.inspect", "ui.waitUntil" };

    public static async Task<XycResponse> ExecuteAsync(XycRequest request,
        IXycUiRuntime? runtime, Func<bool, string?, object?, long?, long?, XycResponse> result,
        CancellationToken cancellationToken)
    {
        if (!Commands.Contains(request.Command))
            return result(false, "UNKNOWN_COMMAND", null, null, null);
        if (runtime is null) return result(false, "UI_HOST_REQUIRED", null, null, null);
        return request.Command switch
        {
            "ui.window.inspect" => result(true, null, runtime.InspectWindow(), null, null),
            "ui.window.resize" => Resize(),
            "ui.theme.inspect" => result(true, null, runtime.InspectTheme(), null, null),
            "ui.theme.set" => Theme(),
            "ui.region.inspect" => result(true, null, runtime.InspectTop(), null, null),
            "ui.capture" => Capture(),
            "ui.control.list" => result(true, null, runtime.ListControls(), null, null),
            "ui.control.inspect" => InspectControl(),
            "ui.control.invoke" => await InvokeControl(),
            "ui.state.inspect" => result(true, null, runtime.InspectState(), null, null),
            "ui.waitUntil" => await WaitUntil(),
            _ => result(false, "UNKNOWN_COMMAND", null, null, null)
        };

        XycResponse Resize()
        {
            var size = request.Args.Deserialize<XycUiResizeRequest>(Json)
                ?? throw new JsonException();
            if (!double.IsFinite(size.WidthDip) || !double.IsFinite(size.HeightDip) ||
                size.WidthDip is < 1024 or > 4096 || size.HeightDip is < 640 or > 2160)
                throw new JsonException("窗口尺寸超出支持范围。");
            return result(true, null, runtime.ResizeWindow(size.WidthDip, size.HeightDip), null, null);
        }

        XycResponse Theme()
        {
            var requestTheme = request.Args.Deserialize<XycUiThemeRequest>(Json)
                ?? throw new JsonException();
            if (requestTheme.Theme is not ("Light" or "Dark")) throw new JsonException();
            return result(true, null, runtime.SetTheme(requestTheme.Theme), null, null);
        }

        XycResponse Capture()
        {
            var capture = request.Args.Deserialize<XycUiCaptureRequest>(Json)
                ?? throw new JsonException();
            if (!CaptureNames.Contains(capture.FileName)) throw new JsonException();
            return result(true, null, runtime.CaptureTop(capture.FileName,
                capture.TemporaryTopmost), null, null);
        }

        XycResponse InspectControl()
        {
            var control = request.Args.Deserialize<XycUiControlRequest>(Json)
                ?? throw new JsonException();
            return result(true, null, runtime.InspectControl(control.ControlId), null, null);
        }

        async Task<XycResponse> InvokeControl()
        {
            var control = request.Args.Deserialize<XycUiInvokeRequest>(Json)
                ?? throw new JsonException();
            var state = await runtime.InvokeControlAsync(control.ControlId,
                control.Operation, cancellationToken);
            return result(true, null, state, null, null);
        }

        async Task<XycResponse> WaitUntil()
        {
            var wait = request.Args.Deserialize<XycUiWaitRequest>(Json)
                ?? throw new JsonException();
            var state = await runtime.WaitUntilAsync(wait.State, wait.Expected,
                wait.TimeoutMs, cancellationToken);
            return result(true, null, state, null, null);
        }
    }
}

internal sealed record XycUiResizeRequest(double WidthDip, double HeightDip);
internal sealed record XycUiThemeRequest(string Theme);
internal sealed record XycUiCaptureRequest(string FileName, bool TemporaryTopmost = false);
internal sealed record XycUiControlRequest(string ControlId);
internal sealed record XycUiInvokeRequest(string ControlId, string Operation);
internal sealed record XycUiWaitRequest(string State, JsonElement Expected, int TimeoutMs);
