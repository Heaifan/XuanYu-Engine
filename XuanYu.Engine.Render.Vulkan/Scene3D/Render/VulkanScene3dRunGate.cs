using XuanYu.Engine.Render.Vulkan.Shaders;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>
/// Scene3D 运行闸门，控制 3D 管线是否允许启动。
///
/// 8.3.1 规则（默认启用，FW_DISABLE_SCENE3D=1 可关闭）：
///   FW_DISABLE_SCENE3D=1                    → Isolated
///   FW_DISABLE_SCENE3D 未设置，Shader 已验证   → Ready
///   FW_DISABLE_SCENE3D 未设置，Shader 未验证   → Isolated
/// </summary>
public sealed record VulkanScene3dRunGate(bool CanRun, string Message)
{
    /// <summary>
    /// 创建隔离状态（CanRun = false）。
    /// </summary>
    public static VulkanScene3dRunGate Isolated(string message)
    {
        return new VulkanScene3dRunGate(false, message);
    }

    /// <summary>
    /// 创建就绪状态（CanRun = true）。
    /// </summary>
    public static VulkanScene3dRunGate Ready(string message)
    {
        return new VulkanScene3dRunGate(true, message);
    }

    /// <summary>
    /// 基于运行环境创建 Scene3D 运行闸门。
    ///
    /// 规则：
    ///   FW_DISABLE_SCENE3D=1           → Isolated
    ///   Shader 未通过验证               → Isolated
    ///   以上均不满足                    → Ready（默认启用）
    /// </summary>
    public static VulkanScene3dRunGate Evaluate()
    {
        var disabled = string.Equals(
            Environment.GetEnvironmentVariable("FW_DISABLE_SCENE3D"),
            "1",
            StringComparison.Ordinal);

        if (disabled)
        {
            return Isolated("Scene3D：已由 FW_DISABLE_SCENE3D=1 禁用。");
        }

        if (!CompiledShaders.HasValidatedBasic3dShaders)
        {
            return Isolated("Scene3D：已隔离，SPIR-V 字节码缺失或未通过 spirv-val 验证。");
        }

        return Ready("Scene3D：已就绪。");
    }
}
