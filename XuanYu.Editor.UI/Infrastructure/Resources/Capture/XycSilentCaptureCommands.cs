using System.Text.Json;
using XuanYu.Editor.UI.XycControl;

namespace XuanYu.Editor.UI;

internal static class XycSilentCaptureCommands
{
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static XycResponse Arm(XycRequest request, IXycRuntime runtime, bool diagnosticWrite,
        Func<bool, string?, object?, long?, long?, XycResponse> result)
    {
        if (!diagnosticWrite) return result(false, "UNAUTHORIZED", null, null, null);
        var args = request.Args.Deserialize<XycCaptureRequest>(Json) ??
            throw new JsonException("capture camera revision missing");
        if (args.CameraRevision <= 0) throw new JsonException("invalid camera revision");
        var armed = runtime.ArmPresentedCapture(args.CameraRevision);
        return result(armed, null,
            new XycCaptureState(true, runtime.InspectSilentHost()), null, args.CameraRevision);
    }

    public static XycResponse Release(XycRequest request, IXycRuntime runtime,
        bool diagnosticWrite, Func<bool, string?, object?, long?, long?, XycResponse> result)
    {
        _ = request;
        if (!diagnosticWrite) return result(false, "UNAUTHORIZED", null, null, null);
        var released = runtime.ReleasePresentedCapture();
        return result(released, null, new XycCaptureState(false,
            runtime.InspectSilentHost()), null, null);
    }
}
