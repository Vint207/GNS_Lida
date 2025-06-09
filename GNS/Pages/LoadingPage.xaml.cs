using CommunityToolkit.Maui.Alerts;
using GNS.Pages.Popup;
using GNS.Pages.Templates;
using Services;
using Services.Intrefaces;
using System.ComponentModel;
using static Services.Intrefaces.IBatchService;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using ErrorOr;

namespace GNS.Pages;

[QueryProperty(nameof(Title), "Title")]
public partial class LoadingPage : ContentPage, INotifyPropertyChanged
{

	public LoadingPage(IBatchService batchService, IBallonService ballonService)
	{
		InitializeComponent();

		_batchService = batchService;
		_ballonService = ballonService;

		ballonLoadingView.LabelUnscannedballons5.Text = "0";
		ballonLoadingView.LabelUnscannedballons12.Text = "0";
		ballonLoadingView.LabelUnscannedballons27.Text = "0";
		ballonLoadingView.LabelUnscannedballons50.Text = "0";
		ballonLoadingView.LabelScannedBallonsAmount.Text = "0";

		//nfcScanView.OnScanSuccessHandler += OnScanSuccessHandler;
		nfcScanView.OnBackButtonClickHandler += OnBackButtonClickHandler;
		Flyout.OnFlyoutOpenHandler += nfcScanView.OnAppearing;
		Flyout.OnFlyoutCloseHandler += nfcScanView.OnDisappearing;
	}

	#region Ñâîéñòâà
	private bool _isPopupNavigation;
	private int? _carId;
	private int? _trailerId;
	private int? _readerId;

	private string _ttn;
	public string TTN
	{
		get => _ttn;
		set
		{
			_ttn = value;
			OnPropertyChanged();
		}
	}
	private int? _amountOfTtn;
	public int? AmountOfTtn
	{
		get => _amountOfTtn;
		set
		{
			_amountOfTtn = value;
			OnPropertyChanged();
		}
	}

	private BatchVM _batchModel;
	public BatchVM BatchModel
	{
		get => _batchModel;
		set => _batchModel = value;
	}

	private int _batchId;
	public int BatchId
	{
		get => _batchId;
		set
		{
			_batchId = value;
			OnPropertyChanged(nameof(BatchId));
		}
	}
	public BallonLoadingView BallonFormView => ballonLoadingView;

	public Action PopupOpenActionHandler;
	public Action PopupCloseActionHandler;

	private enum Mode { AddBallon, DeleteBallon }
	private Mode _mode;

	private bool _timerWorkPermission;
	private IBatchService _batchService;
	private IBallonService _ballonService;

	private string _startLoadingURl;
	private string _startLoadingMessage;
	private string _stopLoadingURl;
	private string _stopLoadingMessage;
	private ReaderType _readerType;
	private bool _batchSelectionSuccess;
	#endregion

	public void ConfigurePage()
	{
		if (Title.Equals("Отгрузка"))
		{
			ballonLoadingView.ButtonStartLoading.Text = "Начать отгрузку";
			_startLoadingURl = "balloons-unloading";
			_startLoadingMessage = "Отгрузка начата";
			_stopLoadingURl = "balloons-unloading";
			_stopLoadingMessage = "Отгрузка закончена";
			_readerType = ReaderType.Unloading;
		}
		else
		{
			ballonLoadingView.ButtonStartLoading.Text = "Начать приемку";
			_startLoadingURl = "balloons-loading";
			_startLoadingMessage = "Приемка начата";
			_stopLoadingURl = "balloons-loading";
			_stopLoadingMessage = "Приемка закончена";
			_readerType = ReaderType.Loading;
		}
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();

		PopupOpenActionHandler?.Invoke();
		_timerWorkPermission = true;
		ConfigurePage();
	}

