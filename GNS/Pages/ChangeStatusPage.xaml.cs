using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using GNS.Pages.Popup;
using GNS.Pages.Templates;
using GNS.Services;
using Services.Intrefaces;

namespace GNS.Pages;

public partial class ChangeStatusPage : ContentPage
{
	public string Status
	{
		get => operationView.Ballon.Status;
		set
		{
			operationView.Ballon.Status = value;
			OnPropertyChanged(nameof(Status));
		}
	}

	private IBallonService _ballonService;

	public ChangeStatusPage(IBallonService ballonService)
	{
		InitializeComponent();

		_ballonService = ballonService;
		operationView.BallonService = _ballonService;
		operationView.WriteBallonActionHandler = (ballon) =>
		{
			ballon.Status = Status;
		};

		BindingContext = this;
	}


	#region OnBackButtonPressed
	protected override bool OnBackButtonPressed()
	{
		base.OnBackButtonPressed();

		Dispatcher.Dispatch(async () =>
		{
			if(operationView.NfcFlyoutIsOpened || operationView.SerialNumberFlyoutIsOpened)
			{
				await operationView.CloseNfcFlyout();
				await operationView.CloseSerialNumberFlyout();
			}
			else await Shell.Current.GoToAsync($"//{nameof(SelectOperationPage)}", true);
		});

		return true;
	}
	#endregion


	#region OnDisappearing
	protected async override void OnDisappearing()
	{
		base.OnDisappearing();

		await operationView.CloseNfcFlyout();
		await operationView.CloseSerialNumberFlyout();
	} 
	#endregion


	#region OnStatusClickHandler
	private async void OnStatusClickHandler(object sender, EventArgs e)
	{
		var popup = new StringOptionsPage();
		await Navigation.PushModalAsync(popup, true);
		var optionsResult = await _ballonService.GetBallonStateOptions();

		if(optionsResult.IsError)
		{
			await Toast.Make($"{optionsResult.FirstError.Description}", ToastDuration.Long, 16).Show();

			if(Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		}

		var options = optionsResult.Value;
		if(options is null || !options.Any())
		{
			if(Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		}

		popup.Options = options.ToList().ToObservableCollection();
		popup.ItemSelected += async (e, selectedItem) =>
		{
			Dispatcher.Dispatch(() =>
			{
				Status = selectedItem.ToString();
			});

			if(Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		};
		popup.OnBackButtonClickHandler += async () =>
		{
			if(Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		};
	} 
	#endregion
}