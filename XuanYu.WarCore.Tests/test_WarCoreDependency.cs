using System.Reflection;
using System.Xml.Linq;

namespace XuanYu.WarCore.Tests;

// WARCORE-A-R1-D1：WarCore 程序集依赖方向契约测试。
// 编译期引用由 arch-a-guard-warcore.ps1 强制；此处运行时复核。
public sealed class WarCoreDependencyTests
{
    [Fact]
    public void WarCore_assembly_dependency_contract_excludes_editor()
    {
        var references = WarCoreAssembly().GetReferencedAssemblies()
            .Select(r => r.Name)
            .ToArray();

        Assert.DoesNotContain(references, n => n!.StartsWith("XuanYu.Editor", StringComparison.Ordinal));
    }

    [Fact]
    public void WarCore_assembly_dependency_contract_excludes_vulkan()
    {
        var references = WarCoreAssembly().GetReferencedAssemblies()
            .Select(r => r.Name)
            .ToArray();

        Assert.DoesNotContain(references, n => n!.Contains("Vulkan", StringComparison.Ordinal));
    }

    [Fact]
    public void WarCore_csproj_architecture_dependency_contract_references_core()
    {
        // 这是项目声明契约，不是运行时程序集依赖行为断言。
        var csprojPath = Path.Combine(
            AppContext.BaseDirectory, "../../../../XuanYu.WarCore/XuanYu.WarCore.csproj");
        var projectReferences = XDocument.Load(csprojPath)
            .Descendants("ProjectReference")
            .Select(reference => (string?)reference.Attribute("Include"))
            .Where(include => include is not null)
            .Select(include => Path.GetFileName(include!))
            .ToArray();

        Assert.Contains("XuanYu.Core.csproj", projectReferences);
    }

    static Assembly WarCoreAssembly()
    {
        return typeof(XuanYu.WarCore.Identity.MilitaryIdentity).Assembly;
    }
}
