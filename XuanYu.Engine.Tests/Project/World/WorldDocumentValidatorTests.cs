using XuanYu.Engine.Project.World.Documents;
using XuanYu.Engine.Project.World.Validation;

namespace XuanYu.Engine.Tests.Project.World;

public sealed class WorldDocumentValidatorTests
{
    [Fact]
    public void Validate_ValidDocument_Passes()
    {
        var doc = new WorldDocument(1, "main", "主世界",
            [new WorldEntityDocument("e1", "实体 1",
                [new TransformComponentDocument(new WorldVector3Document(1f, 2f, 3f))])],
            WorldMetadataDocument.Default);

        var report = WorldDocumentValidator.Validate(doc);
        Assert.True(report.IsValid);
    }

    [Fact]
    public void Validate_EmptyDocument_PassesWithoutEntities()
    {
        var doc = new WorldDocument(1, "main", "空世界", [], WorldMetadataDocument.Default);
        var report = WorldDocumentValidator.Validate(doc);
        Assert.True(report.IsValid);
    }

    [Fact]
    public void Validate_SchemaVersionNot1_Fails()
    {
        var doc = new WorldDocument(0, "main", "世界", [], WorldMetadataDocument.Default);
        var report = WorldDocumentValidator.Validate(doc);
        Assert.Contains(report.Errors, e => e.Message.Contains("SchemaVersion"));
    }

    [Fact]
    public void Validate_WorldIdEmpty_Fails()
    {
        var doc = new WorldDocument(1, "", "世界", [], WorldMetadataDocument.Default);
        var report = WorldDocumentValidator.Validate(doc);
        Assert.Contains(report.Errors, e => e.Message.Contains("WorldId"));
    }

    [Fact]
    public void Validate_DisplayNameEmpty_Fails()
    {
        var doc = new WorldDocument(1, "main", "", [], WorldMetadataDocument.Default);
        var report = WorldDocumentValidator.Validate(doc);
        Assert.Contains(report.Errors, e => e.Message.Contains("DisplayName"));
    }

    [Fact]
    public void Validate_DuplicateEntityId_Fails()
    {
        var doc = new WorldDocument(1, "main", "世界",
            [
                new WorldEntityDocument("dup", "甲", []),
                new WorldEntityDocument("dup", "乙", []),
            ],
            WorldMetadataDocument.Default);

        var report = WorldDocumentValidator.Validate(doc);
        Assert.Contains(report.Errors, e => e.Message.Contains("重复"));
    }

    [Fact]
    public void Validate_EntityIdEmpty_Fails()
    {
        var doc = new WorldDocument(1, "main", "世界",
            [new WorldEntityDocument("", "无名", [])],
            WorldMetadataDocument.Default);

        var report = WorldDocumentValidator.Validate(doc);
        Assert.Contains(report.Errors, e => e.Message.Contains("EntityId"));
    }

    [Fact]
    public void Validate_NullEntities_Fails()
    {
        var doc = new WorldDocument(1, "main", "世界", null!, WorldMetadataDocument.Default);
        var report = WorldDocumentValidator.Validate(doc);
        Assert.Contains(report.Errors, e => e.Message.Contains("Entities"));
    }

    [Fact]
    public void Validate_NanPosition_Fails()
    {
        var doc = new WorldDocument(1, "main", "世界",
            [new WorldEntityDocument("e1", "实体",
                [new TransformComponentDocument(new WorldVector3Document(float.NaN, 0f, 0f))])],
            WorldMetadataDocument.Default);

        var report = WorldDocumentValidator.Validate(doc);
        Assert.Contains(report.Errors, e => e.Message.Contains("NaN") || e.Message.Contains("无效"));
    }

    [Fact]
    public void Validate_InfinityPosition_Fails()
    {
        var doc = new WorldDocument(1, "main", "世界",
            [new WorldEntityDocument("e1", "实体",
                [new TransformComponentDocument(new WorldVector3Document(float.PositiveInfinity, 0f, 0f))])],
            WorldMetadataDocument.Default);

        var report = WorldDocumentValidator.Validate(doc);
        Assert.Contains(report.Errors, e => e.Message.Contains("Infinity") || e.Message.Contains("无效"));
    }
}
