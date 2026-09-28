using System.Reflection;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Platform;

namespace XuanYu.World.Tests.UiRuntime;

internal static class UiTestPlatformServices
{
    internal static void Install()
    {
        var locator = typeof(AvaloniaLocator).GetProperty("CurrentMutable",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(null)!;
        Bind(locator, typeof(IFontManagerImpl), Create<IFontManagerImpl>("HeadlessFontManagerStub", "Arial"));
        Bind(locator, typeof(ICursorFactory), Create<ICursorFactory>("Avalonia.Headless.HeadlessCursorFactoryStub"));
    }

    static void Bind(object locator, Type service, object value)
    {
        var bind = locator.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Single(method => method.Name == "Bind" && method.IsGenericMethodDefinition && method.GetGenericArguments().Length == 1)
            .MakeGenericMethod(service).Invoke(locator, null)!;
        var constant = bind.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Single(method => method.Name == "ToConstant" && method.IsGenericMethodDefinition)
            .MakeGenericMethod(service);
        constant.Invoke(bind, [value]);
    }

    static T Create<T>(string typeName, params object[] args) where T : class
    {
        var type = typeof(AvaloniaHeadlessPlatformOptions).Assembly.GetType(typeName, throwOnError: true)!;
        return (T)Activator.CreateInstance(type, args)!;
    }
}
