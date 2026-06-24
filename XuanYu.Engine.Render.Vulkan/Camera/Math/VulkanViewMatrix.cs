namespace XuanYu.Engine.Render.Vulkan.Camera;

/// <summary>LookAt 矩阵计算（列优先 float[16]）。</summary>
public static class VulkanViewMatrix
{
    internal static float[] LookAt(VulkanCameraInfo camera) =>
        LookAt(camera.PositionX, camera.PositionY, camera.PositionZ, camera.TargetX, camera.TargetY, camera.TargetZ, camera.UpX, camera.UpY, camera.UpZ);

    static float[] LookAt(float ex, float ey, float ez, float cx, float cy, float cz, float ux, float uy, float uz)
    {
        var fl = (float)Math.Sqrt((cx - ex) * (cx - ex) + (cy - ey) * (cy - ey) + (cz - ez) * (cz - ez)); if (fl < 1e-10f) fl = 1;
        var fx = (cx - ex) / fl; var fy = (cy - ey) / fl; var fz = (cz - ez) / fl;
        var sx = fy * uz - fz * uy; var sy = fz * ux - fx * uz; var sz = fx * uy - fy * ux;
        var sl = (float)Math.Sqrt(sx * sx + sy * sy + sz * sz); if (sl < 1e-10f) sl = 1;
        var snx = sx / sl; var sny = sy / sl; var snz = sz / sl;
        var uxn = sny * fz - snz * fy; var uyn = snz * fx - snx * fz; var uzn = snx * fy - sny * fx;
        return new float[] { snx, uxn, -fx, 0, sny, uyn, -fy, 0, snz, uzn, -fz, 0, -(snx * ex + sny * ey + snz * ez), -(uxn * ex + uyn * ey + uzn * ez), fx * ex + fy * ey + fz * ez, 1 };
    }
}
