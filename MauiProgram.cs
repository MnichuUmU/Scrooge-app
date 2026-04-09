using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using Microsoft.Extensions.Logging;
using Scrooge_app.Databases;
using Scrooge_app.Views;

namespace Scrooge_app
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<LocalDbService>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<AddPeoplePage>();
            builder.Services.AddTransient<PeopleView>();
            builder.Services.AddSingleton<IPopupService , PopupService>();
            builder.Services.AddTransient<LoadingPopupView>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
