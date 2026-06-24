using XuanYu.Engine.Project.Loading;

namespace XuanYu.Engine.Tests.Project.Loading;

public sealed class GameProjectLoaderTests
{
    [Fact]
    public void LoadFromDirectory_WithValidDeclaredFolders_ShouldSucceed()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.CreateContentDirectory("units");
        scope.CreateContentDirectory("icons");
        scope.WriteManifest(CreateManifest("""
            {
              "folderName": "units",
              "displayName": "单位",
              "description": "保存项目中的单位模板。",
              "contentKind": "unitTemplate",
              "isRequired": true,
              "allowedExtensions": [ ".json" ]
            },
            {
              "folderName": "icons",
              "displayName": "图标",
              "description": "保存项目中的界面图标资源。",
              "contentKind": "icon",
              "isRequired": false,
              "allowedExtensions": [ ".png", ".svg", ".webp" ]
            }
            """));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        Assert.True(result.Result.IsSuccess);
        Assert.NotNull(result.Project);
        Assert.Equal("test_project", result.Project.ProjectId);
        Assert.Equal(1, result.Project.SchemaVersion);
        Assert.Equal("TestProject", result.Project.DisplayName);
        Assert.Equal(2, result.Project.ContentFolders.Count);
        Assert.Equal("units", result.Project.ContentFolders[0].FolderName);
        Assert.Equal("单位", result.Project.ContentFolders[0].DisplayName);
        Assert.Equal("unitTemplate", result.Project.ContentFolders[0].ContentKind);
        Assert.True(result.Project.ContentFolders[0].IsRequired);
        Assert.Equal([".json"], result.Project.ContentFolders[0].AllowedExtensions);
        Assert.Equal("icons", result.Project.ContentFolders[1].FolderName);
        Assert.Equal([".png", ".svg", ".webp"], result.Project.ContentFolders[1].AllowedExtensions);
        Assert.False(result.ValidationReport.HasIssues);
    }

    [Fact]
    public void LoadFromDirectory_WithMissingDirectory_ShouldFail()
    {
        var missingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

        var result = GameProjectLoader.LoadFromDirectory(missingPath);

        AssertFailure(result, "Project.DirectoryMissing");
    }

    [Fact]
    public void LoadFromDirectory_WithMissingManifest_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ManifestMissing");
    }

    [Fact]
    public void LoadFromDirectory_WithInvalidJson_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest("{ invalid json");

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ManifestInvalid");
    }

    [Fact]
    public void LoadFromDirectory_WithMissingProjectId_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest("""
            {
              "schemaVersion": 1,
              "displayName": "TestProject",
              "contentFolders": []
            }
            """);

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ProjectIdMissing");
    }

    [Fact]
    public void LoadFromDirectory_WithMissingDisplayName_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest("""
            {
              "schemaVersion": 1,
              "projectId": "test_project",
              "contentFolders": []
            }
            """);

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.DisplayNameMissing");
    }

    [Fact]
    public void LoadFromDirectory_WithOldStringFolderFormat_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest("""
            {
              "schemaVersion": 1,
              "projectId": "test_project",
              "displayName": "TestProject",
              "contentFolders": [ "units" ]
            }
            """);

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ContentFolderInvalid");
    }

    [Fact]
    public void LoadFromDirectory_WithMissingContentFolders_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest("""
            {
              "schemaVersion": 1,
              "projectId": "test_project",
              "displayName": "TestProject"
            }
            """);

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ContentFoldersMissing");
    }

    [Fact]
    public void LoadFromDirectory_WithMissingSchemaVersion_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest("""
            {
              "projectId": "test_project",
              "displayName": "TestProject",
              "contentFolders": []
            }
            """);

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.SchemaVersionMissing");
    }

    [Fact]
    public void LoadFromDirectory_WithUnsupportedSchemaVersion_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest("""
            {
              "schemaVersion": 999,
              "projectId": "test_project",
              "displayName": "TestProject",
              "contentFolders": []
            }
            """);

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.SchemaVersionUnsupported");
    }

    [Fact]
    public void LoadFromDirectory_WithMissingFolderName_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest(CreateManifest("""
            {
              "displayName": "单位",
              "description": "保存项目中的单位模板。",
              "contentKind": "unitTemplate",
              "isRequired": true,
              "allowedExtensions": [ ".json" ]
            }
            """));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ContentFolderNameMissing");
    }

    [Fact]
    public void LoadFromDirectory_WithMissingDisplayNameInContentFolder_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest(CreateManifest("""
            {
              "folderName": "units",
              "description": "保存项目中的单位模板。",
              "contentKind": "unitTemplate",
              "isRequired": true,
              "allowedExtensions": [ ".json" ]
            }
            """));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ContentFolderDisplayNameMissing");
    }

    [Fact]
    public void LoadFromDirectory_WithMissingDescription_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest(CreateManifest("""
            {
              "folderName": "units",
              "displayName": "单位",
              "contentKind": "unitTemplate",
              "isRequired": true,
              "allowedExtensions": [ ".json" ]
            }
            """));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ContentFolderDescriptionMissing");
    }

    [Fact]
    public void LoadFromDirectory_WithMissingContentKind_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest(CreateManifest("""
            {
              "folderName": "units",
              "displayName": "单位",
              "description": "保存项目中的单位模板。",
              "isRequired": true,
              "allowedExtensions": [ ".json" ]
            }
            """));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ContentFolderKindMissing");
    }

    [Fact]
    public void LoadFromDirectory_WithInvalidFolderName_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest(CreateSingleFolderManifest("Units", "unitTemplate", ".json"));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ContentFolderNameInvalid");
    }

    [Fact]
    public void LoadFromDirectory_WithInvalidContentKind_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest(CreateSingleFolderManifest("units", "icon/type", ".json"));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ContentKindInvalid");
    }

    [Fact]
    public void LoadFromDirectory_WithInvalidAllowedExtension_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest(CreateSingleFolderManifest("units", "unitTemplate", "json"));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.AllowedExtensionInvalid");
    }

    [Fact]
    public void LoadFromDirectory_WithDuplicatedFolderName_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.CreateContentDirectory("units");
        scope.WriteManifest(CreateManifest("""
            {
              "folderName": "units",
              "displayName": "单位",
              "description": "保存项目中的单位模板。",
              "contentKind": "unitTemplate",
              "isRequired": true,
              "allowedExtensions": [ ".json" ]
            },
            {
              "folderName": "units",
              "displayName": "重复单位",
              "description": "重复声明。",
              "contentKind": "unitTemplate",
              "isRequired": true,
              "allowedExtensions": [ ".json" ]
            }
            """));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ContentFolderDuplicated");
    }

    [Fact]
    public void LoadFromDirectory_WithDeclaredButMissingDirectory_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.WriteManifest(CreateSingleFolderManifest("icons", "icon", ".png"));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ContentFolderDirectoryMissing");
    }

    [Fact]
    public void LoadFromDirectory_WithUndeclaredDirectory_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.CreateContentDirectory("units");
        scope.CreateContentDirectory("assets");
        scope.WriteManifest(CreateSingleFolderManifest("units", "unitTemplate", ".json"));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.UndeclaredContentFolder");
    }

    [Fact]
    public void LoadFromDirectory_WithValidContentFiles_ShouldSucceedAndExposeContentFiles()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.CreateContentDirectory("units");
        scope.CreateContentDirectory("icons");
        scope.CreateContentFile("units", "sample_unit.json");
        scope.CreateContentFile("icons", "sample_icon.svg");
        scope.WriteManifest(CreateManifest("""
            {
              "folderName": "units",
              "displayName": "单位",
              "description": "保存项目中的单位模板。",
              "contentKind": "unitTemplate",
              "isRequired": true,
              "allowedExtensions": [ ".json" ]
            },
            {
              "folderName": "icons",
              "displayName": "图标",
              "description": "保存项目中的界面图标资源。",
              "contentKind": "icon",
              "isRequired": false,
              "allowedExtensions": [ ".png", ".svg", ".webp" ]
            }
            """));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        Assert.True(result.Result.IsSuccess);
        Assert.NotNull(result.Project);
        Assert.Equal(2, result.Project.ContentFiles.Count);

        var unitFile = result.Project.ContentFiles.First(f => f.FolderName == "units");
        Assert.Equal("sample_unit.json", unitFile.FileName);
        Assert.Equal("units/sample_unit.json", unitFile.RelativePath);
        Assert.Equal(".json", unitFile.Extension);
        Assert.Equal("unitTemplate", unitFile.ContentKind);

        var iconFile = result.Project.ContentFiles.First(f => f.FolderName == "icons");
        Assert.Equal("sample_icon.svg", iconFile.FileName);
        Assert.Equal("icons/sample_icon.svg", iconFile.RelativePath);
        Assert.Equal(".svg", iconFile.Extension);
        Assert.Equal("icon", iconFile.ContentKind);
    }

    [Fact]
    public void LoadFromDirectory_WithDisallowedContentFileExtension_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.CreateContentDirectory("icons");
        scope.CreateContentFile("icons", "sample_icon.psd");
        scope.WriteManifest(CreateSingleFolderManifest("icons", "icon", ".svg"));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.ContentFileExtensionNotAllowed");
    }

    [Fact]
    public void LoadFromDirectory_WithNestedContentDirectory_ShouldFail()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.CreateContentDirectory("units");
        scope.CreateNestedSubdirectory("units", "infantry");
        scope.WriteManifest(CreateSingleFolderManifest("units", "unitTemplate", ".json"));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        AssertFailure(result, "Project.NestedContentDirectoryUnsupported");
    }

    [Fact]
    public void LoadFromDirectory_WithMultipleProjectIssues_ShouldReturnValidationReport()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.CreateContentDirectory("units");
        scope.CreateContentDirectory("icons");
        scope.CreateContentFile("units", "good.json");
        scope.CreateContentFile("units", "bad.txt");
        scope.CreateContentFile("icons", "raw.psd");
        scope.CreateContentDirectory("backup");
        scope.CreateContentFile("backup", "old.json");
        scope.WriteManifest(CreateManifest("""
            {
              "folderName": "units",
              "displayName": "单位",
              "description": "保存项目中的单位模板。",
              "contentKind": "unitTemplate",
              "isRequired": true,
              "allowedExtensions": [ ".json" ]
            },
            {
              "folderName": "icons",
              "displayName": "图标",
              "description": "保存项目中的界面图标资源。",
              "contentKind": "icon",
              "isRequired": false,
              "allowedExtensions": [ ".png", ".svg", ".webp" ]
            }
            """));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        Assert.True(result.Result.IsFailure);
        Assert.Null(result.Project);
        Assert.True(result.ValidationReport.HasIssues);
        Assert.True(result.ValidationReport.IssueCount >= 3);

        // 应包含未声明目录
        Assert.Contains(result.ValidationReport.Issues,
            i => i.Code == "Project.UndeclaredContentFolder" && i.Path == "backup");

        // 应包含非法扩展名
        Assert.Contains(result.ValidationReport.Issues,
            i => i.Code == "Project.ContentFileExtensionNotAllowed" && i.Path == "units/bad.txt");

        Assert.Contains(result.ValidationReport.Issues,
            i => i.Code == "Project.ContentFileExtensionNotAllowed" && i.Path == "icons/raw.psd");

        // 第一个错误仍作为 EngineError
        Assert.Equal(result.ValidationReport.FirstIssue?.Code, result.Result.Error?.Code);
    }

    [Fact]
    public void LoadFromDirectory_WithUndeclaredDirectoryAndInvalidFile_ShouldReportBothIssues()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.CreateContentDirectory("units");
        scope.CreateContentFile("units", "good.json");
        scope.CreateContentFile("units", "bad.txt");
        scope.CreateContentDirectory("extra");
        scope.WriteManifest(CreateSingleFolderManifest("units", "unitTemplate", ".json"));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        Assert.True(result.Result.IsFailure);
        Assert.Equal(2, result.ValidationReport.IssueCount);
        Assert.Contains(result.ValidationReport.Issues, i => i.Code == "Project.UndeclaredContentFolder");
        Assert.Contains(result.ValidationReport.Issues, i => i.Code == "Project.ContentFileExtensionNotAllowed");
    }

    [Fact]
    public void LoadFromDirectory_WithValidationIssues_ShouldUseFirstIssueAsEngineError()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.CreateContentDirectory("units");
        scope.CreateContentFile("units", "bad.txt");
        scope.CreateContentDirectory("extra");
        scope.WriteManifest(CreateSingleFolderManifest("units", "unitTemplate", ".json"));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        Assert.True(result.Result.IsFailure);
        Assert.True(result.ValidationReport.IssueCount >= 2);
        Assert.Equal(result.ValidationReport.FirstIssue?.Code, result.Result.Error?.Code);
    }

    [Fact]
    public void LoadFromDirectory_WithValidProject_ShouldReturnEmptyValidationReport()
    {
        using var scope = ProjectTestDirectory.Create();
        scope.CreateContentDirectory("units");
        scope.CreateContentFile("units", "good.json");
        scope.WriteManifest(CreateSingleFolderManifest("units", "unitTemplate", ".json"));

        var result = GameProjectLoader.LoadFromDirectory(scope.Path);

        Assert.True(result.Result.IsSuccess);
        Assert.NotNull(result.Project);
        Assert.False(result.ValidationReport.HasIssues);
        Assert.Equal(0, result.ValidationReport.IssueCount);
    }

    private static void AssertFailure(GameProjectLoadResult result, string code)
    {
        Assert.True(result.Result.IsFailure);
        Assert.Null(result.Project);
        Assert.Equal(code, result.Result.Error?.Code);
        Assert.True(result.ValidationReport.HasIssues);
        Assert.Equal(code, result.ValidationReport.FirstIssue?.Code);
    }

    private static string CreateSingleFolderManifest(string folderName, string contentKind, string extension)
    {
        return CreateManifest($$"""
            {
              "folderName": "{{folderName}}",
              "displayName": "测试目录",
              "description": "测试目录说明。",
              "contentKind": "{{contentKind}}",
              "isRequired": true,
              "allowedExtensions": [ "{{extension}}" ]
            }
            """);
    }

    private static string CreateManifest(string contentFolderItems)
    {
        return $$"""
            {
              "schemaVersion": 1,
              "projectId": "test_project",
              "displayName": "TestProject",
              "description": "测试项目。",
              "contentFolders": [
                {{contentFolderItems}}
              ]
            }
            """;
    }

    private sealed class ProjectTestDirectory : IDisposable
    {
        private ProjectTestDirectory(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public static ProjectTestDirectory Create()
        {
            var path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                $"XuanYuEngineProjectTests_{Guid.NewGuid():N}");

            Directory.CreateDirectory(path);
            return new ProjectTestDirectory(path);
        }

        public void CreateContentDirectory(string name)
        {
            Directory.CreateDirectory(System.IO.Path.Combine(Path, name));
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

        public void WriteManifest(string json)
        {
            File.WriteAllText(System.IO.Path.Combine(Path, "game.project.json"), json);
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
