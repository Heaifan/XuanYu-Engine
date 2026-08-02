using System.Reflection;

namespace XuanYu.WarCore.Tests;

// WARCORE-A-R1-D1：WarCore 程序集依赖方向契约测试。
// 编译期引用由 arch-a-guard-warcore.ps1 强制；此处运行时复核。
public sealed class WarCoreDependencyTests
{
    [Fact]
    public void WarCore_assembly_does_not_reference_editor()
    {
        var references = WarCoreAssembly().GetReferencedAssemblies()
            .Select(r => r.Name)
            .ToArray();

        Assert.DoesNotContain(references, n => n!.StartsWith("XuanYu.Editor", StringComparison.Ordinal));
    }

    [Fact]
    public void WarCore_assembly_does_not_reference_vulkan()
    {
        var references = WarCoreAssembly().GetReferencedAssemblies()
            .Select(r => r.Name)
            .ToArray();

        Assert.DoesNotContain(references, n => n!.Contains("Vulkan", StringComparison.Ordinal));
    }

    [Fact]
    public void WarCore_csproj_references_core()
    {
        // 运行时引用列表只包含被实际使用的程序集（WarCore 当前未直接使用
        // Core 类型，EntityId 关联在 D3），故直接断言 csproj 声明。
        var csproj = File.ReadAllText(
            Path.Combine(AppContext.BaseDirectory, "../../../../XuanYu.WarCore/XuanYu.WarCore.csproj"));

        Assert.Contains("XuanYu.Core.csproj", csproj);
    }

    static Assembly WarCoreAssembly()
    {
        return typeof(XuanYu.WarCore.Identity.MilitaryIdentity).Assembly;
    }
}
