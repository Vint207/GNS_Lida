#if ANDROID
//using GNS.Platforms.Android;
#endif
using CommunityToolkit.Maui.Alerts;
using System;

namespace GNS.Pages;

public partial class QrPage : ContentPage
{
#if ANDROID  
	//QRScanner _qrScanner;
#endif

	public QrPage()
	{
		//InitializeComponent();
#if ANDROID
		//_qrScanner = new QRScanner();
		//_qrScanner.OnPermissionDeniedHandler += async (object s, EventArgs e) => 
		//{
		//	await Toast.Make("Предоставьте приложению права на использование камеры", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
		//	await Shell.Current.GoToAsync($"//{nameof(MainPage)}", true);
		//};
#endif
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
#if ANDROID
		//App.OnResumeHandler += async (object s, EventArgs e) => await _qrScanner.ScanAsync();
		//_qrScanner.OnTagDetectedHandler += HandleQRTagDetected;
		//await _qrScanner.ScanAsync();
#endif
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
#if ANDROID
		App.ClearAppEvents();
#endif
	}

	private void BtnMainMenu_Clicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//" + nameof(MainPage), true);
	}

	private async void BtnScan_Clicked(object sender, EventArgs e)
	{
#if ANDROID
		//await _qrScanner.ScanAsync();
#endif
	}

#if ANDROID
	private void BtnCancel_Clicked(object sender, EventArgs e)
	{
		//_qrScanner.StopScan();
	}
#endif

	private async void HandleQRTagDetected(string tag)
	{
		//await Shell.Current.GoToAsync($"//{nameof(EditBallonPage)}?NFC={tag}", true);		
		//await Shell.Current.GoToAsync($"//{nameof(EditBallonPage)}", true, new Dictionary<string, object>()
		//{
		//	{"NFC", tag}
		//});
	}
}
