using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.World;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly SceneStateOwner _sceneState = new(new GridWorldPartitionStrategy(regionSize: 5));

    public ISceneRenderSnapshotSource SceneSnapshotSource => this;
    public SceneRenderSnapshot RenderSnapshot
    {
        get
        {
            var scene = _sceneState.RenderSnapshot;
            var entity = scene.Entity;
            var selected = HasSelection && SelectionKey == entity.EntityKey.ToString();
            var showMove = EditorTransformCapturePolicy.ShouldShowMoveGizmo(
                _editorState.ToolSnapshot, selected);
            return new SceneRenderSnapshot(
                entity,
                selected,
                _transformSession.Preview,
                showMove,
                scene.Entities,
                _camera);
        }
    }
    public event Action<SceneRenderSnapshot>? RenderSnapshotChanged;

    void ApplyRunCommand(string name)
    {
        if (name is "聚焦") { FrameSelectedCamera(); return; }
        if (name is "查看全部") { FrameAllCamera("查看全部"); return; }
        FooterMessage = UiText.CommandMessages.GetValueOrDefault(name, $"已执行：{name}");
        FooterState = name is "运行" ? "状态：运行中" : "状态：就绪";
        if (name is "运行") CommitTestEntityPosition(new Vector3d(1.0, 0.0, 0.0));
        if (name is "停止") CommitTestEntityPosition(Vector3d.Zero);
        LogCommand(name);
        OnPropertyChanged(nameof(DebugObjectItems));
        OnPropertyChanged(nameof(LogSummary));
    }

    void CommitTestEntityPosition(Vector3d position)
    {
        if (!_sceneState.RenderSnapshot.HasEntity) return;
        if (!_sceneState.CommitPosition(position)) return;
        _logBus.Info(EditorLogSource.Render, EditorLogCategory.Command,
            "场景实体位置已提交",
            $"实体={EditorDisplayText.Entity(_sceneState.RenderSnapshot.Entity.EntityKey)}；位置={EditorDisplayText.Position(position)}");
    }

    void PublishSceneRenderSnapshot()
    {
        TraceSelection("PublishSceneRenderSnapshot", 1,
            $"实体数={RenderSnapshot.Entities.Count}");
        RenderSnapshotChanged?.Invoke(RenderSnapshot);
    }

    IReadOnlyList<string> BuildDebugObjectItems()
    {
        var entity = _sceneState.RenderSnapshot.Entity;
        if (!entity.IsValid) return ["实体：无"];
        var position = entity.Transform.Position;
        return
        [
            $"实体编号：{EditorDisplayText.Entity(entity.EntityKey)}",
            $"名称：{entity.Name}",
            $"类型：{EditorDisplayText.EntityType(entity.Type)}",
            $"位置：{EditorDisplayText.Position(position)}"
        ];
    }
}
