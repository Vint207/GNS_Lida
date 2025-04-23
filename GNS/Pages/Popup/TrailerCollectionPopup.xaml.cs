using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Services;
using Services.Intrefaces;
using System.Collections.ObjectModel;
using static Services.Intrefaces.IBatchService;

namespace GNS.Pages.Popup;

public partial class TrailerCollectionPopup : ContentPage
{
	private ObservableCollection<TrailerVM> _stationTrailers;
	public ObservableCollection<TrailerVM> StationTrailers
	{
		get => _stationTrailers;
		set
		{
			_stationTrailers = value;
			OnPropertyChanged(nameof(StationTrailers));
		}
	}

	public event EventHandler<object> ItemSelected;
	private IBatchService _batchService;

	public TrailerCollectionPopup(IBatchService batchService)
	{
		InitializeComponent();

		_batchService = batchService;
		BindingContext = this;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadTrailerListAsync();
	}

	private async Task LoadTrailerListAsync()
	{
		StationTrailers ??= [];

		Dispatcher.Dispatch(async () =>
		{
			StationTrailers?.Clear();
			var getTrailersResult = await _batchService.GetStationTrailers();

			if (getTrailersResult.IsError)
			{
				await Toast.Make($"{getTrailersResult.FirstError.Description}", ToastDuration.Long, 16).Show();
				return;
			}

			var trailers = getTrailersResult.Value;

			if (trailers is null || !trailers.Any()) 
			{
				if (Navigation.ModalStack.Count > 0)
					await Navigation.PopModalAsync();

				return;
			}

			foreach (var trailer in trailers)
				StationTrailers?.Add(trailer);			
		});
	}

	private async void TrailerCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		ItemSelected?.Invoke(e, e.CurrentSelection[0]);
	}
}