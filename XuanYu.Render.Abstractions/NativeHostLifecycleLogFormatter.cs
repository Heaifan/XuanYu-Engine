using System.Globalization;

namespace XuanYu.Render.Abstractions;

// VK3-A-R1：从 XuanYu.Render.Vulkan 迁入的纯生命周期日志格式器。
// 仅生成中文生命周期日志文本，不含任何 Vulkan / Silk.NET 依赖。
public static class NativeHostLifecycleLogFormatter
{
    public static string Message(NativeHostHandleSnapshot snapshot) => snapshot.State switch
    {
        NativeHostLifecycleState.Created => "【NativeHost】创建宿主控件",
        NativeHostLifecycleState.Attached => "【NativeHost】附加到可视树",
        NativeHostLifecycleState.HandleAvailable => "【NativeHost】原生句柄可用",
        NativeHostLifecycleState.Resized => "【NativeHost】尺寸变化",
        NativeHostLifecycleState.Detached => "【NativeHost】从可视树移除",
        NativeHostLifecycleState.Disposed => "【NativeHost】释放宿主控件",
        NativeHostLifecycleState.Invalidated => "【NativeHost】原生句柄失效",
        _ => "【NativeHost】生命周期事件"
    };

    public static string Detail(NativeHostHandleSnapshot snapshot) =>
        $"窗口句柄：0x{snapshot.Hwnd.ToInt64():X}；宽度：{snapshot.Width}；高度：{snapshot.Height}；DPI缩放：{snapshot.DpiScale.ToString("0.00", CultureInfo.InvariantCulture)}；句柄状态：{(snapshot.IsValid ? "有效" : "无效")}；生命周期版本：{snapshot.Version}";

    public static string MergedMessage(NativeHostHandleSnapshot snapshot, int mergeCount) =>
        snapshot.IsValid
            ? $"【NativeHost】尺寸变化已合并：宽度={snapshot.Width}，高度={snapshot.Height}，DPI={snapshot.DpiScale.ToString("0.00", CultureInfo.InvariantCulture)}，生命周期版本={snapshot.Version}，合并次数={mergeCount}"
            : "【NativeHost】尺寸变化已合并但句柄无效";
}
