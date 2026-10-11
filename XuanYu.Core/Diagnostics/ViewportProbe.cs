using System.Reflection;
using System.Threading;

namespace XuanYu.Core.Diagnostics;

public static partial class ViewportProbe
{
    static readonly object Sync = new();
    static readonly AsyncLocal<long> Wheel = new();
    static readonly AsyncLocal<long> NativeInput = new();
    static bool _initialized;
    static long _frame;
    static long _wheel;
    static long _lastWheel;
    static long _nativeInput;
    static long _lastNativeInput;
    static string _directory = "";

    public static bool Enabled => string.Equals(
        Environment.GetEnvironmentVariable("XYE_PROBE_VIEWPORT"), "1", StringComparison.Ordinal);
    public static long CurrentFrameId => Interlocked.Read(ref _frame);
    public static long CurrentWheelEventId => Wheel.Value != 0 ? Wheel.Value : Interlocked.Read(ref _lastWheel);
    public static long CurrentNativeInputId => NativeInput.Value != 0
        ? NativeInput.Value : Interlocked.Read(ref _lastNativeInput);

    public static void Initialize()
    {
        if (!Enabled || _initialized) return;
        lock (Sync)
        {
            if (_initialized) return;
            _directory = Environment.GetEnvironmentVariable("XYE_PROBE_DIRECTORY") ??
                Path.Combine(Path.GetTempPath(), "xye-viewport-probe");
            Directory.CreateDirectory(_directory);
            _initialized = true;
            Log("runtime", $"[RUNTIME-PROBE] ProbeId={ViewportProbeIdentity.Current()};ProcessId={Environment.ProcessId}");
            Log("runtime", $"ExecutablePath={Environment.ProcessPath};CoreAssembly={Assembly.GetExecutingAssembly().Location}");
            foreach (var name in new[] { "XuanYu.Editor", "XuanYu.Editor.UI", "XuanYu.World", "XuanYu.Render.Vulkan" })
                Log("runtime", $"{name}Assembly={AssemblyPath(name)}");
        }
    }

    public static long BeginFrame()
    {
        if (!Enabled) return 0;
        Initialize();
        return Interlocked.Increment(ref _frame);
    }

    public static void SetWheel(long id) { Wheel.Value = id; Interlocked.Exchange(ref _lastWheel, id); }
    public static long NewWheel() => !Enabled ? 0 : Interlocked.Increment(ref _wheel);
    public static long NewNativeInput() => !Enabled ? 0 : Interlocked.Increment(ref _nativeInput);
    public static long SetNativeInput(long id)
    {
        var previous = NativeInput.Value;
        if (id != 0) { NativeInput.Value = id; Interlocked.Exchange(ref _lastNativeInput, id); }
        return previous;
    }
    public static void RestoreNativeInput(long id) => NativeInput.Value = id;

    public static void RefreshLoadedAssemblies()
    {
        if (!Enabled) return;
        Initialize();
        foreach (var name in new[] { "XuanYu.Editor", "XuanYu.Editor.UI", "XuanYu.World", "XuanYu.Render.Vulkan" })
            Log("runtime", $"{name}Assembly={AssemblyPath(name)}");
    }

    public static void Log(string file, string message)
    {
        if (!Enabled) return;
        Initialize();
        var line = $"{DateTimeOffset.Now:O};Frame={CurrentFrameId};Wheel={CurrentWheelEventId};" +
            $"NativeInput={CurrentNativeInputId};{message}{Environment.NewLine}";
        lock (Sync) File.AppendAllText(Path.Combine(_directory, $"{file}.log"), line);
    }

    static string AssemblyPath(string name) => AppDomain.CurrentDomain.GetAssemblies()
        .FirstOrDefault(x => string.Equals(x.GetName().Name, name, StringComparison.Ordinal))?.Location ?? "NOT_LOADED";
}
