using System.Diagnostics;
using System.Text.Json;
using XuanYu.Editor.UI;

namespace XuanYu.Editor.UI.XycControl;

public sealed class XycCommandProcessor(IXycRuntime runtime, bool diagnosticWrite,
    IXycUiRuntime? uiRuntime = null)
{
    static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    readonly HashSet<string> _requestIds = new(StringComparer.Ordinal);

    public XycResponse Execute(XycRequest request) =>
        ExecuteAsync(request, CancellationToken.None).GetAwaiter().GetResult();

    public async Task<XycResponse> ExecuteAsync(XycRequest request,
        CancellationToken cancellationToken)
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
            if (request.Command.StartsWith("ui.", StringComparison.Ordinal))
                return await XycUiCommandProcessor.ExecuteAsync(request, uiRuntime,
                    Result, cancellationToken);
            return request.Command switch
            {
                "runtime.info" => Result(true, null, runtime.InspectRuntime()),
                "scene.inspect" => Scene(),
                "camera.inspect" => Camera(),
                "camera.set-test-pose" => SetPose(),
                "camera.orbit" => Orbit(),
                "camera.dolly" => Dolly(),
                "capture.arm" => XycSilentCaptureCommands.Arm(request, runtime, diagnosticWrite, Result),
                "capture.release" => XycSilentCaptureCommands.Release(request, runtime, diagnosticWrite, Result),
                "session.close" => Result(true, null, new XycSessionClosed(true)),
                _ => Result(false, "UNKNOWN_COMMAND")
            };
        }
        catch (UnauthorizedAccessException) { return Result(false, "UNAUTHORIZED"); }
        catch (JsonException) { return Result(false, "INVALID_INPUT"); }
        catch (XycUiCommandException error) { return Result(false, error.Code); }
        catch (InvalidOperationException error)
        {
            if (request.Command.StartsWith("ui.", StringComparison.Ordinal))
                Console.Error.WriteLine($"XYC UI NOT_READY: {error.Message}");
            return Result(false, "NOT_READY");
        }
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
            XycCameraValidator.Validate(pose);
            var before = runtime.InspectCamera();
            var after = runtime.SetTestPose(pose);
            return Result(true, null, new { before, after },
                runtime.InspectScene().SceneRevision, after.Revision);
        }

        XycResponse Orbit()
        {
            if (!diagnosticWrite) return Result(false, "UNAUTHORIZED");
            var delta = request.Args.Deserialize<XycOrbitDelta>(Json) ??
                throw new JsonException("orbit delta missing");
            if (!double.IsFinite(delta.DeltaX) || !double.IsFinite(delta.DeltaY) ||
                Math.Abs(delta.DeltaX) > 5000 || Math.Abs(delta.DeltaY) > 5000)
                throw new JsonException("orbit delta outside the supported range");
            var before = runtime.InspectCamera();
            var after = runtime.OrbitCamera(delta.DeltaX, delta.DeltaY);
            return Result(true, null, new { before, after },
                runtime.InspectScene().SceneRevision, after.Revision);
        }
        XycResponse Dolly() => XycDollyCommand.Execute(request, runtime, diagnosticWrite, Result);
    }

}
