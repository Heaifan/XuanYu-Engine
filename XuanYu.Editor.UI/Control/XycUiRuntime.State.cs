using System.Text.Json;

namespace XuanYu.Editor.UI.XycControl;

internal sealed partial class XycUiRuntime
{
    async Task<XycUiState> WaitUntilCoreAsync(string state, JsonElement expected,
        int timeoutMs, CancellationToken cancellationToken)
    {
        if (timeoutMs is < 1 or > 30000) throw new JsonException("timeoutMs out of range");
        using var timeout = new CancellationTokenSource(timeoutMs);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(
            timeout.Token, cancellationToken);
        try
        {
            while (true)
            {
                linked.Token.ThrowIfCancellationRequested();
                var current = InspectState();
                if (Matches(current, state, expected)) return current;
                await Task.Delay(25, linked.Token);
            }
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested)
        { throw new TimeoutException("UI_STATE_TIMEOUT"); }
    }

    static bool Matches(XycUiState state, string key, JsonElement expected) => key switch
    {
        "activeTool" => state.ActiveTool == expected.GetString(),
        "isSelectTool" => state.IsSelectTool == expected.GetBoolean(),
        "isEditMode" => state.IsEditMode == expected.GetBoolean(),
        "isRegionEditMode" => state.IsRegionEditMode == expected.GetBoolean(),
        "cameraRevision" => state.CameraRevision == expected.GetInt64(),
        "footerMessage" => state.FooterMessage == expected.GetString(),
        _ => throw new JsonException("Unsupported UI state key.")
    };
}
