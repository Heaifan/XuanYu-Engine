using XuanYu.Engine.Editor.Input.Actions;
using XuanYu.Engine.Editor.Input.Bindings;
using XuanYu.Engine.Editor.Input.Runtime;

namespace FluidWarfare.Tests.Editor.Input.Runtime;

public sealed class EditorInputServiceTests
{
    public EditorInputServiceTests()
    {
        // 每次测试前重置服务状态
        var service = EditorInputService.Instance;
        service.Initialize();
    }

    [Fact]
    public void Initialize_CreatesDefaultSnapshot()
    {
        var service = EditorInputService.Instance;
        var snapshot = service.CurrentSnapshot;

        Assert.NotNull(snapshot);
        Assert.True(snapshot.Revision >= 1);
    }

    [Fact]
    public void TryApplyNewBindingSet_ReplacesSnapshot()
    {
        var service = EditorInputService.Instance;
        var originalSnapshot = service.CurrentSnapshot;

        var newSet = new EditorInputBindingSet
        {
            Preset = "blender",
            Overrides = new[]
            {
                new EditorInputBindingOverride
                {
                    ActionId = "viewport.orbit",
                    Slot = "primary",
                    Gesture = new EditorInputGesture(
                        EditorInputDevice.Mouse, "Right",
                        EditorInputModifiers.Alt,
                        EditorInputGestureKind.MouseDrag)
                }
            }
        };

        var success = service.TryApplyNewBindingSet(newSet, out var error);

        Assert.True(success);
        Assert.Null(error);

        var newSnapshot = service.CurrentSnapshot;
        Assert.NotSame(originalSnapshot, newSnapshot);
        Assert.True(newSnapshot.Revision > originalSnapshot.Revision);
    }

    [Fact]
    public void TryApplyNewBindingSet_IncrementsRevision()
    {
        var service = EditorInputService.Instance;
        var rev1 = service.CurrentSnapshot.Revision;

        var set = new EditorInputBindingSet { Preset = "blender" };
        service.TryApplyNewBindingSet(set, out _);
        var rev2 = service.CurrentSnapshot.Revision;

        service.TryApplyNewBindingSet(set, out _);
        var rev3 = service.CurrentSnapshot.Revision;

        Assert.True(rev2 > rev1);
        Assert.True(rev3 > rev2);
    }

    [Fact]
    public void TryApplyNewBindingSet_FiresEvent()
    {
        var service = EditorInputService.Instance;
        var fired = false;

        service.SnapshotReplaced += snapshot =>
        {
            fired = true;
            Assert.NotNull(snapshot);
        };

        var set = new EditorInputBindingSet { Preset = "blender" };
        service.TryApplyNewBindingSet(set, out _);

        Assert.True(fired);
    }

    [Fact]
    public void TryApplyNewBindingSet_EndsActiveDrag()
    {
        var service = EditorInputService.Instance;
        var snapshot = service.CurrentSnapshot;

        // 模拟一个活动拖动
        snapshot.BeginDrag("|MouseDrag|Mouse|Middle", 0, 0);

        // 应用新绑定（应结束旧拖动）
        var set = new EditorInputBindingSet { Preset = "blender" };
        service.TryApplyNewBindingSet(set, out _);

        // 旧 snapshot 的拖拽应被结束
        Assert.Null(snapshot.GetActiveDragActionId());
    }
}
