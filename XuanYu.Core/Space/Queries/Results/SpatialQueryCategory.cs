namespace XuanYu.Core.Spatial;

[Flags]
public enum SpatialQueryCategory
{
    None = 0,
    SceneEntity = 1,
    Terrain = 2,
    Gizmo = 4,
    EditorHelper = 8,
    All = SceneEntity | Terrain | Gizmo | EditorHelper
}
