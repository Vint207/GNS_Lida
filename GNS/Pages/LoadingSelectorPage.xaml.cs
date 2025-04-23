using GNS.Pages.Popup;
using Services;
using Services.Intrefaces;
using static Services.Intrefaces.IBatchService;

namespace GNS.Pages;

[QueryProperty(nameof(PageTitle), "PageTitle")]
public partial class LoadingSelectorPage : ContentPage
{
	#region Свойства
	private bool _isPopupNavigation;

	private DateOnly _batchBeginDate;
	public DateOnly BatchBeginDate
	{
		get => _batchBeginDate;
		set 
		{
			_batchBeginDate = value;
			OnPropertyChanged(nameof(BatchBeginDate));
		}
	}

	private TimeOnly _batchBeginTime;
	public TimeOnly BatchBeginTime
	{
		get => _batchBeginTime;
		set
		{
			_batchBeginTime = value;
			OnPropertyChanged(nameof(BatchBeginTime));
		}
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
	public string PageTitle
	{ 
		get => Title;
		set
		{
			Title = value;
			OnPropertyChanged(nameof(PageTitle));
		}
	}
	private bool _batchSelectionSuccess;

	private IBatchService _batchService;
	private IBallonService _ballonService;
	#endregion

	public LoadingSelectorPage(IBatchService batchService, IBallonService ballonService)
	{
		InitializeComponent();

		BindingContext = this;
		_batchService = batchService;
		_ballonService = ballonService;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
	}

	private void Button_select_active_loading_batces_Clicked(object sender, EventArgs e)
	{
		 ShowBatchCollectionPopup("Приемка");
	}

	private async void ShowBatchCollectionPopup(string batchType)
	{
		var popup = new BatchListPopup(batchType, _batchService);

		popup.ItemSelected += async (e, selectedItem) =>
		{
			if (selectedItem is BatchListVM batch)
			{
				_batchBeginDate = batch.BeginDate ?? default;
				_batchBeginTime = batch.BeginTime ?? default;
				_batchId = batch.Id ?? default;
				_batchSelectionSuccess = true;			
			}

			if (Navigation.ModalStack.Count > 0)
				await Navigation.PopModalAsync(true);

			if (_batchSelectionSuccess)
			{				
				var loadingBatchEditPopup = new LoadingPage(_batchService, _ballonService) { Title = batchType};

				loadingBatchEditPopup.PopupOpenActionHandler += () =>
				{
					loadingBatchEditPopup.BatchModel = new BatchVM { Id = _batchId };
					loadingBatchEditPopup.BallonFormView.StackStopLoading.IsVisible = true;
					loadingBatchEditPopup.BallonFormView.ButtonStopLoading.Text = batchType.Equals("Приемка") ? "Закончить приемку" : "Закончить отгрузку";
					loadingBatchEditPopup.StartReadingScannedBallonsAmount();
				};

				loadingBatchEditPopup.PopupCloseActionHandler += async () =>
				{
					if (Navigation.ModalStack.Count > 0)
						await Shell.Current.Navigation.PopModalAsync(true);
				};

				await Shell.Current.Navigation.PushModalAsync(loadingBatchEditPopup, true);
			}

			_batchSelectionSuccess = false;
		};

		await Navigation.PushModalAsync(popup, true);
	}

	protected override bool OnBackButtonPressed()
	{
		Dispatcher.Dispatch (async () => 
		{
			await Shell.Current.GoToAsync($"..", true);		
		});
		return true;
	}

	private async void ButtonStartNewLoading_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync($"{nameof(LoadingPage)}?Title=Приемка", true);
	}
}