using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Windows.Shell;
using XuanYu.Engine.World;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Editor.Windows.Panels.Viewport;

namespace XuanYu.Engine.Editor.Windows.Viewport.Selection.Presentation;

/// <summary>世界实体选择 → Inspector / StatusBar / Viewport 展示。纯转换。</summary>
public sealed class WorldEntitySelectionPresenter
{
    public WorldEntitySelectionResult Present(WorldEntityInfo entity, WorldState? world,
        RenderScene renderScene, bool isScene3dActive)
    {
        var pos = world?.FindPosition(entity.EntityId);
        var posV = pos?.Value;
        var kindText = FindVisualKind(entity.EntityId, renderScene);
        var selection = CreateSelection(entity, posV);

        var viewport = new ViewportEntitySummary(entity.DisplayName,
            entity.EntityId.ToString(),
            posV is not null ? $"({posV.Value.X}, {posV.Value.Y}, {posV.Value.Z})" : "未知",
            entity.Source?.RelativePath, kindText);

        return new WorldEntitySelectionResult(selection, entity.EntityId.Value.ToString(),
            pos?.Value, entity.Source?.RelativePath, entity.DisplayName,
            isScene3dActive, $"已选择 {selection.Kind}：{entity.DisplayName}",
            viewport, kindText);
    }

    static string FindVisualKind(EntityId id, RenderScene scene)
    {
        var obj = scene.Objects.FirstOrDefault(o => o.EntityId == id);
        return obj is not null ? ToKindText(obj.VisualKind) : "未生成";
    }

    static string ToKindText(RenderObjectVisualKind kind) => kind switch
    {
        RenderObjectVisualKind.UnitMarker => "unit_marker",
        _ => kind.ToString()
    };

    static EditorSelection CreateSelection(WorldEntityInfo entity, Vector3d? pos)
    {
        var typeLabel = entity.Source is not null ? "World 占位实体" : "World 实体";
        var desc = pos is not null
            ? $"EntityId({entity.EntityId.Value})，来源：{entity.Source?.RelativePath ?? "无"}，位置：({pos.Value.X}, {pos.Value.Y}, {pos.Value.Z})"
            : $"EntityId({entity.EntityId.Value})，来源：{entity.Source?.RelativePath ?? "无"}";
        return new EditorSelection(typeLabel, entity.DisplayName, desc);
    }
}
