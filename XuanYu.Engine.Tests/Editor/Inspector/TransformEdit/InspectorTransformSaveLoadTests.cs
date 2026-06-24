using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.EntityTransform;
using XuanYu.Engine.Editor.Windows.Inspector.TransformEdit;
using XuanYu.Engine.Editor.Windows.Viewport.World;
using XuanYu.Engine.Project.World.Documents;
using XuanYu.Engine.Project.World.SaveLoad;
using XuanYu.Engine.Project.World.Validation;
using XuanYu.Engine.World;

namespace XuanYu.Engine.Tests.Editor.Inspector.TransformEdit;

/// <summary>集成测试：Apply → WorldState → Save → Load → 比较 Transform 一致。</summary>
public sealed class InspectorTransformSaveLoadTests
{
    [Fact]
    public void ApplyThenSaveLoad_RotationScalePreserved()
    {
        // Phase 1: 在 WorldState 中创建实体
        var world = new WorldState();
        var eid = world.CreateEntity("测试单位", Vector3d.Zero);
        var idStr = eid.Value.ToString();
        var dirty = new EditorWorldDirtyState();

        // Phase 2: Apply 完整的 Transform 编辑
        var applyReq = new TransformEditRequest(idStr,
            new Vector3d(10, 20, 30), new Vector3d(0, 45, 90), new Vector3d(2, 1.5, 0.5));
        var applyResult = SelectedEntityTransformApply.Apply(applyReq, world, dirty, null, null);
        Assert.True(applyResult.Succeeded);

        // Phase 3: 验证 WorldState 已更新
        Assert.Equal(new Vector3d(10, 20, 30), world.FindPosition(eid)!.Value.Value);
        Assert.Equal(new Vector3d(0, 45, 90), world.FindRotation(eid)!.Value.Value);
        Assert.Equal(new Vector3d(2, 1.5, 0.5), world.FindScale(eid)!.Value.Value);

        // Phase 4: WorldState → WorldDocument → 保存 → 加载
        var doc = WorldStateDocumentConvert.ToDocument(world, "test", "测试");
        var path = Path.GetTempFileName();
        try
        {
            var writeResult = WorldDocumentWriter.Write(path, doc);
            Assert.True(writeResult.IsSuccess);

            var readResult = WorldDocumentReader.Read(path);
            Assert.True(readResult.IsSuccess);
            Assert.NotNull(readResult.Document);

            // Phase 5: 校验
            var report = WorldDocumentValidator.Validate(readResult.Document);
            Assert.True(report.IsValid, string.Join("; ", report.Errors.Select(e => e.Message)));

            // Phase 6: 比较保存前后的 Transform 值
            var loadedTrans = readResult.Document.Entities[0].Components
                .OfType<TransformComponentDocument>().First();
            Assert.Equal(10f, loadedTrans.GetPositionOrDefault().X);
            Assert.Equal(20f, loadedTrans.GetPositionOrDefault().Y);
            Assert.Equal(30f, loadedTrans.GetPositionOrDefault().Z);
            Assert.Equal(0f, loadedTrans.GetRotationDegreesOrDefault().X);
            Assert.Equal(45f, loadedTrans.GetRotationDegreesOrDefault().Y);
            Assert.Equal(90f, loadedTrans.GetRotationDegreesOrDefault().Z);
            Assert.Equal(2f, loadedTrans.GetScaleOrDefault().X);
            Assert.Equal(1.5f, loadedTrans.GetScaleOrDefault().Y);
            Assert.Equal(0.5f, loadedTrans.GetScaleOrDefault().Z);

            // Phase 7: WorldDocument → WorldState 往返一致性
            var loadedWorld = WorldStateDocumentConvert.ToWorldState(readResult.Document);
            var loadedEid = loadedWorld.ListEntities()[0].EntityId;
            Assert.Equal(10, loadedWorld.FindPosition(loadedEid)!.Value.Value.X);
            Assert.Equal(45, loadedWorld.FindRotation(loadedEid)!.Value.Value.Y);
            Assert.Equal(0.5, loadedWorld.FindScale(loadedEid)!.Value.Value.Z);
        }
        finally { File.Delete(path); }
    }
}
