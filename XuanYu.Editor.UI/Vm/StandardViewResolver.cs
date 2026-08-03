using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

// F3-D3：六方向标准视角解析（计划 8.1 命名：+X 视图/-X 视图/+Y 视图/-Y 视图/顶视图/底视图）。
// 无歧义映射：点击 +X → 相机在 Pivot 的 +X 侧看向 -X；顶视图 = +Z 上方看向 -Z；
// 底视图 = -Z 下方看向 +Z，Up 固定 +Y（防滚转/镜像）。旧名 前/后/右/左 保留兼容。
public static class StandardViewResolver
{
    public const string Top = "顶视图";
    public const string Bottom = "底视图";

    public static bool TryResolve(string name, out Vector3d forward, out Vector3d up)
    {
        switch (name)
        {
            case Top: forward = new Vector3d(0, 0, -1); up = new Vector3d(0, 1, 0); return true;
            case Bottom: forward = new Vector3d(0, 0, 1); up = new Vector3d(0, -1, 0); return true;
            case "+X 视图": forward = new Vector3d(-1, 0, 0); up = Vector3d.UnitZ; return true;
            case "-X 视图": forward = new Vector3d(1, 0, 0); up = Vector3d.UnitZ; return true;
            case "+Y 视图": forward = new Vector3d(0, -1, 0); up = Vector3d.UnitZ; return true;
            case "-Y 视图": forward = new Vector3d(0, 1, 0); up = Vector3d.UnitZ; return true;
            // 旧名兼容（EDITOR-VIEW-R1 按钮）。
            case "顶": forward = new Vector3d(0, 0, -1); up = new Vector3d(0, 1, 0); return true;
            case "底": forward = new Vector3d(0, 0, 1); up = new Vector3d(0, -1, 0); return true;
            case "前": forward = new Vector3d(0, 1, 0); up = Vector3d.UnitZ; return true;
            case "后": forward = new Vector3d(0, -1, 0); up = Vector3d.UnitZ; return true;
            case "右": forward = new Vector3d(-1, 0, 0); up = Vector3d.UnitZ; return true;
            case "左": forward = new Vector3d(1, 0, 0); up = Vector3d.UnitZ; return true;
            default: forward = default; up = default; return false;
        }
    }

    // Gizmo 端点名（+X/-X/+Y/-Y/+Z/-Z）→ 标准视图命令名。
    public static string EndpointToViewName(string endpoint) => endpoint switch
    {
        "+Z" => Top,
        "-Z" => Bottom,
        _ => $"{endpoint} 视图",
    };

    // F3-F4：标准视图对应的视图平面网格类型。
    // ±X → YZ 平面、±Y → XZ 平面；±Z（顶/底）→ None（Z=0 地面网格即 XY 平面，复用现有网格）。
    public static EditorViewPlaneGridKind ViewPlaneGridFor(string name) => name switch
    {
        "+X 视图" or "-X 视图" => EditorViewPlaneGridKind.YZ,
        "+Y 视图" or "-Y 视图" => EditorViewPlaneGridKind.XZ,
        _ => EditorViewPlaneGridKind.None,
    };
}
