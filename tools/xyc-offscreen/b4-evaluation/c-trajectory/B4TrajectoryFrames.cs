using System.Text.Json;
using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

internal sealed record B4TrajectoryFrame(string Track, int Segment, int Index, int Count,
    RenderCameraProjection Camera, Vector3d ObservationCenter)
{
    public static IEnumerable<B4TrajectoryFrame> Expand(JsonElement root)
    {
        var poses = root.GetProperty("poses"); var revision = 0;
        foreach (var track in root.GetProperty("tracks").EnumerateArray())
        {
            var name = track.GetProperty("id").GetString()!; var segmentIndex = 0;
            foreach (var segment in track.GetProperty("segments").EnumerateArray())
            {
                var from = Read(poses.GetProperty(segment.GetProperty("from").GetString()!));
                var count = segment.GetProperty("frames").GetInt32();
                var isOrbit = segment.TryGetProperty("motion", out var motion) && motion.GetString() == "orbit";
                var to = isOrbit ? from : Read(poses.GetProperty(segment.GetProperty("to").GetString()!));
                for (var i = 0; i < count; i++)
                {
                    var t = count == 1 ? 1 : (double)i / (count - 1);
                    var orbitCenter = from.Center;
                    var camera = isOrbit
                        ? Orbit(from.Camera, orbitCenter, t, segment.GetProperty("angleDegrees").GetDouble(), ++revision)
                        : Interpolate(from.Camera, to.Camera, t, ++revision);
                    yield return new(name, segmentIndex, i, count,
                        camera, isOrbit ? orbitCenter : Lerp(from.Center, to.Center, t));
                }
                segmentIndex++;
            }
        }
    }

    static (RenderCameraProjection Camera, Vector3d Center) Read(JsonElement p)
    {
        var camera = new RenderCameraProjection(Vec(p.GetProperty("position")),
            Vec(p.GetProperty("forward")), Vec(p.GetProperty("up")),
            p.GetProperty("fovDegrees").GetDouble(), p.GetProperty("nearPlane").GetDouble(),
            p.GetProperty("farPlane").GetDouble(), 0);
        return (camera, Vec(p.GetProperty("observationCenter")));
    }

    static Vector3d Vec(JsonElement p) => new(p.GetProperty("x").GetDouble(),
        p.GetProperty("y").GetDouble(), p.GetProperty("z").GetDouble());
    static Vector3d Lerp(Vector3d a, Vector3d b, double t) => a + (b - a) * t;
    static Vector3d UnitLerp(Vector3d a, Vector3d b, double t) => Lerp(a, b, t).Normalize();
    static RenderCameraProjection Orbit(RenderCameraProjection start, Vector3d center,
        double t, double maxDegrees, long rev)
    {
        var phase = t <= 0.5 ? t * 2 : (1 - t) * 2;
        if (phase == 0) return start with { Revision = rev };
        var relative = start.Position - center; var angle = maxDegrees * phase * global::System.Math.PI / 180;
        var position = center + new Vector3d(relative.X * global::System.Math.Cos(angle) - relative.Y * global::System.Math.Sin(angle),
            relative.X * global::System.Math.Sin(angle) + relative.Y * global::System.Math.Cos(angle), relative.Z);
        var forward = (center - position).Normalize();
        var up = (Vector3d.UnitZ - forward * forward.Dot(Vector3d.UnitZ)).Normalize();
        return new(position, forward, up, start.VerticalFovDegrees, start.NearPlane,
            start.FarPlane, rev);
    }
    static RenderCameraProjection Interpolate(RenderCameraProjection a,
        RenderCameraProjection b, double t, long rev)
    {
        if (t == 0) return a with { Revision = rev };
        if (t == 1) return b with { Revision = rev };
        var forward = UnitLerp(a.Forward, b.Forward, t);
        var up = UnitLerp(a.Up, b.Up, t);
        var right = forward.Cross(up).Normalize();
        up = right.Cross(forward).Normalize();
        return new(Lerp(a.Position, b.Position, t), forward, up,
            a.VerticalFovDegrees + (b.VerticalFovDegrees - a.VerticalFovDegrees) * t,
            a.NearPlane + (b.NearPlane - a.NearPlane) * t,
            a.FarPlane + (b.FarPlane - a.FarPlane) * t, rev);
    }
}
