using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Services;
using Services.Intrefaces;
using System.Collections.ObjectModel;
using static Services.Intrefaces.IBatchService;


namespace GNS.Pages.Popup;

public partial class BatchListPopup : ContentPage
{
	private ObservableCollection<BatchVM> _batchModelList;
	public ObservableCollection<BatchVM> BatchModelList
	{
		get => _batchModelList;
		set
		{
			_batchModelList = value;
			OnPropertyChanged(nameof(BatchModelList));
		}
	}

	public event EventHandler<object> ItemSelected;

	private readonly string _batchListType;
	private IBatchService _batchService;

	public BatchListPopup(string batchListType, IBatchService batchService)
	{
		InitializeComponent();

		BindingContext = this;
		_batchListType = batchListType;
		_batchService = batchService;
		LoadBatchListAsync();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
	}

	private async void LoadBatchListAsync()
	{
		try
		{
			BatchModelList ??= [];

			Dispatcher.Dispatch(async () =>
			{
				BatchModelList?.Clear();

				var getActiveBatchResult = await _batchService.GetActiveBatchList(_batchListType);

				if (getActiveBatchResult.IsError)
				{
					await Toast.Make($"{getActiveBatchResult.FirstError.Description}", ToastDuration.Long, 16).Show();
					return;
				}

				var batches = getActiveBatchResult.Value;

				if (batches is null || !batches.Any())
				{
					if (Navigation.ModalStack.Count > 0)
						await Navigation.PopModalAsync();

					return;
				}

				foreach (var batch in batches)
				{
					BatchModelList?.Add(batch);
				}
			});
		}
		catch (Exception)
		{

			throw;
		}
	}

	private void Batch_collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ItemSelected?.Invoke(e, e.CurrentSelection[0]);
	}
}