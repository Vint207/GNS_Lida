#if ANDROID
using CommunityToolkit.Maui.Alerts;
using GNS.Platforms.Android; 
#endif
using Services;

namespace GNS.Pages;

public partial class SelectOperationPage : ContentPage
{
	public SelectOperationPage()
	{
		InitializeComponent();
	}


	protected override bool OnBackButtonPressed()
	{
		Dispatcher.Dispatch(async () =>
		{
			await Shell.Current.GoToAsync($"..", true);
		});
		return true;
	}


	private async void ButtonStatusWrite_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"{nameof(ChangeStatusPage)}", true);
	}


	private async void ButtonChangeNfc_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"{nameof(ChangeNfcPage)}", true);
	}


	private async void ButtonControlWeighing_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"{nameof(ControlWeighingPage)}", true);
	}


	private async void ButtonСarouselSettings_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"{nameof(CarouselSettingsPage)}", true);
	}

	private async void UpdateApp_Button_Clicked(object sender, EventArgs e)
	{
#if ANDROID
	try 
	{	        
		var newVersionAvailableResult = await ApkUpdater.IsNewVersionAvailable();
		if (newVersionAvailableResult.IsError)
		{
			await Toast.Make(newVersionAvailableResult.FirstError.Description, CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
			return;
		}

		bool newVersionAvailable = newVersionAvailableResult.Value;

		if (newVersionAvailable) 
		{
			await Toast.Make($"Загрузка начата", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
			await ApkUpdater.DownloadAndInstallApk();
		}
		else
			await Toast.Make("Установлена последняя версия", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
	}
	catch (global::System.Exception ex)
	{
		await Toast.Make($"{ex.Message}", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
	}
#endif
	}
}