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
                [new TransformComponentDocument { Position = new WorldVector3Document(1f, 2f, 3f) }])],
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
                [new TransformComponentDocument { Position = new WorldVector3Document(float.NaN, 0f, 0f) }])],
            WorldMetadataDocument.Default);

        var report = WorldDocumentValidator.Validate(doc);
        Assert.Contains(report.Errors, e => e.Message.Contains("NaN") || e.Message.Contains("无效"));
    }

    [Fact]
    public void Validate_InfinityPosition_Fails()
    {
        var doc = new WorldDocument(1, "main", "世界",
            [new WorldEntityDocument("e1", "实体",
                [new TransformComponentDocument { Position = new WorldVector3Document(float.PositiveInfinity, 0f, 0f) }])],
            WorldMetadataDocument.Default);

        var report = WorldDocumentValidator.Validate(doc);
        Assert.Contains(report.Errors, e => e.Message.Contains("Infinity") || e.Message.Contains("无效"));
    }

    [Fact]
    public void Validate_NanRotation_Fails()
    {
        var doc = T(E("e1", [new TransformComponentDocument { Position = WorldVector3Document.Zero, RotationDegrees = new WorldVector3Document(float.NaN, 0f, 0f) }]));
        Assert.Contains(WorldDocumentValidator.Validate(doc).Errors, e => e.Message.Contains("无效"));
    }

    [Fact]
    public void Validate_NanScale_Fails()
    {
        var doc = T(E("e1", [new TransformComponentDocument { Position = WorldVector3Document.Zero, Scale = new WorldVector3Document(float.NaN, 1f, 1f) }]));
        Assert.Contains(WorldDocumentValidator.Validate(doc).Errors, e => e.Message.Contains("无效"));
    }

    [Fact]
    public void Validate_ScaleZero_Fails()
    {
        var doc = T(E("e1", [new TransformComponentDocument { Position = WorldVector3Document.Zero, Scale = new WorldVector3Document(0f, 1f, 1f) }]));
        Assert.Contains(WorldDocumentValidator.Validate(doc).Errors, e => e.Message.Contains("大于 0"));
    }

    [Fact]
    public void Validate_ScaleNegative_Fails()
    {
        var doc = T(E("e1", [new TransformComponentDocument { Position = WorldVector3Document.Zero, Scale = new WorldVector3Document(-1f, 1f, 1f) }]));
        Assert.Contains(WorldDocumentValidator.Validate(doc).Errors, e => e.Message.Contains("大于 0"));
    }

    [Fact]
    public void Validate_FullTransform_Passes()
    {
        var doc = T(E("e1", [new TransformComponentDocument { Position = new WorldVector3Document(10f, 20f, 30f), RotationDegrees = new WorldVector3Document(0f, 45f, 90f), Scale = new WorldVector3Document(2f, 2f, 0.5f) }]));
        Assert.True(WorldDocumentValidator.Validate(doc).IsValid);
    }

    // ── 帮助方法：减少重复 ──────────────────────────────
    static WorldDocument T(params WorldEntityDocument[] entities) => new(1, "main", "世界", entities, WorldMetadataDocument.Default);
    static WorldEntityDocument E(string id, WorldComponentDocument[]? comps) => new(id, "实体", comps ?? []);
}
