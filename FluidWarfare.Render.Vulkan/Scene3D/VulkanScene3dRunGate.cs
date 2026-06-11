namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>
/// Scene3D 运行闸门，控制实验性 3D 管线是否允许执行。
/// 双重闸门：环境变量允许 + SPIR-V 合法性允许。
///
/// 当前 8.R.1 阶段：
/// 即使设了 FW_ENABLE_SCENE3D=1，因为 SPIR-V 未通过 spirv-val 验证，
/// CanRun 始终返回 false。Editor 不会自动启动 Scene3D 渲染。
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
    /// 当前 8.R.1 阶段不使用。
    /// </summary>
    public static VulkanScene3dRunGate Ready(string message)
    {
        return new VulkanScene3dRunGate(true, message);
    }

    /// <summary>
    /// 基于运行环境创建 Scene3D 运行闸门。
    /// 当前 8.R.1 阶段始终返回 Isolated，因为 SPIR-V 合法性验证未通过。
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

        // SPIR-V 合法性验证尚未通过，即使设了环境变量也不运行
        return Isolated("Scene3D：已隔离，当前 SPIR-V 编译链未通过 spirv-val 合法性验证。");
    }
}
