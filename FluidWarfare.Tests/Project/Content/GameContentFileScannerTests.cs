using FluidWarfare.Project.Content;

namespace FluidWarfare.Tests.Project.Content;

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

        var result = GameContentFileScanner.Scan(scope.Path, folders, out var files);

        Assert.True(result.IsSuccess);
        Assert.Single(files);
        Assert.Equal("icons", files[0].FolderName);
        Assert.Equal("icon", files[0].ContentKind);
        Assert.Equal("sample_icon.svg", files[0].FileName);
        Assert.Equal("icons/sample_icon.svg", files[0].RelativePath);
        Assert.Equal(".svg", files[0].Extension);
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

        var result = GameContentFileScanner.Scan(scope.Path, folders, out var files);

        Assert.True(result.IsSuccess);
        Assert.Empty(files);
    }

    [Fact]
    public void Scan_WithDisallowedExtension_ShouldFail()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("icons");
        scope.CreateContentFile("icons", "sample_icon.psd");

        var folders = new List<GameContentFolderInfo>
        {
            new("icons", "图标", "图标说明。", "icon", false, [".svg", ".png"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders, out var files);

        Assert.True(result.IsFailure);
        Assert.Equal("Project.ContentFileExtensionNotAllowed", result.Error?.Code);
        Assert.Empty(files);
    }

    [Fact]
    public void Scan_WithNestedDirectory_ShouldFail()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("units");
        scope.CreateNestedSubdirectory("units", "infantry");

        var folders = new List<GameContentFolderInfo>
        {
            new("units", "单位", "单位说明。", "unitTemplate", true, [".json"])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders, out var files);

        Assert.True(result.IsFailure);
        Assert.Equal("Project.NestedContentDirectoryUnsupported", result.Error?.Code);
        Assert.Empty(files);
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

        var result = GameContentFileScanner.Scan(scope.Path, folders, out var files);

        Assert.True(result.IsSuccess);
        Assert.Single(files);
        Assert.Equal(".svg", files[0].Extension);
    }

    [Fact]
    public void Scan_WithEmptyAllowedExtensionsAndRegularFile_ShouldFail()
    {
        using var scope = TestContentDirectory.Create();
        scope.CreateContentFolder("icons");
        scope.CreateContentFile("icons", "sample_icon.svg");

        var folders = new List<GameContentFolderInfo>
        {
            new("icons", "图标", "图标说明。", "icon", false, [])
        };

        var result = GameContentFileScanner.Scan(scope.Path, folders, out var files);

        Assert.True(result.IsFailure);
        Assert.Equal("Project.ContentFileExtensionNotAllowed", result.Error?.Code);
        Assert.Empty(files);
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

        var result = GameContentFileScanner.Scan(scope.Path, folders, out var files);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, files.Count);

        var unitFile = files.First(f => f.FolderName == "units");
        Assert.Equal("sample_unit.json", unitFile.FileName);
        Assert.Equal("units/sample_unit.json", unitFile.RelativePath);
        Assert.Equal(".json", unitFile.Extension);

        var iconFile = files.First(f => f.FolderName == "icons");
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

        var result = GameContentFileScanner.Scan(scope.Path, folders, out var files);

        Assert.True(result.IsSuccess);
        Assert.Empty(files);
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
                $"FluidWarfareContentScanTests_{Guid.NewGuid():N}");

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
