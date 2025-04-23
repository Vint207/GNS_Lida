using GNS.Models;
using GNS.Pages;
using GNS.Pages.Popup;
using Services;
using Services.Intrefaces;
using System.ComponentModel;
using System.Text.Json;
using System.Windows.Input;

namespace GNS;

public partial class AppShell : Shell
{
	private ILoginService _loginService;
	private User _user;
	public UserViewModel UserViewModel { get; set; }

	public AppShell(ILoginService loginService, User user)
	{
		InitializeComponent();

		_user = user;
		_loginService = loginService;
		UserViewModel = new UserViewModel(_user);

		UserViewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
		{
			if (UserViewModel.Type is UserType.Guest )
				{ buttonLogout.Text = "Гость"; }			
			else if (UserViewModel.Type is UserType.Operator)
				{ buttonLogout.Text = "Опер."; }
			else
				{ buttonLogout.Text = "Админ."; }				
		};			

		Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
		Routing.RegisterRoute(nameof(ChangeStatusPage), typeof(ChangeStatusPage));
		Routing.RegisterRoute(nameof(ChangeNfcPage), typeof(ChangeNfcPage));
		Routing.RegisterRoute(nameof(ControlWeighingPage), typeof(ControlWeighingPage));
		Routing.RegisterRoute(nameof(CarouselSettingsPage), typeof(CarouselSettingsPage));

		BindingContext = this;
	}

	protected async override void OnAppearing()
	{
		base.OnAppearing();
	}

	private async void LogoutBtn_Clicked(object sender, EventArgs e)
	{	
		await LoginPage.ShowLoginPopup(_loginService, _user);
		await _loginService.Logout();
	}	
}

