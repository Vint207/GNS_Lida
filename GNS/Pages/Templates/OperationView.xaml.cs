using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ErrorOr;
using GNS.Pages.Popup;
using GNS.XamlHelpers.Converters;
using Microsoft.Maui;
using Services;
using Services.Intrefaces;
using static Services.Intrefaces.IBallonService;

namespace GNS.Pages.Templates;

public partial class OperationView : ContentView
{

	public OperationView()
	{
		InitializeComponent();

		_ballon = new();
		nfcScanView.OnScanSuccessHandler = OnNfcScanSuccessHandler;
		nfcScanView.OnBackButtonClickHandler = OnBackButtonClickHandler;
		Flyout.OnFlyoutOpenHandler = nfcScanView.OnAppearing;
		Flyout.OnFlyoutCloseHandler = nfcScanView.OnDisappearing;
		SerialNumberInputView.OnButtonSearchClickHandler = ButtonSerialNumberSearchFlyout_Clicked;
	}
	

	#region Properties
	public IList<IView> MainContent => mainContent.Children;
	public IList<IView> SubMainContent => subMainContent.Children;

	private IBallonService _ballonService;
	public IBallonService BallonService
	{
		get => _ballonService;
		set
		{
			_ballonService = value;
		}
	} 

	private BallonVM? _ballon;
	public BallonVM Ballon
	{
		get => _ballon;
		set
		{
			_ballon = value;
		}
	}

	public static readonly BindableProperty SerialNumberFlyoutIsOpenedProperty =
		BindableProperty.Create(
			propertyName: nameof(SerialNumberFlyoutIsOpened),
			returnType: typeof(bool),
			declaringType: typeof(OperationView),
			defaultValue: false,
			defaultBindingMode: BindingMode.TwoWay);
	public bool SerialNumberFlyoutIsOpened 
	{
		get => (bool)GetValue(SerialNumberFlyoutIsOpenedProperty); 
		set => SetValue(SerialNumberFlyoutIsOpenedProperty, value); 
	}

	public static readonly BindableProperty NfcFlyoutIsOpenedProperty =
		BindableProperty.Create(
			propertyName: nameof(NfcFlyoutIsOpened),
			returnType: typeof(bool),
			declaringType: typeof(OperationView),
			defaultValue: false,
			defaultBindingMode: BindingMode.TwoWay);
	public bool NfcFlyoutIsOpened
	{
		get => (bool)GetValue(NfcFlyoutIsOpenedProperty);
		set => SetValue(NfcFlyoutIsOpenedProperty, value);
	}

	public string ButtonNfcText
	{
		get => nfcBtn.Text;
		set => nfcBtn.Text = value;
	}

	public string ButtonSerialNumberText
	{
		get => ButtonSerialNumberSearch.Text;
		set => ButtonSerialNumberSearch.Text = value;
	}

	public Action<BallonVM> WriteBallonActionHandler { get; set; }
	public Action<BallonVM> OnBallonsListItemSelecedAlterHandler { get; set;}
	public Action<string> OnNfcScanSuccessAlterHandler { get; set;}
	#endregion


	private void NfcBtn_Clicked(object sender, EventArgs e)
	{
		Flyout.ToggleFlyout();		
	}


	private async Task<ErrorOr<Success>> UpdateBallonAsync(string nfc, BallonVM ballon)
	{
		return await _ballonService.UpdateBallonByNfc(nfc, ballon);
	}


	private async void OnNfcScanSuccessHandler(string nfc)
	{
		if (OnNfcScanSuccessAlterHandler is null)
		{
			var getBallonResult = await _ballonService.GetBallonByNfc(nfc.ToLowerInvariant());

			if (getBallonResult.IsError)
			{
				await Toast.Make($"{getBallonResult.FirstError.Description}", ToastDuration.Long, 16).Show();
				return;
			}

			var ballon = getBallonResult.Value;

			if (ballon is null)
				return;

			WriteBallon(ballon);
		}
		else { OnNfcScanSuccessAlterHandler?.Invoke(nfc); }
	}


	public void WriteBallon(BallonVM ballon)
	{
		try
		{
			Dispatcher.Dispatch(async () =>
			{
				if(ballon is not null)
				{
					WriteBallonActionHandler?.Invoke(ballon);
					var updateBallonResult = await UpdateBallonAsync(ballon?.NFC_Tag, ballon);
					if(!updateBallonResult.IsError)
					{
						await Toast.Make("Свойство записано", ToastDuration.Short, 16).Show(); 
					}
					else await Toast.Make($"{updateBallonResult.FirstError.Description}", ToastDuration.Short, 16).Show();

					await Task.Delay(500);
				}
				else
					await Toast.Make("Баллон не найден", ToastDuration.Short, 16).Show();
			});
		}
		catch(Exception)
		{

			throw;
		}

	}


	private void OnBackButtonClickHandler()
	{
		Flyout.ToggleFlyout();
	}


	public async Task CloseNfcFlyout()
	{
		await Flyout.CloseFlyout();
	}


	public async Task OpenNfcFlyout()
	{
		await Flyout.OpenFlyout();
	}


	public async Task OpenSerialNumberFlyout()
	{
		await FlyoutSerialNumberInput.OpenFlyout();
	}


	public async Task CloseSerialNumberFlyout()
	{
		await FlyoutSerialNumberInput.CloseFlyout();
	}


	private void ButtonSerialNumberSearch_Clicked(object sender, EventArgs e)
	{
		FlyoutSerialNumberInput.ToggleFlyout();
	}


	private async void ButtonSerialNumberSearchFlyout_Clicked()
	{
		string number = SerialNumberInputView.SerialNumber?.ToLowerInvariant();

		if (string.IsNullOrWhiteSpace(number))
			await Toast.Make("Некорректный серийный номер", ToastDuration.Short, 16).Show();
		else
		{
			var popup = new BallonsListPage(number, _ballonService);

			popup.ItemSelected += async (e, selectedItem) =>
			{
				Dispatcher.Dispatch(async () =>
				{
					if (selectedItem is BallonVM ballon)
					{
						if (OnBallonsListItemSelecedAlterHandler is null) { WriteBallon(ballon); }						
						else { OnBallonsListItemSelecedAlterHandler?.Invoke(ballon); }
					}
				});

				if (Navigation.ModalStack.Count > 0)
					await Navigation.PopModalAsync(true);
			};

			await Navigation.PushModalAsync(popup, true);
		}

		OnBallonsListItemSelecedAlterHandler = null;
	}
}