using XuanYu.Engine.Project.World.Documents;
using XuanYu.Engine.Project.World.SaveLoad;

namespace XuanYu.Engine.Tests.Project.World;

public sealed class WorldDocumentWriterTests
{
    static readonly WorldDocument s_validDoc = new(
        1, "test_world", "测试世界",
        [new WorldEntityDocument("e1", "实体 1",
            [new TransformComponentDocument
            {
                Position = new WorldVector3Document(10f, 20f, 30f),
                RotationDegrees = new WorldVector3Document(0f, 45f, 0f),
                Scale = new WorldVector3Document(2f, 2f, 2f),
            }])],
        WorldMetadataDocument.Default);

    [Fact]
    public void Write_ValidDocument_ShouldSucceed()
    {
        var path = Path.GetTempFileName();
        try
        {
            var result = WorldDocumentWriter.Write(path, s_validDoc);
            Assert.True(result.IsSuccess);
            Assert.Null(result.ErrorMessage);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Write_AfterWrite_FileExists()
    {
        var path = Path.GetTempFileName();
        try
        {
            WorldDocumentWriter.Write(path, s_validDoc);
            Assert.True(File.Exists(path));
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Write_JsonContainsSchemaVersion()
    {
        var path = Path.GetTempFileName();
        try
        {
            WorldDocumentWriter.Write(path, s_validDoc);
            var json = File.ReadAllText(path);
            Assert.Contains("\"schemaVersion\"", json);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Write_JsonContainsEntityId()
    {
        var path = Path.GetTempFileName();
        try
        {
            WorldDocumentWriter.Write(path, s_validDoc);
            var json = File.ReadAllText(path);
            Assert.Contains("\"e1\"", json);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Write_ToNonExistentDirectory_CreatesDirectory()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var path = Path.Combine(dir, "sub", "test.world.json");
        try
        {
            var result = WorldDocumentWriter.Write(path, s_validDoc);
            Assert.True(result.IsSuccess);
            Assert.True(File.Exists(path));
        }
        finally { if (Directory.Exists(dir)) Directory.Delete(dir, true); }
    }

    [Fact]
    public void Write_JsonContainsRotationDegrees()
    {
        var path = Path.GetTempFileName();
        try
        {
            WorldDocumentWriter.Write(path, s_validDoc);
            var json = File.ReadAllText(path);
            Assert.Contains("\"rotationDegrees\"", json);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void Write_JsonContainsScale()
    {
        var path = Path.GetTempFileName();
        try
        {
            WorldDocumentWriter.Write(path, s_validDoc);
            var json = File.ReadAllText(path);
            Assert.Contains("\"scale\"", json);
        }
        finally { File.Delete(path); }
    }
}
