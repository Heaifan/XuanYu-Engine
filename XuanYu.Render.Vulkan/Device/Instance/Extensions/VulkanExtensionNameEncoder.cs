using System.Text;

namespace XuanYu.Render.Vulkan;

internal static class VulkanExtensionNameEncoder
{
    public static byte[] Encode(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (name.Contains('\0')) throw new ArgumentException("Extension names must not contain NUL.", nameof(name));
        return Encoding.UTF8.GetBytes(name + "\0");
    }
}
