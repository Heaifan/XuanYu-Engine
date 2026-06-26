using System.Reflection;
using Avalonia.Controls;

namespace XuanYu.Engine.Editor.Windows;

public sealed partial class MainWindowV2 : Window
{
    public MainWindowV2()
    {
        InitializeComponent();
        var ver = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        if (!string.IsNullOrEmpty(ver)) Title = $"{Title} {ver}";
    }
}
