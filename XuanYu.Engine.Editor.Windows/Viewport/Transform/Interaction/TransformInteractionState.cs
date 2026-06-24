namespace FluidWarfare.Editor.Windows.Viewport.Transform.Interaction;

/// <summary>
/// Move 工具和 G 模态的共享状态。
/// 由 TransformPointerRoute 创建，KeyboardRoute 和 Shell 读取修改。
/// </summary>
public sealed class TransformInteractionState
{
    public bool MoveToolActive { get; private set; }
    public bool BlenderMoveActive { get; private set; }

    public void SetToolActive(bool active) => MoveToolActive = active;
    public void SetBlenderGActive(bool active) => BlenderMoveActive = active;
    public bool IsAnyActive => MoveToolActive || BlenderMoveActive;
}
