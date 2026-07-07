using System;
using Avalonia.Threading;
using XuanYu.Render.Vulkan;

namespace XuanYu.Editor.UI;

/// <summary>
/// 合并连续尺寸变化：连续 SizeChanged 只更新快照与合并计数，
/// 250ms 内无新变化后才生成一条低频合并日志，避免高频事件进入日志总线。
/// Detach / Dispose 时调用 Cancel 安全停止 pending debounce，不补写日志。
/// </summary>
public sealed class NativeHostResizeCoalescer
{
    readonly NativeHostLifecycleProbe _probe = new();
    readonly Action<NativeHostHandleSnapshot, int> _onMerged;
    readonly DispatcherTimer _timer;
    NativeHostResizeSnapshot _last;
    int _mergeCount;

    public NativeHostResizeCoalescer(Action<NativeHostHandleSnapshot, int> onMerged)
    {
        _onMerged = onMerged;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _timer.Tick += OnStable;
    }

    public void OnResize(int width, int height, double dpiScale, bool isValid, nint hwnd)
    {
        _last = new NativeHostResizeSnapshot(width, height, dpiScale, isValid, hwnd);
        _mergeCount++;
        _timer.Stop();
        _timer.Start();
    }

    public void Cancel()
    {
        _timer.Stop();
        _mergeCount = 0;
    }

    void OnStable(object? sender, EventArgs e)
    {
        _timer.Stop();
        if (_mergeCount == 0) return;
        var snapshot = _probe.Capture(NativeHostLifecycleState.Resized, _last.Hwnd, _last.Width, _last.Height, _last.DpiScale, _last.IsValid);
        _onMerged(snapshot, _mergeCount);
        _mergeCount = 0;
    }
}
