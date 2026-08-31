using Avalonia.Controls;
using XYUI.Avalonia.Catalog;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

public sealed class XYUI1CoverageTests
{
    [Fact]
    public void Canonical_inventory_has_24_unique_component_ids()
    {
        var entries = Entries();
        Assert.Equal(24, entries.Length);
        Assert.Equal(24, entries.Select(x => x.CanonicalId).Distinct().Count());
        Assert.Equal(Enumerable.Range(1, 24).Select(x => $"XYUI-1-{x:00}"), entries.Select(x => x.CanonicalId));
    }

    [Fact]
    public void Every_XYUI1_entry_has_Avalonia_Catalog_Gallery_and_documentation()
    {
        var entries = Entries();
        Assert.All(entries, entry =>
        {
            Assert.True(entry.Status.Avalonia, entry.CanonicalId);
            Assert.True(entry.Status.Gallery, entry.CanonicalId);
            Assert.True(entry.Status.Documented, entry.CanonicalId);
            Assert.NotEmpty(entry.AvaloniaType);
            Assert.NotEmpty(entry.ApiRefs);
        });
    }

    [Fact]
    public void Every_XYUI1_component_is_creatable_and_has_stable_identity()
    {
        foreach (var entry in Entries())
        {
            var type = Type.GetType($"{entry.AvaloniaType}, XYUI.Avalonia");
            Assert.NotNull(type);
            var control = Activator.CreateInstance(type!);
            Assert.IsAssignableFrom<Control>(control);
            Assert.Equal(entry.CanonicalId, type!.GetProperty("CanonicalId")?.GetValue(control));
        }
    }

    [Fact]
    public void Gallery_uses_real_XYUI1_component_instances_for_all_entries()
    {
        var gallery = XYUI1GalleryCatalog.Build();
        Assert.Equal(24, gallery.Count);
        Assert.All(gallery, item =>
        {
            Assert.NotNull(item.Preview);
            Assert.Contains(item.AvaloniaType, item.Usage);
            Assert.NotEqual("TextBlock", item.Preview.GetType().Name);
        });
    }

    static XyuiCatalogEntry[] Entries() => XyuiCatalogSource.Load(FindRoot())
        .Where(x => x.Module == "XYUI-1").ToArray();

    static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "xyui", "specs", "XYUI1", "XYUI-1.mapping.json"))) return directory.FullName;
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("XYUI repository root not found");
    }
}
