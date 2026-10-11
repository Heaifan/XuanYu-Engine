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
        lastFrame = await TerrainCameraReplayActions.WaitNextFrame(lastFrame);
        if (TerrainNativeInputReplay.Requested)
        {
            await TerrainNativeInputReplay.RunAsync(vm, window, output, runId, lastFrame);
            return;
        }
        lastFrame = await TerrainReplayCapture.CaptureNextAsync(
            vm, window, output, runId, "autoframe", lastFrame);
        for (var index = 0; index < 20; index++)
            lastFrame = await TerrainCameraReplayActions.Wheel(vm, index, 1, lastFrame);
        lastFrame = await TerrainCameraReplayActions.Orbit(vm, 45, 25, lastFrame);
        lastFrame = await TerrainReplayCapture.CaptureNextAsync(
            vm, window, output, runId, "pitch-a", lastFrame);
        for (var index = 20; index < 40; index++)
            lastFrame = await TerrainCameraReplayActions.Wheel(vm, index, 1, lastFrame);
        lastFrame = await TerrainCameraReplayActions.Orbit(vm, -85, -55, lastFrame);
        lastFrame = await TerrainReplayCapture.CaptureNextAsync(
            vm, window, output, runId, "near-ground-ab", lastFrame);
        for (var index = 0; index < 3; index++)
            lastFrame = await TerrainCameraReplayActions.Wheel(vm, index, -1, lastFrame);
        lastFrame = await TerrainReplayCapture.CaptureNextAsync(
            vm, window, output, runId, "reverse-wheel", lastFrame);
        vm.RunCommand.Execute("查看全部");
        lastFrame = await TerrainCameraReplayActions.WaitNextFrame(lastFrame);
        lastFrame = await TerrainReplayCapture.CaptureNextAsync(
            vm, window, output, runId, "view-all", lastFrame);
        ViewportProbe.Log("replay", $"[REPLAY-COMPLETE] RunId={runId};" +
            $"FinalFrameId={lastFrame}");
    }

}
