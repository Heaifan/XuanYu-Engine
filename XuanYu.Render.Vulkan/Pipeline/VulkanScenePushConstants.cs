namespace XuanYu.Render.Vulkan.Pipeline;

internal static class VulkanScenePushConstants
{
    // std140 布局：
    //   mat4 viewProjection      @0   (64)
    //   vec4 worldPosition       @64  (16)
    //   float gizmoMode          @80  (4)    0=Move Gizmo / 1=Rotate Gizmo
    //   float gizmoRingRadius    @84  (4)    仅旋转 Gizmo 环使用（屏幕空间世界半径）
    //   float selectionMode      @88  (4)    0=未选中填充 / 1=选中填充 / 2=外轮廓边带
    //   vec4 entityRotation      @96  (16)   xyz=欧拉角（度），w=viewportWidth
    //   vec4 entityScale         @112 (16)   xyz=实体缩放，w=viewportHeight
    // 合计 128 字节（32 个 float），必须 ≤ 128（Vulkan 最小保证上限）。
    public const uint SizeInBytes = 128;
    public const int FloatCount = 32;
}
