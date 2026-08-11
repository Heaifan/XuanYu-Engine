using System.IO;
using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

// MAP-DOC-A-R1-F1：Manifest ID 的 Text / Tooltip / Copy 权威源与即时刷新合同。
public sealed class UiMapManifestIdentityTests : IDisposable
{
    readonly string _directory = Path.Combine(Path.GetTempPath(), "xy-map-id-f1-" + Guid.NewGuid().ToString("N"));
    readonly MapManifestStorageService _storage = new();

    public UiMapManifestIdentityTests() => Directory.CreateDirectory(_directory);

    string PathFor(string id) => Path.Combine(_directory, id, "map.json");

    [Fact]
    public async Task Open_sequence_refreshes_display_id_immediately()
    {
        await Save("manifest-a", "A");
        await Save("manifest-b", "B");
        await Save("manifest-c", "C");
        var vm = new UiVm(null, () => true);

        foreach (var id in new[] { "manifest-a", "manifest-b", "manifest-c", "manifest-b" })
        {
            Assert.True(await vm.OpenMapManifestAsync(PathFor(id)));
            Assert.Equal(id, vm.MapIdText);
            Assert.Equal(MapIdDisplayFormat.Format(id), vm.MapIdDisplay);
            Assert.Equal(id, vm.CurrentMapManifest.Id);
        }
    }

    [Fact]
    public async Task Save_and_save_as_keep_the_current_id()
    {
        await Save("manifest-a", "A");
        var vm = new UiVm(null, () => true);
        Assert.True(await vm.OpenMapManifestAsync(PathFor("manifest-a")));
        var before = vm.CurrentMapManifest.Id;

        Assert.True(await vm.SaveMapManifestAsync(PathFor("saved-a")));
        Assert.True(await vm.SaveMapManifestAsync(PathFor("saved-b")));

        Assert.Equal(before, vm.MapIdText);
        Assert.Equal(before, vm.CurrentMapManifest.Id);
        Assert.Equal(before, (await _storage.LoadAsync(PathFor("saved-b"))).Value!.Id);
    }

    [Fact]
    public void Id_row_reserves_copy_button_width()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Right", "MapPagePanel.axaml");
        var text = File.ReadAllText(path);

        Assert.Contains("ColumnDefinitions=\"*,Auto\"", text);
        Assert.Contains("Grid.Column=\"1\" Classes=\"copyBtn\"", text);
        Assert.Contains("ToolTip.Tip=\"{Binding MapIdText}\"", text);
    }

    async Task Save(string id, string name)
    {
        var result = await _storage.SaveAsync(PathFor(id), MapManifest.CreateNew(id, name));
        Assert.True(result.Succeeded);
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_directory)) Directory.Delete(_directory, true); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }
}
