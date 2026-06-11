using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Validation;

/// <summary>
/// 持有 DebugUtilsMessengerEXT 的生命周期。
/// 必须持有 callback delegate 防止被 GC 回收。
/// 必须在 Instance 销毁之前释放。
/// </summary>
public sealed unsafe class VulkanDebugMessengerScope : IDisposable
{
    private readonly Vk _vk;
    private readonly Silk.NET.Vulkan.Instance _instance;
    private DebugUtilsMessengerEXT _messenger;
    private bool _disposed;

    // 持有 callback 防止 GC 回收（T0 重要事项）
    private readonly DebugUtilsMessengerCallbackFunctionEXT _callback;
    private readonly VulkanValidationMessageStore _messageStore;

    // 函数指针
    private readonly nint _fnCreateDebugUtilsMessenger;
    private readonly nint _fnDestroyDebugUtilsMessenger;

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate Result CreateDebugUtilsMessengerPtr(
        Silk.NET.Vulkan.Instance instance,
        DebugUtilsMessengerCreateInfoEXT* pCreateInfo,
        AllocationCallbacks* pAllocator,
        DebugUtilsMessengerEXT* pMessenger);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void DestroyDebugUtilsMessengerPtr(
        Silk.NET.Vulkan.Instance instance,
        DebugUtilsMessengerEXT messenger,
        AllocationCallbacks* pAllocator);

    public VulkanDebugMessengerScope(Vk vk, Silk.NET.Vulkan.Instance instance,
        VulkanValidationMessageStore messageStore)
    {
        _vk = vk;
        _instance = instance;
        _messageStore = messageStore;

        // 加载函数指针
        _fnCreateDebugUtilsMessenger = LoadProc("vkCreateDebugUtilsMessengerEXT");
        _fnDestroyDebugUtilsMessenger = LoadProc("vkDestroyDebugUtilsMessengerEXT");

        if (_fnCreateDebugUtilsMessenger == 0 || _fnDestroyDebugUtilsMessenger == 0)
            return;

        // 创建 callback（必须持有）
        _callback = DebugCallback;

        var ci = new DebugUtilsMessengerCreateInfoEXT
        {
            SType = StructureType.DebugUtilsMessengerCreateInfoExt,
            MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt |
                              DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                              DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
            MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                          DebugUtilsMessageTypeFlagsEXT.ValidationBitExt |
                          DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt,
            PfnUserCallback = _callback,
            PUserData = null
        };

        var createFn = Marshal.GetDelegateForFunctionPointer<CreateDebugUtilsMessengerPtr>(
            _fnCreateDebugUtilsMessenger);

        DebugUtilsMessengerEXT messenger;
        createFn(_instance, &ci, null, &messenger);
        _messenger = messenger;
    }

    /// <summary>
    /// 最近的 Validation 消息数量。
    /// </summary>
    public int MessageCount => _messageStore?.Count ?? 0;

    /// <summary>
    /// 获取消息快照。
    /// </summary>
    public IReadOnlyList<VulkanValidationMessageInfo> GetMessages() =>
        _messageStore?.Snapshot() ?? [];

    private uint DebugCallback(
        DebugUtilsMessageSeverityFlagsEXT severity,
        DebugUtilsMessageTypeFlagsEXT types,
        DebugUtilsMessengerCallbackDataEXT* pCallbackData,
        void* pUserData)
    {
        if (pCallbackData is null)
            return Vk.False;

        var msg = Marshal.PtrToStringAnsi((nint)pCallbackData->PMessage) ?? "未知";
        var severityText = (severity & DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt) != 0 ? "报错"
                         : (severity & DebugUtilsMessageSeverityFlagsEXT.WarningBitExt) != 0 ? "警告"
                         : "信息";
        var typeText = (types & DebugUtilsMessageTypeFlagsEXT.ValidationBitExt) != 0 ? "Validation"
                     : (types & DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt) != 0 ? "性能"
                     : "通用";

        _messageStore?.Add(new VulkanValidationMessageInfo(severityText, typeText, msg));
        return Vk.False;
    }

    private nint LoadProc(string name)
    {
        var p = Marshal.StringToHGlobalAnsi(name);
        try
        {
            return (nint)_vk.GetInstanceProcAddr(_instance, (byte*)p);
        }
        finally
        {
            Marshal.FreeHGlobal(p);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (_messenger.Handle != 0 && _fnDestroyDebugUtilsMessenger != 0)
        {
            var destroyFn = Marshal.GetDelegateForFunctionPointer<DestroyDebugUtilsMessengerPtr>(
                _fnDestroyDebugUtilsMessenger);
            destroyFn(_instance, _messenger, null);
            _messenger = default;
        }
    }
}
