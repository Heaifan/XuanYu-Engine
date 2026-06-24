namespace FluidWarfare.Editor.Windows.Shell.Composition;

/// <summary>EditorShell 事件接线。集中管理面板事件与 Route 之间的订阅。</summary>
sealed class EditorShellEventWiring(
    EditorShellContext ctx,
    EditorShellOverlayNavigationRoute overlayNavRoute,
    EditorShellGroundPointerRoute groundPointerRoute,
    EditorShellPickingRoute pickingRoute,
    EditorShellTransformRoute transformRoute,
    EditorShellScrubRoute scrubRoute,
    EditorShellRawInputRoute rawInputRoute,
    EditorShellViewportRedrawRoute viewportRedrawRoute,
    EditorShellViewportFocusRoute viewFocusRoute,
    EditorShellSelectionSyncRoute selectionSyncRoute,
    EditorShellLogRoute logRoute)
{
    public void Wire()
    {
        if (ctx.ViewportPlaceholderPanel is not null)
            ctx.ViewportPlaceholderPanel.ViewportFocused += (_, _) => viewFocusRoute.HandleViewportFocused();
        if (ctx.DockPanel is not null)
        {
            ctx.DockPanel.EntitySelectionRequested += OnHierarchyEntitySelected;
            ctx.DockPanel.ContentSelectionRequested += OnProjectContentSelected;
        }
        if (ctx.VulkanViewportHostPanel is not null)
        {
            ctx.VulkanViewportHostPanel.NativeHostInfoChanged += viewportRedrawRoute.HandleNativeHostInfoChanged;
            ctx.VulkanViewportHostPanel.RawPointerButtonDown += rawInputRoute.HandleRawPointerButtonDown;
            ctx.VulkanViewportHostPanel.RawPointerButtonUp += rawInputRoute.HandleRawPointerButtonUp;
            ctx.VulkanViewportHostPanel.RawPointerMoved += rawInputRoute.HandleRawPointerMoved;
            ctx.VulkanViewportHostPanel.RawKeyDown += rawInputRoute.HandleRawKeyDown;
            ctx.VulkanViewportHostPanel.RawKeyUp += rawInputRoute.HandleRawKeyUp;
            ctx.VulkanViewportHostPanel.RawMouseWheel += rawInputRoute.HandleRawMouseWheel;
            ctx.VulkanViewportHostPanel.RawInputFocusLost += rawInputRoute.HandleRawInputFocusLost;
            ctx.VulkanViewportHostPanel.PickRequested += (x, y) =>
                pickingRoute.HandleViewportPick(x, y, (id, or) => selectionSyncRoute.ApplyEntitySelection(id, or, viewFocusRoute.ShowWorldEntitySelection));
            ctx.VulkanViewportHostPanel.NavigationPointerPressed += overlayNavRoute.HandleOverlayPointerPressed;
            ctx.VulkanViewportHostPanel.NavigationPointerMoved += overlayNavRoute.HandleOverlayPointerMoved;
            ctx.VulkanViewportHostPanel.NavigationPointerReleased += overlayNavRoute.HandleOverlayPointerReleased;
            ctx.VulkanViewportHostPanel.NavigationCaptureLost += overlayNavRoute.HandleOverlayCaptureLost;
            ctx.VulkanViewportHostPanel.SceneToolPointerPressed += rawInputRoute.HandleSceneToolPointerPressed;
            ctx.VulkanViewportHostPanel.SceneToolPointerReleased += rawInputRoute.HandleSceneToolPointerReleased;
            ctx.VulkanViewportHostPanel.PointerMoved += groundPointerRoute.HandleViewportPointerMoved;
            ctx.VulkanViewportHostPanel.PointerLeft += groundPointerRoute.HandleViewportPointerLeft;
        }
        if (ctx.InspectorPanel is not null)
        {
            ctx.InspectorPanel.TransformDraftChanged += transformRoute.HandleTransformDraftChanged;
            ctx.InspectorPanel.TransformApplyRequested += transformRoute.HandleTransformApply;
            ctx.InspectorPanel.TransformResetRequested += transformRoute.HandleTransformReset;
            ctx.InspectorPanel.GroundPlacementRequested += transformRoute.HandleGroundPlacementToggle;
            ctx.InspectorPanel.ScrubValueChanged += scrubRoute.HandleScrubValueChanged;
            ctx.InspectorPanel.ScrubCompleted += scrubRoute.HandleScrubCompleted;
            ctx.InspectorPanel.ScrubCancelled += scrubRoute.HandleScrubCancelled;
        }
    }

    void OnHierarchyEntitySelected(string? entityId) =>
        selectionSyncRoute.ApplyEntitySelection(entityId, EditorEntitySelectionOrigin.WorldHierarchy, viewFocusRoute.ShowWorldEntitySelection);

    void OnProjectContentSelected(string? relativePath)
    {
        var r = ctx.ContentSelectionPresenter.Present(relativePath, ctx.ContentFiles);
        ctx.PanelApplyRoute.ApplyProjectContentSelection(r.InspectorSelection, r.StatusBarSelection, r.LogMessage, logRoute.Info);
    }
}
