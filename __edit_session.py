with open('FluidWarfare.Render.Vulkan/Scene3D/Session/VulkanScene3dSession.cs', 'r', encoding='utf-8') as f:
    lines = f.readlines()

new_lines = lines[:110] + [
    '    // ─── 保留字段（属性在 VulkanScene3dSession.Properties.cs）──\n',
    '    private int _lastOverlayVertexCount;\n',
    '    private Render.ViewportNavigation.ViewportNavigationLayout _lastOverlayLayout = null!;\n',
    '    private Overlay.VulkanOverlayVertex[]? _pendingGizmoVerts;\n',
] + lines[234:]

with open('FluidWarfare.Render.Vulkan/Scene3D/Session/VulkanScene3dSession.cs', 'w', encoding='utf-8') as f:
    f.writelines(new_lines)

print(f"OK: {len(lines)} -> {len(new_lines)} lines")
