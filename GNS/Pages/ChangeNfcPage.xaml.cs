using CommunityToolkit.Maui.Alerts;
using Services;
using Services.Intrefaces;

namespace GNS.Pages;

public partial class ChangeNfcPage : ContentPage
{
	private IBallonService _ballonService;

	public string NFCTag
	{
		get => operationView.Ballon.NFC_Tag;
		set
		{
			operationView.Ballon.NFC_Tag = value?.ToLowerInvariant();
			OnPropertyChanged();
		}
	}

	public ChangeNfcPage(IBallonService ballonService)
	{
		InitializeComponent();

		_ballonService = ballonService;
		operationView.BallonService = _ballonService;
		operationView.WriteBallonActionHandler = (ballon) =>
		{
			ballon.NFC_Tag = NFCTag;
		};

		BindingContext = this;
	}

	protected override bool OnBackButtonPressed()
	{
		Dispatcher.Dispatch(async () =>
		{
			if (operationView.NfcFlyoutIsOpened || operationView.SerialNumberFlyoutIsOpened)
			{
				await operationView.CloseNfcFlyout();
				await operationView.CloseSerialNumberFlyout();
			}
			else
			{
				await Shell.Current.GoToAsync($"//{nameof(SelectOperationPage)}", true);
			}
		});
		return true;
	}

	protected async override void OnDisappearing()
	{
		base.OnDisappearing();

		await operationView.CloseNfcFlyout();
		await operationView.CloseSerialNumberFlyout();
	}

	private async void OnNFCTagClickHandler(object sender, EventArgs e)
	{
		await operationView.OpenNfcFlyout();

		operationView.OnNfcScanSuccessAlterHandler = (string nfc) =>
		{
			Dispatcher.Dispatch(async () =>
			{			
				NFCTag = nfc;
				await operationView.CloseNfcFlyout();
				operationView.OnNfcScanSuccessAlterHandler = null;
			});
		};
	}
}