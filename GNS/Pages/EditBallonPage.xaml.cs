using GNS.Pages.Templates;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using System.ComponentModel;
using GNS.Pages.Popup;
using CommunityToolkit.Maui.Animations;
using Services.Intrefaces;
using GNS.XamlHelpers.Converters;
using Microsoft.Maui.Controls;
using static Services.Intrefaces.IBallonService;
using CommunityToolkit.Maui.Core;
using ErrorOr;
using CommunityToolkit.Maui.Core.Extensions;
using GNS.XamlHelpers.Notifications;


namespace GNS.Pages;

public partial class EditBallonPage : ContentPage, INotifyPropertyChanged
{
	#region Свойства
	private readonly IBallonService _ballonService;
	private BallonVM? _ballon;
	public BallonVM Ballon
	{
		get => _ballon;
		set
		{
			_ballon = new()
			{		
				NFC_Tag = value.NFC_Tag
			};

			NFCTag = value.NFC_Tag;
			SerialNumber = value.Serial_Number;
			CreationDate = value.Creation_Date?.ToDateTime(default);
			Size = value.Size;
			Netto = value.Netto;
			Brutto = value.Brutto;
			CurrentExaminationDate = value.Current_Examination_Date?.ToDateTime(default);
			NextExaminationDate = value.Next_Examination_Date?.ToDateTime(default);
			Status = value.Status;		
		}
	}
	public string NFCTag
	{
		get => _ballon?.NFC_Tag;
		set
		{
			_isPopupNavigation = false;
			_ballon.NFC_Tag = value?.ToLowerInvariant();
			OnPropertyChanged(nameof(NFCTag));
		}
	}
	public string SerialNumber
	{
		get => _ballon?.Serial_Number;
		set
		{
			_ballon.Serial_Number = value;
			OnPropertyChanged(nameof(SerialNumber));
		}
	}
	public DateTime? CreationDate
	{
		get => _ballon.Creation_Date?.ToDateTime(default);	
		set
		{
			if (value is DateTime date)
			{
				creation_Date.DateEntry.Opacity = 1;
				_ballon.Creation_Date = DateOnly.FromDateTime(date);
			}
			else
			{
				creation_Date.DateEntry.Opacity = 0;
				_ballon.Creation_Date = null;
			}

			OnPropertyChanged(nameof(CreationDate));
		}
	}
	public float? Size
	{
		get => _ballon?.Size;
		set
		{
			_ballon.Size = value;
			OnPropertyChanged(nameof(Size));
		}
	}
	public float? Netto
	{
		get => _ballon?.Netto;
		set
		{
			_ballon.Netto = value;
			OnPropertyChanged(nameof(Netto));
		}
	}
	public float? Brutto
	{
		get => _ballon?.Brutto;
		set
		{
			_ballon.Brutto = value;
			OnPropertyChanged(nameof(Brutto));
		}
	}
	public DateTime? CurrentExaminationDate
	{
		get => _ballon?.Current_Examination_Date?.ToDateTime(default);
		set
		{
			if (value is DateTime date)
			{
				current_Examination_Date.DateEntry.Opacity = 1;
				_ballon.Current_Examination_Date = DateOnly.FromDateTime(date);
			}
			else
			{
				current_Examination_Date.DateEntry.Opacity = 0;
				_ballon.Current_Examination_Date = null;
			}

			OnPropertyChanged(nameof(CurrentExaminationDate));
		}
	}
	public DateTime? NextExaminationDate
	{
		get => _ballon?.Next_Examination_Date?.ToDateTime(default);
		set
		{
			if (value is DateTime date)
			{
				next_Examination_Date.DateEntry.Opacity = 1;
				_ballon.Next_Examination_Date = DateOnly.FromDateTime(date);
			}
			else
			{
				next_Examination_Date.DateEntry.Opacity = 0;
				_ballon.Next_Examination_Date = null;
			}

			OnPropertyChanged(nameof(NextExaminationDate));
		}
	}
	public string Status
	{
		get => _ballon?.Status;
		set
		{
			_ballon.Status = value;
			OnPropertyChanged(nameof(Status));
		}
	}

	private bool _isPopupNavigation;

	#endregion

	public EditBallonPage(IBallonService ballonService)
	{
		InitializeComponent();
		
		Ballon = new();
		_ballonService = ballonService;
	
		BindingContext = this;

		nfcScanView.OnBackButtonClickHandler = OnBackButtonClickHandler;
		Flyout.OnFlyoutOpenHandler = nfcScanView.OnAppearing;
		Flyout.OnFlyoutCloseHandler = nfcScanView.OnDisappearing;
		SerialNumberInputView.OnButtonSearchClickHandler = ButtonSerialNumberSearchFlyout_Clicked;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		_isPopupNavigation = false;	
	}

