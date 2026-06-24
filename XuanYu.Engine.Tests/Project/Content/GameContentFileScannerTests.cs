using XuanYu.Engine.Project.Content;

namespace XuanYu.Engine.Tests.Project.Content;

public sealed class GameContentFileScannerTests
{
    [Fact]
    public void Scan_WithAllowedExtension_ShouldReturnContentFile()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("icons");
        scope.CreateContentFile("icons", "sample_icon.svg");

        var folders = new List<GameContentFolderInfo>
        {
            new("icons", "图标", "图标说明。", "icon", false, [".svg"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders);

        Assert.Single(result.ContentFiles);
        Assert.False(result.HasIssues);
        Assert.Equal("icons", result.ContentFiles[0].FolderName);
        Assert.Equal("icon", result.ContentFiles[0].ContentKind);
        Assert.Equal("sample_icon.svg", result.ContentFiles[0].FileName);
        Assert.Equal("icons/sample_icon.svg", result.ContentFiles[0].RelativePath);
        Assert.Equal(".svg", result.ContentFiles[0].Extension);
    }

    [Fact]
    public void Scan_WithGitKeepOnly_ShouldReturnEmptyList()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("icons");
        scope.CreateContentFile("icons", ".gitkeep");

        var folders = new List<GameContentFolderInfo>
        {
            new("icons", "图标", "图标说明。", "icon", false, [".svg"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders);

        Assert.Empty(result.ContentFiles);
        Assert.False(result.HasIssues);
    }

    [Fact]
    public void Scan_WithDisallowedExtension_ShouldCollectIssue()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("icons");
        scope.CreateContentFile("icons", "sample_icon.psd");

        var folders = new List<GameContentFolderInfo>
        {
            new("icons", "图标", "图标说明。", "icon", false, [".svg", ".png"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders);

        Assert.Empty(result.ContentFiles);
        Assert.True(result.HasIssues);
        Assert.Equal("Project.ContentFileExtensionNotAllowed", result.Issues[0].Code);
    }

    [Fact]
    public void Scan_WithNestedDirectory_ShouldCollectIssue()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("units");
        scope.CreateNestedSubdirectory("units", "infantry");

        var folders = new List<GameContentFolderInfo>
        {
            new("units", "单位", "单位说明。", "unitTemplate", true, [".json"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders);

        Assert.Empty(result.ContentFiles);
        Assert.True(result.HasIssues);
        Assert.Equal("Project.NestedContentDirectoryUnsupported", result.Issues[0].Code);
    }

    [Fact]
    public void Scan_WithUppercaseExtension_ShouldNormalizeToLowercase()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("icons");
        scope.CreateContentFile("icons", "RIFLE.SVG");

        var folders = new List<GameContentFolderInfo>
        {
            new("icons", "图标", "图标说明。", "icon", false, [".svg"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders);

        Assert.Single(result.ContentFiles);
        Assert.False(result.HasIssues);
        Assert.Equal(".svg", result.ContentFiles[0].Extension);
    }

    [Fact]
    public void Scan_WithEmptyAllowedExtensionsAndRegularFile_ShouldCollectIssue()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("icons");
        scope.CreateContentFile("icons", "sample_icon.svg");

        var folders = new List<GameContentFolderInfo>
        {
            new("icons", "图标", "图标说明。", "icon", false, [])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders);

        Assert.Empty(result.ContentFiles);
        Assert.True(result.HasIssues);
        Assert.Equal("Project.ContentFileExtensionNotAllowed", result.Issues[0].Code);
    }

    [Fact]
    public void Scan_WithMultipleFolders_ShouldReturnFilesFromAllFolders()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("units");
        scope.CreateContentFile("units", "sample_unit.json");
        scope.CreateContentFolder("icons");
        scope.CreateContentFile("icons", "sample_icon.svg");

        var folders = new List<GameContentFolderInfo>
        {
            new("units", "单位", "单位说明。", "unitTemplate", true, [".json"]),
            new("icons", "图标", "图标说明。", "icon", false, [".svg"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders);

        Assert.False(result.HasIssues);
        Assert.Equal(2, result.ContentFiles.Count);

        var unitFile = result.ContentFiles.First(f => f.FolderName == "units");
        Assert.Equal("sample_unit.json", unitFile.FileName);
        Assert.Equal("units/sample_unit.json", unitFile.RelativePath);
        Assert.Equal(".json", unitFile.Extension);

        var iconFile = result.ContentFiles.First(f => f.FolderName == "icons");
        Assert.Equal("sample_icon.svg", iconFile.FileName);
        Assert.Equal("icons/sample_icon.svg", iconFile.RelativePath);
        Assert.Equal(".svg", iconFile.Extension);
    }

    [Fact]
    public void Scan_WithHiddenFile_ShouldIgnore()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("icons");
        scope.CreateContentFile("icons", ".hidden.svg");

        var folders = new List<GameContentFolderInfo>
        {
            new("icons", "图标", "图标说明。", "icon", false, [".svg"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders);

        Assert.Empty(result.ContentFiles);
        Assert.False(result.HasIssues);
    }

    [Fact]
    public void Scan_WithMultipleInvalidFiles_ShouldCollectAllIssues()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("icons");
        scope.CreateContentFile("icons", "icon_a.psd");
        scope.CreateContentFile("icons", "icon_b.txt");
        scope.CreateContentFile("icons", "icon_c.bmp");

        var folders = new List<GameContentFolderInfo>
        {
            new("icons", "图标", "图标说明。", "icon", false, [".png", ".svg"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders);

        Assert.Empty(result.ContentFiles);
        Assert.True(result.HasIssues);
        Assert.Equal(3, result.Issues.Count);
        Assert.All(result.Issues, i => Assert.Equal("Project.ContentFileExtensionNotAllowed", i.Code));
    }

    [Fact]
    public void Scan_WithValidAndInvalidFiles_ShouldReturnBoth()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("icons");
        scope.CreateContentFile("icons", "good.svg");
        scope.CreateContentFile("icons", "bad.psd");
        scope.CreateContentFile("icons", "also_bad.txt");

        var folders = new List<GameContentFolderInfo>
        {
            new("icons", "图标", "图标说明。", "icon", false, [".svg"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders);

        Assert.Single(result.ContentFiles);
        Assert.Equal("good.svg", result.ContentFiles[0].FileName);
        Assert.Equal(2, result.Issues.Count);
    }

    [Fact]
    public void Scan_WithNestedDirectoryAndInvalidExtension_ShouldCollectBothIssues()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("units");
        scope.CreateContentFile("units", "good_unit.json");
        scope.CreateContentFile("units", "bad.txt");
        scope.CreateNestedSubdirectory("units", "infantry");

        var folders = new List<GameContentFolderInfo>
        {
            new("units", "单位", "单位说明。", "unitTemplate", true, [".json"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders);

        // 合法文件仍然返回
        Assert.Single(result.ContentFiles);
        Assert.Equal("good_unit.json", result.ContentFiles[0].FileName);

        // 两个问题：嵌套目录和非法扩展名
        Assert.Equal(2, result.Issues.Count);
        Assert.Contains(result.Issues, i => i.Code == "Project.NestedContentDirectoryUnsupported");
        Assert.Contains(result.Issues, i => i.Code == "Project.ContentFileExtensionNotAllowed");
    }

    private sealed class TestContentDirectory : IDisposable
    {
        private TestContentDirectory(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public static TestContentDirectory Create()
        {
            var path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                $"XuanYuEngineContentScanTests_{Guid.NewGuid():N}");

            Directory.CreateDirectory(path);
            return new TestContentDirectory(path);
        }

        public void CreateContentFolder(string folderName)
        {
            Directory.CreateDirectory(System.IO.Path.Combine(Path, folderName));
        }

        public void CreateContentFile(string folderName, string fileName)
        {
            var filePath = System.IO.Path.Combine(Path, folderName, fileName);
            var dir = System.IO.Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir!);
            }

            File.WriteAllText(filePath, string.Empty);
        }

        public void CreateNestedSubdirectory(string parentFolder, string subDirName)
        {
            Directory.CreateDirectory(System.IO.Path.Combine(Path, parentFolder, subDirName));
        }

        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
    }
}
