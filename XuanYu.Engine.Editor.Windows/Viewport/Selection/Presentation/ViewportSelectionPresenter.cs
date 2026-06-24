using XuanYu.Engine.Render.Scene;
using FluidWarfare.Editor.Windows.Panels.Viewport;

namespace FluidWarfare.Editor.Windows.Viewport.Selection.Presentation;

/// <summary>视口摘要展示。从 RenderScene 生成视口列表。</summary>
public sealed class ViewportSelectionPresenter
{
    public ViewportRenderSceneSummary CreateRenderSceneSummary(RenderScene scene)
    {
        if (scene.Objects.Count == 0) return ViewportRenderSceneSummary.Empty;

        var objects = scene.Objects
            .Select(o => new ViewportRenderObjectSummary(
                o.DisplayName, ToKindText(o.VisualKind),
                $"({o.Position.X}, {o.Position.Y}, {o.Position.Z})", o.SourcePath))
            .ToArray();
        return new ViewportRenderSceneSummary(objects);
    }

    static string ToKindText(RenderObjectVisualKind kind) => kind switch
    {
        RenderObjectVisualKind.UnitMarker => "unit_marker",
        _ => kind.ToString()
    };
}