	protected async override void OnDisappearing()
	{
		base.OnDisappearing();

		await Flyout.CloseFlyout();
		await FlyoutSerialNumberInput.CloseFlyout();
		
		if (!_isPopupNavigation)
		{
			CloseBallonForm();
			Ballon = new();
		}
	}

	protected override bool OnBackButtonPressed()
	{
		Dispatcher.Dispatch(async () =>
		{
			if (Flyout.IsOpened || FlyoutSerialNumberInput.IsOpened)
			{
				await Flyout.CloseFlyout();
				await FlyoutSerialNumberInput.CloseFlyout();
			}
			else
			{ 			
				await Shell.Current.GoToAsync($"..", true);
			}
		});

		return true;
	}

	private async void BtnSave_Clicked(object sender, EventArgs e)
	{
		if (!ballonFormExpander.IsOpened)
		{
			OpenBallonForm();
			return;
		}

		if (_ballon is null || string.IsNullOrWhiteSpace(_ballon?.NFC_Tag))
		{
			await Toast.Make("Ошибка", ToastDuration.Long, 16).Show();
			return;
		}

		var udateBallonResult = await UpdateBallonAsync(_ballon?.NFC_Tag, Ballon);
		if (udateBallonResult.IsError)
		{
			await Toast.Make($"Ошибка. {udateBallonResult.FirstError.Description}", ToastDuration.Long, 16).Show();

			return;
		}
		await Toast.Make("Баллон сохранен", ToastDuration.Long, 16).Show();
	}

	private async void ButtonCreateBallon_Clicked(object sender, EventArgs e)
	{
		if (!ballonFormExpander.IsOpened)
		{
			OpenBallonForm();
			return;
		}

		if (_ballon is null 
			|| (string.IsNullOrWhiteSpace(_ballon.Serial_Number) && string.IsNullOrWhiteSpace(_ballon.NFC_Tag))
			|| Size == 0)
		{
			await Toast.Make("Укажите серийный номер или NFC, а также размер", ToastDuration.Long, 16).Show(); 
		}
		else
		{
			var createBallonResult = await _ballonService.CreateBallon(_ballon);
			if (!createBallonResult.IsError)
			{
				await Toast.Make("Баллон создан", ToastDuration.Short, 16).Show();
			}
			else
			{
				await Toast.Make($"{createBallonResult.FirstError.Description}", ToastDuration.Long, 16).Show(); 				
			}
		}
	}

	private void NfcBtn_Clicked(object sender, EventArgs e)
	{
		nfcScanView.OnScanSuccessHandler = OnScanSuccessHandler;
		Flyout.ToggleFlyout();
	}

	private async Task GetBallonAsync(string nfc)
	{
		var getBallonByIdResult = await _ballonService.GetBallonByNfc(nfc);
		if (!getBallonByIdResult.IsError)
			Ballon = getBallonByIdResult.Value;

		string message = $"Не удалось получить баллон. {getBallonByIdResult.FirstError.Description}";
		await Toast.Make(message, ToastDuration.Long, 16).Show();
	}

	private async Task<ErrorOr<Success>> UpdateBallonAsync(string nfc, BallonVM ballon)
	{
		return await _ballonService.UpdateBallonByNfc(nfc, ballon);
	}

