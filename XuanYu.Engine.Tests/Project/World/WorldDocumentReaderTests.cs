using XuanYu.Engine.Project.World.Documents;
using XuanYu.Engine.Project.World.SaveLoad;

namespace XuanYu.Engine.Tests.Project.World;

public sealed class WorldDocumentReaderTests
{
    [Fact]
    public void Read_ValidFile_ShouldReturnDocument()
    {
        var path = Path.GetTempFileName();
        try
        {
            var doc = new WorldDocument(1, "w1", "世界",
                [new WorldEntityDocument("e1", "实体",
                    [new TransformComponentDocument { Position = WorldVector3Document.Zero }])],
                WorldMetadataDocument.Default);
            WorldDocumentWriter.Write(path, doc);

            var result = WorldDocumentReader.Read(path);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Document);
            Assert.Equal("w1", result.Document.WorldId);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Read_FileNotFound_ReturnsFail()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".world.json");
        var result = WorldDocumentReader.Read(path);
        Assert.False(result.IsSuccess);
        Assert.Null(result.Document);
        Assert.Contains("不存在", result.ErrorMessage);
    }

    [Fact]
    public void Read_CorruptedJson_ReturnsFail()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "{ invalid json }");
            var result = WorldDocumentReader.Read(path);
            Assert.False(result.IsSuccess);
            Assert.Contains("JSON 格式错误", result.ErrorMessage);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Read_EmptyFilePath_ReturnsFail()
    {
        var result = WorldDocumentReader.Read("");
        Assert.False(result.IsSuccess);
        Assert.Contains("路径", result.ErrorMessage);
    }

    [Fact]
    public void Read_NullDocumentJson_ReturnsFail()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "null");
            var result = WorldDocumentReader.Read(path);
            Assert.False(result.IsSuccess);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Read_OldFormatOnlyPosition_ShouldFillDefaults()
    {
        var path = Path.GetTempFileName();
        try
        {
            // 模拟 9.0A 旧格式：只有 position，没有 rotationDegrees / scale
            var oldJson = @"{
                ""schemaVersion"": 1,
                ""worldId"": ""old_world"",
                ""displayName"": ""旧世界"",
                ""entities"": [
                    {
                        ""entityId"": ""e1"",
                        ""displayName"": ""旧实体"",
                        ""components"": [
                            {
                                ""componentType"": ""Transform"",
                                ""position"": { ""x"": 1, ""y"": 2, ""z"": 3 }
                            }
                        ]
                    }
                ],
                ""metadata"": { ""createdBy"": ""旧版"", ""note"": ""旧格式"" }
            }";
            File.WriteAllText(path, oldJson);
            var result = WorldDocumentReader.Read(path);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Document);
            Assert.Single(result.Document.Entities);

            var t = result.Document.Entities[0].Components
                .OfType<TransformComponentDocument>().First();

            // Position 应正常读取
            Assert.Equal(1f, t.GetPositionOrDefault().X);
            Assert.Equal(3f, t.GetPositionOrDefault().Z);

            // RotationDegrees 缺失 → 默认 (0,0,0)
            Assert.Equal(0f, t.GetRotationDegreesOrDefault().X);

            // Scale 缺失 → 默认 (1,1,1)
            Assert.Equal(1f, t.GetScaleOrDefault().X);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Read_NewFormatFullTransform_ReturnsAllFields()
    {
        var path = Path.GetTempFileName();
        try
        {
            // 9.0B 新格式：包含完整 transform
            var newJson = @"{
                ""schemaVersion"": 1,
                ""worldId"": ""new_world"",
                ""displayName"": ""新世界"",
                ""entities"": [
                    {
                        ""entityId"": ""e1"",
                        ""displayName"": ""新实体"",
                        ""components"": [
                            {
                                ""componentType"": ""Transform"",
                                ""position"": { ""x"": 10, ""y"": 20, ""z"": 30 },
                                ""rotationDegrees"": { ""x"": 0, ""y"": 45, ""z"": 90 },
                                ""scale"": { ""x"": 2, ""y"": 2, ""z"": 0.5 }
                            }
                        ]
                    }
                ],
                ""metadata"": { ""createdBy"": ""新版"", ""note"": ""完整格式"" }
            }";
            File.WriteAllText(path, newJson);
            var result = WorldDocumentReader.Read(path);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Document);

            var t = result.Document.Entities[0].Components
                .OfType<TransformComponentDocument>().First();

            Assert.Equal(10f, t.GetPositionOrDefault().X);
            Assert.Equal(45f, t.GetRotationDegreesOrDefault().Y);
            Assert.Equal(0.5f, t.GetScaleOrDefault().Z);
        }
        finally { File.Delete(path); }
    }
}
