using System.Xml.Linq;

namespace FluidWarfare.Tests.Architecture;

public sealed class ProjectDependencyDirectionTests
{
    [Fact]
    public void ProjectReferences_ShouldMatchAllowedDependencies()
    {
        var allowedDependencies = new Dictionary<string, string[]>
        {
            ["FluidWarfare.Core"] = [],
            ["FluidWarfare.Project"] = ["FluidWarfare.Core"],
            ["FluidWarfare.Engine"] = ["FluidWarfare.Core"],
            ["FluidWarfare.Editor"] = ["FluidWarfare.Core", "FluidWarfare.Engine", "FluidWarfare.Project"],
            ["FluidWarfare.Bridge.ProjectEngine"] = ["FluidWarfare.Core", "FluidWarfare.Engine", "FluidWarfare.Project"],
            ["FluidWarfare.Render"] = ["FluidWarfare.Core", "FluidWarfare.Engine"],
            ["FluidWarfare.Render.Vulkan"] = ["FluidWarfare.Core", "FluidWarfare.Render"],
            ["FluidWarfare.Editor.Windows"] =
            [
                "FluidWarfare.Bridge.ProjectEngine",
                "FluidWarfare.Core",
                "FluidWarfare.Editor",
                "FluidWarfare.Engine",
                "FluidWarfare.Project",
                "FluidWarfare.Render",
                "FluidWarfare.Render.Vulkan"
            ],
            ["FluidWarfare.Tests"] =
            [
                "FluidWarfare.Bridge.ProjectEngine",
                "FluidWarfare.Core",
                "FluidWarfare.Editor",
                "FluidWarfare.Engine",
                "FluidWarfare.Project",
                "FluidWarfare.Render",
                "FluidWarfare.Render.Vulkan"
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
        var packageReferences = ReadPackageReferences("FluidWarfare.Core");

        Assert.Empty(packageReferences);
    }

    [Fact]
    public void PackageReferences_ShouldMatchAllowedPackages()
    {
        var allowedPackages = new Dictionary<string, string[]>
        {
            ["FluidWarfare.Core"] = [],
            ["FluidWarfare.Project"] = [],
            ["FluidWarfare.Engine"] = [],
            ["FluidWarfare.Editor"] = [],
            ["FluidWarfare.Bridge.ProjectEngine"] = [],
            ["FluidWarfare.Render"] = [],
            ["FluidWarfare.Render.Vulkan"] = ["Silk.NET.Vulkan"],
            ["FluidWarfare.Editor.Windows"] =
            [
                "Avalonia",
                "Avalonia.Desktop",
                "Avalonia.Fonts.Inter",
                "Avalonia.Themes.Fluent",
                "Svg.Skia"
            ],
            ["FluidWarfare.Tests"] =
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
            var solutionPath = Path.Combine(current.FullName, "FluidWarfare.sln");
            if (File.Exists(solutionPath))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("未找到 FluidWarfare 仓库根目录。");
    }
}
