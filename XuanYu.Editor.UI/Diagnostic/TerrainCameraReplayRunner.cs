using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using XuanYu.Core.Diagnostics;
using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

internal static class TerrainCameraReplayRunner
{
    public static bool Requested => TerrainReplayOptions.Enabled &&
        File.Exists(Environment.GetEnvironmentVariable("XYE_TERRAIN_REPLAY_HGT"));

    public static void Attach(UiWin window, UiVm vm,
        IClassicDesktopStyleApplicationLifetime desktop)
    {
        if (!Requested) return;
        window.WindowState = WindowState.Maximized;
        window.Opened += async (_, _) =>
        {
            try { await RunAsync(vm, window); }
            catch (Exception error)
            {
                ViewportProbe.Log("replay", $"[REPLAY-FAILED] {error}");
            }
            finally { desktop.Shutdown(); }
        };
    }

    static async Task RunAsync(UiVm vm, UiWin window)
    {
        var output = Environment.GetEnvironmentVariable("XYE_TERRAIN_REPLAY_OUTPUT")
            ?? throw new InvalidOperationException("缺少 replay output 目录。");
        Directory.CreateDirectory(output);
        var runId = Environment.GetEnvironmentVariable("XYE_TERRAIN_REPLAY_RUN_ID")
            ?? Guid.NewGuid().ToString("N");
        var lastFrame = await ViewportProbe.WaitForPresentedFrameAsync(
            0, TimeSpan.FromSeconds(30));
        ViewportProbe.RefreshLoadedAssemblies();
        ViewportProbe.Log("replay", $"[REPLAY-START] RunId={runId};" +
            $"Build={ViewportProbeIdentity.Current()};" +
            $"Variant={TerrainReplayOptions.Variant};" +
            $"Hgt={Environment.GetEnvironmentVariable("XYE_TERRAIN_REPLAY_HGT")}");
        var imported = await vm.ImportTerrainSourceAsync(
            Environment.GetEnvironmentVariable("XYE_TERRAIN_REPLAY_HGT")!);
        if (!imported)
            throw new InvalidOperationException($"HGT 产品导入失败：{vm.TerrainImportMessage}");
        lastFrame = await WaitNextFrame(lastFrame);
        lastFrame = await TerrainReplayCapture.CaptureNextAsync(
            vm, window, output, runId, "autoframe", lastFrame);
        for (var index = 0; index < 20; index++) lastFrame = await Wheel(vm, index, 1, lastFrame);
        lastFrame = await Orbit(vm, 45, 25, lastFrame);
        lastFrame = await TerrainReplayCapture.CaptureNextAsync(
            vm, window, output, runId, "pitch-a", lastFrame);
        for (var index = 20; index < 40; index++) lastFrame = await Wheel(vm, index, 1, lastFrame);
        lastFrame = await Orbit(vm, -85, -55, lastFrame);
        lastFrame = await TerrainReplayCapture.CaptureNextAsync(
            vm, window, output, runId, "near-ground-ab", lastFrame);
        for (var index = 0; index < 3; index++) lastFrame = await Wheel(vm, index, -1, lastFrame);
        lastFrame = await TerrainReplayCapture.CaptureNextAsync(
            vm, window, output, runId, "reverse-wheel", lastFrame);
        vm.RunCommand.Execute("查看全部");
        lastFrame = await WaitNextFrame(lastFrame);
        lastFrame = await TerrainReplayCapture.CaptureNextAsync(
            vm, window, output, runId, "view-all", lastFrame);
        ViewportProbe.Log("replay", $"[REPLAY-COMPLETE] RunId={runId};" +
            $"FinalFrameId={lastFrame}");
    }

    static async Task<long> Wheel(UiVm vm, int step, double delta, long previous)
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

    static async Task<long> Orbit(UiVm vm, double dx, double dy, long previous)
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

    static Task<long> WaitNextFrame(long previous) =>
        ViewportProbe.WaitForPresentedFrameAsync(previous, TimeSpan.FromSeconds(15));

}
