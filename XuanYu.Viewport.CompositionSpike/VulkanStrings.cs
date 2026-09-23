using System.Runtime.InteropServices;

namespace XuanYu.Viewport.CompositionSpike;

unsafe sealed class VulkanStrings : IDisposable
{
    readonly List<IntPtr> _items = new();
    public byte** Pointer { get; }
    public byte* First => (byte*)_items[0];
    public VulkanStrings(IEnumerable<string> values)
    {
        var list = values.Select(Marshal.StringToHGlobalAnsi).ToArray();
        _items.AddRange(list);
        Pointer = (byte**)Marshal.AllocHGlobal(IntPtr.Size * list.Length);
        for (var i = 0; i < list.Length; i++) Pointer[i] = (byte*)list[i];
    }
    public uint Count => (uint)_items.Count;
    public void Dispose()
    {
        foreach (var item in _items) Marshal.FreeHGlobal(item);
        Marshal.FreeHGlobal((IntPtr)Pointer);
    }
}
