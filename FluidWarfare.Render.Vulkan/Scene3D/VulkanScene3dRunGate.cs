using FluidWarfare.Render.Vulkan.Shaders;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// Scene3D 运行闸门，控制实验性 3D 管线是否允许手动触发。
/// 三重闸门：环境变量允许 + SPIR-V 合法性允许 + 视口就绪。
///
/// 当前 8.R.3 阶段：
/// 即使设了 FW_ENABLE_SCENE3D=1 且 CompiledShaders 通过 spirv-val，
/// Scene3D 也不会自动进入 Editor 启动或 resize 路径。
/// 必须通过手动触发入口（如 DebugDock 按钮）调用 TryRunScene3dProbeManually。
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
    /// 仅表示允许手动触发，不表示自动运行。
    /// </summary>
    public static VulkanScene3dRunGate Ready(string message)
    {
        return new VulkanScene3dRunGate(true, message);
    }

    /// <summary>
    /// 基于运行环境创建 Scene3D 运行闸门。
    ///
    /// 8.R.3 规则：
    ///   FW_ENABLE_SCENE3D 未设置                    → Isolated
    ///   FW_ENABLE_SCENE3D=1 但 shader 未验证        → Isolated
    ///   FW_ENABLE_SCENE3D=1 且 shader 已验证         → Ready
    /// </summary>
    public static VulkanScene3dRunGate Evaluate()
    {
        var envRequested = string.Equals(
            Environment.GetEnvironmentVariable("FW_ENABLE_SCENE3D"),
            "1",
            StringComparison.Ordinal);

        if (!envRequested)
        {
            return Isolated("Scene3D：已隔离，未设置 FW_ENABLE_SCENE3D=1。");
        }

        if (!CompiledShaders.HasValidatedBasic3dShaders)
        {
            return Isolated("Scene3D：已隔离，SPIR-V 字节码缺失或未通过 spirv-val 验证。");
        }

        return Ready("Scene3D：已就绪，SPIR-V 已通过 spirv-val 验证，等待手动触发。");
    }
}
