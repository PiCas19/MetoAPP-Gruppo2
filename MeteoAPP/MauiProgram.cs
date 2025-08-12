using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.AspNetCore.Components.WebView.Maui;
using MeteoAPP.Services;

namespace MeteoAPP;

#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

/// <summary>
/// Classe statica responsabile dell'inizializzazione dell'app .NET MAUI.
/// Configura servizi, font, Firebase e l'ambiente Blazor.
/// </summary>
public static class MauiProgram
{

	/// <summary>
	/// Punto di ingresso per la creazione dell'applicazione MAUI.
	/// Configura dipendenze, servizi Firebase, Blazor, logging e font.
	/// </summary>
	/// <returns>Istanza configurata di <see cref="MauiApp"/> pronta all'uso.</returns>
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
		builder.Services.AddSingleton<MeteoAPP.Services.DatabaseService>();
		builder.Services.AddSingleton<AppwriteSyncService>();

#if DEBUG
		builder.Logging.AddDebug();
		builder.Services.AddBlazorWebViewDeveloperTools();
#endif

		return builder.Build();
	}

	/// <summary>
	/// Metodo di estensione che registra e inizializza i servizi Firebase.
	/// Supporta sia Android che iOS tramite eventi specifici del ciclo di vita.
	/// </summary>
	/// <param name="builder">Il builder MAUI da configurare.</param>
	/// <returns>Il builder aggiornato con il supporto Firebase.</returns>
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
