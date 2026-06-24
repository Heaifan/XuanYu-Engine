using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Validation;

/// <summary>持有 DebugUtilsMessengerEXT 生命周期。必须持有 callback 防 GC，在 Instance 销毁前释放。</summary>
public sealed unsafe class VulkanDebugMessengerScope : IDisposable
{
    readonly Vk _vk;
    readonly Silk.NET.Vulkan.Instance _instance;
    DebugUtilsMessengerEXT _messenger;
    bool _disposed;
    readonly DebugUtilsMessengerCallbackFunctionEXT _callback;
    readonly VulkanValidationMessageStore _messageStore;
    readonly nint _fnCreate, _fnDestroy;

    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate Result CreatePtr(Silk.NET.Vulkan.Instance i, DebugUtilsMessengerCreateInfoEXT* ci, AllocationCallbacks* a, DebugUtilsMessengerEXT* m);
    [UnmanagedFunctionPointer(CallingConvention.Winapi)] delegate void DestroyPtr(Silk.NET.Vulkan.Instance i, DebugUtilsMessengerEXT m, AllocationCallbacks* a);

    public VulkanDebugMessengerScope(Vk vk, Silk.NET.Vulkan.Instance instance, VulkanValidationMessageStore messageStore)
    {
        _vk = vk; _instance = instance; _messageStore = messageStore; _callback = DebugCallback;
        _fnCreate = LoadProc("vkCreateDebugUtilsMessengerEXT"); _fnDestroy = LoadProc("vkDestroyDebugUtilsMessengerEXT");
        if (_fnCreate == 0 || _fnDestroy == 0) throw new InvalidOperationException("无法加载 Debug Utils 函数。");
        var ci = new DebugUtilsMessengerCreateInfoEXT
        { SType = StructureType.DebugUtilsMessengerCreateInfoExt,
            MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.VerboseBitExt | DebugUtilsMessageSeverityFlagsEXT.WarningBitExt | DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
            MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt | DebugUtilsMessageTypeFlagsEXT.ValidationBitExt | DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt,
            PfnUserCallback = _callback, PUserData = null };
        var createFn = Marshal.GetDelegateForFunctionPointer<CreatePtr>(_fnCreate);
        DebugUtilsMessengerEXT msg; createFn(_instance, &ci, null, &msg); _messenger = msg;
    }

    public int MessageCount => _messageStore?.Count ?? 0;
    public IReadOnlyList<VulkanValidationMessageInfo> GetMessages() => _messageStore?.Snapshot() ?? [];

    uint DebugCallback(DebugUtilsMessageSeverityFlagsEXT severity, DebugUtilsMessageTypeFlagsEXT types, DebugUtilsMessengerCallbackDataEXT* pData, void* _)
    {
        if (pData is null) return Vk.False;
        var msg = Marshal.PtrToStringAnsi((nint)pData->PMessage) ?? "未知";
        var sv = (severity & DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt) != 0 ? "报错" : (severity & DebugUtilsMessageSeverityFlagsEXT.WarningBitExt) != 0 ? "警告" : "信息";
        var tp = (types & DebugUtilsMessageTypeFlagsEXT.ValidationBitExt) != 0 ? "Validation" : (types & DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt) != 0 ? "性能" : "通用";
        _messageStore?.Add(new VulkanValidationMessageInfo(sv, tp, msg));
        return Vk.False;
    }

    nint LoadProc(string name) { var p = Marshal.StringToHGlobalAnsi(name); try { return (nint)_vk.GetInstanceProcAddr(_instance, (byte*)p); } finally { Marshal.FreeHGlobal(p); } }

    public void Dispose()
    {
        if (_disposed) return; _disposed = true;
        if (_messenger.Handle != 0 && _fnDestroy != 0)
        { var dfn = Marshal.GetDelegateForFunctionPointer<DestroyPtr>(_fnDestroy); dfn(_instance, _messenger, null); _messenger = default; }
    }
}
