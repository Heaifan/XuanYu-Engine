using XuanYu.Core.Diagnostics;

namespace XuanYu.Editor.UI;

internal static class TerrainCameraReplayActions
{
    internal static async Task<long> Wheel(UiVm vm, int step, double delta, long previous)
    {
        var view = vm.CurrentViewport;
        var id = ViewportProbe.BeginWheel(view.LogicalWidth / 2, view.LogicalHeight / 2,
            delta, "Editor", vm.ActiveTool, "TerrainReplay.UiVm.DollyCamera");
        var handled = vm.DollyCamera(delta);
        ViewportProbe.EndWheel(id, handled);
        if (!handled) throw new InvalidOperationException($"Replay Wheel step {step} 未处理。");
        var frame = await WaitNextFrame(previous);
        ViewportProbe.Log("replay", $"[REPLAY-WHEEL] Step={step};Delta={delta};" +
            $"Camera={vm.RenderSnapshot.CameraState}");
        return frame;
    }

    internal static async Task<long> Orbit(UiVm vm, double dx, double dy, long previous)
    {
        var view = vm.CurrentViewport; const long pointer = 73004;
        var x = view.LogicalWidth / 2; var y = view.LogicalHeight / 2;
        var begun = vm.BeginCameraNavigation(pointer, x, y, false,
            (int)view.LogicalWidth, (int)view.LogicalHeight);
        if (!begun || !vm.PreviewCameraNavigation(pointer, x + dx, y + dy))
            throw new InvalidOperationException("Replay Frozen Pivot Orbit 未处理。");
        vm.EndCameraNavigation(pointer);
        return await WaitNextFrame(previous);
    }

    internal static Task<long> WaitNextFrame(long previous) =>
        ViewportProbe.WaitForPresentedFrameAsync(previous, TimeSpan.FromSeconds(15));
}
