using Core.Interfaces;
using Core.Services;
using GUI.ViewModels;
using Microsoft.Extensions.Logging;

namespace GUI;

public static class MauiProgram
{
    public static IServiceProvider? ServiceProvider { get; private set; }

    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<ICertificateManager, CertificateManager>();
        builder.Services.AddSingleton<IRenewalScheduler, RenewalScheduler>();
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddSingleton<AppShell>();



#if DEBUG
        builder.Logging.AddDebug();
#endif
        var app = builder.Build();
        ServiceProvider = app.Services; // 👈 Save the provider

        return app;

        //return builder.Build();
    }
}