	protected async override void OnDisappearing()
	{
		base.OnDisappearing();

		await Flyout.CloseFlyout();
		await FlyoutSerialNumberInput.CloseFlyout();

		if (!_isPopupNavigation)
		{
			ballonLoadingView.CarDataFrame.IsVisible = false;
			ballonLoadingView.ReaderDataFrame.IsVisible = false;
		}

		_isPopupNavigation = false;
		_timerWorkPermission = false;
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
				if (ballonLoadingView.StackStopLoading.IsVisible)
				{
					base.OnBackButtonPressed();
				}
				else
				{
					if (Title.Equals("Приемка"))
					{
						await Shell.Current.GoToAsync($"//{nameof(LoadingSelectorPage)}?PageTitle=Приемка", true);
					}
					else
					{
						await Shell.Current.GoToAsync($"//{nameof(UnloadingSelectorPage)}?PageTitle=Отгрузка", true);
					}
				}
			}
		});

		return true;
	}


	private async void Button_SelectCar_Clicked(object sender, EventArgs e)
	{
		var popup = new CarCollectionPopup(_batchService);
		_isPopupNavigation = true;

		popup.ItemSelected += async (e, selectedItem) =>
		{
			if (selectedItem is CarVM car)
			{
				ballonLoadingView.CarDataFrame.IsVisible = true;

				Dispatcher.Dispatch(() =>
				{
					ballonLoadingView.CarNumber = car.Number;
					ballonLoadingView.CarBrand = car.Brand;
					ballonLoadingView.CarType = car.Type;
					_carId = car.Id;
				});
			}

			if (Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		};

		popup.ItemsLoadingFailed += async (e, selectedItem) =>
		{
			if(Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		};

		_isPopupNavigation = true;
		await Navigation.PushModalAsync(popup, true);
	}

	private async void Button_SelectReader_Clicked(object sender, EventArgs e)
	{
		var popup = new StringOptionsPage();

		_isPopupNavigation = true;
		await Navigation.PushModalAsync(popup, true);

		ErrorOr<IEnumerable<int>> getReadersResult;

		if (_readerType is ReaderType.Loading)
			getReadersResult = await _batchService.GetStationLoadingReaders();
		else
			getReadersResult = await _batchService.GetStationUnloadingReaders();

		if (getReadersResult.IsError)
		{
			await Toast.Make($"{getReadersResult.FirstError.Description}",ToastDuration.Long,16).Show();

			if (Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);

			return;
		}

		var options = getReadersResult.Value?.Select(x => x.ToString());
		if (options is not null && options.Any())
		{
			popup.Options = options.ToObservableCollection();
		}
		else
		{
			if (Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		}

		popup.ItemSelected += async (e, selectedItem) =>
		{
			if (selectedItem is not null)
			{
				ballonLoadingView.ReaderDataFrame.IsVisible = true;

				Dispatcher.Dispatch(() =>
				{
					ballonLoadingView.ReaderType = Int32.Parse(selectedItem.ToString());
					_readerId = ballonLoadingView.ReaderType;
				});
			}

			if (Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);
		};
	}

	private async void Button_StartLoading_Clicked(object sender, EventArgs e)
	{
		if (!ballonLoadingView.CarDataFrame.IsVisible || !ballonLoadingView.ReaderDataFrame.IsVisible)
		{
			await Toast.Make($"Выберите машину и считыватель", ToastDuration.Short, 16).Show();
			return;
		}

		if (string.IsNullOrWhiteSpace(ballonLoadingView.TTN) || !ballonLoadingView.AmountOfTtn.HasValue)
		{
			await Toast.Make($"Введите количество и номер ТТН", ToastDuration.Short, 16).Show();
			return;
		}

		_isPopupNavigation = true;

		var startLoadingRequest = new StartLoadingRequest(
			_carId ?? default,
			_readerId ?? default,
			true,
			ballonLoadingView.TTN ?? "",
			ballonLoadingView.AmountOfTtn ?? 0);

		var startLoadingResult = await _batchService
			.StartLoading(startLoadingRequest, $"{_startLoadingURl}/", $"{_startLoadingMessage}");

		if (startLoadingResult.IsError)
		{
			string message = $"Примка не начата. {startLoadingResult.FirstError.Description}";
			await Toast.Make(message, ToastDuration.Long, 16).Show();
			return;
		}

		if (startLoadingResult.Value is BatchVM batchModel)
		{
			_batchModel = batchModel;
			var loadingPopupPage = new LoadingPage(_batchService, _ballonService)
			{
				Title = this.Title,
				TTN = ballonLoadingView.TTN ?? "",
				AmountOfTtn = ballonLoadingView.AmountOfTtn ?? 0
			};

			loadingPopupPage.PopupOpenActionHandler += async () =>
			{
				loadingPopupPage.BatchModel = _batchModel;
				loadingPopupPage.BallonFormView.StackStopLoading.IsVisible = true;
				loadingPopupPage.BallonFormView.CarTrailerReadderSelectionBlock.IsVisible = false;

				string title = Title.Equals("Приемка") ? "Приемка" : "Отгрузка";
				loadingPopupPage.ballonLoadingView.ButtonStopLoading.Text = title;
				loadingPopupPage.StartReadingScannedBallonsAmount();

				string message = Title.Equals("Приемка") ? "Приемка начата" : "Приемка начата";
				await Toast.Make(message, ToastDuration.Long, 16).Show();
			};

			loadingPopupPage.PopupCloseActionHandler += async () =>
			{
				if (Navigation.ModalStack.Count > 0)
					await Shell.Current.Navigation.PopModalAsync(true);
			};

			await Shell.Current.Navigation.PushModalAsync(loadingPopupPage, true);
		}
		else
		{
			await Toast.Make("Партия не начата", ToastDuration.Long, 16).Show();
			return;
		}
	}

	private async void Button_StopLoading_Clicked(object sender, EventArgs e)
	{
		if (_batchModel.Id is null)
		{
			await Toast.Make("Партия не найдена", ToastDuration.Long, 16).Show();
			return;
		}

		if (!int.TryParse(ballonLoadingView.LabelUnscannedballons5.Text, out int a5) ||
			!int.TryParse(ballonLoadingView.LabelUnscannedballons12.Text, out int a12) ||
			!int.TryParse(ballonLoadingView.LabelUnscannedballons27.Text, out int a27) ||
			!int.TryParse(ballonLoadingView.LabelUnscannedballons50.Text, out int a50))
		{
			await Toast.Make($"Неверное количество баллонов", ToastDuration.Long, 16).Show();
			return;
		}

		var stopLoadingRequest = new StopLoadingRequest(
			a5, a12, a27, a50, false, TTN ?? _batchModel.TTN ?? "0", AmountOfTtn ?? _batchModel.AmountOfTTN ?? 0);

		var stopLoadingResult = await _batchService.StopLoading(
			stopLoadingRequest,
			$"{_stopLoadingURl}/{_batchModel.Id.Value}/",
			$"{_stopLoadingMessage}");

		if (stopLoadingResult.IsError)
		{
			await Toast.Make($"{stopLoadingResult.FirstError.Description}", ToastDuration.Long, 16).Show();
			return;
		}

		string message = Title.Equals("Приемка") ? "Приемка закончена" : "Отгрузка закончена";
		await Toast.Make(message, ToastDuration.Long, 16).Show();

		ballonLoadingView.StackStopLoading.IsVisible = false;
		PopupCloseActionHandler?.Invoke();
	}

	private void Button_Add_Ballon_To_Batch_Clicked(object sender, EventArgs e)
	{
		_mode = Mode.AddBallon;
		nfcScanView.OnScanSuccessHandler = OnScanSuccessHandler;
		Flyout.ToggleFlyout();
	}

	private void Button_Delete_Ballon_From_Batch_Clicked(object sender, EventArgs e)
	{
		_mode = Mode.DeleteBallon;
		nfcScanView.OnScanSuccessHandler = OnScanSuccessHandler;
		Flyout.ToggleFlyout();
	}

	private void OnScanSuccessHandler(string nfc)
	{
		Dispatcher.Dispatch(async () =>
		{
			var getBallonResult = await _ballonService.GetBallonByNfc(nfc.ToLowerInvariant());

			if (getBallonResult.IsError)
			{
				await Toast.Make($"{getBallonResult.FirstError.Description}",ToastDuration.Long,16).Show();
				return;
			}

			var ballon = getBallonResult.Value;

			if (ballon is null || _batchModel.Id is null)
			{
				await Toast.Make(ballon is null ? "Баллон не отсканирован" : "Баллон отсканирован", ToastDuration.Short,16).Show();
				return;
			}

			switch (_mode)
			{
				case Mode.AddBallon:
					var addBallonResult = await _batchService
						.AddBallonToBatch(ballon.Id.Value, _batchModel.Id.Value, Title);

					if (addBallonResult.IsError)
					{
						await Toast.Make($"Баллон не добавлен. {addBallonResult.FirstError.Description}",ToastDuration.Long,16).Show();
					}
					else
					{
						await Toast.Make($"Баллон добавлен",ToastDuration.Long,16).Show();
					}
					break;
				case Mode.DeleteBallon:
					var deleteBallonResult = await _batchService
						.DeleteBallonFromBatch(ballon.Id.Value, _batchModel.Id.Value, Title);

					if (deleteBallonResult.IsError)
					{
						await Toast.Make($"Баллон не удален. {deleteBallonResult.FirstError.Description}",ToastDuration.Long,16).Show();
					}
					else
					{
						await Toast.Make($"Баллон удален",ToastDuration.Long,16).Show();
					}
					break;
			}
		});
	}

	private void OnBackButtonClickHandler()
	{
		Flyout.ToggleFlyout();
	}

	private async void GetScannedBallonsAmount()
	{
		var getAmountResult = await _batchService.GetScannedBallonsAmount(_batchModel.Id.Value, Title);

		if (getAmountResult.IsError)
			return;

		var amount = getAmountResult.Value.Amount?.ToString();

		if (string.IsNullOrWhiteSpace(amount))
			return;

		ballonLoadingView.LabelScannedBallonsAmount.Text = amount;
	}

	public void StartReadingScannedBallonsAmount()
	{
		Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
		{
			GetScannedBallonsAmount();
			return _timerWorkPermission;
		});
	}
}