using CommunityToolkit.Maui.Alerts;
using GNS.Pages;
using Services;
using Services.Intrefaces;
using System.Text.Json;

namespace GNS;

public partial class App : Application
{
	public static Action OnResumeHandler;
	public static Action OnPauseHandler;
	private User _user;

	public App(ILoginService loginService, User user)
	{
		InitializeComponent();

		_user = user;

		HttpService.OnUnauthorizedHandler += (object o, EventArgs e) =>
		{
			Dispatcher.Dispatch(async () => await LoginPage.ShowLoginPopup(loginService, _user));
		};

		MainPage = new AppShell(loginService, _user);
	}

	protected override async void OnStart()
	{
		base.OnStart();

		await SecureStorage.SetAsync("base_address", "http://10.0.2.2:8000");
		//await SecureStorage.SetAsync("base_address", "http://172.20.130.172:8003");
		//await SecureStorage.SetAsync("base_address", "http://192.168.66.248:8000");
		//await SecureStorage.SetAsync("base_address", "http://172.20.130.136:8000");

		LoginService.BaseAddress = await SecureStorage.GetAsync("base_address");

		var savedUser = await User.GetUser();

		if (savedUser is null)
		{
			_user.Id = Guid.NewGuid();
			_user.Name = "Гость";
			_user.Type = UserType.Guest;
		}
		else 
		{
			_user.Id = savedUser.Id;
			_user.Name = savedUser.Name;
			_user.Type = savedUser.Type;
		}
	}

	protected override void OnResume()
	{
		base.OnResume();
		OnResumeHandler?.Invoke();
	}

	protected override void OnSleep()
	{
		base.OnSleep();
		OnPauseHandler?.Invoke();
		SecureStorage.SetAsync("user", JsonSerializer.Serialize(_user));
	}

	public static void ClearAppEvents()
	{
		OnResumeHandler = null;
		OnPauseHandler = null;
	}
}

