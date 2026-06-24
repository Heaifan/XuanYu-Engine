using XuanYu.Engine.Project.World.Documents;
using XuanYu.Engine.Project.World.SaveLoad;
using XuanYu.Engine.Project.World.Validation;

namespace XuanYu.Engine.Tests.Project.World;

public sealed class WorldDocumentRoundTripTests
{
    [Fact]
    public void SaveLoadRoundTrip_CoreFieldsMatch()
    {
        var original = new WorldDocument(1, "rt_world", "往返测试世界",
            [
                new WorldEntityDocument("rt_1", "单位甲",
                    [new TransformComponentDocument(new WorldVector3Document(10f, 20f, 30f))]),
                new WorldEntityDocument("rt_2", "单位乙",
                    [new TransformComponentDocument(new WorldVector3Document(40f, 50f, 60f))]),
                new WorldEntityDocument("rt_3", "单位丙",
                    [new TransformComponentDocument(WorldVector3Document.Zero)]),
            ],
            new WorldMetadataDocument("测试", "RoundTrip 测试"));

        var path = Path.GetTempFileName();
        try
        {
            // 保存
            var writeResult = WorldDocumentWriter.Write(path, original);
            Assert.True(writeResult.IsSuccess);

            // 读取
            var readResult = WorldDocumentReader.Read(path);
            Assert.True(readResult.IsSuccess);
            Assert.NotNull(readResult.Document);

            var loaded = readResult.Document;

            // 校验
            var report = WorldDocumentValidator.Validate(loaded);
            Assert.True(report.IsValid, string.Join("; ", report.Errors.Select(e => e.Message)));

            // 比较核心字段
            Assert.Equal(original.SchemaVersion, loaded.SchemaVersion);
            Assert.Equal(original.WorldId, loaded.WorldId);
            Assert.Equal(original.DisplayName, loaded.DisplayName);
            Assert.Equal(original.Entities.Count, loaded.Entities.Count);
            Assert.Equal(original.Metadata.CreatedBy, loaded.Metadata.CreatedBy);

            // 比较实体字段
            for (int i = 0; i < original.Entities.Count; i++)
            {
                Assert.Equal(original.Entities[i].EntityId, loaded.Entities[i].EntityId);
                Assert.Equal(original.Entities[i].DisplayName, loaded.Entities[i].DisplayName);

                var origComp = original.Entities[i].Components;
                var loadComp = loaded.Entities[i].Components;
                Assert.Equal(origComp.Count, loadComp.Count);

                var origTrans = origComp.OfType<TransformComponentDocument>().First();
                var loadTrans = loadComp.OfType<TransformComponentDocument>().First();
                Assert.Equal(origTrans.Position.X, loadTrans.Position.X);
                Assert.Equal(origTrans.Position.Y, loadTrans.Position.Y);
                Assert.Equal(origTrans.Position.Z, loadTrans.Position.Z);
            }
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void SaveLoadRoundTrip_EmptyWorld()
    {
        var original = new WorldDocument(1, "empty", "空世界", [], WorldMetadataDocument.Default);
        var path = Path.GetTempFileName();
        try
        {
            var writeResult = WorldDocumentWriter.Write(path, original);
            Assert.True(writeResult.IsSuccess);

            var readResult = WorldDocumentReader.Read(path);
            Assert.True(readResult.IsSuccess);
            Assert.NotNull(readResult.Document);

            var report = WorldDocumentValidator.Validate(readResult.Document);
            Assert.True(report.IsValid);

            Assert.Empty(readResult.Document.Entities);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void SaveLoadRoundTrip_JsonIsValidJson()
    {
        var original = new WorldDocument(1, "valid", "合法 JSON 测试",
            [new WorldEntityDocument("e1", "实体",
                [new TransformComponentDocument(new WorldVector3Document(1f, 2f, 3f))])],
            WorldMetadataDocument.Default);

        var path = Path.GetTempFileName();
        try
        {
            WorldDocumentWriter.Write(path, original);
            var json = File.ReadAllText(path);

            // 验证 JSON 是合法 JSON（不会抛出异常）
            var exc = Record.Exception(() =>
                System.Text.Json.JsonDocument.Parse(json));
            Assert.Null(exc);
        }
        finally { File.Delete(path); }
    }
}
