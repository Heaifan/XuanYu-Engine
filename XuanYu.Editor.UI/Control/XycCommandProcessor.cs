using System.Diagnostics;
using System.Text.Json;

namespace XuanYu.Editor.UI.XycControl;

public sealed class XycCommandProcessor(IXycRuntime runtime, bool diagnosticWrite)
{
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    readonly HashSet<string> _requestIds = new(StringComparer.Ordinal);

    public XycResponse Execute(XycRequest request)
    {
        var timer = Stopwatch.StartNew();
        XycResponse Result(bool ok, string? error, object? state = null,
            long? scene = null, long? camera = null) => new(request.RequestId,
                ok, error, state, scene, camera, timer.Elapsed.TotalMilliseconds);
        if (request.SchemaVersion != "XYC-API/1") return Result(false, "INVALID_SCHEMA");
        if (string.IsNullOrWhiteSpace(request.RequestId)) return Result(false, "INVALID_INPUT");
        if (!_requestIds.Add(request.RequestId)) return Result(false, "DUPLICATE_REQUEST");
        if (request.SessionId != runtime.InstanceId) return Result(false, "UNAUTHORIZED");
        try
        {
            return request.Command switch
            {
                "runtime.info" => Result(true, null, runtime.InspectRuntime()),
                "scene.inspect" => Scene(),
                "camera.inspect" => Camera(),
                "camera.set-test-pose" => SetPose(),
                "session.close" => Result(true, null, new XycSessionClosed(true)),
                _ => Result(false, "UNKNOWN_COMMAND")
            };
        }
        catch (UnauthorizedAccessException) { return Result(false, "UNAUTHORIZED"); }
        catch (JsonException) { return Result(false, "INVALID_INPUT"); }
        catch (InvalidOperationException) { return Result(false, "NOT_READY"); }
        catch (OperationCanceledException) { return Result(false, "CANCELLED"); }
        catch (TimeoutException) { return Result(false, "TIMEOUT"); }
        catch { return Result(false, "EXECUTION_FAILED"); }

        XycResponse Scene()
        {
            var scene = runtime.InspectScene();
            return Result(true, null, scene, scene.SceneRevision);
        }
        XycResponse Camera()
        {
            var camera = runtime.InspectCamera();
            return Result(true, null, camera, runtime.InspectScene().SceneRevision,
                camera.Revision);
        }
        XycResponse SetPose()
        {
            if (!diagnosticWrite) return Result(false, "UNAUTHORIZED");
            if (request.Args.ValueKind != JsonValueKind.Object) throw new JsonException();
            var pose = request.Args.Deserialize<XycCamera>(Json) ??
                throw new JsonException("camera pose missing");
            Validate(pose);
            var before = runtime.InspectCamera();
            var after = runtime.SetTestPose(pose);
            return Result(true, null, new { before, after },
                runtime.InspectScene().SceneRevision, after.Revision);
        }
    }

    static void Validate(XycCamera pose)
    {
        var values = new[] { pose.Position.X, pose.Position.Y, pose.Position.Z,
            pose.Forward.X, pose.Forward.Y, pose.Forward.Z, pose.Up.X,
            pose.Up.Y, pose.Up.Z, pose.FovDegrees, pose.NearPlane, pose.FarPlane,
            pose.OrthographicScale, pose.ObservationCenter.X,
            pose.ObservationCenter.Y, pose.ObservationCenter.Z };
        if (values.Any(value => !double.IsFinite(value))) throw new JsonException();
        if (pose.Mode is not ("Perspective" or "Orthographic") || pose.NearPlane <= 0 ||
            pose.FarPlane <= pose.NearPlane || pose.FovDegrees is <= 1 or >= 179 ||
            pose.Mode == "Orthographic" && pose.OrthographicScale <= 0)
            throw new JsonException();
        var f = pose.Forward;
        var u = pose.Up;
        if (Length(f) < 1e-8 || Length(u) < 1e-8 ||
            Math.Abs(Dot(f, u) / (Length(f) * Length(u))) > 1e-6)
            throw new JsonException();
    }

    static double Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    static double Length(Vector3 value) => Math.Sqrt(Dot(value, value));
}
