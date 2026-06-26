using System.Reflection;
using Avalonia.Controls;

namespace XuanYu.Engine.Editor.Windows;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Title = $"{Title} {ReadEditorVersion()}";
    }

    static string ReadEditorVersion()
    {
        var asm = Assembly.GetExecutingAssembly();
        var info = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (!string.IsNullOrEmpty(info)) return info;
        return asm.GetName().Version?.ToString() ?? "未知版本";
    }
}
