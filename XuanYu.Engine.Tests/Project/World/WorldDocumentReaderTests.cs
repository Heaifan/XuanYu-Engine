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
                    [new TransformComponentDocument(WorldVector3Document.Zero)])],
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
}
