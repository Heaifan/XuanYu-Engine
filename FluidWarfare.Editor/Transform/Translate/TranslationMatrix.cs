using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

internal static class TranslationMatrix
{
    public static bool TryUnproject(double[] invViewProjection, double ndcX, double ndcY, double ndcZ, out Vector3d world)
    {
        world = default;
        if (invViewProjection.Length != 16)
            return false;

        var x = invViewProjection[0] * ndcX + invViewProjection[4] * ndcY +
                invViewProjection[8] * ndcZ + invViewProjection[12];
        var y = invViewProjection[1] * ndcX + invViewProjection[5] * ndcY +
                invViewProjection[9] * ndcZ + invViewProjection[13];
        var z = invViewProjection[2] * ndcX + invViewProjection[6] * ndcY +
                invViewProjection[10] * ndcZ + invViewProjection[14];
        var w = invViewProjection[3] * ndcX + invViewProjection[7] * ndcY +
                invViewProjection[11] * ndcZ + invViewProjection[15];

        if (Math.Abs(w) < 1e-15)
            return false;

        world = new Vector3d(x / w, y / w, z / w);
        return double.IsFinite(world.X) && double.IsFinite(world.Y) && double.IsFinite(world.Z);
    }

    public static bool TryProject(double[] viewProjection, Vector3d world, int width, int height, out double x, out double y)
    {
        x = y = 0;
        if (viewProjection.Length != 16 || width <= 0 || height <= 0)
            return false;

        var clipX = viewProjection[0] * world.X + viewProjection[4] * world.Y +
                    viewProjection[8] * world.Z + viewProjection[12];
        var clipY = viewProjection[1] * world.X + viewProjection[5] * world.Y +
                    viewProjection[9] * world.Z + viewProjection[13];
        var clipW = viewProjection[3] * world.X + viewProjection[7] * world.Y +
                    viewProjection[11] * world.Z + viewProjection[15];
        if (Math.Abs(clipW) < 1e-15)
            return false;

        x = ((clipX / clipW) + 1.0) * 0.5 * width;
        y = ((clipY / clipW) + 1.0) * 0.5 * height;
        return double.IsFinite(x) && double.IsFinite(y);
    }
}
