using CommunityToolkit.Maui.Alerts;
using GNS.Pages.Templates;
using Services;
using Services.Intrefaces;
using static Services.Intrefaces.IBallonService;
using CommunityToolkit.Maui.Core;

namespace GNS.Pages;

public partial class ControlWeighingPage : ContentPage
{
	private BallonVM _ballon;
	private float _brutto;
	public float Brutto
	{
		get => _brutto;
		set
		{
			_brutto = value;
			OnPropertyChanged(nameof(Brutto));
		}
	}

	private string? _serialNumberPassport;
	public string? SerialNumberPassport
	{
		get => _serialNumberPassport;
		set
		{
			if (value is not null)
				_serialNumberPassport = value;
			else
				_bruttoPassport = "нет";

			PropertyExpander.ResizeExpander(Popup.Expander.ResizeMode.Resize);
			OnPropertyChanged();
		}
	}

	private string? _bruttoPassport;
	public string? BruttoPassport
	{
		get => _bruttoPassport?.ToString();
		set
		{
			if (value is not null)
				_bruttoPassport = value;
			else
				_bruttoPassport = "нет";

			PropertyExpander.ResizeExpander(Popup.Expander.ResizeMode.Resize);
			OnPropertyChanged();
		}
	}

	private string? _statusPassport;
	public string? StatusPassport
	{
		get => _statusPassport;
		set
		{
			if (value is not null)
				_statusPassport = value;			
			else
				_statusPassport = "нет";

			PropertyExpander.ResizeExpander(Popup.Expander.ResizeMode.Resize);
			OnPropertyChanged();
		}
	}

	private IBallonService _ballonService;

	public ControlWeighingPage(IBallonService ballonService)
	{
		InitializeComponent();
		BindingContext = this;

		_ballonService = ballonService;
		operationView.BallonService = _ballonService;
		operationView.ButtonNfcText = "Найти по метке";
		operationView.ButtonSerialNumberText = "Найти по номеру";
		operationView.WriteBallonActionHandler = (ballon) =>
		{
			ballon.Brutto = Brutto;
			ballon.Status = "Контрольное взвешивание";
		};
	}

	protected override bool OnBackButtonPressed()
	{
		base.OnBackButtonPressed();

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

	protected async override void OnAppearing()
	{
		base.OnAppearing();

		operationView.OnNfcScanSuccessAlterHandler = async (nfc) =>
		{
			var getBallonResult = await _ballonService.GetBallonByNfc(nfc.ToLowerInvariant());

			if (getBallonResult.IsError)
			{
				await Toast.Make($"{getBallonResult.FirstError.Description}", ToastDuration.Long, 16).Show();
				return;
			}

			_ballon = getBallonResult.Value;
			await CheckBallon();
			await operationView.CloseNfcFlyout();
		};

		operationView.OnBallonsListItemSelecedAlterHandler = async (ballon) =>
		{
			_ballon = ballon;
			await CheckBallon();
			await operationView.CloseSerialNumberFlyout();
		};
	}

	private async Task CheckBallon()
	{
		if (_ballon is not null)
		{
			SerialNumberPassport = _ballon.Serial_Number;
			BruttoPassport = _ballon.Brutto?.ToString();
			StatusPassport = _ballon.Status;
			await Task.Delay(400);
			PropertyExpander.ResizeExpander(Popup.Expander.ResizeMode.Open);
			ButtonControlWeight.IsEnabled = true;
		}
		else
		{
			PropertyExpander.ResizeExpander(Popup.Expander.ResizeMode.Close);
			await Toast.Make("Баллон не найден", ToastDuration.Long, 16).Show();
		}
	}

	protected async override void OnDisappearing()
	{
		base.OnDisappearing();

		await operationView.CloseNfcFlyout();
		await operationView.CloseSerialNumberFlyout();
	}

	private void ButtonControlWeight_Clicked(object sender, EventArgs e)
	{
		operationView.WriteBallon(_ballon);
		BruttoPassport = Brutto.ToString();
		StatusPassport = "Контрольное взвешивание";
		ButtonControlWeight.IsEnabled = false;
	}
}