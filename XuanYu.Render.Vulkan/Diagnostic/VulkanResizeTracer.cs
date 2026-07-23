using System;
using System.Diagnostics;

namespace XuanYu.Render.Vulkan.Diagnostic;

// RZ-VK5-D-R1：Resize / Present 慢半拍全链路诊断追踪器。
// 每次 Resize 或自愈生成一个追踪段，用共享 Stopwatch 计算各阶段 elapsedMs，
// 帮助定位"慢半拍"发生在哪一段（UI 合并 / Win32 子窗口 / Swapchain 重建 / Framebuffer 重录 / Present 自愈）。
public static class VulkanResizeTracer
{
    static readonly Stopwatch _sw = new();
    static long _traceStartTicks;
    static bool _running;

    // 开始一段新的 Resize/自愈追踪。返回起点相对绝对时间无意义，只用于 ElapsedMs 差值。
    public static long StartTrace()
    {
        if (!_running) { _sw.Start(); _running = true; }
        _traceStartTicks = _sw.ElapsedTicks;
        return _traceStartTicks;
    }

    // 从追踪起点到现在的毫秒数。
    public static double ElapsedMs(long startTicks = 0)
    {
        var baseTicks = startTicks != 0 ? startTicks : _traceStartTicks;
        return (_sw.ElapsedTicks - baseTicks) / (double)Stopwatch.Frequency * 1000.0;
    }

    // 格式化阶段日志前缀：[T+XXXms 代际=N 阶段]
    public static string Prefix(uint generation, string phase) =>
        $"[T+{ElapsedMs():F0}ms 代际={generation} {phase}]";

    // 完整阶段行：[T+XXXms 代际=N] 阶段：detail
    public static string Stage(uint generation, string stage, string detail) =>
        $"[T+{ElapsedMs():F0}ms 代际={generation}] {stage}：{detail}";

    // 自愈阶段专用：标注来源 + 旧/new extent
    public static string HealStage(uint generation, string source, string oldExtent, string newExtent, string extra = "") =>
        $"[T+{ElapsedMs():F0}ms 代际={generation}] 自愈({source})：旧={oldExtent} → 新={newExtent}{(string.IsNullOrEmpty(extra) ? "" : "；" + extra)}";

    // 重复检测：同一轮中第 N 次 Recreate
    public static string DuplicateWarning(uint generation, int count, string trigger1, string trigger2) =>
        $"[T+{ElapsedMs():F0}ms 代际={generation}] 同轮检测到第 {count} 次 Swapchain 重建：先由「{trigger1}」触发，再由「{trigger2}」触发，可能重复";
}
