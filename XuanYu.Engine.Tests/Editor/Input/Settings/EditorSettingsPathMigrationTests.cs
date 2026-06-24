using XuanYu.Engine.Editor.Input.Settings;

namespace XuanYu.Engine.Tests.Editor.Input.Settings;

/// <summary>
/// 测试 EditorSettingsPathMigration 的目录迁移逻辑。
/// 使用临时目录模拟 %APPDATA%，不触碰真实用户目录。
/// </summary>
public sealed class EditorSettingsPathMigrationTests
{
    [Fact]
    public void NewDirExists_SkipsMigration()
    {
        using var temp = new TempDir();
        var legacyDir = Path.Combine(temp.Root, "FluidWarfare", "Editor");
        var newDir = Path.Combine(temp.Root, "XuanYuEngine", "Editor");
        Directory.CreateDirectory(newDir);
        File.WriteAllText(Path.Combine(newDir, "editor.settings.json"), "existing");

        // Create legacy files too
        Directory.CreateDirectory(legacyDir);
        File.WriteAllText(Path.Combine(legacyDir, "editor.settings.json"), "legacy");

        EditorSettingsPathMigration.MigrateIfNeeded(newDir, legacyDir);

        // New file should NOT be overwritten
        var content = File.ReadAllText(Path.Combine(newDir, "editor.settings.json"));
        Assert.Equal("existing", content);
    }

    [Fact]
    public void LegacyDirExists_CopiesToNewDir()
    {
        using var temp = new TempDir();
        var legacyDir = Path.Combine(temp.Root, "FluidWarfare", "Editor");
        var newDir = Path.Combine(temp.Root, "XuanYuEngine", "Editor");

        Directory.CreateDirectory(legacyDir);
        File.WriteAllText(Path.Combine(legacyDir, "editor.settings.json"), "{\"schemaVersion\":1}");

        EditorSettingsPathMigration.MigrateIfNeeded(newDir, legacyDir);

        Assert.True(Directory.Exists(newDir));
        var content = File.ReadAllText(Path.Combine(newDir, "editor.settings.json"));
        Assert.Equal("{\"schemaVersion\":1}", content);
    }

    [Fact]
    public void NeitherDirExists_DoesNothing()
    {
        using var temp = new TempDir();
        var legacyDir = Path.Combine(temp.Root, "FluidWarfare", "Editor");
        var newDir = Path.Combine(temp.Root, "XuanYuEngine", "Editor");

        EditorSettingsPathMigration.MigrateIfNeeded(newDir, legacyDir);

        Assert.False(Directory.Exists(newDir));
        Assert.False(Directory.Exists(legacyDir));
    }

    [Fact]
    public void LegacyDirExists_DoesNotDeleteLegacy()
    {
        using var temp = new TempDir();
        var legacyDir = Path.Combine(temp.Root, "FluidWarfare", "Editor");
        var newDir = Path.Combine(temp.Root, "XuanYuEngine", "Editor");

        Directory.CreateDirectory(legacyDir);
        File.WriteAllText(Path.Combine(legacyDir, "editor.settings.json"), "stay");

        EditorSettingsPathMigration.MigrateIfNeeded(newDir, legacyDir);

        Assert.True(Directory.Exists(legacyDir));
        Assert.True(File.Exists(Path.Combine(legacyDir, "editor.settings.json")));
    }

    [Fact]
    public void MigrationFailure_DoesNotThrow()
    {
        // Simulate failure by passing an invalid path
        EditorSettingsPathMigration.MigrateIfNeeded(
            "\\\\invalid\\path\\new",
            "\\\\invalid\\path\\legacy");

        // Should not throw — migration failures are silently caught
        Assert.True(true);
    }
}

/// <summary>临时目录辅助，确保测试之间不互相影响。</summary>
public sealed class TempDir : IDisposable
{
    public string Root { get; } = Path.Combine(
        Path.GetTempPath(), $"XuanYuEngineTest_{Guid.NewGuid():N}");

    public TempDir() => Directory.CreateDirectory(Root);

    public void Dispose()
    {
        try { Directory.Delete(Root, recursive: true); } catch { }
    }
}
