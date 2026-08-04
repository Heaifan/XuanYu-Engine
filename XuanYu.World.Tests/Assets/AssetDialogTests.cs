using XuanYu.Editor.UI;
using XuanYu.World.Tests.World;

namespace XuanYu.World.Tests.Assets;

public sealed class AssetDialogTests : IDisposable
{
    readonly ScenePersistenceEnv _env = new();

    [Fact]
    public async Task Damaged_glb_import_shows_one_error_dialog()
    {
        var vm = NewVm(_env.Dialogs);
        var bad = _env.NewGlb("damaged_truncated", [1, 2, 3]);

        var ok = vm.ImportStaticModel(bad);

        Assert.False(ok);
        Assert.Single(_env.Dialogs.Shown);
        Assert.Equal("导入 GLB 失败", _env.Dialogs.Shown[0].Title);
    }

    [Fact]
    public void Successful_import_shows_no_dialog()
    {
        var vm = NewVm(_env.Dialogs);
        var glb = _env.NewGlb("good", GlbFactory.Triangle());

        var ok = vm.ImportStaticModel(glb);

        Assert.True(ok);
        Assert.Empty(_env.Dialogs.Shown);
    }

    [Fact]
    public async Task Invalid_scene_open_shows_one_error_dialog()
    {
        var vm = NewVm(_env.Dialogs);
        var scenePath = _env.NewScenePath();
        await File.WriteAllTextAsync(scenePath, "not json");

        var ok = await vm.OpenSceneAsync(scenePath);

        Assert.False(ok);
        Assert.Single(_env.Dialogs.Shown);
        Assert.Equal("打开场景失败", _env.Dialogs.Shown[0].Title);
    }

    [Fact]
    public async Task Successful_open_shows_no_dialog()
    {
        var vm = NewVm(_env.Dialogs);
        var glb = _env.NewGlb("soldier", GlbFactory.Triangle());
        vm.ImportStaticModel(glb);
        var scenePath = _env.NewScenePath();
        Assert.True(await vm.SaveSceneAsync(scenePath));
        _env.Dialogs.Shown.Clear();

        var ok = await vm.OpenSceneAsync(scenePath);

        Assert.True(ok);
        Assert.Empty(_env.Dialogs.Shown);
    }

    [Fact]
    public async Task Open_with_missing_asset_shows_one_summary_warning()
    {
        var vm = NewVm(_env.Dialogs);
        var glb = _env.NewGlb("soldier", GlbFactory.Triangle());
        vm.ImportStaticModel(glb);
        var scenePath = _env.NewScenePath();
        // 走 UiVm 内部 Catalog 保存（保存时 SourcePath 改绑为托管路径）。
        Assert.True(await vm.SaveSceneAsync(scenePath));
        var hosted = Directory.GetFiles(Path.Combine(_env.Dir, "Battle01.xyassets"),
            "source.glb", SearchOption.AllDirectories).Single();
        File.Delete(hosted);
        _env.Dialogs.Shown.Clear();

        var ok = await vm.OpenSceneAsync(scenePath);

        Assert.True(ok);
        Assert.Single(_env.Dialogs.Shown);
        Assert.Equal("场景已打开，但部分资源不可用", _env.Dialogs.Shown[0].Title);
    }

    static UiVm NewVm(FakeDialogService dialogs) =>
        new(null, () => true, seedInitialScene: false, dialogService: dialogs);

    public void Dispose() => _env.Dispose();
}
