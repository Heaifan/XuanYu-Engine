using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly SceneStateOwner _sceneState = new();

    public ISceneRenderSnapshotSource SceneSnapshotSource => this;
    public SceneRenderSnapshot RenderSnapshot
    {
        get
        {
            var entity = _sceneState.RenderSnapshot.Entity;
            var selected = HasSelection && SelectionKey == entity.EntityKey.ToString();
            var showMove = EditorTransformCapturePolicy.ShouldShowMoveGizmo(
                _editorState.ToolSnapshot, selected);
            return new SceneRenderSnapshot(entity, selected, _transformSession.Preview, showMove);
        }
    }
    public event Action<SceneRenderSnapshot>? RenderSnapshotChanged;

    void ApplyRunCommand(string name)
    {
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
            "ARCH-C-R1 场景实体 Position 已提交",
            $"EntityKey={_sceneState.RenderSnapshot.Entity.EntityKey}; Position={position}");
    }

    void PublishSceneRenderSnapshot() => RenderSnapshotChanged?.Invoke(RenderSnapshot);

    IReadOnlyList<string> BuildDebugObjectItems()
    {
        var entity = _sceneState.RenderSnapshot.Entity;
        if (!entity.IsValid) return ["Entity：无"];
        var position = entity.Transform.Position;
        return
        [
            $"EntityKey：{entity.EntityKey}",
            $"Name：{entity.Name}",
            $"Type：{entity.Type}",
            $"Position：{position}"
        ];
    }
}
