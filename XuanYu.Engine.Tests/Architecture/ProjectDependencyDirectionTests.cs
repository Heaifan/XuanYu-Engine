using System.Xml.Linq;

namespace XuanYu.Engine.Tests.Architecture;

public sealed class ProjectDependencyDirectionTests
{
    [Fact]
    public void ProjectReferences_ShouldMatchAllowedDependencies()
    {
        var allowedDependencies = new Dictionary<string, string[]>
        {
            ["XuanYu.Engine.Core"] = [],
            ["XuanYu.Engine.Project"] = ["XuanYu.Engine.Core"],
            ["XuanYu.Engine"] = ["XuanYu.Engine.Core"],
            ["XuanYu.Engine.Editor"] = ["XuanYu.Engine.Core", "XuanYu.Engine", "XuanYu.Engine.Project"],
            ["XuanYu.Engine.Bridge.ProjectEngine"] = ["XuanYu.Engine.Core", "XuanYu.Engine", "XuanYu.Engine.Project"],
            ["XuanYu.Engine.Render"] = ["XuanYu.Engine.Core", "XuanYu.Engine"],
            ["XuanYu.Engine.Render.Vulkan"] = ["XuanYu.Engine.Core", "XuanYu.Engine.Render"],
            ["XuanYu.Engine.Editor.Windows"] =
            [
                "XuanYu.Engine.Bridge.ProjectEngine",
                "XuanYu.Engine.Core",
                "XuanYu.Engine.Editor",
                "XuanYu.Engine",
                "XuanYu.Engine.Project",
                "XuanYu.Engine.Render",
                "XuanYu.Engine.Render.Vulkan"
            ],
            ["XuanYu.Engine.Tests"] =
            [
                "XuanYu.Engine.Bridge.ProjectEngine",
                "XuanYu.Engine.Core",
                "XuanYu.Engine.Editor",
                "XuanYu.Engine.Editor.Windows",
                "XuanYu.Engine",
                "XuanYu.Engine.Project",
                "XuanYu.Engine.Render",
                "XuanYu.Engine.Render.Vulkan"
            ]
        };

        foreach (var (projectName, allowedProjectNames) in allowedDependencies)
        {
            var references = ReadProjectReferences(projectName);

            Assert.Equal(
                allowedProjectNames.Order(StringComparer.Ordinal),
                references.Order(StringComparer.Ordinal));
        }
    }

    [Fact]
    public void Core_ShouldNotHavePackageReferences()
    {
        var packageReferences = ReadPackageReferences("XuanYu.Engine.Core");

        Assert.Empty(packageReferences);
    }

    [Fact]
    public void PackageReferences_ShouldMatchAllowedPackages()
    {
        var allowedPackages = new Dictionary<string, string[]>
        {
            ["XuanYu.Engine.Core"] = [],
            ["XuanYu.Engine.Project"] = [],
            ["XuanYu.Engine"] = [],
            ["XuanYu.Engine.Editor"] = [],
            ["XuanYu.Engine.Bridge.ProjectEngine"] = [],
            ["XuanYu.Engine.Render"] = [],
            ["XuanYu.Engine.Render.Vulkan"] = ["Silk.NET.Vulkan"],
            ["XuanYu.Engine.Editor.Windows"] =
            [
                "Avalonia",
                "Avalonia.Desktop",
                "Avalonia.Fonts.Inter",
                "Avalonia.Themes.Fluent",
                "Svg.Controls.Skia.Avalonia"
            ],
            ["XuanYu.Engine.Tests"] =
            [
                "coverlet.collector",
                "Microsoft.NET.Test.Sdk",
                "xunit",
                "xunit.runner.visualstudio"
            ]
        };

        foreach (var (projectName, allowedProjectNames) in allowedPackages)
        {
            var references = ReadPackageReferences(projectName);

            Assert.Equal(
                allowedProjectNames.Order(StringComparer.Ordinal),
                references.Order(StringComparer.Ordinal));
        }
    }

    private static IReadOnlyList<string> ReadProjectReferences(string projectName)
    {
        var projectFile = FindProjectFile(projectName);
        var document = XDocument.Load(projectFile);

        return document
            .Descendants("ProjectReference")
            .Select(e => e.Attribute("Include")?.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => Path.GetFileNameWithoutExtension(value!))
            .ToArray();
    }

    private static IReadOnlyList<string> ReadPackageReferences(string projectName)
    {
        var projectFile = FindProjectFile(projectName);
        var document = XDocument.Load(projectFile);

        return document
            .Descendants("PackageReference")
            .Select(e => e.Attribute("Include")?.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value!)
            .ToArray();
    }

    private static string FindProjectFile(string projectName)
    {
        var root = FindRepositoryRoot();
        return Path.Combine(root, projectName, $"{projectName}.csproj");
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            var solutionPath = Path.Combine(current.FullName, "XuanYu.Engine.sln");
            if (File.Exists(solutionPath))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("未找到 XuanYu Engine 仓库根目录。");
    }
}
