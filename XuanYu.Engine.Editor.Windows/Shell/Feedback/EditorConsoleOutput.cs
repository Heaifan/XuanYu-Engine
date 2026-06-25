using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace XuanYu.Engine.Editor.Windows.Shell.Feedback;

/// <summary>让 WinExe 在 run.bat 启动时把 Console.WriteLine 输出回父控制台。</summary>
public static class EditorConsoleOutput
{
    const int AttachParentProcess = -1;

    public static void AttachParentConsole()
    {
        if (!OperatingSystem.IsWindows()) return;
        AttachConsole(AttachParentProcess);
        Console.OutputEncoding = Encoding.UTF8;
        Trace.Listeners.Add(new ConsoleTraceListener());
        Trace.AutoFlush = true;
        Console.Error.WriteLine("[日志][Editor] 控制台输出已连接。");
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool AttachConsole(int dwProcessId);
}
