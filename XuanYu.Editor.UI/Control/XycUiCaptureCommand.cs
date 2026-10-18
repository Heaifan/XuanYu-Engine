using System.Text.Json;
using System.Text.RegularExpressions;

namespace XuanYu.Editor.UI.XycControl;

internal static class XycUiCaptureCommand
{
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    static readonly Regex CaptureName = new(
        "^(Light|Dark)-(1360|1920)(-[A-Za-z0-9_-]{1,32})?\\.png$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static XycResponse Execute(XycRequest request, IXycUiRuntime runtime,
        Func<bool, string?, object?, long?, long?, XycResponse> result,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var capture = request.Args.Deserialize<XycUiCaptureRequest>(Json)
            ?? throw new JsonException();
        if (!CaptureName.IsMatch(capture.FileName)) throw new JsonException();
        if (capture.Backend is not ("avaloniaWindow" or "gdiCompatibility"))
            throw new JsonException();
        if (capture.TemporaryTopmost && capture.Backend != "gdiCompatibility")
            throw new JsonException();
        if (capture.TimeoutMs is < 1 or > 30000) throw new JsonException();
        using var timeout = new CancellationTokenSource(capture.TimeoutMs);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken, timeout.Token);
        try
        {
            return result(true, null, runtime.CaptureTop(capture.FileName,
                capture.Backend, capture.TemporaryTopmost, linked.Token), null, null);
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested &&
            !cancellationToken.IsCancellationRequested) { throw new TimeoutException(); }
    }
}

internal sealed record XycUiCaptureRequest(string FileName,
    string Backend = "avaloniaWindow", bool TemporaryTopmost = false,
    int TimeoutMs = 5000);
