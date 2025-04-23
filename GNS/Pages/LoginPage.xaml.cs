using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.Controls;
using Services;
using Services.Intrefaces;
using System.Text.Json;

namespace GNS.Pages;

public partial class LoginPage : ContentPage
{
	public Action OnLoginSuccessHander;
	ILoginService _loginService;
	private User _user;

	public LoginPage(ILoginService loginService, User user)
	{
		InitializeComponent();

		_user = user;
		_loginService = loginService;
	}

	protected override void OnDisappearing()
	{
		base.OnDisappearing();
		OnLoginSuccessHander = null;
	}

	protected override bool OnBackButtonPressed()
	{
		Dispatcher.Dispatch(async () =>
		{
			await Shell.Current.GoToAsync($"..", true);
		});
		return true;
	}

	private async void ButtonEnter_Clicked(object sender, EventArgs e)
	{
		var loginResult = await _loginService.Login(
			new ILoginService.LoginDataVm(entryLogin.Text, entryPassword.Text));

		if (!loginResult.IsError)
		{
			OnLoginSuccessHander?.Invoke();
			_user.Name = "Администратор";
			_user.Type = UserType.Admin;
			await Toast.Make("Вход выполнен", CommunityToolkit.Maui.Core.ToastDuration.Short, 16).Show();
		}
		else
		{
			_user.Name = "Гость";
			_user.Type = UserType.Guest;
			await Toast.Make("Неверные данные", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
		}

		try
		{
			await SecureStorage.SetAsync("user", JsonSerializer.Serialize(_user));
		}
		catch (Exception ex)
		{
			await Toast.Make($"{ex.Message}", CommunityToolkit.Maui.Core.ToastDuration.Long, 16).Show();
		}
	}

	public static async Task ShowLoginPopup(ILoginService loginService, User user)
	{
		await Task.Delay(300);

		var loginPopup = new LoginPage(loginService, user);
		var page = Shell.Current?.CurrentPage;

		loginPopup.OnLoginSuccessHander += async () =>
		{
			if (page?.Navigation.ModalStack.Count > 0)
				await page?.Navigation.PopModalAsync();
		};
		await page?.Navigation.PushModalAsync(loginPopup);	
	}
}