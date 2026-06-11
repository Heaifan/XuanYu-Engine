namespace FluidWarfare.Render.Vulkan.Shaders;

/// <summary>
/// 预编译 SPIR-V 着色器字节码。
///
/// ⛔ 当前为占位空数组。tools/gen_spirv 手写 SPIR-V 编码器已废弃。
/// 正确的 SPIR-V 需要由标准工具（glslangValidator / shaderc / DXC）编译生成。
/// 本文件在工具链就绪前保持空数组站位，保证编译通过。
///
/// Scene3D 渲染已隔离，不参与 Editor 自动启动。
/// 恢复条件：通过 spirv-val 合法性验证。
/// </summary>
internal static class CompiledShaders
{
    /// <summary>
    /// 基础 3D 顶点着色器 SPIR-V。
    /// 当前为空数组 — 等待标准编译链就绪。
    /// </summary>
    public static uint[] Basic3dVert { get; } = [];

    /// <summary>
    /// 基础 3D 片段着色器 SPIR-V。
    /// 当前为空数组 — 等待标准编译链就绪。
    /// </summary>
    public static uint[] Basic3dFrag { get; } = [];
}
