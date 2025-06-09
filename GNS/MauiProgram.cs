using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Alerts;
using GNS.Pages;
using GNS.Pages.Popup;
using GNS.Pages.Templates;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.LifecycleEvents;
using Services;
using Services.Intrefaces;
using SkiaSharp.Views.Maui.Controls.Hosting;
using GNS.Models;
using Syncfusion.Maui.Toolkit.Hosting;


#if ANDROID
using GNS.Platforms.Android;
#endif

namespace GNS;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		builder
			.UseMauiApp<App>()
			.UseSkiaSharp()
			//.UseBarcodeReader()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionToolkit()
			.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				handlers.AddHandler(typeof(DatePicker), typeof(Platforms.Android.Handlers.MyDatePickerHandler));
#endif
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// регистрация сервисов
		builder.Services.AddSingleton<IBallonService, BallonService>();
		builder.Services.AddSingleton<IBatchService, BatchService>();
		builder.Services.AddSingleton<ICarouselService, CarouselService>();
		builder.Services.AddSingleton<ILoginService, LoginService>();
		builder.Services.AddSingleton<User>();
			
		// регистрация страниц
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<StringOptionsPage>();
		builder.Services.AddSingleton<BatchListPopup>();
		builder.Services.AddSingleton<CarCollectionPopup>();
		builder.Services.AddSingleton<TrailerCollectionPopup>();
		builder.Services.AddSingleton<LoadingSelectorPage>();
		builder.Services.AddSingleton<UnloadingSelectorPage>();
		builder.Services.AddSingleton<EditBallonPage>();
		builder.Services.AddSingleton<LoginPage>();
		builder.Services.AddSingleton<QrPage>();
		builder.Services.AddSingleton<ChangeStatusPage>();
		builder.Services.AddSingleton<CarouselSettingsPage>();

		builder.Services.AddTransient<LoadingPage>();
		builder.Services.AddTransient<OperationView>();
		builder.Services.AddTransient<SelectOperationPage>();
		builder.Services.AddTransient<ChangeNfcPage>();
		builder.Services.AddTransient<ControlWeighingPage>();
		builder.Services.AddTransient<NfcScanView>();
		builder.Services.AddTransient<PropertyEntry>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

