using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Mopups.Hosting;
using Plugin.LocalNotification;

namespace ArbolesMAUI;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiMaps()
            .ConfigureMopups()
            .UseMauiCommunityToolkit()
            .UseLocalNotification();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Add handlers
        builder.ConfigureMauiHandlers(handlers => {
#if ANDROID
            // Add Map Handler
            handlers.AddHandler<Microsoft.Maui.Controls.Maps.Map, ViewModels.MapUtil.CustomMapHandler>();
#endif
        });

        return builder.Build();
    }
}