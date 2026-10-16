using System.Text.Json;

namespace XuanYu.Editor.UI.XycControl;

internal static class XycDollyCommand
{
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static XycResponse Execute(XycRequest request, IXycRuntime runtime,
        bool diagnosticWrite,
        Func<bool, string?, object?, long?, long?, XycResponse> result)
    {
        if (!diagnosticWrite) return result(false, "UNAUTHORIZED", null, null, null);
        var delta = request.Args.Deserialize<XycDollyDelta>(Json) ??
            throw new JsonException("dolly delta missing");
        if (!double.IsFinite(delta.WheelDelta) || Math.Abs(delta.WheelDelta) > 5000)
            throw new JsonException("dolly delta outside the supported range");
        var dolly = runtime.DollyCamera(delta.WheelDelta);
        return result(dolly.Handled, dolly.Handled ? null : "DOLLY_NOT_HANDLED",
            new { before = dolly.Before, after = dolly.After, constrained = dolly.Constrained },
            runtime.InspectScene().SceneRevision, dolly.After.Revision);
    }
}
