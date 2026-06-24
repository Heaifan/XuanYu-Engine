using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Editor.EntityTransform;

/// <summary>
/// 一次正式实体位置修改。
/// </summary>
public sealed record EditorEntityTransformChange(
    string EntityId,
    Vector3d PreviousPosition,
    Vector3d CurrentPosition,
    EditorEntityTransformOrigin Origin,
    int Revision);

/// <summary>
/// Transform 修改来源。
/// </summary>
public enum EditorEntityTransformOrigin
{
    InspectorInput,
    GroundPlacement,
    MoveTool,
    DragScrub,
}
