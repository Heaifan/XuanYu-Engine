namespace FluidWarfare.Render.ViewportNavigation;

/// <summary>ViewportNavigationLayout 的工厂方法。</summary>
public static class ViewportNavigationLayoutCompute
{
    public static ViewportNavigationLayout Compute(
        int vw, int vh, Camera.SceneCameraPose cam)
    {
        var s = vw < 320 || vh < 240 ? Math.Min(vw / 320f, vh / 240f) : 1f;
        var gx = vw - 18f * s - 104f * s * 0.5f;
        var gy = 16f * s + 104f * s * 0.5f;

        var proj = ComputeProjections(cam, gx, gy, s);
        var bs = 30f * s; var sp = 6f * s;
        var bx = gx - bs * 0.5f; var by = gy + 104f * s * 0.5f + sp;

        return new ViewportNavigationLayout
        {
            ViewportWidth = vw, ViewportHeight = vh, Scale = s,
            GizmoCenterX = gx, GizmoCenterY = gy,
            ButtonAreaX = bx, ButtonAreaY = by,
            AxisProjections = proj,
            GizmoCenterCircle = new Circle(gx, gy, 12f * s),
            GizmoOrbitCircle = new Circle(gx, gy, 52f * s),
            PanButtonRect = new Rect(bx, by, bs, bs),
            FrameButtonRect = new Rect(bx, by + bs + sp, bs, bs),
            ProjectionButtonRect = new Rect(bx, by + 2 * (bs + sp), bs, bs),
        };
    }

    private static IReadOnlyList<AxisProjection> ComputeProjections(
        Camera.SceneCameraPose cam, float gx, float gy, float s)
    {
        var (fx, fy, fz) = Norm(cam.TargetX - cam.PositionX,
            cam.TargetY - cam.PositionY, cam.TargetZ - cam.PositionZ);
        var (ux, uy, uz) = Norm(cam.UpX, cam.UpY, cam.UpZ);
        if (Math.Abs(fx * ux + fy * uy + fz * uz) > 0.995f) (ux, uy, uz) = (0, 1, 0);

        var (rx, ry, rz) = Norm(fy * uz - fz * uy, fz * ux - fx * uz, fx * uy - fy * ux);
        var (vx, vy, vz) = Norm(ry * fz - rz * fy, rz * fx - rx * fz, rx * fy - ry * fx);

        var al = 34f * s; var fr = 10f * s; var br = 7f * s;

        (ViewportNavigationElement, float X, float Y, float Z, float R, float G, float B)[] axes =
        [
            (ViewportNavigationElement.PositiveX, 1,0,0, 0.941f,0.294f,0.243f),
            (ViewportNavigationElement.NegativeX,-1,0,0, 0.502f,0.157f,0.125f),
            (ViewportNavigationElement.PositiveY, 0,1,0, 0.396f,0.784f,0.290f),
            (ViewportNavigationElement.NegativeY, 0,-1,0,0.208f,0.420f,0.157f),
            (ViewportNavigationElement.PositiveZ, 0,0,1, 0.224f,0.482f,1.000f),
            (ViewportNavigationElement.NegativeZ, 0,0,-1,0.141f,0.271f,0.549f),
        ];

        var results = new List<AxisProjection>(6);
        foreach (var (e, ax, ay, az, cr, cg, cb) in axes)
        {
            var sx = ax * rx + ay * ry + az * rz;
            var sy = -(ax * vx + ay * vy + az * vz);
            var d = ax * fx + ay * fy + az * fz;
            results.Add(new(e, gx + sx * al, gy + sy * al, d, d > 0 ? fr : br, (cr, cg, cb)));
        }
        results.Sort((a, b) => a.Depth.CompareTo(b.Depth));
        return results;
    }

    private static (float X, float Y, float Z) Norm(float x, float y, float z)
    {
        var len = MathF.Sqrt(x * x + y * y + z * z);
        return len < 1e-10f ? (0, 0, 1) : (x / len, y / len, z / len);
    }
}
