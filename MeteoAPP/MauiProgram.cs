using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.AspNetCore.Components.WebView.Maui;

namespace MeteoAPP;

#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.RegisterFirebaseServices()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
			});
		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddSingleton<MeteoAPP.Services.IParameterService, MeteoAPP.Services.ParameterService>();

#if DEBUG
		builder.Logging.AddDebug();
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

		return builder.Build();
	}


	private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
	{
		builder.ConfigureLifecycleEvents(events =>
		{
#if IOS
				events.AddiOS(iOS => iOS.WillFinishLaunching((_, __) => {
					CrossFirebase.Initialize();
					FirebaseCloudMessagingImplementation.Initialize();
					return false;
				}));
#elif ANDROID
				events.AddAndroid(android => android.OnCreate((activity, _) => 
				CrossFirebase.Initialize(activity)));
#endif

		});
		return builder;
	}
}