	private async void OnSizeClickHandler(object sender, EventArgs e)
	{
		_isPopupNavigation = true;

		var popup = new StringOptionsPage
		{
			Options = ["5", "12", "27", "50"]
		};

		popup.ItemSelected += async (e, selectedItem) =>
		{
			Dispatcher.Dispatch(() =>
			{
				var r = int.Parse(selectedItem.ToString());
				Size = r;	
			});
			OpenBallonForm();

			if (Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		};

		popup.OnBackButtonClickHandler += async () =>
		{
			OpenBallonForm();

			if (Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		};

		await Navigation.PushModalAsync(popup, true);
	}

	private async void OnStatusClickHandler(object sender, EventArgs e)
	{	
		var popup = new StringOptionsPage();

		_isPopupNavigation = true;
		await Navigation.PushModalAsync(popup, true);

		var getOptionsResult = await _ballonService.GetBallonStateOptions();

		if (getOptionsResult.IsError)
		{
			await Toast.Make(
				$"{getOptionsResult.FirstError.Description}",
				ToastDuration.Long,
				16).Show();

			if (Navigation.ModalStack.Count > 0) 
				await Navigation.PopModalAsync(true);

			return;
		}

		var options = getOptionsResult.Value?.Select(x => x.ToString());

		if (options is not null && options.Any())
			popup.Options = options.ToObservableCollection();
		else
		{
			if (Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		}

		popup.ItemSelected += async (e, selectedItem) =>
		{
			Dispatcher.Dispatch(() =>
			{
				Status = selectedItem.ToString();
			});
			OpenBallonForm();

			if (Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		};
		popup.OnBackButtonClickHandler += async () =>
		{
			OpenBallonForm();

			if (Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		};		
	}    

	private async void OpenBallonForm()
	{		
		ballonFormExpander.ResizeExpander(Popup.Expander.ResizeMode.Open);
		await Task.Delay(400);
		//await ButtonCreateBallonSpan.FadeTo(0, 300);
		ButtonCreateBallonSpan.WidthRequest = ButtonCreateBallon.Width;

		var animation = new Animation(
			h => ButtonCreateBallonSpan.WidthRequest = h,
			buttons_box.Width,
			buttons_box.Width / 2 - buttons_box.ColumnSpacing / 2);
		animation.Commit(ButtonCreateBallonSpan, "WidthAnimation", length: 200, easing: Easing.SinInOut);
		await Task.Delay(200);
		ButtonCreateBallonSpan.IsVisible = false;
	}

	private void CloseBallonForm()
	{
		ButtonCreateBallonSpan.HorizontalOptions = LayoutOptions.StartAndExpand;
		ButtonCreateBallonSpan.IsVisible = true;
		ButtonCreateBallonSpan.Opacity = 1;

		var animation = new Animation(
			h => ButtonCreateBallonSpan.WidthRequest = h,
			ButtonCreateBallonSpan.WidthRequest, 
			buttons_box.Width);
		animation.Commit(ButtonCreateBallonSpan, "WidthAnimation", length: 300, easing: Easing.SinInOut);

		ballonFormExpander.ResizeExpander(Popup.Expander.ResizeMode.Close);
		_ballon = new();
		Brutto = null;
	}

	private void BallonFormView_SizeChanged(object sender, EventArgs e)
	{
		ballonFormExpander.ResizeExpander(Popup.Expander.ResizeMode.Resize);
	}

	private void OnScanSuccessHandler(string nfc)
	{
		Dispatcher.Dispatch(async () =>
		{
			var getBallonResult= await _ballonService.GetBallonByNfc(nfc.ToLowerInvariant());

			if (getBallonResult.IsError)
				await Toast.Make($"{getBallonResult.FirstError.Description}", ToastDuration.Long, 16).Show();

			var ballon = getBallonResult.Value;

			if (ballon is not null)
			{
				Ballon = ballon;			
				await Task.Delay(500);
				OpenBallonForm();
			}
			else
			{
				Ballon = new BallonVM();
				await Task.Delay(500);
				CloseBallonForm();
			}

			Flyout.ToggleFlyout();
		});
	}

	private void OnBackButtonClickHandler()
	{
		Flyout.ToggleFlyout();
	}

	private void ButtonSerialNumberSearch_Clicked(object sender, EventArgs e)
	{
		_isPopupNavigation = true;
		FlyoutSerialNumberInput.ToggleFlyout();
	}

	private async void ButtonSerialNumberSearchFlyout_Clicked()
	{
		string number = SerialNumberInputView.SerialNumber?.ToLowerInvariant();
		if (string.IsNullOrWhiteSpace(number))
		{ 
			await Toast.Make("Некорректный серийный номер", ToastDuration.Long, 16).Show(); 
		}
		else
		{
			var popup = new BallonsListPage(number, _ballonService);
			popup.ItemSelected += async (e, selectedItem) =>
			{
				Dispatcher.Dispatch(async () =>
				{
					if (selectedItem is BallonVM ballon)
					{
						Ballon = ballon;
						await Task.Delay(500);
						OpenBallonForm();
					}
					else
					{
						Ballon = new BallonVM();
						await Task.Delay(500);
						CloseBallonForm();
					}
				});

				if (Navigation.ModalStack.Count > 0)
					await Navigation.PopModalAsync(true);
			};

			await Navigation.PushModalAsync(popup, true);
		}

		await FlyoutSerialNumberInput.CloseFlyout();
	}

	private void OnNFCTagClickHandler(object sender, EventArgs e)
	{
		Flyout.ToggleFlyout();

		nfcScanView.OnScanSuccessHandler = (string nfc) =>
		{
			Dispatcher.Dispatch(async () =>
			{
				NFCTag = nfcScanView.Nfc;
				Flyout.ToggleFlyout();
			});
		};
	}

	private async void ButtonCloseForm_Clicked(object sender, EventArgs e)
	{
		CloseBallonForm();
		await Task.Delay(700);
		Ballon = new();
	}
}