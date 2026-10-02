# XYE-SRP-R1-B0-B state ownership contract audit

- WorldState is the current authoritative World owner, but it has no Revision publisher. World mutations are direct methods and Dirty revision is separate.
- RenderScene ownership is ViewportRenderSceneStore for the current derived scene, but it has no Revision. Initialization and position updates replace the scene object.
- Selection is not fully frozen: EditorSelectionRoute owns EditorSelectionState in Editor.Windows, while EditorEntitySelectionState in Editor.Editor separately owns a SelectedEntityId and Revision. This is a split ownership contract.
- Input ownership is split by concern: EditorInputService publishes binding snapshot revisions; EditorInputBindingSnapshot owns active drag but has no independent revision; EditorViewportInputState owns pointer coordinates/translator without revision.
- Mode has no single mutable owner or revision publisher. Projection/translation modes are enums, while session/tool/context flags are distributed.
- Tool is split: ViewportToolPalette owns UI ActiveTool, while TransformInteractionState owns Move/G state. Neither publishes a tool revision.
- Session ownership is clear in Scene3dSessionState/Scene3dSessionLifecycle; Generation advances on Clear, but Start/Set does not publish a new generation.
- Dirty ownership is clear in EditorWorldDirtyState; MarkDirty and Reset publish an incrementing Revision.
- Freeze rule: every future mutable state must have exactly one writer/owner, explicit readers, and one revision publisher. Derived state may replace snapshots but must not become a second authority.
