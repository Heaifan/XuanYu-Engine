using System.Reflection;
using System.IO;

namespace XuanYu.Editor.UI;

public static class F1ForensicTrace
{
    static readonly string TracePath = Path.Combine(Path.GetTempPath(), "XuanYu", "f1-region-trace.log");
    [ThreadStatic] static bool _nativeClick;

    public static bool IsNativeClick => _nativeClick;
    public static void BeginNativeClick() => _nativeClick = true;
    public static void EndNativeClick() => _nativeClick = false;

    public static void Version(Assembly app, Assembly ui)
    {
        Console.WriteLine("[F1TRACE-VERSION]");
        Console.WriteLine($"App ProductVersion={Product(app)}");
        Console.WriteLine($"UI ProductVersion={Product(ui)}");
        Console.WriteLine($"App BaseDirectory={AppContext.BaseDirectory}");
        Console.WriteLine($"App DLL path={app.Location}");
        Console.WriteLine($"UI DLL path={ui.Location}");
    }

    public static void NativePointer(UiVm vm, double x, double y, double px, double py,
        double dpi, double logicalW, double logicalH, int swapW, int swapH) =>
        Write(vm, "A", $"Message=WM_LBUTTONDOWN; logical=({x:0.##},{y:0.##}); physical=({px:0.##},{py:0.##}); dpi={dpi:0.###}; viewport=({logicalW:0.##}x{logicalH:0.##}); swapchain=({swapW}x{swapH})");

    public static void Routing(UiVm vm, double x, double y) =>
        Write(vm, "B", $"CurrentTool={vm.ActiveTool}; RegionDrawingEnabled={vm.IsRegionDrawingTool}; ActiveLayerId={vm.MapSession.ActiveRegionLayerId}; pointer=({x:0.##},{y:0.##})");

    public static void Picker(UiVm vm, bool hit, XuanYu.Core.Space.WorldRay ray, XuanYu.World.Map.MapPoint point) =>
        Write(vm, "C", $"Result={(hit ? "HIT" : "MISS")}; MapPoint=({point.X:0.##},{point.Y:0.##},0); RayOrigin={ray.Origin}; RayDirection={ray.Direction}");

    public static void Draft(UiVm vm, int oldCount, int newCount, object? preview, bool exists) =>
        Write(vm, "D", $"OldState={oldCount}; NewState={newCount}; VertexCount={newCount}; PreviewPoint={preview}; DraftExists={exists}");

    public static void Projection(UiVm vm, int vertices, int edges, int primitives, int regions, string key) =>
        Write(vm, "E", $"DraftVertexCount={vertices}; DraftEdgeCount={edges}; DraftPrimitiveCount={primitives}; RegionCount={regions}; Resource={key}");

    static string Product(Assembly assembly) =>
        assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";
    static void Write(UiVm? vm, string stage, string message)
    {
        var line = $"[{DateTimeOffset.Now:O}] [F1TRACE-{stage}] {message}";
        vm?.LogF1Trace(stage, message);
        Console.WriteLine(line);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(TracePath)!);
            File.AppendAllText(TracePath, line + Environment.NewLine);
        }
        catch (IOException error) { Console.WriteLine($"[F1TRACE-ERROR] {error.Message}"); }
    }
}
