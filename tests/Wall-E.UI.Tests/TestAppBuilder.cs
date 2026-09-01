using Avalonia;
using Avalonia.Headless;
using Wall_E.UI.Avalonia;

[assembly: AvaloniaTestApplication(typeof(Wall_E.UI.Tests.TestAppBuilder))]

namespace Wall_E.UI.Tests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}