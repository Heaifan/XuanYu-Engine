using XYUI.Avalonia.Catalog;

namespace XYUI.Avalonia.Tests;

public class CatalogSourceTests
{
    [Fact]
    public void Catalog_Uses_Registry_And_All_Current_Mappings()
    {
        var root = FindRoot();
        var entries = XyuiCatalogSource.Load(root);

        Assert.Equal(209, entries.Count);
        Assert.Equal(44, entries.Count(x => x.Module == "XYUI-0"));
        Assert.Equal(164, entries.Count(x => x.Module.StartsWith("XYUI-")
            && x.Module != "XYUI-0" && x.Module != "XYUI-9"));
        Assert.Single(entries, x => x.Module == "XYUI-9");
        Assert.Equal(entries.Count, entries.Select(x => x.SourceItemId).Distinct().Count());
    }

    [Fact]
    public void Missing_XYUI9_Is_Explicit_And_Not_Ready()
    {
        var entry = XyuiCatalogSource.Load(FindRoot()).Single(x => x.Module == "XYUI-9");

        Assert.False(entry.SourcePresent);
        Assert.False(entry.Status.Ready);
        Assert.Equal("SOURCE NOT PRESENT IN CURRENT REPOSITORY", entry.AvaloniaText);
    }

    [Fact]
    public void Implemented_Controls_Expose_Real_Avalonia_Types()
    {
        var entries = XyuiCatalogSource.Load(FindRoot());
        var controls = entries.Where(x => x.Module == "XYUI-2" && x.Status.Avalonia).ToArray();

        Assert.Equal(5, controls.Length);
        Assert.All(controls, x => Assert.Contains("XYUI.Avalonia.Controls", x.AvaloniaType));
        Assert.All(controls, x => Assert.True(x.Status.Gallery));
    }

    [Fact]
    public void Catalog_Exposes_Documentation_Fields_From_Spec()
    {
        var button = XyuiCatalogSource.Load(FindRoot()).Single(x => x.SourceItemId == "XYUI-2-01");

        Assert.NotEmpty(button.Description);
        Assert.NotEmpty(button.Usage);
        Assert.NotEmpty(button.Variants);
        Assert.NotEmpty(button.States);
        Assert.NotEmpty(button.ApiRefs);
    }

    static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "xyui", "registry", "foundation", "identity-map.json")))
            {
                return directory.FullName;
            }
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("XYUI repository root not found");
    }
}
