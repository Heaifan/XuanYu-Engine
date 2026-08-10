using XuanYu.Core.Math;

namespace XuanYu.Core.Space;

public static class WorldRayFactory
{
    public static WorldRay FromViewportPoint(
        ViewProjectionState state,
        double logicalX,
        double logicalY)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (!double.IsFinite(logicalX)) throw new ArgumentOutOfRangeException(nameof(logicalX));
        if (!double.IsFinite(logicalY)) throw new ArgumentOutOfRangeException(nameof(logicalY));

        var ndcX = ToNdcX(state.Viewport, logicalX);
        var ndcY = ToNdcY(state.Viewport, logicalY);
        var aspect = state.Viewport.LogicalWidth / state.Viewport.LogicalHeight;
        if (state.Camera.Mode == ProjectionMode.Orthographic)
        {
            var halfHeight = state.Camera.OrthographicScale * 0.5;
            var near = state.Camera.Position
                + (state.Camera.Right * (ndcX * halfHeight * aspect))
                + (state.Camera.Up * (ndcY * halfHeight))
                + (state.Camera.Forward * state.Camera.NearPlane);
            return new WorldRay(near, state.Camera.Forward);
        }

        var tangent = global::System.Math.Tan(state.Camera.VerticalFovDegrees * global::System.Math.PI / 360.0);
        var direction = state.Camera.Forward
            + (state.Camera.Right * (ndcX * aspect * tangent))
            + (state.Camera.Up * (ndcY * tangent));
        var origin = state.Camera.Position + (state.Camera.Forward * state.Camera.NearPlane);

        return new WorldRay(origin, direction);
    }

    static double ToNdcX(ViewportState viewport, double logicalX)
    {
        return ((logicalX - viewport.LogicalX) / viewport.LogicalWidth * 2.0) - 1.0;
    }

    static double ToNdcY(ViewportState viewport, double logicalY)
    {
        return 1.0 - ((logicalY - viewport.LogicalY) / viewport.LogicalHeight * 2.0);
    }
}
