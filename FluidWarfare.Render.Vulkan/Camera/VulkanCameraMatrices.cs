namespace FluidWarfare.Render.Vulkan.Camera;

/// <summary>3D 相机矩阵计算。提供 View、Projection 和 ViewProjection 矩阵。Vulkan NDC：深度 0..1，Y 向下。</summary>
public static class VulkanCameraMatrices
{
    public static float[] ComputeVulkanMVP(VulkanCameraInfo camera, float aspectRatio)
    { var view = VulkanViewMatrix.LookAt(camera); var proj = PerspectiveVulkan(camera.FieldOfViewDegrees, aspectRatio, camera.NearPlane, camera.FarPlane); return VulkanMatrixOperations.Mul(proj, view); }

    public static float[] ComputeVulkanOrthoMVP(VulkanCameraInfo camera, float aspectRatio, float orthoHeight)
    {
        var view = VulkanViewMatrix.LookAt(camera); var halfH = orthoHeight / 2f; var halfW = halfH * aspectRatio; var range = camera.NearPlane - camera.FarPlane;
        var proj = new float[] { 1f / halfW, 0, 0, 0, 0, -1f / halfH, 0, 0, 0, 0, 1f / range, 0, 0, 0, camera.NearPlane / range, 1 };
        return VulkanMatrixOperations.Mul(proj, view);
    }

    static float[] PerspectiveVulkan(float fovDeg, float aspect, float near, float far)
    {
        var f = 1.0f / (float)Math.Tan(fovDeg * Math.PI / 360.0); var range = near - far;
        return new float[] { f / aspect, 0, 0, 0, 0, -f, 0, 0, 0, 0, far / range, -1, 0, 0, near * far / range, 0 };
    }
}
