using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Selection;
using FluidWarfare.Editor.Windows.Panels.Inspector;
using FluidWarfare.Editor.Windows.Panels.Viewport;
using FluidWarfare.Editor.Windows.Viewport.Selection.Presentation;

namespace FluidWarfare.Editor.Windows.Shell.Panels;

public sealed class EditorPanelApplyRoute
{
    public EditorPanelApplyState State { get; } = new();
    EditorPanelApplyPanels _p = null!;
    public void SetPanels(EditorPanelApplyPanels p) => _p = p;

    public void ApplyEntitySelection(EditorSelection selection, string? entityId, Vector3d? position,
        string? sourcePath, bool groundPlaceEnabled, string? statusBarSelection,
        ViewportEntitySummary? viewportSummary, string? logMessage, Action<string> log)
    {
        _p.Inspector?.ShowWorldEntitySelection(selection, entityId ?? "", position, sourcePath);
        if (_p.Inspector is InspectorPanel i) i.ScrubEntityId = entityId ?? "";
        _p.Inspector?.SetGroundPlaceEnabled(groundPlaceEnabled);
        _p.StatusBar?.SetCurrentSelection(statusBarSelection ?? "无");
        if (viewportSummary is not null) _p.ViewportPlaceholder?.ShowEntitySummary(viewportSummary);
        if (!string.IsNullOrEmpty(logMessage)) log(logMessage);
        State.LastSelectedEntityId = entityId;
    }

    public void ClearSelection()
    { _p.Inspector?.ShowNoSelection(); _p.StatusBar?.SetCurrentSelection("无"); _p.DockPanel?.ClearEntitySelection(); State.LastSelectedEntityId = null; }

    public void ApplyProjectContentSelection(EditorSelection inspectorSelection, string? statusBarSelection, string? logMessage, Action<string> log)
    { _p.Inspector?.ShowProjectFileSelection(inspectorSelection); _p.StatusBar?.SetCurrentSelection(statusBarSelection ?? "无"); if (!string.IsNullOrEmpty(logMessage)) log(logMessage); }

    public void ShowProjectLoadFailure(string failureMessage, Action<string> errorLog)
    {
        _p.ViewportPlaceholder?.ShowNoWorldEntity();
        _p.Inspector?.ShowSelection(new EditorSelection("项目加载", "加载失败", $"项目加载失败：{failureMessage}"));
        _p.StatusBar?.SetCurrentSelection("项目加载失败");
        errorLog($"项目加载失败：{failureMessage}");
    }

    public void ApplyStartupWorld(EditorPanelApplyStartupWorld world)
    { _p.ViewportPlaceholder?.ShowNoWorldEntity(); _p.ViewportPlaceholder?.ShowRenderSceneSummary(world.RenderSceneSummary); }

    public void ShowViewportFocused(EditorSelection? selection, string statusText, bool showEmptyWorld)
    {
        _p.Inspector?.ShowSelection(selection ?? new EditorSelection("默认", "视口", "战场视口"));
        _p.StatusBar?.SetCurrentSelection(statusText);
        if (showEmptyWorld) _p.ViewportPlaceholder?.ShowEmptyWorld();
    }
}

public sealed record EditorPanelApplyStartupWorld(bool HasEntities, ViewportRenderSceneSummary RenderSceneSummary);
